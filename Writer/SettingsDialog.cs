using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using Writer.SettingsNS;
using Writer.LanguageNS;

namespace Writer.ControlsNS {
    public partial class SettingsDialog : Form {
        readonly Panel pnMenu, pnItems;
        readonly ComboBox cmbTheme, cmbLang;//, cmbFontName, cmbFontStyle;
        readonly TextBox txtIniDir, txtNameOfFile;//, txtTypeOfFile;
        //readonly NumericUpDown nupFontSize;
        //readonly Label lbFontPreview;
        readonly ErrorProvider errorProvider = new ErrorProvider { BlinkRate = 0 };
        int y = 0;
        const byte _MenuItemHeight = 35;
        const string chars = @"/\:*<>|";

        private void Tb_Validating(object sender, EventArgs e) {
            var tb = (TextBox)sender;
            bool checking = true;
            string msg;
            switch (tb.Name) {
                case "IniDir":
                    checking = Directory.Exists(tb.Text);
                    msg = Language.Get("msgDirIsNotFound");
                    break;
                case "NameOfFile":
                    foreach (char ch in tb.Text) if (chars.IndexOf(ch) != -1) { checking = false; break; }
                    msg = Language.Get("msgCharsInName");
                    break;
                default: msg = "error"; break;
            }
            if (checking) { errorProvider.Clear(); tb.ForeColor = Color.Black; }
            else { errorProvider.SetError(tb, msg); tb.ForeColor = Color.Red; }
        }

        //private void Font_Changed(object sender, EventArgs e) {
        //    lbFontPreview.Font = new Font(cmbFontName.SelectedItem.ToString(), (float)nupFontSize.Value, (FontStyle)cmbFontStyle.SelectedIndex);
        //}

        private void SettingsDialog_Click(object sender, EventArgs e) {
            //void err(string msg) { MessageBox.Show("", Language.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Warning); return }
            string name = ((Button)sender).Name;
            switch (name) {
                case "OK":
                    //bool checkData(bool cond, string msg) {
                    //    if (!cond) MessageBox.Show(msg, Language.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    return cond;
                    //}

                    if (Directory.Exists(txtIniDir.Text))
                        Settings.IniDir = txtIniDir.Text;
                    else {
                        Directory.CreateDirectory(Application.StartupPath + "/files");
                        Settings.IniDir = Application.StartupPath + "/files";
                    }

                    Settings.Theme = Theme.FromFile(cmbTheme.SelectedItem.ToString());

                    LangIsChanged = cmbLang.SelectedItem.ToString() != Settings.LanguageName;
                    Settings.LanguageName = cmbLang.SelectedItem.ToString();
                    if (LangIsChanged) Language.Load(Settings.LanguageName);
                    
                    Settings.NameOfFile = txtNameOfFile.Text;

                    // TODO: Font (NumUpDown)
                    //Settings.mdFont = new Font(cmbFontName.SelectedItem.ToString(), (float)nupFontSize.Value, (FontStyle)cmbFontStyle.SelectedIndex);
                    //((Button)sender).DialogResult = DialogResult.None;
                    Settings.Save();
                    break;
                case "cTheme":
                case "nTheme":
                    ThemeDialog themeDialog = new ThemeDialog(name[0] == 'c' ? cmbTheme.SelectedItem.ToString() : null);
                    if (themeDialog.ShowDialog() == DialogResult.OK) {
                        
                    }
                    themeDialog.Dispose();
                    break;
                case "rTheme":
                    if (MessageBox.Show(Language.Get("msgRemoveTheme"), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        File.Delete($"./themes/{cmbTheme.SelectedItem}.theme");
                        cmbTheme.Items.RemoveAt(cmbTheme.SelectedIndex);
                    }
                    break;
                case "sFolder":
                    var frm = new FolderBrowserDialog();
                    if (frm.ShowDialog() == DialogResult.OK)
                        txtIniDir.Text = frm.SelectedPath;
                    frm.Dispose();
                    break;
            }
        }
    }
}