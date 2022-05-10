using System;
using System.Collections.Generic;
using System.IO;

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
            /*words = new (string key, string val)[] {
            ("File", null),
            ("newFile", null),
            ("newFileWin", null),
            ("openFile", null),
            ("openFileWin", null),
            ("save", null),
            ("saveAs", null),
            ("export", null),
            ("recentFiles", null),
            ("exit", null),
            ("Edit", null),
            ("undo", null),
            ("redo", null),
            ("cut", null),
            ("copy", null),
            ("paste", null),
            ("selectAll", null),
            ("Window", null),
            ("_topMost", null),
            ("_full", null),
            ("nWin", null),
            ("closeWin", null),
            ("Settings", null),
            ("font", null),
            ("fp", null),
            ("fm", null),
            ("fr", null),
            ("_wordWrap", null),
            ("settings", null),
            ("Help", null),
            ("docs", null),
            ("about", null),
            ("settings-win-title", null),
            ("OK", null),
            ("Cancel", null),
            ("NewFile", null),
            ("folder", null),
            ("name", null),
            ("View", null),
            ("theme", null),
            ("cTheme", null),
            ("rTheme", null),
            ("nTheme", null),
            ("Language", null),
            ("downloadLang", null),
            ("changeTheme", null),
            ("newTheme", null),
            ("saveTheme", null),
            ("color", null),
            ("bg", null),
            ("bgOver", null),
            ("bgDown", null),
            ("textColor", null),
            ("caret", null),
            ("menu", null),
            ("text", null),
            ("error", null),
            ("msgDirIsNotFound", null),
            ("msgCharsInName", null),
            ("msgThemeIsNotFound", null),
            ("msgRemoveTheme", null),
            ("msgThemeAlreadyExists", null),
            ("msgErrorOpenFile", null),
            ("msgErrorSaveFile", null),
            ("msgUnsavedChanges", null),
            ("msgSaveBeforeExiting", null),
            ("msgSaveBeforeOpening", null),
            ("msgSaveBeforeCreating", null)
        };*/

            words = new Word[] {
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
                if (!Directory.Exists("./settings/languages/")) Directory.CreateDirectory("./settings/languages/");
                using (StreamReader r = new StreamReader($"./settings/languages/{langName}.language")) {
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
