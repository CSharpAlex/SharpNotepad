using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Writer.ControlsNS;
using Writer.SettingsNS;
using Writer.LanguageNS;

namespace Writer {
    partial class FrmMain : Form {
        bool CloseFile(string msg) {
            if ((changed && path != "" && tb.Text.Length != 0) || (path == "" && tb.Text.Length != 0)) {
                DialogResult res = MessageBox.Show(msg, Language.Get("msgUnsavedChanges"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (res == DialogResult.Yes)
                    Save();
                else if (res == DialogResult.Cancel)
                    return false;
            }
            path = "";
            return true;
        }
        public void Open(string path) {
            try {
                tb.Text = File.ReadAllText(path);
                this.path = path;
                changed = false;
                Text = Path.GetFileName(path) + " - Sharp Notepad";
            }
            catch (Exception ex) {
                MessageBox.Show(Language.Get("msgErrorOpenFile") + "\n(" + ex.Message + ')', Language.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Save(bool saveAs = false) {
            if (saveAs || path == "") {
                string name; int i = 0;
                do { i++; name = Settings.NameOfFile + i + '.' + "txt"; } while (File.Exists(Settings.IniDir + '/' + name));
                sfDialog.FileName = name;
                var dr = sfDialog.ShowDialog();
                ofDialog.FilterIndex = sfDialog.FilterIndex;
                if (dr == DialogResult.OK) {
                    path = sfDialog.FileName;
                    Text = Path.GetFileName(path) + " - Sharp Notepad";
                }
                else return;
            }
            try { File.WriteAllText(path, tb.Text); }
            catch (Exception ex) { MessageBox.Show(Language.Get("msgErrorSaveFile") + "\n(" + ex.Message + ')', Language.Get("error"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
            changed = false;
        }
    }
}