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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pictureBox1 = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            numericUpDown1 = new NumericUpDown();
            label1 = new Label();
            button3 = new Button();
            button4 = new Button();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            trackBar1 = new TrackBar();
            label2 = new Label();
            ProgressText = new Label();
            label4 = new Label();
            comboBox1 = new ComboBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox5 = new PictureBox();
            label6 = new Label();
            checkBox1 = new CheckBox();
            button5 = new Button();
            StopWatchLabel = new Label();
            toolTip1 = new ToolTip(components);
            checkBox2 = new CheckBox();
            label3 = new Label();
            ETA = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Image = Properties.Resources.A8;
            pictureBox1.Location = new Point(194, -1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(769, 768);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.MouseDoubleClick += ToScreen;
            pictureBox1.MouseDown += DragNDrop;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 12F);
            button1.Image = Properties.Resources.load;
            button1.Location = new Point(5, 5);
            button1.Name = "button1";
            button1.Size = new Size(183, 54);
            button1.TabIndex = 1;
            button1.Text = Strings.LoadButton;
            button1.TextAlign = ContentAlignment.MiddleRight;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Font = new Font("Segoe UI", 12F);
            button2.Location = new Point(6, 596);
            button2.Name = "button2";
            button2.Size = new Size(185, 54);
            button2.TabIndex = 2;
            button2.Text = Strings.SaveButton;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(87, 383);
            numericUpDown1.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(43, 23);
            numericUpDown1.TabIndex = 4;
            numericUpDown1.TextAlign = HorizontalAlignment.Right;
            numericUpDown1.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 10F);
            label1.Location = new Point(0, 382);
            label1.Name = "label1";
            label1.Size = new Size(190, 21);
            label1.TabIndex = 5;
            label1.Text = "Scale:            x";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.MouseDoubleClick += ToScreen;
            label1.MouseDown += DragNDrop;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 12F);
            button3.Location = new Point(5, 501);
            button3.Name = "button3";
            button3.Size = new Size(185, 54);
            button3.TabIndex = 6;
            button3.Text = Strings.Upscale;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Font = new Font("Segoe UI", 12F);
            button4.Location = new Point(5, 665);
            button4.Name = "button4";
            button4.Size = new Size(185, 54);
            button4.TabIndex = 7;
            button4.Text = Strings.CloseButton;
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(16, 406);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(49, 19);
            radioButton1.TabIndex = 8;
            radioButton1.TabStop = true;
            radioButton1.Text = Strings.GrayImage;
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.CheckAlign = ContentAlignment.MiddleRight;
            radioButton2.Location = new Point(87, 406);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(90, 19);
            radioButton2.TabIndex = 9;
            radioButton2.Text = Strings.Color;
            radioButton2.TextAlign = ContentAlignment.MiddleRight;
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(6, 431);
            trackBar1.Maximum = 100;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(182, 45);
            trackBar1.TabIndex = 11;
            trackBar1.TickFrequency = 25;
            trackBar1.Value = 42;
            trackBar1.Scroll += trackBar1_Scroll;
            trackBar1.KeyPress += DefaultIs42;
            // 
            // label2
            // 
            label2.Location = new Point(6, 451);
            label2.Name = "label2";
            label2.Size = new Size(183, 16);
            label2.TabIndex = 12;
            label2.Text = "Fast                       Accurate";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.MouseDoubleClick += ToScreen;
            label2.MouseDown += DragNDrop;
            // 
            // ProgressText
            // 
            ProgressText.Location = new Point(139, 561);
            ProgressText.Margin = new Padding(0);
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
            label4.Location = new Point(16, 561);
            label4.Name = "label4";
            label4.Size = new Size(161, 15);
            label4.TabIndex = 18;
            label4.Text = "Calculating...                         %";
            label4.Visible = false;
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Font = new Font("Segoe UI", 10F);
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "FastNearestNeighbour", "Smooth", "AntiBicubic", "SmoothContinuous", "SmoothContrast", "BA", "Bold", "BASmoothContrast", "BAContrast", "BAMonochrome2", "ContrastBold", "BAMonochrome", "Separate", "SmoothCAS", "255BA", "DerivativeBA", "Thin255BA", "BAExtremum", "Furry", "Rough" });
            comboBox1.Location = new Point(6, 64);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(183, 25);
            comboBox1.TabIndex = 21;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.shortBil;
            pictureBox2.Location = new Point(0, 282);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(97, 97);
            pictureBox2.TabIndex = 22;
            pictureBox2.TabStop = false;
            pictureBox2.MouseDoubleClick += ToScreen;
            pictureBox2.MouseDown += DragNDrop;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.shortSmooth;
            pictureBox3.Location = new Point(97, 282);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(97, 97);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 23;
            pictureBox3.TabStop = false;
            pictureBox3.MouseDoubleClick += ToScreen;
            pictureBox3.MouseDown += DragNDrop;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.arRight;
            pictureBox4.Location = new Point(172, 95);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(17, 12);
            pictureBox4.TabIndex = 24;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = Properties.Resources.arLeft;
            pictureBox5.Location = new Point(6, 95);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(17, 12);
            pictureBox5.TabIndex = 25;
            pictureBox5.TabStop = false;
            pictureBox5.Click += pictureBox5_Click;
            // 
            // label6
            // 
            label6.Font = new Font("Segoe UI", 8F);
            label6.Location = new Point(0, 110);
            label6.Name = "label6";
            label6.Size = new Size(194, 171);
            label6.TabIndex = 26;
            label6.TextAlign = ContentAlignment.MiddleCenter;
            label6.TextChanged += Warning;
            label6.MouseDoubleClick += ToScreen;
            label6.MouseDown += DragNDrop;
            // 
            // checkBox1
            // 
            checkBox1.Font = new Font("Segoe UI", 9F);
            checkBox1.Location = new Point(41, 95);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(118, 19);
            checkBox1.TabIndex = 27;
            checkBox1.Text = Strings.GPUacceleration;
            checkBox1.TextAlign = ContentAlignment.MiddleCenter;
            checkBox1.ThreeState = true;
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.Visible = false;
            // 
            // button5
            // 
            button5.Image = Properties.Resources.restore;
            button5.ImageAlign = ContentAlignment.MiddleLeft;
            button5.Location = new Point(8, 8);
            button5.Name = "button5";
            button5.Size = new Size(73, 23);
            button5.TabIndex = 28;
            button5.Text = Strings.RestoreButton;
            button5.TextAlign = ContentAlignment.MiddleRight;
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // StopWatchLabel
            // 
            StopWatchLabel.Location = new Point(6, 554);
            StopWatchLabel.Name = "StopWatchLabel";
            StopWatchLabel.Size = new Size(182, 29);
            StopWatchLabel.TabIndex = 29;
            StopWatchLabel.Text = "Stopwatch";
            StopWatchLabel.TextAlign = ContentAlignment.MiddleCenter;
            StopWatchLabel.Visible = false;
            StopWatchLabel.MouseClick += CopyStopwatch;
            StopWatchLabel.MouseDoubleClick += ToScreen;
            StopWatchLabel.MouseDown += DragNDrop;
            // 
            // toolTip1
            // 
            toolTip1.AutomaticDelay = 2000;
            toolTip1.AutoPopDelay = 4000;
            toolTip1.InitialDelay = 2000;
            toolTip1.ReshowDelay = 400;
            toolTip1.UseAnimation = false;
            toolTip1.UseFading = false;
            // 
            // checkBox2
            // 
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Font = new Font("Segoe UI", 9F);
            checkBox2.Location = new Point(25, 478);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(158, 19);
            checkBox2.TabIndex = 30;
            checkBox2.Text = Strings.Notification;
            checkBox2.TextAlign = ContentAlignment.MiddleCenter;
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(552, 5);
            label3.Name = "label3";
            label3.Size = new Size(31, 15);
            label3.TabIndex = 31;
            label3.Text = "10x8";
            // 
            // ETA
            // 
            ETA.Location = new Point(8, 578);
            ETA.Name = "ETA";
            ETA.Size = new Size(181, 15);
            ETA.TabIndex = 32;
            ETA.Text = "ETA: 0:00:20:35";
            ETA.TextAlign = ContentAlignment.MiddleCenter;
            ETA.Visible = false;
            label1.Text = Strings.Scale;
            label2.Text = Strings.Fast + "                       " + Strings.Accurate;
            label4.Text = Strings.Wait;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 768);
            Controls.Add(ETA);
            Controls.Add(label3);
            Controls.Add(checkBox2);
            Controls.Add(StopWatchLabel);
            Controls.Add(button5);
            Controls.Add(checkBox1);
            Controls.Add(label6);
            Controls.Add(pictureBox5);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(comboBox1);
            Controls.Add(ProgressText);
            Controls.Add(label2);
            Controls.Add(trackBar1);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(numericUpDown1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(label4);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "scaleSmooth";
            Load += Form1_Load;
            MouseDoubleClick += ToScreen;
            MouseDown += DragNDrop;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
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
        private TrackBar trackBar1;
        private Label label2;
        private Label ProgressText;
        private Label label4;
        private ComboBox comboBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private Label label6;
        private CheckBox checkBox1;
        private Button button5;
        private Label StopWatchLabel;
        private ToolTip toolTip1;
        private CheckBox checkBox2;
        private Label label3;
        private Label ETA;
    }
    
}
