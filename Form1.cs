using System.Runtime.InteropServices;

namespace ScaleSmooth
{
    public partial class Form1 : Form
    {

        string pp = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Raster image|*.png;*.jpg;*.bmp;*.gif";
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
            button1.Enabled = false;
            if (radioButton1.Checked)
            {
                switch (comboBox1.SelectedItem)
                {
                    case "scaleSmooth":
                        pictureBox1.Image = ScaleSmoothGray(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "scaleSmoothContinuous":
                        pictureBox1.Image = ScaleSmoothContinuousGray(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "scaleFurry":
                        pictureBox1.Image = ScaleFurryGray(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "scaleRough":
                        pictureBox1.Image = ScaleRoughGray(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "contrastBoldScale":
                        pictureBox1.Image = ContrastBoldScaleGray(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "boldScale":
                        pictureBox1.Image = BoldScaleGray(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "scaleSeparate":
                        pictureBox1.Image = ScaleSeparateGray(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                        break;
                    case "scaleBilinearApproximation":
                        pictureBox1.Image = ScaleBilinearApproximationGray(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                        break;
                    case "neighborSubpixel":
                        pictureBox1.Image = neighborSubpixelGray(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                        break;
                }
            }
            else
            {
                switch (comboBox1.SelectedItem)
                {
                    case "scaleSmooth":
                        pictureBox1.Image = ScaleSmoothColor(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "scaleSmoothContinuous":
                        pictureBox1.Image = ScaleSmoothContinuousColor(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "scaleFurry":
                        pictureBox1.Image = ScaleFurryColor(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "scaleRough":
                        pictureBox1.Image = ScaleRoughColor(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "contrastBoldScale":
                        pictureBox1.Image = ContrastBoldScaleColor(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "boldScale":
                        pictureBox1.Image = BoldScaleColor(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
                        break;
                    case "scaleSeparate":
                        pictureBox1.Image = ScaleSeparateColor(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                        break;
                    case "scaleBilinearApproximation":
                        pictureBox1.Image = ScaleBilinearApproximationColor(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                        break;
                }
            }

            ProgressText.Text = "100";
            pictureBox1.Refresh();
            button3.Enabled = true;
            button2.Enabled = true;
            button1.Enabled = true;
        }

        private Bitmap neighborSubpixelGray(Image img, int x, int ac) //color
        {
            int ni, ns, oi, os;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            byte[,] r = new byte[ni, ns];

            double[,] xr = new double[oi + 2, os + 2];
            double[,] yr = new double[oi + 2, os + 2];
            byte[,] sr = GrayFromBMP(img, 1, 1, 1, 1, oi, os);

            NaiveExtrapolation(sr, oi, os);

            return BMPfromGray(sr, oi + 2, os + 2);
        }

        static byte[,] GrayFromBMP(Image img, int marginLeft, int marginRight, int marginTop, int marginBottom, int w, int h)
        {
            Bitmap bmp = (Bitmap)img;
            byte[,] sr = new byte[w + marginLeft + marginRight, h + marginTop + marginBottom];
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int stride = Math.Abs(bmpData.Stride);
            int bytes = stride * bmp.Height;
            IntPtr ptr = bmpData.Scan0;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                int w3 = w * 3;
                int stridew3 = stride - w3;
                Parallel.For(0, rgbValues.Length, c =>
                {
                    if (c % stride < w3)
                    {
                        int realc = c - stridew3 * (c / stride);
                        int realc3 = realc / 3;
                        int i = realc3 % w + marginLeft;
                        int s = realc3 / w + marginTop;
                        if (realc % 3 == 0) sr[i, s] = (byte)((rgbValues[c] + rgbValues[c + 1] + rgbValues[c + 2]) / 3.0);
                    }
                });
            }
            else
            {
                int w4 = w * 4;
                int stridew4 = stride - w4;
                Parallel.For(0, rgbValues.Length, c =>
                {
                    if (c % stride < w4)
                    {
                        int realc = c - stridew4 * (c / stride);
                        int realc4 = realc / 4;
                        int i = realc4 % w + marginLeft;
                        int s = realc4 / w + marginTop;
                        if (realc % 4 == 0) sr[i, s] = (byte)((rgbValues[c] + rgbValues[c + 1] + rgbValues[c + 2]) / 3.0);
                    }
                });
            }
            bmp.UnlockBits(bmpData);
            return sr;
        }

        static byte[,,] RGBfromBMP(Image img, int marginLeft, int marginRight, int marginTop, int marginBottom, int w, int h)
        {
            Bitmap bmp = (Bitmap)img;
            byte[,,] sr = new byte[w + marginLeft + marginRight, h + marginTop + marginBottom, 3];
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int stride = Math.Abs(bmpData.Stride);
            int bytes = stride * bmp.Height;
            IntPtr ptr = bmpData.Scan0;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                int w3 = w * 3;
                int stridew3 = stride - w3;
                Parallel.For(0, rgbValues.Length, c =>
                {
                    if (c % stride < w3)
                    {
                        int realc = c - stridew3 * (c / stride);
                        int realc3 = realc / 3;
                        int i = realc3 % w + marginLeft;
                        int s = realc3 / w + marginTop;
                        sr[i, s, 2 - realc % 3] = rgbValues[c];
                    }

                });
            }
            else
            {
                int w4 = w * 4;
                int stridew4 = stride - w4;
                Parallel.For(0, rgbValues.Length, c =>
                {
                    if (c % stride < w4)
                    {
                        int realc = c - stridew4 * (c / stride);
                        int realc4 = realc / 4;
                        int i = realc4 % w + marginLeft;
                        int s = realc4 / w + marginTop;
                        int realcm4 = realc % 4;
                        if (realcm4 < 3) sr[i, s, 2 - realcm4] = rgbValues[c];
                    }

                });
            }
            bmp.UnlockBits(bmpData);
            return sr;
        }

        static Bitmap BMPfromGray(byte[,] sr, int w, int h)
        {
            Bitmap bmp = new(w, h);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int bytes = w * h * 4;
            IntPtr ptr = bmpData.Scan0;
            byte[] rgbValues = new byte[bytes];

            Parallel.For(0, rgbValues.Length / 4, c =>
            {
                int c4 = c * 4;
                int i = c % w;
                int s = c / w;
                rgbValues[c4] = sr[i, s];
                rgbValues[c4 + 1] = sr[i, s];
                rgbValues[c4 + 2] = sr[i, s];
                rgbValues[c4 + 3] = 255;
            });
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        static Bitmap BMPfromRGB(byte[,,] sr, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            int bytes = w * h * 4;
            IntPtr ptr = bmpData.Scan0;
            byte[] rgbValues = new byte[bytes];

            Parallel.For(0, rgbValues.Length / 4, c =>
            {
                int c4 = c * 4;
                int i = c % w;
                int s = c / w;
                rgbValues[c4] = sr[i, s, 2];
                rgbValues[c4 + 1] = sr[i, s, 1];
                rgbValues[c4 + 2] = sr[i, s, 0];
                rgbValues[c4 + 3] = 255;
            });
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private static void NaiveExtrapolation(byte[,] sr, int oi, int os)
        {
            int oip, oim, osp, osm;
            double oi2, os2, oi2p, os2p;
            oip = oi + 1;
            oim = oi - 1;
            osp = os + 1;
            osm = os - 1;
            oi2 = (oi - 1) / 2.0 + 1;
            os2 = (os - 1) / 2.0 + 1;
            oi2p = oi2 + 1;
            os2p = os2 + 1;

            Parallel.Invoke(
                () =>
                {
                    int r = (int)(oi2p / os2p + 0.5);
                    int r2 = r * 2;
                    int rp = r + 1;
                    sr[oip, 0] = (byte)Math.Clamp((sr[oip - r, 1] * 2 - sr[oip - r2, 2]) / 2.0 + (sr[oip - r, 1] * 2 + sr[oip - rp, 1] / 2.0 + sr[oip - r, 2] / 2.0) / 6.0 + 0.5, 0, 255);
                    sr[oip, osp] = (byte)Math.Clamp((sr[oip - r, os] * 2 - sr[oip - r2, osm]) / 2.0 + (sr[oip - r, os] * 2 + sr[oip - r, osm] / 2.0 + sr[oip - rp, os] / 2.0) / 6.0 + 0.5, 0, 255);
                    sr[0, osp] = (byte)Math.Clamp((sr[r, os] * 2 - sr[r2, osm]) / 2.0 + (sr[r, osm] / 2.0 + sr[rp, os] / 2.0 + sr[r, os] * 2) / 6.0 + 0.5, 0, 255);
                    sr[0, 0] = (byte)Math.Clamp((sr[r, 1] * 2 - sr[r2, 2]) / 2.0 + (sr[r, 1] * 2 + sr[rp, 1] / 2.0 + sr[r, 2] / 2.0) / 6.0 + 0.5, 0, 255);

                    r = (int)(oi2 / os2p + 0.5);
                    r2 = r * 2;
                    rp = r + 1;
                    int r2p = r2 + 1;
                    sr[oi, 0] = (byte)Math.Clamp((sr[oi - r, 1] * 2 - sr[oi - r2, 2]) / 2.0 + (sr[oi - r, 1] * 2 + sr[oi - r, 2] / 2.0 + sr[oi - rp, 1] / 2.0) / 6.0 + 0.5, 0, 255);
                    sr[oi, osp] = (byte)Math.Clamp((sr[oi - r, os] * 2 - sr[oi - r2, osm]) / 2.0 + (sr[oi - r, os] * 2 + sr[oi - rp, os] / 2.0 + sr[oi - r, osm] / 2.0) / 6.0 + 0.5, 0, 255);
                    sr[1, 0] = (byte)Math.Clamp((sr[rp, 1] * 2 - sr[r2p, 2]) / 2.0 + (sr[rp, 1] * 2 + sr[rp + 1, 1] / 2.0 + sr[rp, 2] / 2.0) / 6.0 + 0.5, 0, 255);
                    sr[1, osp] = (byte)Math.Clamp((sr[rp, os] * 2 - sr[r2p, osm]) / 2.0 + (sr[rp, os] * 2 + sr[rp + 1, os] / 2.0 + sr[rp, osm] / 2.0) / 6.0 + 0.5, 0, 255);

                    r = (int)(os2 / oi2p + 0.5);
                    r2 = r * 2;
                    rp = r + 1;
                    r2p = r2 + 1;
                    sr[oip, 1] = (byte)Math.Clamp((sr[oi, rp] * 2 - sr[oim, r2p]) / 2.0 + (sr[oi, rp] * 2 + sr[oi, rp + 1] / 2.0 + sr[oim, rp] / 2.0) / 6.0 + 0.5, 0, 255);
                    sr[oip, os] = (byte)Math.Clamp((sr[oi, os - r] * 2 - sr[oim, os - r2]) / 2.0 + (sr[oi, os - r] * 2 + sr[oi, osm - r] / 2.0 + sr[oim, os - r] / 2.0) / 6.0 + 0.5, 0, 255);
                    sr[0, 1] = (byte)Math.Clamp((sr[1, rp] * 2 - sr[2, r2p]) / 2.0 + (sr[1, rp] * 2 + sr[2, rp] / 2.0 + sr[1, rp + 1] / 2.0) / 6.0 + 0.5, 0, 255);
                    sr[0, os] = (byte)Math.Clamp((sr[1, os - r] * 2 - sr[2, os - r2]) / 2.0 + (sr[2, os - r] / 2.0 + sr[1, osm - r] / 2.0 + sr[1, os - r] * 2) / 6.0 + 0.5, 0, 255);
                },
                () =>
                {
                    Parallel.For(2, (int)os2p, s =>
                    {
                        int r = (int)((os2p - s) / oi2p + 0.5);
                        int r2 = r * 2;
                        int rp = r + 1;
                        int rm = r - 1;
                        sr[0, s] = (byte)Math.Clamp((sr[1, s + r] * 2 - sr[2, s + r2]) / 2.0 + (sr[1, s + rp] / 2.0 + sr[1, s + rm] / 2.0 + sr[1, s + r] * 2) / 6.0 + 0.5, 0, 255);
                        sr[oip, s] = (byte)Math.Clamp((sr[oi, s + r] * 2 - sr[oim, s + r2]) / 2.0 + (sr[oi, s + rm] / 2.0 + sr[oi, s + rp] / 2.0 + sr[oi, s + r] * 2) / 6.0 + 0.5, 0, 255);
                        int osps = osp - s;
                        sr[0, osps] = (byte)Math.Clamp((sr[1, osps - r] * 2 - sr[2, osps - r2]) / 2.0 + (sr[1, osps - rp] / 2.0 + sr[1, osps - rm] / 2.0 + sr[1, osps - r] * 2) / 6.0 + 0.5, 0, 255);
                        sr[oip, osps] = (byte)Math.Clamp((sr[oi, osps - r] * 2 - sr[oim, osps - r2]) / 2.0 + (sr[oi, osps - rm] / 2.0 + sr[oi, osps - rp] / 2.0 + sr[oi, osps - r] * 2) / 6.0 + 0.5, 0, 255);
                    });
                },
                () =>
                {
                    Parallel.For(2, (int)oi2p, i =>
                    {
                        int r = (int)((oi2p - i) / os2p + 0.5);
                        int r2 = r * 2;
                        int rp = r + 1;
                        int rm = r - 1;
                        int oipi = oip - i;
                        sr[i, 0] = (byte)Math.Clamp((sr[i + r, 1] * 2 - sr[i + r2, 2]) / 2.0 + (sr[i + r, 1] * 2 + sr[i + rm, 1] / 2.0 + sr[i + rp, 1] / 2.0) / 6.0 + 0.5, 0, 255);
                        sr[i, osp] = (byte)Math.Clamp((sr[i + r, os] * 2 - sr[i + r2, osm]) / 2.0 + (sr[i + r, os] * 2 + sr[i + rp, os] / 2.0 + sr[i + rm, os] / 2.0) / 6.0 + 0.5, 0, 255);
                        sr[oip - i, 0] = (byte)Math.Clamp((sr[oip - i - r, 1] * 2 - sr[oip - i - r2, 2]) / 2.0 + (sr[oipi - r, 1] * 2 + sr[oipi - rm, 1] / 2.0 + sr[oipi - rp, 1] / 2.0) / 6.0 + 0.5, 0, 255);
                        sr[oipi, osp] = (byte)Math.Clamp((sr[oipi - r, os] * 2 - sr[oipi - r2, osm]) / 2.0 + (sr[oipi - r, os] * 2 + sr[oipi - rp, os] / 2.0 + sr[oipi - rm, os] / 2.0) / 6.0 + 0.5, 0, 255);
                    });
                }
            );
        }

        private Bitmap ScaleSmoothContinuousGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm;
            xm = x - 1;
            ni = img.Width * x;
            ns = img.Height * x;
            oi = img.Width;
            os = img.Height;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = x * (ac * (oim - 1) / 100 + 1);
            osm = os - 1;

            byte[,] d = new byte[ni, ns];
            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);
            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[ix, s / x];
                }
            });

            int r, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx + xm; i > ixx - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1] + d[i - 1, s - 1] + d[i + 1, s + 1] + d[i + 1, s - 1] + d[i - 1, s + 1]) / 8d + 0.5);
                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                            for (int i = ixx + xm; i > ixx - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s]) / 2d + 0.5);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1]) / 4d + 0.5);
                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                for (int s = sxx; s < sxx + x; s++)
                                {
                                    if (s != 0 && s != nsm)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i, s - 1] + d[i, s + 1]) / 2d + 0.5);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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

            return BMPfromGray(d, ni, ns);
        }

        private Bitmap ScaleSmoothGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = x * (ac * (oim - 1) / 100 + 1);
            osm = os - 1;

            byte[,] d = new byte[ni, ns];
            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);
            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[ix, s / x];
                }
            });

            long sumr = 0;
            for (int i = 0; i < oi; i++)
            {
                sumr += sr[i, 0];
                sumr += sr[i, osm];
            }

            for (int s = 0; s < os; s++)
            {
                sumr += sr[0, s];
                sumr += sr[oim, s];
            }

            double dp = 0.99;
            for (byte t = 0; t < 3; t++)
            {
                if (sumr > 127.5 * (oi + os)) dp = 0.01;
            }

            int r, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx + xm; i > ixx - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1] + d[i - 1, s - 1] + d[i + 1, s + 1] + d[i + 1, s - 1] + d[i - 1, s + 1]) / 8d + dp);
                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                            for (int i = ixx + xm; i > ixx - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s]) / 2d + dp);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1]) / 4d + dp);
                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                for (int s = sxx; s < sxx + x; s++)
                                {
                                    if (s != 0 && s != nsm)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i, s - 1] + d[i, s + 1]) / 2d + dp);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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

            return BMPfromGray(d, ni, ns);
        }

        private Bitmap ScaleSmoothContinuousColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;

            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = x * (ac * (oim - 1) / 100 + 1);
            osm = os - 1;

            byte[,,] r = new byte[ni, ns, 3];
            byte[,,] sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);
            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    r[i, s, 0] = sr[ix, s / x, 0];
                    r[i, s, 1] = sr[ix, s / x, 1];
                    r[i, s, 2] = sr[ix, s / x, 2];
                }
            });

            int rn, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx + xm; i > ixx - 1; i--)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx + xm; s > sxx - 1; s--)
                                        {
                                            if (s != 0 && s != nsm)
                                            {
                                                ld = r[i, s, t];
                                                r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t] + r[i - 1, s - 1, t] + r[i + 1, s + 1, t] + r[i + 1, s - 1, t] + r[i - 1, s + 1, t]) / 8d + 0.5);
                                                rn = r[i, s, t] - ld;
                                                if (rn > 0)
                                                {
                                                    while (rn != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] > 0)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                            rn--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (rn != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] < 255)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
                                                            rn++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                for (int i = ixx + xm; i > ixx - 1; i--)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx + xm; s > sxx - 1; s--)
                                        {
                                            ld = r[i, s, t];
                                            r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t]) / 2d + 0.5);
                                            rn = r[i, s, t] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] > 0)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);

                                                    if (r[ii, ss, t] < 255)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
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
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx; s < sxx + x; s++)
                                        {
                                            if (s != 0 && s != nsm)
                                            {
                                                ld = r[i, s, t];
                                                r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t]) / 4d + 0.5);
                                                rn = r[i, s, t] - ld;
                                                if (rn > 0)
                                                {
                                                    while (rn != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] > 0)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                            rn--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (rn != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);

                                                        if (r[ii, ss, t] < 255)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
                                                            rn++;
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = r[i, s, t];
                                            r[i, s, t] = (byte)((r[i, s - 1, t] + r[i, s + 1, t]) / 2d + 0.5); //2d?
                                            rn = r[i, s, t] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] > 0)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);

                                                    if (r[ii, ss, t] < 255)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
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
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }

            return BMPfromRGB(r, ni, ns);
        }

        private Bitmap ScaleSmoothColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;

            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = x * (ac * (oim - 1) / 100 + 1);
            osm = os - 1;

            byte[,,] r = new byte[ni, ns, 3];
            byte[,,] sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);
            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    r[i, s, 0] = sr[ix, s / x, 0];
                    r[i, s, 1] = sr[ix, s / x, 1];
                    r[i, s, 2] = sr[ix, s / x, 2];
                }
            });

            long[] sumr = [0, 0, 0];
            for (int i = 0; i < oi; i++)
            {
                sumr[0] += sr[i, 0, 0];
                sumr[1] += sr[i, 0, 1];
                sumr[2] += sr[i, 0, 2];
                sumr[0] += sr[i, osm, 0];
                sumr[1] += sr[i, osm, 1];
                sumr[2] += sr[i, osm, 2];
            }

            for (int s= 0; s < os; s++)
            {
                sumr[0] += sr[0, s, 0];
                sumr[1] += sr[0, s, 1];
                sumr[2] += sr[0,s, 2];
                sumr[0] += sr[oim, s, 0];
                sumr[1] += sr[oim, s, 1];
                sumr[2] += sr[oim, s, 2];
            }

            double[] dp = [0.99, 0.99, 0.99];

            for (byte t = 0; t < 3; t++)
            {
                if (sumr[t] > 127.5 * (oi + os)) dp[t] =0.01;
            }

            int rn, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx + xm; i > ixx - 1; i--)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx + xm; s > sxx - 1; s--)
                                        {
                                            if (s != 0 && s != nsm)
                                            {
                                                ld = r[i, s, t];
                                                r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t] + r[i - 1, s - 1, t] + r[i + 1, s + 1, t] + r[i + 1, s - 1, t] + r[i - 1, s + 1, t]) / 8d + dp[t]);
                                                rn = r[i, s, t] - ld;
                                                if (rn > 0)
                                                {
                                                    while (rn != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] > 0)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                            rn--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (rn != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] < 255)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
                                                            rn++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                for (int i = ixx + xm; i > ixx - 1; i--)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx + xm; s > sxx - 1; s--)
                                        {
                                            ld = r[i, s, t];
                                            r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t]) / 2d + dp[t]);
                                            rn = r[i, s, t] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] > 0)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);

                                                    if (r[ii, ss, t] < 255)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
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
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx; s < sxx + x; s++)
                                        {
                                            if (s != 0 && s != nsm)
                                            {
                                                ld = r[i, s, t];
                                                r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t]) / 4d + dp[t]);
                                                rn = r[i, s, t] - ld;
                                                if (rn > 0)
                                                {
                                                    while (rn != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] > 0)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                            rn--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (rn != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);

                                                        if (r[ii, ss, t] < 255)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
                                                            rn++;
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = r[i, s, t];
                                            r[i, s, t] = (byte)((r[i, s - 1, t] + r[i, s + 1, t]) / 2d + dp[t]);
                                            rn = r[i, s, t] - ld;
                                            if (rn > 0)
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] > 0)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                        rn--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rn != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);

                                                    if (r[ii, ss, t] < 255)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
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
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }

            return BMPfromRGB(r, ni, ns);
        }

        private Bitmap ScaleRoughGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, ac2;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;

            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = x * (ac * (oim - 1) / 100 + 1);
            ac2 = ac / 2;
            osm = os - 1;

            byte[,] d = new byte[ni, ns];
            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);
            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[ix, s / x];
                }
            });

            int r, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx + xm; i > ixx - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = d[i, s];
                                            if (ac2 <= c)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    d[i, s] = S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9d);
                                                }
                                                else
                                                {
                                                    d[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                d[i, s] = S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9d);
                                            }
                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                            for (int i = ixx + xm; i > ixx - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2d);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];

                                            d[i, s] = S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s] + d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 4d);

                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                for (int s = sxx; s < sxx + x; s++)
                                {
                                    if (s != 0 && s != nsm)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = S255((d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 2d);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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

            return BMPfromGray(d, ni, ns);
        }

        private Bitmap ScaleFurryGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, ac2;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = x * (ac * (oim - 1) / 100 + 1);
            ac2 = ac / 2;
            osm = os - 1;

            byte[,] d = new byte[ni, ns];
            float[,] ds = new float[ni, ns];
            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);
            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[ix, s / x];
                    ds[i, s] = d[i, s];
                }
            });

            int r, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                int cac2 = c - ac2;
                bool cac2b = cac2 >= 0;
                int cac2p = cac2 + 1;
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx + xm; i > ixx - 1; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            ld = d[i, s];
                                            if (!cac2b)
                                            {
                                                if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                {
                                                    d[i, s] = S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9d);
                                                }
                                                else
                                                {
                                                    d[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                d[i, s] = S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9d);
                                            }

                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (d[ii, ss] < 255)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (cac2b)
                                        {
                                            ds[i, s] = (ds[i, s] * cac2 + d[i, s]) / cac2p;
                                        }
                                    }
                                }
                            }
                            for (int i = ixx + xm; i > ixx - 1; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2d));
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                        if (cac2b)
                                        {
                                            ds[i, s] = (ds[i, s] * cac2 + d[i, s]) / cac2p;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = d[i, s];

                                            d[i, s] = (byte)(S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s] + d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 4d));

                                            r = d[i, s] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x); //ixx+x as other variable
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (d[ii, ss] < 255)
                                                    {
                                                        d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (cac2b)
                                        {
                                            ds[i, s] = (ds[i, s] * cac2 + d[i, s]) / cac2p;
                                        }
                                    }
                                }

                            }
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                for (int s = sxx; s < sxx + x; s++)
                                {
                                    if (s != 0 && s != nsm)
                                    {
                                        ld = d[i, s]; //2d?
                                        d[i, s] = S255((d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 2d);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
                                                if (d[ii, ss] < 255)
                                                {
                                                    d[ii, ss] = (byte)(d[ii, ss] + 1);
                                                    r++;
                                                }
                                            }
                                        }
                                    }
                                    if (cac2b)
                                    {
                                        ds[i, s] = (ds[i, s] * cac2 + d[i, s]) / cac2p;
                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }

            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = (byte)ds[i, s];
                }
            });

            return BMPfromGray(d, ni, ns);
        }


        private Bitmap ScaleRoughColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, ac2;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = x * (ac * (oim - 1) / 100 + 1);
            ac2 = ac / 2;
            osm = os - 1;

            byte[,,] dr = new byte[ni, ns, 3];
            byte[,,] sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);
            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    dr[i, s, 0] = sr[ix, s / x, 0];
                    dr[i, s, 1] = sr[ix, s / x, 1];
                    dr[i, s, 2] = sr[ix, s / x, 2];
                }
            });

            for (int c = 0; c < ac; c++)
            {
                for (int t = 0; t < 3; t++)
                {
                    int r, ld, ii, ss;
                    bool ac2c = ac2 <= c;
                    for (int ix = oim; ix > -1; ix--)
                    {
                        int ixx = ix * x;
                        for (int sx = osm; sx > -1; sx--)
                        {
                            int sxx = sx * x;
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx + xm; i > ixx - 1; i--)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx + xm; s > sxx - 1; s--)
                                        {
                                            if (s > 0 && s < nsm)
                                            {
                                                ld = dr[i, s, t];
                                                if (ac2c)
                                                {
                                                    if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                    {
                                                        dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9d);
                                                    }
                                                    else
                                                    {
                                                        dr[i, s, t] = (byte)(rnd.Next(0, 2) * 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9d);
                                                }
                                                r = dr[i, s, t] - ld;
                                                if (r > 0)
                                                {
                                                    while (r != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (dr[ii, ss, t] > 0)
                                                        {
                                                            dr[ii, ss, t] = (byte)(dr[ii, ss, t] - 1);
                                                            r--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (r != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (dr[ii, ss, t] < 255)
                                                        {
                                                            dr[ii, ss, t] = (byte)(dr[ii, ss, t] + 1);
                                                            r++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                for (int i = ixx + xm; i > ixx - 1; i--)
                                {
                                    if (i > 0 && i < nim)
                                    {
                                        for (int s = sxx + xm; s > sxx - 1; s--)
                                        {
                                            ld = dr[i, s, t];
                                            dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s, t] + dr[i + rnd.Next(-1, 2), s, t]) / 2d);
                                            r = dr[i, s, t] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (dr[ii, ss, t] > 0)
                                                    {
                                                        dr[ii, ss, t] = (byte)(dr[ii, ss, t] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (dr[ii, ss, t] < 255)
                                                    {
                                                        dr[ii, ss, t] = (byte)(dr[ii, ss, t] + 1);
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
                        int ixx = ix * x;
                        for (int sx = 0; sx < os; sx++)
                        {
                            int sxx = sx * x;
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx; s < sxx + x; s++)
                                        {
                                            if (s != 0 && s != nsm)
                                            {
                                                ld = dr[i, s, t];

                                                dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s, t] + dr[i + rnd.Next(-1, 2), s, t] + dr[i, s + rnd.Next(-1, 2), t] + dr[i, s + rnd.Next(-1, 2), t]) / 4d);

                                                r = dr[i, s, t] - ld;
                                                if (r > 0)
                                                {
                                                    while (r != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (dr[ii, ss, t] > 0)
                                                        {
                                                            dr[ii, ss, t] = (byte)(dr[ii, ss, t] - 1);
                                                            r--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (r != 0)
                                                    {
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
                                                        if (dr[ii, ss, t] < 255)
                                                        {
                                                            dr[ii, ss, t] = (byte)(dr[ii, ss, t] + 1);
                                                            r++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            ld = dr[i, s, t];
                                            dr[i, s, t] = S255((dr[i, s + rnd.Next(-1, 2), t] + dr[i, s + rnd.Next(-1, 2), t]) / 2d);
                                            r = dr[i, s, t] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (dr[ii, ss, t] > 0)
                                                    {
                                                        dr[ii, ss, t] = (byte)(dr[ii, ss, t] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
                                                    if (dr[ii, ss, t] < 255)
                                                    {
                                                        dr[ii, ss, t] = (byte)(dr[ii, ss, t] + 1);
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
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }

            return BMPfromRGB(dr, ni, ns);
        }

        private Bitmap ScaleFurryColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, ac2;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            nim = ni - 1;
            nsm = ns - 1;
            ac = x * (ac * (oim - 1) / 100 + 1);
            ac2 = ac / 2;
            osm = os - 1;

            byte[,,] dr = new byte[ni, ns, 3];
            float[,,] fr = new float[ni, ns, 3];
            byte[,,] sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);
            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    for (int t = 0; t < 3; t++)
                    {
                        dr[i, s, t] = sr[ix, s / x, t];
                        fr[i, s, t] = dr[i, s, t];
                    }

                }
            });

            for (int c = 0; c < ac; c++)
            {
                int cac2 = c - ac2;
                bool cac2b = cac2 >= 0;
                int cac2p = cac2 + 1;
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx + xm; i > ixx - 1; i--)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx + xm; s > sxx - 1; s--)
                                        {
                                            if (s > 0 && s < nsm)
                                            {
                                                int ld = dr[i, s, t];
                                                if (!cac2b)
                                                {
                                                    if (rnd.Next(0, 2) * rnd.Next(0, ac2) < c)
                                                    {
                                                        dr[i, s, t] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 0] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 1] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 2] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9));
                                                    }
                                                    else
                                                    {
                                                        dr[i, s, t] = (byte)(rnd.Next(0, 2) * 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dr[i, s, t] = (byte)(S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 0] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 1] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 2] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9));
                                                }

                                                int r = dr[i, s, t] - ld;
                                                if (r > 0)
                                                {
                                                    while (r != 0)
                                                    {
                                                        int ii = rnd.Next(ixx, ixx + x);
                                                        int ss = rnd.Next(sxx, sxx + x);
                                                        if (dr[ii, ss, t] > 0)
                                                        {
                                                            dr[ii, ss, t] = (byte)(dr[ii, ss, t] - 1);
                                                            r--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (r != 0)
                                                    {
                                                        int ii = rnd.Next(ixx, ixx + x);
                                                        int ss = rnd.Next(sxx, sxx + x);
                                                        if (dr[ii, ss, t] < 255)
                                                        {
                                                            dr[ii, ss, t] = (byte)(dr[ii, ss, t] + 1);
                                                            r++;
                                                        }
                                                    }
                                                }
                                            }
                                            if (cac2b)
                                            {
                                                fr[i, s, t] = (fr[i, s, t] * cac2 + dr[i, s, t]) / cac2p;
                                            }
                                        }
                                    }
                                }
                            }
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx + xm; i > ixx - 1; i--)
                                {
                                    if (i > 0 && i < nim)
                                    {
                                        for (int s = sxx + xm; s > sxx - 1; s--)
                                        {
                                            int ld = dr[i, s, t];
                                            dr[i, s, t] = (byte)(S255((dr[i + rnd.Next(-1, 2), s, t] + dr[i + rnd.Next(-1, 2), s, t]) / 2));
                                            int r = dr[i, s, t] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (dr[ii, ss, t] > 0)
                                                    {
                                                        dr[ii, ss, t] = (byte)(dr[ii, ss, t] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (dr[ii, ss, t] < 255)
                                                    {
                                                        dr[ii, ss, t] = (byte)(dr[ii, ss, t] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                            if (cac2b)
                                            {
                                                fr[i, s, t] = (fr[i, s, t] * cac2 + dr[i, s, t]) / cac2p;
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
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx; s < sxx + x; s++)
                                        {
                                            if (s != 0 && s != nsm)
                                            {
                                                int ld = dr[i, s, t];

                                                dr[i, s, t] = (byte)(S255((dr[i + rnd.Next(-1, 2), s, t] + dr[i + rnd.Next(-1, 2), s, t] + dr[i, s + rnd.Next(-1, 2), t] + dr[i, s + rnd.Next(-1, 2), t]) / 4));

                                                int r = dr[i, s, t] - ld;
                                                if (r > 0)
                                                {
                                                    while (r != 0)
                                                    {
                                                        int ii = rnd.Next(ixx, ixx + x);
                                                        int ss = rnd.Next(sxx, sxx + x);
                                                        if (dr[ii, ss, t] > 0)
                                                        {
                                                            dr[ii, ss, t] = (byte)(dr[ii, ss, t] - 1);
                                                            r--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (r != 0)
                                                    {
                                                        int ii = rnd.Next(ixx, ixx + x);
                                                        int ss = rnd.Next(sxx, sxx + x);
                                                        if (dr[ii, ss, t] < 255)
                                                        {
                                                            dr[ii, ss, t] = (byte)(dr[ii, ss, t] + 1);
                                                            r++;
                                                        }
                                                    }
                                                }
                                            }
                                            if (cac2b)
                                            {
                                                fr[i, s, t] = (fr[i, s, t] * cac2 + dr[i, s, t]) / cac2p;
                                            }
                                        }
                                    }
                                }
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            int ld = dr[i, s, t];
                                            dr[i, s, t] = (byte)(S255((dr[i, s + rnd.Next(-1, 2), t] + dr[i, s + rnd.Next(-1, 2), t]) / 2));
                                            int r = dr[i, s, t] - ld;
                                            if (r > 0)
                                            {
                                                while (r != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (dr[ii, ss, t] > 0)
                                                    {
                                                        dr[ii, ss, t] = (byte)(dr[ii, ss, t] - 1);
                                                        r--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (r != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (dr[ii, ss, t] < 255)
                                                    {
                                                        dr[ii, ss, t] = (byte)(dr[ii, ss, t] + 1);
                                                        r++;
                                                    }
                                                }
                                            }
                                        }
                                        if (cac2b)
                                        {
                                            fr[i, s, t] = (fr[i, s, t] * cac2 + dr[i, s, t]) / cac2p;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }

            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    dr[i, s, 0] = (byte)fr[i, s, 0];
                    dr[i, s, 1] = (byte)fr[i, s, 1];
                    dr[i, s, 2] = (byte)fr[i, s, 2];
                }
            });

            return BMPfromRGB(dr, ni, ns);
        }

        static byte S255(double v)
        {
            return (byte)(0.000000002833333 * Math.Pow(v, 5) - 0.00000181137 * Math.Pow(v, 4) + 0.0003605953 * Math.Pow(v, 3) - 0.01970911609 * Math.Pow(v, 2) + 0.63373610992 * v + 0.17238095178);
        }

        static float S255f(float v)
        {
            return (float)(0.000000002833333 * Math.Pow(v, 5) - 0.00000181137 * Math.Pow(v, 4) + 0.0003605953 * Math.Pow(v, 3) - 0.01970911609 * Math.Pow(v, 2) + 0.63316688184 * v);
        }

        private Bitmap ContrastBoldScaleGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, xoim, xoim2, xoimac;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoim = x * oim;
            xoimac = xoim * ac / 200 + 1;
            xoim2 = xoimac * 2;

            byte[,] d = new byte[ni, ns];
            float[,] ds = new float[ni, ns];
            float[,] ds2;
            Random rnd = new();

            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[ix, s / x];
                }
            });

            Parallel.For(0, ni - x, i =>
            {
                int ix = i / x;
                int ixx = i / x * x;
                for (int s = 0; s < ns - x; s++)
                {
                    int sx = s / x;
                    int sxx = s / x * x;
                    if (i == ixx && s == sxx)
                    {
                        ds[i, s] = sr[ix, sx];
                    }
                    else
                    {
                        ds[i, s] = (float)((sr[ix, sx] / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx, 2)) + sr[ix + 1, sx] / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx, 2)) + sr[ix, sx + 1] / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx - x, 2)) + sr[ix + 1, sx + 1] / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx - x, 2))) / (1 / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx, 2)) + 1 / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx, 2)) + 1 / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx - x, 2)) + 1 / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx - x, 2))));
                    }
                }
            });

            for (int i = nim; i > -1; i--)
            {
                int ix = i / x;
                for (int s = nsm; s > -1; s--)
                {
                    int sx = s / x;
                    if (sr[ix, sx] == 0 || sr[ix, sx] == 255)
                    {
                        ds[i, s] = sr[ix, sx];
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
                bool cxoimac = c <= xoimac;
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx + xm; i >= ixx; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            if (cxoimac)
                                            {
                                                ld = d[i, s];
                                                if (c < xoimac)
                                                {
                                                    if (rnd.Next(0, xoim) > c)   //you can remove this
                                                    {                            //to reduce grid structure
                                                        d[i, s] = (byte)(S255f((ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                    }                                         //you can
                                                    else                                      //remove this
                                                    {                                         //to reduce
                                                        d[i, s] = (byte)rnd.Next(0, 256);     //grid
                                                    }                                         //structure
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
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
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
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
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

                            for (int i = ixx + xm; i >= ixx; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sxx + xm; s >= sxx; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ixx; i < ixx + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                for (int s = sxx; s < sxx + x; s++)
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                if (!cxoimac)
                {
                    for (int i = 0; i < ni; i++)
                    {
                        for (int s = 0; s < ns; s++)
                        {

                            ds[i, s] = (ds[i, s] * c + d[i, s] + S255f(d[i, s])) / (c + 2);
                        }
                    }
                }
            }


            ds2 = ds;

            Parallel.For(0, ni, i =>
            {
                if (i != 0 && i != nim)
                {

                    for (int s = 0; s < ns; s++)
                    {
                        if (s != 0 && s != nsm)
                        {
                            d[i, s] = (byte)((S255f((ds2[i, s] * 2 + ds2[i + 1, s] + ds2[i, s + 1] + ds2[i - 1, s] + ds2[i, s - 1]) / 6) * 2 + ds2[i, s]) / 3);
                        }
                        else
                        {
                            d[i, s] = (byte)((S255f(ds2[i, s]) * 2 + ds2[i, s]) / 3);
                        }
                    }
                }
                else
                {
                    for (int s = 0; s < ns; s++)
                    {
                        d[i, s] = (byte)((S255f(ds2[i, s]) * 2 + ds2[i, s]) / 3);
                    }
                }

            });

            return BMPfromGray(d, ni, ns);
        }

        private Bitmap ContrastBoldScaleColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, xoim, xoim2, xoimac;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoim = x * oim;
            xoimac = xoim * ac / 200 + 1;
            xoim2 = xoimac * 2;

            byte[,,] r = new byte[ni, ns, 3];
            float[,,] rs = new float[ni, ns, 3];
            float[,,] rs2;

            byte[,,] sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);

            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    int sx = s / x;
                    r[i, s, 0] = sr[ix, sx, 0];
                    r[i, s, 1] = sr[ix, sx, 1];
                    r[i, s, 2] = sr[ix, sx, 2];
                }
            });

            Parallel.For(0, ni - x, i =>
            {
                int ix = i / x;
                int ixx = i / x * x;
                for (int s = 0; s < ns - x; s++)
                {
                    int sx = s / x;
                    int sxx = s / x * x;

                    if (i == ixx && s == sxx)
                    {
                        rs[i, s, 0] = sr[ix, sx, 0];
                        rs[i, s, 1] = sr[ix, sx, 1];
                        rs[i, s, 2] = sr[ix, sx, 2];
                    }
                    else
                    {
                        for (int t = 0; t < 3; t++)
                        {
                            rs[i, s, t] = (float)((sr[ix, sx, t] / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx, 2)) + sr[ix + 1, sx, t] / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx, 2)) + sr[ix, sx + 1, t] / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx - x, 2)) + sr[ix + 1, sx + 1, t] / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx - x, 2))) / (1 / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx, 2)) + 1 / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx, 2)) + 1 / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx - x, 2)) + 1 / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx - x, 2))));
                        }
                    }
                }
            });

            for (int i = nim; i > -1; i--)
            {
                int ix = i / x;
                for (int s = nsm; s > -1; s--)
                {
                    int sx = s / x;
                    for (int t = 0; t < 3; t++)
                    {
                        if (sr[ix, sx, t] == 0 || sr[ix, sx, t] == 255)
                        {
                            rs[i, s, t] = sr[ix, sx, t];
                        }
                        else
                        {
                            rs[i, s, t] = rs[i * (ni - x) / ni, s * (ns - x) / ns, t];
                        }
                    }
                }
            }

            for (int c = 0; c < xoim2; c++)
            {
                bool cxoimac = c <= xoimac;
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx + xm; i >= ixx; i--)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx + xm; s >= sxx; s--)
                                        {
                                            if (s > 0 && s < nsm)
                                            {
                                                if (cxoimac)
                                                {
                                                    int ld = r[i, s, t];
                                                    if (c < xoimac) //two for c???
                                                    {
                                                        if (rnd.Next(0, xoim) > c)   //you can remove this
                                                        {                            //to reduce grid structure
                                                            r[i, s, t] = (byte)(S255f((rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 0] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 1] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 2] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9));
                                                        }                                          //you can
                                                        else                                       //remove this
                                                        {                                          //to reduce
                                                            r[i, s, t] = (byte)rnd.Next(0, 256);      //grid
                                                        }                                          //structure
                                                    }
                                                    else if (c == xoimac) //three for c???
                                                    {
                                                        r[i, s, t] = (byte)rs[i, s, t];
                                                    }

                                                    int rz = r[i, s, t] - ld;
                                                    if (rz > 0)
                                                    {
                                                        while (rz != 0)
                                                        {
                                                            int ii = rnd.Next(ixx, ixx + x);
                                                            int ss = rnd.Next(sxx, sxx + x);
                                                            if (r[ii, ss, t] > 0)
                                                            {
                                                                r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                                rz--;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        while (rz != 0)
                                                        {
                                                            int ii = rnd.Next(ixx, ixx + x);
                                                            int ss = rnd.Next(sxx, sxx + x);
                                                            if (r[ii, ss, t] < 255)
                                                            {
                                                                r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
                                                                rz++;
                                                            }
                                                        }
                                                    }
                                                    rs[i, s, t] = (rs[i, s, t] * c + r[i, s, t]) / (c + 1);
                                                }
                                            }
                                        }
                                    }
                                }

                                for (int i = ixx + xm; i >= ixx; i--)
                                {
                                    if (i > 0 && i < nim)
                                    {
                                        for (int s = sxx + xm; s >= sxx; s--)
                                        {
                                            int ld = r[i, s, t];
                                            r[i, s, t] = (byte)((r[i + rnd.Next(-1, 2), s, t] + r[i + rnd.Next(-1, 2), s, t]) / 2);
                                            int rz = r[i, s, t] - ld;
                                            if (rz > 0)
                                            {
                                                while (rz != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] > 0)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                        rz--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rz != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] < 255)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
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
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx; s < sxx + x; s++)
                                        {
                                            if (s != 0 && s != nsm)
                                            {
                                                int ld = r[i, s, t];
                                                r[i, s, t] = (byte)((r[i + rnd.Next(-1, 2), s, t] + r[i + rnd.Next(-1, 2), s, t] + r[i, s + rnd.Next(-1, 2), t] + r[i, s + rnd.Next(-1, 2), t]) / 4);

                                                int rz = r[i, s, t] - ld;
                                                if (rz > 0)
                                                {
                                                    while (rz != 0)
                                                    {
                                                        int ii = rnd.Next(ixx, ixx + x);
                                                        int ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] > 0)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                            rz--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (rz != 0)
                                                    {
                                                        int ii = rnd.Next(ixx, ixx + x);
                                                        int ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] < 255)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
                                                            rz++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            int ld = r[i, s, t];
                                            r[i, s, t] = (byte)((r[i, s + rnd.Next(-1, 2), t] + r[i, s + rnd.Next(-1, 2), t]) / 2);

                                            int rz = r[i, s, t] - ld;
                                            if (rz > 0)
                                            {
                                                while (rz != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] > 0)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                        rz--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rz != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] < 255)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
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
                }

                ProgressText.Text = (c * 100 / xoim2).ToString();
                if (!cxoimac)
                {
                    Parallel.For(0, ni, i =>
                    {
                        for (int s = 0; s < ns; s++)
                        {
                            rs[i, s, 0] = (rs[i, s, 0] * c + r[i, s, 0] + S255f(r[i, s, 0])) / (c + 2);
                            rs[i, s, 1] = (rs[i, s, 1] * c + r[i, s, 1] + S255f(r[i, s, 1])) / (c + 2);
                            rs[i, s, 2] = (rs[i, s, 2] * c + r[i, s, 2] + S255f(r[i, s, 2])) / (c + 2);
                        }
                    });
                }
            }

            rs2 = rs;

            Parallel.For(0, ni, i =>
            {
                if (i != 0 && i != nim)
                {
                    for (int s = 0; s < ns; s++)
                    {
                        if (s != 0 && s != nsm)
                        {
                            r[i, s, 0] = (byte)((S255f((rs2[i, s, 0] * 2 + rs2[i + 1, s, 0] + rs2[i, s + 1, 0] + rs2[i - 1, s, 0] + rs2[i, s - 1, 0]) / 6) * 2 + rs2[i, s, 0]) / 3 + 0.5);
                            r[i, s, 1] = (byte)((S255f((rs2[i, s, 1] * 2 + rs2[i + 1, s, 1] + rs2[i, s + 1, 1] + rs2[i - 1, s, 1] + rs2[i, s - 1, 1]) / 6) * 2 + rs2[i, s, 1]) / 3 + 0.5);
                            r[i, s, 2] = (byte)((S255f((rs2[i, s, 2] * 2 + rs2[i + 1, s, 2] + rs2[i, s + 1, 2] + rs2[i - 1, s, 2] + rs2[i, s - 1, 2]) / 6) * 2 + rs2[i, s, 2]) / 3 + 0.5);
                        }
                        else
                        {
                            r[i, s, 0] = (byte)((S255f(rs2[i, s, 0]) * 2 + rs2[i, s, 0]) / 3 + 0.5);
                            r[i, s, 1] = (byte)((S255f(rs2[i, s, 1]) * 2 + rs2[i, s, 1]) / 3 + 0.5);
                            r[i, s, 2] = (byte)((S255f(rs2[i, s, 2]) * 2 + rs2[i, s, 2]) / 3 + 0.5);
                        }
                    }
                }
                else
                {
                    for (int s = 0; s < ns; s++)
                    {
                        r[i, s, 0] = (byte)((S255f(rs2[i, s, 0]) * 2 + rs2[i, s, 0]) / 3 + 0.5);
                        r[i, s, 1] = (byte)((S255f(rs2[i, s, 1]) * 2 + rs2[i, s, 1]) / 3 + 0.5);
                        r[i, s, 2] = (byte)((S255f(rs2[i, s, 2]) * 2 + rs2[i, s, 2]) / 3 + 0.5);
                    }
                }
            });

            return BMPfromRGB(r, ni, ns);
        }

        private Bitmap BoldScaleColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, xoim, xoim2, xoimac;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoim = x * oim;
            xoimac = xoim * ac / 200 + 1;
            xoim2 = xoimac * 2;

            byte[,,] r = new byte[ni, ns, 3];
            float[,,] rs = new float[ni, ns, 3];
            byte[,,] sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);

            Random rnd = new();

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    int sx = s / x;
                    r[i, s, 0] = sr[ix, sx, 0];
                    r[i, s, 1] = sr[ix, sx, 1];
                    r[i, s, 2] = sr[ix, sx, 2];
                }
            });

            Parallel.For(0, ni - x, i =>
            {
                int ix = i / x;
                int ixx = i / x * x;
                for (int s = 0; s < ns - x; s++)
                {
                    int sx = s / x;
                    int sxx = s / x * x;

                    if (i == ixx && s == sxx)
                    {
                        rs[i, s, 0] = sr[ix, sx, 0];
                        rs[i, s, 1] = sr[ix, sx, 1];
                        rs[i, s, 2] = sr[ix, sx, 2];
                    }
                    else
                    {
                        for (int t = 0; t < 3; t++)
                        {
                            rs[i, s, t] = (float)((sr[ix, sx, t] / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx, 2)) + sr[ix + 1, sx, t] / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx, 2)) + sr[ix, sx + 1, t] / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx - x, 2)) + sr[ix + 1, sx + 1, t] / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx - x, 2))) / (1 / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx, 2)) + 1 / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx, 2)) + 1 / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx - x, 2)) + 1 / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx - x, 2))));
                        }
                    }
                }
            });

            for (int i = nim; i > -1; i--)
            {
                int ix = i / x;
                for (int s = nsm; s > -1; s--)
                {
                    int sx = s / x;
                    for (int t = 0; t < 3; t++)
                    {
                        if (sr[ix, sx, t] == 0 || sr[ix, sx, t] == 255)
                        {
                            rs[i, s, t] = sr[ix, sx, t];
                        }
                        else
                        {
                            rs[i, s, t] = rs[i * (ni - x) / ni, s * (ns - x) / ns, t];
                        }
                    }
                }
            }

            for (int c = 0; c < xoim2; c++)
            {
                bool cxoimac = c <= xoimac;
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx + xm; i >= ixx; i--)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx + xm; s >= sxx; s--)
                                        {
                                            if (s > 0 && s < nsm)
                                            {
                                                if (cxoimac)
                                                {
                                                    int ld = r[i, s, t];
                                                    if (c < xoimac) //two for c???
                                                    {
                                                        if (rnd.Next(0, xoim) > c)   //you can remove this
                                                        {                            //to reduce grid structure
                                                            r[i, s, t] = (byte)(S255f((rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 0] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 1] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 2] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9));
                                                        }                                          //you can
                                                        else                                       //remove this
                                                        {                                          //to reduce
                                                            r[i, s, t] = (byte)rnd.Next(0, 256);      //grid
                                                        }                                          //structure
                                                    }
                                                    else if (c == xoimac) //three for c???
                                                    {
                                                        r[i, s, t] = (byte)rs[i, s, t];
                                                    }

                                                    int rz = r[i, s, t] - ld;
                                                    if (rz > 0)
                                                    {
                                                        while (rz != 0)
                                                        {
                                                            int ii = rnd.Next(ixx, ixx + x);
                                                            int ss = rnd.Next(sxx, sxx + x);
                                                            if (r[ii, ss, t] > 0)
                                                            {
                                                                r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                                rz--;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        while (rz != 0)
                                                        {
                                                            int ii = rnd.Next(ixx, ixx + x);
                                                            int ss = rnd.Next(sxx, sxx + x);
                                                            if (r[ii, ss, t] < 255)
                                                            {
                                                                r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
                                                                rz++;
                                                            }
                                                        }
                                                    }
                                                    rs[i, s, t] = (rs[i, s, t] * c + r[i, s, t]) / (c + 1);
                                                }
                                            }
                                        }
                                    }
                                }

                                for (int i = ixx + xm; i >= ixx; i--)
                                {
                                    if (i > 0 && i < nim)
                                    {
                                        for (int s = sxx + xm; s >= sxx; s--)
                                        {
                                            int ld = r[i, s, t];
                                            r[i, s, t] = (byte)((r[i + rnd.Next(-1, 2), s, t] + r[i + rnd.Next(-1, 2), s, t]) / 2);
                                            int rz = r[i, s, t] - ld;
                                            if (rz > 0)
                                            {
                                                while (rz != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] > 0)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                        rz--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rz != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] < 255)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
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
                }

                for (int ix = 0; ix < oi; ix++)
                {
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        for (int t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    if (i != 0 && i != nim)
                                    {
                                        for (int s = sxx; s < sxx + x; s++)
                                        {
                                            if (s != 0 && s != nsm)
                                            {
                                                int ld = r[i, s, t];
                                                r[i, s, t] = (byte)((r[i + rnd.Next(-1, 2), s, t] + r[i + rnd.Next(-1, 2), s, t] + r[i, s + rnd.Next(-1, 2), t] + r[i, s + rnd.Next(-1, 2), t]) / 4);

                                                int rz = r[i, s, t] - ld;
                                                if (rz > 0)
                                                {
                                                    while (rz != 0)
                                                    {
                                                        int ii = rnd.Next(ixx, ixx + x);
                                                        int ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] > 0)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                            rz--;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    while (rz != 0)
                                                    {
                                                        int ii = rnd.Next(ixx, ixx + x);
                                                        int ss = rnd.Next(sxx, sxx + x);
                                                        if (r[ii, ss, t] < 255)
                                                        {
                                                            r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
                                                            rz++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                for (int i = ixx; i < ixx + x; i++)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
                                    {
                                        if (s != 0 && s != nsm)
                                        {
                                            int ld = r[i, s, t];
                                            r[i, s, t] = (byte)((r[i, s + rnd.Next(-1, 2), t] + r[i, s + rnd.Next(-1, 2), t]) / 2);

                                            int rz = r[i, s, t] - ld;
                                            if (rz > 0)
                                            {
                                                while (rz != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] > 0)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] - 1);
                                                        rz--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                while (rz != 0)
                                                {
                                                    int ii = rnd.Next(ixx, ixx + x);
                                                    int ss = rnd.Next(sxx, sxx + x);
                                                    if (r[ii, ss, t] < 255)
                                                    {
                                                        r[ii, ss, t] = (byte)(r[ii, ss, t] + 1);
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
                }

                ProgressText.Text = (c * 100 / xoim2).ToString();
                if (!cxoimac)
                {
                    Parallel.For(0, ni, i =>
                    {
                        for (int s = 0; s < ns; s++)
                        {
                            rs[i, s, 0] = (rs[i, s, 0] * c + r[i, s, 0] + S255f(r[i, s, 0])) / (c + 2);
                            rs[i, s, 1] = (rs[i, s, 1] * c + r[i, s, 1] + S255f(r[i, s, 1])) / (c + 2);
                            rs[i, s, 2] = (rs[i, s, 2] * c + r[i, s, 2] + S255f(r[i, s, 2])) / (c + 2);
                        }
                    });
                }
            }

            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    r[i, s, 0] = (byte)(rs[i, s, 0]);
                    r[i, s, 1] = (byte)(rs[i, s, 1]);
                    r[i, s, 2] = (byte)(rs[i, s, 2]);
                }
            });

            return BMPfromRGB(r, ni, ns);
        }


        private Bitmap BoldScaleGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm, xoim, xoim2, xoimac;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            nim = ni - 1;
            nsm = ns - 1;

            xoim = x * oim;
            xoimac = xoim * ac / 200 + 1;
            xoim2 = xoimac * 2;

            byte[,] d = new byte[ni, ns];
            float[,] ds = new float[ni, ns];
            Random rnd = new();
            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);

            Parallel.For(0, ni, i =>
            {
                int ix = i / x;
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = sr[ix, s / x];
                }
            });

            Parallel.For(0, ni - x, i =>
            {
                int ix = i / x;
                int ixx = i / x * x;
                for (int s = 0; s < ns - x; s++)
                {
                    int sx = s / x;
                    int sxx = s / x * x;
                    if (i == ixx && s == sxx)
                    {
                        ds[i, s] = sr[ix, sx];
                    }
                    else
                    {
                        ds[i, s] = (float)((sr[ix, sx] / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx, 2)) + sr[ix + 1, sx] / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx, 2)) + sr[ix, sx + 1] / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx - x, 2)) + sr[ix + 1, sx + 1] / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx - x, 2))) / (1 / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx, 2)) + 1 / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx, 2)) + 1 / (Math.Pow(i - ixx, 2) + Math.Pow(s - sxx - x, 2)) + 1 / (Math.Pow(i - ixx - x, 2) + Math.Pow(s - sxx - x, 2))));
                    }
                }
            });

            for (int i = nim; i > -1; i--)
            {
                int ix = i / x;
                for (int s = nsm; s > -1; s--)
                {
                    int sx = s / x;
                    if (sr[ix, sx] == 0 || sr[ix, sx] == 255)
                    {
                        ds[i, s] = sr[ix, sx];
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
                bool cxoimac = c <= xoimac;
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {
                            for (int i = ixx + xm; i >= ixx; i--)
                            {
                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx + xm; s > sxx - 1; s--)
                                    {
                                        if (s > 0 && s < nsm)
                                        {
                                            if (cxoimac)
                                            {
                                                ld = d[i, s];
                                                if (c < xoimac)
                                                {
                                                    if (rnd.Next(0, xoim) > c)   //you can remove this
                                                    {                            //to reduce grid structure
                                                        d[i, s] = (byte)(S255f((ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                                    }                                         //you can
                                                    else                                      //remove this
                                                    {                                         //to reduce
                                                        d[i, s] = (byte)rnd.Next(0, 256);     //grid
                                                    }                                         //structure
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
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
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
                                                        ii = rnd.Next(ixx, ixx + x);
                                                        ss = rnd.Next(sxx, sxx + x);
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

                            for (int i = ixx + xm; i >= ixx; i--)
                            {
                                if (i > 0 && i < nim)
                                {
                                    for (int s = sxx + xm; s >= sxx; s--)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = (byte)((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2);
                                        r = d[i, s] - ld;
                                        if (r > 0)
                                        {
                                            while (r != 0)
                                            {
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                    int ixx = ix * x;
                    for (int sx = 0; sx < os; sx++)
                    {
                        int sxx = sx * x;
                        if (sr[ix, sx] > 0 && sr[ix, sx] < 255)
                        {

                            for (int i = ixx; i < ixx + x; i++)
                            {

                                if (i != 0 && i != nim)
                                {
                                    for (int s = sxx; s < sxx + x; s++)
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                                                    ii = rnd.Next(ixx, ixx + x);
                                                    ss = rnd.Next(sxx, sxx + x);
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
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                for (int s = sxx; s < sxx + x; s++)
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                                                ii = rnd.Next(ixx, ixx + x);
                                                ss = rnd.Next(sxx, sxx + x);
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
                if (!cxoimac)
                {
                    for (int i = 0; i < ni; i++)
                    {
                        for (int s = 0; s < ns; s++)
                        {

                            ds[i, s] = (ds[i, s] * c + d[i, s] + S255f(d[i, s])) / (c + 2);
                        }
                    }
                }
            }


            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    d[i, s] = (byte)(ds[i, s]);
                }
            });

            return BMPfromGray(d, ni, ns);
        }

        private Bitmap ScaleSeparateGray(Image img, int x, int ac)
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
            byte min, max;
            double[,] r = new double[ni, ns];
            double[,] ri = new double[ni, ns];
            double[,] ris = new double[ni, ns];
            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);

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

            Parallel.For(ixsx, (xs + 2) * x, i =>
            {
                for (int s = iysx; s < ys2x; s++)
                {
                    ri[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sr[xs, ys], sr[xsp, ys], sr[xsp, ysp], sr[xs, ysp]);
                }
            });

            for (int ix = xs + 2; ix < oi; ix++)
            {
                int dixxm = ix * x;
                ixxm = dixxm - 1;
                int dixx = (ix + 1) * x;
                double dixxp = dixxm + x50p;

                Parallel.For(dixxm, dixx, i =>
                {
                    for (int s = iysx; s < ys2x; s++)
                    {
                        ri[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (ri[ixxm, iysx] + ri[ixxm, iysxp]) / 2, sr[ix, ys], sr[ix, ys + 1], (ri[ixxm, iysxpi] + ri[ixxm, iysxpp]) / 2);
                    }
                });
            }

            for (int ix = xs - 1; ix > -1; ix--)
            {
                int dixx = ix * x;
                int dixxp = dixx + x;
                double dixxm = dixx + x50p;

                Parallel.For(dixx, dixxp, i =>
                {
                    for (int s = iysx; s < ys2x; s++)
                    {
                        ri[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, ys], (ri[dixxp, iysx] + ri[dixxp, iysxp]) / 2, (ri[dixxp, iysxpi] + ri[dixxp, iysxpp]) / 2, sr[ix, ys + 1]);
                    }
                });
            }
            ProgressText.Text = (100 / ac).ToString();
            int xs2x = (xs + 2) * x;
            for (int sx = ys + 2; sx < os; sx++)
            {
                int dsxxm = sx * x;
                sxxm = (int)dsxxm - 1;
                int dsxx = (sx + 1) * x;
                double dsxxp = dsxxm + x50p;
                Parallel.For(ixsx, xs2x, i =>
                {
                    for (int s = dsxxm; s < dsxx; s++)
                    {
                        ri[i, s] = Bilinear(i, s, xsx, sxxm, xsxp, dsxxp, (ri[ixsx, sxxm] + ri[ixsxp, sxxm]) / 2, (ri[ixsxpi, sxxm] + ri[ixsxpp, sxxm]) / 2, sr[xs + 1, sx], sr[xs, sx]);
                    }
                });
            }

            for (int sx = ys - 1; sx > -1; sx--)
            {
                int dsxx = sx * x;
                sxxm = dsxx + x;
                double dsxxm = dsxx + x50p;

                Parallel.For(ixsx, xs2x, i =>
                {
                    for (int s = dsxx; s < sxxm; s++)
                    {
                        ri[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sr[xs, sx], sr[xs + 1, sx], (ri[ixsxpi, sxxm] + ri[ixsxpp, sxxm]) / 2, (ri[ixsx, sxxm] + ri[ixsxp, sxxm]) / 2);
                    }
                });
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
                    Parallel.For(dixxm, dixx, i =>
                    {
                        for (int s = dsxxm; s < dsxx; s++)
                        {
                            ri[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, ri[ixxm, sxxm], (ri[dixxm, sxxm] + ri[iixx, sxxm]) / 2, sr[ix, sx], (ri[ixxm, dsxxm] + ri[ixxm, isxx]) / 2);
                        }
                    });
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

                    Parallel.For(dixxm, ixxm, i =>
                    {
                        for (int s = dsxxm; s < sxxm; s++)
                        {
                            ri[i, s] = Bilinear((int)i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx], (ri[ixxm, dsxxm] + ri[ixxm, isxx]) / 2, ri[ixxm, sxxm], (ri[dixxm, sxxm] + ri[iixx, sxxm]) / 2);
                        }
                    });
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

                    Parallel.For(dixxm, dixx, i =>
                    {
                        for (int s = sxx; s < dsxx; s++)
                        {
                            ri[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (ri[dixxm, isxxm] + ri[iixx, isxxm]) / 2, ri[dixx, isxxm], (ri[dixx, isxx] + ri[dixx, sxx]) / 2, sr[ix, sx]);
                        }
                    });
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

                    Parallel.For(ixx, dixx, i =>
                    {
                        for (int s = sxx; s < dsxx; s++)
                        {
                            ri[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (ri[ixxm, sxx] + ri[ixxm, isxx]) / 2, sr[ix, sx], (ri[ixx, dsxx] + ri[iixx, dsxx]) / 2, ri[ixxm, dsxx]);
                        }
                    });
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

                        Parallel.For(ix * x, (ix + 1) * x, i =>
                        {
                            for (int s = sx * x; s < (sx + 1) * x; s++)
                            {
                                r[i, s] = Bilinear(i, s, x1, y1, x2, y2, q1, q2, q3, q4);
                                double dist1 = Math.Max(nim - i + s - x, 0);
                                double dist2 = Math.Max(Math.Abs(ixsxpi - i) + Math.Abs(iysxpi - s) - x, 0);
                                ri[i, s] = (ri[i, s] * dist1 + r[i, s] * dist2) / (dist1 + dist2);
                                ris[i, s] = Math.Min(dist1, dist2);
                            }
                        });
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


                            Parallel.For(ix * x, (ix + 1) * x, i =>
                            {
                                for (int s = sx * x; s < (sx + 1) * x; s++)
                                {
                                    r[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, q1, q2, q3, q4);
                                    double dist1 = Math.Max(i + s - x, 0);
                                    ri[i, s] = (ri[i, s] * dist1 + r[i, s] * ris[i, s]) / (dist1 + ris[i, s]);
                                    ris[i, s] = Math.Min(dist1, ris[i, s]);
                                }
                            });
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

                                Parallel.For(ix * x, (ix + 1) * x, i =>
                                {
                                    for (int s = sx * x; s < (sx + 1) * x; s++)
                                    {
                                        r[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, q1, q2, q3, q4);
                                        double dist2 = Math.Max(nim - i + nsm - s - x, 0);
                                        ri[i, s] = (ri[i, s] * dist2 + r[i, s] * ris[i, s]) / (ris[i, s] + dist2);
                                        ris[i, s] = Math.Min(ris[i, s], dist2);
                                    }
                                });
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

                                    Parallel.For(ix * x, (ix + 1) * x, i =>
                                    {
                                        for (int s = sx * x; s < (sx + 1) * x; s++)
                                        {
                                            r[i, s] = Quadrilateral(i, s, x1, y1, x2, y2, x3, y3, x4, y4, q1, q2, q3, q4);
                                            double dist2 = Math.Max(i + nsm - s - x, 0);
                                            ri[i, s] = (ri[i, s] * dist2 + r[i, s] * ris[i, s]) / (ris[i, s] + dist2);
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
            }

            byte[,] rb = new byte[ni, ns];
            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    if (ri[i, s] < maax2)
                    {
                        rb[i, s] = min;
                    }
                    else if (ri[i, s] > paax2)
                    {
                        rb[i, s] = max;
                    }
                    else
                    {
                        rb[i, s] = (byte)((ri[i, s] - maax2) / aax * rex + min + 0.5);
                    }
                }
            });

            return BMPfromGray(rb, ni, ns);
        }

        private Bitmap ScaleSeparateColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, xm, oim, osm, sxx, sxxm, ixx, ixxm, halfx;
            double x50p, x150p;
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
            byte[] dr = new byte[3];
            byte[] rmin = [255, 255, 255];
            byte[] rmax = [0, 0, 0];
            double[] rex = new double[3];
            double[] rex2 = new double[3];
            double[,,] r = new double[ni, ns, 3];
            double[,,] ri = new double[ni, ns, 3];
            double[,] ris = new double[ni, ns];
            byte[,,] sr = new byte[oi, os, 3];
            ac = ac * 100 / 42;
            if (ac > 79) ac = 80;
            else if (ac < 20) ac = 20;
            else if (ac < 40) ac = 40;
            else if (ac < 79) ac = 60;

            sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);

            Parallel.For(0, oi, ix =>
            {
                for (int sx = 0; sx < os; sx++)
                {
                    for (int t = 0; t < 3; t++)
                    {
                        if (sr[ix, sx, t] < rmin[t])
                            rmin[t] = sr[ix, sx, t];
                        else if (sr[ix, sx, t] > rmax[t])
                            rmax[t] = sr[ix, sx, t];
                    }

                }
            });

            rex[0] = rmax[0] - rmin[0];
            rex2[0] = (rmax[0] + rmin[0]) / 2;
            rex[1] = rmax[1] - rmin[1];
            rex2[1] = (rmax[1] + rmin[1]) / 2;
            rex[2] = rmax[2] - rmin[2];
            rex2[2] = (rmax[2] + rmin[2]) / 2;

            double aax = (Math.Log(x, 0.96) + 127.5) * (rex[0] + rex[1] + rex[2]) / 765; //0.96 - AntiAliasing separate, 0.99 - no AA, separate, 1.01 - average colors
            if (aax < 1) aax = 1;
            double aax2 = aax / 2;
            double[] rmaax2 = [rex2[0] - aax2, rex2[1] - aax2, rex2[2] - aax2];
            double[] rpaax2 = [rex2[0] + aax2, rex2[1] + aax2, rex2[2] + aax2];

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

            Parallel.For(ixsx, (xs + 2) * x, i =>
            {
                for (int s = iysx; s < ys2x; s++)
                {
                    double[] b3 = Bilinear3(i, s, xsx, ysx, xsxp, ysxp, sr[xs, ys, 0], sr[xsp, ys, 0], sr[xsp, ysp, 0], sr[xs, ysp, 0], sr[xs, ys, 1], sr[xsp, ys, 1], sr[xsp, ysp, 1], sr[xs, ysp, 1], sr[xs, ys, 2], sr[xsp, ys, 2], sr[xsp, ysp, 2], sr[xs, ysp, 2]);
                    ri[i, s, 0] = b3[0];
                    ri[i, s, 1] = b3[1];
                    ri[i, s, 2] = b3[2];
                }
            });

            for (int ix = xs + 2; ix < oi; ix++)
            {
                int dixxm = ix * x;
                ixxm = dixxm - 1;
                int dixx = (ix + 1) * x;
                double dixxp = dixxm + x50p;
                Parallel.For(dixxm, dixx, i =>
                {
                    for (int s = iysx; s < ys2x; s++)
                    {
                        double[] b3 = Bilinear3(i, s, ixxm, ysx, dixxp, ysxp, (ri[ixxm, iysx, 0] + ri[ixxm, iysxp, 0]) / 2, sr[ix, ys, 0], sr[ix, ys + 1, 0], (ri[ixxm, iysxpi, 0] + ri[ixxm, iysxpp, 0]) / 2, (ri[ixxm, iysx, 1] + ri[ixxm, iysxp, 1]) / 2, sr[ix, ys, 1], sr[ix, ys + 1, 1], (ri[ixxm, iysxpi, 1] + ri[ixxm, iysxpp, 1]) / 2, (ri[ixxm, iysx, 2] + ri[ixxm, iysxp, 2]) / 2, sr[ix, ys, 2], sr[ix, ys + 1, 2], (ri[ixxm, iysxpi, 2] + ri[ixxm, iysxpp, 2]) / 2);
                        ri[i, s, 0] = b3[0];
                        ri[i, s, 1] = b3[1];
                        ri[i, s, 2] = b3[2];
                    }
                });
            }

            for (int ix = xs - 1; ix > -1; ix--)
            {
                int dixx = ix * x;
                int dixxp = dixx + x;
                double dixxm = dixx + x50p;
                Parallel.For(dixx, dixxp, i =>
                {
                    for (int s = iysx; s < ys2x; s++)
                    {
                        double[] b3 = Bilinear3(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, ys, 0], (ri[dixxp, iysx, 0] + ri[dixxp, iysxp, 0]) / 2, (ri[dixxp, iysxpi, 0] + ri[dixxp, iysxpp, 0]) / 2, sr[ix, ys + 1, 0], sr[ix, ys, 1], (ri[dixxp, iysx, 1] + ri[dixxp, iysxp, 1]) / 2, (ri[dixxp, iysxpi, 1] + ri[dixxp, iysxpp, 1]) / 2, sr[ix, ys + 1, 1], sr[ix, ys, 2], (ri[dixxp, iysx, 2] + ri[dixxp, iysxp, 2]) / 2, (ri[dixxp, iysxpi, 2] + ri[dixxp, iysxpp, 2]) / 2, sr[ix, ys + 1, 2]);
                        ri[i, s, 0] = b3[0];
                        ri[i, s, 1] = b3[1];
                        ri[i, s, 2] = b3[2];
                    }
                });
            }
            ProgressText.Text = (200 / ac).ToString();
            int xs2x = (xs + 2) * x;
            for (int sx = ys + 2; sx < os; sx++)
            {
                int dsxxm = sx * x;
                sxxm = dsxxm - 1;
                int dsxx = (sx + 1) * x;
                double dsxxp = dsxxm + x50p;
                Parallel.For(ixsx, xs2x, i =>
                {
                    for (int s = dsxxm; s < dsxx; s++)
                    {
                        double[] b3 = Bilinear3(i, s, xsx, sxxm, xsxp, dsxxp, (ri[ixsx, sxxm, 0] + ri[ixsxp, sxxm, 0]) / 2, (ri[ixsxpi, sxxm, 0] + ri[ixsxpp, sxxm, 0]) / 2, sr[xs + 1, sx, 0], sr[xs, sx, 0], (ri[ixsx, sxxm, 1] + ri[ixsxp, sxxm, 1]) / 2, (ri[ixsxpi, sxxm, 1] + ri[ixsxpp, sxxm, 1]) / 2, sr[xs + 1, sx, 1], sr[xs, sx, 1], (ri[ixsx, sxxm, 2] + ri[ixsxp, sxxm, 2]) / 2, (ri[ixsxpi, sxxm, 2] + ri[ixsxpp, sxxm, 2]) / 2, sr[xs + 1, sx, 2], sr[xs, sx, 2]);
                        ri[i, s, 0] = b3[0];
                        ri[i, s, 1] = b3[1];
                        ri[i, s, 2] = b3[2];
                    }
                });
            }
            for (int sx = ys - 1; sx > -1; sx--)
            {
                int dsxx = sx * x;
                sxxm = dsxx + x;
                double dsxxm = dsxx + x50p;

                Parallel.For(ixsx, xs2x, i =>
                {
                    for (int s = dsxx; s < sxxm; s++)
                    {
                        double[] b3 = Bilinear3(i, s, xsx, dsxxm, xsxp, sxxm, sr[xs, sx, 0], sr[xs + 1, sx, 0], (ri[ixsxpi, sxxm, 0] + ri[ixsxpp, sxxm, 0]) / 2, (ri[ixsx, sxxm, 0] + ri[ixsxp, sxxm, 0]) / 2, sr[xs, sx, 1], sr[xs + 1, sx, 1], (ri[ixsxpi, sxxm, 1] + ri[ixsxpp, sxxm, 1]) / 2, (ri[ixsx, sxxm, 1] + ri[ixsxp, sxxm, 1]) / 2, sr[xs, sx, 2], sr[xs + 1, sx, 2], (ri[ixsxpi, sxxm, 2] + ri[ixsxpp, sxxm, 2]) / 2, (ri[ixsx, sxxm, 2] + ri[ixsxp, sxxm, 2]) / 2);
                        ri[i, s, 0] = b3[0];
                        ri[i, s, 1] = b3[1];
                        ri[i, s, 2] = b3[2];
                    }
                });
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

                    Parallel.For(dixxm, dixx, i =>
                    {
                        for (int s = dsxxm; s < dsxx; s++)
                        {
                            double[] b3 = Bilinear3(i, s, ixxm, sxxm, dixxp, dsxxp, ri[ixxm, sxxm, 0], (ri[dixxm, sxxm, 0] + ri[iixx, sxxm, 0]) / 2, sr[ix, sx, 0], (ri[ixxm, dsxxm, 0] + ri[ixxm, isxx, 0]) / 2, ri[ixxm, sxxm, 1], (ri[dixxm, sxxm, 1] + ri[iixx, sxxm, 1]) / 2, sr[ix, sx, 1], (ri[ixxm, dsxxm, 1] + ri[ixxm, isxx, 1]) / 2, ri[ixxm, sxxm, 2], (ri[dixxm, sxxm, 2] + ri[iixx, sxxm, 2]) / 2, sr[ix, sx, 2], (ri[ixxm, dsxxm, 2] + ri[ixxm, isxx, 2]) / 2);
                            ri[i, s, 0] = b3[0];
                            ri[i, s, 1] = b3[1];
                            ri[i, s, 2] = b3[2];
                        }
                    });
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

                    Parallel.For(dixxm, ixxm, i =>
                    {
                        for (int s = dsxxm; s < sxxm; s++)
                        {
                            double[] b3 = Bilinear3(i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx, 0], (ri[ixxm, dsxxm, 0] + ri[ixxm, isxx, 0]) / 2, ri[ixxm, sxxm, 0], (ri[dixxm, sxxm, 0] + ri[iixx, sxxm, 0]) / 2, sr[ix, sx, 1], (ri[ixxm, dsxxm, 1] + ri[ixxm, isxx, 1]) / 2, ri[ixxm, sxxm, 1], (ri[dixxm, sxxm, 1] + ri[iixx, sxxm, 1]) / 2, sr[ix, sx, 2], (ri[ixxm, dsxxm, 2] + ri[ixxm, isxx, 2]) / 2, ri[ixxm, sxxm, 2], (ri[dixxm, sxxm, 2] + ri[iixx, sxxm, 2]) / 2);
                            ri[i, s, 0] = b3[0];
                            ri[i, s, 1] = b3[1];
                            ri[i, s, 2] = b3[2];
                        }
                    });
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

                    Parallel.For(dixxm, dixx, i =>
                    {
                        for (int s = sxx; s < dsxx; s++)
                        {
                            double[] b3 = Bilinear3(i, s, dixxp, isxxm, dixx, dsxxp, (ri[dixxm, isxxm, 0] + ri[iixx, isxxm, 0]) / 2, ri[dixx, isxxm, 0], (ri[dixx, isxx, 0] + ri[dixx, sxx, 0]) / 2, sr[ix, sx, 0], (ri[dixxm, isxxm, 1] + ri[iixx, isxxm, 1]) / 2, ri[dixx, isxxm, 1], (ri[dixx, isxx, 1] + ri[dixx, sxx, 1]) / 2, sr[ix, sx, 1], (ri[dixxm, isxxm, 2] + ri[iixx, isxxm, 2]) / 2, ri[dixx, isxxm, 2], (ri[dixx, isxx, 2] + ri[dixx, sxx, 2]) / 2, sr[ix, sx, 2]);
                            ri[i, s, 0] = b3[0];
                            ri[i, s, 1] = b3[1];
                            ri[i, s, 2] = b3[2];
                        }
                    });
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

                    Parallel.For(ixx, dixx, i =>
                    {
                        for (int s = sxx; s < dsxx; s++)
                        {
                            double[] b3 = Bilinear3(i, s, ixxm, dsxxp, dixxp, dsxx, (ri[ixxm, sxx, 0] + ri[ixxm, isxx, 0]) / 2, sr[ix, sx, 0], (ri[ixx, dsxx, 0] + ri[iixx, dsxx, 0]) / 2, ri[ixxm, dsxx, 0], (ri[ixxm, sxx, 1] + ri[ixxm, isxx, 1]) / 2, sr[ix, sx, 1], (ri[ixx, dsxx, 1] + ri[iixx, dsxx, 1]) / 2, ri[ixxm, dsxx, 1], (ri[ixxm, sxx, 2] + ri[ixxm, isxx, 2]) / 2, sr[ix, sx, 2], (ri[ixx, dsxx, 2] + ri[iixx, dsxx, 2]) / 2, ri[ixxm, dsxx, 2]);
                            ri[i, s, 0] = b3[0];
                            ri[i, s, 1] = b3[1];
                            ri[i, s, 2] = b3[2];
                        }
                    });
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
                            {                           // 1     [2]
                                x1 = oimx - x;          //  
                                y1 = x50p;              //  
                                rq1 = sr[ix - 1, 0, 0]; // 4      3
                                gq1 = sr[ix - 1, 0, 1];
                                bq1 = sr[ix - 1, 0, 2];

                                x2 = oimx;
                                y2 = x150p;

                                rq3 = sr[ix, 1, 0];
                                rq2 = sr[ix, 0, 0];
                                rq4 = sr[ix - 1, 1, 0];
                                gq3 = sr[ix, 1, 1];
                                gq2 = sr[ix, 0, 1];
                                gq4 = sr[ix - 1, 1, 1];
                                bq3 = sr[ix, 1, 2];
                                bq2 = sr[ix, 0, 2];
                                bq4 = sr[ix - 1, 1, 2];
                            }
                            else
                            {
                                y1 = sx * x;            //   1    2
                                x1 = oimx - x50p;       //         
                                y2 = y1 + x50p;         //  4/3  [3]
                                rq4 = (double)(sr[ix, sx, 0] + sr[ix, sx - 1, 0]) / 2;
                                gq4 = (double)(sr[ix, sx, 1] + sr[ix, sx - 1, 1]) / 2;
                                bq4 = (double)(sr[ix, sx, 1] + sr[ix, sx - 1, 1]) / 2;

                                x2 = oimx;
                                y1--;

                                rq2 = (r[nim, (int)y1, 0] + r[nim - xm, (int)y1, 0]) / 2;
                                rq3 = sr[ix, sx, 0];
                                rq1 = r[(int)x1, (int)y1, 0];
                                gq2 = (r[nim, (int)y1, 1] + r[nim - xm, (int)y1, 1]) / 2;
                                gq3 = sr[ix, sx, 1];
                                gq1 = r[(int)x1, (int)y1, 1];
                                bq2 = (r[nim, (int)y1, 2] + r[nim - xm, (int)y1, 2]) / 2;
                                bq3 = sr[ix, sx, 2];
                                bq1 = r[(int)x1, (int)y1, 2];
                            }
                        }
                        else
                        {
                            if (sx == 0)
                            {
                                x1 = ix * x;
                                ixx = (int)(x1);
                                ixxm = ixx + x;
                                x1 += x50p;          //         
                                y1 = x50p;           //   [1]   2
                                rq1 = sr[ix, 0, 0];  //   1/4   3
                                gq1 = sr[ix, 0, 1];
                                bq1 = sr[ix, 0, 2];

                                x2 = ixxm;
                                y2 = xm;

                                rq3 = r[ixxm, xm, 0];
                                rq2 = (r[ixxm, 0, 0] + r[ixxm, xm, 0]) / 2;
                                rq4 = (double)(sr[ix, 0, 0] + sr[ix, 1, 0]) / 2;
                                gq3 = r[ixxm, xm, 1];
                                gq2 = (r[ixxm, 0, 1] + r[ixxm, xm, 1]) / 2;
                                gq4 = (double)(sr[ix, 0, 1] + sr[ix, 1, 1]) / 2;
                                bq3 = r[ixxm, xm, 2];
                                bq2 = (r[ixxm, 0, 2] + r[ixxm, xm, 2]) / 2;
                                bq4 = (double)(sr[ix, 0, 2] + sr[ix, 1, 2]) / 2;
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
                                rq2 = r[(int)x2, sxxm, 0];
                                gq2 = r[(int)x2, sxxm, 1];
                                bq2 = r[(int)x2, sxxm, 2];

                                x1 = x3 + x50p;
                                y2 = y3 + x50p;
                                if (xEven)
                                {
                                    rq1 = (r[ixx + halfx, sxxm, 0] + r[ixx + halfx - 1, sxxm, 0]) / 2;
                                    gq1 = (r[ixx + halfx, sxxm, 1] + r[ixx + halfx - 1, sxxm, 1]) / 2;
                                    bq1 = (r[ixx + halfx, sxxm, 2] + r[ixx + halfx - 1, sxxm, 2]) / 2;
                                }
                                else
                                {
                                    rq1 = r[ixx + halfx, sxxm, 0];
                                    gq1 = r[ixx + halfx, sxxm, 1];
                                    bq1 = r[ixx + halfx, sxxm, 2];
                                }

                                rq4 = sr[ix, sx, 0];
                                gq4 = sr[ix, sx, 1];
                                bq4 = sr[ix, sx, 2];

                                if (xEven)
                                {
                                    rq3 = (r[(int)x2, sxx + halfx, 0] + r[(int)x2, sxx + halfx - 1, 0]) / 2;
                                    gq3 = (r[(int)x2, sxx + halfx, 1] + r[(int)x2, sxx + halfx - 1, 1]) / 2;
                                    bq3 = (r[(int)x2, sxx + halfx, 2] + r[(int)x2, sxx + halfx - 1, 2]) / 2;
                                }
                                else
                                {
                                    rq3 = r[(int)x2, sxx + halfx, 0];
                                    bq3 = r[(int)x2, sxx + halfx, 1];
                                    gq3 = r[(int)x2, sxx + halfx, 2];
                                }
                            }
                        }
                        Parallel.For(ix * x, (ix + 1) * x, i =>
                        {
                            for (int s = sx * x; s < (sx + 1) * x; s++)
                            {
                                double[] b3 = Bilinear3(i, s, x1, y1, x2, y2, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1];
                                r[i, s, 2] = b3[2];
                                double dist1 = Math.Max(nim - i + s - x, 0);
                                double dist2 = Math.Max(Math.Abs(ixsxpi - i) + Math.Abs(iysxpi - s) - x, 0);
                                double sdist = dist1 + dist2;
                                double dis1sdist = dist1 / sdist;
                                double dis2sdist = dist2 / sdist;
                                ri[i, s, 0] = ri[i, s, 0] * dis1sdist + r[i, s, 0] * dis2sdist;
                                ri[i, s, 1] = ri[i, s, 1] * dis1sdist + r[i, s, 1] * dis2sdist;
                                ri[i, s, 2] = ri[i, s, 2] * dis1sdist + r[i, s, 2] * dis2sdist;
                                ris[i, s] = Math.Min(dist1, dist2);
                            }
                        });
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
                                {                     //[1]     2
                                    x1 = x50p;        //  
                                    y1 = x50p;        //  
                                    rq1 = sr[0, 0, 0];// 4      3
                                    gq1 = sr[0, 0, 1];
                                    bq1 = sr[0, 0, 2];

                                    x2 = x150p;
                                    y2 = x50p;
                                    rq2 = sr[1, 0, 0];
                                    gq2 = sr[1, 0, 1];
                                    bq2 = sr[1, 0, 2];

                                    x3 = x150p;
                                    y3 = x150p;
                                    rq3 = sr[1, 1, 0];
                                    gq3 = sr[1, 1, 1];
                                    bq3 = sr[1, 1, 2];

                                    x4 = x50p;
                                    y4 = x150p;
                                    rq4 = sr[0, 1, 0];
                                    gq4 = sr[0, 1, 1];
                                    bq4 = sr[0, 1, 2];
                                }
                                else
                                {
                                    y4 = sx * x;
                                    sxxm = (int)(y4) - 1;  // 1   2
                                    x4 = x50p;             //         
                                    y4 += x50p;            //  [4]     3
                                    rq4 = sr[0, sx, 0];
                                    gq4 = sr[0, sx, 1];
                                    bq4 = sr[0, sx, 2];

                                    x3 = x150p;
                                    y3 = y4;
                                    rq3 = sr[1, sx, 0];
                                    gq3 = sr[1, sx, 1];
                                    bq3 = sr[1, sx, 2];

                                    x2 = xm;
                                    y2 = sxxm;
                                    rq2 = r[xm, sxxm, 0];
                                    gq2 = r[xm, sxxm, 1];
                                    bq2 = r[xm, sxxm, 2];

                                    x1 = 0;
                                    y1 = sxxm;
                                    rq1 = r[0, sxxm, 0];
                                    gq1 = r[0, sxxm, 1];
                                    bq1 = r[0, sxxm, 2];
                                }
                            }
                            else
                            {
                                if (sx == 0)
                                {
                                    ixx = (int)(ix * x);
                                    ixxm = ixx - 1;
                                    x2 = ix * x + x50p;    //    1   [2]
                                    y2 = x50p;             //  
                                    rq2 = sr[ix, 0, 0];    //    4   2/3
                                    gq2 = sr[ix, 0, 1];
                                    bq2 = sr[ix, 0, 2];

                                    x3 = x2;
                                    y3 = xm;
                                    rq3 = (double)(sr[ix, 1, 0] + sr[ix, 0, 0]) / 2;
                                    gq3 = (double)(sr[ix, 1, 1] + sr[ix, 0, 1]) / 2;
                                    bq3 = (double)(sr[ix, 1, 2] + sr[ix, 0, 2]) / 2;

                                    x1 = ixxm;
                                    y1 = x50p;
                                    rq1 = (r[ixxm, 0, 0] + r[ixxm, xm, 0]) / 2;
                                    gq1 = (r[ixxm, 0, 1] + r[ixxm, xm, 1]) / 2;
                                    bq1 = (r[ixxm, 0, 2] + r[ixxm, xm, 2]) / 2;

                                    x4 = ixxm;
                                    y4 = xm;
                                    rq4 = r[ixxm, xm, 0];
                                    gq4 = r[ixxm, xm, 1];
                                    bq4 = r[ixxm, xm, 2];

                                }
                                else
                                {
                                    ixx = (int)(ix * x);
                                    ixxm = ixx - 1;
                                    sxx = (int)(sx * x);
                                    sxxm = sxx - 1;

                                    x2 = ixx + xm;             //     1         2 
                                    y2 = sxxm;                 //   
                                    rq2 = r[(int)x2, sxxm, 0]; //    4    [3]
                                    gq2 = r[(int)x2, sxxm, 1];
                                    bq2 = r[(int)x2, sxxm, 2];

                                    x1 = ixx;
                                    y1 = sxxm;
                                    rq1 = r[ixx, sxxm, 0];
                                    gq1 = r[ixx, sxxm, 1];
                                    bq1 = r[ixx, sxxm, 2];

                                    x3 = ix * x + x50p;
                                    y3 = sx * x + x50p;
                                    rq3 = sr[ix, sx, 0];
                                    gq3 = sr[ix, sx, 1];
                                    bq3 = sr[ix, sx, 2];

                                    x4 = ixxm;
                                    y4 = y3;
                                    if (xEven)
                                    {
                                        rq4 = (r[ixxm, sxx + halfx, 0] + r[ixxm, sxx + halfx - 1, 0]) / 2;
                                        gq4 = (r[ixxm, sxx + halfx, 1] + r[ixxm, sxx + halfx - 1, 1]) / 2;
                                        bq4 = (r[ixxm, sxx + halfx, 2] + r[ixxm, sxx + halfx - 1, 2]) / 2;
                                    }
                                    else
                                    {
                                        rq4 = r[ixxm, sxx + halfx, 0];
                                        gq4 = r[ixxm, sxx + halfx, 1];
                                        bq4 = r[ixxm, sxx + halfx, 2];
                                    }
                                }
                            }

                            Parallel.For(ix * x, (ix + 1) * x, i =>
                            {
                                for (int s = sx * x; s < (sx + 1) * x; s++)
                                {
                                    double[] b3 = Quadrilateral3(i, s, x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4);

                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];
                                    double dist1 = Math.Max(i + s - x, 0);
                                    double sdist = dist1 + ris[i, s];
                                    double risdist = ris[i, s] / sdist;
                                    double disdist = dist1 / sdist;
                                    ri[i, s, 0] = ri[i, s, 0] * disdist + r[i, s, 0] * risdist;
                                    ri[i, s, 1] = ri[i, s, 1] * disdist + r[i, s, 1] * risdist;
                                    ri[i, s, 2] = ri[i, s, 2] * disdist + r[i, s, 2] * risdist;

                                    ris[i, s] = Math.Min(dist1, ris[i, s]);
                                }
                            });
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
                                    {                         // 1      2
                                        x3 = oimx;            //  
                                        y3 = osmx;            //  
                                        rq3 = sr[oim, osm, 0];// 4     [3]
                                        gq3 = sr[oim, osm, 1];
                                        bq3 = sr[oim, osm, 2];

                                        x2 = x3;
                                        y2 = y3 - x;
                                        rq2 = sr[oim, osmm, 0];
                                        gq2 = sr[oim, osmm, 1];
                                        bq2 = sr[oim, osmm, 2];

                                        x1 = x3 - x;
                                        y1 = y2;
                                        rq1 = sr[oimm, osmm, 0];
                                        gq1 = sr[oimm, osmm, 1];
                                        bq1 = sr[oimm, osmm, 2];

                                        x4 = x1;
                                        y4 = x3;
                                        rq4 = sr[0, 1, 0];
                                        gq4 = sr[0, 1, 1];
                                        bq4 = sr[0, 1, 2];
                                    }
                                    else
                                    {
                                        sxx = (int)(sx * x);      //1    [2]
                                        x2 = oimx;                //         
                                        y2 = sx * x + x50p;       //   4     3
                                        rq2 = sr[oim, sx, 0];
                                        gq2 = sr[oim, sx, 1];
                                        bq2 = sr[oim, sx, 2];

                                        x3 = nim;
                                        y3 = sxx + x;
                                        rq3 = r[nim, (int)y3, 0];
                                        gq3 = r[nim, (int)y3, 1];
                                        bq3 = r[nim, (int)y3, 2];

                                        x4 = nim - xm;
                                        y4 = y3;
                                        rq4 = r[(int)x4, (int)y4, 0];
                                        gq4 = r[(int)x4, (int)y4, 1];
                                        bq4 = r[(int)x4, (int)y4, 2];

                                        x1 = x2 - x;
                                        y1 = y2;
                                        rq1 = sr[oimm, sx, 0];
                                        gq1 = sr[oimm, sx, 1];
                                        bq1 = sr[oimm, sx, 2];
                                    }
                                }
                                else
                                {
                                    if (sx == osm)
                                    {
                                        ixx = (int)(ix * x);
                                        x4 = ix * x + x50p;   //   1/4  2
                                        y4 = osmx;            //  
                                        rq4 = sr[ix, osm, 0]; //   [4]  3
                                        gq4 = sr[ix, osm, 1];
                                        bq4 = sr[ix, osm, 2];

                                        x3 = ixx + x;
                                        y3 = osmx;
                                        rq3 = (r[(int)x3, nsm, 0] + r[(int)x3, nsm - xm, 0]) / 2;
                                        gq3 = (r[(int)x3, nsm, 1] + r[(int)x3, nsm - xm, 1]) / 2;
                                        bq3 = (r[(int)x3, nsm, 2] + r[(int)x3, nsm - xm, 2]) / 2;

                                        x1 = x4;
                                        y1 = y4 - x50p;
                                        rq1 = (double)(sr[ix, osmm, 0] + sr[ix, osm, 0]) / 2;
                                        gq1 = (double)(sr[ix, osmm, 1] + sr[ix, osm, 1]) / 2;
                                        bq1 = (double)(sr[ix, osmm, 2] + sr[ix, osm, 2]) / 2;

                                        x2 = x3;
                                        y2 = y1;
                                        rq2 = r[(int)x2, (int)y2, 0];
                                        gq2 = r[(int)x2, (int)y2, 1];
                                        bq2 = r[(int)x2, (int)y2, 2];
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
                                            rq2 = (r[(int)x2, sxx + halfx, 0] + r[(int)x2, sxx + halfx - 1, 0]) / 2;
                                            gq2 = (r[(int)x2, sxx + halfx, 1] + r[(int)x2, sxx + halfx - 1, 1]) / 2;
                                            bq2 = (r[(int)x2, sxx + halfx, 2] + r[(int)x2, sxx + halfx - 1, 2]) / 2;
                                        }
                                        else
                                        {
                                            rq2 = r[(int)x2, sxx + halfx, 0];
                                            gq2 = r[(int)x2, sxx + halfx, 1];
                                            bq2 = r[(int)x2, sxx + halfx, 2];
                                        }

                                        x3 = x2 - 1;
                                        y3 = y4 + x;
                                        rq3 = r[(int)x3, (int)y3, 0];
                                        gq3 = r[(int)x3, (int)y3, 1];
                                        bq3 = r[(int)x3, (int)y3, 2];

                                        x1 = x4 + x50p;
                                        y1 = y2;
                                        rq1 = sr[ix, sx, 0];
                                        gq1 = sr[ix, sx, 1];
                                        bq1 = sr[ix, sx, 2];

                                        y4 = y3;
                                        rq4 = r[ixx, (int)y4, 0];
                                        gq4 = r[ixx, (int)y4, 1];
                                        bq4 = r[ixx, (int)y4, 2];
                                    }
                                }

                                Parallel.For(ix * x, (ix + 1) * x, i =>
                                {
                                    for (int s = sx * x; s < (sx + 1) * x; s++)
                                    {
                                        double[] b3 = Quadrilateral3(i, s, x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4);
                                        r[i, s, 0] = b3[0];
                                        r[i, s, 1] = b3[1];
                                        r[i, s, 2] = b3[2];
                                        double dist2 = Math.Max(nim - i + nsm - s - x, 0);
                                        double sdist = ris[i, s] + dist2;
                                        double dist2sdist = dist2 / sdist;
                                        double rissdist = ris[i, s] / sdist;
                                        ri[i, s, 0] = ri[i, s, 0] * dist2sdist + r[i, s, 0] * rissdist;
                                        ri[i, s, 1] = ri[i, s, 1] * dist2sdist + r[i, s, 1] * rissdist;
                                        ri[i, s, 2] = ri[i, s, 2] * dist2sdist + r[i, s, 2] * rissdist;
                                        ris[i, s] = Math.Min(ris[i, s], dist2);
                                    }
                                });
                            }
                        }

                        if (ac >= 80)
                        {
                            for (int sx = osm; sx > -1; sx--)
                            {
                                ProgressText.Text = ((7000 - 1000 * (sx - osm) / osm) / ac).ToString();
                                for (int ix = 0; ix < oi; ix++)
                                {
                                    if (ix == 0)
                                    {
                                        if (sx == osm)
                                        {                          // 1      2
                                            x1 = x50p;             //  
                                            y1 = osmx - x;         //  
                                            rq1 = sr[0, sx - 1, 0];//[4]     3
                                            gq1 = sr[0, sx - 1, 1];
                                            bq1 = sr[0, sx - 1, 2];

                                            x2 = x150p;
                                            y2 = y1;
                                            rq2 = sr[1, sx - 1, 0];
                                            gq2 = sr[1, sx - 1, 1];
                                            bq2 = sr[1, sx - 1, 2];

                                            x3 = x150p;
                                            y3 = osmx;
                                            rq3 = sr[1, sx, 0];
                                            gq3 = sr[1, sx, 1];
                                            bq3 = sr[1, sx, 2];

                                            x4 = x50p;
                                            y4 = y3;
                                            rq4 = sr[0, sx, 0];
                                            gq4 = sr[0, sx, 1];
                                            bq4 = sr[0, sx, 2];
                                        }
                                        else
                                        {
                                            y1 = sx * x;          //  [1] 1/2
                                            x4 = x50p;            //         
                                            y4 = y1 + x;          //   4   3
                                            rq4 = (r[0, (int)y4, 0] + r[xm, (int)y4, 0]) / 2;
                                            gq4 = (r[0, (int)y4, 1] + r[xm, (int)y4, 1]) / 2;
                                            bq4 = (r[0, (int)y4, 2] + r[xm, (int)y4, 2]) / 2;

                                            x2 = xm;
                                            y2 = y1 + x50p;
                                            rq2 = (double)(sr[1, sx, 0] + sr[0, sx, 0]) / 2;
                                            gq2 = (double)(sr[1, sx, 1] + sr[0, sx, 1]) / 2;
                                            bq2 = (double)(sr[1, sx, 2] + sr[0, sx, 2]) / 2;

                                            x1 = x50p;
                                            y1 = y2;
                                            rq1 = sr[0, sx, 0];
                                            gq1 = sr[0, sx, 1];
                                            bq1 = sr[0, sx, 2];

                                            x3 = xm;
                                            y3 = y4;
                                            rq3 = r[xm, (int)y4, 0];
                                            gq3 = r[xm, (int)y4, 1];
                                            bq3 = r[xm, (int)y4, 2];
                                        }
                                    }
                                    else
                                    {
                                        if (sx == osm)
                                        {
                                            x2 = ix * x;
                                            ixx = (int)(x2);
                                            ixxm = ixx - 1;
                                            x2 += x50p;                                        //        2/3
                                            y2 = ns - x - 0.5;                                 //    1   
                                            rq2 = (double)(sr[ix, osmm, 0] + sr[ix, osm, 0]) / 2; //    4   [3]
                                            gq2 = (double)(sr[ix, osmm, 1] + sr[ix, osm, 1]) / 2;
                                            bq2 = (double)(sr[ix, osmm, 2] + sr[ix, osm, 2]) / 2;

                                            x3 = x2;
                                            y3 = osmx;
                                            rq3 = sr[ix, sx, 0];
                                            gq3 = sr[ix, sx, 1];
                                            bq3 = sr[ix, sx, 2];

                                            x1 = ixxm;
                                            y1 = ns - x;
                                            rq1 = r[ixxm, (int)y1, 0];
                                            gq1 = r[ixxm, (int)y1, 1];
                                            bq1 = r[ixxm, (int)y1, 2];

                                            x4 = ixxm;
                                            y4 = osmx;
                                            rq4 = (r[ixxm, ns - halfx, 0] + r[ixxm, ns - halfx - 1, 0]) / 2;
                                            gq4 = (r[ixxm, ns - halfx, 1] + r[ixxm, ns - halfx - 1, 1]) / 2;
                                            bq4 = (r[ixxm, ns - halfx, 2] + r[ixxm, ns - halfx - 1, 2]) / 2;
                                        }
                                        else
                                        {
                                            x2 = ix * x;
                                            ixx = (int)(x2);
                                            ixxm = ixx - 1;
                                            y2 = sx * x;
                                            sxx = (int)(y2);

                                            x4 = ixxm;
                                            y4 = y2 + x;                 //    1   [2]
                                            rq4 = r[ixxm, (int)y4, 0];   //    4    3
                                            gq4 = r[ixxm, (int)y4, 1];
                                            bq4 = r[ixxm, (int)y4, 2];

                                            x1 = ixxm;
                                            y1 = y2 + x50p;
                                            if (xEven)
                                            {
                                                rq1 = (r[ixxm, sxx + halfx, 0] + r[ixxm, sxx + halfx - 1, 0]) / 2;
                                                gq1 = (r[ixxm, sxx + halfx, 1] + r[ixxm, sxx + halfx - 1, 1]) / 2;
                                                bq1 = (r[ixxm, sxx + halfx, 2] + r[ixxm, sxx + halfx - 1, 2]) / 2;
                                            }
                                            else
                                            {
                                                rq1 = r[ixxm, sxx + halfx, 0];
                                                gq1 = r[ixxm, sxx + halfx, 1];
                                                bq1 = r[ixxm, sxx + halfx, 2];
                                            }


                                            x2 += x50p;
                                            y2 = y1;
                                            rq2 = sr[ix, sx, 0];
                                            gq2 = sr[ix, sx, 1];
                                            bq2 = sr[ix, sx, 2];

                                            x3 = x2;
                                            y3 = y4;
                                            if (xEven)
                                            {
                                                rq3 = (r[ixx + halfx, (int)y3, 0] + r[ixx + halfx - 1, (int)y3, 0]) / 2;
                                                gq3 = (r[ixx + halfx, (int)y3, 1] + r[ixx + halfx - 1, (int)y3, 1]) / 2;
                                                bq3 = (r[ixx + halfx, (int)y3, 2] + r[ixx + halfx - 1, (int)y3, 2]) / 2;
                                            }
                                            else
                                            {
                                                rq3 = r[ixx + halfx, (int)y3, 0];
                                                gq3 = r[ixx + halfx, (int)y3, 1];
                                                bq3 = r[ixx + halfx, (int)y3, 2];
                                            }
                                        }
                                    }

                                    Parallel.For(ix * x, (ix + 1) * x, i =>
                                    {
                                        for (int s = sx * x; s < (sx + 1) * x; s++)
                                        {
                                            double[] b3 = Quadrilateral3(i, s, x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4);
                                            r[i, s, 0] = b3[0];
                                            r[i, s, 1] = b3[1];
                                            r[i, s, 2] = b3[2];
                                            double dist2 = Math.Max(i + nsm - s - x, 0);
                                            double sdist = ris[i, s] + dist2;
                                            double disdist = dist2 / sdist;
                                            double risdist = ris[i, s] / sdist;
                                            ri[i, s, 0] = ri[i, s, 0] * disdist + r[i, s, 0] * risdist;
                                            ri[i, s, 1] = ri[i, s, 1] * disdist + r[i, s, 1] * risdist;
                                            ri[i, s, 2] = ri[i, s, 2] * disdist + r[i, s, 2] * risdist;
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
            }

            byte[,,] rb = new byte[ni, ns, 3];
            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    for (int t = 0; t < 3; t++)
                    {
                        if (ri[i, s, t] < rmaax2[t])
                        {
                            rb[i, s, t] = rmin[t];
                        }
                        else if (ri[i, s, t] > rpaax2[t])
                        {
                            rb[i, s, t] = rmax[t];
                        }
                        else
                        {
                            rb[i, s, t] = (byte)((ri[i, s, t] - rmaax2[t]) * rex[t] / aax + rmin[t] + 0.5);
                        }
                    }
                }
            });

            return BMPfromRGB(rb, ni, ns);
        }

        static double[] Quadrilateral3(int x, int y, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double rq1, double rq2, double rq3, double rq4, double gq1, double gq2, double gq3, double gq4, double bq1, double bq2, double bq3, double bq4)
        {
            double r1, r2, ry1, ry2, r12, r21, r34, r43, d21, d34, dry21;
            d21 = x2 - x1;
            d34 = x3 - x4;
            r21 = (x2 - x) / d21;
            r12 = (x - x1) / d21;
            r43 = (x - x4) / d34;
            r34 = (x3 - x) / d34;
            ry1 = r21 * y1 + r12 * y2;
            ry2 = r34 * y4 + r43 * y3;
            dry21 = ry2 - ry1;
            double ry2y = (ry2 - y) / dry21;
            double yry1 = (y - ry1) / dry21;

            double[] res = new double[3];
            r1 = r21 * rq1 + r12 * rq2;
            r2 = r34 * rq4 + r43 * rq3;
            res[0] = ry2y * r1 + yry1 * r2;
            if (double.IsNaN(res[0])) //?
            {
                res[0] = (r1 + r2) / 2;
            }

            r1 = r21 * gq1 + r12 * gq2;
            r2 = r34 * gq4 + r43 * gq3;
            res[1] = ry2y * r1 + yry1 * r2;
            if (double.IsNaN(res[1])) //?
            {
                res[1] = (r1 + r2) / 2;
            }

            r1 = r21 * bq1 + r12 * bq2;
            r2 = r34 * bq4 + r43 * bq3;
            res[2] = ry2y * r1 + yry1 * r2;
            if (double.IsNaN(res[2])) //?
            {
                res[2] = (r1 + r2) / 2;
            }

            return res;
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
            if (double.IsNaN((ry2 - y) / dry21 * r1 + (y - ry1) / dry21 * r2))
            {
                return (r1 + r2) / 2;
            }
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

        static double[] Bilinear3(int x, int y, double x1, double y1, double x2, double y2, double q1r, double q2r, double q3r, double q4r, double q1g, double q2g, double q3g, double q4g, double q1b, double q2b, double q3b, double q4b)
        {
            double r1r, r2r, r1g, r2g, r1b, r2b, r12, r21, d21, y21;
            d21 = x2 - x1;
            y21 = y2 - y1;
            r21 = (x2 - x) / d21;
            r12 = (x - x1) / d21;
            r1r = r21 * q1r + r12 * q2r;
            r2r = r21 * q4r + r12 * q3r;
            r1g = r21 * q1g + r12 * q2g;
            r2g = r21 * q4g + r12 * q3g;
            r1b = r21 * q1b + r12 * q2b;
            r2b = r21 * q4b + r12 * q3b;
            double y2yy21 = (y2 - y) / y21;
            double yy1y21 = (y - y1) / y21;
            return [y2yy21 * r1r + yy1y21 * r2r, y2yy21 * r1g + yy1y21 * r2g, y2yy21 * r1b + yy1y21 * r2b];
        }

        private Bitmap ScaleBilinearApproximationGray(Image img, int x, int ac)
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
            double[,] r = new double[ni, ns];
            double[,] ri = new double[ni, ns];
            double[,] ris = new double[ni, ns];
            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);

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

                        Parallel.For(ixsx, (kki + 2) * x, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                ri[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks], sr[xsp, kks], sr[xsp, ysp], sr[kki, ysp]);
                                ris[i, s] = Math.Pow(2, Math.Max(Math.Abs(i - nxs) / ki, Math.Abs(s - nys) / ks) - x);
                            }
                        });
                    }
                }
            }
            double k = 2d; //0 - save derivative; 32 - save mean value 
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

                    Parallel.For(ixsx, (kki + 2) * x, i =>
                    {
                        for (int s = iysx; s < ys2x; s++)
                        {
                            r[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks], sr[xsp, kks], sr[xsp, ysp], sr[kki, ysp]);
                            double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), k);
                            ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                            ris[i, s] = dist + ris[i, s];
                        }
                    });

                    for (int ix = kki + 2; ix < kki + 2 + ki2 && ix < oi; ix++)
                    {
                        int dixxm = ix * x;
                        ixxm = dixxm - 1;
                        int dixx = (ix + 1) * x;
                        double dixxp = dixxm + x50p;

                        Parallel.For(dixxm, dixx, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                r[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (r[ixxm, iysx] + r[ixxm, iysxp]) / 2, sr[ix, kks], sr[ix, kks + 1], (r[ixxm, iysxpi] + r[ixxm, iysxpp]) / 2);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), k);
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                ris[i, s] = dist + ris[i, s];
                            }
                        });
                    }

                    for (int ix = kki - 1; ix > kki - 1 - ki2 && ix > -1; ix--)
                    {
                        int dixx = ix * x;
                        int dixxp = dixx + x;
                        double dixxm = dixx + x50p;

                        Parallel.For(dixx, dixxp, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                r[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, kks], (r[dixxp, iysx] + r[dixxp, iysxp]) / 2, (r[dixxp, iysxpi] + r[dixxp, iysxpp]) / 2, sr[ix, kks + 1]);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), k);
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                ris[i, s] = dist + ris[i, s];
                            }
                        });
                    }

                    int xs2x = (kki + 2) * x;
                    for (int sx = kks + 2; sx < kks + 2 + ks2 && sx < os; sx++)
                    {
                        int dsxxm = sx * x;
                        sxxm = (int)dsxxm - 1;
                        int dsxx = (sx + 1) * x;
                        double dsxxp = dsxxm + x50p;

                        Parallel.For(ixsx, xs2x, i =>
                        {
                            for (int s = dsxxm; s < dsxx; s++)
                            {
                                r[i, s] = Bilinear(i, s, xsx, sxxm, xsxp, dsxxp, (r[ixsx, sxxm] + r[ixsxp, sxxm]) / 2, (r[ixsxpi, sxxm] + r[ixsxpp, sxxm]) / 2, sr[kki + 1, sx], sr[kki, sx]);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), k);
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                ris[i, s] = dist + ris[i, s];
                            }
                        });
                    }

                    for (int sx = kks - 1; sx > kks - 1 - ks2 && sx > -1; sx--)
                    {
                        int dsxx = sx * x;
                        sxxm = dsxx + x;
                        double dsxxm = dsxx + x50p;

                        Parallel.For(ixsx, xs2x, i =>
                        {
                            for (int s = dsxx; s < sxxm; s++)
                            {
                                r[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sr[kki, sx], sr[xsp, sx], (r[ixsxpi, sxxm] + r[ixsxpp, sxxm]) / 2, (r[ixsx, sxxm] + r[ixsxp, sxxm]) / 2);
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), k);
                                ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                ris[i, s] = dist + ris[i, s];
                            }
                        });
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

                            Parallel.For(dixxm, dixx, i =>
                            {
                                for (int s = dsxxm; s < dsxx; s++)
                                {
                                    r[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, r[ixxm, sxxm], (r[dixxm, sxxm] + r[iixx, sxxm]) / 2, sr[ix, sx], (r[ixxm, dsxxm] + r[ixxm, isxx]) / 2);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), k);
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                    ris[i, s] = dist + ris[i, s];
                                }
                            });
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

                            Parallel.For(dixxm, ixxm, i =>
                            {
                                for (int s = dsxxm; s < sxxm; s++)
                                {
                                    r[i, s] = Bilinear(i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx], (r[ixxm, dsxxm] + r[ixxm, isxx]) / 2, r[ixxm, sxxm], (r[dixxm, sxxm] + r[iixx, sxxm]) / 2);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), k);
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                    ris[i, s] = dist + ris[i, s];
                                }
                            });
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

                            Parallel.For(dixxm, dixx, i =>
                            {
                                for (int s = sxx; s < dsxx; s++)
                                {
                                    r[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (r[dixxm, isxxm] + r[iixx, isxxm]) / 2, r[dixx, isxxm], (r[dixx, isxx] + r[dixx, sxx]) / 2, sr[ix, sx]);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), k);
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                    ris[i, s] = dist + ris[i, s];
                                }
                            });
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

                            Parallel.For(ixx, dixx, i =>
                            {
                                for (int s = sxx; s < dsxx; s++)
                                {
                                    r[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (r[ixxm, sxx] + r[ixxm, isxx]) / 2, sr[ix, sx], (r[ixx, dsxx] + r[iixx, dsxx]) / 2, r[ixxm, dsxx]);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), k);
                                    ri[i, s] = (r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]);
                                    ris[i, s] = dist + ris[i, s];
                                }
                            });
                        }
                    }
                }
            }

            byte[,] rb = new byte[ni, ns];
            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    if (ri[i, s] < 0.5)
                        rb[i, s] = 0;
                    else if (ri[i, s] > 254.5)
                        rb[i, s] = 255;
                    else
                        rb[i, s] = (byte)(ri[i, s] + 0.5);
                }
            });

            return BMPfromGray(rb, ni, ns);
        }

        private Bitmap ScaleBilinearApproximationColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, xm, oim, osm, sxx, sxxm, ixx, ixxm;
            double halfx = (double)x / 2;
            double x50p = halfx - 0.5;

            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            xm = x - 1;
            double[,,] r = new double[ni, ns, 3];
            double[,,] ri = new double[ni, ns, 3];
            double[,] ris = new double[ni, ns];
            byte[,,] sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);

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

                        Parallel.For(ixsx, (kki + 2) * x, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                double[] b3 = Bilinear3(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks, 0], sr[xsp, kks, 0], sr[xsp, ysp, 0], sr[kki, ysp, 0], sr[kki, kks, 1], sr[xsp, kks, 1], sr[xsp, ysp, 1], sr[kki, ysp, 1], sr[kki, kks, 2], sr[xsp, kks, 2], sr[xsp, ysp, 2], sr[kki, ysp, 2]);
                                ri[i, s, 0] = b3[0];
                                ri[i, s, 1] = b3[1];
                                ri[i, s, 2] = b3[2];
                                ris[i, s] = Math.Pow(2, Math.Max(Math.Abs(i - nxs) / ki, Math.Abs(s - nys) / ks) - x);
                            }
                        });
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

                    Parallel.For(ixsx, (kki + 2) * x, i =>
                    {
                        for (int s = iysx; s < ys2x; s++)
                        {
                            double[] b3 = Bilinear3(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks, 0], sr[xsp, kks, 0], sr[xsp, ysp, 0], sr[kki, ysp, 0], sr[kki, kks, 1], sr[xsp, kks, 1], sr[xsp, ysp, 1], sr[kki, ysp, 1], sr[kki, kks, 2], sr[xsp, kks, 2], sr[xsp, ysp, 2], sr[kki, ysp, 2]);
                            r[i, s, 0] = b3[0];
                            r[i, s, 1] = b3[1];
                            r[i, s, 2] = b3[2];
                            double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                            double sdist = dist + ris[i, s];
                            double risdist = ris[i, s] / sdist;
                            double disdist = dist / sdist;
                            ri[i, s, 0] = r[i, s, 0] * disdist + ri[i, s, 0] * risdist;
                            ri[i, s, 1] = r[i, s, 1] * disdist + ri[i, s, 1] * risdist;
                            ri[i, s, 2] = r[i, s, 2] * disdist + ri[i, s, 2] * risdist;
                            ris[i, s] = sdist;
                        }
                    });

                    for (int ix = kki + 2; ix < kki + 2 + ki2 && ix < oi; ix++)
                    {
                        int dixxm = ix * x;
                        ixxm = dixxm - 1;
                        int dixx = (ix + 1) * x;
                        double dixxp = dixxm + x50p;

                        Parallel.For(dixxm, dixx, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                double[] b3 = Bilinear3(i, s, ixxm, ysx, dixxp, ysxp, (r[ixxm, iysx, 0] + r[ixxm, iysxp, 0]) / 2, sr[ix, kks, 0], sr[ix, kks + 1, 0], (r[ixxm, iysxpi, 0] + r[ixxm, iysxpp, 0]) / 2, (r[ixxm, iysx, 1] + r[ixxm, iysxp, 1]) / 2, sr[ix, kks, 1], sr[ix, kks + 1, 1], (r[ixxm, iysxpi, 1] + r[ixxm, iysxpp, 1]) / 2, (r[ixxm, iysx, 2] + r[ixxm, iysxp, 2]) / 2, sr[ix, kks, 2], sr[ix, kks + 1, 2], (r[ixxm, iysxpi, 2] + r[ixxm, iysxpp, 2]) / 2);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1];
                                r[i, s, 2] = b3[2];
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                double sdist = dist + ris[i, s];
                                double risdist = ris[i, s] / sdist;
                                double disdist = dist / sdist;
                                ri[i, s, 0] = r[i, s, 0] * disdist + ri[i, s, 0] * risdist;
                                ri[i, s, 1] = r[i, s, 1] * disdist + ri[i, s, 1] * risdist;
                                ri[i, s, 2] = r[i, s, 2] * disdist + ri[i, s, 2] * risdist;
                                ris[i, s] = sdist;
                            }
                        });
                    }

                    for (int ix = kki - 1; ix > kki - 1 - ki2 && ix > -1; ix--)
                    {
                        int dixx = ix * x;
                        int dixxp = dixx + x;
                        double dixxm = dixx + x50p;

                        Parallel.For(dixx, dixxp, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                double[] b3 = Bilinear3(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, kks, 0], (r[dixxp, iysx, 0] + r[dixxp, iysxp, 0]) / 2, (r[dixxp, iysxpi, 0] + r[dixxp, iysxpp, 0]) / 2, sr[ix, kks + 1, 1], sr[ix, kks, 1], (r[dixxp, iysx, 1] + r[dixxp, iysxp, 1]) / 2, (r[dixxp, iysxpi, 1] + r[dixxp, iysxpp, 1]) / 2, sr[ix, kks + 1, 1], sr[ix, kks, 2], (r[dixxp, iysx, 2] + r[dixxp, iysxp, 2]) / 2, (r[dixxp, iysxpi, 2] + r[dixxp, iysxpp, 2]) / 2, sr[ix, kks + 1, 2]);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1];
                                r[i, s, 2] = b3[2];
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                double sdist = dist + ris[i, s];
                                double risdist = ris[i, s] / sdist;
                                double disdist = dist / sdist;
                                ri[i, s, 0] = r[i, s, 0] * disdist + ri[i, s, 0] * risdist;
                                ri[i, s, 1] = r[i, s, 1] * disdist + ri[i, s, 1] * risdist;
                                ri[i, s, 2] = r[i, s, 2] * disdist + ri[i, s, 2] * risdist;
                                ris[i, s] = sdist;
                            }
                        });
                    }

                    int xs2x = (kki + 2) * x;
                    for (int sx = kks + 2; sx < kks + 2 + ks2 && sx < os; sx++)
                    {
                        int dsxxm = sx * x;
                        sxxm = (int)dsxxm - 1;
                        int dsxx = (sx + 1) * x;
                        double dsxxp = dsxxm + x50p;

                        Parallel.For(ixsx, xs2x, i =>
                        {
                            for (int s = dsxxm; s < dsxx; s++)
                            {
                                double[] b3 = Bilinear3(i, s, xsx, sxxm, xsxp, dsxxp, (r[ixsx, sxxm, 0] + r[ixsxp, sxxm, 0]) / 2, (r[ixsxpi, sxxm, 0] + r[ixsxpp, sxxm, 0]) / 2, sr[kki + 1, sx, 0], sr[kki, sx, 0], (r[ixsx, sxxm, 1] + r[ixsxp, sxxm, 1]) / 2, (r[ixsxpi, sxxm, 1] + r[ixsxpp, sxxm, 1]) / 2, sr[kki + 1, sx, 1], sr[kki, sx, 1], (r[ixsx, sxxm, 2] + r[ixsxp, sxxm, 2]) / 2, (r[ixsxpi, sxxm, 2] + r[ixsxpp, sxxm, 2]) / 2, sr[kki + 1, sx, 2], sr[kki, sx, 2]);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1];
                                r[i, s, 2] = b3[2];
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                double sdist = dist + ris[i, s];
                                double risdist = ris[i, s] / sdist;
                                double disdist = dist / sdist;
                                ri[i, s, 0] = r[i, s, 0] * disdist + ri[i, s, 0] * risdist;
                                ri[i, s, 1] = r[i, s, 1] * disdist + ri[i, s, 1] * risdist;
                                ri[i, s, 2] = r[i, s, 2] * disdist + ri[i, s, 2] * risdist;
                                ris[i, s] = sdist;
                            }
                        });
                    }

                    for (int sx = kks - 1; sx > kks - 1 - ks2 && sx > -1; sx--)
                    {
                        int dsxx = sx * x;
                        sxxm = dsxx + x;
                        double dsxxm = dsxx + x50p;

                        Parallel.For(ixsx, xs2x, i =>
                        {
                            for (int s = dsxx; s < sxxm; s++)
                            {
                                double[] b3 = Bilinear3(i, s, xsx, dsxxm, xsxp, sxxm, sr[kki, sx, 0], sr[xsp, sx, 0], (r[ixsxpi, sxxm, 0] + r[ixsxpp, sxxm, 0]) / 2, (r[ixsx, sxxm, 0] + r[ixsxp, sxxm, 0]) / 2, sr[kki, sx, 1], sr[xsp, sx, 1], (r[ixsxpi, sxxm, 1] + r[ixsxpp, sxxm, 1]) / 2, (r[ixsx, sxxm, 1] + r[ixsxp, sxxm, 1]) / 2, sr[kki, sx, 2], sr[xsp, sx, 2], (r[ixsxpi, sxxm, 2] + r[ixsxpp, sxxm, 2]) / 2, (r[ixsx, sxxm, 2] + r[ixsxp, sxxm, 2]) / 2);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1];
                                r[i, s, 2] = b3[2];
                                double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                double sdist = dist + ris[i, s];
                                double risdist = ris[i, s] / sdist;
                                double disdist = dist / sdist;
                                ri[i, s, 0] = r[i, s, 0] * disdist + ri[i, s, 0] * risdist;
                                ri[i, s, 1] = r[i, s, 1] * disdist + ri[i, s, 1] * risdist;
                                ri[i, s, 2] = r[i, s, 2] * disdist + ri[i, s, 2] * risdist;
                                ris[i, s] = sdist;
                            }
                        });
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

                            Parallel.For(dixxm, dixx, i =>
                            {
                                for (int s = dsxxm; s < dsxx; s++)
                                {
                                    double[] b3 = Bilinear3(i, s, ixxm, sxxm, dixxp, dsxxp, r[ixxm, sxxm, 0], (r[dixxm, sxxm, 0] + r[iixx, sxxm, 0]) / 2, sr[ix, sx, 0], (r[ixxm, dsxxm, 0] + r[ixxm, isxx, 0]) / 2, r[ixxm, sxxm, 1], (r[dixxm, sxxm, 1] + r[iixx, sxxm, 1]) / 2, sr[ix, sx, 1], (r[ixxm, dsxxm, 1] + r[ixxm, isxx, 1]) / 2, r[ixxm, sxxm, 2], (r[dixxm, sxxm, 2] + r[iixx, sxxm, 2]) / 2, sr[ix, sx, 2], (r[ixxm, dsxxm, 2] + r[ixxm, isxx, 2]) / 2);
                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    double sdist = dist + ris[i, s];
                                    double risdist = ris[i, s] / sdist;
                                    double disdist = dist / sdist;
                                    ri[i, s, 0] = r[i, s, 0] * disdist + ri[i, s, 0] * risdist;
                                    ri[i, s, 1] = r[i, s, 1] * disdist + ri[i, s, 1] * risdist;
                                    ri[i, s, 2] = r[i, s, 2] * disdist + ri[i, s, 2] * risdist;
                                    ris[i, s] = sdist;
                                }
                            });
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

                            Parallel.For(dixxm, ixxm, i =>
                            {
                                for (int s = dsxxm; s < sxxm; s++)
                                {
                                    double[] b3 = Bilinear3(i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx, 0], (r[ixxm, dsxxm, 0] + r[ixxm, isxx, 0]) / 2, r[ixxm, sxxm, 0], (r[dixxm, sxxm, 0] + r[iixx, sxxm, 0]) / 2, sr[ix, sx, 1], (r[ixxm, dsxxm, 1] + r[ixxm, isxx, 1]) / 2, r[ixxm, sxxm, 1], (r[dixxm, sxxm, 1] + r[iixx, sxxm, 1]) / 2, sr[ix, sx, 2], (r[ixxm, dsxxm, 2] + r[ixxm, isxx, 2]) / 2, r[ixxm, sxxm, 2], (r[dixxm, sxxm, 2] + r[iixx, sxxm, 2]) / 2);
                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    double sdist = dist + ris[i, s];
                                    double risdist = ris[i, s] / sdist;
                                    double disdist = dist / sdist;
                                    ri[i, s, 0] = r[i, s, 0] * disdist + ri[i, s, 0] * risdist;
                                    ri[i, s, 1] = r[i, s, 1] * disdist + ri[i, s, 1] * risdist;
                                    ri[i, s, 2] = r[i, s, 2] * disdist + ri[i, s, 2] * risdist;
                                    ris[i, s] = sdist;
                                }
                            });
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

                            Parallel.For(dixxm, dixx, i =>
                            {
                                for (int s = sxx; s < dsxx; s++)
                                {
                                    double[] b3 = Bilinear3(i, s, dixxp, isxxm, dixx, dsxxp, (r[dixxm, isxxm, 0] + r[iixx, isxxm, 0]) / 2, r[dixx, isxxm, 0], (r[dixx, isxx, 0] + r[dixx, sxx, 0]) / 2, sr[ix, sx, 0], (r[dixxm, isxxm, 1] + r[iixx, isxxm, 1]) / 2, r[dixx, isxxm, 1], (r[dixx, isxx, 1] + r[dixx, sxx, 1]) / 2, sr[ix, sx, 1], (r[dixxm, isxxm, 2] + r[iixx, isxxm, 2]) / 2, r[dixx, isxxm, 2], (r[dixx, isxx, 2] + r[dixx, sxx, 2]) / 2, sr[ix, sx, 2]);
                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    double sdist = dist + ris[i, s];
                                    double risdist = ris[i, s] / sdist;
                                    double disdist = dist / sdist;
                                    ri[i, s, 0] = r[i, s, 0] * disdist + ri[i, s, 0] * risdist;
                                    ri[i, s, 1] = r[i, s, 1] * disdist + ri[i, s, 1] * risdist;
                                    ri[i, s, 2] = r[i, s, 2] * disdist + ri[i, s, 2] * risdist;
                                    ris[i, s] = sdist;
                                }
                            });
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

                            Parallel.For(ixx, dixx, i =>
                            {
                                for (int s = sxx; s < dsxx; s++)
                                {
                                    double[] b3 = Bilinear3(i, s, ixxm, dsxxp, dixxp, dsxx, (r[ixxm, sxx, 0] + r[ixxm, isxx, 0]) / 2, sr[ix, sx, 0], (r[ixx, dsxx, 0] + r[iixx, dsxx, 0]) / 2, r[ixxm, dsxx, 0], (r[ixxm, sxx, 1] + r[ixxm, isxx, 1]) / 2, sr[ix, sx, 1], (r[ixx, dsxx, 1] + r[iixx, dsxx, 1]) / 2, r[ixxm, dsxx, 1], (r[ixxm, sxx, 2] + r[ixxm, isxx, 2]) / 2, sr[ix, sx, 2], (r[ixx, dsxx, 2] + r[iixx, dsxx, 2]) / 2, r[ixxm, dsxx, 2]);
                                    double dist = 1 / Math.Pow(Dist4(i, s, cx, cy, halfx), 2);
                                    double sdist = dist + ris[i, s];
                                    double risdist = ris[i, s] / sdist;
                                    double disdist = dist / sdist;

                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];

                                    ri[i, s, 0] = b3[0] * disdist + ri[i, s, 0] * risdist;
                                    ri[i, s, 1] = b3[1] * disdist + ri[i, s, 1] * risdist;
                                    ri[i, s, 2] = b3[2] * disdist + ri[i, s, 2] * risdist;
                                    ris[i, s] = sdist;
                                }
                            });
                        }
                    }
                }
            }

            byte[,,] rb = new byte[ni, ns, 3];

            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    for (int t = 0; t < 3; t++)
                    {
                        if (ri[i, s, t] < 0.5)
                            rb[i, s, t] = 0;
                        else if (ri[i, s, t] > 254.5)
                            rb[i, s, t] = 255;
                        else
                            rb[i, s, t] = (byte)(ri[i, s, t] + 0.5);
                    }
                }
            });

            return BMPfromRGB(rb, ni, ns);
        }


        static double Dist4(int i, int s, double cx, double cy, double x2)
        {
            if (i < cx)
            {
                if (s < cy)
                {
                    return Math.Max(Math.Abs(i - cx + x2) + Math.Abs(s - cy + x2), 0.0001d);
                }
                else
                {
                    return Math.Max(Math.Abs(i - cx + x2) + Math.Abs(s - cy - x2), 0.0001d);
                }
            }
            else
            {
                if (s < cy)
                {
                    return Math.Max(Math.Abs(i - cx - x2) + Math.Abs(s - cy + x2), 0.0001d);
                }
                else
                {
                    return Math.Max(Math.Abs(i - cx - x2) + Math.Abs(s - cy - x2), 0.0001d);
                }
            }
        }

        private void Progress(object sender, EventArgs e)
        {
            if (ProgressText.Text != pp)
            {
                pp = ProgressText.Text;
                TaskbarProgress.SetValue(this.Handle, Int32.Parse(ProgressText.Text), 100);
                if (ProgressText.Text == "100")
                {
                    ProgressText.Visible = false;
                    label4.Visible = false;
                }
                else
                {
                    ProgressText.Visible = true;
                    label4.Visible = true;
                }
                Application.DoEvents();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem)
            {
                case "scaleSmooth":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortSmooth);
                    label6.Text = "Most accurate for scenes where objects are completely in the image, but little bit blurred (much less than any interpolation) and grid structure is still visible\r\n\r\nFast, and you can process multiple images at the same time without losing speed";
                    break;
                case "scaleSmoothContinuous":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortSmoothCont);
                    label6.Text = "Most accurate for scenes where objects extend beyond the boundaries of the image, but little bit blurred (much less than any interpolation) and grid structure is still visible\r\n\r\nFast, and you can process multiple images at the same time without losing speed";
                    break;
                case "boldScale":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortBold);
                    label6.Text = "Little bit grid structure, noisy and contrasty (for accuracy, subsequent reverse correction is desirable) and too small details may lost\r\n\r\nFast - Slow, but you can process multiple images at the same time without losing speed";
                    break;
                case "contrastBoldScale":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortContrast);
                    label6.Text = "Perfect result, but too contrasty (for accuracy, subsequent reverse correction is required) and too small details are lost\r\n\r\nFast - Slow, but you can process multiple images at the same time without losing speed";
                    break;
                case "scaleFurry":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortFurry);
                    label6.Text = "Beautiful and detailed result, but only for monochrome images (only pure black and white, or for color - only pure red, black, green, yellow, fuchsia, blue, cyan and white)\r\n\r\nSlow - Very slow, but you can process multiple images at the same time without losing speed";
                    break;
                case "scaleRough":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortRough);
                    label6.Text = "Typographic raster stylization, but for monochrome images it gives acceptable result\r\n\r\nSlow - Very slow, but you can process multiple images at the same time without losing speed";
                    break;
                case "scaleSeparate":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortSeparate);
                    label6.Text = "Gives almost monochrome result and there are Gibbs ringing artifacts\r\n\r\nVery fast, but you can't process multiple images at the same time without losing speed";
                    break;
                case "scaleBilinearApproximation":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortBilApprox);
                    label6.Text = "A clearly defined grid structure and Gibbs ringing artifacts are present, but even if these shortcomings are not removed with other tools, it is more accurate than Lanczos and clearer than Lanczos and Bicubic\r\n\r\nVery fast - Very very slow, and you can't process multiple images at the same time without losing speed";
                    break;
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == comboBox1.Items.Count - 1)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                comboBox1.SelectedIndex++;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            }
            else
            {
                comboBox1.SelectedIndex--;
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
