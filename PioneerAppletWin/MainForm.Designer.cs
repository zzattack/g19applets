namespace PioneerApplet {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private System.Windows.Forms.Label lblDisplay;
		private System.Windows.Forms.ProgressBar pbVolume;
		private System.Windows.Forms.Label lblLine2;
		private System.Windows.Forms.Label lblLine1;
		private void InitializeComponent() {
			this.lblDisplay = new System.Windows.Forms.Label();
			this.pbVolume = new System.Windows.Forms.ProgressBar();
			this.lblLine2 = new System.Windows.Forms.Label();
			this.lblLine1 = new System.Windows.Forms.Label();
			this._lblLine3 = new System.Windows.Forms.Label();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnLeft = new System.Windows.Forms.Button();
			this.btnRight = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblDisplay
			// 
			this.lblDisplay.BackColor = System.Drawing.Color.Transparent;
			this.lblDisplay.Font = new System.Drawing.Font("Lucida Console", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDisplay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblDisplay.Location = new System.Drawing.Point(0, 117);
			this.lblDisplay.Name = "lblDisplay";
			this.lblDisplay.Size = new System.Drawing.Size(320, 35);
			this.lblDisplay.TabIndex = 0;
			// 
			// pbVolume
			// 
			this.pbVolume.Location = new System.Drawing.Point(6, 166);
			this.pbVolume.Maximum = 185;
			this.pbVolume.Name = "pbVolume";
			this.pbVolume.Size = new System.Drawing.Size(197, 23);
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
			// _lblLine3
			// 
			this._lblLine3.BackColor = System.Drawing.Color.Transparent;
			this._lblLine3.Font = new System.Drawing.Font("Lucida Console", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._lblLine3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this._lblLine3.Location = new System.Drawing.Point(0, 82);
			this._lblLine3.Name = "_lblLine3";
			this._lblLine3.Size = new System.Drawing.Size(320, 35);
			this._lblLine3.TabIndex = 6;
			this._lblLine3.Text = "-----------------";
			// 
			// btnUp
			// 
			this.btnUp.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnUp.Location = new System.Drawing.Point(258, 166);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(22, 22);
			this.btnUp.TabIndex = 7;
			this.btnUp.Text = "▲";
			this.btnUp.UseVisualStyleBackColor = true;
			this.btnUp.Click += new System.EventHandler(this.DeviceUp);
			// 
			// btnDown
			// 
			this.btnDown.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnDown.Location = new System.Drawing.Point(280, 166);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(22, 22);
			this.btnDown.TabIndex = 8;
			this.btnDown.Text = "▼";
			this.btnDown.UseVisualStyleBackColor = true;
			this.btnDown.Click += new System.EventHandler(this.DeviceDown);
			// 
			// btnLeft
			// 
			this.btnLeft.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnLeft.Location = new System.Drawing.Point(209, 166);
			this.btnLeft.Name = "btnLeft";
			this.btnLeft.Size = new System.Drawing.Size(22, 22);
			this.btnLeft.TabIndex = 9;
			this.btnLeft.Text = "◀";
			this.btnLeft.UseVisualStyleBackColor = true;
			this.btnLeft.Click += new System.EventHandler(this.DeviceLeft);
			// 
			// btnRight
			// 
			this.btnRight.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRight.Location = new System.Drawing.Point(231, 166);
			this.btnRight.Name = "btnRight";
			this.btnRight.Size = new System.Drawing.Size(22, 22);
			this.btnRight.TabIndex = 10;
			this.btnRight.Text = "▶";
			this.btnRight.UseVisualStyleBackColor = true;
			this.btnRight.Click += new System.EventHandler(this.DeviceRight);
			// 
			// MainForm
			// 
			this.ClientSize = new System.Drawing.Size(336, 201);
			this.Controls.Add(this.btnRight);
			this.Controls.Add(this.btnLeft);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this._lblLine3);
			this.Controls.Add(this.lblLine1);
			this.Controls.Add(this.lblLine2);
			this.Controls.Add(this.pbVolume);
			this.Controls.Add(this.lblDisplay);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Location = new System.Drawing.Point(10, 10);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.ResumeLayout(false);

		}

		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnLeft;
		private System.Windows.Forms.Button btnRight;
	}
}