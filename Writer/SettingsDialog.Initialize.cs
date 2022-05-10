using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using Writer.SettingsNS;
using Writer.LanguageNS;

namespace Writer.ControlsNS {
    public partial class SettingsDialog : Form {
        public bool LangIsChanged;
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
                    FlatAppearance = { MouseOverBackColor = Settings.Theme.mBgOver, MouseDownBackColor = Settings.Theme.mBgDown, CheckedBackColor = Settings.Theme.mBgDown, BorderSize = 0 },
                    Location = point,
                    Size = size,
                    BackColor = Settings.Theme.mBackColor,
                    ForeColor = Settings.Theme.mForeColor,
                    Text = name == "sFolder" ? ". . ." : Language.Get(name),
                    TabIndex = tind++,
                    Name = name
                };
                btn.Click += SettingsDialog_Click;
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
                    Text = caption == null ? Language.Get(name) : caption,
                    Name = name,
                    //Width = 75,
                    AutoSize = true,
                    Height = 24,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(20, PlusY()),
                    Font = new Font(Font.FontFamily, pnItems.Font.Size + 1)
                };
            /*CheckBox GetCheckBox(string caption, string name, bool ischecked) {
                var chb = new CheckBox {
                    Text = caption,
                    Name = name,
                    Location = new Point(121, PlusY()),
                };
                return chb;
            }*/
            /*GroupBox groupBox(string caption, Control[] controls) {
                var gb = new GroupBox {
                    Text = caption,
                    Name = caption,
                    Location = new Point(20, yItem),
                    AutoSize = true,
                    Width = pnItems.Width - 40,
                    ForeColor = ForeColor
                };
                gb.Controls.AddRange(controls);
                return gb;
            }*/
            #endregion

            #region Ini
            // SettingsDialog
            ClientSize = new Size(650, 350);
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
            pnSetButtons.Controls.AddRange(new Control[] { GenButton("OK", new Point(Width - 235, 10), new Size(95, 30)), GenButton("Cancel", new Point(Width - 125, 10), new Size(95, 30)) });

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
                    LbGen("folder"), txtIniDir = TxtGen(Settings.IniDir,"IniDir", 304), GenButton("sFolder", new Point(407, yItem), new Size(38, 24)),
                    LbGen("name"), txtNameOfFile = TxtGen(Settings.NameOfFile,"NameOfFile", 350),
					//new Label {Text = "Type",AutoSize = true,Location = new Point(20, PlusY()) }, txtTypeOfFile = TxtGen(Settings.TypeOfFile, 350),
			});
            GenerateItem("View", new Control[]{
                    LbGen("theme"),
                    cmbTheme = GetComboBox(new Point(125, yItem), new Size(121, 26)),
                    GenButton("cTheme", new Point(252, yItem), new Size(90, 26)),
                    GenButton("nTheme", new Point(252, PlusY()), new Size(90, 26)),
                    GenButton("rTheme", new Point(252, PlusY()), new Size(90, 26)),

                    /*groupBox("Font", new Control[] {
                        new Label { Text = "Name", Location = new Point(20, 30), AutoSize=true }, cmbFontName = GetComboBox(new Point(105, yItem), new Size(121, 26)),
                        LbGen("Size"),
                        nupFontSize = new NumericUpDown() {
                            Name = "fSize", Minimum = 1, Maximum = 72, DecimalPlaces = 2, Value = (decimal)Settings.mdFont.Size, Location = new Point(105, yItem), Size = new Size(121, 26),
                        },
                        LbGen("Style"),  cmbFontStyle = GetComboBox(new Point(105, yItem), new Size(121, 26), new object[] { "Regular","Bold","Italic", "Underline"}),
                        lbFontPreview = LbGen("Plain text"),

                        //GetCheckBox("Bold", "bold", Settings.mdFont.Bold),
                        //GetCheckBox("Italic", "italic", Settings.mdFont.Italic),
                        //GetCheckBox("Underline", "underline", Settings.mdFont.Underline),
                    }),*/
                                        
                    //LbGen(Settings.mdFont.Name + ", " + Settings.mdFont.SizeInPoints + "p", "fontName"), GenButton("Select font...","sFont", new Point(252, yItem), new Size(90, 26)),
            }); ;
            GenerateItem("Language", new Control[]{
                LbGen("Language"),
                cmbLang = GetComboBox(/*"cmbLang", */new Point(125, yItem), new Size(121, 26)),
                GenButton("downloadLang", new Point(252, yItem), new Size(90, 26)),
            });

            pnMenu.Controls[0].BackColor = Settings.Theme.mBgDown;
            #endregion

            /**/
            string[] files = Directory.GetFiles("./settings/languages", "*.language");
            int start = "./settings/languages/".Length;
            foreach (string lang in files)
                cmbLang.Items.Add(lang.Substring(start, lang.Length - start - ".language".Length));
            cmbLang.SelectedItem = Settings.LanguageName;

            files = Directory.GetFiles("./themes", "*.theme");
            start = "./themes/".Length;
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
        }
    }
}