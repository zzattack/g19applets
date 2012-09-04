using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using LgLcd;

namespace PioneerApplet {

	/// <summary> Registers a window for some keyboard/broadcast hooks </summary>
	public class PioneerHooks : IDisposable {

		Device _device;
		HookListenerWindow _hookListenerWindow = new HookListenerWindow();
		int _currentId;

		public PioneerHooks(Device device) {
			_device = device;

			// register the event of the inner native window.
			_hookListenerWindow.KeyPressed += delegate(object sender, KeyPressedEventArgs args) {
				if (KeyPressed != null) KeyPressed(this, args);
			};
			_hookListenerWindow.SetForegroundApplet += delegate(object sender, EventArgs args) {
				_device.SetAsLCDForegroundApp(true);
			};
		}

		public event EventHandler<KeyPressedEventArgs> KeyPressed;

		#region interop crap
		public const int WM_HOTKEY = 0x0312;
		public const int HWND_BROADCAST = 0xFFFF;
		public static readonly int WM_LGLCD_PIONEER_SETFOREGROUND = RegisterWindowMessage("WM_LGLCD_PIONEER_SETFOREGROUND");

		[DllImport("user32")]
		public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

		[DllImport("user32")]
		public static extern int RegisterWindowMessage(string message);

		[DllImport("user32.dll")]
		static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		static extern bool UnregisterHotKey(IntPtr hWnd, int id);
		#endregion

		Dictionary<KeyValuePair<KeyModifiers, Keys>, int> _registeredHotKeys =
			new Dictionary<KeyValuePair<KeyModifiers, Keys>, int>();

		/// <summary>
		/// Represents the window that is used internally to get the messages.
		/// </summary>
		sealed class HookListenerWindow : NativeWindow, IDisposable {
			public event EventHandler<KeyPressedEventArgs> KeyPressed;
			public event EventHandler SetForegroundApplet;
			
			public HookListenerWindow() {
				// create the handle for the window.
				CreateHandle(new CreateParams());
			}

			protected override void WndProc(ref Message m) {
				// check if we got a hot key pressed.
				if (m.Msg == WM_HOTKEY) {
					// get the keys.
					var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
					var modifier = (KeyModifiers)((int)m.LParam & 0xFFFF);

					// invoke the event to notify the parent.
					if (KeyPressed != null) 
						KeyPressed(this, new KeyPressedEventArgs(modifier, key));
				}
				else if (m.Msg == WM_LGLCD_PIONEER_SETFOREGROUND) {
					if (SetForegroundApplet != null)
						SetForegroundApplet(this, EventArgs.Empty);
				}
				else {
					base.WndProc(ref m);
				}
			}
			
			public void Dispose() {
				DestroyHandle();
			}
		}
		
		public bool RegisterHotKey(KeyModifiers modifier, Keys key) {
			_currentId++;
			// register the hot key.
			if (RegisterHotKey(_hookListenerWindow.Handle, _currentId, (uint)modifier, (uint)key)) {
				_registeredHotKeys[new KeyValuePair<KeyModifiers, Keys>(modifier, key)] = _currentId;
				return true;
			}
			else return false;
		}

		public bool UnregisterHotKey(KeyModifiers modifier, Keys key) {
			return _registeredHotKeys.Remove(new KeyValuePair<KeyModifiers, Keys>(modifier, key));
		}

		public void UnregisterAllHotKeys() {
			// unregister all the registered hot keys.
			while (_currentId > 0) UnregisterHotKey(_hookListenerWindow.Handle, _currentId--);
			_registeredHotKeys.Clear();
		}


		public void Dispose() {
			UnregisterAllHotKeys();
			_hookListenerWindow.Dispose();
		}

		public static void BroadcastForegroundRequest() {
			PostMessage((IntPtr)HWND_BROADCAST, WM_LGLCD_PIONEER_SETFOREGROUND, new IntPtr(0xCDCD), new IntPtr(0xEFEF));
		}
	}

	/// <summary>
	/// Event Args for the event that is fired after the hot key has been pressed.
	/// </summary>
	public class KeyPressedEventArgs : EventArgs {
		public KeyPressedEventArgs(KeyModifiers modifier, Keys key) {
			Modifier = modifier;
			Key = key;
		}
		public KeyModifiers Modifier { get; private set; }
		public Keys Key { get; private set; }
	}

	[Flags]
	public enum KeyModifiers : uint {
		None = 0,
		Alt = 1,
		Control = 2,
		Shift = 4,
		Win = 8
	}

}