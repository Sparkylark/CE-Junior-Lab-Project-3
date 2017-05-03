namespace Hangman
{
    partial class mainPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusPic = new System.Windows.Forms.PictureBox();
            this.wordLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.promptLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.statusPic)).BeginInit();
            this.SuspendLayout();
            // 
            // statusPic
            // 
            this.statusPic.Image = global::Hangman.Properties.Resources.State1;
            this.statusPic.Location = new System.Drawing.Point(13, 13);
            this.statusPic.Name = "statusPic";
            this.statusPic.Size = new System.Drawing.Size(371, 466);
            this.statusPic.TabIndex = 0;
            this.statusPic.TabStop = false;
            // 
            // wordLabel
            // 
            this.wordLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.wordLabel.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wordLabel.Location = new System.Drawing.Point(522, 83);
            this.wordLabel.Name = "wordLabel";
            this.wordLabel.Size = new System.Drawing.Size(377, 49);
            this.wordLabel.TabIndex = 1;
            this.wordLabel.Text = "No Game Active";
            this.wordLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // statusLabel
            // 
            this.statusLabel.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(390, 230);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(630, 28);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "No Game Active";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // promptLabel
            // 
            this.promptLabel.Font = new System.Drawing.Font("Trebuchet MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promptLabel.Location = new System.Drawing.Point(395, 336);
            this.promptLabel.Name = "promptLabel";
            this.promptLabel.Size = new System.Drawing.Size(625, 37);
            this.promptLabel.TabIndex = 3;
            this.promptLabel.Text = "Start New Game? [Y]";
            this.promptLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mainPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1032, 491);
            this.Controls.Add(this.promptLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.wordLabel);
            this.Controls.Add(this.statusPic);
            this.MaximumSize = new System.Drawing.Size(1048, 530);
            this.MinimumSize = new System.Drawing.Size(1048, 530);
            this.Name = "mainPanel";
            this.Text = "Hangman";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mainPanel_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.statusPic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox statusPic;
        private System.Windows.Forms.Label wordLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label promptLabel;
    }
}

