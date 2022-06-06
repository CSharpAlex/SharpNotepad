using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Writer.SettingsNS;
using Writer.LanguageNS;

namespace Writer.ControlsNS {
    internal static class ControlGenerator {
        internal static Button GenButton(string name, Point point, Size size) {
            var btn = new Button {
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { MouseOverBackColor = Settings.Theme.mBgOver, MouseDownBackColor = Settings.Theme.mBgDown, CheckedBackColor = Settings.Theme.mBgDown, BorderSize = 0 },
                Location = point,
                Size = size,
                BackColor = Settings.Theme.mBackColor,
                ForeColor = Settings.Theme.mForeColor,
                Text = Language.Get(name),
                Name = name
            };
            //btn.Click += (s, e) => clickDelegate(;
            return btn;
        }
        internal delegate void ClickDelegate(string name, bool ischecked = false);
        internal static MainMenu MainMenuGen(ClickDelegate clickDelegate, bool wordWrap, bool menu_visible) {
            void MItem_Click(object sender, EventArgs e) {
                var mItem = (MenuItem)sender;
                string name = mItem.Name;

                if (name.Length >= 1 && name[0] == '_') {
                    mItem.Checked = !mItem.Checked;
                    clickDelegate(name, mItem.Checked);
                }
                else clickDelegate(name);
            }
            //MenuItem[] GetRecentFiles() {
            //    string[] rfiles = new string[0];
            //    try { rfiles = System.IO.File.ReadAllLines(SETTINGS_DIR + "/recent_files.dat"); }
            //    catch (Exception) { }

            //    MenuItem[] mItems = new MenuItem[rfiles.Length];
            //    for (int i = 0; i < mItems.Length; i++) {
            //        mItems[i] = new MenuItem {
            //            Text = rfiles[i],
            //            Name = "open_rfile" + rfiles[i]
            //        }; mItems[i].Click += MItem_Click;
            //    }
            //    return mItems;
            //}
            MenuItem GenerateItem(string name, Shortcut shortcut) {
                MenuItem mItem = new MenuItem {
                    Text = Language.Get(name),
                    Name = name,
                    Shortcut = shortcut,
                    ShowShortcut = true,
                };

                if (name == "_wordWrap")
                    mItem.Checked = wordWrap;
                    
                if (name == "_menu_visible")
                    mItem.Checked = menu_visible;

                mItem.Click += MItem_Click;

                return mItem;
            }

            MainMenu menu = new MainMenu();
            //File
            menu.MenuItems.Add(Language.Get("File"), new MenuItem[] {
                GenerateItem("newFile", Shortcut.CtrlN),
                GenerateItem("newFileWin", Shortcut.CtrlShiftN),
                new MenuItem("-"),
                GenerateItem("openFile", Shortcut.CtrlO),
                GenerateItem("openFileWin", Shortcut.CtrlShiftO),
                new MenuItem("-"),
                GenerateItem("save", Shortcut.CtrlS),
                GenerateItem("saveAs", Shortcut.CtrlShiftS),
                GenerateItem("export", Shortcut.CtrlE),
                new MenuItem("-"),
                //new MenuItem(Language.Get("recentFiles"), GetRecentFiles()),
                //new MenuItem("-"),
                GenerateItem("exit", Shortcut.AltF4),
            });
            //Edit
            menu.MenuItems.Add(Language.Get("Edit"), new MenuItem[] {
                GenerateItem("undo", Shortcut.CtrlZ),
                GenerateItem("redo", Shortcut.CtrlY),
                new MenuItem("-"),
                GenerateItem("cut", Shortcut.CtrlX),
                GenerateItem("copy", Shortcut.CtrlC),
                GenerateItem("paste", Shortcut.CtrlV),
                new MenuItem("-"),
                GenerateItem("selectAll", Shortcut.CtrlA),
                new MenuItem("-"),
                GenerateItem("find", Shortcut.CtrlF),
                GenerateItem("replace", Shortcut.CtrlH),
            });
            //Window
            menu.MenuItems.Add(Language.Get("Window"), new MenuItem[] {
                //GenerateItem("_showMenu", Shortcut.CtrlShiftM),
                GenerateItem("_topMost", Shortcut.CtrlT),
                //GenerateItem("_menu_visible", Shortcut.CtrlM),
                GenerateItem("_full", Shortcut.F11),
                new MenuItem("-"),
                GenerateItem("nWin", Shortcut.CtrlShiftN),
                GenerateItem("closeWin", Shortcut.CtrlShiftW),
            });
            //Settings
            menu.MenuItems.Add(Language.Get("Settings"), new MenuItem[] {
                //GenerateItem("font", Shortcut.None),
                new MenuItem(Language.Get("font"), new MenuItem[] {
                    GenerateItem("fp", Shortcut.None),
                    GenerateItem("fm", Shortcut.None),
                    new MenuItem("-"),
                    GenerateItem("fr", Shortcut.Ctrl0),
                }),
                GenerateItem("_wordWrap", Shortcut.CtrlF3),
                GenerateItem("settings", Shortcut.F2),
            });
            //Help
            menu.MenuItems.Add(Language.Get("Help"), new MenuItem[] {
                GenerateItem("docs", Shortcut.F1),
                GenerateItem("about", Shortcut.None),
            });

            return menu;
        }
    }
}
