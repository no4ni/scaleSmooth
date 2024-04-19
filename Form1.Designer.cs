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
            components = new System.ComponentModel.Container();
            pictureBox1 = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            progressBar1 = new ProgressBar();
            numericUpDown1 = new NumericUpDown();
            label1 = new Label();
            button3 = new Button();
            button4 = new Button();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            button5 = new Button();
            trackBar1 = new TrackBar();
            label2 = new Label();
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
            button2.Location = new Point(11, 455);
            button2.Name = "button2";
            button2.Size = new Size(176, 54);
            button2.TabIndex = 2;
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // timer1
            // 
            timer1.Interval = 10;
            timer1.Tick += timer1_Tick;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 191);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(175, 23);
            progressBar1.TabIndex = 3;
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
            button3.Location = new Point(12, 249);
            button3.Name = "button3";
            button3.Size = new Size(176, 54);
            button3.TabIndex = 6;
            button3.Text = "scaleSmooth";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(11, 560);
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
            button5.Location = new Point(12, 309);
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
            trackBar1.Value = 50;
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 768);
            Controls.Add(label2);
            Controls.Add(trackBar1);
            Controls.Add(button5);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(numericUpDown1);
            Controls.Add(progressBar1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
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
        private System.Windows.Forms.Timer timer1;
        private ProgressBar progressBar1;
        private NumericUpDown numericUpDown1;
        private Label label1;
        private Button button3;
        private Button button4;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Button button5;
        private TrackBar trackBar1;
        private Label label2;
    }
}
