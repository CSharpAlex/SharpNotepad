using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Writer.SettingsNS;
using Writer.LanguageNS;

namespace Writer.ControlsNS {
    internal class ThemeDialog : Form {
        readonly TextBox txtName;
        readonly ColorDialog colorDialog;
        public ThemeDialog(string themeName = null) {
            #region IniFunctions
            short x = 5, y = 72;
            const byte CPANEL_HEIGHT = 50;
            void AddColorPanel(string name) {
                var pnColor = new Panel {
                    Location = new Point(x, y),
                    Size = new Size(CPANEL_HEIGHT, CPANEL_HEIGHT),
                    BackColor = Settings.Theme.txtBackColor,
                    ForeColor = Settings.Theme.txtForeColor,
                    BorderStyle = BorderStyle.FixedSingle,
                    Name = (x == 5) ? 'm' + name : name
                };

                pnColor.Click += PnColor_Click;

                var label = new Label {
                    Location = new Point(x + CPANEL_HEIGHT, y),
                    Size = new Size(this.ClientSize.Width / 2 - CPANEL_HEIGHT - 10, CPANEL_HEIGHT),
                    BackColor = Settings.Theme.txtBackColor,
                    ForeColor = Settings.Theme.txtForeColor,
                    BorderStyle = BorderStyle.FixedSingle,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Text = Language.Get(name),
                    Name = (x == 5) ? 'm' + name + '*' : name + '*'
                };

                label.Click += PnColor_Click;

                Controls.Add(pnColor);
                Controls.Add(label);

                y += CPANEL_HEIGHT/* + 3*/;
            }

            Button GenButton(string name, Point point) {
                var btn = new Button {
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { MouseOverBackColor = Settings.Theme.mBgOver, MouseDownBackColor = Settings.Theme.mBgDown, CheckedBackColor = Settings.Theme.mBgDown, BorderSize = 0 },
                    Location = point,
                    Size = new Size(95, 30),
                    BackColor = Settings.Theme.mBackColor,
                    ForeColor = Settings.Theme.mForeColor,
                    Text = Language.Get(name),
                    Font = new Font("Arial", 11),
                    Name = name
                };
                if (name == "OK") { btn.DialogResult = DialogResult.OK; }
                else if (name == "Cancel") CancelButton = btn;
                return btn;
            }
            #endregion

            Theme theme;
            #region __init__
            BackColor = Settings.Theme.txtBackColor;
            ForeColor = Settings.Theme.txtForeColor;
            Font = new Font("Arial", 13);
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(500, 370);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Text = Language.Get(themeName == null ? "newTheme" : "changeTheme");
            AutoScroll = true;

            var lb = new Label {
                AutoSize = true,
                Location = new Point(20, 20),
                Text = Language.Get("theme"),
                Width = 75,
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtName = new TextBox {
                Location = new Point(85, 20),
                Width = ClientSize.Width - lb.Width - 20,
                Text = Language.Get("theme")
            };

            Controls.Add(txtName);
            Controls.Add(lb);

            Controls.Add(new Label {
                Location = new Point(x, y += 2),
                Size = new Size(ClientSize.Width / 2 - 10, CPANEL_HEIGHT / 2),
                BackColor = Settings.Theme.mBackColor,
                ForeColor = Settings.Theme.mForeColor,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = Language.Get("menu")
            });
            y += CPANEL_HEIGHT / 2;
            AddColorPanel("bg");
            AddColorPanel("bgOver");
            AddColorPanel("bgDown");
            AddColorPanel("textColor");

            x = (short)(ClientSize.Width / 2);
            y = 72;

            Controls.Add(new Label {
                Location = new Point(x, y += 2),
                Size = new Size(ClientSize.Width / 2 - 10, CPANEL_HEIGHT / 2),
                BackColor = Settings.Theme.mBackColor,
                ForeColor = Settings.Theme.mForeColor,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = Language.Get("text")
            });
            y += CPANEL_HEIGHT / 2;

            AddColorPanel("bg");
            AddColorPanel("textColor");
            AddColorPanel("caret");

            var btnCancel = GenButton("Cancel", new Point(Width - 125, 10));
            var btnOK = GenButton("OK", new Point(Width - 235, 10));
            var pn = new Control {
                BackColor = Settings.Theme.mForeColor,
                Dock = DockStyle.Bottom,
                Height = 50,
                Location = new Point(0, ClientSize.Height - 50)
            };

            pn.Controls.Add(btnOK); pn.Controls.Add(btnCancel);
            Controls.Add(pn);

            //data
            if (themeName != null) {
                theme = Theme.FromFile(themeName);
                txtName.Text = themeName;
                Controls["mbg"].BackColor = theme.mBackColor;
                Controls["mbgOver"].BackColor = theme.mBgOver;
                Controls["mbgDown"].BackColor = theme.mBgDown;
                Controls["mtextColor"].BackColor = theme.mForeColor;
                Controls["bg"].BackColor = theme.txtBackColor;
                Controls["textColor"].BackColor = theme.txtForeColor;
                Controls["caret"].BackColor = theme.txtCaretColor;
            }

            //dialog
            colorDialog = new ColorDialog {
                FullOpen = true
            };
            #endregion

            btnOK.Click += delegate (object sender, EventArgs e) {
                Theme theme1 = new Theme(
                    txtName.Text,
                    Controls["bg"].BackColor,
                    Controls["textColor"].BackColor,
                    Controls["caret"].BackColor,
                    Controls["mbg"].BackColor,
                    Controls["mtextColor"].BackColor,
                    Controls["mbgOver"].BackColor,
                    Controls["mbgDown"].BackColor);
                if (themeName == null && File.Exists($"./themes/{theme1.Name}.theme")) {
                    MessageBox.Show(Language.Get("msgThemeAlreadyExists"), "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (themeName != null && theme1.Name != themeName) File.Delete($"./themes/{themeName}.theme");
                Theme.SaveTheme(theme1);
            };
        }

        private void PnColor_Click(object sender, EventArgs e) {
            var pnc = (Control)sender;
            colorDialog.Color = Controls[pnc.Name.Replace("*", "")].BackColor;
            colorDialog.ShowDialog();
            Controls[pnc.Name.Replace("*", "")].BackColor = colorDialog.Color;
        }
    }
}
