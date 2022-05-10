using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Writer {
    static class Program {
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain(args.Length == 1 ? args[0] : null));
        }
    }
}
