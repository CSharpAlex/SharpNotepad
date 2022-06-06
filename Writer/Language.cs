using System;
using System.Collections.Generic;
using System.IO;
using Writer.SettingsNS;

namespace Writer.LanguageNS {
    static public class Language {
        struct Word {
            public string key, val;
            public Word(string key) {
                this.key = key;
                val = null;
            }
        }
        static Word[] words;
        //static (string key, string val)[] words;
        public static void Load(string langName) {
            //default values
            words = new Word[] {
                new Word("_menu_visible"),
                new Word("File"),
                new Word("newFile"),
                new Word("newFileWin"),
                new Word("openFile"),
                new Word("openFileWin"),
                new Word("save"),
                new Word("saveAs"),
                new Word("export"),
                new Word("recentFiles"),
                new Word("exit"),
                new Word("Edit"),
                new Word("undo"),
                new Word("redo"),
                new Word("cut"),
                new Word("copy"),
                new Word("paste"),
                new Word("selectAll"),
                new Word("find"),
                new Word("replace"),
                new Word("Window"),
                new Word("_topMost"),
                new Word("_full"),
                new Word("nWin"),
                new Word("closeWin"),
                new Word("Settings"),
                new Word("font"),
                new Word("fp"),
                new Word("fm"),
                new Word("fr"),
                new Word("_wordWrap"),
                new Word("settings"),
                new Word("Help"),
                new Word("docs"),
                new Word("about"),
                new Word("settings-win-title"),
                new Word("OK"),
                new Word("Cancel"),
                new Word("NewFile"),
                new Word("folder"),
                new Word("name"),
                new Word("View"),
                new Word("theme"),
                new Word("cTheme"),
                new Word("rTheme"),
                new Word("nTheme"),
                new Word("Language"),
                new Word("downloadLang"),
                new Word("changeTheme"),
                new Word("newTheme"),
                new Word("saveTheme"),
                new Word("color"),
                new Word("bg"),
                new Word("bgOver"),
                new Word("bgDown"),
                new Word("textColor"),
                new Word("caret"),
                new Word("menu"),
                new Word("text"),
                new Word("error"),
                new Word("msgDirIsNotFound"),
                new Word("msgCharsInName"),
                new Word("msgThemeIsNotFound"),
                new Word("msgRemoveTheme"),
                new Word("msgThemeAlreadyExists"),
                new Word("msgErrorOpenFile"),
                new Word("msgErrorSaveFile"),
                new Word("msgUnsavedChanges"),
                new Word("msgSaveBeforeExiting"),
                new Word("msgSaveBeforeOpening"),
                new Word("msgSaveBeforeCreating")
            };

            string line, key, val;

            try {
                if (!Directory.Exists(Settings.SETTINGS_DIR + "/languages/")) Directory.CreateDirectory(Settings.SETTINGS_DIR + "/languages/");
                using (StreamReader r = new StreamReader(Settings.SETTINGS_DIR + $"/languages/{langName}.language")) {
                    while ((line = r.ReadLine()) != null) {
                        if (line.Length > 1 && line[0] != '#' && !string.IsNullOrWhiteSpace(line)) {
                            int ind = line.IndexOf(':');
                            key = line.Substring(0, ind).Trim();
                            val = line.Substring(ind + 1).Trim();
                            bool err = true;
                            for (byte i = 0; i < words.Length; i++)
                                if (words[i].key == key) {
                                    words[i].val = val;
                                    err = false;
                                    break;
                                }

                            //если несуществующий key
                            if (err) throw new Exception($"\"{key}\" is not defined");
                        }
                    }
                }

                //все ли определены
                foreach (var word in words)
                    if (word.val == null) throw new Exception($"Value of the key \"{word.key}\" is not found");
            }
            catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show($"Error: {ex.Message}\n\n in the file \"{langName}\"");
            }
        }

        public static string Get(string key) {
            for (byte i = 0; i < words.Length; i++)
                if (words[i].key == key) return words[i].val;
            return "";
        }
    }
}
