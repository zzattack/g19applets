using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using LgLcd;

namespace PioneerApplet {
	static class Program {

		
		static Mutex _mutex = new Mutex(true, "{DFF73E5D-6A74-49CE-9BF5-1A1C49C0C4EC}");

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			if (_mutex.WaitOne(TimeSpan.Zero, true)) {
				Application.EnableVisualStyles();
				try {
					new MainForm();
					Application.Run();
				}
				catch {
				}
				finally {
					_mutex.ReleaseMutex();
				} 
				
			}
			else {
				PioneerHooks.BroadcastForegroundRequest();
			}
		}

	}
}
