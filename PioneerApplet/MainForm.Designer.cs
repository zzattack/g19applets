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
		private System.Windows.Forms.Label lblBass;
		private System.Windows.Forms.Label lblTreble;
		private void InitializeComponent() {
			this.lblDisplay = new System.Windows.Forms.Label();
			this.pbVolume = new System.Windows.Forms.ProgressBar();
			this.lblLine2 = new System.Windows.Forms.Label();
			this.lblLine1 = new System.Windows.Forms.Label();
			this.lblBass = new System.Windows.Forms.Label();
			this.lblTreble = new System.Windows.Forms.Label();
			this._lblLine3 = new System.Windows.Forms.Label();
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
			this.lblBass.Visible = false;
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
			this.lblTreble.Visible = false;
			// 
			// lblLine3
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
			// Applet
			// 
			this.Controls.Add(this._lblLine3);
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