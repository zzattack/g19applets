using LgLcd;
using PioneerAvrControlLib.Message;
using PioneerAvrControlLib;
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PioneerApplet {
	class Applet : WinFormsApplet {
		public Applet() {
			InitializeComponent();
			device.SetAsLCDForegroundApp(true);

			if (!IsHandleCreated) CreateHandle();
			hotkey = new KeyboardHook();
			hotkey.RegisterHotKey(KeyModifiers.None, Keys.VolumeUp);
			hotkey.RegisterHotKey(KeyModifiers.None, Keys.VolumeDown);
			hotkey.KeyPressed += new EventHandler<KeyPressedEventArgs>(hotkey_KeyPressed);

			device.Up += new EventHandler(device_Up);
			device.Down += new EventHandler(device_Down);
			device.Left += new EventHandler(device_Left);
			device.Right += new EventHandler(device_Right);
			device.Menu += new EventHandler(device_Menu);

			currentTheme = themer.GetTheme();
			ApplyTheme();
		}

		PioneerTCPConnection conn = new PioneerAvrControlLib.PioneerTCPConnection("10.31.45.25");
		KeyboardHook hotkey;
		InputType lastInputType;
		private Label lblLine3;
		Dictionary<string, string> TunerPresetNames = new Dictionary<string, string>();
		FormThemer themer = new FormThemer();
		Theme currentTheme;

		void hotkey_KeyPressed(object sender, KeyPressedEventArgs e) {
			if (e.Key == Keys.VolumeDown) {
				conn.SendMessage(new VolumeDown());
			}
			else if (e.Key == Keys.VolumeUp) {
				conn.SendMessage(new VolumeUp());
			}
		}
		void device_Up(object sender, EventArgs e) {
			conn.SendMessage(new InputTypeChangePrevious());
		}
		void device_Down(object sender, EventArgs e) {
			conn.SendMessage(new InputTypeChangeNext());
		}
		void device_Left(object sender, EventArgs e) {
			if (lastInputType == InputType.Tuner)
				conn.SendMessage(new TunerPresetDecrement());
		}
		void device_Right(object sender, EventArgs e) {
			if (lastInputType == InputType.Tuner)
				conn.SendMessage(new TunerPresetIncrement());
		}
		void device_Menu(object sender, EventArgs e) {
			currentTheme = themer.GetTheme();
			BeginInvoke(new MethodInvoker(ApplyTheme));
		}

		private void ApplyTheme() {
			this.BackgroundImage = currentTheme.Background;
			foreach (Control c in this.Controls) {
				if (c is Label) {
					c.ForeColor = currentTheme.ForeColor;
					c.BackColor = currentTheme.BackColor;
				}
			}
			UpdateLcdScreen(this, EventArgs.Empty);
		}


		public override void OnDeviceArrival(DeviceType deviceType) {
			conn.Open();
			conn.MessageReceived += new EventHandler<MessageReceivedEventArgs>(conn_MessageReceived);

			// obtain current status
			conn.SendMessage(new InputTypeRequest());
			conn.SendMessage(new VolumeRequest());
			conn.SendMessage(new BassRequest());
			conn.SendMessage(new TrebleRequest());
			conn.SendMessage(new ToneRequest());
			conn.SendMessage(new PlayingListeningModeRequest());
		}

		void conn_MessageReceived(object sender, PioneerAvrControlLib.MessageReceivedEventArgs e) {
			if (InvokeRequired) {
				this.BeginInvoke(new EventHandler<MessageReceivedEventArgs>(conn_MessageReceived), sender, e);
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
				this.lblBass.Text = "Bass: " + (msg.BassLevel > 0 ? "+" : "") + msg.BassLevel.ToString() + "dB";
			}
			else if (e.message.GetType() == typeof(TrebleResponse)) {
				var msg = e.message as TrebleResponse;
				this.lblTreble.Text = "Treble: " + (msg.TrebleLevel > 0 ? "+" : "") + msg.TrebleLevel.ToString() + "dB";
			}
			else if (e.message.GetType() == typeof(InputTypeResponse)) {
				var msg = e.message as InputTypeResponse;
				lastInputType = msg.Input;
				lblLine1.Text = msg.Input.ToFriendlyName();
				lblLine2.Text = "";

				if (msg.Input == InputType.Tuner) {
					// get frequency preset, if any
					conn.SendMessage(new TunerPresetNamesRequest());
					conn.SendMessage(new TunerPresetRequest());
				}					
			}
			else if (e.message.GetType() == typeof(TunerPresetNamesResponse)) {
				var msg = e.message as TunerPresetNamesResponse;
				TunerPresetNames[msg.Preset] = msg.Name;
			}
			else if (e.message.GetType() == typeof(TunerPresetResponse)) {
				var msg = e.message as TunerPresetResponse;
				string preset = msg.Class.ToString() + msg.Number.ToString();
				if (TunerPresetNames.ContainsKey(preset)) {
					this.lblLine2.Text = TunerPresetNames[preset];
				}
			}
			else if (e.message.GetType() == typeof(PlayingListeningModeResponse)) {
				var msg = e.message as PlayingListeningModeResponse;
				this.lblLine3.Text = msg.PlayListeningMode;
			}
			UpdateLcdScreen(this, EventArgs.Empty);
		}

		public override event System.EventHandler UpdateLcdScreen;
		public override string AppletName {
			get { return "Pioneer Control"; }
		}

		private System.Windows.Forms.Label lblDisplay;
		private System.Windows.Forms.ProgressBar pbVolume;
		private System.Windows.Forms.Label lblLine2;
		private System.Windows.Forms.Label lblLine1;
		private System.Windows.Forms.Label lblBass;
		private System.Windows.Forms.Label lblTreble;
		private void InitializeComponent() {
			this.lblDisplay = new System.Windows.Forms.Label();
			this.pbVolume = new System.Windows.Forms.ProgressBar();
			this.lblLine2 = new System.Windows.Forms.Label();
			this.lblLine1 = new System.Windows.Forms.Label();
			this.lblBass = new System.Windows.Forms.Label();
			this.lblTreble = new System.Windows.Forms.Label();
			this.lblLine3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblDisplay
			// 
			this.lblDisplay.BackColor = System.Drawing.Color.Transparent;
			this.lblDisplay.Font = new System.Drawing.Font("Lucida Console", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDisplay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblDisplay.Location = new System.Drawing.Point(0, 130);
			this.lblDisplay.Name = "lblDisplay";
			this.lblDisplay.Size = new System.Drawing.Size(320, 35);
			this.lblDisplay.TabIndex = 0;
			// 
			// pbVolume
			// 
			this.pbVolume.Location = new System.Drawing.Point(10, 188);
			this.pbVolume.Maximum = 185;
			this.pbVolume.Name = "pbVolume";
			this.pbVolume.Size = new System.Drawing.Size(300, 23);
			this.pbVolume.TabIndex = 1;
			// 
			// lblLine2
			// 
			this.lblLine2.BackColor = System.Drawing.Color.Transparent;
			this.lblLine2.Font = new System.Drawing.Font("Lucida Console", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblLine2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblLine2.Location = new System.Drawing.Point(0, 47);
			this.lblLine2.Name = "lblLine2";
			this.lblLine2.Size = new System.Drawing.Size(320, 35);
			this.lblLine2.TabIndex = 2;
			this.lblLine2.Text = "-----------------";
			// 
			// lblLine1
			// 
			this.lblLine1.BackColor = System.Drawing.Color.Transparent;
			this.lblLine1.Font = new System.Drawing.Font("Lucida Console", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblLine1.Location = new System.Drawing.Point(0, 12);
			this.lblLine1.Name = "lblLine1";
			this.lblLine1.Size = new System.Drawing.Size(320, 35);
			this.lblLine1.TabIndex = 3;
			this.lblLine1.Text = "-----------------";
			// 
			// lblBass
			// 
			this.lblBass.AutoSize = true;
			this.lblBass.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblBass.Location = new System.Drawing.Point(7, 214);
			this.lblBass.Name = "lblBass";
			this.lblBass.Size = new System.Drawing.Size(45, 16);
			this.lblBass.TabIndex = 4;
			this.lblBass.Text = "Bass: ";
			// 
			// lblTreble
			// 
			this.lblTreble.AutoSize = true;
			this.lblTreble.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTreble.Location = new System.Drawing.Point(82, 214);
			this.lblTreble.Name = "lblTreble";
			this.lblTreble.Size = new System.Drawing.Size(54, 16);
			this.lblTreble.TabIndex = 5;
			this.lblTreble.Text = "Treble: ";
			// 
			// lblLine3
			// 
			this.lblLine3.BackColor = System.Drawing.Color.Transparent;
			this.lblLine3.Font = new System.Drawing.Font("Lucida Console", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblLine3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblLine3.Location = new System.Drawing.Point(0, 82);
			this.lblLine3.Name = "lblLine3";
			this.lblLine3.Size = new System.Drawing.Size(320, 35);
			this.lblLine3.TabIndex = 6;
			this.lblLine3.Text = "-----------------";
			// 
			// Applet
			// 
			this.Controls.Add(this.lblLine3);
			this.Controls.Add(this.lblTreble);
			this.Controls.Add(this.lblBass);
			this.Controls.Add(this.lblLine1);
			this.Controls.Add(this.lblLine2);
			this.Controls.Add(this.pbVolume);
			this.Controls.Add(this.lblDisplay);
			this.DoubleBuffered = true;
			this.Name = "Applet";
			this.Size = new System.Drawing.Size(320, 240);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}