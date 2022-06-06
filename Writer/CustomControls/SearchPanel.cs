using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CustomControls {
    public partial class SearchPanel : UserControl {
        public bool EnRegex { get { return chRegex.Checked; } set { chRegex.Checked = value; } }
        public bool EnIgnoreCase { get { return chIgnoreCase.Checked; } set { chIgnoreCase.Checked = value; } }
        private RichTextBox tb;

        private System.Windows.Forms.CheckBox chIgnoreCase;
        private System.Windows.Forms.CheckBox chRegex;
        private System.Windows.Forms.TextBox tbSearchText;
        private System.Windows.Forms.Button btnFindNext, btnClose;
        private System.Windows.Forms.Button btnFindPrev;

        void find(bool findNext) {
            string text = tb.Text;
            string textForSearch = tbSearchText.Text;
            int len = 0;

            if (EnIgnoreCase) {
                text = text.ToUpper();
                textForSearch = textForSearch.ToUpper();
            }

            int ind = -1;

            if (EnRegex) {
                try {
                    var regex = new Regex(textForSearch);

                    Match match = regex.Match(text, tb.SelectionStart + tb.SelectionLength);



                    if (match == null) {
                        match = regex.Match(text, tb.SelectionStart + tb.SelectionLength);
                    }

                    if (match != null) {
                        len = match.Length;
                        ind = match.Index;
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else {
                ind = findNext ? text.IndexOf(textForSearch, tb.SelectionStart + tb.SelectionLength) :
                                 text.LastIndexOf(textForSearch);

                if (ind == -1) {
                    ind = findNext ? text.IndexOf(textForSearch, 0) :
                                     text.LastIndexOf(textForSearch);
                }

                len = tbSearchText.TextLength;
            }

            if (ind != -1) {
                //tb.Select(ind, textForSearch.Length);
                tb.SelectionStart = ind;
                tb.SelectionLength = len;
            }
        }

        public SearchPanel(RichTextBox tb) {
            InitializeComponent();
            this.tb = tb;

            Name = "SearchPanel";
            this.Dock = DockStyle.Bottom;

            btnFindNext.Click += (s, e) => find(true);
            btnFindPrev.Click += (s, e) => find(false);

            btnClose.Click += (s, e) => {
                this.Dispose();
            };
        }

        private void InitializeComponent() {
            this.chIgnoreCase = new System.Windows.Forms.CheckBox();
            this.chRegex = new System.Windows.Forms.CheckBox();
            this.tbSearchText = new System.Windows.Forms.TextBox();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnFindPrev = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chIgnoreCase
            // 
            this.chIgnoreCase.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chIgnoreCase.Location = new System.Drawing.Point(12, 41);
            this.chIgnoreCase.Name = "chIgnoreCase";
            this.chIgnoreCase.Size = new System.Drawing.Size(155, 17);
            this.chIgnoreCase.TabIndex = 10;
            this.chIgnoreCase.Text = "Игнорировать регистр";
            this.chIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // chRegex
            // 
            this.chRegex.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chRegex.Location = new System.Drawing.Point(12, 18);
            this.chRegex.Name = "chRegex";
            this.chRegex.Size = new System.Drawing.Size(141, 24);
            this.chRegex.TabIndex = 6;
            this.chRegex.Text = "Regex";
            this.chRegex.UseVisualStyleBackColor = true;
            // 
            // tbSearchText
            // 
            this.tbSearchText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tbSearchText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbSearchText.Location = new System.Drawing.Point(171, 26);
            this.tbSearchText.Name = "tbSearchText";
            this.tbSearchText.Size = new System.Drawing.Size(264, 30);
            this.tbSearchText.TabIndex = 9;
            // 
            // btnFindNext
            // 
            this.btnFindNext.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnFindNext.FlatAppearance.BorderSize = 0;
            this.btnFindNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFindNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnFindNext.Location = new System.Drawing.Point(441, 25);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(127, 32);
            this.btnFindNext.TabIndex = 8;
            this.btnFindNext.Text = "Find";
            this.btnFindNext.UseVisualStyleBackColor = false;
            // 
            // btnFindPrev
            // 
            this.btnFindPrev.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnFindPrev.FlatAppearance.BorderSize = 0;
            this.btnFindPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFindPrev.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnFindPrev.Location = new System.Drawing.Point(563, 25);
            this.btnFindPrev.Name = "btnFindPrev";
            this.btnFindPrev.Size = new System.Drawing.Size(127, 32);
            this.btnFindPrev.TabIndex = 7;
            this.btnFindPrev.Text = "Find Prev";
            this.btnFindPrev.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            this.btnClose = new Button();
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            btnClose.ForeColor = Color.White;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClose.Location = new System.Drawing.Point(703, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(30, 30);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // SearchPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.chIgnoreCase);
            this.Controls.Add(this.chRegex);
            this.Controls.Add(this.tbSearchText);
            this.Controls.Add(this.btnFindNext);
            this.Controls.Add(this.btnFindPrev);
            this.Controls.Add(this.btnClose);
            this.Name = "SearchPanel";
            this.Size = new System.Drawing.Size(733, 76);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
