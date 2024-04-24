using System.Windows.Forms;
using System.Xml;

namespace ScaleSmooth
{
    public partial class Form1 : Form
    {


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

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            if (radioButton1.Checked)
            {
                pictureBox1.Image = ScaleSmoothGray(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }
            else
            {
                pictureBox1.Image = ScaleSmoothColor(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }

            progressBar1.Value = 100;

            pictureBox1.Refresh();
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
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
                progressBar1.Value = c * 100 / ac;

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
                progressBar1.Value = c * 100 / ac;

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
            if (radioButton1.Checked)
            {
                pictureBox1.Image = ScaleRoughGray(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }
            else
            {
                pictureBox1.Image = ScaleRoughColor(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }

            progressBar1.Value = 100;

            pictureBox1.Refresh();
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
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
                                            if (rnd.Next(0, ac2) * rnd.Next(0, 2) < c)
                                            {
                                                d[i, s] = (byte)(s255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            else
                                            {
                                                d[i, s] = (byte)(rnd.Next(0, 2) * 255);
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
                                        d[i, s] = (byte)(s255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2));
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

                                            d[i, s] = (byte)(s255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s] + d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 4));


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
                                        d[i, s] = (byte)(s255((d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 2));
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
                progressBar1.Value = c * 100 / ac;

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
                                            if (rnd.Next(0, ac2) * rnd.Next(0, 2) < c)
                                            {
                                                d[i, s] = (byte)(s255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            else
                                            {
                                                d[i, s] = (byte)(rnd.Next(0, 2) * 255);
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
                                        d[i, s] = (byte)(s255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2));
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

                                            d[i, s] = (byte)(s255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s] + d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 4));


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
                                        d[i, s] = (byte)(s255((d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 2));
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
                progressBar1.Value = c * 100 / ac;
                if (c >= ac2)
                {
                    for (int i = 0; i < ni; i++)
                    {
                        for (int s = 0; s < ns; s++)
                        {
                            ds[i, s] = (ds[i, s] * (c - ac2) + d[i, s]) / (c - ac2 + 1);
                        }
                    }
                }
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
                                            if (rnd.Next(0, ac2 * rnd.Next(0, 2)) < c)
                                            {
                                                dr[i, s] = (byte)(s255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            else
                                            {
                                                dr[i, s] = (byte)(rnd.Next(0, 2) * 255);
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
                                        dr[i, s] = (byte)(s255((dr[i + rnd.Next(-1, 2), s] + dr[i + rnd.Next(-1, 2), s]) / 2));
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
                                            if (rnd.Next(0, ac2 * rnd.Next(0, 2)) < c)
                                            {
                                                dg[i, s] = (byte)(s255((dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            else
                                            {
                                                dg[i, s] = (byte)(rnd.Next(0, 2) * 255);
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
                                        dg[i, s] = (byte)(s255((dg[i + rnd.Next(-1, 2), s] + dg[i + rnd.Next(-1, 2), s]) / 2));
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
                                            if (rnd.Next(0, ac2 * rnd.Next(0, 2)) < c)
                                            {
                                                db[i, s] = (byte)(s255((db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            else
                                            {
                                                db[i, s] = (byte)(rnd.Next(0, 2) * 255);
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
                                        db[i, s] = (byte)(s255((db[i + rnd.Next(-1, 2), s] + db[i + rnd.Next(-1, 2), s]) / 2));
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

                                            dr[i, s] = (byte)(s255((dr[i + rnd.Next(-1, 2), s] + dr[i + rnd.Next(-1, 2), s] + dr[i, s + rnd.Next(-1, 2)] + dr[i, s + rnd.Next(-1, 2)]) / 4));


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
                                        dr[i, s] = (byte)(s255((dr[i, s + rnd.Next(-1, 2)] + dr[i, s + rnd.Next(-1, 2)]) / 2));
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

                                            dg[i, s] = (byte)(s255((dg[i + rnd.Next(-1, 2), s] + dg[i + rnd.Next(-1, 2), s] + dg[i, s + rnd.Next(-1, 2)] + dg[i, s + rnd.Next(-1, 2)]) / 4));


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
                                        dg[i, s] = (byte)(s255((dg[i, s + rnd.Next(-1, 2)] + dg[i, s + rnd.Next(-1, 2)]) / 2));
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

                                            db[i, s] = (byte)(s255((db[i + rnd.Next(-1, 2), s] + db[i + rnd.Next(-1, 2), s] + db[i, s + rnd.Next(-1, 2)] + db[i, s + rnd.Next(-1, 2)]) / 4));


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
                                        db[i, s] = (byte)(s255((db[i, s + rnd.Next(-1, 2)] + db[i, s + rnd.Next(-1, 2)]) / 2));
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
                progressBar1.Value = c * 100 / ac;
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
                                            if (rnd.Next(0, ac2 * rnd.Next(0, 2)) < c)
                                            {
                                                dr[i, s] = (byte)(s255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            else
                                            {
                                                dr[i, s] = (byte)(rnd.Next(0, 2) * 255);
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
                                        dr[i, s] = (byte)(s255((dr[i + rnd.Next(-1, 2), s] + dr[i + rnd.Next(-1, 2), s]) / 2));
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
                                            if (rnd.Next(0, ac2 * rnd.Next(0, 2)) < c)
                                            {
                                                dg[i, s] = (byte)(s255((dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + dg[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            else
                                            {
                                                dg[i, s] = (byte)(rnd.Next(0, 2) * 255);
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
                                        dg[i, s] = (byte)(s255((dg[i + rnd.Next(-1, 2), s] + dg[i + rnd.Next(-1, 2), s]) / 2));
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
                                            if (rnd.Next(0, ac2 * rnd.Next(0, 2)) < c)
                                            {
                                                db[i, s] = (byte)(s255((db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + db[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }
                                            else
                                            {
                                                db[i, s] = (byte)(rnd.Next(0, 2) * 255);
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
                                        db[i, s] = (byte)(s255((db[i + rnd.Next(-1, 2), s] + db[i + rnd.Next(-1, 2), s]) / 2));
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

                                            dr[i, s] = (byte)(s255((dr[i + rnd.Next(-1, 2), s] + dr[i + rnd.Next(-1, 2), s] + dr[i, s + rnd.Next(-1, 2)] + dr[i, s + rnd.Next(-1, 2)]) / 4));


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
                                        dr[i, s] = (byte)(s255((dr[i, s + rnd.Next(-1, 2)] + dr[i, s + rnd.Next(-1, 2)]) / 2));
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

                                            dg[i, s] = (byte)(s255((dg[i + rnd.Next(-1, 2), s] + dg[i + rnd.Next(-1, 2), s] + dg[i, s + rnd.Next(-1, 2)] + dg[i, s + rnd.Next(-1, 2)]) / 4));


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
                                        dg[i, s] = (byte)(s255((dg[i, s + rnd.Next(-1, 2)] + dg[i, s + rnd.Next(-1, 2)]) / 2));
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

                                            db[i, s] = (byte)(s255((db[i + rnd.Next(-1, 2), s] + db[i + rnd.Next(-1, 2), s] + db[i, s + rnd.Next(-1, 2)] + db[i, s + rnd.Next(-1, 2)]) / 4));


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
                                        db[i, s] = (byte)(s255((db[i, s + rnd.Next(-1, 2)] + db[i, s + rnd.Next(-1, 2)]) / 2));
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
                progressBar1.Value = c * 100 / ac;
                if (c >= ac2)
                {
                    for (int i = 0; i < ni; i++)
                    {
                        for (int s = 0; s < ns; s++)
                        {
                            fr[i, s] = (fr[i, s] * (c - ac2) + dr[i, s]) / (c - ac2 + 1);
                            fg[i, s] = (fg[i, s] * (c - ac2) + dg[i, s]) / (c - ac2 + 1);
                            fb[i, s] = (fb[i, s] * (c - ac2) + db[i, s]) / (c - ac2 + 1);
                        }
                    }
                }
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

        private byte s255(double v)
        {
            return (byte)(0.000000002833333 * Math.Pow(v, 5) - 0.00000181137 * Math.Pow(v, 4) + 0.0003605953 * Math.Pow(v, 3) - 0.01970911609 * Math.Pow(v, 2) + 0.63373610992 * v + 0.17238095178);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            if (radioButton1.Checked)
            {
                pictureBox1.Image = ScaleFurryGray(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }
            else
            {
                pictureBox1.Image = ScaleFurryColor(pictureBox1.Image, (int)(numericUpDown1.Value), (int)(trackBar1.Value));
            }

            progressBar1.Value = 100;

            pictureBox1.Refresh();
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }
    }
}
