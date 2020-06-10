namespace Scacchi
{
    partial class Welcome
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
            this.NewGame = new System.Windows.Forms.Button();
            this.Exit = new System.Windows.Forms.PictureBox();
            this.ExpertMode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Exit)).BeginInit();
            this.SuspendLayout();
            // 
            // NewGame
            // 
            this.NewGame.BackColor = System.Drawing.SystemColors.Window;
            this.NewGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewGame.Location = new System.Drawing.Point(126, 252);
            this.NewGame.Name = "NewGame";
            this.NewGame.Size = new System.Drawing.Size(107, 35);
            this.NewGame.TabIndex = 1;
            this.NewGame.UseVisualStyleBackColor = false;
            this.NewGame.Click += new System.EventHandler(this.NewGame_Click);
            // 
            // Exit
            // 
            this.Exit.BackColor = System.Drawing.Color.Transparent;
            this.Exit.Location = new System.Drawing.Point(12, 12);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(37, 39);
            this.Exit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Exit.TabIndex = 86;
            this.Exit.TabStop = false;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // ExpertMode
            // 
            this.ExpertMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.ExpertMode.AutoSize = true;
            this.ExpertMode.BackColor = System.Drawing.SystemColors.Window;
            this.ExpertMode.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ExpertMode.Cursor = System.Windows.Forms.Cursors.Default;
            this.ExpertMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExpertMode.Location = new System.Drawing.Point(252, 252);
            this.ExpertMode.Name = "ExpertMode";
            this.ExpertMode.Size = new System.Drawing.Size(107, 35);
            this.ExpertMode.TabIndex = 88;
            this.ExpertMode.Text = "EXPERT";
            this.ExpertMode.UseVisualStyleBackColor = false;
            // 
            // Welcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(484, 461);
            this.Controls.Add(this.ExpertMode);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.NewGame);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Welcome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome";
            ((System.ComponentModel.ISupportInitialize)(this.Exit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button NewGame;
        private System.Windows.Forms.PictureBox Exit;
        public System.Windows.Forms.CheckBox ExpertMode;
    }
}