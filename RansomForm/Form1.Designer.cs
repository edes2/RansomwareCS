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
      this.buttonDecrypt = new System.Windows.Forms.Button();
      this.label1a = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label2 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblCount
      // 
      this.lblCount.AutoSize = true;
      this.lblCount.BackColor = System.Drawing.Color.WhiteSmoke;
      this.lblCount.Location = new System.Drawing.Point(253, 70);
      this.lblCount.Name = "lblCount";
      this.lblCount.Size = new System.Drawing.Size(65, 15);
      this.lblCount.TabIndex = 0;
      this.lblCount.Text = "labelCount";
      // 
      // buttonDecrypt
      // 
      this.buttonDecrypt.Location = new System.Drawing.Point(216, 595);
      this.buttonDecrypt.Name = "buttonDecrypt";
      this.buttonDecrypt.Size = new System.Drawing.Size(272, 47);
      this.buttonDecrypt.TabIndex = 2;
      this.buttonDecrypt.Text = "Decrypt";
      this.buttonDecrypt.UseVisualStyleBackColor = true;
      this.buttonDecrypt.Click += new System.EventHandler(this.buttonDecrypt_Click);
      // 
      // label1a
      // 
      this.label1a.AutoSize = true;
      this.label1a.BackColor = System.Drawing.Color.Transparent;
      this.label1a.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.label1a.Location = new System.Drawing.Point(25, 550);
      this.label1a.Name = "label1a";
      this.label1a.Size = new System.Drawing.Size(684, 32);
      this.label1a.TabIndex = 4;
      this.label1a.Text = "Place the private key file and click decrypt to retrieve your files";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::RansomForm.Properties.Resources.Padlock_PNG_Transparent_Image;
      this.pictureBox1.Location = new System.Drawing.Point(1, 1);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(216, 202);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 5;
      this.pictureBox1.TabStop = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.label1.Location = new System.Drawing.Point(243, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(473, 45);
      this.label1.TabIndex = 6;
      this.label1.Text = "Your files have been encrypted !";
      // 
      // textBox1
      // 
      this.textBox1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      this.textBox1.Location = new System.Drawing.Point(253, 88);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.textBox1.Size = new System.Drawing.Size(463, 447);
      this.textBox1.TabIndex = 7;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Location = new System.Drawing.Point(25, 218);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(200, 186);
      this.groupBox1.TabIndex = 8;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "How to retrive your files";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(5, 27);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(187, 80);
      this.label2.TabIndex = 0;
      this.label2.Text = "Send us bitcoins and we will unlock your files";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Firebrick;
      this.ClientSize = new System.Drawing.Size(739, 650);
      this.ControlBox = false;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.label1a);
      this.Controls.Add(this.buttonDecrypt);
      this.Controls.Add(this.lblCount);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MaximumSize = new System.Drawing.Size(755, 689);
      this.MinimizeBox = false;
      this.Name = "Form1";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Your files got encrypted !";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private Label lblCount;
        private Button buttonDecrypt;
        private Label labelMain;
    private Label label1a;
    private PictureBox pictureBox1;
    private Label label1;
    private TextBox textBox1;
    private GroupBox groupBox1;
    private Label label2;
  }
}