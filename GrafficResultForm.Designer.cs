namespace CG_Lab5
{
    partial class GrafficResultForm
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
            pictureBox1 = new PictureBox();
            choosePhotoBtn = new Button();
            addImageBtn = new Button();
            analyzeBtn = new Button();
            pictureBox2 = new PictureBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            textBox4 = new TextBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(31, 29);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1159, 387);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // choosePhotoBtn
            // 
            choosePhotoBtn.BackColor = SystemColors.MenuText;
            choosePhotoBtn.FlatAppearance.BorderSize = 0;
            choosePhotoBtn.ForeColor = SystemColors.ButtonHighlight;
            choosePhotoBtn.Location = new Point(1253, 44);
            choosePhotoBtn.Name = "choosePhotoBtn";
            choosePhotoBtn.Size = new Size(181, 54);
            choosePhotoBtn.TabIndex = 1;
            choosePhotoBtn.Text = "Выбрать фото для анализа";
            choosePhotoBtn.UseVisualStyleBackColor = false;
            choosePhotoBtn.Click += choosePhotoBtn_Click;
            // 
            // addImageBtn
            // 
            addImageBtn.BackColor = Color.Olive;
            addImageBtn.FlatAppearance.BorderSize = 0;
            addImageBtn.ForeColor = SystemColors.ButtonHighlight;
            addImageBtn.Location = new Point(1226, 320);
            addImageBtn.Name = "addImageBtn";
            addImageBtn.Size = new Size(241, 54);
            addImageBtn.TabIndex = 12;
            addImageBtn.Text = "Добавить изображение в базу";
            addImageBtn.UseVisualStyleBackColor = false;
            addImageBtn.Click += addImageBtn_Click;
            // 
            // analyzeBtn
            // 
            analyzeBtn.BackColor = Color.Magenta;
            analyzeBtn.FlatAppearance.BorderSize = 0;
            analyzeBtn.ForeColor = SystemColors.ButtonHighlight;
            analyzeBtn.Location = new Point(1253, 176);
            analyzeBtn.Name = "analyzeBtn";
            analyzeBtn.Size = new Size(181, 54);
            analyzeBtn.TabIndex = 15;
            analyzeBtn.Text = "Анализ изображения";
            analyzeBtn.UseVisualStyleBackColor = false;
            analyzeBtn.Click += analyzeBtn_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(31, 477);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(207, 160);
            pictureBox2.TabIndex = 16;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(406, 423);
            label4.Name = "label4";
            label4.Size = new Size(164, 35);
            label4.TabIndex = 17;
            label4.Text = "Топ похожих";
            label4.Click += label4_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(382, -9);
            label5.Name = "label5";
            label5.Size = new Size(358, 35);
            label5.TabIndex = 18;
            label5.Text = "Фото для анализа и результат";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 15F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(406, 652);
            label6.Name = "label6";
            label6.Size = new Size(124, 35);
            label6.TabIndex = 19;
            label6.Text = "Результат";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(12, 731);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(1141, 27);
            textBox4.TabIndex = 21;
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(358, 477);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(199, 160);
            pictureBox3.TabIndex = 22;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // pictureBox4
            // 
            pictureBox4.Location = new Point(652, 477);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(183, 160);
            pictureBox4.TabIndex = 23;
            pictureBox4.TabStop = false;
            // 
            // MainScreen
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1492, 798);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(textBox4);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(pictureBox2);
            Controls.Add(analyzeBtn);
            Controls.Add(addImageBtn);
            Controls.Add(choosePhotoBtn);
            Controls.Add(pictureBox1);
            Name = "MainScreen";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button choosePhotoBtn;
        private Button addImageBtn;
        private Button analyzeBtn;
        private PictureBox pictureBox2;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox textBox4;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
    }
}