using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Writer.SettingsNS;

namespace Writer.ControlsNS {
    public class FlatToolMenuControl : Panel {
        public delegate void ClickDelegate(string name, bool ischecked = false);
        readonly ClickDelegate cl_d;
        int x;

        /*public void SetNewStyles() {
            BackColor = Settings.Theme.mBackColor;
            ForeColor = Settings.Theme.mForeColor;
            for (byte i = 0; i < Controls.Count; i++)
                if (Controls[i].Name != "sep") {
                    var b = (Button)Controls[i];
                    b.FlatAppearance.MouseOverBackColor = Settings.Theme.mBgOver;
                    b.FlatAppearance.MouseDownBackColor = Settings.Theme.mBgDown;
                    b.BackColor = Settings.Theme.mBackColor;
                    b.ForeColor = Settings.Theme.mForeColor;
                }
                else {
                    ((Panel)Controls[i]).BackColor = Settings.Theme.mBgDown;
                }
            SetNewBitmaps();
        }*/

        /*        public void SetNewStylesAll() {
                    BackColor = Settings.Theme.mBackColor;
                    ForeColor = Settings.Theme.mForeColor;
                    for (byte i = 0; i < Controls.Count; i++)
                        if (Controls[i].Name != "sep") {
                            var b = (Button)Controls[i];
                            b.FlatAppearance.MouseOverBackColor = Settings.Theme.mBgOver;
                            b.FlatAppearance.MouseDownBackColor = Settings.Theme.mBgDown;
                            //b.BackColor = Settings.Theme.mBackColor;
                            //b.ForeColor = Settings.Theme.mForeColor;
                            if (b.BackgroundImage != null) {
                                Bitmap bmp = (Bitmap)b.BackgroundImage;
                                for (byte x = 0; x < bmp.Width; x++)
                                    for (byte y = 0; y < bmp.Height; y++) {
                                        Color c = bmp.GetPixel(x, y);
                                        if (c.A > 0)
                                            c = Color.FromArgb(c.A, Settings.Theme.mForeColor);
                                        bmp.SetPixel(x, y, c);
                                    }
                                b.BackgroundImage = bmp;
                            }
                        }
                        else ((Panel)Controls[i]).BackColor = Settings.Theme.mBgDown;
                }*/


        public Bitmap CreateNonIndexedImage(Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }

        public FlatToolMenuControl(ClickDelegate click) {
            x = 0;
            cl_d = click;
            Height = 30;
            Dock = DockStyle.Top;
            BackColor = Settings.Theme.mBackColor;                  
            ForeColor = Settings.Theme.mForeColor;

            string[] arr = { "newFile", "save", "openFile", "cut", "copy", "paste" };
            foreach (byte fmItem in Settings.fmItems) {
                if (fmItem == 255)
                    AddSeparator();
                else if (fmItem < arr.Length) {
                    Bitmap bmpOrig = (Bitmap)Properties.Resources.ResourceManager.GetObject(arr[fmItem]);

                    Bitmap bmp = CreateNonIndexedImage(bmpOrig); //bmpOrig.Clone(new Rectangle(0, 0, bmpOrig.Width, bmpOrig.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);


                    for (byte x = 0; x < bmp.Width; x++)
                        for (byte y = 0; y < bmp.Height; y++) {
                            Color c = bmp.GetPixel(x, y);
                            if (c.A > 0)
                                c = Color.FromArgb(c.A, Settings.Theme.mForeColor);
                            bmp.SetPixel(x, y, c);
                        }
                    AddItem(null, arr[fmItem], bmp);
                }
            }
        }
        public void AddItem(string caption, string name, Image image = null) {
            Button btn = new Button {
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Settings.Theme.mBgOver, MouseDownBackColor = Settings.Theme.mBgDown },
                Name = name,
                Text = caption,
                Size = new Size(30, 30),
                Location = new Point(x, 0),
                AutoSize = false,
                BackgroundImage = image,
                BackgroundImageLayout = ImageLayout.Center,
                //ForeColor = Settings.Theme.mForeColor,
                //BackColor = Settings.Theme.mBackColor,
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Calibri", 13),
            };
            btn.Click += Btn_Click;
            
            //для Mono
            btn.MouseEnter += (s, e) => (s as Control).BackColor=Settings.Theme.mBgOver;
            btn.MouseLeave += (s, e) => (s as Control).BackColor=Settings.Theme.mBackColor;
            btn.MouseDown += (s, e) => (s as Control).BackColor = Settings.Theme.mBgDown;
            btn.MouseUp += (s, e) => (s as Control).BackColor = Settings.Theme.mBackColor;
            
            Controls.Add(btn);
            x += btn.Width;
        }

        void AddSeparator() {
            var separator = new Panel {
                Size = new Size(1, Height),
                Location = new Point(x, 0),
                BackColor = Settings.Theme.mBgOver,
                Name = "sep",
            };
            Controls.Add(separator);
            x += separator.Width;
        }

        private void Btn_Click(object sender, EventArgs e) {
            string name = ((Button)sender).Name;
            if (name != "sep") cl_d(name);
        }
    }
}
