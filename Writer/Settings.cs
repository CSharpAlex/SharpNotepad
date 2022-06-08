using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Writer.ControlsNS;
using Writer.LanguageNS;

namespace Writer.SettingsNS {
    static public partial class Settings {
    
        public readonly static string APP_PATH = Application.StartupPath, SETTINGS_DIR = APP_PATH + "/settings";
        public readonly static string THEME_DIR = APP_PATH + "/themes";

        public static Theme Theme;
        public static Font txtFont;
        public static string IniDir, NameOfFile, LanguageName;
        public static byte[] fmItems;

        static BinaryReader r;
        static BinaryWriter w;

        static public void Save() {
            try {
                if (!Directory.Exists(SETTINGS_DIR + "")) Directory.CreateDirectory(SETTINGS_DIR + "");
                using (w = new BinaryWriter(File.OpenWrite(SETTINGS_DIR + "/.settings"))) {
                    w.Write((byte)1);

                    w.Write(LanguageName);
                    w.Write(Theme.Name);

                    w.Write(txtFont.FontFamily.Name);
                    w.Write(txtFont.Size);
                    w.Write((byte)txtFont.Style);

                    w.Write(IniDir);
                    w.Write(NameOfFile);

                    w.Write((byte)fmItems.Length);
                    w.Write(fmItems);

                    // TODO: TypeOfFile
                    //w.Write(TypeOfFile);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        static public void Load() {
            try {
                if (!Directory.Exists(SETTINGS_DIR + "")) Directory.CreateDirectory(SETTINGS_DIR + "");
                using (r = new BinaryReader(File.OpenRead(SETTINGS_DIR + "/.settings"))) {
                    byte version = r.ReadByte();
                    if (version == 1) {

                        try {
                            LanguageName = r.ReadString();
                            Language.Load(LanguageName);
                        }
                        catch (Exception) { LanguageName = "english"; Language.Load(LanguageName); }

                        try {
                            Theme = Theme.FromFile(r.ReadString());
                        }
                        catch (FileNotFoundException) {
                            Theme = new Theme("default", Color.FromArgb(60, 60, 60), Color.White, Color.FromArgb(80, 80, 80), Color.FromArgb(85, 85, 85), Color.FromArgb(230, 230, 230), Color.FromArgb(100, 100, 100), Color.FromArgb(70, 70, 70));
                            Theme.SaveTheme(Theme);

                            MessageBox.Show(Language.Get("msgThemeIsNotFound"), Language.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        try {
                            txtFont = new Font(r.ReadString(), r.ReadSingle(), (FontStyle)r.ReadByte());
                        }
                        catch { txtFont = new Font(FontFamily.GenericMonospace, 11, FontStyle.Regular); }

                        IniDir = r.ReadString();
                        NameOfFile = r.ReadString();

                        fmItems = r.ReadBytes(r.ReadByte());
                        // TODO: fmItems
                        fmItems = new byte[] { 0, 1, 2, 255, 3, 4, 5 };
                    }
                }
            }
            catch (Exception ex) {
                Theme = new Theme("default", Color.FromArgb(60, 60, 60), Color.White, Color.FromArgb(80, 80, 80), Color.FromArgb(85, 85, 85), Color.FromArgb(230, 230, 230), Color.FromArgb(100, 100, 100), Color.FromArgb(90, 90, 90));
                Theme.SaveTheme(Theme);
                LanguageName = "english";

                txtFont = new Font("Consolas", 11.25f);

                if (!Directory.Exists("./files")) Directory.CreateDirectory("./files");
                IniDir = Application.StartupPath + "\\files";
                NameOfFile = "file";

                fmItems = new byte[] { 0, 1, 2, 255, 3, 4, 5 };

                Settings.Save();
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //static Color ReadColor() => Color.FromArgb(r.ReadByte(), r.ReadByte(), r.ReadByte());
        //static void WriteColor(Color color) => w.Write(new byte[] { color.R, color.G, color.B });
    }
}
