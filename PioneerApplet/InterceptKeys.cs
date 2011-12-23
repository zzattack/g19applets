using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PioneerApplet {

	/// <summary> This class allows you to manage a hotkey </summary>
	public class KeyboardHook : IDisposable {

		// Registers a hot key with Windows.
		[DllImport("user32.dll")]
		static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		// Unregisters the hot key with Windows.
		[DllImport("user32.dll")]
		static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		Dictionary<KeyValuePair<KeyModifiers, Keys>, int> RegisteredHotKeys =
			new Dictionary<KeyValuePair<KeyModifiers, Keys>, int>();

		/// <summary>
		/// Represents the window that is used internally to get the messages.
		/// </summary>
		class Window : NativeWindow, IDisposable {
			static int WM_HOTKEY = 0x0312;

			public Window() {
				// create the handle for the window.
				this.CreateHandle(new CreateParams());
			}

			/// <summary>
			/// Overridden to get the notifications.
			/// </summary>
			/// <param name="m"></param>
			protected override void WndProc(ref Message m) {
				base.WndProc(ref m);

				// check if we got a hot key pressed.
				if (m.Msg == WM_HOTKEY) {
					// get the keys.
					Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
					KeyModifiers modifier = (KeyModifiers)((int)m.LParam & 0xFFFF);

					// invoke the event to notify the parent.
					if (KeyPressed != null) KeyPressed(this, new KeyPressedEventArgs(modifier, key));
				}
			}

			public event EventHandler<KeyPressedEventArgs> KeyPressed;

			#region IDisposable Members

			public void Dispose() {
				this.DestroyHandle();
			}

			#endregion
		}

		Window _window = new Window();
		int _currentId;

		public KeyboardHook() {
			// register the event of the inner native window.
			_window.KeyPressed += delegate(object sender, KeyPressedEventArgs args) {
				if (KeyPressed != null) KeyPressed(this, args);
			};
		}

		/// <summary>
		/// Registers a hot key in the system.
		/// </summary>
		/// <param name="modifier">The modifiers that are associated with the hot key.</param>
		/// <param name="key">The key itself that is associated with the hot key.</param>
		public bool RegisterHotKey(KeyModifiers modifier, Keys key) {
			// increment the counter.
			_currentId = _currentId + 1;

			// register the hot key.
			if (RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key)) {
				RegisteredHotKeys[new KeyValuePair<KeyModifiers, Keys>(modifier, key)] = _currentId;
				return true;
			}
			else return false;
		}

		public bool UnregisterHotKey(KeyModifiers modifier, Keys key) {
			return RegisteredHotKeys.Remove(new KeyValuePair<KeyModifiers, Keys>(modifier, key));
		}

		public void UnregisterAllHotKeys() {
			// unregister all the registered hot keys.
			while (_currentId > 0) UnregisterHotKey(_window.Handle, _currentId--);
			RegisteredHotKeys.Clear();
		}

		/// <summary>
		/// A hot key has been pressed.
		/// </summary>
		public event EventHandler<KeyPressedEventArgs> KeyPressed;

		#region IDisposable Members

		public void Dispose() {
			// unregister all the registered hot keys
			UnregisterAllHotKeys();

			// dispose the inner native window.
			_window.Dispose();
		}

		#endregion
	}

	/// <summary>
	/// Event Args for the event that is fired after the hot key has been pressed.
	/// </summary>
	public class KeyPressedEventArgs : EventArgs {
		KeyModifiers _modifier;
		Keys _key;

		internal KeyPressedEventArgs(KeyModifiers modifier, Keys key) {
			_modifier = modifier;
			_key = key;
		}

		public KeyModifiers Modifier {
			get { return _modifier; }
		}

		public Keys Key {
			get { return _key; }
		}
	}

	/// <summary>
	/// The enumeration of possible modifiers.
	/// </summary>
	[Flags]
	public enum KeyModifiers : uint {
		None = 0,
		Alt = 1,
		Control = 2,
		Shift = 4,
		Win = 8
	}

}