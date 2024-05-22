namespace ScaleSmooth
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
            pictureBox1 = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            numericUpDown1 = new NumericUpDown();
            label1 = new Label();
            button3 = new Button();
            button4 = new Button();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            button5 = new Button();
            trackBar1 = new TrackBar();
            label2 = new Label();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            ProgressText = new Label();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Image = Properties.Resources.A8;
            pictureBox1.Location = new Point(194, -1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(768, 768);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(176, 54);
            button1.TabIndex = 1;
            button1.Text = "Load";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Location = new Point(11, 596);
            button2.Name = "button2";
            button2.Size = new Size(176, 54);
            button2.TabIndex = 2;
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(78, 72);
            numericUpDown1.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(52, 23);
            numericUpDown1.TabIndex = 4;
            numericUpDown1.TextAlign = HorizontalAlignment.Right;
            numericUpDown1.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(39, 74);
            label1.Name = "label1";
            label1.Size = new Size(103, 15);
            label1.TabIndex = 5;
            label1.Text = "Scale:                    x";
            // 
            // button3
            // 
            button3.Location = new Point(12, 196);
            button3.Name = "button3";
            button3.Size = new Size(176, 54);
            button3.TabIndex = 6;
            button3.Text = "scaleSmooth";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(11, 656);
            button4.Name = "button4";
            button4.Size = new Size(176, 54);
            button4.TabIndex = 7;
            button4.Text = "Close";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(13, 91);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(49, 19);
            radioButton1.TabIndex = 8;
            radioButton1.TabStop = true;
            radioButton1.Text = "Gray";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.CheckAlign = ContentAlignment.MiddleRight;
            radioButton2.Location = new Point(134, 91);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(54, 19);
            radioButton2.TabIndex = 9;
            radioButton2.Text = "Color";
            radioButton2.TextAlign = ContentAlignment.MiddleRight;
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(13, 316);
            button5.Name = "button5";
            button5.Size = new Size(176, 54);
            button5.TabIndex = 10;
            button5.Text = "scaleRough";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(13, 116);
            trackBar1.Maximum = 100;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(174, 45);
            trackBar1.TabIndex = 11;
            trackBar1.TickFrequency = 25;
            trackBar1.Value = 42;
            // 
            // label2
            // 
            label2.Location = new Point(13, 146);
            label2.Name = "label2";
            label2.Size = new Size(174, 15);
            label2.TabIndex = 12;
            label2.Text = "Fast                       Accurate";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button6
            // 
            button6.Location = new Point(12, 256);
            button6.Name = "button6";
            button6.Size = new Size(176, 54);
            button6.TabIndex = 13;
            button6.Text = "scaleFurry";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button7
            // 
            button7.Location = new Point(13, 376);
            button7.Name = "button7";
            button7.Size = new Size(176, 54);
            button7.TabIndex = 14;
            button7.Text = "contrastBoldScale";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // button8
            // 
            button8.Location = new Point(13, 436);
            button8.Name = "button8";
            button8.Size = new Size(176, 54);
            button8.TabIndex = 15;
            button8.Text = "scaleSeparate";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button9
            // 
            button9.Location = new Point(13, 496);
            button9.Name = "button9";
            button9.Size = new Size(176, 54);
            button9.TabIndex = 16;
            button9.Text = "scaleBilinearInterExtra";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // ProgressText
            // 
            ProgressText.Location = new Point(143, 178);
            ProgressText.Name = "ProgressText";
            ProgressText.Size = new Size(25, 15);
            ProgressText.TabIndex = 17;
            ProgressText.Text = "100";
            ProgressText.TextAlign = ContentAlignment.MiddleRight;
            ProgressText.Visible = false;
            ProgressText.TextChanged += Progress;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(20, 178);
            label4.Name = "label4";
            label4.Size = new Size(161, 15);
            label4.TabIndex = 18;
            label4.Text = "Calculating...                         %";
            label4.Visible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 768);
            Controls.Add(ProgressText);
            Controls.Add(button9);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(label2);
            Controls.Add(trackBar1);
            Controls.Add(button5);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(numericUpDown1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(label4);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "scaleSmooth";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button button1;
        private Button button2;
        private NumericUpDown numericUpDown1;
        private Label label1;
        private Button button3;
        private Button button4;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Button button5;
        private TrackBar trackBar1;
        private Label label2;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Label ProgressText;
        private Label label4;
    }
}
