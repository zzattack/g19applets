using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using PioneerAvrControlLib;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Threading.Timer;

namespace PioneerApplet {
	public sealed partial class MainForm : Form {

		public MainForm() {
			InitializeComponent();

			_conn = new PioneerConnection(new List<WritableDataSource> {
					new TcpClientDataSource("10.31.45.25", 8102),
					new TcpClientDataSource("10.31.45.25", 23),
				});
			_conn.ConnectionEstablished += (sender, args) => ReadCurrentSettings();
			_conn.MessageReceived += MessageReceived;
			_conn.Start();

			if (!IsHandleCreated) CreateHandle();
			_hotkey = new PioneerHooks();
			_hotkey.RegisterHotKey(KeyModifiers.None, Keys.VolumeUp);
			_hotkey.RegisterHotKey(KeyModifiers.None, Keys.VolumeDown);
			_hotkey.RegisterHotKey(KeyModifiers.Control, Keys.VolumeUp);
			_hotkey.RegisterHotKey(KeyModifiers.Control, Keys.VolumeDown);
			_hotkey.RegisterHotKey(KeyModifiers.Control | KeyModifiers.Alt, Keys.VolumeUp);
			_hotkey.RegisterHotKey(KeyModifiers.Control | KeyModifiers.Alt, Keys.VolumeDown);
			_hotkey.RegisterHotKey(KeyModifiers.Control | KeyModifiers.Shift, Keys.P);
			_hotkey.RegisterHotKey(KeyModifiers.Control | KeyModifiers.Shift, Keys.Q);
			_hotkey.KeyPressed += HotkeyKeyPressed;

			/*Device.Up += DeviceUp;
			Device.Down += DeviceDown;
			Device.Left += DeviceLeft;
			Device.Right += DeviceRight;
			Device.Menu += DeviceMenu;
			Device.Ok += DeviceOk;*/

			_currentTheme = _themer.GetTheme();
			ApplyTheme();

			_switchableModes.Add(0x0009); // STEREO 
			_switchableModes.Add(0x0112); // EXTENDED STEREO
			_switchableModes.Add(0x0109); // UNPLUGGED
			_switchableModes.Add(0x0118); // ADVANCED GAME

			// use a delayed timer in case the DeviceArrival event doesn't come up
			this._t = new Timer(delegate {
				_conn.Start();
				this._t.Dispose();
				this._t = null;
			}, null, 3000000, -1);
			ShowMe();
		}

		private Timer _t;
		readonly PioneerConnection _conn;
		readonly PioneerHooks _hotkey;
		InputType _lastInputType;
		private Label _lblLine3;
		readonly Dictionary<string, string> _tunerPresetNames = new Dictionary<string, string>();
		readonly FormThemer _themer = new FormThemer();
		Theme _currentTheme;
		int _currentMode; // current listening mode
		readonly List<int> _switchableModes = new List<int>();

		void HotkeyKeyPressed(object sender, KeyPressedEventArgs e) {
			if (e.Modifier == KeyModifiers.None) {
				// receiver zone 1
				if (e.Key == Keys.VolumeDown) _conn.SendMessage(new VolumeDown());
				else if (e.Key == Keys.VolumeUp) _conn.SendMessage(new VolumeUp());
				ShowMe();
			}
			else if (e.Modifier == KeyModifiers.Control) {
				// system volume
				const int APPCOMMAND_VOLUME_UP = 0xA0000;
				const int APPCOMMAND_VOLUME_DOWN = 0x90000;
				const int WM_APPCOMMAND = 0x319;

				if (e.Key == Keys.VolumeDown) SendMessage(new HandleRef(this, Handle), WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_DOWN);
				else if (e.Key == Keys.VolumeUp) SendMessage(new HandleRef(this, Handle), WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
				ShowMe();
			}
			/*else if (e.Modifier == (KeyModifiers.Control | KeyModifiers.Alt)) {
				// receiver zone 2
				if (e.Key == Keys.VolumeDown) _conn.SendMessage(new Z2VolumeDown());
				else if (e.Key == Keys.VolumeUp) _conn.SendMessage(new Z2VolumeUp());
			}*/
			else if (e.Modifier == (KeyModifiers.Control | KeyModifiers.Shift)) {
				// send power toggle
				if (e.Key == Keys.P) {
					_conn.SendMessage(new PowerToggle());
					ShowMe();
				}
				else if (e.Key == Keys.Q) {
					_conn.SendMessage(new PowerOff());
					_conn.Dispose();
					Application.Exit();
				}
			}
		}

		private Timer _hideTimer;
		private void ShowMe() {
			this.Show();
			SetForegroundWindow(Handle);

			if (_hideTimer != null) {
				_hideTimer.Change(TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan);
			}
			else {
				_hideTimer = new Timer(state => HideMe(), null, TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan);
			}
		}

		private void HideMe() {
			if (InvokeRequired) Invoke((Action)Hide);
			else Hide();
		}


		void DeviceUp(object sender, EventArgs e) {
			_conn.SendMessage(new InputTypeChangePrevious());
			ShowMe();
		}
		void DeviceDown(object sender, EventArgs e) {
			_conn.SendMessage(new InputTypeChangeNext());
			ShowMe();
		}
		void DeviceLeft(object sender, EventArgs e) {
			if (_lastInputType == InputType.Tuner)
				_conn.SendMessage(new TunerPresetDecrement());
			ShowMe();
		}
		void DeviceRight(object sender, EventArgs e) {
			if (_lastInputType == InputType.Tuner)
				_conn.SendMessage(new TunerPresetIncrement());
			ShowMe();
		}
		void DeviceMenu(object sender, EventArgs e) {
			_currentTheme = _themer.GetTheme();
			BeginInvoke(new MethodInvoker(ApplyTheme));
		}
		void DeviceOk(object sender, EventArgs e) {
			// select next mode from list or first if unmatched
			int current = (_switchableModes.IndexOf(_currentMode) + 1) % _switchableModes.Count;
			_conn.SendMessage(new ListeningModeSet((ListeningMode)_switchableModes[current]));
		}

		private void ApplyTheme() {
			this.BackgroundImage = _currentTheme.Background;
			foreach (Label c in this.Controls.OfType<Label>()) {
				c.ForeColor = _currentTheme.ForeColor;
				c.BackColor = _currentTheme.BackColor;
			}
		}

		readonly List<Timer> _timerRefs = new List<Timer>();
		private void ReadCurrentSettings() {
			_timerRefs.Add(new Timer(delegate {
				_conn.SendMessage(new PowerOn());
			}, null, new TimeSpan(100000), new TimeSpan(-1)));

			// obtain current status
			_timerRefs.Add(new Timer(delegate {
				_conn.SendMessage(new InputTypeRequest());
			}, null, new TimeSpan(200000), new TimeSpan(-1)));

			_timerRefs.Add(new Timer(delegate {
				_conn.SendMessage(new PlayingListeningModeRequest());
			}, null, new TimeSpan(600000), new TimeSpan(-1)));

			_timerRefs.Add(new Timer(delegate {
				_conn.SendMessage(new DisplayInformationRequest());
			}, null, new TimeSpan(600000), new TimeSpan(-1)));

			_timerRefs.Add(new Timer(delegate {
				_conn.SendMessage(new VolumeRequest());
			}, null, new TimeSpan(400000), new TimeSpan(-1)));

			//conn.SendMessage(new BassRequest());
			//conn.SendMessage(new TrebleRequest());
		}

		void MessageReceived(object sender, MessageReceivedEventArgs e) {
			if (InvokeRequired) {
				this.BeginInvoke(new EventHandler<MessageReceivedEventArgs>(MessageReceived), sender, e);
				return;
			}

			if (e.message.GetType() == typeof(DisplayInformationResponse)) {
				var msg = e.message as DisplayInformationResponse;
				this.lblDisplay.Text = msg.DisplayMessage;
			}
			else if (e.message.GetType() == typeof(VolumeStatusResponse)) {
				var msg = e.message as VolumeStatusResponse;
				this.pbVolume.Value = msg.Volume;
			}
			else if (e.message.GetType() == typeof(BassResponse)) {
				var msg = e.message as BassResponse;
				//this.lblBass.Text = "Bass: " + (msg.BassLevel > 0 ? "+" : "") + msg.BassLevel.ToString() + "dB";
			}
			else if (e.message.GetType() == typeof(TrebleResponse)) {
				var msg = e.message as TrebleResponse;
				//this.lblTreble.Text = "Treble: " + (msg.TrebleLevel > 0 ? "+" : "") + msg.TrebleLevel.ToString() + "dB";
			}
			else if (e.message.GetType() == typeof(InputTypeResponse)) {
				var msg = e.message as InputTypeResponse;
				_lastInputType = msg.Input;
				lblLine1.Text = msg.Input.ToFriendlyName();
				lblLine2.Text = "";

				if (msg.Input == InputType.Tuner) {
					// get frequency preset, if any
					_conn.SendMessage(new TunerPresetNamesRequest());
					_conn.SendMessage(new TunerPresetRequest());
				}
			}
			else if (e.message.GetType() == typeof(TunerPresetNamesResponse)) {
				var msg = e.message as TunerPresetNamesResponse;
				_tunerPresetNames[msg.Preset] = msg.Name;
			}
			else if (e.message.GetType() == typeof(TunerPresetResponse)) {
				var msg = e.message as TunerPresetResponse;
				string preset = msg.Class + msg.Number.ToString(CultureInfo.InvariantCulture);
				if (_tunerPresetNames.ContainsKey(preset)) {
					this.lblLine2.Text = _tunerPresetNames[preset];
				}
			}
			else if (e.message.GetType() == typeof(ListeningModeResponse)) {
				var msg = e.message as ListeningModeResponse;
				_currentMode = (int)msg.ListeningMode;
				_lblLine3.Text = msg.ListeningModeString;
			}
		}


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern bool SetForegroundWindow(IntPtr hWnd);

	}
}
