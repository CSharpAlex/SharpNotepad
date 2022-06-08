using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using Writer.SettingsNS;
using Writer.LanguageNS;

namespace Writer.ControlsNS {
    public partial class SettingsDialog : Form {
        public bool LangIsChanged;
        private NumericUpDown nupFontSize;
        private Label lbFontPreview;
        private ComboBox cmbFontStyle;
        private ComboBox cmbFontName;

        public SettingsDialog() {
            void button_Click(object sender, EventArgs e) {
                var but = (Button)sender;
                /**/
                foreach (Button b in pnMenu.Controls)
                    b.BackColor = Settings.Theme.mBackColor;
                but.BackColor = Settings.Theme.mBgDown;
                /**/
                pnItems.Controls[but.Name].BringToFront();
            }
            #region colors
            BackColor = Settings.Theme.mBackColor;
            ForeColor = Settings.Theme.txtForeColor;
            #endregion
            int yItem = 0, tind = 0; //y, item's TabIndex
            int PlusY() =>
                yItem += (int)Font.Size + 20;
            void GenerateItem(string name, Control[] controls) {
                yItem = 0;
                //pnMenu
                var button = new Button {
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0, MouseOverBackColor = Settings.Theme.mBgOver, MouseDownBackColor = Settings.Theme.mBgDown, },
                    AutoSize = true,
                    Height = _MenuItemHeight,
                    Width = pnMenu.Width,
                    Text = Language.Get(name),
                    Top = y,
                    Name = name,
                };
                button.Click += new EventHandler(button_Click);
                pnMenu.Controls.Add(button);

                //pnItems
                var pn = new Panel {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    Name = name
                };
                pn.Controls.AddRange(controls);
                pnItems.Controls.Add(pn);
                y += _MenuItemHeight;
                yItem = 0;
            }

            #region Ini Functions
            Button GenButton(string name, Point point, Size size) {
                var btn = new Button {
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Location = point,
                    Size = size,
                    BackColor = Settings.Theme.mBackColor,
                    ForeColor = Settings.Theme.mForeColor,
                    Text = name == "sFolder" ? ". . ." : Language.Get(name),
                    TabIndex = tind++,
                    Name = name
                };
                btn.Click += SettingsDialog_Click;


                //для Mono
                btn.MouseEnter += (s, e) => (s as Control).BackColor = Settings.Theme.mBgOver;
                btn.MouseLeave += (s, e) => (s as Control).BackColor = Settings.Theme.mBackColor;
                btn.MouseDown += (s, e) => (s as Control).BackColor = Settings.Theme.mBgDown;
                btn.MouseUp += (s, e) => (s as Control).BackColor = Settings.Theme.mBackColor;


                if (name == "OK") { /*AcceptButton = btn;*/ btn.DialogResult = DialogResult.OK; }
                else if (name == "Cancel") CancelButton = btn;
                return btn;
            }
            ComboBox GetComboBox(/*string name, */Point point, Size size, string[] items = null, int selectedIndex = -1) {
                var cmb = new ComboBox {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    //Name = name,
                    Location = point,
                    Size = size,
                };
                if (items != null) cmb.Items.AddRange(items);
                cmb.SelectedIndex = selectedIndex;
                return cmb;
            }
            TextBox TxtGen(string caption, string name, int width) {
                var tb = new TextBox { Text = caption, Name = name, Width = width, Location = new Point(95, yItem) };
                //tb.Validating += Tb_Validating;
                tb.TextChanged += Tb_Validating;
                return tb;
            }
            Label LbGen(string name, string caption = null) =>
                new Label {
                    Text = caption ?? Language.Get(name),
                    Name = name,
                    //Width = 75,
                    AutoSize = true,
                    Height = 24,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(20, PlusY() + 3),
                    Font = new Font(Font.FontFamily, pnItems.Font.Size + 1)
                };
            CheckBox GetCheckBox(string caption, string name, bool ischecked) {
                var chb = new CheckBox {
                    Text = caption,
                    Name = name,
                    Location = new Point(121, PlusY()),
                };
                return chb;
            }

            GroupBox groupBox(string caption, Control[] controls) {
                yItem = 0;
                var gb = new GroupBox {
                    Text = Language.Get(caption),
                    Name = caption,
                    Location = new Point(20, 20),
                    Width = pnItems.Width - 40,
                    ForeColor = ForeColor
                };
                gb.Controls.AddRange(controls);

                gb.Height = gb.Controls[gb.Controls.Count - 1].Top + gb.Controls[gb.Controls.Count - 1].Height + 20;

                return gb;
            }
            #endregion

            #region Ini
            // SettingsDialog
            ClientSize = new Size(700, 400);
            Font = new Font("Microsoft Sans Serif", 11.25F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Text = Language.Get("settings-win-title");
            MaximizeBox = false;

            var pnSetButtons = new Panel {
                Dock = DockStyle.Bottom,
                Location = new Point(0, 268),
                Height = 50,
                BackColor = Settings.Theme.mForeColor
            };
            pnSetButtons.Controls.AddRange(new Control[] { GenButton("OK", new Point(Width - 235, 10), new Size(105, 30)), GenButton("Cancel", new Point(Width - 125, 10), new Size(105, 30)) });

            pnMenu = new Panel {
                Width = 181,
                AutoScroll = true,
                Dock = DockStyle.Left,
                Font = new Font("Microsoft Sans Serif", 11.5f),
                BackColor = Settings.Theme.mBackColor,
                ForeColor = Settings.Theme.mForeColor
            };

            pnItems = new Panel {
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft Sans Serif", 11),
                BackColor = Settings.Theme.txtBackColor,
                ForeColor = Settings.Theme.txtForeColor
            };

            pnItems.Dock = DockStyle.Fill;

            Controls.Add(pnItems);
            Controls.Add(pnMenu);
            Controls.Add(pnSetButtons);
            #endregion

            #region Ini Items
            GenerateItem("NewFile", new Control[]{
                    groupBox("values_for_a_new_file", new Control[] {
                        LbGen("folder"), txtIniDir = TxtGen(Settings.IniDir,"IniDir", 304), GenButton("sFolder", new Point(407, yItem), new Size(38, 24)),
                        LbGen("name"), txtNameOfFile = TxtGen(Settings.NameOfFile,"NameOfFile", 350)
                    })
					//new Label {Text = "Type",AutoSize = true,Location = new Point(20, PlusY()) }, txtTypeOfFile = TxtGen(Settings.TypeOfFile, 350),
			});

            GroupBox g1, g2;
            Button btnFont;
            GenerateItem("View", new Control[]{
                    g1 = groupBox("theme", new Control[] {
                        LbGen("theme"),
                        cmbTheme = GetComboBox(new Point(100, yItem), new Size(121, 30)),
                        GenButton("cTheme", new Point(230, yItem), new Size(170, 30)),
                        GenButton("nTheme", new Point(230, PlusY()), new Size(170, 30)),
                        GenButton("rTheme", new Point(230, PlusY()), new Size(170, 30)),
                    }),

                    g2 = groupBox("changeFont", new Control[] {
                        LbGen("changeFont"),
                        btnFont = GenButton("changeFont", new Point(230, yItem), new Size(170, 30))
                    })
                    //LbGen(Settings.mdFont.Name + ", " + Settings.mdFont.SizeInPoints + "p", "fontName"), GenButton("Select font...","sFont", new Point(252, yItem), new Size(90, 26)),
            });

            g1.Top = 20;
            g2.Top = g1.Top + g1.Height + 20;

            GenerateItem("Language", new Control[]{
                LbGen("Language"),
                cmbLang = GetComboBox(/*"cmbLang", */new Point(125, yItem), new Size(121, 26)),
                GenButton("downloadLang", new Point(252, yItem), new Size(90, 26)),
            });

            pnMenu.Controls[0].BackColor = Settings.Theme.mBgDown;
            #endregion

            /**/
            string[] files = Directory.GetFiles(Settings.SETTINGS_DIR + "/languages", "*.language");
            int start = (Settings.SETTINGS_DIR + "/languages/").Length;
            foreach (string lang in files)
                cmbLang.Items.Add(lang.Substring(start, lang.Length - start - ".language".Length));
            cmbLang.SelectedItem = Settings.LanguageName;

            files = Directory.GetFiles(Settings.THEME_DIR, "*.theme");
            start = (Settings.THEME_DIR + "/").Length;
            foreach (string theme in files)
                cmbTheme.Items.Add(theme.Substring(start, theme.Length - start - ".theme".Length));
            cmbTheme.SelectedItem = Settings.Theme.Name;

            /*foreach (FontFamily fontFamily in FontFamily.Families)
                cmbFontName.Items.Add(fontFamily.Name);
            cmbFontName.SelectedItem = Settings.mdFont.FontFamily.Name;
            cmbFontName.SelectedIndexChanged += Font_Changed;
            cmbFontStyle.SelectedIndexChanged += Font_Changed;
            nupFontSize.ValueChanged += Font_Changed;*/

            //cmbTheme.SelectedIndexChanged +=
            //delegate (object sender, EventArgs e) { pnItems.Controls["View"].Controls["cTheme"].Visible = cmbTheme.SelectedIndex == 3; };

            //pnItems.Controls["View"].Controls["fontName"].Text = Settings.mdFont.FontFamily.Name + ", " + Settings.mdFont.SizeInPoints + "p, " + Settings.mdFont.Style.ToString();


            //TODO: загрузка языков с сервера
            pnItems.Controls["Language"].Controls["downloadLang"].Hide();
            
            btnFont.Click += (sender, e) => {
                FontDialog dialog = new FontDialog {
                    Font = Settings.txtFont,
                    FontMustExist = true,
                    ShowEffects = false
                };

                if (dialog.ShowDialog() == DialogResult.OK) {
                    Settings.txtFont = dialog.Font;
                }
            };
        }
    }
}