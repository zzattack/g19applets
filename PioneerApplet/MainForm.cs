using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using LgLcd;
using PioneerAvrControlLib;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using SPAA05.Shared.DataSources;
using Timer = System.Threading.Timer;

namespace PioneerApplet {
	public sealed partial class MainForm : WinFormsApplet {

		public MainForm() {
			InitializeComponent();

			_conn = new PioneerConnection(new List<WritableDataSource> {
					new TcpClientDataSource("10.31.45.25", 8102),
					new TcpClientDataSource("10.31.45.25", 23),
				});
			_conn.ConnectionEstablished += (sender, args) => ReadCurrentSettings();

			base.InitializeApplet();

			Device.SetAsLCDForegroundApp(true);

			if (!IsHandleCreated) CreateHandle();
			_hotkey = new PioneerHooks(Device);
			_hotkey.RegisterHotKey(KeyModifiers.None, Keys.VolumeUp);
			_hotkey.RegisterHotKey(KeyModifiers.None, Keys.VolumeDown);
			_hotkey.RegisterHotKey(KeyModifiers.Control, Keys.VolumeUp);
			_hotkey.RegisterHotKey(KeyModifiers.Control, Keys.VolumeDown);
			_hotkey.RegisterHotKey(KeyModifiers.Control | KeyModifiers.Alt, Keys.VolumeUp);
			_hotkey.RegisterHotKey(KeyModifiers.Control | KeyModifiers.Alt, Keys.VolumeDown);
			_hotkey.KeyPressed += HotkeyKeyPressed;

			Device.Up += DeviceUp;
			Device.Down += DeviceDown;
			Device.Left += DeviceLeft;
			Device.Right += DeviceRight;
			Device.Menu += DeviceMenu;
			Device.Ok += DeviceOk;

			_currentTheme = _themer.GetTheme();
			ApplyTheme();

			_switchableModes.Add(0x0009); // STEREO 
			_switchableModes.Add(0x0112); // EXTENDED STEREO
			_switchableModes.Add(0x0109); // UNPLUGGED
			_switchableModes.Add(0x0118); // ADVANCED GAME
		}

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
			}
			else if (e.Modifier == KeyModifiers.Control) {
				// system volume
				const int APPCOMMAND_VOLUME_UP = 0xA0000;
				const int APPCOMMAND_VOLUME_DOWN = 0x90000;
				const int WM_APPCOMMAND = 0x319;

				if (e.Key == Keys.VolumeDown) SendMessage(new HandleRef(this, Handle), WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_DOWN);
				else if (e.Key == Keys.VolumeUp) SendMessage(new HandleRef(this, Handle), WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
			}
			else if (e.Modifier == (KeyModifiers.Control | KeyModifiers.Alt)) {
				// receiver zone 2
				if (e.Key == Keys.VolumeDown) _conn.SendMessage(new Z2VolumeDown());
				else if (e.Key == Keys.VolumeUp) _conn.SendMessage(new Z2VolumeUp());
			}
		}
		void DeviceUp(object sender, EventArgs e) {
			_conn.SendMessage(new InputTypeChangePrevious());
		}
		void DeviceDown(object sender, EventArgs e) {
			_conn.SendMessage(new InputTypeChangeNext());
		}
		void DeviceLeft(object sender, EventArgs e) {
			if (_lastInputType == InputType.Tuner)
				_conn.SendMessage(new TunerPresetDecrement());
		}
		void DeviceRight(object sender, EventArgs e) {
			if (_lastInputType == InputType.Tuner)
				_conn.SendMessage(new TunerPresetIncrement());
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
			UpdateLcdScreen(this, EventArgs.Empty);
		}

		readonly List<Timer> _timerRefs = new List<Timer>();
		public override void OnDeviceArrival(DeviceType deviceType) {
			_conn.MessageReceived += MessageReceived;
			_conn.Start();
		}

		private void ReadCurrentSettings() {
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

		void MessageReceived(object sender, PioneerAvrControlLib.MessageReceivedEventArgs e) {
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
				string preset = msg.Class.ToString() + msg.Number.ToString();
				if (_tunerPresetNames.ContainsKey(preset)) {
					this.lblLine2.Text = _tunerPresetNames[preset];
				}
			}
			else if (e.message.GetType() == typeof(ListeningModeResponse)) {
				var msg = e.message as ListeningModeResponse;
				_currentMode = (int)msg.ListeningMode;
				this._lblLine3.Text = msg.ListeningModeString;
			}
			UpdateLcdScreen(this, EventArgs.Empty);
		}

		public override event EventHandler UpdateLcdScreen;
		public override string AppletName {
			get { return "Pioneer Control"; }
		}

	}
}
