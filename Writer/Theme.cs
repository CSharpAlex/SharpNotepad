using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Writer.ControlsNS {
    public struct Theme {
        public Color txtBackColor, txtForeColor, txtCaretColor;
        public Color mBackColor, mForeColor, mBgOver, mBgDown;
        public string Name;
        static readonly Regex regex = new Regex("[/\\:*<>|]");
        public static string GetName(string name) => regex.Replace(name, "%");

        public Theme(string _Name, Color _txtBackColor, Color _txtForeColor, Color _txtCaretColor, Color _mBackColor, Color _mForeColor, Color _mBgOver, Color _mBgDown) {
            txtBackColor = _txtBackColor;
            txtForeColor = _txtForeColor;
            txtCaretColor = _txtCaretColor;
            mBackColor = _mBackColor;
            mForeColor = _mForeColor;
            mBgOver = _mBgOver;
            mBgDown = _mBgDown;

            Name = _Name;
        }

        static BinaryReader r;
        static BinaryWriter w;
        public static Theme FromFile(string themeName) {
            Theme theme;
            if (!Directory.Exists("./themes/")) { Directory.CreateDirectory("./themes/"); throw new FileNotFoundException(); }
            using (r = new BinaryReader(File.OpenRead($"./themes/{themeName}.theme"))) {
                byte version = r.ReadByte();
                //if (version == 0)
                theme = new Theme(r.ReadString(), ReadColor(), ReadColor(), ReadColor(), ReadColor(), ReadColor(), ReadColor(), ReadColor());
            }
            return theme;
        }
        public static void SaveTheme(Theme theme) {
            string themeName = theme.Name;
            //TODO: regex (replace (/|\\|*|...))
            if (!Directory.Exists("./themes/")) Directory.CreateDirectory("./themes/");
            using (w = new BinaryWriter(File.OpenWrite($"./themes/{themeName}.theme"))) {
                w.Write((byte)1); //version
                w.Write(theme.Name);
                WriteColor(theme.txtBackColor);
                WriteColor(theme.txtForeColor);
                WriteColor(theme.txtCaretColor);
                WriteColor(theme.mBackColor);
                WriteColor(theme.mForeColor);
                WriteColor(theme.mBgOver);
                WriteColor(theme.mBgDown);
            }
        }

        static Color ReadColor() => Color.FromArgb(r.ReadByte(), r.ReadByte(), r.ReadByte());
        static void WriteColor(Color color) => w.Write(new byte[] { color.R, color.G, color.B });
        //public static List<Theme> LoadThemes() {
        //    List<Theme> themes = new List<Theme>();
        //    //themes.Add(new Theme("Default", Color.FromArgb(60, 60, 60), Color.White, Color.FromArgb(80, 80, 80), Color.FromArgb(85, 85, 85), Color.FromArgb(230, 230, 230), Color.FromArgb(100, 100, 100), Color.FromArgb(90, 90, 90)));
        //    try {
        //        using (var fs = File.OpenRead("./settings/themes.themes"))
        //            themes.AddRange((Theme[])new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(fs));
        //    }
        //    catch { MessageBox.Show("Не удалось загрузить темы", Language.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        //    return themes;
        //}
        //public static void SaveThemes(Theme[] themes) {
        //    try {
        //        using (var fs = File.OpenRead("./settings/themes.themes"))
        //            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(fs, themes);
        //    }
        //    catch { MessageBox.Show("Не удалось сохранить темы");}
        //}
    }
}
