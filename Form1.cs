using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;



namespace ScaleSmooth
{
    public partial class Form1 : Form
    {

        string pp="";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }


        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image|*.png;*.jpg;*.bmp;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(@openFileDialog.FileName);
                    this.Text = Path.GetFileNameWithoutExtension(@openFileDialog.FileName);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new() { Filter = @"PNG|*.png" })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(saveFileDialog.FileName);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            if (radioButton1.Checked)
            {
                pictureBox1.Image = ScaleSmoothGray(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }
            else
            {
                pictureBox1.Image = ScaleSmoothColor(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }

            ProgressText.Text = 100.ToString();

            pictureBox1.Refresh();
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
        }

        private Image ScaleSmoothGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, xmm, xoim, nim, nsm;
            xm = x - 1;
            xmm = xm - 1;
            ni = img.Width * x;
            ns = img.Height * x;
            oi = img.Width;
            os = img.Height;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoim = oim * x;

            ac = ac * (oim - 1) / 100;
            ac = x * (ac + 1);
            osm = os - 1;

            byte[,] d = new byte[ni, ns];
            byte[,] sr = new byte[img.Width, img.Height];
            Random rnd = new Random();

            for (int i = 0; i < oi; i++)
            {
                for (int s = 0; s < os; s++)
                {
                    sr[i, s] = ((Bitmap)img).GetPixel(i, s).R;
                }
            }


            img = new Bitmap(img, new Size(ni, ns));
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[i / x, s / x];
                }
            }

            int r, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    for (int sx = osm; sx > -1; sx--)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1] + d[i - 1, s - 1] + d[i + 1, s + 1] + d[i + 1, s - 1] + d[i - 1, s + 1]) / 8 + 0.5);
                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] > 0)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] < 255)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s]) / 2 + 0.5);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] > 0)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    for (int sx = 0; sx < os; sx++)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1]) / 4 + 0.5);
                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] > 0)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] < 255)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i, s - 1] + d[i, s + 1]) / 2 + 0.5);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] > 0)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();

            }
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb(d[i, s], d[i, s], d[i, s]));
                }
            }
            return img;
        }

        private Image ScaleSmoothColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, xmm, xoim, nim, nsm;
            xm = x - 1;
            xmm = xm - 1;
            ni = img.Width * x;
            ns = img.Height * x;
            oi = img.Width;
            os = img.Height;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoim = oim * x;
            ac = ac * (oim - 1) / 100;
            ac = x * (ac + 1);
            osm = os - 1;

            byte[,] r = new byte[ni, ns];
            byte[,] g = new byte[ni, ns];
            byte[,] b = new byte[ni, ns];
            byte[,] sr = new byte[img.Width, img.Height];
            byte[,] sg = new byte[img.Width, img.Height];
            byte[,] sb = new byte[img.Width, img.Height];
            Random rnd = new Random();

            for (int i = 0; i < oi; i++)
            {
                for (int s = 0; s < os; s++)
                {
                    sr[i, s] = ((Bitmap)img).GetPixel(i, s).R;
                    sg[i, s] = ((Bitmap)img).GetPixel(i, s).G;
                    sb[i, s] = ((Bitmap)img).GetPixel(i, s).B;
                }
            }


            img = new Bitmap(img, new Size(ni, ns));
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    r[i, s] = sr[i / x, s / x];
                    g[i, s] = sg[i / x, s / x];
                    b[i, s] = sb[i / x, s / x];
                }
            }

            int rn, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    for (int sx = osm; sx > -1; sx--)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = r[i, s];
                                            r[i, s] = (byte)((r[i - 1, s] + r[i + 1, s] + r[i, s - 1] + r[i, s + 1] + r[i - 1, s - 1] + r[i + 1, s + 1] + r[i + 1, s - 1] + r[i - 1, s + 1]) / 8 + 0.5);
                                            rn = r[i, s] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (r[ii, ss] > 0)
                                                    {
                                                        r[ii, ss] = (byte)(r[ii, ss] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (r[ii, ss] < 255)
                                                    {
                                                        r[ii, ss] = (byte)(r[ii, ss] + 1);
                                                        rn++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {

                                        ld = r[i, s];
                                        r[i, s] = (byte)((r[i - 1, s] + r[i + 1, s]) / 2 + 0.5);
                                        rn = r[i, s] - ld;
                                        if (rn > 0)
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (r[ii, ss] > 0)
                                                {
                                                    r[ii, ss] = (byte)(r[ii, ss] - 1);
                                                    rn--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);

                                                if (r[ii, ss] < 255)
                                                {
                                                    r[ii, ss] = (byte)(r[ii, ss] + 1);
                                                    rn++;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        if (sg[ix, sx] > 0 && sg[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = g[i, s];
                                            g[i, s] = (byte)((g[i - 1, s] + g[i + 1, s] + g[i, s - 1] + g[i, s + 1] + g[i - 1, s - 1] + g[i + 1, s + 1] + g[i + 1, s - 1] + g[i - 1, s + 1]) / 8 + 0.5);
                                            rn = g[i, s] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (g[ii, ss] > 0)
                                                    {
                                                        g[ii, ss] = (byte)(g[ii, ss] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (g[ii, ss] < 255)
                                                    {
                                                        g[ii, ss] = (byte)(g[ii, ss] + 1);
                                                        rn++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = g[i, s];
                                        g[i, s] = (byte)((g[i - 1, s] + g[i + 1, s]) / 2 + 0.5);
                                        rn = g[i, s] - ld;
                                        if (rn > 0)
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (g[ii, ss] > 0)
                                                {
                                                    g[ii, ss] = (byte)(g[ii, ss] - 1);
                                                    rn--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);

                                                if (g[ii, ss] < 255)
                                                {
                                                    g[ii, ss] = (byte)(g[ii, ss] + 1);
                                                    rn++;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        if (sb[ix, sx] > 0 && sb[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = b[i, s];
                                            b[i, s] = (byte)((b[i - 1, s] + b[i + 1, s] + b[i, s - 1] + b[i, s + 1] + b[i - 1, s - 1] + b[i + 1, s + 1] + b[i + 1, s - 1] + b[i - 1, s + 1]) / 8 + 0.5);
                                            rn = b[i, s] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (b[ii, ss] > 0)
                                                    {
                                                        b[ii, ss] = (byte)(b[ii, ss] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (b[ii, ss] < 255)
                                                    {
                                                        b[ii, ss] = (byte)(b[ii, ss] + 1);
                                                        rn++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = b[i, s];
                                        b[i, s] = (byte)((b[i - 1, s] + b[i + 1, s]) / 2 + 0.5);
                                        rn = b[i, s] - ld;
                                        if (rn > 0)
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (b[ii, ss] > 0)
                                                {
                                                    b[ii, ss] = (byte)(b[ii, ss] - 1);
                                                    rn--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (b[ii, ss] < 255)
                                                {
                                                    b[ii, ss] = (byte)(b[ii, ss] + 1);
                                                    rn++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    for (int sx = 0; sx < os; sx++)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = r[i, s];
                                            r[i, s] = (byte)((r[i - 1, s] + r[i + 1, s] + r[i, s - 1] + r[i, s + 1]) / 4 + 0.5);
                                            rn = r[i, s] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (r[ii, ss] > 0)
                                                    {
                                                        r[ii, ss] = (byte)(r[ii, ss] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);

                                                    if (r[ii, ss] < 255)
                                                    {
                                                        r[ii, ss] = (byte)(r[ii, ss] + 1);
                                                        rn++;
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = r[i, s];
                                        r[i, s] = (byte)((r[i, s - 1] + r[i, s + 1]) / 2 + 0.5);
                                        rn = r[i, s] - ld;
                                        if (rn > 0)
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (r[ii, ss] > 0)
                                                {
                                                    r[ii, ss] = (byte)(r[ii, ss] - 1);
                                                    rn--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);

                                                if (r[ii, ss] < 255)
                                                {
                                                    r[ii, ss] = (byte)(r[ii, ss] + 1);
                                                    rn++;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        if (sg[ix, sx] > 0 && sg[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {

                                            ld = g[i, s];
                                            g[i, s] = (byte)((g[i - 1, s] + g[i + 1, s] + g[i, s - 1] + g[i, s + 1]) / 4 + 0.5);
                                            rn = g[i, s] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (g[ii, ss] > 0)
                                                    {
                                                        g[ii, ss] = (byte)(g[ii, ss] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);

                                                    if (g[ii, ss] < 255)
                                                    {
                                                        g[ii, ss] = (byte)(g[ii, ss] + 1);
                                                        rn++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {

                                        ld = g[i, s];
                                        g[i, s] = (byte)((g[i, s - 1] + g[i, s + 1]) / 2 + 0.5);
                                        rn = g[i, s] - ld;
                                        if (rn > 0)
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (g[ii, ss] > 0)
                                                {
                                                    g[ii, ss] = (byte)(g[ii, ss] - 1);
                                                    rn--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);

                                                if (g[ii, ss] < 255)
                                                {
                                                    g[ii, ss] = (byte)(g[ii, ss] + 1);
                                                    rn++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (sb[ix, sx] > 0 && sb[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {

                                            ld = b[i, s];
                                            b[i, s] = (byte)((b[i - 1, s] + b[i + 1, s] + b[i, s - 1] + b[i, s + 1]) / 4 + 0.5);
                                            rn = b[i, s] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (b[ii, ss] > 0)
                                                    {
                                                        b[ii, ss] = (byte)(b[ii, ss] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);

                                                    if (b[ii, ss] < 255)
                                                    {
                                                        b[ii, ss] = (byte)(b[ii, ss] + 1);
                                                        rn++;
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {

                                        ld = b[i, s];
                                        b[i, s] = (byte)((b[i, s - 1] + b[i, s + 1]) / 2 + 0.5);
                                        rn = b[i, s] - ld;
                                        if (rn > 0)
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (b[ii, ss] > 0)
                                                {
                                                    b[ii, ss] = (byte)(b[ii, ss] - 1);
                                                    rn--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rn != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (b[ii, ss] < 255)
                                                {
                                                    b[ii, ss] = (byte)(b[ii, ss] + 1);
                                                    rn++;
                                                }

                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();

            }
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb(r[i, s], g[i, s], b[i, s]));
                }
            }
            return img;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            if (radioButton1.Checked)
            {
                pictureBox1.Image = ScaleRoughGray(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }
            else
            {
                pictureBox1.Image = ScaleRoughColor(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }

            ProgressText.Text = "100";

            pictureBox1.Refresh();
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
        }

        private Image ScaleRoughGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, xmm, xoim, nim, nsm, ac2;
            xm = x - 1;
            xmm = xm - 1;
            ni = img.Width * x;
            ns = img.Height * x;
            oi = img.Width;
            os = img.Height;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoim = oim * x;
            ac = ac * (oim - 1) / 100;
            ac = x * (ac + 1);
            ac2 = ac / 2;
            osm = os - 1;

            byte[,] d = new byte[ni, ns];
            byte[,] sr = new byte[img.Width, img.Height];
            Random rnd = new Random();

            for (int i = 0; i < oi; i++)
            {
                for (int s = 0; s < os; s++)
                {
                    sr[i, s] = ((Bitmap)img).GetPixel(i, s).R;
                }
            }


            img = new Bitmap(img, new Size(ni, ns));
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[i / x, s / x];
                }
            }

            int r, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    for (int sx = osm; sx > -1; sx--)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = d[i, s];
                                            if (ac2 <= c)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                }
                                                else
                                                {
                                                    d[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] > 0)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] < 255)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2));
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] > 0)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    for (int sx = 0; sx < os; sx++)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];

                                            d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s] + d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 4));


                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] > 0)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] < 255)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)(S255((d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 2));
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] > 0)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();

            }
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb(d[i, s], d[i, s], d[i, s]));
                }
            }
            return img;
        }

        private Image ScaleFurryGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, ac2;
            xm = x - 1;
            ni = img.Width * x;
            ns = img.Height * x;
            oi = img.Width;
            os = img.Height;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = ac * (oim - 1) / 100;
            ac = x * (ac + 1);
            ac2 = ac / 2;
            osm = os - 1;

            byte[,] d = new byte[ni, ns];
            float[,] ds = new float[ni, ns];
            byte[,] sr = new byte[img.Width, img.Height];
            Random rnd = new Random();

            for (int i = 0; i < oi; i++)
            {
                for (int s = 0; s < os; s++)
                {
                    sr[i, s] = ((Bitmap)img).GetPixel(i, s).R;
                }
            }


            img = new Bitmap(img, new Size(ni, ns));
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[i / x, s / x];
                    ds[i, s] = d[i, s];
                }
            }

            int r, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    for (int sx = osm; sx > -1; sx--)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = d[i, s];
                                            if (ac2 >= c)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                }
                                                else
                                                {
                                                    d[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }

                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] > 0)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] < 255)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            ds[i, s] = (ds[i, s] * (c - ac2) + d[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2));
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] > 0)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            ds[i, s] = (ds[i, s] * (c - ac2) + d[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    for (int sx = 0; sx < os; sx++)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];

                                            d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s] + d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 4));


                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] > 0)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] < 255)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            ds[i, s] = (ds[i, s] * (c - ac2) + d[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)(S255((d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 2));
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] > 0)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                    if (c >= ac2)
                                    {
                                        ds[i, s] = (ds[i, s] * (c - ac2) + d[i, s]) / (c - ac2 + 1);
                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }

            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb((byte)ds[i, s], (byte)ds[i, s], (byte)ds[i, s]));
                }
            }
            return img;
        }


        private Image ScaleRoughColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, xmm, xoim, nim, nsm, ac2;
            xm = x - 1;
            xmm = xm - 1;
            ni = img.Width * x;
            ns = img.Height * x;
            oi = img.Width;
            os = img.Height;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoim = oim * x;
            ac = ac * (oim - 1) / 100;
            ac = x * (ac + 1);
            ac2 = ac / 2;
            osm = os - 1;

            byte[,] dr = new byte[ni, ns];
            byte[,] dg = new byte[ni, ns];
            byte[,] db = new byte[ni, ns];
            byte[,] sr = new byte[img.Width, img.Height];
            byte[,] sg = new byte[img.Width, img.Height];
            byte[,] sb = new byte[img.Width, img.Height];
            Random rnd = new Random();

            for (int i = 0; i < oi; i++)
            {
                for (int s = 0; s < os; s++)
                {
                    sr[i, s] = ((Bitmap)img).GetPixel(i, s).R;
                    sg[i, s] = ((Bitmap)img).GetPixel(i, s).G;
                    sb[i, s] = ((Bitmap)img).GetPixel(i, s).B;
                }
            }


            img = new Bitmap(img, new Size(ni, ns));
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    dr[i, s] = sr[i / x, s / x];
                    dg[i, s] = sg[i / x, s / x];
                    db[i, s] = sb[i / x, s / x];
                }
            }

            int r, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    for (int sx = osm; sx > -1; sx--)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = dr[i, s];
                                            if (ac2 <= c)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    dr[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                }
                                                else
                                                {
                                                    dr[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                dr[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            r = dr[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dr[ii, ss] > 0)
                                                    {
                                                        dr[ii, ss] = (byte)(dr[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dr[ii, ss] < 255)
                                                    {
                                                        dr[ii, ss] = (byte)(dr[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = dr[i, s];
                                        dr[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s] + dr[i + rnd.Next(-1, 2), s]) / 2));
                                        r = dr[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dr[ii, ss] > 0)
                                                {
                                                    dr[ii, ss] = (byte)(dr[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dr[ii, ss] < 255)
                                                {
                                                    dr[ii, ss] = (byte)(dr[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        if (sg[ix, sx] > 0 && sg[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = dg[i, s];
                                            if (ac2 <= c)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    dg[i, s] = (byte)(S255((dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                }
                                                else
                                                {
                                                    dg[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                dg[i, s] = (byte)(S255((dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            r = dg[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dg[ii, ss] > 0)
                                                    {
                                                        dg[ii, ss] = (byte)(dg[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dg[ii, ss] < 255)
                                                    {
                                                        dg[ii, ss] = (byte)(dg[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = dg[i, s];
                                        dg[i, s] = (byte)(S255((dg[i + rnd.Next(-1, 2), s] + dg[i + rnd.Next(-1, 2), s]) / 2));
                                        r = dg[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dg[ii, ss] > 0)
                                                {
                                                    dg[ii, ss] = (byte)(dg[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dg[ii, ss] < 255)
                                                {
                                                    dg[ii, ss] = (byte)(dg[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        if (sb[ix, sx] > 0 && sb[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = db[i, s];
                                            if (ac2 <= c)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    db[i, s] = (byte)(S255((db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                }
                                                else
                                                {
                                                    db[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                db[i, s] = (byte)(S255((db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            r = db[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (db[ii, ss] > 0)
                                                    {
                                                        db[ii, ss] = (byte)(db[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (db[ii, ss] < 255)
                                                    {
                                                        db[ii, ss] = (byte)(db[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = db[i, s];
                                        db[i, s] = (byte)(S255((db[i + rnd.Next(-1, 2), s] + db[i + rnd.Next(-1, 2), s]) / 2));
                                        r = db[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (db[ii, ss] > 0)
                                                {
                                                    db[ii, ss] = (byte)(db[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (db[ii, ss] < 255)
                                                {
                                                    db[ii, ss] = (byte)(db[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    for (int sx = 0; sx < os; sx++)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = dr[i, s];

                                            dr[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s] + dr[i + rnd.Next(-1, 2), s] + dr[i, s + rnd.Next(-1, 2)] + dr[i, s + rnd.Next(-1, 2)]) / 4));


                                            r = dr[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dr[ii, ss] > 0)
                                                    {
                                                        dr[ii, ss] = (byte)(dr[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dr[ii, ss] < 255)
                                                    {
                                                        dr[ii, ss] = (byte)(dr[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = dr[i, s];
                                        dr[i, s] = (byte)(S255((dr[i, s + rnd.Next(-1, 2)] + dr[i, s + rnd.Next(-1, 2)]) / 2));
                                        r = dr[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dr[ii, ss] > 0)
                                                {
                                                    dr[ii, ss] = (byte)(dr[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dr[ii, ss] < 255)
                                                {
                                                    dr[ii, ss] = (byte)(dr[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (sg[ix, sx] > 0 && sg[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = dg[i, s];

                                            dg[i, s] = (byte)(S255((dg[i + rnd.Next(-1, 2), s] + dg[i + rnd.Next(-1, 2), s] + dg[i, s + rnd.Next(-1, 2)] + dg[i, s + rnd.Next(-1, 2)]) / 4));


                                            r = dg[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dg[ii, ss] > 0)
                                                    {
                                                        dg[ii, ss] = (byte)(dg[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dg[ii, ss] < 255)
                                                    {
                                                        dg[ii, ss] = (byte)(dg[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = dg[i, s];
                                        dg[i, s] = (byte)(S255((dg[i, s + rnd.Next(-1, 2)] + dg[i, s + rnd.Next(-1, 2)]) / 2));
                                        r = dg[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dg[ii, ss] > 0)
                                                {
                                                    dg[ii, ss] = (byte)(dg[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dg[ii, ss] < 255)
                                                {
                                                    dg[ii, ss] = (byte)(dg[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (sb[ix, sx] > 0 && sb[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = db[i, s];

                                            db[i, s] = (byte)(S255((db[i + rnd.Next(-1, 2), s] + db[i + rnd.Next(-1, 2), s] + db[i, s + rnd.Next(-1, 2)] + db[i, s + rnd.Next(-1, 2)]) / 4));


                                            r = db[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (db[ii, ss] > 0)
                                                    {
                                                        db[ii, ss] = (byte)(db[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (db[ii, ss] < 255)
                                                    {
                                                        db[ii, ss] = (byte)(db[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = db[i, s];
                                        db[i, s] = (byte)(S255((db[i, s + rnd.Next(-1, 2)] + db[i, s + rnd.Next(-1, 2)]) / 2));
                                        r = db[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (db[ii, ss] > 0)
                                                {
                                                    db[ii, ss] = (byte)(db[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (db[ii, ss] < 255)
                                                {
                                                    db[ii, ss] = (byte)(db[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb(dr[i, s], dg[i, s], db[i, s]));
                }
            }
            return img;
        }

        private Image ScaleFurryColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, xmm, xoim, nim, nsm, ac2;
            xm = x - 1;
            xmm = xm - 1;
            ni = img.Width * x;
            ns = img.Height * x;
            oi = img.Width;
            os = img.Height;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoim = oim * x;
            ac = ac * (oim - 1) / 100;
            ac = x * (ac + 1);
            ac2 = ac / 2;
            osm = os - 1;

            byte[,] dr = new byte[ni, ns];
            byte[,] dg = new byte[ni, ns];
            byte[,] db = new byte[ni, ns];
            float[,] fr = new float[ni, ns];
            float[,] fg = new float[ni, ns];
            float[,] fb = new float[ni, ns];
            byte[,] sr = new byte[img.Width, img.Height];
            byte[,] sg = new byte[img.Width, img.Height];
            byte[,] sb = new byte[img.Width, img.Height];
            Random rnd = new Random();

            for (int i = 0; i < oi; i++)
            {
                for (int s = 0; s < os; s++)
                {
                    sr[i, s] = ((Bitmap)img).GetPixel(i, s).R;
                    sg[i, s] = ((Bitmap)img).GetPixel(i, s).G;
                    sb[i, s] = ((Bitmap)img).GetPixel(i, s).B;
                }
            }


            img = new Bitmap(img, new Size(ni, ns));
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    dr[i, s] = sr[i / x, s / x];
                    dg[i, s] = sg[i / x, s / x];
                    db[i, s] = sb[i / x, s / x];
                    fr[i, s] = dr[i, s];
                    fg[i, s] = dg[i, s];
                    fb[i, s] = db[i, s];
                }
            }

            int r, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    for (int sx = osm; sx > -1; sx--)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = dr[i, s];
                                            if (ac2 >= c)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    dr[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                }
                                                else
                                                {
                                                    dr[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                dr[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }

                                            r = dr[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dr[ii, ss] > 0)
                                                    {
                                                        dr[ii, ss] = (byte)(dr[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dr[ii, ss] < 255)
                                                    {
                                                        dr[ii, ss] = (byte)(dr[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            fr[i, s] = (fr[i, s] * (c - ac2) + dr[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = dr[i, s];
                                        dr[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s] + dr[i + rnd.Next(-1, 2), s]) / 2));
                                        r = dr[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dr[ii, ss] > 0)
                                                {
                                                    dr[ii, ss] = (byte)(dr[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dr[ii, ss] < 255)
                                                {
                                                    dr[ii, ss] = (byte)(dr[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            fr[i, s] = (fr[i, s] * (c - ac2) + dr[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }

                            }
                        }
                        if (sg[ix, sx] > 0 && sg[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = dg[i, s];
                                            if (ac2 >= c)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    dg[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                }
                                                else
                                                {
                                                    dg[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                dg[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            r = dg[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dg[ii, ss] > 0)
                                                    {
                                                        dg[ii, ss] = (byte)(dg[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dg[ii, ss] < 255)
                                                    {
                                                        dg[ii, ss] = (byte)(dg[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            fg[i, s] = (fg[i, s] * (c - ac2) + dg[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = dg[i, s];
                                        dg[i, s] = (byte)(S255((dg[i + rnd.Next(-1, 2), s] + dg[i + rnd.Next(-1, 2), s]) / 2));
                                        r = dg[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dg[ii, ss] > 0)
                                                {
                                                    dg[ii, ss] = (byte)(dg[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dg[ii, ss] < 255)
                                                {
                                                    dg[ii, ss] = (byte)(dg[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            fg[i, s] = (fg[i, s] * (c - ac2) + dg[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }

                            }
                        }
                        if (sb[ix, sx] > 0 && sb[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = db[i, s];
                                            if (ac2 >= c)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    db[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                }
                                                else
                                                {
                                                    db[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                db[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            r = db[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (db[ii, ss] > 0)
                                                    {
                                                        db[ii, ss] = (byte)(db[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (db[ii, ss] < 255)
                                                    {
                                                        db[ii, ss] = (byte)(db[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            fb[i, s] = (fb[i, s] * (c - ac2) + db[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }
                            }
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = db[i, s];
                                        db[i, s] = (byte)(S255((db[i + rnd.Next(-1, 2), s] + db[i + rnd.Next(-1, 2), s]) / 2));
                                        r = db[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (db[ii, ss] > 0)
                                                {
                                                    db[ii, ss] = (byte)(db[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (db[ii, ss] < 255)
                                                {
                                                    db[ii, ss] = (byte)(db[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }

                                        if (c >= ac2)
                                        {
                                            fb[i, s] = (fb[i, s] * (c - ac2) + db[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    for (int sx = 0; sx < os; sx++)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = dr[i, s];

                                            dr[i, s] = (byte)(S255((dr[i + rnd.Next(-1, 2), s] + dr[i + rnd.Next(-1, 2), s] + dr[i, s + rnd.Next(-1, 2)] + dr[i, s + rnd.Next(-1, 2)]) / 4));


                                            r = dr[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dr[ii, ss] > 0)
                                                    {
                                                        dr[ii, ss] = (byte)(dr[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dr[ii, ss] < 255)
                                                    {
                                                        dr[ii, ss] = (byte)(dr[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            fr[i, s] = (fr[i, s] * (c - ac2) + dr[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = dr[i, s];
                                        dr[i, s] = (byte)(S255((dr[i, s + rnd.Next(-1, 2)] + dr[i, s + rnd.Next(-1, 2)]) / 2));
                                        r = dr[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dr[ii, ss] > 0)
                                                {
                                                    dr[ii, ss] = (byte)(dr[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dr[ii, ss] < 255)
                                                {
                                                    dr[ii, ss] = (byte)(dr[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                    if (c >= ac2)
                                    {
                                        fr[i, s] = (fr[i, s] * (c - ac2) + dr[i, s]) / (c - ac2 + 1);
                                    }
                                }
                            }
                        }
                        if (sg[ix, sx] > 0 && sg[ix, sx] < 255)
                        {
                            for (int i = ix * x; i < ix * x + x; i++)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = dg[i, s];

                                            dg[i, s] = (byte)(S255((dg[i + rnd.Next(-1, 2), s] + dg[i + rnd.Next(-1, 2), s] + dg[i, s + rnd.Next(-1, 2)] + dg[i, s + rnd.Next(-1, 2)]) / 4));


                                            r = dg[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dg[ii, ss] > 0)
                                                    {
                                                        dg[ii, ss] = (byte)(dg[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (dg[ii, ss] < 255)
                                                    {
                                                        dg[ii, ss] = (byte)(dg[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            fg[i, s] = (fg[i, s] * (c - ac2) + dg[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = dg[i, s];
                                        dg[i, s] = (byte)(S255((dg[i, s + rnd.Next(-1, 2)] + dg[i, s + rnd.Next(-1, 2)]) / 2));
                                        r = dg[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dg[ii, ss] > 0)
                                                {
                                                    dg[ii, ss] = (byte)(dg[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (dg[ii, ss] < 255)
                                                {
                                                    dg[ii, ss] = (byte)(dg[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                    if (c >= ac2)
                                    {
                                        fg[i, s] = (fg[i, s] * (c - ac2) + dg[i, s]) / (c - ac2 + 1);
                                    }
                                }
                            }
                        }
                        if (sb[ix, sx] > 0 && sb[ix, sx] < 255)
                        {
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = db[i, s];

                                            db[i, s] = (byte)(S255((db[i + rnd.Next(-1, 2), s] + db[i + rnd.Next(-1, 2), s] + db[i, s + rnd.Next(-1, 2)] + db[i, s + rnd.Next(-1, 2)]) / 4));


                                            r = db[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (db[ii, ss] > 0)
                                                    {
                                                        db[ii, ss] = (byte)(db[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (db[ii, ss] < 255)
                                                    {
                                                        db[ii, ss] = (byte)(db[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (c >= ac2)
                                        {
                                            fb[i, s] = (fb[i, s] * (c - ac2) + db[i, s]) / (c - ac2 + 1);
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = db[i, s];
                                        db[i, s] = (byte)(S255((db[i, s + rnd.Next(-1, 2)] + db[i, s + rnd.Next(-1, 2)]) / 2));
                                        r = db[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (db[ii, ss] > 0)
                                                {
                                                    db[ii, ss] = (byte)(db[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (db[ii, ss] < 255)
                                                {
                                                    db[ii, ss] = (byte)(db[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                    if (c >= ac2)
                                    {
                                        fb[i, s] = (fb[i, s] * (c - ac2) + db[i, s]) / (c - ac2 + 1);
                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb((byte)fr[i, s], (byte)fg[i, s], (byte)fb[i, s]));
                }
            }
            return img;
        }

        static byte S255(double v)
        {
            return (byte)(0.000000002833333 * Math.Pow(v, 5) - 0.00000181137 * Math.Pow(v, 4) + 0.0003605953 * Math.Pow(v, 3) - 0.01970911609 * Math.Pow(v, 2) + 0.63373610992 * v + 0.17238095178);
        }

        static float S255f(float v)
        {
            return (float)(0.000000002833333 * Math.Pow(v, 5) - 0.00000181137 * Math.Pow(v, 4) + 0.0003605953 * Math.Pow(v, 3) - 0.01970911609 * Math.Pow(v, 2) + 0.63316688184 * v);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            if (radioButton1.Checked)
            {
                pictureBox1.Image = ScaleFurryGray(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }
            else
            {
                pictureBox1.Image = ScaleFurryColor(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }

            ProgressText.Text = "100";

            pictureBox1.Refresh();
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            if (radioButton1.Checked)
            {
                pictureBox1.Image = ContrastBoldScaleGray(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }
            else
            {
                pictureBox1.Image = ContrastBoldScaleColor(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }

            ProgressText.Text = "100";

            pictureBox1.Refresh();
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
        }

        private Image ContrastBoldScaleGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, xoim, xoim2, xoimac;
            xm = x - 1;
            ni = img.Width * x;
            ns = img.Height * x;
            oi = img.Width;
            os = img.Height;
            oim = oi - 1;
            osm = os - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoimac = x * oim * ac / 200 + 1;
            xoim = x * oim;
            xoim2 = xoimac * 2;

            byte[,] d = new byte[ni, ns];
            float[,] ds = new float[ni, ns];
            float[,] ds2 = new float[ni, ns];
            byte[,] sr = new byte[img.Width, img.Height];
            Random rnd = new Random();

            for (int i = 0; i < oi; i++)
            {
                for (int s = 0; s < os; s++)
                {
                    sr[i, s] = ((Bitmap)img).GetPixel(i, s).R;
                }
            }


            img = new Bitmap(img, new Size(ni, ns));
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[i / x, s / x];
                }
            }

            for (int i = 0; i < ni - x; i++)
            {
                for (int s = 0; s < ns - x; s++)
                {
                    if (i == (i / x) * x)
                    {
                        if (s == (s / x) * x)
                        {
                            ds[i, s] = sr[i / x, s / x];
                        }
                        else
                        {
                            ds[i, s] = (float)((sr[i / x, s / x] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sr[i / x + 1, s / x] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sr[i / x, s / x + 1] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + sr[i / x + 1, s / x + 1] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))) / (1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))));
                        }
                    }
                    else
                    {
                        ds[i, s] = (float)((sr[i / x, s / x] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sr[i / x + 1, s / x] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sr[i / x, s / x + 1] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + sr[i / x + 1, s / x + 1] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))) / (1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))));
                    }
                }
            }

            for (int i = nim; i > -1; i--)
            {
                for (int s = nsm; s > -1; s--)
                {
                    if (sr[i / x, s / x] == 0 || sr[i / x, s / x] == 255)
                    {
                        ds[i, s] = sr[i / x, s / x];
                    }
                    else
                    {
                        ds[i, s] = ds[i * (ni - x) / ni, s * (ns - x) / ns];
                    }
                }
            }

            int r, ld;
            int ii, ss;

            for (int c = 0; c < xoim2; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    for (int sx = osm; sx > -1; sx--)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            if (c <= xoimac)
                                            {
                                                ld = d[i, s];
                                                if (c < xoimac)
                                                {
                                                    if (rnd.Next(0, xoim) > c)
                                                    {
                                                        d[i, s] = (byte)(S255((ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                    }
                                                    else
                                                    {
                                                        d[i, s] = (byte)rnd.Next(0, 256);
                                                    }
                                                }
                                                else if (c == xoimac)
                                                {
                                                    d[i, s] = (byte)ds[i, s];
                                                }

                                                r = d[i, s] - ld;
                                                if (r > 0)
                                                {
                                                    while (r != 0)
                                                    {
                                                        ii = rnd.Next(ix * x, ix * x + x);
                                                        ss = rnd.Next(sx * x, sx * x + x);
                                                        if (d[ii, ss] > 0)
                                                        {
                                                            d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                            r--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (r != 0)
                                                    {
                                                        ii = rnd.Next(ix * x, ix * x + x);
                                                        ss = rnd.Next(sx * x, sx * x + x);
                                                        if (d[ii, ss] < 255)
                                                        {
                                                            d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                            r++;
                                                        }
                                                    }
                                                }
                                                ds[i, s] = (ds[i, s] * c + d[i, s]) / (c + 1);
                                            }
                                        }
                                    }
                                }
                            }

                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] > 0)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    for (int sx = 0; sx < os; sx++)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];

                                            d[i, s] = (byte)((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s] + d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 4);


                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] > 0)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (d[ii, ss] < 255)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 2);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] > 0)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] - 1);
                                                    r--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ProgressText.Text = (c * 100 / xoim2).ToString();
                for (int i = 0; i < ni; i++)
                {
                    for (int s = 0; s < ns; s++)
                    {
                        if (c > xoimac)
                        {
                            ds[i, s] = (ds[i, s] * c + d[i, s] + S255(d[i, s])) / (c + 2);
                        }
                    }
                }
            }


            ds2 = ds;

            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    if (i != 0 && i != nim && s != 0 && s != nsm)
                    {
                        ds[i, s] = (S255f((ds2[i, s] * 2 + ds2[i + 1, s] + ds2[i, s + 1] + ds2[i - 1, s] + ds2[i, s - 1]) / 6) * 2 + ds2[i, s]) / 3;
                    }
                    else
                    {
                        ds[i, s] = (S255f(ds2[i, s]) * 2 + ds2[i, s]) / 3;
                    }

                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb((byte)ds[i, s], (byte)ds[i, s], (byte)ds[i, s]));
                }
            }
            return img;
        }

        private Image ContrastBoldScaleColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, xoim, xoim2, xoimac;
            xm = x - 1;
            ni = img.Width * x;
            ns = img.Height * x;
            oi = img.Width;
            os = img.Height;
            oim = oi - 1;
            osm = os - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoimac = x * oim * ac / 200 + 1;
            xoim = x * oim;
            xoim2 = xoimac * 2;

            byte[,] r = new byte[ni, ns];
            byte[,] g = new byte[ni, ns];
            byte[,] b = new byte[ni, ns];
            float[,] rs = new float[ni, ns];
            float[,] gs = new float[ni, ns];
            float[,] bs = new float[ni, ns];
            float[,] rs2 = new float[ni, ns];
            float[,] gs2 = new float[ni, ns];
            float[,] bs2 = new float[ni, ns];

            byte[,] sr = new byte[img.Width, img.Height];
            byte[,] sg = new byte[img.Width, img.Height];
            byte[,] sb = new byte[img.Width, img.Height];

            Random rnd = new Random();

            for (int i = 0; i < oi; i++)
            {
                for (int s = 0; s < os; s++)
                {
                    sr[i, s] = ((Bitmap)img).GetPixel(i, s).R;
                    sg[i, s] = ((Bitmap)img).GetPixel(i, s).G;
                    sb[i, s] = ((Bitmap)img).GetPixel(i, s).B;
                }
            }

            img = new Bitmap(img, new Size(ni, ns));
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    r[i, s] = sr[i / x, s / x];
                    g[i, s] = sg[i / x, s / x];
                    b[i, s] = sb[i / x, s / x];
                }
            }

            for (int i = 0; i < ni - x; i++)
            {
                for (int s = 0; s < ns - x; s++)
                {
                    if (i == (i / x) * x)
                    {
                        if (s == (s / x) * x)
                        {
                            rs[i, s] = sr[i / x, s / x];
                            gs[i, s] = sg[i / x, s / x];
                            bs[i, s] = sb[i / x, s / x];
                        }
                        else
                        {
                            rs[i, s] = (float)((sr[i / x, s / x] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sr[i / x + 1, s / x] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sr[i / x, s / x + 1] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + sr[i / x + 1, s / x + 1] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))) / (1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))));
                            gs[i, s] = (float)((sg[i / x, s / x] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sg[i / x + 1, s / x] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sg[i / x, s / x + 1] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + sg[i / x + 1, s / x + 1] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))) / (1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))));
                            bs[i, s] = (float)((sb[i / x, s / x] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sb[i / x + 1, s / x] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sb[i / x, s / x + 1] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + sb[i / x + 1, s / x + 1] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))) / (1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))));
                        }
                    }
                    else
                    {
                        rs[i, s] = (float)((sr[i / x, s / x] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sr[i / x + 1, s / x] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sr[i / x, s / x + 1] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + sr[i / x + 1, s / x + 1] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))) / (1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))));
                        gs[i, s] = (float)((sg[i / x, s / x] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sg[i / x + 1, s / x] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sg[i / x, s / x + 1] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + sg[i / x + 1, s / x + 1] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))) / (1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))));
                        bs[i, s] = (float)((sb[i / x, s / x] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sb[i / x + 1, s / x] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + sb[i / x, s / x + 1] / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + sb[i / x + 1, s / x + 1] / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))) / (1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x) * x, 2)) + 1 / (Math.Pow(i - (i / x) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2)) + 1 / (Math.Pow(i - (i / x + 1) * x, 2) + Math.Pow(s - (s / x + 1) * x, 2))));
                    }
                }
            }

            for (int i = nim; i > -1; i--)
            {
                for (int s = nsm; s > -1; s--)
                {
                    if (sr[i / x, s / x] == 0 || sr[i / x, s / x] == 255)
                    {
                        rs[i, s] = sr[i / x, s / x];
                    }
                    else
                    {
                        rs[i, s] = rs[i * (ni - x) / ni, s * (ns - x) / ns];
                    }

                    if (sg[i / x, s / x] == 0 || sg[i / x, s / x] == 255)
                    {
                        gs[i, s] = sg[i / x, s / x];
                    }
                    else
                    {
                        gs[i, s] = gs[i * (ni - x) / ni, s * (ns - x) / ns];
                    }

                    if (sb[i / x, s / x] == 0 || sb[i / x, s / x] == 255)
                    {
                        bs[i, s] = sb[i / x, s / x];
                    }
                    else
                    {
                        bs[i, s] = bs[i * (ni - x) / ni, s * (ns - x) / ns];
                    }
                }
            }

            int rz, ld;
            int ii, ss;

            for (int c = 0; c < xoim2; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    for (int sx = osm; sx > -1; sx--)
                    {
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            if (c <= xoimac)
                                            {
                                                ld = r[i, s];
                                                if (c < xoimac)
                                                {
                                                    if (rnd.Next(0, xoim) > c)
                                                    {
                                                        r[i, s] = (byte)(S255((rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                    }
                                                    else
                                                    {
                                                        r[i, s] = (byte)rnd.Next(0, 256);
                                                    }
                                                }
                                                else if (c == xoimac)
                                                {
                                                    r[i, s] = (byte)rs[i, s];
                                                }

                                                rz = r[i, s] - ld;
                                                if (rz > 0)
                                                {
                                                    while (rz != 0)
                                                    {
                                                        ii = rnd.Next(ix * x, ix * x + x);
                                                        ss = rnd.Next(sx * x, sx * x + x);
                                                        if (r[ii, ss] > 0)
                                                        {
                                                            r[ii, ss] = (byte)(r[ii, ss] - 1);
                                                            rz--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (rz != 0)
                                                    {
                                                        ii = rnd.Next(ix * x, ix * x + x);
                                                        ss = rnd.Next(sx * x, sx * x + x);
                                                        if (r[ii, ss] < 255)
                                                        {
                                                            r[ii, ss] = (byte)(r[ii, ss] + 1);
                                                            rz++;
                                                        }
                                                    }
                                                }
                                                rs[i, s] = (rs[i, s] * c + r[i, s]) / (c + 1);
                                            }
                                        }
                                    }
                                }
                            }

                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = r[i, s];
                                        r[i, s] = (byte)((r[i + rnd.Next(-1, 2), s] + r[i + rnd.Next(-1, 2), s]) / 2);
                                        rz = r[i, s] - ld;
                                        if (rz > 0)
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (r[ii, ss] > 0)
                                                {
                                                    r[ii, ss] = (byte)(r[ii, ss] - 1);
                                                    rz--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (r[ii, ss] < 255)
                                                {
                                                    r[ii, ss] = (byte)(r[ii, ss] + 1);
                                                    rz++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (sg[ix, sx] > 0 && sg[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            if (c <= xoimac)
                                            {
                                                ld = g[i, s];
                                                if (c < xoimac)
                                                {
                                                    if (rnd.Next(0, xoim) > c)
                                                    {
                                                        g[i, s] = (byte)(S255((gs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + gs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + gs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + gs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + gs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + gs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + gs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + gs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + gs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                    }
                                                    else
                                                    {
                                                        g[i, s] = (byte)rnd.Next(0, 256);
                                                    }
                                                }
                                                else if (c == xoimac)
                                                {
                                                    g[i, s] = (byte)gs[i, s];
                                                }

                                                rz = g[i, s] - ld;
                                                if (rz > 0)
                                                {
                                                    while (rz != 0)
                                                    {
                                                        ii = rnd.Next(ix * x, ix * x + x);
                                                        ss = rnd.Next(sx * x, sx * x + x);
                                                        if (g[ii, ss] > 0)
                                                        {
                                                            g[ii, ss] = (byte)(g[ii, ss] - 1);
                                                            rz--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (rz != 0)
                                                    {
                                                        ii = rnd.Next(ix * x, ix * x + x);
                                                        ss = rnd.Next(sx * x, sx * x + x);
                                                        if (g[ii, ss] < 255)
                                                        {
                                                            g[ii, ss] = (byte)(g[ii, ss] + 1);
                                                            rz++;
                                                        }
                                                    }
                                                }
                                                gs[i, s] = (gs[i, s] * c + g[i, s]) / (c + 1);
                                            }
                                        }
                                    }
                                }
                            }

                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = g[i, s];
                                        g[i, s] = (byte)((g[i + rnd.Next(-1, 2), s] + g[i + rnd.Next(-1, 2), s]) / 2);
                                        rz = g[i, s] - ld;
                                        if (rz > 0)
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (g[ii, ss] > 0)
                                                {
                                                    g[ii, ss] = (byte)(g[ii, ss] - 1);
                                                    rz--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (g[ii, ss] < 255)
                                                {
                                                    g[ii, ss] = (byte)(g[ii, ss] + 1);
                                                    rz++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (sb[ix, sx] > 0 && sb[ix, sx] < 255)
                        {
                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            if (c <= xoimac)
                                            {
                                                ld = b[i, s];
                                                if (c < xoimac)
                                                {
                                                    if (rnd.Next(0, xoim) > c)
                                                    {
                                                        b[i, s] = (byte)(S255((bs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + bs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + bs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + bs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + bs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + bs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + bs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + bs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + bs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                    }
                                                    else
                                                    {
                                                        b[i, s] = (byte)rnd.Next(0, 256);
                                                    }
                                                }
                                                else if (c == xoimac)
                                                {
                                                    b[i, s] = (byte)bs[i, s];
                                                }

                                                rz = b[i, s] - ld;
                                                if (rz > 0)
                                                {
                                                    while (rz != 0)
                                                    {
                                                        ii = rnd.Next(ix * x, ix * x + x);
                                                        ss = rnd.Next(sx * x, sx * x + x);
                                                        if (b[ii, ss] > 0)
                                                        {
                                                            b[ii, ss] = (byte)(b[ii, ss] - 1);
                                                            rz--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (rz != 0)
                                                    {
                                                        ii = rnd.Next(ix * x, ix * x + x);
                                                        ss = rnd.Next(sx * x, sx * x + x);
                                                        if (b[ii, ss] < 255)
                                                        {
                                                            b[ii, ss] = (byte)(b[ii, ss] + 1);
                                                            rz++;
                                                        }
                                                    }
                                                }
                                                bs[i, s] = (bs[i, s] * c + b[i, s]) / (c + 1);
                                            }
                                        }
                                    }
                                }
                            }

                            for (int i = ix * x + xm; i > ix * x - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sx * x + xm; s > sx * x - 1; s--)
                                    {
                                        ld = b[i, s];
                                        b[i, s] = (byte)((b[i + rnd.Next(-1, 2), s] + b[i + rnd.Next(-1, 2), s]) / 2);
                                        rz = b[i, s] - ld;
                                        if (rz > 0)
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (b[ii, ss] > 0)
                                                {
                                                    b[ii, ss] = (byte)(b[ii, ss] - 1);
                                                    rz--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (b[ii, ss] < 255)
                                                {
                                                    b[ii, ss] = (byte)(b[ii, ss] + 1);
                                                    rz++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    for (int sx = 0; sx < os; sx++)
                    {
                        if (sg[ix, sx] > 0 && sg[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = g[i, s];
                                            g[i, s] = (byte)((g[i + rnd.Next(-1, 2), s] + g[i + rnd.Next(-1, 2), s] + g[i, s + rnd.Next(-1, 2)] + g[i, s + rnd.Next(-1, 2)]) / 4);

                                            rz = g[i, s] - ld;
                                            if (rz > 0)
                                            {
                                                while (rz != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (g[ii, ss] > 0)
                                                    {
                                                        g[ii, ss] = (byte)(g[ii, ss] - 1);
                                                        rz--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rz != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (g[ii, ss] < 255)
                                                    {
                                                        g[ii, ss] = (byte)(g[ii, ss] + 1);
                                                        rz++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = g[i, s];
                                        g[i, s] = (byte)((g[i, s + rnd.Next(-1, 2)] + g[i, s + rnd.Next(-1, 2)]) / 2);

                                        rz = g[i, s] - ld;
                                        if (rz > 0)
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (g[ii, ss] > 0)
                                                {
                                                    g[ii, ss] = (byte)(g[ii, ss] - 1);
                                                    rz--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (g[ii, ss] < 255)
                                                {
                                                    g[ii, ss] = (byte)(g[ii, ss] + 1);
                                                    rz++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = r[i, s];
                                            r[i, s] = (byte)((r[i + rnd.Next(-1, 2), s] + r[i + rnd.Next(-1, 2), s] + r[i, s + rnd.Next(-1, 2)] + r[i, s + rnd.Next(-1, 2)]) / 4);

                                            rz = r[i, s] - ld;
                                            if (rz > 0)
                                            {
                                                while (rz != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (r[ii, ss] > 0)
                                                    {
                                                        r[ii, ss] = (byte)(r[ii, ss] - 1);
                                                        rz--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rz != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (r[ii, ss] < 255)
                                                    {
                                                        r[ii, ss] = (byte)(r[ii, ss] + 1);
                                                        rz++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {

                                for (int s = sx * x; s < sx * x + x; s++)
                                {

                                    if (s != 0 && s != nsm)
                                    {
                                        ld = r[i, s];
                                        r[i, s] = (byte)((r[i, s + rnd.Next(-1, 2)] + r[i, s + rnd.Next(-1, 2)]) / 2);

                                        rz = r[i, s] - ld;
                                        if (rz > 0)
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (r[ii, ss] > 0)
                                                {
                                                    r[ii, ss] = (byte)(r[ii, ss] - 1);
                                                    rz--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (r[ii, ss] < 255)
                                                {
                                                    r[ii, ss] = (byte)(r[ii, ss] + 1);
                                                    rz++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (sb[ix, sx] > 0 && sb[ix, sx] < 255)
                        {
                            for (int i = ix * x; i < ix * x + x; i++)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sx * x; s < sx * x + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = b[i, s];
                                            b[i, s] = (byte)((b[i + rnd.Next(-1, 2), s] + b[i + rnd.Next(-1, 2), s] + b[i, s + rnd.Next(-1, 2)] + b[i, s + rnd.Next(-1, 2)]) / 4);

                                            rz = b[i, s] - ld;
                                            if (rz > 0)
                                            {
                                                while (rz != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (b[ii, ss] > 0)
                                                    {
                                                        b[ii, ss] = (byte)(b[ii, ss] - 1);
                                                        rz--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rz != 0)
                                                {
                                                    ii = rnd.Next(ix * x, ix * x + x);
                                                    ss = rnd.Next(sx * x, sx * x + x);
                                                    if (b[ii, ss] < 255)
                                                    {
                                                        b[ii, ss] = (byte)(b[ii, ss] + 1);
                                                        rz++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            for (int i = ix * x; i < ix * x + x; i++)
                            {
                                for (int s = sx * x; s < sx * x + x; s++)
                                {
                                    if (s != 0 && s != nsm)
                                    {

                                        ld = b[i, s];
                                        b[i, s] = (byte)((b[i, s + rnd.Next(-1, 2)] + b[i, s + rnd.Next(-1, 2)]) / 2);

                                        rz = b[i, s] - ld;
                                        if (rz > 0)
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (b[ii, ss] > 0)
                                                {
                                                    b[ii, ss] = (byte)(b[ii, ss] - 1);
                                                    rz--;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            while (rz != 0)
                                            {
                                                ii = rnd.Next(ix * x, ix * x + x);
                                                ss = rnd.Next(sx * x, sx * x + x);
                                                if (b[ii, ss] < 255)
                                                {
                                                    b[ii, ss] = (byte)(b[ii, ss] + 1);
                                                    rz++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ProgressText.Text = (c * 100 / xoim2).ToString();
                for (int i = 0; i < ni; i++)
                {
                    for (int s = 0; s < ns; s++)
                    {
                        if (c > xoimac)
                        {
                            rs[i, s] = (rs[i, s] * c + r[i, s] + S255(r[i, s])) / (c + 2);
                            gs[i, s] = (gs[i, s] * c + g[i, s] + S255(g[i, s])) / (c + 2);
                            bs[i, s] = (bs[i, s] * c + b[i, s] + S255(b[i, s])) / (c + 2);
                        }
                    }
                }
            }


            rs2 = rs;
            gs2 = gs;
            bs2 = bs;

            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    if (i != 0 && i != nim && s != 0 && s != nsm)
                    {
                        rs[i, s] = (S255f((rs2[i, s] * 2 + rs2[i + 1, s] + rs2[i, s + 1] + rs2[i - 1, s] + rs2[i, s - 1]) / 6) * 2 + rs2[i, s]) / 3;
                        gs[i, s] = (S255f((gs2[i, s] * 2 + gs2[i + 1, s] + gs2[i, s + 1] + gs2[i - 1, s] + gs2[i, s - 1]) / 6) * 2 + gs2[i, s]) / 3;
                        bs[i, s] = (S255f((bs2[i, s] * 2 + bs2[i + 1, s] + bs2[i, s + 1] + bs2[i - 1, s] + bs2[i, s - 1]) / 6) * 2 + bs2[i, s]) / 3;
                    }
                    else
                    {
                        rs[i, s] = (S255f(rs2[i, s]) * 2 + rs2[i, s]) / 3;
                        gs[i, s] = (S255f(gs2[i, s]) * 2 + gs2[i, s]) / 3;
                        bs[i, s] = (S255f(bs2[i, s]) * 2 + bs2[i, s]) / 3;
                    }
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb((byte)rs[i, s], (byte)gs[i, s], (byte)bs[i, s]));
                }
            }
            return img;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;

            Image smpl = pictureBox1.Image;

            
            if (radioButton1.Checked)
            {
                pictureBox1.Image = ScaleSeparateGray(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
            }
            else
            {
                pictureBox1.Image = ScaleSeparateColor(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
            }

            ProgressText.Text = "100";

            pictureBox1.Refresh();
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
        }

        private Image ScaleSeparateGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, xm, oim, osm, sxx, sxxm, ixx, ixxm, halfx;
            double x50p, x150p, rex, rex2;
            bool xEven;
            xEven = (x % 2) == 0;
            x50p = (double)x / 2 - 0.5;
            halfx = x / 2;
            x150p = (double)x * 1.5 - 0.5;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            xm = x - 1;
            byte d, min, max;
            double[,] r = new double[ni, ns];
            double[,] ri = new double[ni, ns];
            double[,] ris = new double[ni, ns];
            byte[,] sr = new byte[oi, os];

            if (ac > 41) ac = 42;
            else if (ac < 10) ac = 10;
            else if (ac < 20) ac = 20;
            else if (ac < 41) ac = 30;

            min = 255;
            max = 0;
            for (int ix = 0; ix < oi; ix++)
            {
                for (int sx = 0; sx < os; sx++)
                {
                    sr[ix, sx] = ((Bitmap)img).GetPixel(ix, sx).R;
                    if (sr[ix, sx] < min)
                        min = sr[ix, sx];
                    else if (sr[ix, sx] > max)
                        max = sr[ix, sx];
                }
            }
            rex = max - min;
            rex2 = (max + min) / 2;

            double aax = (Math.Log(x, 0.96) + 127.5) * rex / 255; //0.96 - AntiAliasing separate, 0.99 - no AA, separate, 1.01 - average colors
            if (aax < 1) aax = 1;
            double aax2 = aax / 2;
            double maax2 = rex2 - aax2;
            double paax2 = rex2 + aax2;

            img = new Bitmap(img, new Size(ni, ns));

            double x1, y1, x2, y2, x3, y3, x4, y4, q1, q2, q3, q4;

            int xs = oim / 2, ys = osm / 2, xsp = xs + 1, ysp = ys + 1;
            double xsx = xs * x + x50p;
            double xsxp = xsx + x;
            double ysx = ys * x + x50p;
            double ysxp = ysx + x;
            int ys2x = (ys + 2) * x;

            int iysx = ys * x;
            int iysxp = iysx + xm;
            int iysxpi = iysxp + 1;
            int iysxpp = iysxpi + xm;

            int ixsx = xs * x;
            int ixsxp = ixsx + xm;
            int ixsxpi = ixsxp + 1;
            int ixsxpp = ixsxpi + xm;


            for (int i = ixsx; i < (xs + 2) * x; i++)
            {
                Parallel.For(iysx, ys2x, s =>
                {
                    ri[i, s] = Bilinear(i, (int)s, xsx, ysx, xsxp, ysxp, sr[xs, ys], sr[xsp, ys], sr[xsp, ysp], sr[xs, ysp]);
                });
            }

            for (int ix = xs + 2; ix < oi; ix++)
            {
                int dixxm = ix * x;
                ixxm = dixxm - 1;
                double dixx = (ix + 1) * x;
                double dixxp = dixxm + x50p;

                for (int i = dixxm; i < dixx; i++)
                {
                    Parallel.For(iysx, ys2x, s =>
                    {
                        ri[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (ri[ixxm, iysx] + ri[ixxm, iysxp]) / 2, sr[ix, ys], sr[ix, ys + 1], (ri[ixxm, iysxpi] + ri[ixxm, iysxpp]) / 2);
                    });
                }
            }

            for (int ix = xs - 1; ix > -1; ix--)
            {
                int dixx = ix * x;
                int dixxp = dixx + x;
                double dixxm = dixx + x50p;

                for (int i = dixx; i < dixxp; i++)
                {
                    Parallel.For(iysx, ys2x, s =>
                    {
                        ri[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, ys], (ri[dixxp, iysx] + ri[dixxp, iysxp]) / 2, (ri[dixxp, iysxpi] + ri[dixxp, iysxpp]) / 2, sr[ix, ys + 1]);
                    });
                }
            }
            ProgressText.Text = (100 / ac).ToString();
            int xs2x = (xs + 2) * x;
            for (int sx = ys + 2; sx < os; sx++)
            {
                int dsxxm = sx * x;
                sxxm = (int)dsxxm - 1;
                int dsxx = (sx + 1) * x;
                double dsxxp = dsxxm + x50p;

                for (int i = ixsx; i < xs2x; i++)
                {
                    Parallel.For(dsxxm, dsxx, s =>
                    {
                        ri[i, s] = Bilinear(i, (int)s, xsx, sxxm, xsxp, dsxxp, (ri[ixsx, sxxm] + ri[ixsxp, sxxm]) / 2, (ri[ixsxpi, sxxm] + ri[ixsxpp, sxxm]) / 2, sr[xs + 1, sx], sr[xs, sx]);
                    });
                }
            }
            for (int sx = ys - 1; sx > -1; sx--)
            {
                int dsxx = sx * x;
                sxxm = dsxx + x;
                double dsxxm = dsxx + x50p;

                for (int i = ixsx; i < xs2x; i++)
                {
                    Parallel.For(dsxx, sxxm, s =>
                    {
                        ri[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sr[xs, sx], sr[xs + 1, sx], (ri[ixsxpi, sxxm] + ri[ixsxpp, sxxm]) / 2, (ri[ixsx, sxxm] + ri[ixsxp, sxxm]) / 2);
                    });
                }
            }
            ProgressText.Text = (200 / ac).ToString();
            for (int sx = ys + 2; sx < os; sx++)
            {
                for (int ix = xs + 2; ix < oi; ix++)
                {
                    int dsxxm = sx * x;
                    sxxm = dsxxm - 1;
                    int dsxx = (sx + 1) * x;
                    int isxx = dsxx - 1;
                    double dsxxp = dsxxm + x50p;
                    int dixxm = ix * x;
                    ixxm = dixxm - 1;
                    int dixx = (ix + 1) * x;
                    int iixx = dixx - 1;
                    double dixxp = dixxm + x50p;

                    for (int i = dixxm; i < dixx; i++)
                    {
                        Parallel.For(dsxxm, dsxx, s =>
                        {
                            ri[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, ri[ixxm, sxxm], (ri[dixxm, sxxm] + ri[iixx, sxxm]) / 2, sr[ix, sx], (ri[ixxm, dsxxm] + ri[ixxm, isxx]) / 2);
                        });
                    }
                }
            }
            ProgressText.Text = (400 / ac).ToString();
            for (int sx = ys - 1; sx > -1; sx--)
            {
                for (int ix = xs - 1; ix > -1; ix--)
                {
                    int dsxxm = sx * x;
                    sxxm = (sx + 1) * x;
                    int isxx = sxxm - 1;
                    double dsxxp = dsxxm + x50p;
                    int dixxm = ix * x;
                    ixxm = (ix + 1) * x;
                    int iixx = ixxm - 1;
                    double dixxp = dixxm + x50p;

                    for (int i = dixxm; i < ixxm; i++)
                    {
                        Parallel.For(dsxxm, sxxm, s =>
                        {
                            ri[i, s] = Bilinear((int)i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx], (ri[ixxm, dsxxm] + ri[ixxm, isxx]) / 2, ri[ixxm, sxxm], (ri[dixxm, sxxm] + ri[iixx, sxxm]) / 2);
                        });
                    }
                }
            }
            ProgressText.Text = (600 / ac).ToString();
            for (int sx = ys + 2; sx < os; sx++)
            {
                for (int ix = xs - 1; ix > -1; ix--)
                {
                    sxx = sx * x;
                    int isxxm = sxx - 1;
                    int dsxx = (sx + 1) * x;
                    int isxx = dsxx - 1;
                    double dsxxp = sxx + x50p;
                    int dixxm = ix * x;
                    int dixx = dixxm + x;
                    int iixx = dixx - 1;
                    double dixxp = dixxm + x50p;

                    for (int i = dixxm; i < dixx; i++)
                    {
                        Parallel.For(sxx, dsxx, s =>
                        {
                            ri[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (ri[dixxm, isxxm] + ri[iixx, isxxm]) / 2, ri[dixx, isxxm], (ri[dixx, isxx] + ri[dixx, sxx]) / 2, sr[ix, sx]);
                        });
                    }
                }
            }
            ProgressText.Text = (800 / ac).ToString();
            for (int ix = xs + 2; ix < oi; ix++)
            {
                for (int sx = ys - 1; sx > -1; sx--)
                {
                    sxx = sx * x;
                    int dsxx = sxx + x;
                    int isxx = dsxx - 1;
                    double dsxxp = sxx + x50p;
                    ixx = ix * x;
                    int dixx = ixx + x;
                    ixxm = ixx - 1;
                    int iixx = dixx - 1;
                    double dixxp = ixx + x50p;

                    for (int i = ixx; i < dixx; i++)
                    {
                        Parallel.For(sxx, dsxx, s =>
                        {
                            ri[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (ri[ixxm, sxx] + ri[ixxm, isxx]) / 2, sr[ix, sx], (ri[ixx, dsxx] + ri[iixx, dsxx]) / 2, ri[ixxm, dsxx]);
                        });
                    }
                }
            }
            ProgressText.Text = (1000 / ac).ToString();

            if (ac > 10)
            {
                int nim = ni - 1, nsm = ns - 1;
                double oimx = oim * x + x50p, osmx = osm * x + x50p;
                for (int sx = 0; sx < os; sx++)
                {
                    ProgressText.Text = ((2000 - 1000 * (os - sx) / os) / ac).ToString();
                    for (int ix = oim; ix > -1; ix--)
                    {
                        if (ix == oim)
                        {
                            if (sx == 0)
                            {                       // 1     [2]
                                x1 = oimx - x;      //  
                                y1 = x50p;          //  
                                q1 = sr[ix - 1, 0]; // 4      3

                                x2 = oimx;
                                y2 = x150p;

                                q3 = sr[ix, 1];
                                q2 = sr[ix, 0];
                                q4 = sr[ix - 1, 1];
                            }
                            else
                            {
                                y1 = sx * x;            //   1    2
                                x1 = oimx - x50p;       //         
                                y2 = y1 + x50p;         //  4/3  [3]
                                q4 = (double)(sr[ix, sx] + sr[ix, sx - 1]) / 2;

                                x2 = oimx;
                                y1--;
                                q2 = (r[nim, (int)y1] + r[nim - xm, (int)y1]) / 2;

                                q3 = sr[ix, sx];
                                q1 = r[(int)x1, (int)y1];
                            }
                        }
                        else
                        {
                            if (sx == 0)
                            {
                                x1 = ix * x;
                                ixx = (int)(x1);
                                ixxm = ixx + x;
                                x1 += x50p;       //         
                                y1 = x50p;        //   [1]   2
                                q1 = sr[ix, 0];   //   1/4   3

                                x2 = ixxm;
                                y2 = xm;
                                q3 = r[ixxm, xm];

                                q2 = (r[ixxm, 0] + r[ixxm, xm]) / 2;
                                q4 = (double)(sr[ix, 0] + sr[ix, 1]) / 2;
                            }
                            else
                            {
                                x3 = ix * x;
                                ixx = (int)(x3);
                                y3 = sx * x;
                                sxx = (int)(y3);
                                sxxm = sxx - 1;

                                x2 = ixx + x;
                                y1 = sxxm;                //    1    2
                                q2 = r[(int)x2, sxxm];    //    
                                                          //   [4]   3     
                                x1 = x3 + x50p;
                                y2 = y3 + x50p;
                                if (xEven)
                                    q1 = (r[ixx + halfx, sxxm] + r[ixx + halfx - 1, sxxm]) / 2;
                                else
                                    q1 = r[ixx + halfx, sxxm];

                                q4 = sr[ix, sx];

                                if (xEven)
                                    q3 = (r[(int)x2, sxx + halfx] + r[(int)x2, sxx + halfx - 1]) / 2;
                                else
                                    q3 = r[(int)x2, sxx + halfx];
                            }
                        }

                        for (int i = ix * x; i < (ix + 1) * x; i++)
                        {
                            Parallel.For(sx * x, (sx + 1) * x, s =>
                            {
                                r[i, s] = Bilinear(i, s, x1, y1, x2, y2, q1, q2, q3, q4);
                                double dist1 = Math.Max(nim - i + s - x, 0);
                                double dist2 = Math.Max(Math.Abs(ixsxpi - i) + Math.Abs(iysxpi - s) - x, 0);
                                ri[i, s] = (ri[i, s] * dist1 + r[i, s] * dist2) / (dist1 + dist2);
                                ris[i, s] = Math.Min(dist1, dist2);
                            });
                        }
                    }
                }

                if (ac > 20)
                {
                    for (int ix = 0; ix < oi; ix++)
                    {
                        ProgressText.Text = ((3000 - 1000 * (oi - ix) / oi) / ac).ToString();
                        for (int sx = 0; sx < os; sx++)
                        {
                            if (ix == 0)
                            {
                                if (sx == 0)
                                {                  //[1]     2
                                    x1 = x50p;     //  
                                    y1 = x50p;     //  
                                    q1 = sr[0, 0]; // 4      3

                                    x2 = x150p;
                                    y2 = x50p;
                                    q2 = sr[1, 0];

                                    x3 = x150p;
                                    y3 = x150p;
                                    q3 = sr[1, 1];

                                    x4 = x50p;
                                    y4 = x150p;
                                    q4 = sr[0, 1];
                                }
                                else
                                {
                                    y4 = sx * x;
                                    sxxm = (int)(y4) - 1;  // 1   2
                                    x4 = x50p;             //         
                                    y4 += x50p;            //  [4]     3
                                    q4 = sr[0, sx];

                                    x3 = x150p;
                                    y3 = y4;
                                    q3 = sr[1, sx];

                                    x2 = xm;
                                    y2 = sxxm;
                                    q2 = r[xm, sxxm];

                                    x1 = 0;
                                    y1 = sxxm;
                                    q1 = r[0, sxxm];
                                }
                            }
                            else
                            {
                                if (sx == 0)
                                {
                                    ixx = (int)(ix * x);
                                    ixxm = ixx - 1;
                                    x2 = ix * x + x50p;//    1   [2]
                                    y2 = x50p;         //  
                                    q2 = sr[ix, 0];    //    4   2/3

                                    x3 = x2;
                                    y3 = xm;
                                    q3 = (double)(sr[ix, 1] + sr[ix, 0]) / 2;

                                    x1 = ixxm;
                                    y1 = x50p;
                                    q1 = (r[ixxm, 0] + r[ixxm, xm]) / 2;

                                    x4 = ixxm;
                                    y4 = xm;
                                    q4 = r[ixxm, xm];

                                }
                                else
                                {
                                    ixx = (int)(ix * x);
                                    ixxm = ixx - 1;
                                    sxx = (int)(sx * x);
                                    sxxm = sxx - 1;

                                    x2 = ixx + xm;         //     1         2 
                                    y2 = sxxm;             //   
                                    q2 = r[(int)x2, sxxm]; //    4    [3]

                                    x1 = ixx;
                                    y1 = sxxm;
                                    q1 = r[ixx, sxxm];

                                    x3 = ix * x + x50p;
                                    y3 = sx * x + x50p;
                                    q3 = sr[ix, sx];

                                    x4 = ixxm;
                                    y4 = y3;
                                    if (xEven)
                                        q4 = (r[ixxm, sxx + halfx] + r[ixxm, sxx + halfx - 1]) / 2;
                                    else
                                        q4 = r[ixxm, sxx + halfx];
                                }
                            }

                            for (int i = ix * x; i < (ix + 1) * x; i++)
                            {
                                Parallel.For(sx * x, (sx + 1) * x, s =>
                                {
                                    r[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, q1, q2, q3, q4);
                                    double dist1 = Math.Max(i + s - x, 0);
                                    ri[i, s] = (ri[i, s] * dist1 + r[i, s] * ris[i, s]) / (dist1 + ris[i, s]);
                                    ris[i, s] = Math.Min(dist1, ris[i, s]);
                                });
                            }
                        }
                    }

                    if (ac > 30)
                    {
                        int oimm = oim - 1, osmm = osm - 1;

                        for (int ix = oim; ix > -1; ix--)
                        {
                            ProgressText.Text = ((3000 - 500 * (ix - oim) / oim) / ac).ToString();
                            for (int sx = osm; sx > -1; sx--)
                            {
                                if (ix == oim)
                                {
                                    if (sx == osm)
                                    {                      // 1      2
                                        x3 = oimx;         //  
                                        y3 = osmx;         //  
                                        q3 = sr[oim, osm]; // 4     [3]

                                        x2 = x3;
                                        y2 = y3 - x;
                                        q2 = sr[oim, osmm];

                                        x1 = x3 - x;
                                        y1 = y2;
                                        q1 = sr[oimm, osmm];

                                        x4 = x1;
                                        y4 = x3;
                                        q4 = sr[0, 1];
                                    }
                                    else
                                    {
                                        sxx = (int)(sx * x);      //1    [2]
                                        x2 = oimx;                //         
                                        y2 = sx * x + x50p;       //   4     3
                                        q2 = sr[oim, sx];

                                        x3 = nim;
                                        y3 = sxx + x;
                                        q3 = r[nim, (int)y3];

                                        x4 = nim - xm;
                                        y4 = y3;
                                        q4 = r[(int)x4, (int)y4];

                                        x1 = x2 - x;
                                        y1 = y2;
                                        q1 = sr[oimm, sx];
                                    }
                                }
                                else
                                {
                                    if (sx == osm)
                                    {
                                        ixx = (int)(ix * x);
                                        x4 = ix * x + x50p;//   1/4  2
                                        y4 = osmx;         //  
                                        q4 = sr[ix, osm];  //   [4]  3

                                        x3 = ixx + x;
                                        y3 = osmx;
                                        q3 = (r[(int)x3, nsm] + r[(int)x3, nsm - xm]) / 2;

                                        x1 = x4;
                                        y1 = y4 - x50p;
                                        q1 = (double)(sr[ix, osmm] + sr[ix, osm]) / 2;

                                        x2 = x3;
                                        y2 = y1;
                                        q2 = r[(int)x2, (int)y2];
                                    }
                                    else
                                    {
                                        x4 = ix * x;             //          
                                        y4 = sx * x;             //    [1]  2 
                                        ixx = (int)(x4);         //
                                        sxx = (int)(y4);         // 4      3
                                                                 //    
                                        x2 = x4 + x;
                                        y2 = y4 + x50p;
                                        if (xEven)
                                            q2 = (r[(int)x2, sxx + halfx] + r[(int)x2, sxx + halfx - 1]) / 2;
                                        else
                                            q2 = r[(int)x2, sxx + halfx];

                                        x3 = x2 - 1;
                                        y3 = y4 + x;
                                        q3 = r[(int)x3, (int)y3];

                                        x1 = x4 + x50p;
                                        y1 = y2;
                                        q1 = sr[ix, sx];

                                        y4 = y3;
                                        q4 = r[ixx, (int)y4];
                                    }
                                }

                                for (int i = ix * x; i < (ix + 1) * x; i++)
                                {
                                    Parallel.For(sx * x, (sx + 1) * x, s =>
                                    {
                                        r[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, q1, q2, q3, q4);
                                        double dist2 = Math.Max(nim - i + nsm - s - x, 0);
                                        ri[i, s] = (ri[i, s] * dist2 + r[i, s] * ris[i, s]) / (ris[i, s] + dist2);
                                        ris[i, s] = Math.Min(ris[i, s], dist2);
                                    });
                                }
                            }
                        }

                        if (ac > 41)
                        {
                            for (int sx = osm; sx > -1; sx--)
                            {
                                ProgressText.Text = ((3500 - 600 * (sx - osm) / osm) / ac).ToString();
                                for (int ix = 0; ix < oi; ix++)
                                {
                                    if (ix == 0)
                                    {
                                        if (sx == osm)
                                        {                       // 1      2
                                            x1 = x50p;          //  
                                            y1 = osmx - x;      //  
                                            q1 = sr[0, sx - 1]; //[4]     3

                                            x2 = x150p;
                                            y2 = y1;
                                            q2 = sr[1, sx - 1];

                                            x3 = x150p;
                                            y3 = osmx;
                                            q3 = sr[1, sx];

                                            x4 = x50p;
                                            y4 = y3;
                                            q4 = sr[0, sx];
                                        }
                                        else
                                        {
                                            y1 = sx * x;          //  [1] 1/2
                                            x4 = x50p;            //         
                                            y4 = y1 + x;          //   4   3
                                            q4 = (r[0, (int)y4] + r[xm, (int)y4]) / 2;

                                            x2 = xm;
                                            y2 = y1 + x50p;
                                            q2 = (double)(sr[1, sx] + sr[0, sx]) / 2;

                                            x1 = x50p;
                                            y1 = y2;
                                            q1 = sr[0, sx];

                                            x3 = xm;
                                            y3 = y4;
                                            q3 = r[xm, (int)y4];
                                        }
                                    }
                                    else
                                    {
                                        if (sx == osm)
                                        {
                                            x2 = ix * x;
                                            ixx = (int)(x2);
                                            ixxm = ixx - 1;
                                            x2 += x50p;                                     //        2/3
                                            y2 = ns - x - 0.5;                              //    1   
                                            q2 = (double)(sr[ix, osmm] + sr[ix, osm]) / 2;  //    4   [3]
                                                                                            //        
                                            x3 = x2;
                                            y3 = osmx;
                                            q3 = sr[ix, sx];

                                            x1 = ixxm;
                                            y1 = ns - x;
                                            q1 = r[ixxm, (int)y1];

                                            x4 = ixxm;
                                            y4 = osmx;
                                            q4 = (r[ixxm, ns - halfx] + r[ixxm, ns - halfx - 1]) / 2;
                                        }
                                        else
                                        {
                                            x2 = ix * x;
                                            ixx = (int)(x2);
                                            ixxm = ixx - 1;
                                            y2 = sx * x;
                                            sxx = (int)(y2);

                                            x4 = ixxm;
                                            y4 = y2 + x;              //    1   [2]
                                            q4 = r[ixxm, (int)y4];    //    4    3
                                                                      //         
                                            x1 = ixxm;
                                            y1 = y2 + x50p;
                                            if (xEven)
                                                q1 = (r[ixxm, sxx + halfx] + r[ixxm, sxx + halfx - 1]) / 2;
                                            else
                                                q1 = r[ixxm, sxx + halfx];

                                            x2 += x50p;
                                            y2 = y1;
                                            q2 = sr[ix, sx];

                                            x3 = x2;
                                            y3 = y4;
                                            if (xEven)
                                                q3 = (r[ixx + halfx, (int)y3] + r[ixx + halfx - 1, (int)y3]) / 2;
                                            else
                                                q3 = r[ixx + halfx, (int)y3];
                                        }
                                    }

                                    for (int i = ix * x; i < (ix + 1) * x; i++)
                                    {
                                        Parallel.For(sx * x, (sx + 1) * x, s =>
                                        {
                                            r[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, q1, q2, q3, q4);
                                            double dist2 = Math.Max(i + nsm - s - x, 0);
                                            ri[i, s] = (ri[i, s] * dist2 + r[i, s] * ris[i, s]) / (ris[i, s] + dist2);
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }


            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    if (ri[i, s] < maax2)
                    {
                        d = min;
                    }
                    else if (ri[i, s] > paax2)
                    {
                        d = max;
                    }
                    else
                    {
                        d = (byte)((ri[i, s] - maax2) / aax * rex + min + 0.5);
                    }
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb(d, d, d));
                }
            }
            return img;
        }

        private Image ScaleSeparateColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, xm, oim, osm, sxx, sxxm, ixx, ixxm, halfx;
            double x50p, x150p, rex, rex2, gex, gex2, bex, bex2;
            bool xEven;
            xEven = (x % 2) == 0;
            x50p = (double)x / 2 - 0.5;
            halfx = x / 2;
            x150p = (double)x * 1.5 - 0.5;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            xm = x - 1;
            byte dr, dg, db, rmin, rmax, gmin, gmax, bmin, bmax;
            double[,] r = new double[ni, ns];
            double[,] ri = new double[ni, ns];
            double[,] g = new double[ni, ns];
            double[,] gi = new double[ni, ns];
            double[,] b = new double[ni, ns];
            double[,] bi = new double[ni, ns];
            double[,] ris = new double[ni, ns];
            byte[,] sr = new byte[oi, os];
            byte[,] sg = new byte[oi, os];
            byte[,] sb = new byte[oi, os];

            if (ac > 79) ac = 80;
            else if (ac < 20) ac = 20;
            else if (ac < 40) ac = 40;
            else if (ac < 79) ac = 60;

            rmin = gmin = bmin = 255;
            rmax = gmax = bmax = 0;
            Color c;
            for (int ix = 0; ix < oi; ix++)
            {
                for (int sx = 0; sx < os; sx++)
                {
                    c = ((Bitmap)img).GetPixel(ix, sx);
                    sr[ix, sx] = c.R;
                    sg[ix, sx] = c.G;
                    sb[ix, sx] = c.B;
                    if (sr[ix, sx] < rmin)
                        rmin = sr[ix, sx];
                    else if (sr[ix, sx] > rmax)
                        rmax = sr[ix, sx];
                    if (sg[ix, sx] < gmin)
                        gmin = sg[ix, sx];
                    else if (sg[ix, sx] > gmax)
                        gmax = sg[ix, sx];
                    if (sb[ix, sx] < bmin)
                        bmin = sb[ix, sx];
                    else if (sb[ix, sx] > bmax)
                        bmax = sb[ix, sx];
                }
            }
            rex = rmax - rmin;
            rex2 = (rmax + rmin) / 2;
            gex = gmax - gmin;
            gex2 = (gmax + gmin) / 2;
            bex = bmax - bmin;
            bex2 = (bmax + bmin) / 2;

            double aax = (Math.Log(x, 0.96) + 127.5) * (rex + gex + bex) / 765; //0.96 - AntiAliasing separate, 0.99 - no AA, separate, 1.01 - average colors
            if (aax < 1) aax = 1;
            double aax2 = aax / 2;
            double rmaax2 = rex2 - aax2;
            double rpaax2 = rex2 + aax2;
            double gmaax2 = gex2 - aax2;
            double gpaax2 = gex2 + aax2;
            double bmaax2 = bex2 - aax2;
            double bpaax2 = bex2 + aax2;

            img = new Bitmap(img, new Size(ni, ns));

            double x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4;

            int xs = oim / 2, ys = osm / 2, xsp = xs + 1, ysp = ys + 1;
            double xsx = xs * x + x50p;
            double xsxp = xsx + x;
            double ysx = ys * x + x50p;
            double ysxp = ysx + x;
            int ys2x = (ys + 2) * x;

            int iysx = ys * x;
            int iysxp = iysx + xm;
            int iysxpi = iysxp + 1;
            int iysxpp = iysxpi + xm;

            int ixsx = xs * x;
            int ixsxp = ixsx + xm;
            int ixsxpi = ixsxp + 1;
            int ixsxpp = ixsxpi + xm;


            for (int i = ixsx; i < (xs + 2) * x; i++)
            {
                Parallel.For(iysx, ys2x, s =>
                {
                    ri[i, s] = Bilinear(i, (int)s, xsx, ysx, xsxp, ysxp, sr[xs, ys], sr[xsp, ys], sr[xsp, ysp], sr[xs, ysp]);
                    gi[i, s] = Bilinear(i, (int)s, xsx, ysx, xsxp, ysxp, sg[xs, ys], sg[xsp, ys], sg[xsp, ysp], sg[xs, ysp]);
                    bi[i, s] = Bilinear(i, (int)s, xsx, ysx, xsxp, ysxp, sb[xs, ys], sb[xsp, ys], sb[xsp, ysp], sb[xs, ysp]);
                });
            }

            for (int ix = xs + 2; ix < oi; ix++)
            {
                int dixxm = ix * x;
                ixxm = dixxm - 1;
                double dixx = (ix + 1) * x;
                double dixxp = dixxm + x50p;

                for (int i = dixxm; i < dixx; i++)
                {
                    Parallel.For(iysx, ys2x, s =>
                    {
                        ri[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (ri[ixxm, iysx] + ri[ixxm, iysxp]) / 2, sr[ix, ys], sr[ix, ys + 1], (ri[ixxm, iysxpi] + ri[ixxm, iysxpp]) / 2);
                        gi[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (gi[ixxm, iysx] + gi[ixxm, iysxp]) / 2, sg[ix, ys], sg[ix, ys + 1], (gi[ixxm, iysxpi] + gi[ixxm, iysxpp]) / 2);
                        bi[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (bi[ixxm, iysx] + bi[ixxm, iysxp]) / 2, sb[ix, ys], sb[ix, ys + 1], (bi[ixxm, iysxpi] + bi[ixxm, iysxpp]) / 2);
                    });
                }
            }

            for (int ix = xs - 1; ix > -1; ix--)
            {
                int dixx = ix * x;
                int dixxp = dixx + x;
                double dixxm = dixx + x50p;

                for (int i = dixx; i < dixxp; i++)
                {
                    Parallel.For(iysx, ys2x, s =>
                    {
                        ri[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, ys], (ri[dixxp, iysx] + ri[dixxp, iysxp]) / 2, (ri[dixxp, iysxpi] + ri[dixxp, iysxpp]) / 2, sr[ix, ys + 1]);
                        gi[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sg[ix, ys], (gi[dixxp, iysx] + gi[dixxp, iysxp]) / 2, (gi[dixxp, iysxpi] + gi[dixxp, iysxpp]) / 2, sg[ix, ys + 1]);
                        bi[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sb[ix, ys], (bi[dixxp, iysx] + bi[dixxp, iysxp]) / 2, (bi[dixxp, iysxpi] + bi[dixxp, iysxpp]) / 2, sb[ix, ys + 1]);
                    });
                }
            }
            ProgressText.Text = (200 / ac).ToString();
            int xs2x = (xs + 2) * x;
            for (int sx = ys + 2; sx < os; sx++)
            {
                int dsxxm = sx * x;
                sxxm = (int)dsxxm - 1;
                int dsxx = (sx + 1) * x;
                double dsxxp = dsxxm + x50p;

                for (int i = ixsx; i < xs2x; i++)
                {
                    Parallel.For(dsxxm, dsxx, s =>
                    {
                        ri[i, s] = Bilinear(i, (int)s, xsx, sxxm, xsxp, dsxxp, (ri[ixsx, sxxm] + ri[ixsxp, sxxm]) / 2, (ri[ixsxpi, sxxm] + ri[ixsxpp, sxxm]) / 2, sr[xs + 1, sx], sr[xs, sx]);
                        gi[i, s] = Bilinear(i, (int)s, xsx, sxxm, xsxp, dsxxp, (gi[ixsx, sxxm] + gi[ixsxp, sxxm]) / 2, (gi[ixsxpi, sxxm] + gi[ixsxpp, sxxm]) / 2, sg[xs + 1, sx], sg[xs, sx]);
                        bi[i, s] = Bilinear(i, (int)s, xsx, sxxm, xsxp, dsxxp, (bi[ixsx, sxxm] + bi[ixsxp, sxxm]) / 2, (bi[ixsxpi, sxxm] + bi[ixsxpp, sxxm]) / 2, sb[xs + 1, sx], sb[xs, sx]);
                    });
                }
            }
            for (int sx = ys - 1; sx > -1; sx--)
            {
                int dsxx = sx * x;
                sxxm = dsxx + x;
                double dsxxm = dsxx + x50p;

                for (int i = ixsx; i < xs2x; i++)
                {
                    Parallel.For(dsxx, sxxm, s =>
                    {
                        ri[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sr[xs, sx], sr[xs + 1, sx], (ri[ixsxpi, sxxm] + ri[ixsxpp, sxxm]) / 2, (ri[ixsx, sxxm] + ri[ixsxp, sxxm]) / 2);
                        gi[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sg[xs, sx], sg[xs + 1, sx], (gi[ixsxpi, sxxm] + gi[ixsxpp, sxxm]) / 2, (gi[ixsx, sxxm] + gi[ixsxp, sxxm]) / 2);
                        bi[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sb[xs, sx], sb[xs + 1, sx], (bi[ixsxpi, sxxm] + bi[ixsxpp, sxxm]) / 2, (bi[ixsx, sxxm] + bi[ixsxp, sxxm]) / 2);
                    });
                }
            }
            ProgressText.Text = (400 / ac).ToString();
            for (int sx = ys + 2; sx < os; sx++)
            {
                for (int ix = xs + 2; ix < oi; ix++)
                {
                    int dsxxm = sx * x;
                    sxxm = dsxxm - 1;
                    int dsxx = (sx + 1) * x;
                    int isxx = dsxx - 1;
                    double dsxxp = dsxxm + x50p;
                    int dixxm = ix * x;
                    ixxm = dixxm - 1;
                    int dixx = (ix + 1) * x;
                    int iixx = dixx - 1;
                    double dixxp = dixxm + x50p;

                    for (int i = dixxm; i < dixx; i++)
                    {
                        Parallel.For(dsxxm, dsxx, s =>
                        {
                            ri[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, ri[ixxm, sxxm], (ri[dixxm, sxxm] + ri[iixx, sxxm]) / 2, sr[ix, sx], (ri[ixxm, dsxxm] + ri[ixxm, isxx]) / 2);
                            gi[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, gi[ixxm, sxxm], (gi[dixxm, sxxm] + gi[iixx, sxxm]) / 2, sg[ix, sx], (gi[ixxm, dsxxm] + gi[ixxm, isxx]) / 2);
                            bi[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, bi[ixxm, sxxm], (bi[dixxm, sxxm] + bi[iixx, sxxm]) / 2, sb[ix, sx], (bi[ixxm, dsxxm] + bi[ixxm, isxx]) / 2);
                        });
                    }
                }
            }
            ProgressText.Text = (800 / ac).ToString();
            for (int sx = ys - 1; sx > -1; sx--)
            {
                for (int ix = xs - 1; ix > -1; ix--)
                {
                    int dsxxm = sx * x;
                    sxxm = (sx + 1) * x;
                    int isxx = sxxm - 1;
                    double dsxxp = dsxxm + x50p;
                    int dixxm = ix * x;
                    ixxm = (ix + 1) * x;
                    int iixx = ixxm - 1;
                    double dixxp = dixxm + x50p;

                    for (int i = dixxm; i < ixxm; i++)
                    {
                        Parallel.For(dsxxm, sxxm, s =>
                        {
                            ri[i, s] = Bilinear((int)i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx], (ri[ixxm, dsxxm] + ri[ixxm, isxx]) / 2, ri[ixxm, sxxm], (ri[dixxm, sxxm] + ri[iixx, sxxm]) / 2);
                            gi[i, s] = Bilinear((int)i, s, dixxp, dsxxp, ixxm, sxxm, sg[ix, sx], (gi[ixxm, dsxxm] + gi[ixxm, isxx]) / 2, gi[ixxm, sxxm], (gi[dixxm, sxxm] + gi[iixx, sxxm]) / 2);
                            bi[i, s] = Bilinear((int)i, s, dixxp, dsxxp, ixxm, sxxm, sb[ix, sx], (bi[ixxm, dsxxm] + bi[ixxm, isxx]) / 2, bi[ixxm, sxxm], (bi[dixxm, sxxm] + bi[iixx, sxxm]) / 2);
                        });
                    }
                }
            }
            ProgressText.Text = (1200 / ac).ToString();
            for (int sx = ys + 2; sx < os; sx++)
            {
                for (int ix = xs - 1; ix > -1; ix--)
                {
                    sxx = sx * x;
                    int isxxm = sxx - 1;
                    int dsxx = (sx + 1) * x;
                    int isxx = dsxx - 1;
                    double dsxxp = sxx + x50p;
                    int dixxm = ix * x;
                    int dixx = dixxm + x;
                    int iixx = dixx - 1;
                    double dixxp = dixxm + x50p;

                    for (int i = dixxm; i < dixx; i++)
                    {
                        Parallel.For(sxx, dsxx, s =>
                        {
                            ri[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (ri[dixxm, isxxm] + ri[iixx, isxxm]) / 2, ri[dixx, isxxm], (ri[dixx, isxx] + ri[dixx, sxx]) / 2, sr[ix, sx]);
                            gi[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (gi[dixxm, isxxm] + gi[iixx, isxxm]) / 2, gi[dixx, isxxm], (gi[dixx, isxx] + gi[dixx, sxx]) / 2, sg[ix, sx]);
                            bi[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (bi[dixxm, isxxm] + bi[iixx, isxxm]) / 2, bi[dixx, isxxm], (bi[dixx, isxx] + bi[dixx, sxx]) / 2, sb[ix, sx]);
                        });
                    }
                }
            }
            ProgressText.Text = (1600 / ac).ToString();
            for (int ix = xs + 2; ix < oi; ix++)
            {
                for (int sx = ys - 1; sx > -1; sx--)
                {
                    sxx = sx * x;
                    int dsxx = sxx + x;
                    int isxx = dsxx - 1;
                    double dsxxp = sxx + x50p;
                    ixx = ix * x;
                    int dixx = ixx + x;
                    ixxm = ixx - 1;
                    int iixx = dixx - 1;
                    double dixxp = ixx + x50p;

                    for (int i = ixx; i < dixx; i++)
                    {
                        Parallel.For(sxx, dsxx, s =>
                        {
                            ri[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (ri[ixxm, sxx] + ri[ixxm, isxx]) / 2, sr[ix, sx], (ri[ixx, dsxx] + ri[iixx, dsxx]) / 2, ri[ixxm, dsxx]);
                            gi[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (gi[ixxm, sxx] + gi[ixxm, isxx]) / 2, sg[ix, sx], (gi[ixx, dsxx] + gi[iixx, dsxx]) / 2, gi[ixxm, dsxx]);
                            bi[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (bi[ixxm, sxx] + bi[ixxm, isxx]) / 2, sb[ix, sx], (bi[ixx, dsxx] + bi[iixx, dsxx]) / 2, bi[ixxm, dsxx]);
                        });
                    }
                }
            }
            ProgressText.Text = (2000 / ac).ToString();

            if (ac > 20)
            {
                int nim = ni - 1, nsm = ns - 1;
                double oimx = oim * x + x50p, osmx = osm * x + x50p;
                for (int sx = 0; sx < os; sx++)
                {
                    ProgressText.Text = ((4000 - 2000 * (os - sx) / os) / ac).ToString();
                    for (int ix = oim; ix > -1; ix--)
                    {
                        if (ix == oim)
                        {
                            if (sx == 0)
                            {                        // 1     [2]
                                x1 = oimx - x;       //  
                                y1 = x50p;           //  
                                rq1 = sr[ix - 1, 0]; // 4      3
                                gq1 = sg[ix - 1, 0];
                                bq1 = sb[ix - 1, 0];

                                x2 = oimx;
                                y2 = x150p;

                                rq3 = sr[ix, 1];
                                rq2 = sr[ix, 0];
                                rq4 = sr[ix - 1, 1];
                                gq3 = sg[ix, 1];
                                gq2 = sg[ix, 0];
                                gq4 = sg[ix - 1, 1];
                                bq3 = sb[ix, 1];
                                bq2 = sb[ix, 0];
                                bq4 = sb[ix - 1, 1];
                            }
                            else
                            {
                                y1 = sx * x;            //   1    2
                                x1 = oimx - x50p;       //         
                                y2 = y1 + x50p;         //  4/3  [3]
                                rq4 = (double)(sr[ix, sx] + sr[ix, sx - 1]) / 2;
                                gq4 = (double)(sg[ix, sx] + sg[ix, sx - 1]) / 2;
                                bq4 = (double)(sb[ix, sx] + sb[ix, sx - 1]) / 2;

                                x2 = oimx;
                                y1--;

                                rq2 = (r[nim, (int)y1] + r[nim - xm, (int)y1]) / 2;
                                rq3 = sr[ix, sx];
                                rq1 = r[(int)x1, (int)y1];
                                gq2 = (g[nim, (int)y1] + g[nim - xm, (int)y1]) / 2;
                                gq3 = sg[ix, sx];
                                gq1 = g[(int)x1, (int)y1];
                                bq2 = (b[nim, (int)y1] + b[nim - xm, (int)y1]) / 2;
                                bq3 = sb[ix, sx];
                                bq1 = b[(int)x1, (int)y1];
                            }
                        }
                        else
                        {
                            if (sx == 0)
                            {
                                x1 = ix * x;
                                ixx = (int)(x1);
                                ixxm = ixx + x;
                                x1 += x50p;       //         
                                y1 = x50p;        //   [1]   2
                                rq1 = sr[ix, 0];  //   1/4   3
                                gq1 = sg[ix, 0];
                                bq1 = sb[ix, 0];

                                x2 = ixxm;
                                y2 = xm;

                                rq3 = r[ixxm, xm];
                                rq2 = (r[ixxm, 0] + r[ixxm, xm]) / 2;
                                rq4 = (double)(sr[ix, 0] + sr[ix, 1]) / 2;
                                gq3 = g[ixxm, xm];
                                gq2 = (g[ixxm, 0] + g[ixxm, xm]) / 2;
                                gq4 = (double)(sg[ix, 0] + sg[ix, 1]) / 2;
                                bq3 = b[ixxm, xm];
                                bq2 = (b[ixxm, 0] + b[ixxm, xm]) / 2;
                                bq4 = (double)(sb[ix, 0] + sb[ix, 1]) / 2;
                            }
                            else
                            {
                                x3 = ix * x;
                                ixx = (int)(x3);  //    1    2
                                y3 = sx * x;      //    
                                sxx = (int)(y3);  //   [4]   3  
                                sxxm = sxx - 1;

                                x2 = ixx + x;
                                y1 = sxxm;
                                rq2 = r[(int)x2, sxxm];
                                gq2 = g[(int)x2, sxxm];
                                bq2 = b[(int)x2, sxxm];

                                x1 = x3 + x50p;
                                y2 = y3 + x50p;
                                if (xEven)
                                {
                                    rq1 = (r[ixx + halfx, sxxm] + r[ixx + halfx - 1, sxxm]) / 2;
                                    gq1 = (g[ixx + halfx, sxxm] + g[ixx + halfx - 1, sxxm]) / 2;
                                    bq1 = (b[ixx + halfx, sxxm] + b[ixx + halfx - 1, sxxm]) / 2;
                                }
                                else
                                {
                                    rq1 = r[ixx + halfx, sxxm];
                                    gq1 = g[ixx + halfx, sxxm];
                                    bq1 = b[ixx + halfx, sxxm];
                                }


                                rq4 = sr[ix, sx];
                                gq4 = sg[ix, sx];
                                bq4 = sb[ix, sx];

                                if (xEven)
                                {
                                    rq3 = (r[(int)x2, sxx + halfx] + r[(int)x2, sxx + halfx - 1]) / 2;
                                    gq3 = (g[(int)x2, sxx + halfx] + g[(int)x2, sxx + halfx - 1]) / 2;
                                    bq3 = (b[(int)x2, sxx + halfx] + b[(int)x2, sxx + halfx - 1]) / 2;
                                }
                                else
                                {
                                    rq3 = r[(int)x2, sxx + halfx];
                                    bq3 = b[(int)x2, sxx + halfx];
                                    gq3 = g[(int)x2, sxx + halfx];
                                }
                            }
                        }

                        for (int i = ix * x; i < (ix + 1) * x; i++)
                        {
                            Parallel.For(sx * x, (sx + 1) * x, s =>
                            {
                                r[i, s] = Bilinear(i, s, x1, y1, x2, y2, rq1, rq2, rq3, rq4);
                                g[i, s] = Bilinear(i, s, x1, y1, x2, y2, gq1, gq2, gq3, gq4);
                                b[i, s] = Bilinear(i, s, x1, y1, x2, y2, bq1, bq2, bq3, bq4);
                                double dist1 = Math.Max(nim - i + s - x, 0);
                                double dist2 = Math.Max(Math.Abs(ixsxpi - i) + Math.Abs(iysxpi - s) - x, 0);
                                double sdist = dist1 + dist2;
                                ri[i, s] = (ri[i, s] * dist1 + r[i, s] * dist2) / sdist;
                                gi[i, s] = (gi[i, s] * dist1 + g[i, s] * dist2) / sdist;
                                bi[i, s] = (bi[i, s] * dist1 + b[i, s] * dist2) / sdist;
                                ris[i, s] = Math.Min(dist1, dist2);
                            });
                        }
                    }
                }

                if (ac > 40)
                {
                    for (int ix = 0; ix < oi; ix++)
                    {
                        ProgressText.Text = ((6000 - 2000 * (oi - ix) / oi) / ac).ToString();
                        for (int sx = 0; sx < os; sx++)
                        {
                            if (ix == 0)
                            {
                                if (sx == 0)
                                {                  //[1]     2
                                    x1 = x50p;     //  
                                    y1 = x50p;     //  
                                    rq1 = sr[0, 0];// 4      3
                                    gq1 = sg[0, 0];
                                    bq1 = sb[0, 0];

                                    x2 = x150p;
                                    y2 = x50p;
                                    rq2 = sr[1, 0];
                                    gq2 = sg[1, 0];
                                    bq2 = sb[1, 0];

                                    x3 = x150p;
                                    y3 = x150p;
                                    rq3 = sr[1, 1];
                                    gq3 = sg[1, 1];
                                    bq3 = sb[1, 1];

                                    x4 = x50p;
                                    y4 = x150p;
                                    rq4 = sr[0, 1];
                                    gq4 = sg[0, 1];
                                    bq4 = sb[0, 1];
                                }
                                else
                                {
                                    y4 = sx * x;
                                    sxxm = (int)(y4) - 1;  // 1   2
                                    x4 = x50p;             //         
                                    y4 += x50p;            //  [4]     3
                                    rq4 = sr[0, sx];
                                    gq4 = sg[0, sx];
                                    bq4 = sb[0, sx];

                                    x3 = x150p;
                                    y3 = y4;
                                    rq3 = sr[1, sx];
                                    gq3 = sg[1, sx];
                                    bq3 = sb[1, sx];

                                    x2 = xm;
                                    y2 = sxxm;
                                    rq2 = r[xm, sxxm];
                                    gq2 = g[xm, sxxm];
                                    bq2 = b[xm, sxxm];

                                    x1 = 0;
                                    y1 = sxxm;
                                    rq1 = r[0, sxxm];
                                    gq1 = g[0, sxxm];
                                    bq1 = b[0, sxxm];
                                }
                            }
                            else
                            {
                                if (sx == 0)
                                {
                                    ixx = (int)(ix * x);
                                    ixxm = ixx - 1;
                                    x2 = ix * x + x50p;//    1   [2]
                                    y2 = x50p;         //  
                                    rq2 = sr[ix, 0];    //    4   2/3
                                    gq2 = sg[ix, 0];
                                    bq2 = sb[ix, 0];

                                    x3 = x2;
                                    y3 = xm;
                                    rq3 = (double)(sr[ix, 1] + sr[ix, 0]) / 2;
                                    gq3 = (double)(sg[ix, 1] + sg[ix, 0]) / 2;
                                    bq3 = (double)(sb[ix, 1] + sb[ix, 0]) / 2;

                                    x1 = ixxm;
                                    y1 = x50p;
                                    rq1 = (r[ixxm, 0] + r[ixxm, xm]) / 2;
                                    gq1 = (g[ixxm, 0] + g[ixxm, xm]) / 2;
                                    bq1 = (b[ixxm, 0] + b[ixxm, xm]) / 2;

                                    x4 = ixxm;
                                    y4 = xm;
                                    rq4 = r[ixxm, xm];
                                    gq4 = g[ixxm, xm];
                                    bq4 = b[ixxm, xm];

                                }
                                else
                                {
                                    ixx = (int)(ix * x);
                                    ixxm = ixx - 1;
                                    sxx = (int)(sx * x);
                                    sxxm = sxx - 1;

                                    x2 = ixx + xm;         //     1         2 
                                    y2 = sxxm;             //   
                                    rq2 = r[(int)x2, sxxm]; //    4    [3]
                                    gq2 = g[(int)x2, sxxm];
                                    bq2 = b[(int)x2, sxxm];

                                    x1 = ixx;
                                    y1 = sxxm;
                                    rq1 = r[ixx, sxxm];
                                    gq1 = g[ixx, sxxm];
                                    bq1 = b[ixx, sxxm];

                                    x3 = ix * x + x50p;
                                    y3 = sx * x + x50p;
                                    rq3 = sr[ix, sx];
                                    gq3 = sg[ix, sx];
                                    bq3 = sb[ix, sx];

                                    x4 = ixxm;
                                    y4 = y3;
                                    if (xEven)
                                    {
                                        rq4 = (r[ixxm, sxx + halfx] + r[ixxm, sxx + halfx - 1]) / 2;
                                        gq4 = (g[ixxm, sxx + halfx] + g[ixxm, sxx + halfx - 1]) / 2;
                                        bq4 = (b[ixxm, sxx + halfx] + b[ixxm, sxx + halfx - 1]) / 2;
                                    }
                                    else
                                    {
                                        rq4 = r[ixxm, sxx + halfx];
                                        gq4 = g[ixxm, sxx + halfx];
                                        bq4 = b[ixxm, sxx + halfx];
                                    }
                                }
                            }

                            for (int i = ix * x; i < (ix + 1) * x; i++)
                            {
                                Parallel.For(sx * x, (sx + 1) * x, s =>
                                {
                                    r[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4);
                                    g[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, gq1, gq2, gq3, gq4);
                                    b[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, bq1, bq2, bq3, bq4);
                                    double dist1 = Math.Max(i + s - x, 0);
                                    double sdist = dist1 + ris[i, s];
                                    ri[i, s] = (ri[i, s] * dist1 + r[i, s] * ris[i, s]) / sdist;
                                    gi[i, s] = (gi[i, s] * dist1 + g[i, s] * ris[i, s]) / sdist;
                                    bi[i, s] = (bi[i, s] * dist1 + b[i, s] * ris[i, s]) / sdist;
                                    ris[i, s] = Math.Min(dist1, ris[i, s]);
                                });
                            }
                        }
                    }

                    if (ac > 60)
                    {
                        int oimm = oim - 1, osmm = osm - 1;

                        for (int ix = oim; ix > -1; ix--)
                        {
                            ProgressText.Text = ((6000 - 1000 * (ix - oim) / oim) / ac).ToString();
                            for (int sx = osm; sx > -1; sx--)
                            {
                                if (ix == oim)
                                {
                                    if (sx == osm)
                                    {                      // 1      2
                                        x3 = oimx;         //  
                                        y3 = osmx;         //  
                                        rq3 = sr[oim, osm];// 4     [3]
                                        gq3 = sg[oim, osm];
                                        bq3 = sb[oim, osm];

                                        x2 = x3;
                                        y2 = y3 - x;
                                        rq2 = sr[oim, osmm];
                                        gq2 = sg[oim, osmm];
                                        bq2 = sb[oim, osmm];

                                        x1 = x3 - x;
                                        y1 = y2;
                                        rq1 = sr[oimm, osmm];
                                        gq1 = sg[oimm, osmm];
                                        bq1 = sb[oimm, osmm];

                                        x4 = x1;
                                        y4 = x3;
                                        rq4 = sr[0, 1];
                                        gq4 = sg[0, 1];
                                        bq4 = sb[0, 1];
                                    }
                                    else
                                    {
                                        sxx = (int)(sx * x);      //1    [2]
                                        x2 = oimx;                //         
                                        y2 = sx * x + x50p;       //   4     3
                                        rq2 = sr[oim, sx];
                                        gq2 = sg[oim, sx];
                                        bq2 = sb[oim, sx];

                                        x3 = nim;
                                        y3 = sxx + x;
                                        rq3 = r[nim, (int)y3];
                                        gq3 = g[nim, (int)y3];
                                        bq3 = b[nim, (int)y3];

                                        x4 = nim - xm;
                                        y4 = y3;
                                        rq4 = r[(int)x4, (int)y4];
                                        gq4 = g[(int)x4, (int)y4];
                                        bq4 = b[(int)x4, (int)y4];

                                        x1 = x2 - x;
                                        y1 = y2;
                                        rq1 = sr[oimm, sx];
                                        gq1 = sg[oimm, sx];
                                        bq1 = sb[oimm, sx];
                                    }
                                }
                                else
                                {
                                    if (sx == osm)
                                    {
                                        ixx = (int)(ix * x);
                                        x4 = ix * x + x50p;//   1/4  2
                                        y4 = osmx;         //  
                                        rq4 = sr[ix, osm]; //   [4]  3
                                        gq4 = sg[ix, osm];
                                        bq4 = sb[ix, osm];

                                        x3 = ixx + x;
                                        y3 = osmx;
                                        rq3 = (r[(int)x3, nsm] + r[(int)x3, nsm - xm]) / 2;
                                        gq3 = (g[(int)x3, nsm] + g[(int)x3, nsm - xm]) / 2;
                                        bq3 = (b[(int)x3, nsm] + b[(int)x3, nsm - xm]) / 2;

                                        x1 = x4;
                                        y1 = y4 - x50p;
                                        rq1 = (double)(sr[ix, osmm] + sr[ix, osm]) / 2;
                                        gq1 = (double)(sg[ix, osmm] + sg[ix, osm]) / 2;
                                        bq1 = (double)(sb[ix, osmm] + sb[ix, osm]) / 2;

                                        x2 = x3;
                                        y2 = y1;
                                        rq2 = r[(int)x2, (int)y2];
                                        gq2 = g[(int)x2, (int)y2];
                                        bq2 = b[(int)x2, (int)y2];
                                    }
                                    else
                                    {
                                        x4 = ix * x;             //          
                                        y4 = sx * x;             //    [1]  2 
                                        ixx = (int)(x4);         //
                                        sxx = (int)(y4);         // 4      3

                                        x2 = x4 + x;
                                        y2 = y4 + x50p;
                                        if (xEven)
                                        {
                                            rq2 = (r[(int)x2, sxx + halfx] + r[(int)x2, sxx + halfx - 1]) / 2;
                                            gq2 = (g[(int)x2, sxx + halfx] + g[(int)x2, sxx + halfx - 1]) / 2;
                                            bq2 = (b[(int)x2, sxx + halfx] + b[(int)x2, sxx + halfx - 1]) / 2;
                                        }
                                        else
                                        {
                                            rq2 = r[(int)x2, sxx + halfx];
                                            gq2 = g[(int)x2, sxx + halfx];
                                            bq2 = b[(int)x2, sxx + halfx];
                                        }

                                        x3 = x2 - 1;
                                        y3 = y4 + x;
                                        rq3 = r[(int)x3, (int)y3];
                                        gq3 = g[(int)x3, (int)y3];
                                        bq3 = b[(int)x3, (int)y3];

                                        x1 = x4 + x50p;
                                        y1 = y2;
                                        rq1 = sr[ix, sx];
                                        gq1 = sg[ix, sx];
                                        bq1 = sb[ix, sx];

                                        y4 = y3;
                                        rq4 = r[ixx, (int)y4];
                                        gq4 = g[ixx, (int)y4];
                                        bq4 = b[ixx, (int)y4];
                                    }
                                }

                                for (int i = ix * x; i < (ix + 1) * x; i++)
                                {
                                    Parallel.For(sx * x, (sx + 1) * x, s =>
                                    {
                                        r[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4);
                                        g[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, gq1, gq2, gq3, gq4);
                                        b[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, bq1, bq2, bq3, bq4);
                                        double dist2 = Math.Max(nim - i + nsm - s - x, 0);
                                        double sdist = ris[i, s] + dist2;
                                        ri[i, s] = (ri[i, s] * dist2 + r[i, s] * ris[i, s]) / sdist;
                                        gi[i, s] = (gi[i, s] * dist2 + g[i, s] * ris[i, s]) / sdist;
                                        bi[i, s] = (bi[i, s] * dist2 + b[i, s] * ris[i, s]) / sdist;
                                        ris[i, s] = Math.Min(ris[i, s], dist2);
                                    });
                                }
                            }
                        }

                        if (ac > 80)
                        {
                            for (int sx = osm; sx > -1; sx--)
                            {
                                ProgressText.Text = ((7000 - 1000 * (sx - osm) / osm) / ac).ToString();
                                for (int ix = 0; ix < oi; ix++)
                                {
                                    if (ix == 0)
                                    {
                                        if (sx == osm)
                                        {                       // 1      2
                                            x1 = x50p;          //  
                                            y1 = osmx - x;      //  
                                            rq1 = sr[0, sx - 1];//[4]     3
                                            gq1 = sg[0, sx - 1];
                                            bq1 = sb[0, sx - 1];

                                            x2 = x150p;
                                            y2 = y1;
                                            rq2 = sr[1, sx - 1];
                                            gq2 = sg[1, sx - 1];
                                            bq2 = sb[1, sx - 1];

                                            x3 = x150p;
                                            y3 = osmx;
                                            rq3 = sr[1, sx];
                                            gq3 = sg[1, sx];
                                            bq3 = sb[1, sx];

                                            x4 = x50p;
                                            y4 = y3;
                                            rq4 = sr[0, sx];
                                            gq4 = sg[0, sx];
                                            bq4 = sb[0, sx];
                                        }
                                        else
                                        {
                                            y1 = sx * x;          //  [1] 1/2
                                            x4 = x50p;            //         
                                            y4 = y1 + x;          //   4   3
                                            rq4 = (r[0, (int)y4] + r[xm, (int)y4]) / 2;
                                            gq4 = (g[0, (int)y4] + g[xm, (int)y4]) / 2;
                                            bq4 = (b[0, (int)y4] + b[xm, (int)y4]) / 2;

                                            x2 = xm;
                                            y2 = y1 + x50p;
                                            rq2 = (double)(sr[1, sx] + sr[0, sx]) / 2;
                                            gq2 = (double)(sg[1, sx] + sg[0, sx]) / 2;
                                            bq2 = (double)(sb[1, sx] + sb[0, sx]) / 2;

                                            x1 = x50p;
                                            y1 = y2;
                                            rq1 = sr[0, sx];
                                            gq1 = sg[0, sx];
                                            bq1 = sb[0, sx];

                                            x3 = xm;
                                            y3 = y4;
                                            rq3 = r[xm, (int)y4];
                                            gq3 = g[xm, (int)y4];
                                            bq3 = b[xm, (int)y4];
                                        }
                                    }
                                    else
                                    {
                                        if (sx == osm)
                                        {
                                            x2 = ix * x;
                                            ixx = (int)(x2);
                                            ixxm = ixx - 1;
                                            x2 += x50p;                                     //        2/3
                                            y2 = ns - x - 0.5;                              //    1   
                                            rq2 = (double)(sr[ix, osmm] + sr[ix, osm]) / 2; //    4   [3]
                                            gq2 = (double)(sg[ix, osmm] + sg[ix, osm]) / 2;
                                            bq2 = (double)(sb[ix, osmm] + sb[ix, osm]) / 2;

                                            x3 = x2;
                                            y3 = osmx;
                                            rq3 = sr[ix, sx];
                                            gq3 = sg[ix, sx];
                                            bq3 = sb[ix, sx];

                                            x1 = ixxm;
                                            y1 = ns - x;
                                            rq1 = r[ixxm, (int)y1];
                                            gq1 = g[ixxm, (int)y1];
                                            bq1 = b[ixxm, (int)y1];

                                            x4 = ixxm;
                                            y4 = osmx;
                                            rq4 = (r[ixxm, ns - halfx] + r[ixxm, ns - halfx - 1]) / 2;
                                            gq4 = (g[ixxm, ns - halfx] + g[ixxm, ns - halfx - 1]) / 2;
                                            bq4 = (b[ixxm, ns - halfx] + b[ixxm, ns - halfx - 1]) / 2;
                                        }
                                        else
                                        {
                                            x2 = ix * x;
                                            ixx = (int)(x2);
                                            ixxm = ixx - 1;
                                            y2 = sx * x;
                                            sxx = (int)(y2);

                                            x4 = ixxm;
                                            y4 = y2 + x;              //    1   [2]
                                            rq4 = r[ixxm, (int)y4];   //    4    3
                                            gq4 = g[ixxm, (int)y4];
                                            bq4 = b[ixxm, (int)y4];

                                            x1 = ixxm;
                                            y1 = y2 + x50p;
                                            if (xEven)
                                            {
                                                rq1 = (r[ixxm, sxx + halfx] + r[ixxm, sxx + halfx - 1]) / 2;
                                                gq1 = (g[ixxm, sxx + halfx] + g[ixxm, sxx + halfx - 1]) / 2;
                                                bq1 = (b[ixxm, sxx + halfx] + b[ixxm, sxx + halfx - 1]) / 2;
                                            }
                                            else
                                            {
                                                rq1 = r[ixxm, sxx + halfx];
                                                gq1 = g[ixxm, sxx + halfx];
                                                bq1 = b[ixxm, sxx + halfx];
                                            }


                                            x2 += x50p;
                                            y2 = y1;
                                            rq2 = sr[ix, sx];
                                            gq2 = sg[ix, sx];
                                            bq2 = sb[ix, sx];

                                            x3 = x2;
                                            y3 = y4;
                                            if (xEven)
                                            {
                                                rq3 = (r[ixx + halfx, (int)y3] + r[ixx + halfx - 1, (int)y3]) / 2;
                                                gq3 = (g[ixx + halfx, (int)y3] + g[ixx + halfx - 1, (int)y3]) / 2;
                                                bq3 = (b[ixx + halfx, (int)y3] + b[ixx + halfx - 1, (int)y3]) / 2;
                                            }
                                            else
                                            {
                                                rq3 = r[ixx + halfx, (int)y3];
                                                gq3 = g[ixx + halfx, (int)y3];
                                                bq3 = b[ixx + halfx, (int)y3];
                                            }
                                        }
                                    }

                                    for (int i = ix * x; i < (ix + 1) * x; i++)
                                    {
                                        Parallel.For(sx * x, (sx + 1) * x, s =>
                                        {
                                            r[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4);
                                            g[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, gq1, gq2, gq3, gq4);
                                            b[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, bq1, bq2, bq3, bq4);
                                            double dist2 = Math.Max(i + nsm - s - x, 0);
                                            double sdist = ris[i, s] + dist2;
                                            ri[i, s] = (ri[i, s] * dist2 + r[i, s] * ris[i, s]) / sdist;
                                            gi[i, s] = (gi[i, s] * dist2 + g[i, s] * ris[i, s]) / sdist;
                                            bi[i, s] = (bi[i, s] * dist2 + b[i, s] * ris[i, s]) / sdist;
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }


            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    if (ri[i, s] < rmaax2)
                    {
                        dr = rmin;
                    }
                    else if (ri[i, s] > rpaax2)
                    {
                        dr = rmax;
                    }
                    else
                    {
                        dr = (byte)((ri[i, s] - rmaax2) / aax * rex + rmin + 0.5);
                    }

                    if (gi[i, s] < gmaax2)
                    {
                        dg = gmin;
                    }
                    else if (gi[i, s] > gpaax2)
                    {
                        dg = gmax;
                    }
                    else
                    {
                        dg = (byte)((gi[i, s] - gmaax2) / aax * gex + gmin + 0.5);
                    }

                    if (bi[i, s] < bmaax2)
                    {
                        db = bmin;
                    }
                    else if (bi[i, s] > bpaax2)
                    {
                        db = bmax;
                    }
                    else
                    {
                        db = (byte)((bi[i, s] - bmaax2) / aax * bex + bmin + 0.5);
                    }
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb(dr, dg, db));
                }
            }
            return img;
        }

        static double Quadrilateral(int x, int y, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double q1, double q2, double q3, double q4)
        {
            double r1, r2, ry1, ry2, r12, r21, r34, r43, d21, d34, dry21;
            d21 = x2 - x1;
            d34 = x3 - x4;
            r21 = (x2 - x) / d21;
            r12 = (x - x1) / d21;
            r43 = (x - x4) / d34;
            r34 = (x3 - x) / d34;
            r1 = r21 * q1 + r12 * q2;
            r2 = r34 * q4 + r43 * q3;
            ry1 = r21 * y1 + r12 * y2;
            ry2 = r34 * y4 + r43 * y3;
            dry21 = ry2 - ry1;
            return (ry2 - y) / dry21 * r1 + (y - ry1) / dry21 * r2;
        }

        static double Bilinear(int x, int y, double x1, double y1, double x2, double y2, double q1, double q2, double q3, double q4)
        {
            double r1, r2, r12, r21, d21, y21;
            d21 = x2 - x1;
            r21 = (x2 - x) / d21;
            r12 = (x - x1) / d21;
            r1 = r21 * q1 + r12 * q2;
            r2 = r21 * q4 + r12 * q3;
            y21 = y2 - y1;
            return (y2 - y) / y21 * r1 + (y - y1) / y21 * r2;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;

            if (radioButton1.Checked)
            {
                pictureBox1.Image = ScaleBilinearInterExtraGray(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
            }
            else
            {
                pictureBox1.Image = ScaleBilinearInterExtraColor(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
            }

            ProgressText.Text = "100";

            pictureBox1.Refresh();
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;
            button9.Enabled = true;
        }

        private Image ScaleBilinearInterExtraGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, xm, oim, osm, sxx, sxxm, ixx, ixxm;
            double x50p;
            double halfx = (double)x / 2;
            x50p = halfx - 0.5;

            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            xm = x - 1;
            byte d;
            double[,] r = new double[ni, ns];
            double[,] ri = new double[ni, ns];
            double[,] ris = new double[ni, ns];
            byte[,] sr = new byte[oi, os];

            for (int ix = 0; ix < oi; ix++)
            {
                for (int sx = 0; sx < os; sx++)
                {
                    sr[ix, sx] = ((Bitmap)img).GetPixel(ix, sx).R;
                }
            }

            img = new Bitmap(img, new Size(ni, ns));

            double xs = oim / 2d, ys = osm / 2d;
            double nxs = (ni - 1) / 2d, nys = (ns - 1) / 2d;
            
            double ki = oim * ac / 100;
            double ks = osm * ac / 100;
            double ki2 = ki - 1;
            double ks2 = ks - 1;
            
            if (ac < 100)
            {
                for (int kki = 0; kki < oim; kki += 2)
                {
                    ProgressText.Text = ((int)(kki * 100 / oim / (ki + 2))).ToString();
                    for (int kks = 0; kks < osm; kks += 2)
                    {
                        double xsx = kki * x + x50p;
                        double xsxp = xsx + x;
                        double ysx = kks * x + x50p;
                        double ysxp = ysx + x;
                        int ys2x = (kks + 2) * x;

                        int iysx = kks * x;
                        int iysxp = iysx + xm;
                        int iysxpi = iysxp + 1;
                        int iysxpp = iysxpi + xm;

                        int ixsx = kki * x;
                        int ixsxp = ixsx + xm;
                        int ixsxpi = ixsxp + 1;
                        double cx = ixsxp + 0.5;
                        double cy = iysxp + 0.5;
                        int ixsxpp = ixsxpi + xm;
                        int xsp = kki + 1, ysp = kks + 1;

                        for (int i = ixsx; i < (kki + 2) * x; i++)
                        {
                            Parallel.For(iysx, ys2x, s =>
                            {
                                ri[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks], sr[xsp, kks], sr[xsp, ysp], sr[kki, ysp]);
                                ris[i, s] = Math.Pow(2,Math.Max(Math.Abs(i - nxs)/ki , Math.Abs(s - nys)/ks)-x);
                            });
                        }
                    }
                }
            }
            
            for (int kki = (int)(xs - ki / 2); kki < xs + ki / 2 - 0.5; kki++)
            {
                ProgressText.Text = ((int)((kki - (int)(xs - ki / 2) + 1) / (ki + 2) * 100)).ToString();
                for (int kks = (int)(ys - ks / 2); kks < ys + ks / 2 - 0.5; kks++)
                {
                    double xsx = kki * x + x50p;
                    double xsxp = xsx + x;
                    double ysx = kks * x + x50p;
                    double ysxp = ysx + x;
                    int ys2x = (kks + 2) * x;

                    int iysx = kks * x;
                    int iysxp = iysx + xm;
                    int iysxpi = iysxp + 1;
                    int iysxpp = iysxpi + xm;

                    int ixsx = kki * x;
                    int ixsxp = ixsx + xm;
                    int ixsxpi = ixsxp + 1;
                    double cx = ixsxp + 0.5;
                    double cy = iysxp + 0.5;
                    int ixsxpp = ixsxpi + xm;
                    int xsp = kki + 1, ysp = kks + 1;

                    for (int i = ixsx; i < (kki + 2) * x; i++)
                    {
                        Parallel.For(iysx, ys2x, s =>
                        {
                            r[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks], sr[xsp, kks], sr[xsp, ysp], sr[kki, ysp]);
                            double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                            ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                            ris[i, s] = dist + ris[i, s];
                        });
                    }
                    
                    for (int ix = kki + 2; ix < kki + 2 + ki2 && ix < oi; ix++)
                    {
                        int dixxm = ix * x;
                        ixxm = dixxm - 1;
                        double dixx = (ix + 1) * x;
                        double dixxp = dixxm + x50p;

                        for (int i = dixxm; i < dixx; i++)
                        {
                            Parallel.For(iysx, ys2x, s =>
                            {
                                r[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (r[ixxm, iysx] + r[ixxm, iysxp]) / 2, sr[ix, kks], sr[ix, kks + 1], (r[ixxm, iysxpi] + r[ixxm, iysxpp]) / 2);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                ris[i, s] = dist + ris[i, s];
                            });
                        }
                    }

                    for (int ix = kki - 1; ix > kki - 1 - ki2 && ix > -1; ix--)
                    {
                        int dixx = ix * x;
                        int dixxp = dixx + x;
                        double dixxm = dixx + x50p;

                        for (int i = dixx; i < dixxp; i++)
                        {
                            Parallel.For(iysx, ys2x, s =>
                            {
                                r[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, kks], (r[dixxp, iysx] + r[dixxp, iysxp]) / 2, (r[dixxp, iysxpi] + r[dixxp, iysxpp]) / 2, sr[ix, kks + 1]);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                ris[i, s] = dist + ris[i, s];
                            });
                        }
                    }

                    int xs2x = (kki + 2) * x;
                    for (int sx = kks + 2; sx < kks + 2 + ks2 && sx < os; sx++)
                    {
                        int dsxxm = sx * x;
                        sxxm = (int)dsxxm - 1;
                        int dsxx = (sx + 1) * x;
                        double dsxxp = dsxxm + x50p;

                        for (int i = ixsx; i < xs2x; i++)
                        {
                            Parallel.For(dsxxm, dsxx, s =>
                            {
                                r[i, s] = Bilinear(i, s, xsx, sxxm, xsxp, dsxxp, (r[ixsx, sxxm] + r[ixsxp, sxxm]) / 2, (r[ixsxpi, sxxm] + r[ixsxpp, sxxm]) / 2, sr[kki + 1, sx], sr[kki, sx]);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                ris[i, s] = dist + ris[i, s];
                            });
                        }
                    }

                    for (int sx = kks - 1; sx > kks - 1 - ks2 && sx > -1; sx--)
                    {
                        int dsxx = sx * x;
                        sxxm = dsxx + x;
                        double dsxxm = dsxx + x50p;

                        for (int i = ixsx; i < xs2x; i++)
                        {
                            Parallel.For(dsxx, sxxm, s =>
                            {
                                r[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sr[kki, sx], sr[xsp, sx], (r[ixsxpi, sxxm] + r[ixsxpp, sxxm]) / 2, (r[ixsx, sxxm] + r[ixsxp, sxxm]) / 2);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx),2);
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                ris[i, s] = dist + ris[i, s];
                            });
                        }
                    }

                    for (int sx = kks + 2; sx < kks + 2 + ks2 && sx < os; sx++)
                    {
                        for (int ix = kki + 2; ix < kki + 2 + ki2 && ix < oi; ix++)
                        {
                            int dsxxm = sx * x;
                            sxxm = dsxxm - 1;
                            int dsxx = (sx + 1) * x;
                            int isxx = dsxx - 1;
                            double dsxxp = dsxxm + x50p;
                            int dixxm = ix * x;
                            ixxm = dixxm - 1;
                            int dixx = (ix + 1) * x;
                            int iixx = dixx - 1;
                            double dixxp = dixxm + x50p;

                            for (int i = dixxm; i < dixx; i++)
                            {
                                Parallel.For(dsxxm, dsxx, s =>
                                {
                                    r[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, r[ixxm, sxxm], (r[dixxm, sxxm] + r[iixx, sxxm]) / 2, sr[ix, sx], (r[ixxm, dsxxm] + r[ixxm, isxx]) / 2);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                    ris[i, s] = dist + ris[i, s];
                                });
                            }
                        }
                    }

                    for (int sx = kks - 1; sx > kks - 1 - ks2 && sx > -1; sx--)
                    {
                        for (int ix = kki - 1; ix > kki - 1 - ki2 && ix > -1; ix--)
                        {
                            int dsxxm = sx * x;
                            sxxm = (sx + 1) * x;
                            int isxx = sxxm - 1;
                            double dsxxp = dsxxm + x50p;
                            int dixxm = ix * x;
                            ixxm = (ix + 1) * x;
                            int iixx = ixxm - 1;
                            double dixxp = dixxm + x50p;

                            for (int i = dixxm; i < ixxm; i++)
                            {
                                Parallel.For(dsxxm, sxxm, s =>
                                {
                                    r[i, s] = Bilinear(i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx], (r[ixxm, dsxxm] + r[ixxm, isxx]) / 2, r[ixxm, sxxm], (r[dixxm, sxxm] + r[iixx, sxxm]) / 2);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                    ris[i, s] = dist + ris[i, s];
                                });
                            }
                        }
                    }

                    for (int sx = kks + 2; sx < os && sx < kks + 2 + ks2; sx++)
                    {
                        for (int ix = kki - 1; ix > -1 && ix > kki - 1 - ki2; ix--)
                        {
                            sxx = sx * x;
                            int isxxm = sxx - 1;
                            int dsxx = (sx + 1) * x;
                            int isxx = dsxx - 1;
                            double dsxxp = sxx + x50p;
                            int dixxm = ix * x;
                            int dixx = dixxm + x;
                            int iixx = dixx - 1;
                            double dixxp = dixxm + x50p;

                            for (int i = dixxm; i < dixx; i++)
                            {
                                Parallel.For(sxx, dsxx, s =>
                                {
                                    r[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (r[dixxm, isxxm] + r[iixx, isxxm]) / 2, r[dixx, isxxm], (r[dixx, isxx] + r[dixx, sxx]) / 2, sr[ix, sx]);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                    ris[i, s] = dist + ris[i, s];
                                });
                            }
                        }
                    }

                    for (int ix = kki + 2; ix < oi && ix < kki + 2 + ki2; ix++)
                    {
                        for (int sx = kks - 1; sx > -1 && sx > kks - 1 - ks2; sx--)
                        {
                            sxx = sx * x;
                            int dsxx = sxx + x;
                            int isxx = dsxx - 1;
                            double dsxxp = sxx + x50p;
                            ixx = ix * x;
                            int dixx = ixx + x;
                            ixxm = ixx - 1;
                            int iixx = dixx - 1;
                            double dixxp = ixx + x50p;

                            for (int i = ixx; i < dixx; i++)
                            {
                                Parallel.For(sxx, dsxx, s =>
                                {
                                    r[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (r[ixxm, sxx] + r[ixxm, isxx]) / 2, sr[ix, sx], (r[ixx, dsxx] + r[iixx, dsxx]) / 2, r[ixxm, dsxx]);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                    ris[i, s] = dist + ris[i, s];
                                });
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    if (ri[i, s] < 0.5)
                        d = 0;
                    else if (ri[i, s] > 254.5)
                        d = 255;
                    else
                        d = (byte)(ri[i, s] + 0.5);
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb(d, d, d));
                }
            }

            return img;
        }

        private Image ScaleBilinearInterExtraColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, xm, oim, osm, sxx, sxxm, ixx, ixxm;
            double x50p;
            double halfx = (double)x / 2;
            x50p = halfx - 0.5;

            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            xm = x - 1;
            byte dr, dg, db;
            double[,] r = new double[ni, ns];
            double[,] g = new double[ni, ns];
            double[,] b = new double[ni, ns];
            double[,] ri = new double[ni, ns];
            double[,] gi = new double[ni, ns];
            double[,] bi = new double[ni, ns];
            double[,] ris = new double[ni, ns];
            byte[,] sr = new byte[oi, os];
            byte[,] sg = new byte[oi, os];
            byte[,] sb = new byte[oi, os];

            for (int ix = 0; ix < oi; ix++)
            {
                for (int sx = 0; sx < os; sx++)
                {
                    sr[ix, sx] = ((Bitmap)img).GetPixel(ix, sx).R;
                    sg[ix, sx] = ((Bitmap)img).GetPixel(ix, sx).G;
                    sb[ix, sx] = ((Bitmap)img).GetPixel(ix, sx).B;
                }
            }

            img = new Bitmap(img, new Size(ni, ns));

            double xs = oim / 2d, ys = osm / 2d;
            double nxs = (ni - 1) / 2d, nys = (ns - 1) / 2d;

            double ki = oim * ac / 100;
            double ks = osm * ac / 100;
            double ki2 = ki - 1;
            double ks2 = ks - 1;

            if (ac < 100)
            {
                for (int kki = 0; kki < oim; kki += 2)
                {
                    ProgressText.Text = ((int)(kki * 100 / oim / (ki + 2))).ToString();
                    for (int kks = 0; kks < osm; kks += 2)
                    {
                        double xsx = kki * x + x50p;
                        double xsxp = xsx + x;
                        double ysx = kks * x + x50p;
                        double ysxp = ysx + x;
                        int ys2x = (kks + 2) * x;

                        int iysx = kks * x;
                        int iysxp = iysx + xm;
                        int iysxpi = iysxp + 1;
                        int iysxpp = iysxpi + xm;

                        int ixsx = kki * x;
                        int ixsxp = ixsx + xm;
                        int ixsxpi = ixsxp + 1;
                        double cx = ixsxp + 0.5;
                        double cy = iysxp + 0.5;
                        int ixsxpp = ixsxpi + xm;
                        int xsp = kki + 1, ysp = kks + 1;

                        for (int i = ixsx; i < (kki + 2) * x; i++)
                        {
                            Parallel.For(iysx, ys2x, s =>
                            {
                                ri[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks], sr[xsp, kks], sr[xsp, ysp], sr[kki, ysp]);
                                gi[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sg[kki, kks], sg[xsp, kks], sg[xsp, ysp], sb[kki, ysp]);
                                bi[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sb[kki, kks], sb[xsp, kks], sb[xsp, ysp], sb[kki, ysp]);
                                ris[i, s] = Math.Pow(2, Math.Max(Math.Abs(i - nxs) / ki, Math.Abs(s - nys) / ks) - x);
                            });
                        }
                    }
                }
            }

            for (int kki = (int)(xs - ki / 2); kki < xs + ki / 2 - 0.5; kki++)
            {
                ProgressText.Text = ((int)((kki - (int)(xs - ki / 2) + 1) / (ki + 2) * 100)).ToString();
                for (int kks = (int)(ys - ks / 2); kks < ys + ks / 2 - 0.5; kks++)
                {
                    double xsx = kki * x + x50p;
                    double xsxp = xsx + x;
                    double ysx = kks * x + x50p;
                    double ysxp = ysx + x;
                    int ys2x = (kks + 2) * x;

                    int iysx = kks * x;
                    int iysxp = iysx + xm;
                    int iysxpi = iysxp + 1;
                    int iysxpp = iysxpi + xm;

                    int ixsx = kki * x;
                    int ixsxp = ixsx + xm;
                    int ixsxpi = ixsxp + 1;
                    double cx = ixsxp + 0.5;
                    double cy = iysxp + 0.5;
                    int ixsxpp = ixsxpi + xm;
                    int xsp = kki + 1, ysp = kks + 1;

                    for (int i = ixsx; i < (kki + 2) * x; i++)
                    {
                        Parallel.For(iysx, ys2x, s =>
                        {
                            r[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks], sr[xsp, kks], sr[xsp, ysp], sr[kki, ysp]);
                            g[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sg[kki, kks], sg[xsp, kks], sg[xsp, ysp], sg[kki, ysp]);
                            b[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sb[kki, kks], sb[xsp, kks], sb[xsp, ysp], sb[kki, ysp]);
                            double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                            double sdist = dist + ris[i, s];
                            ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / sdist;
                            gi[i, s] = (g[i, s] * dist + gi[i, s] * ris[i, s]) / sdist;
                            bi[i, s] = (b[i, s] * dist + bi[i, s] * ris[i, s]) / sdist;
                            ris[i, s] = sdist;
                        });
                    }

                    for (int ix = kki + 2; ix < kki + 2 + ki2 && ix < oi; ix++)
                    {
                        int dixxm = ix * x;
                        ixxm = dixxm - 1;
                        double dixx = (ix + 1) * x;
                        double dixxp = dixxm + x50p;

                        for (int i = dixxm; i < dixx; i++)
                        {
                            Parallel.For(iysx, ys2x, s =>
                            {
                                r[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (r[ixxm, iysx] + r[ixxm, iysxp]) / 2, sr[ix, kks], sr[ix, kks + 1], (r[ixxm, iysxpi] + r[ixxm, iysxpp]) / 2);
                                g[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (g[ixxm, iysx] + g[ixxm, iysxp]) / 2, sg[ix, kks], sg[ix, kks + 1], (g[ixxm, iysxpi] + g[ixxm, iysxpp]) / 2);
                                b[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (b[ixxm, iysx] + b[ixxm, iysxp]) / 2, sb[ix, kks], sr[ix, kks + 1], (b[ixxm, iysxpi] + b[ixxm, iysxpp]) / 2);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                double sdist = dist + ris[i, s];
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / sdist;
                                gi[i, s] = (g[i, s] * dist + gi[i, s] * ris[i, s]) / sdist;
                                bi[i, s] = (b[i, s] * dist + bi[i, s] * ris[i, s]) / sdist;
                                ris[i, s] = sdist;
                            });
                        }
                    }

                    for (int ix = kki - 1; ix > kki - 1 - ki2 && ix > -1; ix--)
                    {
                        int dixx = ix * x;
                        int dixxp = dixx + x;
                        double dixxm = dixx + x50p;

                        for (int i = dixx; i < dixxp; i++)
                        {
                            Parallel.For(iysx, ys2x, s =>
                            {
                                r[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, kks], (r[dixxp, iysx] + r[dixxp, iysxp]) / 2, (r[dixxp, iysxpi] + r[dixxp, iysxpp]) / 2, sr[ix, kks + 1]);
                                g[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sg[ix, kks], (g[dixxp, iysx] + g[dixxp, iysxp]) / 2, (g[dixxp, iysxpi] + g[dixxp, iysxpp]) / 2, sg[ix, kks + 1]);
                                b[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, kks], (b[dixxp, iysx] + b[dixxp, iysxp]) / 2, (b[dixxp, iysxpi] + b[dixxp, iysxpp]) / 2, sb[ix, kks + 1]);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                double sdist = dist + ris[i, s];
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / sdist;
                                gi[i, s] = (g[i, s] * dist + gi[i, s] * ris[i, s]) / sdist;
                                bi[i, s] = (b[i, s] * dist + bi[i, s] * ris[i, s]) / sdist;
                                ris[i, s] = sdist;
                            });
                        }
                    }

                    int xs2x = (kki + 2) * x;
                    for (int sx = kks + 2; sx < kks + 2 + ks2 && sx < os; sx++)
                    {
                        int dsxxm = sx * x;
                        sxxm = (int)dsxxm - 1;
                        int dsxx = (sx + 1) * x;
                        double dsxxp = dsxxm + x50p;

                        for (int i = ixsx; i < xs2x; i++)
                        {
                            Parallel.For(dsxxm, dsxx, s =>
                            {
                                r[i, s] = Bilinear(i, s, xsx, sxxm, xsxp, dsxxp, (r[ixsx, sxxm] + r[ixsxp, sxxm]) / 2, (r[ixsxpi, sxxm] + r[ixsxpp, sxxm]) / 2, sr[kki + 1, sx], sr[kki, sx]);
                                g[i, s] = Bilinear(i, s, xsx, sxxm, xsxp, dsxxp, (g[ixsx, sxxm] + g[ixsxp, sxxm]) / 2, (g[ixsxpi, sxxm] + g[ixsxpp, sxxm]) / 2, sg[kki + 1, sx], sg[kki, sx]);
                                b[i, s] = Bilinear(i, s, xsx, sxxm, xsxp, dsxxp, (b[ixsx, sxxm] + b[ixsxp, sxxm]) / 2, (b[ixsxpi, sxxm] + b[ixsxpp, sxxm]) / 2, sb[kki + 1, sx], sb[kki, sx]);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                double sdist = dist + ris[i, s];
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / sdist;
                                gi[i, s] = (g[i, s] * dist + gi[i, s] * ris[i, s]) / sdist;
                                bi[i, s] = (b[i, s] * dist + bi[i, s] * ris[i, s]) / sdist;
                                ris[i, s] = sdist;
                            });
                        }
                    }

                    for (int sx = kks - 1; sx > kks - 1 - ks2 && sx > -1; sx--)
                    {
                        int dsxx = sx * x;
                        sxxm = dsxx + x;
                        double dsxxm = dsxx + x50p;

                        for (int i = ixsx; i < xs2x; i++)
                        {
                            Parallel.For(dsxx, sxxm, s =>
                            {
                                r[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sr[kki, sx], sr[xsp, sx], (r[ixsxpi, sxxm] + r[ixsxpp, sxxm]) / 2, (r[ixsx, sxxm] + r[ixsxp, sxxm]) / 2);
                                g[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sg[kki, sx], sg[xsp, sx], (g[ixsxpi, sxxm] + g[ixsxpp, sxxm]) / 2, (g[ixsx, sxxm] + g[ixsxp, sxxm]) / 2);
                                b[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sb[kki, sx], sb[xsp, sx], (b[ixsxpi, sxxm] + b[ixsxpp, sxxm]) / 2, (b[ixsx, sxxm] + b[ixsxp, sxxm]) / 2);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                double sdist = dist + ris[i, s];
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / sdist;
                                gi[i, s] = (g[i, s] * dist + gi[i, s] * ris[i, s]) / sdist;
                                bi[i, s] = (b[i, s] * dist + bi[i, s] * ris[i, s]) / sdist;
                                ris[i, s] = sdist;
                            });
                        }
                    }

                    for (int sx = kks + 2; sx < kks + 2 + ks2 && sx < os; sx++)
                    {
                        for (int ix = kki + 2; ix < kki + 2 + ki2 && ix < oi; ix++)
                        {
                            int dsxxm = sx * x;
                            sxxm = dsxxm - 1;
                            int dsxx = (sx + 1) * x;
                            int isxx = dsxx - 1;
                            double dsxxp = dsxxm + x50p;
                            int dixxm = ix * x;
                            ixxm = dixxm - 1;
                            int dixx = (ix + 1) * x;
                            int iixx = dixx - 1;
                            double dixxp = dixxm + x50p;

                            for (int i = dixxm; i < dixx; i++)
                            {
                                Parallel.For(dsxxm, dsxx, s =>
                                {
                                    r[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, r[ixxm, sxxm], (r[dixxm, sxxm] + r[iixx, sxxm]) / 2, sr[ix, sx], (r[ixxm, dsxxm] + r[ixxm, isxx]) / 2);
                                    g[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, g[ixxm, sxxm], (g[dixxm, sxxm] + g[iixx, sxxm]) / 2, sg[ix, sx], (g[ixxm, dsxxm] + g[ixxm, isxx]) / 2);
                                    b[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, b[ixxm, sxxm], (b[dixxm, sxxm] + b[iixx, sxxm]) / 2, sb[ix, sx], (b[ixxm, dsxxm] + b[ixxm, isxx]) / 2);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    double sdist = dist + ris[i, s];
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / sdist;
                                    gi[i, s] = (g[i, s] * dist + gi[i, s] * ris[i, s]) / sdist;
                                    bi[i, s] = (b[i, s] * dist + bi[i, s] * ris[i, s]) / sdist;
                                    ris[i, s] = sdist;
                                });
                            }
                        }
                    }

                    for (int sx = kks - 1; sx > kks - 1 - ks2 && sx > -1; sx--)
                    {
                        for (int ix = kki - 1; ix > kki - 1 - ki2 && ix > -1; ix--)
                        {
                            int dsxxm = sx * x;
                            sxxm = (sx + 1) * x;
                            int isxx = sxxm - 1;
                            double dsxxp = dsxxm + x50p;
                            int dixxm = ix * x;
                            ixxm = (ix + 1) * x;
                            int iixx = ixxm - 1;
                            double dixxp = dixxm + x50p;

                            for (int i = dixxm; i < ixxm; i++)
                            {
                                Parallel.For(dsxxm, sxxm, s =>
                                {
                                    r[i, s] = Bilinear(i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx], (r[ixxm, dsxxm] + r[ixxm, isxx]) / 2, r[ixxm, sxxm], (r[dixxm, sxxm] + r[iixx, sxxm]) / 2);
                                    g[i, s] = Bilinear(i, s, dixxp, dsxxp, ixxm, sxxm, sg[ix, sx], (g[ixxm, dsxxm] + g[ixxm, isxx]) / 2, g[ixxm, sxxm], (g[dixxm, sxxm] + g[iixx, sxxm]) / 2);
                                    b[i, s] = Bilinear(i, s, dixxp, dsxxp, ixxm, sxxm, sb[ix, sx], (b[ixxm, dsxxm] + b[ixxm, isxx]) / 2, b[ixxm, sxxm], (b[dixxm, sxxm] + b[iixx, sxxm]) / 2);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    double sdist = dist + ris[i, s];
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / sdist;
                                    gi[i, s] = (g[i, s] * dist + gi[i, s] * ris[i, s]) / sdist;
                                    bi[i, s] = (b[i, s] * dist + bi[i, s] * ris[i, s]) / sdist;
                                    ris[i, s] = sdist;
                                });
                            }
                        }
                    }

                    for (int sx = kks + 2; sx < os && sx < kks + 2 + ks2; sx++)
                    {
                        for (int ix = kki - 1; ix > -1 && ix > kki - 1 - ki2; ix--)
                        {
                            sxx = sx * x;
                            int isxxm = sxx - 1;
                            int dsxx = (sx + 1) * x;
                            int isxx = dsxx - 1;
                            double dsxxp = sxx + x50p;
                            int dixxm = ix * x;
                            int dixx = dixxm + x;
                            int iixx = dixx - 1;
                            double dixxp = dixxm + x50p;

                            for (int i = dixxm; i < dixx; i++)
                            {
                                Parallel.For(sxx, dsxx, s =>
                                {
                                    r[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (r[dixxm, isxxm] + r[iixx, isxxm]) / 2, r[dixx, isxxm], (r[dixx, isxx] + r[dixx, sxx]) / 2, sr[ix, sx]);
                                    g[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (g[dixxm, isxxm] + g[iixx, isxxm]) / 2, g[dixx, isxxm], (g[dixx, isxx] + g[dixx, sxx]) / 2, sg[ix, sx]);
                                    b[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (b[dixxm, isxxm] + b[iixx, isxxm]) / 2, b[dixx, isxxm], (b[dixx, isxx] + b[dixx, sxx]) / 2, sb[ix, sx]);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    double sdist = dist + ris[i, s];
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / sdist;
                                    gi[i, s] = (g[i, s] * dist + gi[i, s] * ris[i, s]) / sdist;
                                    bi[i, s] = (b[i, s] * dist + bi[i, s] * ris[i, s]) / sdist;
                                    ris[i, s] = sdist;
                                });
                            }
                        }
                    }

                    for (int ix = kki + 2; ix < oi && ix < kki + 2 + ki2; ix++)
                    {
                        for (int sx = kks - 1; sx > -1 && sx > kks - 1 - ks2; sx--)
                        {
                            sxx = sx * x;
                            int dsxx = sxx + x;
                            int isxx = dsxx - 1;
                            double dsxxp = sxx + x50p;
                            ixx = ix * x;
                            int dixx = ixx + x;
                            ixxm = ixx - 1;
                            int iixx = dixx - 1;
                            double dixxp = ixx + x50p;

                            for (int i = ixx; i < dixx; i++)
                            {
                                Parallel.For(sxx, dsxx, s =>
                                {
                                    r[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (r[ixxm, sxx] + r[ixxm, isxx]) / 2, sr[ix, sx], (r[ixx, dsxx] + r[iixx, dsxx]) / 2, r[ixxm, dsxx]);
                                    g[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (g[ixxm, sxx] + g[ixxm, isxx]) / 2, sg[ix, sx], (g[ixx, dsxx] + g[iixx, dsxx]) / 2, g[ixxm, dsxx]);
                                    b[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (b[ixxm, sxx] + b[ixxm, isxx]) / 2, sb[ix, sx], (b[ixx, dsxx] + b[iixx, dsxx]) / 2, b[ixxm, dsxx]);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    double sdist = dist + ris[i, s];
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / sdist;
                                    gi[i, s] = (g[i, s] * dist + gi[i, s] * ris[i, s]) / sdist;
                                    bi[i, s] = (b[i, s] * dist + bi[i, s] * ris[i, s]) / sdist;
                                    ris[i, s] = sdist;
                                });
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    if (ri[i, s] < 0.5)
                        dr = 0;
                    else if (ri[i, s] > 254.5)
                        dr = 255;
                    else
                        dr = (byte)(ri[i, s] + 0.5);

                    if (gi[i, s] < 0.5)
                        dg = 0;
                    else if (gi[i, s] > 254.5)
                        dg = 255;
                    else
                        dg = (byte)(gi[i, s] + 0.5);

                    if (bi[i, s] < 0.5)
                        db = 0;
                    else if (bi[i, s] > 254.5)
                        db = 255;
                    else
                        db = (byte)(bi[i, s] + 0.5);
                    ((Bitmap)img).SetPixel(i, s, Color.FromArgb(dr, dg, db));
                }
            }

            return img;
        }


        static double Dist4(int i, int s, double cx, double cy, double x2)
        {
            if (i < cx)
            {
                if (s < cy)
                {
                    return Math.Max(Math.Abs(i - cx + x2) + Math.Abs(s - cy + x2),0.0001d);
                }
                else
                {
                    return Math.Max(Math.Abs(i - cx + x2) + Math.Abs(s - cy - x2),0.0001d);
                }
            }
            else
            {
                if (s < cy)
                {
                    return Math.Max(Math.Abs(i - cx - x2) + Math.Abs(s - cy + x2),0.0001d);
                }
                else
                {
                    return Math.Max(Math.Abs(i - cx - x2) + Math.Abs(s - cy - x2),0.0001d);
                }
            }
        }

        private void Progress(object sender, EventArgs e)
        {
            if (ProgressText.Text!=pp)
            {
                pp = ProgressText.Text;
                ProgressText.Refresh();
                TaskbarProgress.SetValue(this.Handle, Int32.Parse(ProgressText.Text), 100);
                Application.DoEvents();
                if (ProgressText.Text == "100")
                {
                    ProgressText.Visible = false;
                    label4.Visible = false;
                } else
                {
                    ProgressText.Visible = true;
                    label4.Visible = true;
                }
            }
        }
    }

    public static class TaskbarProgress
    {
        public enum TaskbarStates
        {
            NoProgress = 0,
            Indeterminate = 0x1,
            Normal = 0x2,
            Error = 0x4,
            Paused = 0x8
        }

        [ComImport()]
        [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ITaskbarList3
        {
            // ITaskbarList
            [PreserveSig]
            void HrInit();
            [PreserveSig]
            void AddTab(IntPtr hwnd);
            [PreserveSig]
            void DeleteTab(IntPtr hwnd);
            [PreserveSig]
            void ActivateTab(IntPtr hwnd);
            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            [PreserveSig]
            void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
            [PreserveSig]
            void SetProgressState(IntPtr hwnd, TaskbarStates state);
        }

        [ComImport()]
        [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class TaskbarInstance
        {
        }

        private static ITaskbarList3 taskbarInstance = (ITaskbarList3)new TaskbarInstance();
        private static bool taskbarSupported = Environment.OSVersion.Version >= new Version(6, 1);

        public static void SetState(IntPtr windowHandle, TaskbarStates taskbarState)
        {
            if (taskbarSupported) taskbarInstance.SetProgressState(windowHandle, taskbarState);
        }

        public static void SetValue(IntPtr windowHandle, double progressValue, double progressMax)
        {
            if (taskbarSupported) taskbarInstance.SetProgressValue(windowHandle, (ulong)progressValue, (ulong)progressMax);
        }
    }
}
