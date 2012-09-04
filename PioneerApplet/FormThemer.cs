using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace PioneerApplet {

	public class Theme {
		public const string ThemeDir = "themes/";

		public string Name { get; set; }
		public Image Background { get { return new Bitmap(ThemeDir + this.backgroundUrl); } }
		private string backgroundUrl;
		public Color ForeColor { get; set; }
		public Color BackColor { get; set; }

		public Theme(string name, string backgroundUrl, string foreColor, string backColor) {
			this.Name = name;
			this.backgroundUrl = backgroundUrl;
			this.ForeColor = Color.FromName(foreColor);
			this.BackColor = Color.FromName(backColor);
			new Bitmap(ThemeDir + this.backgroundUrl);
		}
	}

	class FormThemer {
		List<Theme> _themes = new List<Theme>();

		public FormThemer() {

			try {
				XmlDocument xdoc = new XmlDocument();
				xdoc.Load(Theme.ThemeDir + "themes.xml");

				foreach (XmlNode theme in xdoc["themes"]) {
					try {
						_themes.Add(new Theme(
							theme["name"].InnerText,
							theme["background"].InnerText,
							theme["forecolor"].InnerText,
							theme["backcolor"].InnerText));
					}
					catch { }
				}
			}
			catch { }
		}

		public int NumThemes { get { return _themes.Count; } }

		public Theme GetTheme(string name) {
			return _themes.FirstOrDefault(t => t.Name == name);
		}

		public Theme GetTheme(int idx) {
			return _themes[idx];
		}

		public Theme GetTheme() {
			Random r = new Random();
			return _themes[r.Next(_themes.Count)];
		}

	}
}
