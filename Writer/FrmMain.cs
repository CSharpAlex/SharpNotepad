using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Writer.ControlsNS;
using Writer.SettingsNS;
using Writer.LanguageNS;
using System.Collections.Generic;

namespace Writer {
    partial class FrmMain : Form {
        readonly RichTextBox tb;
        FlatToolMenuControl flatToolMenu;
        readonly SaveFileDialog sfDialog;
        readonly OpenFileDialog ofDialog;
        SettingsDialog settingsDialog;

        string path = "";
        bool changed;
        bool menu_visible = true;

        public FrmMain(string path = null) {
                
            #region Initialize frm
            Settings.Load();
            BackColor = Settings.Theme.txtBackColor;

            tb = new RichTextBox {
                Location = new Point(7, 33),
                Size = new Size(ClientSize.Width - 7, ClientSize.Height - 33),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.None,
                BackColor = Settings.Theme.txtBackColor,
                ForeColor = Settings.Theme.txtForeColor,
                Font = Settings.txtFont,
                AllowDrop = true
            };
            tb.DragEnter += (sender, e) => {
                if (e.Data.GetDataPresent(DataFormats.Text))
                    e.Effect = DragDropEffects.Copy;
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Move;
                else
                    e.Effect = DragDropEffects.None;
            };
            tb.DragDrop += (sender, e) => {
                if (e.Data.GetDataPresent(DataFormats.Text)) {
                    int start = tb.SelectionStart;
                    tb.Text = tb.Text.Insert(start, e.Data.GetData(DataFormats.Text).ToString());
                    tb.SelectionStart = start;
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    tb.Text = File.ReadAllText(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
                else
                    e.Effect = DragDropEffects.None;
            };

            //Form
            Controls.Add(tb);
            Controls.Add(flatToolMenu = new FlatToolMenuControl(MenuClick));
            Text = "Sharp Notepad";
            FormClosing += FrmMain_FormClosing;
            StartPosition = FormStartPosition.Manual;

            //dialogs
            sfDialog = new SaveFileDialog {
                Title = Language.Get("save").Replace("&", ""),
                Filter = "Text|*.txt|Markdown|*.md|All|*.*",
                // TODO: if (!Settings.UseLastFolder)
                InitialDirectory = Settings.IniDir
            };
            ofDialog = new OpenFileDialog {
                Title = Language.Get("openFile").Replace("&", ""),
                Filter = "Text|*.txt|Markdown|*.md|All|*.*",
                InitialDirectory = Settings.IniDir
            };

            #endregion

            #region Load user's setting
            try {
                using (var r = new BinaryReader(File.OpenRead(Settings.SETTINGS_DIR + "/user.settings"))) {
                    byte version = r.ReadByte();
                    if (version == 1) {
                        WindowState = r.ReadBoolean() ? FormWindowState.Maximized : FormWindowState.Normal;
                        Location = new Point(r.ReadInt32(), r.ReadInt32()); //location
                        Width = r.ReadInt32(); Height = r.ReadInt32();      //size

                        //dialogs
                        ofDialog.FilterIndex = r.ReadByte();
                        sfDialog.FilterIndex = ofDialog.FilterIndex;

                        tb.WordWrap = r.ReadBoolean();
                        menu_visible = r.ReadBoolean();
                    }
                }
            }
            catch {
                StartPosition = FormStartPosition.CenterScreen;
                ClientSize = new Size(700, 400);
            }
            #endregion
            //Menu
            Menu = ControlGenerator.MainMenuGen(MenuClick, tb.WordWrap, true);

            if (path != null) Open(path);

            try {
                Icon = new Icon(Settings.APP_PATH + "/res/icon.ico");
            }
            catch { /*без иконки*/ }
            

            GC.Collect();
        }
        void NewWindow(string file = "") {
            System.Diagnostics.Process.Start(Application.ExecutablePath, file);
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e) {
            if (!CloseFile(Language.Get("msgSaveBeforeExiting"))) e.Cancel = true;
            else {
                try {
                    if (!Directory.Exists(Settings.SETTINGS_DIR + "")) Directory.CreateDirectory(Settings.SETTINGS_DIR + "");
                    using (var w = new BinaryWriter(File.OpenWrite(Settings.SETTINGS_DIR + "/user.settings"))) {
                        w.Write((byte)1); //version

                        w.Write(WindowState == FormWindowState.Maximized); //maximized
                        if (WindowState == FormWindowState.Maximized) {
                            w.Write(RestoreBounds.X); w.Write(RestoreBounds.Y);
                            w.Write(RestoreBounds.Width); w.Write(RestoreBounds.Height); //location
                        }
                        else {
                            w.Write(Left); w.Write(Top);
                            w.Write(Width); w.Write(Height);
                        }
                        w.Write((byte)sfDialog.FilterIndex); //filter index
                        w.Write(tb.WordWrap); //_wordWrap
                        w.Write(menu_visible);
                    }
                }
                catch { }
            }
        }

        //Dictionary<Keys, string> hotkeys = new Dictionary<Keys, string>() {
        //    //{ Keys.Control & Keys.C , "copy" },
        //    //{ Keys.Control & Keys.X , "cut" },
        //    //{ Keys.Control & Keys.V , "paste" },
        //    //{ Keys.Control & Keys.Z , "undo" },
        //    //{ Keys.Control & Keys.Y , "redo" },
        //    { Keys.F2, "settings" }
        //};

        public void MenuClick(string name, bool ischecked = false) {
            tb.Focus();
            switch (name) {
                //case "_menu_visible":
                    ////TODO: добавить эл. в меню
                    //Menu = ischecked ? ControlGenerator.MainMenuGen(MenuClick, tb.WordWrap, menu_visible) : null;
                    //break;
                //editing
                case "undo": tb.Undo(); break;
                case "redo": tb.Redo(); break;
                case "cut": tb.Cut(); break;
                case "copy": tb.Copy(); break;
                case "paste": tb.Paste(DataFormats.GetFormat(DataFormats.Text)); break;
                case "selectAll": tb.SelectAll(); break;

                case "find":
                    if (Controls.ContainsKey("SearchPanel")) {
                        Controls["SearchPanel"].Dispose();
                    }

                    var pnSearch = new CustomControls.SearchPanel(tb);

                    if (tb.SelectionLength > 0)
                        pnSearch.Controls["tbSearchText"].Text = tb.SelectedText;

                    Theme t = Settings.Theme;
                    pnSearch.ForeColor = t.mForeColor;
                    pnSearch.BackColor = t.mBackColor;

                    foreach (Control item in pnSearch.Controls) {
                        if (item is Button) {
                            var b = item as Button;
                            b.FlatAppearance.MouseOverBackColor = t.mBgOver;
                            b.FlatAppearance.MouseDownBackColor = t.mBgDown;
                        }
                    }

                    pnSearch.Disposed += (s, e) => {
                        tb.Height = ClientSize.Height - 33;
                    };

                    tb.Height = ClientSize.Height - 33 - pnSearch.Height;
                    Controls.Add(pnSearch);
                    break;

                //file
                case "newFile":
                    if (CloseFile(Language.Get("msgSaveBeforeCreating"))) {
                        tb.Text = "";
                        path = Settings.NameOfFile;
                        changed = false;
                    }
                    break;
                case "openFile":
                    if (CloseFile(Language.Get("msgSaveBeforeOpening")) && ofDialog.ShowDialog() == DialogResult.OK) {
                        Open(ofDialog.FileName);
                        sfDialog.FilterIndex = ofDialog.FilterIndex;
                    }
                    break;
                case "openFileWin":
                    if (ofDialog.ShowDialog() == DialogResult.OK)
                        NewWindow(ofDialog.FileName);
                    //System.Diagnostics.Process.Start(Application.StartupPath + "/MDEditor.exe", ofDialog.FileName);
                    break;
                //case "openFolder": break;
                case "save": Save(); break;
                case "saveAs": Save(true); break;
                case "export":
                    string text = "<pre>" + tb.Text + "</pre>";
                    if (sfDialog.ShowDialog() == DialogResult.OK) {
                        File.WriteAllText(sfDialog.FileName, text);
                    }
                    break;
                //window
                case "nWin":
                case "newFileWin": NewWindow(); break;
                case "_topMost": TopMost = ischecked; break;
                case "_full":
                    if (ischecked) { FormBorderStyle = FormBorderStyle.None; WindowState = FormWindowState.Normal; WindowState = FormWindowState.Maximized; }
                    else { FormBorderStyle = FormBorderStyle.Sizable; WindowState = FormWindowState.Normal; }
                    break;
                //settings 
                //font
                case "fp": tb.Font = new Font(tb.Font.FontFamily, tb.Font.Size + 1); break;
                case "fm": tb.Font = new Font(tb.Font.FontFamily, tb.Font.Size - 1); break;
                case "fr": tb.Font = Settings.txtFont; break;

                // TODO: сохранить значение в настройках (+при ининциализации установить)
                case "_wordWrap": tb.WordWrap = ischecked; break;
                //help
                // TODO: Открыть справку (web page)
                case "docs": System.Diagnostics.Process.Start("https://webpage"); break;
                case "about": break;
                case "settings":
                    if (settingsDialog == null) settingsDialog = new SettingsDialog();
                    Theme theme = Settings.Theme;
                    if (settingsDialog.ShowDialog() == DialogResult.OK) {

                        //change theme
                        if (!Settings.Theme.Equals(theme)) {
                            tb.BackColor = Settings.Theme.txtBackColor;
                            tb.ForeColor = Settings.Theme.txtForeColor;
                            BackColor = Settings.Theme.txtBackColor;

                            flatToolMenu.Dispose();
                            flatToolMenu = new FlatToolMenuControl(MenuClick);
                            Controls.Add(flatToolMenu);
                        }
                        
                        tb.Font = Settings.txtFont;
                        
                        //TODO: fmItems constructor

                        //dialogs
                        ofDialog.InitialDirectory = Settings.IniDir;
                        sfDialog.InitialDirectory = Settings.IniDir;

                        //language
                        if (settingsDialog.LangIsChanged) {
                            Menu = ControlGenerator.MainMenuGen(MenuClick, tb.WordWrap, menu_visible);
                            settingsDialog = new SettingsDialog();
                            ofDialog.Title = Language.Get("open").Replace("&", "");
                            sfDialog.Title = Language.Get("save").Replace("&", "");
                        }

                        settingsDialog = new SettingsDialog();
                    }
                    GC.Collect();
                    break;
                //exit
                case "exit": case "closeWin": Application.Exit(); break;
                default:
                    //Recent Files
                    if (name.StartsWith("open_rfile")) {
                        string path = name.Substring(10);
                        //OpenFile(ref path);
                    }
                    break;
            }
        }
    }
}
