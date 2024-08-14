using ILGPU;
using ILGPU.Runtime;
using ScaleSmooth.Properties;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ScaleSmooth
{
    public partial class Form1 : Form
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool PostMessage(IntPtr hWnd, uint Msg, uint WParam, uint LParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal extern static bool ReleaseCapture();

        string pp = "";
        string lastfile = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Raster image|*.png;*.jpg;*.bmp;*.gif;*.jpeg";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                lastfile = @openFileDialog.FileName;
                pictureBox1.Image = Image.FromFile(@openFileDialog.FileName);
                this.Text = Path.GetFileNameWithoutExtension(@openFileDialog.FileName);
            }
            StopWatchLabel.Visible = false;
            label3.Text = pictureBox1.Image.Width + "x" + pictureBox1.Image.Height;
        } //after stage4 (YUVA/HSKBA experiment) -> R?G?B?A -> color - pictogram interface 

        private void button2_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new() { Filter = @"PNG|*.png" };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Settings.Default.method = comboBox1.SelectedIndex; //after optimization, translate to Russian after settings
            switch (checkBox1.CheckState)
            {
                case CheckState.Checked:
                    Settings.Default.gpu = 1;
                    break;
                case CheckState.Unchecked:
                    Settings.Default.gpu = 0;
                    break;
                default:
                    Settings.Default.gpu = 2;
                    break;
            }

            Settings.Default.scale = (byte)numericUpDown1.Value;
            if (radioButton2.Checked)
                Settings.Default.channels = 3;
            else
                Settings.Default.channels = 1;
            Settings.Default.accuracy = (byte)trackBar1.Value;
            Settings.Default.sound = checkBox2.Checked;
            Settings.Default.location = (Size)this.Location;
            Properties.Settings.Default.Save();
            Environment.Exit(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            toolTip1.Active = false;
            button2.Enabled = false;
            button5.Enabled = false;
            button3.Enabled = false;
            button1.Enabled = false;
            Stopwatch stopWatch = new();
            StopWatchLabel.Visible = false;
            stopWatch.Start();
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
                    case "scaleSmoothContrast":
                        pictureBox1.Image = ScaleSmoothContrastGray(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
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
                        switch (checkBox1.CheckState)
                        {
                            case CheckState.Checked:
                                pictureBox1.Image = ScaleBilinearApproximationGrayGPU(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                                break;
                            case CheckState.Unchecked:
                                pictureBox1.Image = ScaleBilinearApproximationGray(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                                break;
                            default:
                                pictureBox1.Image = ScaleBilinearApproximationGrayAuto(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                                break;
                        }
                        break;
                    case "neighborSubpixel":
                        for (int i = 0; i < 100; i++)
                            pictureBox1.Image = NeighborSubpixelGray(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
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
                    case "scaleSmoothContrast"://afterALL, shorts
                        pictureBox1.Image = ScaleSmoothContrastColor(pictureBox1.Image, (int)(numericUpDown1.Value), trackBar1.Value);
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
                        switch (checkBox1.CheckState)
                        {
                            case CheckState.Checked:
                                pictureBox1.Image = ScaleBilinearApproximationColorGPU(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                                break;
                            case CheckState.Unchecked:
                                pictureBox1.Image = ScaleBilinearApproximationColor(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                                break;
                            default:
                                pictureBox1.Image = ScaleBilinearApproximationColorAuto(pictureBox1.Image, (int)numericUpDown1.Value, trackBar1.Value);
                                break;
                        }
                        break;
                }
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            StopWatchLabel.Text = String.Format("{0:0.000} seconds spent", ts.TotalSeconds);
            StopWatchLabel.Visible = true;
            ProgressText.Text = "100";
            pictureBox1.Refresh();
            button3.Enabled = true;
            button5.Enabled = true;
            button2.Enabled = true;
            button1.Enabled = true;
            Application.DoEvents();
            if (checkBox2.Checked)
            {
                if (ts.TotalSeconds > 3)
                {
                    if (ts.TotalSeconds < 1600)
                    {
                        Console.Beep(500, 400);
                    }
                    else
                    {
                        Console.Beep(600, (int)Math.Sqrt(ts.TotalSeconds) * 10);
                    }
                }
                else
                {
                    Console.Beep(300, (int)Math.Ceiling(ts.TotalSeconds * 50));
                }
            }
            label3.Text = pictureBox1.Image.Width + "x" + pictureBox1.Image.Height;
            toolTip1.Active = true;
        }

        private Bitmap ScaleBilinearApproximationColorAuto(Image img, int x, int ac)
        {
            Context context = Context.Create(b => b.AllAccelerators());
            Accelerator accelerator = context.GetPreferredDevice(false).CreateAccelerator(context);
            if (accelerator.AcceleratorType == AcceleratorType.CPU)
            {
                accelerator.Dispose();
                context.Dispose();
                return ScaleBilinearApproximationColor(img, x, ac);
            }
            else
            {
                accelerator.Dispose();
                context.Dispose();
                if (ac < 22)
                {
                    return ScaleBilinearApproximationColor(img, x, ac);
                }
                else
                {
                    return ScaleBilinearApproximationColorGPU(img, x, ac);
                }
            }
        }

        private Bitmap ScaleBilinearApproximationGrayAuto(Image img, int x, int ac)
        {
            Context context = Context.Create(b => b.AllAccelerators());
            Accelerator accelerator = context.GetPreferredDevice(false).CreateAccelerator(context);
            if (accelerator.AcceleratorType == AcceleratorType.CPU)
            {
                accelerator.Dispose();
                context.Dispose();
                return ScaleBilinearApproximationGray(img, x, ac);
            }
            else
            {
                accelerator.Dispose();
                context.Dispose();
                if (img.Height < 25)
                {
                    return ScaleBilinearApproximationGray(img, x, ac);
                }
                else
                {
                    return ScaleBilinearApproximationGrayGPU(img, x, ac);
                }
            }
        }

        private Bitmap ScaleBilinearApproximationGrayGPU(Image img, int x, int ac) //After ALL, update demo
        {
            Context context = Context.Create(b => b.AllAccelerators().EnableAlgorithms().Optimize(OptimizationLevel.O2).PageLocking(PageLockingMode.Aggressive));
#if DEBUG
            Accelerator accelerator = context.GetPreferredDevice(true).CreateAccelerator(context);
            ulong mem = 4 * (ulong)MathF.Pow(2, 30);//4GB VRAM for debug
#else
            Accelerator accelerator = context.GetPreferredDevice(false).CreateAccelerator(context);
            ulong mem = (ulong)accelerator.Device.MemorySize;
            if (accelerator.AcceleratorType == AcceleratorType.CPU)
            {
                label6.Text = "Better video card required! (Recommended NVIDIA GeForce 410+)";
                return new Bitmap(1, 1);
            }
#endif
            if (ac < 1) ac = 1;
            int ni, ns, oi, os, oim, osm;
            int xx4 = 4 * x * x;
            int xx2 = xx4 / 2;
            int xx = xx2 / 2;

            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;

            short[,,] ri = new short[ni, ns, 1];
            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);

            int ki = (int)MathF.Ceiling(oim * ac / 100f);
            int ks = (int)MathF.Ceiling(osm * ac / 100f);
            int kiks = ki * ks + 1;
            ulong nins = (ulong)(ni * ns * 6);
            int rki = 1;

            if (nins * (ulong)kiks > mem)
            {
                ac = (int)(MathF.Sqrt((mem / (float)nins - 1) / (oim * osm / 10000f)));
                if (ac < 1)
                {
                    label6.Text = "Better video card required! (More VRAM required for this size)";
                    return new Bitmap(1, 1);
                }
                rki = ki;
                ki = (int)MathF.Ceiling(oim * ac / 100f);
                ks = (int)MathF.Ceiling(osm * ac / 100f);
                rki = (int)(rki / (float)ki + 0.5f);
                kiks = ki * ks + 1;

            }
            int oiki = (int)(oim / (float)ki + 0.5f);
            int ski = (oi - rki) / 2 / ki;
            int sks = (os - rki) / 2 / ks;

            var riG = accelerator.Allocate3DDenseXY<short>(new Index3D(ni, ns, kiks));
            var risG = accelerator.Allocate3DDenseXY<float>(new Index3D(ni, ns, kiks));
            var srG = accelerator.Allocate2DDenseX<byte>(sr);
            float halfx = x / 2f;
            float x2p = halfx + 0.5f;
            float sqeq = 1 / (1f + 2 * x + xx);

            var center = accelerator.LoadStreamKernel(
                (ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView2D<byte, Stride2D.DenseX> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                    if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[16])
                    {
                        int iysx = kks * ints[0];
                        int ixsx = kki * ints[0];
                        int xsp = kki + 1, ysp = kks + 1;

                        float xsx = kki * ints[0] + dbls[0];
                        float ysx = kks * ints[0] + dbls[0];

                        int i = ixsx + Grid.GlobalIndex.X / ints[6];
                        int s = iysx + (Grid.GlobalIndex.X % ints[6]);
                        ri[i, s, kkikks] = Bilinear(i, s, xsx, ysx, xsx + ints[0], ysx + ints[0], sr[kki, kks], sr[xsp, kks], sr[xsp, ysp], sr[kki, ysp]);
                        ris[i, s, kkikks] = MathF.Pow(Dist4(i, s, ixsx + dbls[1], iysx + dbls[1], dbls[4]), -4);
                    }
                });

            var right = accelerator.LoadKernel(
                (ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView2D<byte, Stride2D.DenseX> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    if (kki < ints[2] && kks < ints[14] && Grid.GlobalIndex.X < ints[10])
                    {
                        int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                        float ysx = kks * ints[0] + dbls[0];
                        int iysx = kks * ints[0];
                        int iysxp = iysx + ints[1];
                        int iysxpi = iysxp + 1;
                        int iysxpp = iysxpi + ints[1];
                        float cx = kki * ints[0] + dbls[1];

                        for (int ix = kki + 2; ix < ints[2]; ix++)
                        {
                            int dixxm = ix * ints[0];
                            int ixxm = dixxm - 1;

                            int i = dixxm + Grid.GlobalIndex.X / ints[6];
                            int s = iysx + Grid.GlobalIndex.X % ints[6];
                            float r12, r21;
                            float q1 = (ri[ixxm, iysx, kkikks] + ri[ixxm, iysxp, kkikks]) / 2;
                            float q4 = (ri[ixxm, iysxpi, kkikks] + ri[ixxm, iysxpp, kkikks]) / 2;
                            r21 = dixxm + dbls[0] - i;
                            r12 = i - ixxm;
                            ri[i, s, kkikks] = (short)(((ysx + ints[0] - s) * (r21 * q1 + r12 * sr[ix, kks]) + (s - ysx) * (r21 * q4 + r12 * sr[ix, kks + 1])) / dbls[2] + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(Dist2Right(i, s, cx, iysxp + 0.5f, dbls[4]), -4);

                            i = ixxm + ints[0];
                            s = iysx;
                            q1 = dbls[8] * sr[ix, kks] + dbls[7] * q1;
                            q4 = dbls[8] * sr[ix, kks + 1] + dbls[7] * q4;
                            float r12q4 = dbls[0] * q4;
                            float r12q1 = dbls[0] * q1;
                            ri[i, s, kkikks] = (short)(dbls[5] * q1 - r12q4 + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(i - cx - 0.5f, -4);

                            s = iysxp;
                            ri[i, s, kkikks] = (short)(dbls[6] * q1 + r12q4 + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];

                            s = iysxpi;
                            ri[i, s, kkikks] = (short)(dbls[6] * q4 + r12q1 + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];

                            s = iysxpp;
                            ri[i, s, kkikks] = (short)(dbls[5] * q4 - r12q1 + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];
                        }
                    }
                });

            var left = accelerator.LoadKernel(
                (ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView2D<byte, Stride2D.DenseX> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[10])
                    {
                        int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                        float ysx = kks * ints[0] + dbls[0];

                        int iysx = kks * ints[0];
                        int iysxp = iysx + ints[1];
                        int iysxpi = iysxp + 1;
                        int iysxpp = iysxpi + ints[1];

                        float cx = kki * ints[0] + dbls[1];

                        for (int ix = kki - 1; ix > -1; ix--)
                        {
                            int dixx = ix * ints[0];
                            int dixxp = dixx + ints[0];

                            int i = dixx + Grid.GlobalIndex.X / ints[6];
                            int s = iysx + Grid.GlobalIndex.X % ints[6];
                            float r12, r21;
                            float q2 = (ri[dixxp, iysx, kkikks] + ri[dixxp, iysxp, kkikks]) / 2;
                            float q3 = (ri[dixxp, iysxpi, kkikks] + ri[dixxp, iysxpp, kkikks]) / 2;
                            r21 = dixxp - i;
                            r12 = i - dixx - dbls[0];
                            ri[i, s, kkikks] = (short)(((ysx + ints[0] - s) * (r21 * sr[ix, kks] + r12 * q2) + (s - ysx) * (r21 * sr[ix, kks + 1] + r12 * q3)) / dbls[2] + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(Dist2Left(i, s, cx, iysxp + 0.5f, dbls[4]), -4);

                            i = dixx;
                            s = iysx;
                            q2 = dbls[8] * sr[ix, kks] + dbls[7] * q2;
                            q3 = dbls[8] * sr[ix, kks + 1] + dbls[7] * q3;
                            float r12q3 = dbls[0] * q3;
                            float r12q2 = dbls[0] * q2;
                            ri[i, s, kkikks] = (short)(dbls[5] * q2 - r12q3 + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(cx - i - 0.5f, -4);

                            s = iysxp;
                            ri[i, s, kkikks] = (short)(dbls[6] * q2 + r12q3 + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];

                            s = iysxpi;
                            ri[i, s, kkikks] = (short)(r12q2 + dbls[6] * q3 + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];

                            s = iysxpp;
                            ri[i, s, kkikks] = (short)(dbls[5] * q3 - r12q2 + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];
                        }
                    }
                });

            var bottom = accelerator.LoadKernel(
                (ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView2D<byte, Stride2D.DenseX> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    if (kki < ints[4] && kks < ints[3] && Grid.GlobalIndex.X < ints[10])
                    {
                        int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                        float xsx = kki * ints[0] + dbls[0];

                        int ixsx = kki * ints[0];
                        int ixsxp = ixsx + ints[1];
                        int ixsxpi = ixsxp + 1;
                        float cy = kks * ints[0] + dbls[1];
                        int ixsxpp = ixsxpi + ints[1];

                        for (int sx = kks + 2; sx < ints[3]; sx++)
                        {
                            int dsxxm = sx * ints[0];
                            int sxxm = dsxxm - 1;
                            float dsxxp = dsxxm + dbls[0];

                            int i = ixsx + Grid.GlobalIndex.X % ints[6];
                            int s = dsxxm + Grid.GlobalIndex.X / ints[6];
                            float rx12, rx21, ry12, ry21;
                            float q1 = (ri[ixsx, sxxm, kkikks] + ri[ixsxp, sxxm, kkikks]) / 2;
                            float q2 = (ri[ixsxpi, sxxm, kkikks] + ri[ixsxpp, sxxm, kkikks]) / 2;
                            ry21 = dsxxp - s;
                            ry12 = s - sxxm;
                            rx21 = xsx + ints[0] - i;
                            rx12 = i - xsx;
                            ri[i, s, kkikks] = (short)((ry21 * (rx21 * q1 + rx12 * q2) + ry12 * (rx21 * sr[kki, sx] + rx12 * sr[kki + 1, sx])) / dbls[2] + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(Dist2Bottom(i, s, ixsxp + 0.5f, cy, dbls[4]), -4);

                            s = sxxm + ints[0];
                            i = ixsx;
                            rx21 = dbls[5];
                            rx12 = dbls[0];
                            float rx12q2 = rx12 * q2;
                            float rx12srkki1 = rx12 * sr[kki + 1, sx];
                            ry21 = dbls[7];
                            ry12 = dbls[8];
                            ri[i, s, kkikks] = (short)(ry21 * (rx21 * q1 - rx12q2) + ry12 * (rx21 * sr[kki, sx] - rx12srkki1) + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(s - cy - 0.5f, -4);

                            i = ixsxp;
                            rx21 = dbls[6];
                            ri[i, s, kkikks] = (short)(ry21 * (rx21 * q1 + rx12q2) + ry12 * (rx21 * sr[kki, sx] + rx12srkki1) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];

                            i = ixsxpi;
                            ri[i, s, kkikks] = (short)(ry21 * (rx21 * q2 + rx12 * q1) + ry12 * (dbls[0] * sr[kki, sx] + rx21 * sr[kki + 1, sx]) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];

                            i = ixsxpp;
                            rx12 = dbls[5];
                            ri[i, s, kkikks] = (short)(ry21 * (rx12 * q2 - dbls[0] * q1) + ry12 * (rx12 * sr[kki + 1, sx] - dbls[0] * sr[kki, sx]) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];
                        }
                    }
                });

            var top = accelerator.LoadKernel(
                (ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView2D<byte, Stride2D.DenseX> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[10])
                    {
                        int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                        float xsx = kki * ints[0] + dbls[0];

                        int ixsx = kki * ints[0];
                        int ixsxp = ixsx + ints[1];
                        int ixsxpi = ixsxp + 1;
                        float cy = kks * ints[0] + dbls[1];
                        int ixsxpp = ixsxpi + ints[1];
                        int xsp = kki + 1;

                        for (int sx = kks - 1; sx > -1; sx--)
                        {
                            int dsxx = sx * ints[0];
                            int sxxm = dsxx + ints[0];
                            float dsxxm = dsxx + dbls[0];
                            int i = ixsx + Grid.GlobalIndex.X % ints[6];
                            int s = dsxx + Grid.GlobalIndex.X / ints[6];
                            float rx12, rx21, ry12, ry21;
                            float q4 = (ri[ixsx, sxxm, kkikks] + ri[ixsxp, sxxm, kkikks]) / 2;
                            float q3 = (ri[ixsxpi, sxxm, kkikks] + ri[ixsxpp, sxxm, kkikks]) / 2;
                            ry21 = (sxxm - s);
                            ry12 = (s - dsxxm); //After optimization, 1bit channel! 192thresold ||to2bits
                            rx21 = (xsx + ints[0] - i);
                            rx12 = (i - xsx);
                            ri[i, s, kkikks] = (short)((ry21 * (rx21 * sr[kki, sx] + rx12 * sr[xsp, sx]) + ry12 * (rx21 * q4 + rx12 * q3)) / dbls[2] + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(Dist2Top(i, s, ixsxp + 0.5f, cy, dbls[4]), -4);

                            s = dsxx;
                            i = ixsx;
                            ry21 = dbls[8];
                            ry12 = dbls[7];
                            rx21 = dbls[5];
                            rx12 = dbls[0];
                            float rx12srxsp = rx12 * sr[xsp, sx];
                            float rx12q3 = rx12 * q3;
                            ri[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx] - rx12srxsp) + ry12 * (rx21 * q4 - rx12q3) + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(cy - s - 0.5f, -4);

                            i = ixsxp;
                            rx21 = dbls[6];
                            ri[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx] + rx12srxsp) + ry12 * (rx21 * q4 + rx12q3) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];

                            i = ixsxpi;
                            ri[i, s, kkikks] = (short)(ry21 * (rx12 * sr[kki, sx] + rx21 * sr[xsp, sx]) + ry12 * (rx12 * q4 + rx21 * q3) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];

                            i = ixsxpp;
                            rx12 = dbls[5];
                            ri[i, s, kkikks] = (short)(ry21 * (rx12 * sr[xsp, sx] - dbls[0] * sr[kki, sx]) + ry12 * (rx12 * q3 - dbls[0] * q4) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];
                        }
                    }
                });

            var rightbottom = accelerator.LoadKernel(
        (ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView2D<byte, Stride2D.DenseX> sr) =>
        {
            int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
            int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
            if (kki < ints[2] && kks < ints[3] && Grid.GlobalIndex.X < ints[11])
            {
                int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;

                float cxcyx = (kki + kks + 3) * ints[0] - 1;
                for (int sx = kks + 2; sx < ints[3]; sx++)
                {
                    for (int ix = kki + 2; ix < ints[2]; ix++)
                    {
                        int dsxxm = sx * ints[0];
                        int sxxm = dsxxm - 1;
                        int dsxx = (sx + 1) * ints[0];
                        int isxx = dsxx - 1;
                        int dixxm = ix * ints[0];
                        int ixxm = dixxm - 1;
                        int iixx = (ix + 1) * ints[0] - 1;

                        int i = dixxm + Grid.GlobalIndex.X % ints[0];
                        int s = dsxxm + Grid.GlobalIndex.X / ints[0];
                        float rx12, rx21;
                        float q2 = (ri[dixxm, sxxm, kkikks] + ri[iixx, sxxm, kkikks]) / 2;
                        float q4 = (ri[ixxm, dsxxm, kkikks] + ri[ixxm, isxx, kkikks]) / 2;
                        rx21 = dixxm + dbls[0] - i;
                        rx12 = i - ixxm;
                        ri[i, s, kkikks] = (short)(((dsxxm + dbls[0] - s) * (rx21 * ri[ixxm, sxxm, kkikks] + rx12 * q2) + (s - sxxm) * (rx21 * q4 + rx12 * sr[ix, sx])) / dbls[3] + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i - cxcyx + s, -4);

                        s = isxx;
                        i = dixxm;
                        float dbls0q4 = dbls[0] * q4;
                        float dbls0riixxm = dbls[0] * ri[ixxm, sxxm, kkikks];
                        ri[i, s, kkikks] = (short)(dbls[9] * (dbls0riixxm + q2) + dbls[10] * (dbls0q4 + sr[ix, sx]) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i - cxcyx + s, -4);

                        i = iixx;
                        q2 = dbls[9] * (ints[0] * q2 - dbls0riixxm);
                        q4 = (ints[0] * sr[ix, sx] - dbls0q4);
                        ri[i, s, kkikks] = (short)(q2 + dbls[10] * q4 + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i - cxcyx + s, -4);

                        s = dsxxm;
                        ri[i, s, kkikks] = (short)(q4 * dbls[11] - q2 + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i - cxcyx + s, -4);
                    }
                }
            }
        });

            var lefttop = accelerator.LoadKernel(
        (ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView2D<byte, Stride2D.DenseX> sr) =>
        {
            int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
            int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
            int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
            if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[11] && kkikks < ints[5])
            {
                float cxcyx = (kki + kks + 1) * ints[0] - 1;
                for (int sx = kks - 1; sx > -1; sx--)
                {
                    for (int ix = kki - 1; ix > -1; ix--)
                    {
                        int dsxxm = sx * ints[0];
                        int sxxm = (sx + 1) * ints[0];
                        int isxx = sxxm - 1;
                        int dixxm = ix * ints[0];
                        int ixxm = (ix + 1) * ints[0];
                        int iixx = ixxm - 1;

                        int i = dixxm + Grid.GlobalIndex.X % ints[0];
                        int s = dsxxm + Grid.GlobalIndex.X / ints[0];
                        float rx12, rx21, ry12, ry21;
                        float q2 = (ri[ixxm, dsxxm, kkikks] + ri[ixxm, isxx, kkikks]) / 2;
                        float q4 = (ri[dixxm, sxxm, kkikks] + ri[iixx, sxxm, kkikks]) / 2;
                        ry21 = sxxm - s;
                        ry12 = s - dsxxm - dbls[0];
                        rx21 = ixxm - i;
                        rx12 = i - dixxm - dbls[0];
                        ri[i, s, kkikks] = (short)((ry21 * (rx21 * sr[ix, sx] + rx12 * q2) + ry12 * (rx21 * q4 + rx12 * ri[ixxm, sxxm, kkikks])) / dbls[3] + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i - s, -4);

                        i = dixxm;
                        s = isxx;
                        ry12 = dbls[0];
                        rx21 = ints[0];
                        rx12 = -ry12 * ri[ixxm, sxxm, kkikks];
                        q2 = -ry12 * q2;
                        float rx21srq2 = rx21 * sr[ix, sx] + q2;
                        float rx21q4rx12 = rx21 * q4 + rx12;
                        ri[i, s, kkikks] = (short)((rx21srq2 + ry12 * rx21q4rx12) * dbls[11] + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i - s, -4);

                        s = dsxxm;
                        ry21 = dbls[10];
                        ry12 = dbls[9];
                        ri[i, s, kkikks] = (short)(ry21 * rx21srq2 + ry12 * rx21q4rx12 + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i - s, -4);

                        i = iixx;
                        ri[i, s, kkikks] = (short)(ry21 * (sr[ix, sx] - q2) + ry12 * (q4 - rx12) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i - s, -4);
                    }
                }
            }
        });

            var leftbottom = accelerator.LoadKernel(
        (ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView2D<byte, Stride2D.DenseX> sr) =>
        {
            int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
            int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
            if (kki < ints[2] && kks < ints[3] && Grid.GlobalIndex.X < ints[11])
            {
                int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;

                float cxcyx = (kki - kks - 1) * ints[0];
                for (int sx = kks + 2; sx < ints[3]; sx++)
                {
                    for (int ix = kki - 1; ix > -1; ix--)
                    {
                        int sxx = sx * ints[0];
                        int isxxm = sxx - 1;
                        int isxx = (sx + 1) * ints[0] - 1;
                        int dixxm = ix * ints[0];
                        int dixx = dixxm + ints[0];
                        int iixx = dixx - 1;

                        int i = dixxm + Grid.GlobalIndex.X % ints[0];
                        int s = sxx + Grid.GlobalIndex.X / ints[0];
                        float rx12, rx21, ry12, ry21;
                        float q1 = (ri[dixxm, isxxm, kkikks] + ri[iixx, isxxm, kkikks]) / 2;
                        float q3 = (ri[dixx, isxx, kkikks] + ri[dixx, sxx, kkikks]) / 2;
                        ry21 = sxx + dbls[0] - s;
                        ry12 = s - isxxm;
                        rx21 = dixx - i;
                        rx12 = i - dixxm - dbls[0];
                        ri[i, s, kkikks] = (short)((ry21 * (rx21 * q1 + rx12 * ri[dixx, isxxm, kkikks]) + ry12 * (rx21 * sr[ix, sx] + rx12 * q3)) / dbls[3] + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i + s, -4);

                        i = dixxm;
                        s = sxx;
                        ry21 = dbls[9];
                        ry12 = dbls[11];
                        rx12 = -dbls[0];
                        q3 = rx12 * q3;
                        float rxri = rx12 * ri[dixx, isxxm, kkikks];
                        float q1rx = ints[0] * q1 + rxri;
                        float xsr = ints[0] * sr[ix, sx];
                        ri[i, s, kkikks] = (short)(ry12 * (xsr + q3) - ry21 * q1rx + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i + s, -4);

                        s = isxx;
                        ry12 = dbls[10];
                        ri[i, s, kkikks] = (short)(ry21 * q1rx + ry12 * (xsr + q3) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i + s, -4);

                        i = iixx;
                        ri[i, s, kkikks] = (short)(ry21 * (q1 - rxri) + ry12 * (sr[ix, sx] - q3) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i + s, -4);
                    }
                }
            }
        });

            var righttop = accelerator.LoadKernel(
        (ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView2D<byte, Stride2D.DenseX> sr) =>
        {
            int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
            int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
            int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
            if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[11] && kkikks < ints[5])
            {
                float cxcyx = (kks - kki - 1) * ints[0];
                for (int ix = kki + 2; ix < ints[2]; ix++)
                {
                    for (int sx = kks - 1; sx > -1; sx--)
                    {
                        int sxx = sx * ints[0];
                        int dsxx = sxx + ints[0];
                        int isxx = dsxx - 1;
                        int ixx = ix * ints[0];
                        int ixxm = ixx - 1;
                        int iixx = ixx + ints[1];

                        int i = ixx + Grid.GlobalIndex.X % ints[0];
                        int s = sxx + Grid.GlobalIndex.X / ints[0];
                        float rx12, rx21, ry12, ry21;
                        float q1 = (ri[ixxm, sxx, kkikks] + ri[ixxm, isxx, kkikks]) / 2;
                        float q3 = (ri[ixx, dsxx, kkikks] + ri[iixx, dsxx, kkikks]) / 2;
                        ry21 = dsxx - s;
                        ry12 = s - sxx - dbls[0];
                        rx21 = ixx + dbls[0] - i;
                        rx12 = i - ixxm;
                        ri[i, s, kkikks] = (short)((ry21 * (rx21 * q1 + rx12 * sr[ix, sx]) + ry12 * (rx21 * ri[ixxm, dsxx, kkikks] + rx12 * q3)) / dbls[3] + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i + cxcyx - s, -4);

                        i = iixx;
                        s = sxx;
                        ry21 = dbls[10];
                        ry12 = dbls[9];
                        rx21 = dbls[0];
                        rx12 = ints[0];
                        q1 = rx21 * q1;
                        float rxsr = rx12 * sr[ix, sx] - q1;
                        float ryrx = ry12 * (rx12 * q3 - rx21 * ri[ixxm, dsxx, kkikks]);
                        ri[i, s, kkikks] = (short)(ry21 * rxsr + ryrx + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i + cxcyx - s, -4);

                        s = isxx;
                        ry21 = dbls[11];
                        ri[i, s, kkikks] = (short)(ry21 * rxsr - ryrx + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i + cxcyx - s, -4);

                        i = ixx;
                        ri[i, s, kkikks] = (short)(ry21 * (sr[ix, sx] + q1) - ry12 * (q3 + rx21 * ri[ixxm, dsxx, kkikks]) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i + cxcyx - s, -4);
                    }
                }
            }
        });

            int threadsPerGroup = accelerator.Device.WarpSize;

            var ttl = accelerator.LoadAutoGroupedStreamKernel(
        (Index2D ii, ArrayView3D<short, Stride3D.DenseXY> ri, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints) =>
        {
            ris[ii.X, ii.Y, 0] = 0;
            for (int kkikks = 1; kkikks < ints[5]; kkikks++)
            {
                ri[ii.X, ii.Y, 0] = (short)((ri[ii.X, ii.Y, kkikks] * ris[ii.X, ii.Y, kkikks] + ri[ii.X, ii.Y, 0] * ris[ii.X, ii.Y, 0]) / (ris[ii.X, ii.Y, kkikks] + ris[ii.X, ii.Y, 0]) + 0.5);
                ris[ii.X, ii.Y, 0] = ris[ii.X, ii.Y, kkikks] + ris[ii.X, ii.Y, 0];
            }
        });

            float progress = 0;
            float rki2 = rki * rki;
            float step = 0.98f / rki2;
            float step8 = 7.84f / rki2;
            float step89 = 87.22f / rki2;
            var dblG = accelerator.Allocate1D<float>([halfx - 0.5f, x - 0.5f, x2p * x, x2p * x2p, halfx, x * 1.5f - 0.5f, x2p, (1f - x) / (x + xx), 2f / (1 + x), (2f - 2 * x) * sqeq, 4f * x * sqeq, 4f * sqeq]);
            var intG = accelerator.Allocate1D<int>(17);

            for (int irk = 0; irk < rki; irk++)
            {
                for (int srk = 0; srk < rki; srk++)
                {
                    intG = accelerator.Allocate1D<int>([x, x - 1, oi, os, oim, kiks, 2 * x, ki, oim / 2 - ki / 2, osm / 2 - ks / 2, xx2, xx, oiki, irk, osm, srk, xx4, ski, sks]);
                    Vector3 grp = FitToVolumeByX(xx4, accelerator.Device.MaxGroupSize, threadsPerGroup);
                    KernelConfig kc = new(new Index3D((int)MathF.Ceiling(xx4 / grp.X), (int)MathF.Ceiling(ki / grp.Y), (int)MathF.Ceiling(ks / grp.Z)), new((int)grp.X, (int)grp.Y, (int)grp.Z));

                    center(kc, riG.View, risG.View, intG.View, dblG.View, srG.View);

                    grp = FitToVolumeByX(xx2, accelerator.Device.MaxGroupSize, threadsPerGroup);
                    kc = new(new Index3D((int)MathF.Ceiling(xx2 / grp.X), (int)MathF.Ceiling(ki / grp.Y), (int)MathF.Ceiling(ks / grp.Z)), new((int)grp.X, (int)grp.Y, (int)grp.Z));
                    progress += step;
                    ProgressText.Text = ((int)progress).ToString();
                    AcceleratorStream Stream1 = accelerator.CreateStream();
                    AcceleratorStream Stream2 = accelerator.CreateStream();
                    AcceleratorStream Stream3 = accelerator.CreateStream();
                    AcceleratorStream Stream4 = accelerator.CreateStream();
                    accelerator.Synchronize();

                    right(Stream1, kc, riG.View, risG.View, intG.View, dblG.View, srG.View);
                    left(Stream2, kc, riG.View, risG.View, intG.View, dblG.View, srG.View);
                    bottom(Stream3, kc, riG.View, risG.View, intG.View, dblG.View, srG.View);
                    top(Stream4, kc, riG.View, risG.View, intG.View, dblG.View, srG.View);
                    grp = FitToVolumeByX(xx, accelerator.Device.MaxGroupSize, threadsPerGroup);
                    kc = new(new Index3D((int)MathF.Ceiling(xx / grp.X), (int)MathF.Ceiling(ki / grp.Y), (int)MathF.Ceiling(ks / grp.Z)), new((int)grp.X, (int)grp.Y, (int)grp.Z));
                    progress += step;
                    ProgressText.Text = ((int)progress).ToString();
                    accelerator.Synchronize();

                    rightbottom(Stream1, kc, riG.View, risG.View, intG.View, dblG.View, srG.View);
                    lefttop(Stream2, kc, riG.View, risG.View, intG.View, dblG.View, srG.View);
                    leftbottom(Stream3, kc, riG.View, risG.View, intG.View, dblG.View, srG.View);
                    righttop(Stream4, kc, riG.View, risG.View, intG.View, dblG.View, srG.View);
                    progress += step8;
                    ProgressText.Text = ((int)progress).ToString();
                    accelerator.Synchronize();

                    ttl(new Index2D(ni, ns), riG.View, risG.View, intG.View);
                    progress += step89;
                    ProgressText.Text = ((int)progress).ToString();
                    accelerator.Synchronize();
                }
            }
            ProgressText.Text = "99";
            ri = riG.View.SubView(new Index3D(0, 0, 0), new Index3D(ni, ns, 1)).GetAsArray3D();
            accelerator.Dispose();
            context.Dispose();
            byte[,] rb = new byte[ni, ns];
            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    if (ri[i, s, 0] < 0.5)
                        rb[i, s] = 0; //AFTER ALL (and release) 0&&0=0  //contrast??? 0x1 - not gray //sobel? 
                    else if (ri[i, s, 0] > 254.5) //AFTER ALL, AverageBilinear(parabola)
                        rb[i, s] = 255;
                    else
                        rb[i, s] = (byte)(ri[i, s, 0] + 0.5);
                }
            });

            return BMPfromGray(rb, ni, ns);
        }


        private Bitmap ScaleBilinearApproximationColorGPU(Image img, int x, int ac)
        {
            Context context = Context.Create(b => b.AllAccelerators().EnableAlgorithms().Optimize(OptimizationLevel.O2).PageLocking(PageLockingMode.Aggressive));
#if DEBUG
            Accelerator accelerator = context.GetPreferredDevice(true).CreateAccelerator(context);
            ulong mem = 4 * (ulong)MathF.Pow(2, 30);//4GB VRAM for debug
#else
            Accelerator accelerator = context.GetPreferredDevice(false).CreateAccelerator(context);
            ulong mem = (ulong)accelerator.Device.MemorySize;
            if (accelerator.AcceleratorType == AcceleratorType.CPU)
            {
                label6.Text = "Better video card required! (Recommended NVIDIA GeForce 410+)";
                return new Bitmap(1, 1);
            }
#endif
            if (ac < 1) ac = 1;
            int ni, ns, oi, os, oim, osm;
            int xx4 = 4 * x * x;
            int xx2 = xx4 / 2;
            int xx = xx2 / 2;

            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;

            short[,,] rR = new short[ni, ns, 1];
            short[,,] rG = new short[ni, ns, 1];
            short[,,] rB = new short[ni, ns, 1];
            byte[,,] sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);

            int ki = (int)MathF.Ceiling(oim * ac / 100f);
            int ks = (int)MathF.Ceiling(osm * ac / 100f);
            int kiks = ki * ks + 1;
            ulong nins = (ulong)(ni * ns * 10);
            int rki = 1;
            if (nins * (ulong)kiks > mem)
            {
                ac = (int)(MathF.Sqrt((mem / (float)nins - 1) / (oim * osm / 10000f)));
                if (ac < 1)
                {
                    label6.Text = "Better video card required! (More VRAM required for this size)";
                    return new Bitmap(1, 1);
                }
                rki = ki;
                ki = (int)MathF.Ceiling(oim * ac / 100f);
                ks = (int)MathF.Ceiling(osm * ac / 100f);
                rki = (int)(rki / (float)ki + 0.5f);
                kiks = ki * ks + 1;

            }
            int oiki = (int)(oim / (float)ki + 0.5f);
            int ski = (oi - rki) / 2 / ki;
            int sks = (os - rki) / 2 / ks;

            var riRG = accelerator.Allocate3DDenseXY<short>(new Index3D(ni, ns, kiks));
            var riGG = accelerator.Allocate3DDenseXY<short>(new Index3D(ni, ns, kiks));
            var riBG = accelerator.Allocate3DDenseXY<short>(new Index3D(ni, ns, kiks));
            var risG = accelerator.Allocate3DDenseXY<float>(new Index3D(ni, ns, kiks));
            var srG = accelerator.Allocate3DDenseXY<byte>(sr);
            float halfx = x / 2f;
            float x2p = halfx + 0.5f;
            float sqeq = 1 / (1f + 2 * x + xx);


            var center = accelerator.LoadStreamKernel(
                (ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView3D<byte, Stride3D.DenseXY> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                    if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[16])
                    {
                        int iysx = kks * ints[0];
                        int ixsx = kki * ints[0];
                        int xsp = kki + 1, ysp = kks + 1;

                        float xsx = kki * ints[0] + dbls[0];
                        float ysx = kks * ints[0] + dbls[0];

                        int i = ixsx + Grid.GlobalIndex.X / ints[6];
                        int s = iysx + (Grid.GlobalIndex.X % ints[6]);
                        short[] r = Bilinear3(i, s, xsx, ysx, xsx + ints[0], ysx + ints[0], sr[kki, kks, 0], sr[xsp, kks, 0], sr[xsp, ysp, 0], sr[kki, ysp, 0], sr[kki, kks, 1], sr[xsp, kks, 1], sr[xsp, ysp, 1], sr[kki, ysp, 1], sr[kki, kks, 2], sr[xsp, kks, 2], sr[xsp, ysp, 2], sr[kki, ysp, 2]);
                        riR[i, s, kkikks] = r[0];
                        riG[i, s, kkikks] = r[1];
                        riB[i, s, kkikks] = r[2];
                        ris[i, s, kkikks] = MathF.Pow(Dist4(i, s, ixsx + dbls[1], iysx + dbls[1], dbls[4]), -4);
                    }
                });

            var right = accelerator.LoadKernel(
                (ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView3D<byte, Stride3D.DenseXY> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    if (kki < ints[2] && kks < ints[14] && Grid.GlobalIndex.X < ints[10])
                    {
                        int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                        float ysx = kks * ints[0] + dbls[0];
                        int iysx = kks * ints[0];
                        int iysxp = iysx + ints[1];
                        int iysxpi = iysxp + 1;
                        int iysxpp = iysxpi + ints[1];
                        float cx = kki * ints[0] + dbls[1];

                        for (int ix = kki + 2; ix < ints[2]; ix++)
                        {
                            int dixxm = ix * ints[0];
                            int ixxm = dixxm - 1;

                            int i = dixxm + Grid.GlobalIndex.X / ints[6];
                            int s = iysx + Grid.GlobalIndex.X % ints[6];
                            float r12, r21;
                            float q1R = (riR[ixxm, iysx, kkikks] + riR[ixxm, iysxp, kkikks]) / 2;
                            float q4R = (riR[ixxm, iysxpi, kkikks] + riR[ixxm, iysxpp, kkikks]) / 2;
                            float q1G = (riG[ixxm, iysx, kkikks] + riG[ixxm, iysxp, kkikks]) / 2;
                            float q4G = (riG[ixxm, iysxpi, kkikks] + riG[ixxm, iysxpp, kkikks]) / 2;
                            float q1B = (riB[ixxm, iysx, kkikks] + riB[ixxm, iysxp, kkikks]) / 2;
                            float q4B = (riB[ixxm, iysxpi, kkikks] + riB[ixxm, iysxpp, kkikks]) / 2;
                            r21 = dixxm + dbls[0] - i;
                            r12 = i - ixxm;
                            float ry21 = (ysx + ints[0] - s) / dbls[2];
                            float ry12 = (s - ysx) / dbls[2];
                            riR[i, s, kkikks] = (short)(ry21 * (r21 * q1R + r12 * sr[ix, kks, 0]) + ry12 * (r21 * q4R + r12 * sr[ix, kks + 1, 0]) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (r21 * q1G + r12 * sr[ix, kks, 1]) + ry12 * (r21 * q4G + r12 * sr[ix, kks + 1, 1]) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (r21 * q1B + r12 * sr[ix, kks, 2]) + ry12 * (r21 * q4B + r12 * sr[ix, kks + 1, 2]) + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(Dist2Right(i, s, cx, iysxp + 0.5f, dbls[4]), -4);

                            i = ixxm + ints[0];
                            s = iysx;
                            q1R = dbls[8] * sr[ix, kks, 0] + dbls[7] * q1R;
                            q4R = dbls[8] * sr[ix, kks + 1, 0] + dbls[7] * q4R;
                            q1G = dbls[8] * sr[ix, kks, 1] + dbls[7] * q1G;
                            q4G = dbls[8] * sr[ix, kks + 1, 1] + dbls[7] * q4G;
                            q1B = dbls[8] * sr[ix, kks, 2] + dbls[7] * q1B;
                            q4B = dbls[8] * sr[ix, kks + 1, 2] + dbls[7] * q4B;
                            float r12q4R = dbls[0] * q4R;
                            float r12q1R = dbls[0] * q1R;
                            float r12q4G = dbls[0] * q4G;
                            float r12q1G = dbls[0] * q1G;
                            float r12q4B = dbls[0] * q4B;
                            float r12q1B = dbls[0] * q1B;
                            riR[i, s, kkikks] = (short)(dbls[5] * q1R - r12q4R + 0.5);
                            riG[i, s, kkikks] = (short)(dbls[5] * q1G - r12q4G + 0.5);
                            riB[i, s, kkikks] = (short)(dbls[5] * q1B - r12q4B + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(i - cx - 0.5f, -4);

                            s = iysxp;
                            riR[i, s, kkikks] = (short)(dbls[6] * q1R + r12q4R + 0.5);
                            riG[i, s, kkikks] = (short)(dbls[6] * q1G + r12q4G + 0.5);
                            riB[i, s, kkikks] = (short)(dbls[6] * q1B + r12q4B + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];

                            s = iysxpi;
                            riR[i, s, kkikks] = (short)(dbls[6] * q4R + r12q1R + 0.5);
                            riG[i, s, kkikks] = (short)(dbls[6] * q4G + r12q1G + 0.5);
                            riB[i, s, kkikks] = (short)(dbls[6] * q4B + r12q1B + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];

                            s = iysxpp;
                            riR[i, s, kkikks] = (short)(dbls[5] * q4R - r12q1R + 0.5);
                            riG[i, s, kkikks] = (short)(dbls[5] * q4G - r12q1G + 0.5);
                            riB[i, s, kkikks] = (short)(dbls[5] * q4B - r12q1B + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];
                        }
                    }
                });

            var left = accelerator.LoadKernel(
                (ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView3D<byte, Stride3D.DenseXY> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[10])
                    {
                        int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                        float ysx = kks * ints[0] + dbls[0];

                        int iysx = kks * ints[0];
                        int iysxp = iysx + ints[1];
                        int iysxpi = iysxp + 1;
                        int iysxpp = iysxpi + ints[1];

                        float cx = kki * ints[0] + dbls[1];

                        for (int ix = kki - 1; ix > -1; ix--)
                        {
                            int dixx = ix * ints[0];
                            int dixxp = dixx + ints[0];

                            int i = dixx + Grid.GlobalIndex.X / ints[6];
                            int s = iysx + Grid.GlobalIndex.X % ints[6];
                            float r12, r21;
                            float q2R = (riR[dixxp, iysx, kkikks] + riR[dixxp, iysxp, kkikks]) / 2;
                            float q3R = (riR[dixxp, iysxpi, kkikks] + riR[dixxp, iysxpp, kkikks]) / 2;
                            float q2G = (riG[dixxp, iysx, kkikks] + riG[dixxp, iysxp, kkikks]) / 2;
                            float q3G = (riG[dixxp, iysxpi, kkikks] + riG[dixxp, iysxpp, kkikks]) / 2;
                            float q2B = (riB[dixxp, iysx, kkikks] + riB[dixxp, iysxp, kkikks]) / 2;
                            float q3B = (riB[dixxp, iysxpi, kkikks] + riB[dixxp, iysxpp, kkikks]) / 2;
                            r21 = dixxp - i;
                            r12 = i - dixx - dbls[0];
                            float ry21 = (ysx + ints[0] - s) / dbls[2];
                            float ry12 = (s - ysx) / dbls[2];
                            riR[i, s, kkikks] = (short)(ry21 * (r21 * sr[ix, kks, 0] + r12 * q2R) + ry12 * (r21 * sr[ix, kks + 1, 0] + r12 * q3R) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (r21 * sr[ix, kks, 1] + r12 * q2G) + ry12 * (r21 * sr[ix, kks + 1, 1] + r12 * q3G) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (r21 * sr[ix, kks, 2] + r12 * q2B) + ry12 * (r21 * sr[ix, kks + 1, 2] + r12 * q3B) + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(Dist2Left(i, s, cx, iysxp + 0.5f, dbls[4]), -4);

                            i = dixx;
                            s = iysx;
                            q2R = dbls[8] * sr[ix, kks, 0] + dbls[7] * q2R;
                            q3R = dbls[8] * sr[ix, kks + 1, 0] + dbls[7] * q3R;
                            q2G = dbls[8] * sr[ix, kks, 1] + dbls[7] * q2G;
                            q3G = dbls[8] * sr[ix, kks + 1, 1] + dbls[7] * q3G;
                            q2B = dbls[8] * sr[ix, kks, 2] + dbls[7] * q2B;
                            q3B = dbls[8] * sr[ix, kks + 1, 2] + dbls[7] * q3B;
                            float r12q3R = dbls[0] * q3R;
                            float r12q2R = dbls[0] * q2R;
                            float r12q3G = dbls[0] * q3G;
                            float r12q2G = dbls[0] * q2G;
                            float r12q3B = dbls[0] * q3B;
                            float r12q2B = dbls[0] * q2B;
                            riR[i, s, kkikks] = (short)(dbls[5] * q2R - r12q3R + 0.5);
                            riG[i, s, kkikks] = (short)(dbls[5] * q2G - r12q3G + 0.5);
                            riB[i, s, kkikks] = (short)(dbls[5] * q2B - r12q3B + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(cx - i - 0.5f, -4);

                            s = iysxp;
                            riR[i, s, kkikks] = (short)(dbls[6] * q2R + r12q3R + 0.5);
                            riG[i, s, kkikks] = (short)(dbls[6] * q2G + r12q3G + 0.5);
                            riB[i, s, kkikks] = (short)(dbls[6] * q2B + r12q3B + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];

                            s = iysxpi;
                            riR[i, s, kkikks] = (short)(r12q2R + dbls[6] * q3R + 0.5);
                            riG[i, s, kkikks] = (short)(r12q2G + dbls[6] * q3G + 0.5);
                            riB[i, s, kkikks] = (short)(r12q2B + dbls[6] * q3B + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];

                            s = iysxpp;
                            riR[i, s, kkikks] = (short)(dbls[5] * q3R - r12q2R + 0.5);
                            riG[i, s, kkikks] = (short)(dbls[5] * q3G - r12q2G + 0.5);
                            riB[i, s, kkikks] = (short)(dbls[5] * q3B - r12q2B + 0.5);
                            ris[i, s, kkikks] = ris[i, iysx, kkikks];
                        }
                    }
                });

            var bottom = accelerator.LoadKernel(
                (ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView3D<byte, Stride3D.DenseXY> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    if (kki < ints[4] && kks < ints[3] && Grid.GlobalIndex.X < ints[10])
                    {
                        int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                        float xsx = kki * ints[0] + dbls[0];

                        int ixsx = kki * ints[0];
                        int ixsxp = ixsx + ints[1];
                        int ixsxpi = ixsxp + 1;
                        float cy = kks * ints[0] + dbls[1];
                        int ixsxpp = ixsxpi + ints[1];

                        for (int sx = kks + 2; sx < ints[3]; sx++)
                        {
                            int dsxxm = sx * ints[0];
                            int sxxm = dsxxm - 1;
                            float dsxxp = dsxxm + dbls[0];

                            int i = ixsx + Grid.GlobalIndex.X % ints[6];
                            int s = dsxxm + Grid.GlobalIndex.X / ints[6];
                            float rx12, rx21, ry12, ry21;
                            float q1R = (riR[ixsx, sxxm, kkikks] + riR[ixsxp, sxxm, kkikks]) / 2;
                            float q2R = (riR[ixsxpi, sxxm, kkikks] + riR[ixsxpp, sxxm, kkikks]) / 2;
                            float q1G = (riG[ixsx, sxxm, kkikks] + riG[ixsxp, sxxm, kkikks]) / 2;
                            float q2G = (riG[ixsxpi, sxxm, kkikks] + riG[ixsxpp, sxxm, kkikks]) / 2;
                            float q1B = (riB[ixsx, sxxm, kkikks] + riB[ixsxp, sxxm, kkikks]) / 2;
                            float q2B = (riB[ixsxpi, sxxm, kkikks] + riB[ixsxpp, sxxm, kkikks]) / 2;
                            ry21 = (dsxxp - s) / dbls[2];
                            ry12 = (s - sxxm) / dbls[2];
                            rx21 = xsx + ints[0] - i;
                            rx12 = i - xsx;
                            riR[i, s, kkikks] = (short)(ry21 * (rx21 * q1R + rx12 * q2R) + ry12 * (rx21 * sr[kki, sx, 0] + rx12 * sr[kki + 1, sx, 0]) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx21 * q1G + rx12 * q2G) + ry12 * (rx21 * sr[kki, sx, 1] + rx12 * sr[kki + 1, sx, 1]) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx21 * q1B + rx12 * q2B) + ry12 * (rx21 * sr[kki, sx, 2] + rx12 * sr[kki + 1, sx, 2]) + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(Dist2Bottom(i, s, ixsxp + 0.5f, cy, dbls[4]), -4);

                            s = sxxm + ints[0];
                            i = ixsx;
                            rx21 = dbls[5];
                            rx12 = dbls[0];
                            float rx12q2R = rx12 * q2R;
                            float rx12srkki1R = rx12 * sr[kki + 1, sx, 0];
                            float rx12q2G = rx12 * q2G;
                            float rx12srkki1G = rx12 * sr[kki + 1, sx, 1];
                            float rx12q2B = rx12 * q2B;
                            float rx12srkki1B = rx12 * sr[kki + 1, sx, 2];
                            ry21 = dbls[7];
                            ry12 = dbls[8];
                            riR[i, s, kkikks] = (short)(ry21 * (rx21 * q1R - rx12q2R) + ry12 * (rx21 * sr[kki, sx, 0] - rx12srkki1R) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx21 * q1G - rx12q2G) + ry12 * (rx21 * sr[kki, sx, 1] - rx12srkki1G) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx21 * q1B - rx12q2B) + ry12 * (rx21 * sr[kki, sx, 2] - rx12srkki1B) + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(s - cy - 0.5f, -4);

                            i = ixsxp;
                            rx21 = dbls[6];
                            riR[i, s, kkikks] = (short)(ry21 * (rx21 * q1R + rx12q2R) + ry12 * (rx21 * sr[kki, sx, 0] + rx12srkki1R) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx21 * q1G + rx12q2G) + ry12 * (rx21 * sr[kki, sx, 1] + rx12srkki1G) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx21 * q1B + rx12q2B) + ry12 * (rx21 * sr[kki, sx, 2] + rx12srkki1B) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];

                            i = ixsxpi;
                            riR[i, s, kkikks] = (short)(ry21 * (rx21 * q2R + rx12 * q1R) + ry12 * (dbls[0] * sr[kki, sx, 0] + rx21 * sr[kki + 1, sx, 0]) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx21 * q2G + rx12 * q1G) + ry12 * (dbls[0] * sr[kki, sx, 1] + rx21 * sr[kki + 1, sx, 1]) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx21 * q2B + rx12 * q1B) + ry12 * (dbls[0] * sr[kki, sx, 2] + rx21 * sr[kki + 1, sx, 2]) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];

                            i = ixsxpp;
                            rx12 = dbls[5];
                            riR[i, s, kkikks] = (short)(ry21 * (rx12 * q2R - dbls[0] * q1R) + ry12 * (rx12 * sr[kki + 1, sx, 0] - dbls[0] * sr[kki, sx, 0]) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx12 * q2G - dbls[0] * q1G) + ry12 * (rx12 * sr[kki + 1, sx, 1] - dbls[0] * sr[kki, sx, 1]) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx12 * q2B - dbls[0] * q1B) + ry12 * (rx12 * sr[kki + 1, sx, 2] - dbls[0] * sr[kki, sx, 2]) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];
                        }
                    }
                });

            var top = accelerator.LoadKernel(
                (ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView3D<byte, Stride3D.DenseXY> sr) =>
                {
                    int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
                    int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
                    if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[10])
                    {
                        int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
                        float xsx = kki * ints[0] + dbls[0];

                        int ixsx = kki * ints[0];
                        int ixsxp = ixsx + ints[1];
                        int ixsxpi = ixsxp + 1;
                        float cy = kks * ints[0] + dbls[1];
                        int ixsxpp = ixsxpi + ints[1];
                        int xsp = kki + 1;

                        for (int sx = kks - 1; sx > -1; sx--)
                        {
                            int dsxx = sx * ints[0];
                            int sxxm = dsxx + ints[0];
                            float dsxxm = dsxx + dbls[0];
                            int i = ixsx + Grid.GlobalIndex.X % ints[6];
                            int s = dsxx + Grid.GlobalIndex.X / ints[6];
                            float rx12, rx21, ry12, ry21;
                            float q4R = (riR[ixsx, sxxm, kkikks] + riR[ixsxp, sxxm, kkikks]) / 2;
                            float q3R = (riR[ixsxpi, sxxm, kkikks] + riR[ixsxpp, sxxm, kkikks]) / 2;
                            float q4G = (riG[ixsx, sxxm, kkikks] + riG[ixsxp, sxxm, kkikks]) / 2;
                            float q3G = (riG[ixsxpi, sxxm, kkikks] + riG[ixsxpp, sxxm, kkikks]) / 2;
                            float q4B = (riB[ixsx, sxxm, kkikks] + riB[ixsxp, sxxm, kkikks]) / 2;
                            float q3B = (riB[ixsxpi, sxxm, kkikks] + riB[ixsxpp, sxxm, kkikks]) / 2;
                            ry21 = (sxxm - s) / dbls[2];
                            ry12 = (s - dsxxm) / dbls[2]; //After ALL, 1bit channel! 192thresold ||to2bits
                            rx21 = (xsx + ints[0] - i);
                            rx12 = (i - xsx);
                            riR[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx, 0] + rx12 * sr[xsp, sx, 0]) + ry12 * (rx21 * q4R + rx12 * q3R) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx, 1] + rx12 * sr[xsp, sx, 1]) + ry12 * (rx21 * q4G + rx12 * q3G) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx, 2] + rx12 * sr[xsp, sx, 2]) + ry12 * (rx21 * q4B + rx12 * q3B) + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(Dist2Top(i, s, ixsxp + 0.5f, cy, dbls[4]), -4);

                            s = dsxx;
                            i = ixsx;
                            ry21 = dbls[8];
                            ry12 = dbls[7];
                            rx21 = dbls[5];
                            rx12 = dbls[0];
                            float rx12srxspR = rx12 * sr[xsp, sx, 0];
                            float rx12q3R = rx12 * q3R;
                            float rx12srxspG = rx12 * sr[xsp, sx, 1];
                            float rx12q3G = rx12 * q3G;
                            float rx12srxspB = rx12 * sr[xsp, sx, 2];
                            float rx12q3B = rx12 * q3B;
                            riR[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx, 0] - rx12srxspR) + ry12 * (rx21 * q4R - rx12q3R) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx, 1] - rx12srxspG) + ry12 * (rx21 * q4G - rx12q3G) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx, 2] - rx12srxspB) + ry12 * (rx21 * q4B - rx12q3B) + 0.5);
                            ris[i, s, kkikks] = MathF.Pow(cy - s - 0.5f, -4);

                            i = ixsxp;
                            rx21 = dbls[6];
                            riR[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx, 0] + rx12srxspR) + ry12 * (rx21 * q4R + rx12q3R) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx, 1] + rx12srxspG) + ry12 * (rx21 * q4G + rx12q3G) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx21 * sr[kki, sx, 2] + rx12srxspB) + ry12 * (rx21 * q4B + rx12q3B) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];

                            i = ixsxpi;
                            riR[i, s, kkikks] = (short)(ry21 * (rx12 * sr[kki, sx, 0] + rx21 * sr[xsp, sx, 0]) + ry12 * (rx12 * q4R + rx21 * q3R) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx12 * sr[kki, sx, 1] + rx21 * sr[xsp, sx, 1]) + ry12 * (rx12 * q4G + rx21 * q3G) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx12 * sr[kki, sx, 2] + rx21 * sr[xsp, sx, 2]) + ry12 * (rx12 * q4B + rx21 * q3B) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];

                            i = ixsxpp;
                            rx12 = dbls[5];
                            riR[i, s, kkikks] = (short)(ry21 * (rx12 * sr[xsp, sx, 0] - dbls[0] * sr[kki, sx, 0]) + ry12 * (rx12 * q3R - dbls[0] * q4R) + 0.5);
                            riG[i, s, kkikks] = (short)(ry21 * (rx12 * sr[xsp, sx, 1] - dbls[0] * sr[kki, sx, 1]) + ry12 * (rx12 * q3G - dbls[0] * q4G) + 0.5);
                            riB[i, s, kkikks] = (short)(ry21 * (rx12 * sr[xsp, sx, 2] - dbls[0] * sr[kki, sx, 2]) + ry12 * (rx12 * q3B - dbls[0] * q4B) + 0.5);
                            ris[i, s, kkikks] = ris[ixsx, s, kkikks];
                        }
                    }
                });

            var rightbottom = accelerator.LoadKernel(
        (ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView3D<byte, Stride3D.DenseXY> sr) =>
        {
            int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
            int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
            if (kki < ints[2] && kks < ints[3] && Grid.GlobalIndex.X < ints[11])
            {
                int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;

                float cxcyx = (kki + kks + 3) * ints[0] - 1;
                for (int sx = kks + 2; sx < ints[3]; sx++)
                {
                    for (int ix = kki + 2; ix < ints[2]; ix++)
                    {
                        int dsxxm = sx * ints[0];
                        int sxxm = dsxxm - 1;
                        int dsxx = (sx + 1) * ints[0];
                        int isxx = dsxx - 1;
                        int dixxm = ix * ints[0];
                        int ixxm = dixxm - 1;
                        int iixx = (ix + 1) * ints[0] - 1;

                        int i = dixxm + Grid.GlobalIndex.X % ints[0];
                        int s = dsxxm + Grid.GlobalIndex.X / ints[0];
                        float rx12, rx21;
                        float q2R = (riR[dixxm, sxxm, kkikks] + riR[iixx, sxxm, kkikks]) / 2;
                        float q4R = (riR[ixxm, dsxxm, kkikks] + riR[ixxm, isxx, kkikks]) / 2;
                        float q2G = (riG[dixxm, sxxm, kkikks] + riG[iixx, sxxm, kkikks]) / 2;
                        float q4G = (riG[ixxm, dsxxm, kkikks] + riG[ixxm, isxx, kkikks]) / 2;
                        float q2B = (riB[dixxm, sxxm, kkikks] + riB[iixx, sxxm, kkikks]) / 2;
                        float q4B = (riB[ixxm, dsxxm, kkikks] + riB[ixxm, isxx, kkikks]) / 2;
                        rx21 = dixxm + dbls[0] - i;
                        rx12 = i - ixxm;
                        float ry21 = (dsxxm + dbls[0] - s) / dbls[3];
                        float ry12 = (s - sxxm) / dbls[3];
                        riR[i, s, kkikks] = (short)(ry21 * (rx21 * riR[ixxm, sxxm, kkikks] + rx12 * q2R) + ry12 * (rx21 * q4R + rx12 * sr[ix, sx, 0]) + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * (rx21 * riG[ixxm, sxxm, kkikks] + rx12 * q2G) + ry12 * (rx21 * q4G + rx12 * sr[ix, sx, 1]) + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * (rx21 * riB[ixxm, sxxm, kkikks] + rx12 * q2B) + ry12 * (rx21 * q4B + rx12 * sr[ix, sx, 2]) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i - cxcyx + s, -4);

                        s = isxx;
                        i = dixxm;
                        float dbls0q4R = dbls[0] * q4R;
                        float dbls0riixxmR = dbls[0] * riR[ixxm, sxxm, kkikks];
                        float dbls0q4G = dbls[0] * q4G;
                        float dbls0riixxmG = dbls[0] * riG[ixxm, sxxm, kkikks];
                        float dbls0q4B = dbls[0] * q4B;
                        float dbls0riixxmB = dbls[0] * riB[ixxm, sxxm, kkikks];
                        riR[i, s, kkikks] = (short)(dbls[9] * (dbls0riixxmR + q2R) + dbls[10] * (dbls0q4R + sr[ix, sx, 0]) + 0.5);
                        riG[i, s, kkikks] = (short)(dbls[9] * (dbls0riixxmG + q2G) + dbls[10] * (dbls0q4G + sr[ix, sx, 1]) + 0.5);
                        riB[i, s, kkikks] = (short)(dbls[9] * (dbls0riixxmB + q2B) + dbls[10] * (dbls0q4B + sr[ix, sx, 2]) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i - cxcyx + s, -4);

                        i = iixx;
                        q2R = dbls[9] * (ints[0] * q2R - dbls0riixxmR);
                        q4R = (ints[0] * sr[ix, sx, 0] - dbls0q4R);
                        q2G = dbls[9] * (ints[0] * q2G - dbls0riixxmG);
                        q4G = (ints[0] * sr[ix, sx, 1] - dbls0q4G);
                        q2B = dbls[9] * (ints[0] * q2B - dbls0riixxmB);
                        q4B = (ints[0] * sr[ix, sx, 2] - dbls0q4B);
                        riR[i, s, kkikks] = (short)(q2R + dbls[10] * q4R + 0.5);
                        riG[i, s, kkikks] = (short)(q2G + dbls[10] * q4G + 0.5);
                        riB[i, s, kkikks] = (short)(q2B + dbls[10] * q4B + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i - cxcyx + s, -4);

                        s = dsxxm;
                        riR[i, s, kkikks] = (short)(q4R * dbls[11] - q2R + 0.5);
                        riG[i, s, kkikks] = (short)(q4G * dbls[11] - q2G + 0.5);
                        riB[i, s, kkikks] = (short)(q4B * dbls[11] - q2B + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i - cxcyx + s, -4);
                    }
                }
            }
        });

            var lefttop = accelerator.LoadKernel(
        (ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView3D<byte, Stride3D.DenseXY> sr) =>
        {
            int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
            int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
            int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
            if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[11] && kkikks < ints[5])
            {
                float cxcyx = (kki + kks + 1) * ints[0] - 1;
                for (int sx = kks - 1; sx > -1; sx--)
                {
                    for (int ix = kki - 1; ix > -1; ix--)
                    {
                        int dsxxm = sx * ints[0];
                        int sxxm = (sx + 1) * ints[0];
                        int isxx = sxxm - 1;
                        int dixxm = ix * ints[0];
                        int ixxm = (ix + 1) * ints[0];
                        int iixx = ixxm - 1;

                        int i = dixxm + Grid.GlobalIndex.X % ints[0];
                        int s = dsxxm + Grid.GlobalIndex.X / ints[0];
                        float rx12, rx21, ry12, ry21;
                        float q2R = (riR[ixxm, dsxxm, kkikks] + riR[ixxm, isxx, kkikks]) / 2;
                        float q4R = (riR[dixxm, sxxm, kkikks] + riR[iixx, sxxm, kkikks]) / 2;
                        float q2G = (riG[ixxm, dsxxm, kkikks] + riG[ixxm, isxx, kkikks]) / 2;
                        float q4G = (riG[dixxm, sxxm, kkikks] + riG[iixx, sxxm, kkikks]) / 2;
                        float q2B = (riB[ixxm, dsxxm, kkikks] + riB[ixxm, isxx, kkikks]) / 2;
                        float q4B = (riB[dixxm, sxxm, kkikks] + riB[iixx, sxxm, kkikks]) / 2;
                        ry21 = (sxxm - s) / dbls[3];
                        ry12 = (s - dsxxm - dbls[0]) / dbls[3];
                        rx21 = ixxm - i;
                        rx12 = i - dixxm - dbls[0];
                        riR[i, s, kkikks] = (short)(ry21 * (rx21 * sr[ix, sx, 0] + rx12 * q2R) + ry12 * (rx21 * q4R + rx12 * riR[ixxm, sxxm, kkikks]) + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * (rx21 * sr[ix, sx, 1] + rx12 * q2G) + ry12 * (rx21 * q4G + rx12 * riG[ixxm, sxxm, kkikks]) + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * (rx21 * sr[ix, sx, 2] + rx12 * q2B) + ry12 * (rx21 * q4B + rx12 * riB[ixxm, sxxm, kkikks]) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i - s, -4);

                        i = dixxm;
                        s = isxx;
                        ry12 = dbls[0];
                        rx21 = ints[0];
                        float rx12R = -ry12 * riR[ixxm, sxxm, kkikks];
                        float rx12G = -ry12 * riG[ixxm, sxxm, kkikks];
                        float rx12B = -ry12 * riB[ixxm, sxxm, kkikks];
                        q2R = -ry12 * q2R;
                        q2G = -ry12 * q2G;
                        q2B = -ry12 * q2B;
                        float rx21srq2R = rx21 * sr[ix, sx, 0] + q2R;
                        float rx21q4rx12R = rx21 * q4R + rx12R;
                        float rx21srq2G = rx21 * sr[ix, sx, 1] + q2G;
                        float rx21q4rx12G = rx21 * q4G + rx12G;
                        float rx21srq2B = rx21 * sr[ix, sx, 2] + q2B;
                        float rx21q4rx12B = rx21 * q4B + rx12B;
                        riR[i, s, kkikks] = (short)((rx21srq2R + ry12 * rx21q4rx12R) * dbls[11] + 0.5);
                        riG[i, s, kkikks] = (short)((rx21srq2G + ry12 * rx21q4rx12G) * dbls[11] + 0.5);
                        riB[i, s, kkikks] = (short)((rx21srq2B + ry12 * rx21q4rx12B) * dbls[11] + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i - s, -4);

                        s = dsxxm;
                        ry21 = dbls[10];
                        ry12 = dbls[9];
                        riR[i, s, kkikks] = (short)(ry21 * rx21srq2R + ry12 * rx21q4rx12R + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * rx21srq2G + ry12 * rx21q4rx12G + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * rx21srq2B + ry12 * rx21q4rx12B + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i - s, -4);

                        i = iixx;
                        riR[i, s, kkikks] = (short)(ry21 * (sr[ix, sx, 0] - q2R) + ry12 * (q4R - rx12R) + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * (sr[ix, sx, 1] - q2G) + ry12 * (q4G - rx12G) + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * (sr[ix, sx, 2] - q2B) + ry12 * (q4B - rx12B) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i - s, -4);
                    }
                }
            }
        });

            var leftbottom = accelerator.LoadKernel(
        (ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView3D<byte, Stride3D.DenseXY> sr) =>
        {
            int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
            int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
            if (kki < ints[4] && kks < ints[3] && Grid.GlobalIndex.X < ints[11])
            {
                int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;

                float cxcyx = (kki - kks - 1) * ints[0];
                for (int sx = kks + 2; sx < ints[3]; sx++)
                {
                    for (int ix = kki - 1; ix > -1; ix--)
                    {
                        int sxx = sx * ints[0];
                        int isxxm = sxx - 1;
                        int isxx = (sx + 1) * ints[0] - 1;
                        int dixxm = ix * ints[0];
                        int dixx = dixxm + ints[0];
                        int iixx = dixx - 1;

                        int i = dixxm + Grid.GlobalIndex.X % ints[0];
                        int s = sxx + Grid.GlobalIndex.X / ints[0];
                        float rx12, rx21, ry12, ry21;
                        float q1R = (riR[dixxm, isxxm, kkikks] + riR[iixx, isxxm, kkikks]) / 2;
                        float q3R = (riR[dixx, isxx, kkikks] + riR[dixx, sxx, kkikks]) / 2;
                        float q1G = (riG[dixxm, isxxm, kkikks] + riG[iixx, isxxm, kkikks]) / 2;
                        float q3G = (riG[dixx, isxx, kkikks] + riG[dixx, sxx, kkikks]) / 2;
                        float q1B = (riB[dixxm, isxxm, kkikks] + riB[iixx, isxxm, kkikks]) / 2;
                        float q3B = (riB[dixx, isxx, kkikks] + riB[dixx, sxx, kkikks]) / 2;
                        ry21 = (sxx + dbls[0] - s) / dbls[3];
                        ry12 = (s - isxxm) / dbls[3];
                        rx21 = dixx - i;
                        rx12 = i - dixxm - dbls[0];
                        riR[i, s, kkikks] = (short)(ry21 * (rx21 * q1R + rx12 * riR[dixx, isxxm, kkikks]) + ry12 * (rx21 * sr[ix, sx, 0] + rx12 * q3R) + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * (rx21 * q1G + rx12 * riG[dixx, isxxm, kkikks]) + ry12 * (rx21 * sr[ix, sx, 1] + rx12 * q3G) + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * (rx21 * q1B + rx12 * riB[dixx, isxxm, kkikks]) + ry12 * (rx21 * sr[ix, sx, 2] + rx12 * q3B) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i + s, -4);

                        i = dixxm;
                        s = sxx;
                        ry21 = dbls[9];
                        ry12 = dbls[11];
                        rx12 = -dbls[0];
                        q3R = rx12 * q3R;
                        q3G = rx12 * q3G;
                        q3B = rx12 * q3B;
                        float rxriR = rx12 * riR[dixx, isxxm, kkikks];
                        float rxriG = rx12 * riG[dixx, isxxm, kkikks];
                        float rxriB = rx12 * riB[dixx, isxxm, kkikks];
                        float q1rxR = ints[0] * q1R + rxriR;
                        float q1rxG = ints[0] * q1G + rxriG;
                        float q1rxB = ints[0] * q1B + rxriB;
                        float xsrR = ints[0] * sr[ix, sx, 0];
                        float xsrG = ints[0] * sr[ix, sx, 1];
                        float xsrB = ints[0] * sr[ix, sx, 2];
                        riR[i, s, kkikks] = (short)(ry12 * (xsrR + q3R) - ry21 * q1rxR + 0.5);
                        riG[i, s, kkikks] = (short)(ry12 * (xsrG + q3G) - ry21 * q1rxG + 0.5);
                        riB[i, s, kkikks] = (short)(ry12 * (xsrB + q3B) - ry21 * q1rxB + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i + s, -4);

                        s = isxx;
                        ry12 = dbls[10];
                        riR[i, s, kkikks] = (short)(ry21 * q1rxR + ry12 * (xsrR + q3R) + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * q1rxG + ry12 * (xsrG + q3G) + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * q1rxB + ry12 * (xsrB + q3B) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i + s, -4);

                        i = iixx;
                        riR[i, s, kkikks] = (short)(ry21 * (q1R - rxriR) + ry12 * (sr[ix, sx, 0] - q3R) + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * (q1G - rxriG) + ry12 * (sr[ix, sx, 1] - q3G) + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * (q1B - rxriB) + ry12 * (sr[ix, sx, 2] - q3B) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(cxcyx - i + s, -4);
                    }
                }
            }
        });

            var righttop = accelerator.LoadKernel(
        (ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints, ArrayView<float> dbls, ArrayView3D<byte, Stride3D.DenseXY> sr) =>
        {
            int kki = Grid.GlobalIndex.Y * ints[12] + ints[13] + ints[17];
            int kks = Grid.GlobalIndex.Z * ints[12] + ints[15] + ints[18];
            int kkikks = Grid.GlobalIndex.Z * ints[7] + Grid.GlobalIndex.Y + 1;
            if (kki < ints[4] && kks < ints[14] && Grid.GlobalIndex.X < ints[11] && kkikks < ints[5])
            {
                float cxcyx = (kks - kki - 1) * ints[0];
                for (int ix = kki + 2; ix < ints[2]; ix++)
                {
                    for (int sx = kks - 1; sx > -1; sx--)
                    {
                        int sxx = sx * ints[0];
                        int dsxx = sxx + ints[0];
                        int isxx = dsxx - 1;
                        int ixx = ix * ints[0];
                        int ixxm = ixx - 1;
                        int iixx = ixx + ints[1];

                        int i = ixx + Grid.GlobalIndex.X % ints[0];
                        int s = sxx + Grid.GlobalIndex.X / ints[0];
                        float rx12, rx21, ry12, ry21;
                        float q1R = (riR[ixxm, sxx, kkikks] + riR[ixxm, isxx, kkikks]) / 2;
                        float q3R = (riR[ixx, dsxx, kkikks] + riR[iixx, dsxx, kkikks]) / 2;
                        float q1G = (riG[ixxm, sxx, kkikks] + riG[ixxm, isxx, kkikks]) / 2;
                        float q3G = (riG[ixx, dsxx, kkikks] + riG[iixx, dsxx, kkikks]) / 2;
                        float q1B = (riB[ixxm, sxx, kkikks] + riB[ixxm, isxx, kkikks]) / 2;
                        float q3B = (riB[ixx, dsxx, kkikks] + riB[iixx, dsxx, kkikks]) / 2;
                        ry21 = (dsxx - s) / dbls[3];
                        ry12 = (s - sxx - dbls[0]) / dbls[3];
                        rx21 = ixx + dbls[0] - i;
                        rx12 = i - ixxm;
                        riR[i, s, kkikks] = (short)(ry21 * (rx21 * q1R + rx12 * sr[ix, sx, 0]) + ry12 * (rx21 * riR[ixxm, dsxx, kkikks] + rx12 * q3R) + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * (rx21 * q1G + rx12 * sr[ix, sx, 1]) + ry12 * (rx21 * riG[ixxm, dsxx, kkikks] + rx12 * q3G) + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * (rx21 * q1B + rx12 * sr[ix, sx, 2]) + ry12 * (rx21 * riB[ixxm, dsxx, kkikks] + rx12 * q3B) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i + cxcyx - s, -4);

                        i = iixx;
                        s = sxx;
                        ry21 = dbls[10];
                        ry12 = dbls[9];
                        rx21 = dbls[0];
                        rx12 = ints[0];
                        q1R = rx21 * q1R;
                        q1G = rx21 * q1G;
                        q1B = rx21 * q1B;
                        float rxsrR = rx12 * sr[ix, sx, 0] - q1R;
                        float rxsrG = rx12 * sr[ix, sx, 1] - q1G;
                        float rxsrB = rx12 * sr[ix, sx, 2] - q1B;
                        float ryrxR = ry12 * (rx12 * q3R - rx21 * riR[ixxm, dsxx, kkikks]);
                        float ryrxG = ry12 * (rx12 * q3G - rx21 * riG[ixxm, dsxx, kkikks]);
                        float ryrxB = ry12 * (rx12 * q3B - rx21 * riB[ixxm, dsxx, kkikks]);
                        riR[i, s, kkikks] = (short)(ry21 * rxsrR + ryrxR + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * rxsrG + ryrxG + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * rxsrB + ryrxB + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i + cxcyx - s, -4);

                        s = isxx;
                        ry21 = dbls[11];
                        riR[i, s, kkikks] = (short)(ry21 * rxsrR - ryrxR + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * rxsrG - ryrxG + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * rxsrB - ryrxB + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i + cxcyx - s, -4);

                        i = ixx;
                        riR[i, s, kkikks] = (short)(ry21 * (sr[ix, sx, 0] + q1R) - ry12 * (q3R + rx21 * riR[ixxm, dsxx, kkikks]) + 0.5);
                        riG[i, s, kkikks] = (short)(ry21 * (sr[ix, sx, 1] + q1G) - ry12 * (q3G + rx21 * riG[ixxm, dsxx, kkikks]) + 0.5);
                        riB[i, s, kkikks] = (short)(ry21 * (sr[ix, sx, 2] + q1B) - ry12 * (q3B + rx21 * riB[ixxm, dsxx, kkikks]) + 0.5);
                        ris[i, s, kkikks] = MathF.Pow(i + cxcyx - s, -4);
                    }
                }
            }
        });

            int threadsPerGroup = accelerator.Device.WarpSize;

            var ttl = accelerator.LoadAutoGroupedStreamKernel(
        (Index2D ii, ArrayView3D<short, Stride3D.DenseXY> riR, ArrayView3D<short, Stride3D.DenseXY> riG, ArrayView3D<short, Stride3D.DenseXY> riB, ArrayView3D<float, Stride3D.DenseXY> ris, ArrayView<int> ints) =>
        {
            ris[ii.X, ii.Y, 0] = 0;
            for (int kkikks = 1; kkikks < ints[5]; kkikks++)
            {
                riR[ii.X, ii.Y, 0] = (short)((riR[ii.X, ii.Y, kkikks] * ris[ii.X, ii.Y, kkikks] + riR[ii.X, ii.Y, 0] * ris[ii.X, ii.Y, 0]) / (ris[ii.X, ii.Y, kkikks] + ris[ii.X, ii.Y, 0]) + 0.5);
                riG[ii.X, ii.Y, 0] = (short)((riG[ii.X, ii.Y, kkikks] * ris[ii.X, ii.Y, kkikks] + riG[ii.X, ii.Y, 0] * ris[ii.X, ii.Y, 0]) / (ris[ii.X, ii.Y, kkikks] + ris[ii.X, ii.Y, 0]) + 0.5);
                riB[ii.X, ii.Y, 0] = (short)((riB[ii.X, ii.Y, kkikks] * ris[ii.X, ii.Y, kkikks] + riB[ii.X, ii.Y, 0] * ris[ii.X, ii.Y, 0]) / (ris[ii.X, ii.Y, kkikks] + ris[ii.X, ii.Y, 0]) + 0.5);
                ris[ii.X, ii.Y, 0] = ris[ii.X, ii.Y, kkikks] + ris[ii.X, ii.Y, 0];
            }
        });

            float progress = 0;
            float rki2 = rki * rki;
            float step = 0.98f / rki2;
            float step8 = 7.84f / rki2;
            float step89 = 87.22f / rki2;
            var dblG = accelerator.Allocate1D<float>([halfx - 0.5f, x - 0.5f, x2p * x, x2p * x2p, halfx, x * 1.5f - 0.5f, x2p, (1f - x) / (x + xx), 2f / (1 + x), (2f - 2 * x) * sqeq, 4f * x * sqeq, 4f * sqeq]);
            var intG = accelerator.Allocate1D<int>(17);
            for (int irk = 0; irk < rki; irk++)
            {
                for (int srk = 0; srk < rki; srk++)
                {
                    intG = accelerator.Allocate1D<int>([x, x - 1, oi, os, oim, kiks, 2 * x, ki, oim / 2 - ki / 2, osm / 2 - ks / 2, xx2, xx, oiki, irk, osm, srk, xx4, ski, sks]);
                    Vector3 grp = FitToVolumeByX(xx4, accelerator.Device.MaxGroupSize, threadsPerGroup);
                    KernelConfig kc = new(new Index3D((int)MathF.Ceiling(xx4 / grp.X), (int)MathF.Ceiling(ki / grp.Y), (int)MathF.Ceiling(ks / grp.Z)), new((int)grp.X, (int)grp.Y, (int)grp.Z));

                    center(kc, riRG.View, riGG.View, riBG.View, risG.View, intG.View, dblG.View, srG.View);

                    grp = FitToVolumeByX(xx2, accelerator.Device.MaxGroupSize, threadsPerGroup);
                    kc = new(new Index3D((int)MathF.Ceiling(xx2 / grp.X), (int)MathF.Ceiling(ki / grp.Y), (int)MathF.Ceiling(ks / grp.Z)), new((int)grp.X, (int)grp.Y, (int)grp.Z));
                    progress += step;
                    ProgressText.Text = ((int)progress).ToString();
                    AcceleratorStream Stream1 = accelerator.CreateStream();
                    AcceleratorStream Stream2 = accelerator.CreateStream();
                    AcceleratorStream Stream3 = accelerator.CreateStream();
                    AcceleratorStream Stream4 = accelerator.CreateStream();
                    accelerator.Synchronize();

                    right(Stream1, kc, riRG.View, riGG.View, riBG.View, risG.View, intG.View, dblG.View, srG.View);
                    left(Stream2, kc, riRG.View, riGG.View, riBG.View, risG.View, intG.View, dblG.View, srG.View);
                    bottom(Stream3, kc, riRG.View, riGG.View, riBG.View, risG.View, intG.View, dblG.View, srG.View);
                    top(Stream4, kc, riRG.View, riGG.View, riBG.View, risG.View, intG.View, dblG.View, srG.View);
                    grp = FitToVolumeByX(xx, accelerator.Device.MaxGroupSize, threadsPerGroup);
                    kc = new(new Index3D((int)MathF.Ceiling(xx / grp.X), (int)MathF.Ceiling(ki / grp.Y), (int)MathF.Ceiling(ks / grp.Z)), new((int)grp.X, (int)grp.Y, (int)grp.Z));
                    progress += step;
                    ProgressText.Text = ((int)progress).ToString();
                    accelerator.Synchronize();

                    rightbottom(Stream1, kc, riRG.View, riGG.View, riBG.View, risG.View, intG.View, dblG.View, srG.View);
                    lefttop(Stream2, kc, riRG.View, riGG.View, riBG.View, risG.View, intG.View, dblG.View, srG.View);
                    leftbottom(Stream3, kc, riRG.View, riGG.View, riBG.View, risG.View, intG.View, dblG.View, srG.View);
                    righttop(Stream4, kc, riRG.View, riGG.View, riBG.View, risG.View, intG.View, dblG.View, srG.View);
                    progress += step8;
                    ProgressText.Text = ((int)progress).ToString();//After all, oi/ki or center? accuracy analyze
                    accelerator.Synchronize();//After ALL, BilinearApprox2bit -> Smooth?

                    progress += step89; //After ALL, contrast +80contrast?
                    ttl(new Index2D(ni, ns), riRG.View, riGG.View, riBG.View, risG.View, intG.View);
                    ProgressText.Text = ((int)progress).ToString();//After ALL, contrast+80/thin sobel->Furry/Smooth
                    accelerator.Synchronize();
                }
            }

            ProgressText.Text = "99";
            rR = riRG.View.SubView(new Index3D(0, 0, 0), new Index3D(ni, ns, 1)).GetAsArray3D();
            rG = riGG.View.SubView(new Index3D(0, 0, 0), new Index3D(ni, ns, 1)).GetAsArray3D();
            rB = riBG.View.SubView(new Index3D(0, 0, 0), new Index3D(ni, ns, 1)).GetAsArray3D();
            accelerator.Dispose();
            context.Dispose();
            byte[,,] rb = new byte[ni, ns, 3];
            Parallel.For(0, ni, i =>
            {
                for (int s = 0; s < ns; s++)
                {
                    if (rR[i, s, 0] < 0.5)
                        rb[i, s, 0] = 0; //AFTER ALL and release - 0&&0=0  //contrast??? 0x1 - not gray //sobel? 
                    else if (rR[i, s, 0] > 254.5) //AFTER ALL and release - AverageBilinear 
                        rb[i, s, 0] = 255;
                    else
                        rb[i, s, 0] = (byte)(rR[i, s, 0] + 0.5);

                    if (rG[i, s, 0] < 0.5)
                        rb[i, s, 1] = 0;
                    else if (rG[i, s, 0] > 254.5)
                        rb[i, s, 1] = 255;
                    else
                        rb[i, s, 1] = (byte)(rG[i, s, 0] + 0.5);

                    if (rB[i, s, 0] < 0.5)
                        rb[i, s, 2] = 0;
                    else if (rB[i, s, 0] > 254.5)
                        rb[i, s, 2] = 255;
                    else
                        rb[i, s, 2] = (byte)(rB[i, s, 0] + 0.5);
                }
            });

            return BMPfromRGB(rb, ni, ns);
        }

        private static float Dist2Top(int i, int s, float cx, float cy, float x2)
        {
            if (i < cx)
            {
                return MathF.Abs(i - cx + x2) - s + cy - x2;
            }
            else
            {
                return MathF.Abs(i - cx - x2) - s + cy - x2;
            }
        }

        static float Dist4(int i, int s, float cx, float cy, float x2)
        {
            if (i < cx)
            {
                if (s < cy) //After ALL, parabola?
                {
                    return MathF.Max(MathF.Abs(i - cx + x2) + MathF.Abs(s - cy + x2), 0.0001f);
                }
                else
                {
                    return MathF.Max(MathF.Abs(i - cx + x2) + MathF.Abs(s - cy - x2), 0.0001f);
                }
            }
            else
            {
                if (s < cy)
                {
                    return MathF.Max(MathF.Abs(i - cx - x2) + MathF.Abs(s - cy + x2), 0.0001f);
                }
                else
                {
                    return MathF.Max(MathF.Abs(i - cx - x2) + MathF.Abs(s - cy - x2), 0.0001f);
                }
            }
        }

        private static float Dist2Bottom(int i, int s, float cx, float cy, float x2)
        {
            if (i < cx)
            {
                return MathF.Abs(i - cx + x2) + s - cy - x2;
            }
            else
            {
                return MathF.Abs(i - cx - x2) + s - cy - x2;
            }
        }

        private static float Dist2Left(int i, int s, float cx, float cy, float x2)
        {
            if (s < cy)
            {
                return cx - i - x2 + MathF.Abs(s - cy + x2);
            }
            else
            {
                return cx - i - x2 + MathF.Abs(s - cy - x2);
            }
        }

        private static float Dist2Right(int i, int s, float cx, float cy, float x2)
        {
            if (s < cy)
            {
                return i - cx - x2 + MathF.Abs(s - cy + x2);
            }
            else
            {
                return i - cx - x2 + MathF.Abs(s - cy - x2);
            }
        }

        private static Vector3 FitToVolumeByX(int x, Index3D maxGroupSize, int threads)
        {
            Vector3 grp = new(x, maxGroupSize.Y, maxGroupSize.Z);
            if (threads < x)
            {
                grp.X = threads;
                grp.Y = 1;
                grp.Z = 1;
            }
            else
            {
                if (grp.X * grp.Y * grp.Z > threads)
                {
                    grp.Y = MathF.Ceiling(threads / x / grp.Z);
                }
                if (grp.X * grp.Y * grp.Z > threads)
                {
                    grp.Z = MathF.Ceiling(threads / x / grp.Y);
                }
                while (grp.Y < grp.Z)
                {
                    grp.Z /= 2;
                    grp.Y *= 2;
                }
            }
            return grp;
        }

        private Bitmap NeighborSubpixelGray(Image img, int x, int ac) //After stage3, color
        {
            int ni, ns, oi, os;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            byte[,] r = new byte[ni, ns];

            float[,] xr = new float[oi + 2, os + 2];
            float[,] yr = new float[oi + 2, os + 2];
            byte[,] sr = GrayFromBMP(img, 1, 1, 1, 1, oi, os);

            NaiveExtrapolation(sr, oi, os);

            return BMPfromGray(sr, oi + 2, os + 2);
            /*byte[,,] sr = RGBfromBMP(img, 1, 1, 1, 1, oi, os);
            byte[,] srR = new byte[oi + 2, os + 2];
            byte[,] srG = new byte[oi + 2, os + 2];
            byte[,] srB = new byte[oi + 2, os + 2];
            for (int i = 0; i < oi + 2; i++)
            {
                for (int s = 0; s < os + 2; s++)
                {
                    srR[i, s] = sr[i, s, 0];
                    srG[i, s] = sr[i, s, 1];
                    srB[i, s] = sr[i, s, 2];
                }
            }
            NaiveExtrapolation(srR, oi, os);
            NaiveExtrapolation(srG, oi, os);
            NaiveExtrapolation(srB, oi, os);
            int o = 2;
            if (os < 1416) o = 0;
            for (int i = 0; i < oi + 2; i++)
            {
                for (int s = o/2; s < os + 2; s++)
                {
                    sr[i, s - o / 2, 0] = srR[i, s];
                    sr[i, s - o / 2, 1] = srG[i, s];
                    sr[i, s - o / 2, 2] = srB[i, s];
                }
            }
            return BMPfromRGB(sr, oi + 2, os + 2 - o);*/
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
                        if (realc % 3 == 0)
                            sr[i, s] = (byte)(rgbValues[c] * 0.114 + rgbValues[c + 1] * 0.587 + rgbValues[c + 2] * 0.299 + 0.5);
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
                        if (realc % 4 == 0) sr[i, s] = (byte)(rgbValues[c] * 0.114 + rgbValues[c + 1] * 0.587 + rgbValues[c + 2] * 0.299 + 0.5);
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
            float oi2, os2, oi2p, os2p;
            oip = oi + 1;
            oim = oi - 1;
            osp = os + 1;
            osm = os - 1;
            oi2 = (oi - 1) / 2f + 1;
            os2 = (os - 1) / 2f + 1;
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
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1] + d[i - 1, s - 1] + d[i + 1, s + 1] + d[i + 1, s - 1] + d[i - 1, s + 1]) / 8f + 0.5);
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
                                        d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s]) / 2f + 0.5);
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
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1]) / 4f + 0.5);
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
                                        d[i, s] = (byte)((d[i, s - 1] + d[i, s + 1]) / 2f + 0.5);
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

        private Bitmap ScaleSmoothGray(Image img, int x, int ac) //AFTER optimization, COMMENT EACH ALGORITHM
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

            float dp = 0.99f;
            for (byte t = 0; t < 3; t++)
            {
                if (sumr > 127.5 * (oi + os)) dp = 0.01f;
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
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1] + d[i - 1, s - 1] + d[i + 1, s + 1] + d[i + 1, s - 1] + d[i - 1, s + 1]) / 8f + dp);

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
                                        d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s]) / 2f + dp);
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
                                            d[i, s] = (byte)((d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1]) / 4f + dp);
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
                                        d[i, s] = (byte)((d[i, s - 1] + d[i, s + 1]) / 2f + dp);
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

        private Bitmap ScaleSmoothContrastGray(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm;
            int xx = x * x;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;

            oim = oi - 1;
            osm = os - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = (int)MathF.Ceiling(x * ac / 133f);

            float[,] d = new float[ni, ns];
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

            float ld;

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
                                            d[i, s] = (d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1] + d[i - 1, s - 1] + d[i + 1, s + 1] + d[i + 1, s - 1] + d[i - 1, s + 1]) / 8f;
                                            changePixelWithoutChangingMeanValueBlock(ld, d[i, s], ixx, sxx, ref d, x, xx);
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
                                        d[i, s] = (d[i - 1, s] + d[i + 1, s]) / 2f;
                                        changePixelWithoutChangingMeanValueBlock(ld, d[i, s], ixx, sxx, ref d, x, xx);
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
                                            d[i, s] = (d[i - 1, s] + d[i + 1, s] + d[i, s - 1] + d[i, s + 1]) / 4f;
                                            changePixelWithoutChangingMeanValueBlock(ld, d[i, s], ixx, sxx, ref d, x, xx);
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
                                        d[i, s] = (d[i, s - 1] + d[i, s + 1]) / 2f;
                                        changePixelWithoutChangingMeanValueBlock(ld, d[i, s], ixx, sxx, ref d, x, xx);
                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }

            byte[,] dd = new byte[ni, ns];
            for (int i = 0; i < ni; i++)
            {
                for (int s = 0; s < ns; s++)
                {
                    dd[i, s] = Contrast80(d[i, s]);
                }
            }

            return BMPfromGray(dd, ni, ns);
        }

        private static byte Contrast80(float v)
        {
            switch (v)
            {
                case <= 89:
                    return 0;
                case >= 166:
                    return 255;
                default:
                    return (byte)((v - 89f) / 77f * 255f + 0.5f);
            }
        }

        private static void changePixelWithoutChangingMeanValueBlock(float oldValue, float newValue, int leftCoordBlock, int topCoordBlock, ref float[,] d, int x, int xx)
        {
            float r = oldValue - newValue;
            float dif;
            float nd;
            int cnt = xx;

            if (r > 0.5f)
            {
                for (int i = leftCoordBlock; i < leftCoordBlock + x; i++)
                {
                    for (int s = topCoordBlock; s < topCoordBlock + x; s++)
                    {
                        if (d[i, s] == 255f)
                        {
                            cnt--;
                        }
                    }
                }
                while (r > 0.5f && cnt > 0)
                {
                    dif = r / cnt; //AFTER ALL, for hypoten?
                    for (int i = leftCoordBlock; i < leftCoordBlock + x; i++)
                    {
                        for (int s = topCoordBlock; s < topCoordBlock + x; s++)
                        {
                            if (d[i, s] < 255f)
                            {
                                nd = d[i, s] + dif;
                                if (nd > 255)
                                {
                                    r -= 255 - d[i, s];
                                    d[i, s] = 255;
                                    cnt--;
                                }
                                else
                                {
                                    d[i, s] = nd;
                                    r -= dif;
                                }
                            }
                        }
                    }
                }
            }
            else if (r < -0.5f)
            {
                for (int i = leftCoordBlock; i < leftCoordBlock + x; i++)
                {
                    for (int s = topCoordBlock; s < topCoordBlock + x; s++)
                    {
                        if (d[i, s] == 0f)
                        {
                            cnt--;
                        }
                    }
                }
                while (r < -0.5f && cnt > 0)
                {
                    dif = r / cnt;
                    for (int i = leftCoordBlock; i < leftCoordBlock + x; i++)
                    {
                        for (int s = topCoordBlock; s < topCoordBlock + x; s++)
                        {
                            if (d[i, s] > 0f)
                            {
                                nd = d[i, s] + dif;
                                if (nd < 0)
                                {
                                    r -= d[i, s];
                                    d[i, s] = 0;
                                    cnt--;
                                }
                                else
                                {
                                    d[i, s] = nd;
                                    r -= dif;
                                }
                            }
                        }
                    }
                }
            }
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
                        for (byte t = 0; t < 3; t++)
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
                                                r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t] + r[i - 1, s - 1, t] + r[i + 1, s + 1, t] + r[i + 1, s - 1, t] + r[i - 1, s + 1, t]) / 8f + 0.5);
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
                                            r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t]) / 2f + 0.5);
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
                        for (byte t = 0; t < 3; t++)
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
                                                r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t]) / 4f + 0.5);
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
                                            r[i, s, t] = (byte)((r[i, s - 1, t] + r[i, s + 1, t]) / 2f + 0.5);
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

            for (int s = 0; s < os; s++)
            {
                sumr[0] += sr[0, s, 0];
                sumr[1] += sr[0, s, 1];
                sumr[2] += sr[0, s, 2];
                sumr[0] += sr[oim, s, 0];
                sumr[1] += sr[oim, s, 1];
                sumr[2] += sr[oim, s, 2];
            }

            float[] dp = [0.99f, 0.99f, 0.99f];

            for (byte t = 0; t < 3; t++)
            {
                if (sumr[t] > 127.5 * (oi + os)) dp[t] = 0.01f;
            }

            int rn, ld;
            int ii, ss;

            for (int c = 0; c < ac; c++)
            {
                for (byte t = 0; t < 3; t++)
                {
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
                                            if (s != 0 && s != nsm)
                                            {
                                                ld = r[i, s, t];
                                                r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t] + r[i - 1, s - 1, t] + r[i + 1, s + 1, t] + r[i + 1, s - 1, t] + r[i - 1, s + 1, t]) / 8f + dp[t]);
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
                                            r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t]) / 2f + dp[t]);
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
                                                ld = r[i, s, t];
                                                r[i, s, t] = (byte)((r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t]) / 4f + dp[t]);
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
                                            r[i, s, t] = (byte)((r[i, s - 1, t] + r[i, s + 1, t]) / 2f + dp[t]);
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

        private Bitmap ScaleSmoothContrastColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, oim, osm, xm, nim, nsm;
            xm = x - 1;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            int xx = x * x;
            oim = oi - 1;
            osm = os - 1;
            nim = ni - 1;
            nsm = ns - 1;

            ac = (int)MathF.Ceiling(x * ac / 133f);

            float[,,] r = new float[ni, ns, 3];
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

            float ld;

            for (int c = 0; c < ac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (byte t = 0; t < 3; t++)
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
                                                r[i, s, t] = (r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t] + r[i - 1, s - 1, t] + r[i + 1, s + 1, t] + r[i + 1, s - 1, t] + r[i - 1, s + 1, t]) / 8f;
                                                changePixelWithoutChangingMeanValueBlockMultichannel(t, ld, r[i, s, t], ixx, sxx, r, x, xx);
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
                                            r[i, s, t] = (r[i - 1, s, t] + r[i + 1, s, t]) / 2f;
                                            changePixelWithoutChangingMeanValueBlockMultichannel(t, ld, r[i, s, t], ixx, sxx, r, x, xx);
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
                        for (byte t = 0; t < 3; t++)
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
                                                r[i, s, t] = (r[i - 1, s, t] + r[i + 1, s, t] + r[i, s - 1, t] + r[i, s + 1, t]) / 4f;
                                                changePixelWithoutChangingMeanValueBlockMultichannel(t, ld, r[i, s, t], ixx, sxx, r, x, xx);
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
                                            r[i, s, t] = (r[i, s - 1, t] + r[i, s + 1, t]) / 2f;
                                            changePixelWithoutChangingMeanValueBlockMultichannel(t, ld, r[i, s, t], ixx, sxx, r, x, xx);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ProgressText.Text = (c * 100 / ac).ToString();
            }

            byte[,,] dd = new byte[ni, ns, 3];
            for (byte t = 0; t < 3; t++)
            {
                for (int i = 0; i < ni; i++)
                {
                    for (int s = 0; s < ns; s++)
                    {
                        dd[i, s, t] = Contrast80(r[i, s, t]);
                    }
                }
            }

            return BMPfromRGB(dd, ni, ns);
        }

        private void changePixelWithoutChangingMeanValueBlockMultichannel(byte layer, float oldValue, float newValue, int leftCoordBlock, int topCoordBlock, float[,,] d, int x, int xx)
        {
            float r = oldValue - newValue;
            float dif;
            float nd;
            int cnt = xx;

            if (r > 0.5f)
            {
                for (int i = leftCoordBlock; i < leftCoordBlock + x; i++)
                {
                    for (int s = topCoordBlock; s < topCoordBlock + x; s++)
                    {
                        if (d[i, s, layer] == 255f)
                        {
                            cnt--;
                        }
                    }
                }
                while (r > 0.5f && cnt > 0)
                {
                    dif = r / cnt; // for hypoten?
                    for (int i = leftCoordBlock; i < leftCoordBlock + x; i++)
                    {
                        for (int s = topCoordBlock; s < topCoordBlock + x; s++)
                        {
                            if (d[i, s, layer] < 255f)
                            {
                                nd = d[i, s, layer] + dif;
                                if (nd > 255)
                                {
                                    r -= 255 - d[i, s, layer];
                                    d[i, s, layer] = 255;
                                    cnt--;
                                }
                                else
                                {
                                    d[i, s, layer] = nd;
                                    r -= dif;
                                }
                            }
                        }
                    }
                }
            }
            else if (r < -0.5f)
            {
                for (int i = leftCoordBlock; i < leftCoordBlock + x; i++)
                {
                    for (int s = topCoordBlock; s < topCoordBlock + x; s++)
                    {
                        if (d[i, s, layer] == 0f)
                        {
                            cnt--;
                        }
                    }
                }
                while (r < -0.5f && cnt > 0)
                {
                    dif = r / cnt;
                    for (int i = leftCoordBlock; i < leftCoordBlock + x; i++)
                    {
                        for (int s = topCoordBlock; s < topCoordBlock + x; s++)
                        {
                            if (d[i, s, layer] > 0f)
                            {
                                nd = d[i, s, layer] + dif;
                                if (nd < 0)
                                {
                                    r -= d[i, s, layer];
                                    d[i, s, layer] = 0;
                                    cnt--;
                                }
                                else
                                {
                                    d[i, s, layer] = nd;
                                    r -= dif;
                                }
                            }
                        }
                    }
                }
            }
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
                                                    d[i, s] = S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9f);
                                                }
                                                else
                                                {
                                                    d[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                d[i, s] = S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9f);
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
                                        d[i, s] = S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2f);
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

                                            d[i, s] = S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s] + d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 4f);

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
                                        d[i, s] = S255((d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 2f);
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

        private Bitmap ScaleFurryGray(Image img, int x, int ac) //after ALL, 256monoGray for Furrys and Roughs
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
                                                    d[i, s] = S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9f);
                                                }
                                                else
                                                {
                                                    d[i, s] = (byte)(rnd.Next(0, 2) * 255);
                                                }
                                            }
                                            else
                                            {
                                                d[i, s] = S255((d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + d[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9f);
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
                                        d[i, s] = S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s]) / 2f);
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

                                            d[i, s] = S255((d[i + rnd.Next(-1, 2), s] + d[i + rnd.Next(-1, 2), s] + d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 4f);

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
                            for (int i = ixx; i < ixx + x; i++)
                            {
                                for (int s = sxx; s < sxx + x; s++)
                                {
                                    if (s != 0 && s != nsm)
                                    {
                                        ld = d[i, s];
                                        d[i, s] = S255((d[i, s + rnd.Next(-1, 2)] + d[i, s + rnd.Next(-1, 2)]) / 2f);
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
                for (byte t = 0; t < 3; t++)
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
                                                        dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9f);
                                                    }
                                                    else
                                                    {
                                                        dr[i, s, t] = (byte)(rnd.Next(0, 2) * 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9f);
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
                                            dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s, t] + dr[i + rnd.Next(-1, 2), s, t]) / 2f);
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

                                                dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s, t] + dr[i + rnd.Next(-1, 2), s, t] + dr[i, s + rnd.Next(-1, 2), t] + dr[i, s + rnd.Next(-1, 2), t]) / 4f);

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
                                            dr[i, s, t] = S255((dr[i, s + rnd.Next(-1, 2), t] + dr[i, s + rnd.Next(-1, 2), t]) / 2f);
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
                    for (byte t = 0; t < 3; t++)
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
                        for (byte t = 0; t < 3; t++)
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
                                                        dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 0] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 1] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 2] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9);
                                                    }
                                                    else
                                                    {
                                                        dr[i, s, t] = (byte)(rnd.Next(0, 2) * 255);
                                                    }
                                                }
                                                else
                                                {
                                                    dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 0] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 1] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 2] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + dr[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9);
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
                                            dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s, t] + dr[i + rnd.Next(-1, 2), s, t]) / 2);
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
                        for (byte t = 0; t < 3; t++)
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

                                                dr[i, s, t] = S255((dr[i + rnd.Next(-1, 2), s, t] + dr[i + rnd.Next(-1, 2), s, t] + dr[i, s + rnd.Next(-1, 2), t] + dr[i, s + rnd.Next(-1, 2), t]) / 4);

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
                                            dr[i, s, t] = S255((dr[i, s + rnd.Next(-1, 2), t] + dr[i, s + rnd.Next(-1, 2), t]) / 2);
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

        static byte S255(float v)
        {
            return (byte)(0.000000002833333 * MathF.Pow(v, 5) - 0.00000181137 * MathF.Pow(v, 4) + 0.0003605953 * MathF.Pow(v, 3) - 0.01970911609 * MathF.Pow(v, 2) + 0.63373610992 * v + 0.17238095178);
        }

        static float S255f(float v)
        {
            return (float)(0.000000002833333 * MathF.Pow(v, 5) - 0.00000181137 * MathF.Pow(v, 4) + 0.0003605953 * MathF.Pow(v, 3) - 0.01970911609 * MathF.Pow(v, 2) + 0.63316688184 * v);
        }

        private Bitmap ContrastBoldScaleGray(Image img, int x, int ac) //AFTER ALL, one more time contrast s255f
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
                        ds[i, s] = (sr[ix, sx] / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx, 2)) + sr[ix + 1, sx] / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx, 2)) + sr[ix, sx + 1] / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx - x, 2)) + sr[ix + 1, sx + 1] / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx - x, 2))) / (1 / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx, 2)) + 1 / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx, 2)) + 1 / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx - x, 2)) + 1 / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx - x, 2)));
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
                        for (byte t = 0; t < 3; t++)
                        {
                            rs[i, s, t] = (sr[ix, sx, t] / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx, 2)) + sr[ix + 1, sx, t] / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx, 2)) + sr[ix, sx + 1, t] / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx - x, 2)) + sr[ix + 1, sx + 1, t] / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx - x, 2))) / (1 / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx, 2)) + 1 / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx, 2)) + 1 / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx - x, 2)) + 1 / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx - x, 2)));
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
                    for (byte t = 0; t < 3; t++)
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

            for (int c = 0; c < xoimac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (byte t = 0; t < 3; t++)
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
                                                int ld = r[i, s, t];

                                                if (rnd.Next(0, xoim) > c)   //you can remove this
                                                {                            //to reduce grid structure
                                                    r[i, s, t] = (byte)(S255f((rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 0] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 1] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 2] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9));
                                                }                                          //you can
                                                else                                       //remove this
                                                {                                          //to reduce
                                                    r[i, s, t] = (byte)rnd.Next(0, 256);   //grid
                                                }                                          //structure

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
                        for (byte t = 0; t < 3; t++)
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
            }


            for (int ix = oim; ix > -1; ix--)
            {
                int ixx = ix * x;
                for (int sx = osm; sx > -1; sx--)
                {
                    int sxx = sx * x;
                    for (byte t = 0; t < 3; t++)
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
                                            int ld = r[i, s, t];
                                            r[i, s, t] = (byte)rs[i, s, t];

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
                                            rs[i, s, t] = (rs[i, s, t] * xoimac + r[i, s, t]) / (xoimac + 1);
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
                    for (byte t = 0; t < 3; t++)
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
            ProgressText.Text = "50";

            for (int c = xoimac + 1; c < xoim2; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (byte t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
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
                        for (byte t = 0; t < 3; t++)
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
                        for (byte t = 0; t < 3; t++)
                        {
                            rs[i, s, t] = (sr[ix, sx, t] / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx, 2)) + sr[ix + 1, sx, t] / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx, 2)) + sr[ix, sx + 1, t] / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx - x, 2)) + sr[ix + 1, sx + 1, t] / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx - x, 2))) / (1 / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx, 2)) + 1 / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx, 2)) + 1 / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx - x, 2)) + 1 / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx - x, 2)));
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
                    for (byte t = 0; t < 3; t++)
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

            for (int c = 0; c < xoimac; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (byte t = 0; t < 3; t++)
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
                                                int ld = r[i, s, t];

                                                if (rnd.Next(0, xoim) > c)   //you can remove this
                                                {                            //to reduce grid structure
                                                    r[i, s, t] = (byte)(S255f((rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 0] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 1] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), 2] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t] + rs[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2), t]) / 9));
                                                }                                          //you can
                                                else                                       //remove this
                                                {                                          //to reduce
                                                    r[i, s, t] = (byte)rnd.Next(0, 256);   //grid
                                                }                                          //structure

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
                        for (byte t = 0; t < 3; t++)
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
            }

            for (int ix = oim; ix > -1; ix--)
            {
                int ixx = ix * x;
                for (int sx = osm; sx > -1; sx--)
                {
                    int sxx = sx * x;
                    for (byte t = 0; t < 3; t++)
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
                                            int ld = r[i, s, t];
                                            r[i, s, t] = (byte)rs[i, s, t];

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
                                            rs[i, s, t] = (rs[i, s, t] * xoimac + r[i, s, t]) / (xoimac + 1);
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
                    for (byte t = 0; t < 3; t++)
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
            ProgressText.Text = "50";

            for (int c = xoimac + 1; c < xoim2; c++)
            {
                for (int ix = oim; ix > -1; ix--)
                {
                    int ixx = ix * x;
                    for (int sx = osm; sx > -1; sx--)
                    {
                        int sxx = sx * x;
                        for (byte t = 0; t < 3; t++)
                        {
                            if (sr[ix, sx, t] > 0 && sr[ix, sx, t] < 255)
                            {
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
                        for (byte t = 0; t < 3; t++)
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
                        ds[i, s] = (sr[ix, sx] / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx, 2)) + sr[ix + 1, sx] / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx, 2)) + sr[ix, sx + 1] / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx - x, 2)) + sr[ix + 1, sx + 1] / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx - x, 2))) / (1 / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx, 2)) + 1 / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx, 2)) + 1 / (MathF.Pow(i - ixx, 2) + MathF.Pow(s - sxx - x, 2)) + 1 / (MathF.Pow(i - ixx - x, 2) + MathF.Pow(s - sxx - x, 2)));
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

            for (int c = 0; c < xoimac; c++)
            {
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
                                            ld = d[i, s];

                                            if (rnd.Next(0, xoim) > c)   //you can remove this
                                            {                            //to reduce grid structure
                                                d[i, s] = (byte)(S255f((ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)] + ds[i + rnd.Next(-1, 2), s + rnd.Next(-1, 2)]) / 9));
                                            }                                         //you can
                                            else                                      //remove this
                                            {                                         //to reduce
                                                d[i, s] = (byte)rnd.Next(0, 256);     //grid
                                            }                                         //structure

                                            r = d[i, s] - ld; ////float function!?
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
            }

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
                                        ld = d[i, s];
                                        d[i, s] = (byte)ds[i, s];

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
                                        ds[i, s] = (ds[i, s] * xoimac + d[i, s]) / (xoimac + 1);

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
            ProgressText.Text = "50";

            for (int c = xoimac + 1; c < xoim2; c++)
            {
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

                for (int i = 0; i < ni; i++)
                {
                    for (int s = 0; s < ns; s++)
                    {
                        ds[i, s] = (ds[i, s] * c + d[i, s] + S255f(d[i, s])) / (c + 2);
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
            float x50p, x150p, rex, rex2;
            bool xEven;
            xEven = (x % 2) == 0;
            x50p = (float)x / 2 - 0.5f;
            halfx = x / 2;
            x150p = x * 1.5f - 0.5f;
            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            xm = x - 1;
            byte min, max;
            short[,] r = new short[ni, ns];
            short[,] ri = new short[ni, ns];
            float[,] ris = new float[ni, ns];
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

            float aax = (MathF.Log(x, 0.96f) + 127.5f) * rex / 255; //0.96 - AntiAliasing separate, 0.99 - no AA, separate, 1.01 - average colors
            if (aax < 1) aax = 1;
            float aax2 = aax / 2;
            float maax2 = rex2 - aax2;
            float paax2 = rex2 + aax2;

            float x1, y1, x2, y2, x3, y3, x4, y4, q1, q2, q3, q4;

            int xs = oim / 2, ys = osm / 2, xsp = xs + 1, ysp = ys + 1;
            float xsx = xs * x + x50p;
            float xsxp = xsx + x;
            float ysx = ys * x + x50p;
            float ysxp = ysx + x;
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
                    ri[i, s] = Bilinear(i, s, (float)xsx, (float)ysx, (float)xsxp, (float)ysxp, sr[xs, ys], sr[xsp, ys], sr[xsp, ysp], sr[xs, ysp]);
                }
            });

            for (int ix = xs + 2; ix < oi; ix++)
            {
                int dixxm = ix * x;
                ixxm = dixxm - 1;
                int dixx = (ix + 1) * x;
                float dixxp = dixxm + x50p;

                Parallel.For(dixxm, dixx, i =>
                {
                    for (int s = iysx; s < ys2x; s++)
                    {
                        ri[i, s] = Bilinear(i, s, ixxm, (float)ysx, (float)dixxp, (float)ysxp, (ri[ixxm, iysx] + ri[ixxm, iysxp]) / 2, sr[ix, ys], sr[ix, ys + 1], (ri[ixxm, iysxpi] + ri[ixxm, iysxpp]) / 2);
                    }
                });
            }

            for (int ix = xs - 1; ix > -1; ix--)
            {
                int dixx = ix * x;
                int dixxp = dixx + x;
                float dixxm = dixx + x50p;

                Parallel.For(dixx, dixxp, i =>
                {
                    for (int s = iysx; s < ys2x; s++)
                    {
                        ri[i, s] = Bilinear(i, s, (float)dixxm, (float)ysx, dixxp, (float)ysxp, sr[ix, ys], (ri[dixxp, iysx] + ri[dixxp, iysxp]) / 2, (ri[dixxp, iysxpi] + ri[dixxp, iysxpp]) / 2, sr[ix, ys + 1]);
                    }
                });
            }
            ProgressText.Text = (100 / ac).ToString();
            int xs2x = (xs + 2) * x;
            for (int sx = ys + 2; sx < os; sx++)
            {
                int dsxxm = sx * x;
                sxxm = dsxxm - 1;
                int dsxx = (sx + 1) * x;
                float dsxxp = dsxxm + x50p; 
                Parallel.For(ixsx, xs2x, i =>
                {
                    for (int s = dsxxm; s < dsxx; s++)
                    {
                        ri[i, s] = Bilinear(i, s, (float)xsx, sxxm, (float)xsxp, (float)dsxxp, (ri[ixsx, sxxm] + ri[ixsxp, sxxm]) / 2, (ri[ixsxpi, sxxm] + ri[ixsxpp, sxxm]) / 2, sr[xs + 1, sx], sr[xs, sx]);
                    }
                });
            }

            for (int sx = ys - 1; sx > -1; sx--)
            {
                int dsxx = sx * x;
                sxxm = dsxx + x;
                float dsxxm = dsxx + x50p;

                Parallel.For(ixsx, xs2x, i =>
                {
                    for (int s = dsxx; s < sxxm; s++)
                    {
                        ri[i, s] = Bilinear(i, s, (float)xsx, (float)dsxxm, (float)xsxp, sxxm, sr[xs, sx], sr[xs + 1, sx], (ri[ixsxpi, sxxm] + ri[ixsxpp, sxxm]) / 2, (ri[ixsx, sxxm] + ri[ixsxp, sxxm]) / 2);
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
                    float dsxxp = dsxxm + x50p;
                    int dixxm = ix * x;
                    ixxm = dixxm - 1;
                    int dixx = (ix + 1) * x;
                    int iixx = dixx - 1;
                    float dixxp = dixxm + x50p;
                    Parallel.For(dixxm, dixx, i =>
                    {
                        for (int s = dsxxm; s < dsxx; s++)
                        {
                            ri[i, s] = Bilinear(i, s, ixxm, sxxm, (float)dixxp, (float)dsxxp, ri[ixxm, sxxm], (ri[dixxm, sxxm] + ri[iixx, sxxm]) / 2, sr[ix, sx], (ri[ixxm, dsxxm] + ri[ixxm, isxx]) / 2);
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
                    float dsxxp = dsxxm + x50p;
                    int dixxm = ix * x;
                    ixxm = (ix + 1) * x;
                    int iixx = ixxm - 1;
                    float dixxp = dixxm + x50p;

                    Parallel.For(dixxm, ixxm, i =>
                    {
                        for (int s = dsxxm; s < sxxm; s++)
                        {
                            ri[i, s] = Bilinear(i, s, (float)dixxp, (float)dsxxp, ixxm, sxxm, sr[ix, sx], (ri[ixxm, dsxxm] + ri[ixxm, isxx]) / 2, ri[ixxm, sxxm], (ri[dixxm, sxxm] + ri[iixx, sxxm]) / 2);
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
                    float dsxxp = sxx + x50p;
                    int dixxm = ix * x;
                    int dixx = dixxm + x;
                    int iixx = dixx - 1;
                    float dixxp = dixxm + x50p; 

                    Parallel.For(dixxm, dixx, i =>
                    {
                        for (int s = sxx; s < dsxx; s++)
                        {
                            ri[i, s] = Bilinear(i, s, (float)dixxp, isxxm, dixx, (float)dsxxp, (ri[dixxm, isxxm] + ri[iixx, isxxm]) / 2, ri[dixx, isxxm], (ri[dixx, isxx] + ri[dixx, sxx]) / 2, sr[ix, sx]);
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
                    float dsxxp = sxx + x50p;
                    ixx = ix * x;
                    int dixx = ixx + x;
                    ixxm = ixx - 1;
                    int iixx = dixx - 1;
                    float dixxp = ixx + x50p;

                    Parallel.For(ixx, dixx, i =>
                    {
                        for (int s = sxx; s < dsxx; s++)
                        {
                            ri[i, s] = Bilinear(i, s, ixxm, (float)dsxxp, (float)dixxp, dsxx, (ri[ixxm, sxx] + ri[ixxm, isxx]) / 2, sr[ix, sx], (ri[ixx, dsxx] + ri[iixx, dsxx]) / 2, ri[ixxm, dsxx]);
                        }
                    });
                }
            }
            ProgressText.Text = (1000 / ac).ToString();

            if (ac > 10)
            {
                int nim = ni - 1, nsm = ns - 1;
                float oimx = oim * x + x50p, osmx = osm * x + x50p;
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
                                q4 = (float)(sr[ix, sx] + sr[ix, sx - 1]) / 2;

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
                                x1 += x50p;     
                                y1 = x50p;        //   [1]   2
                                q1 = sr[ix, 0];   //   1/4   3

                                x2 = ixxm;
                                y2 = xm;
                                q3 = r[ixxm, xm];

                                q2 = (r[ixxm, 0] + r[ixxm, xm]) / 2;
                                q4 = (float)(sr[ix, 0] + sr[ix, 1]) / 2;
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
                                r[i, s] = Bilinear(i, s, (float)x1, (float)y1, (float)x2, (float)y2, (float)q1, (float)q2, (float)q3, (float)q4);
                                float dist1 = MathF.Max(nim - i + s - x, 0);
                                float dist2 = MathF.Max(MathF.Abs(ixsxpi - i) + MathF.Abs(iysxpi - s) - x, 0);
                                ri[i, s] = (short)((ri[i, s] * dist1 + r[i, s] * dist2) / (dist1 + dist2) + 0.5);
                                ris[i, s] = MathF.Min(dist1, dist2); 
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
                                    ixx = ix * x;
                                    ixxm = ixx - 1;
                                    x2 = ix * x + x50p;//    1   [2]
                                    y2 = x50p;         //  
                                    q2 = sr[ix, 0];    //    4   2/3

                                    x3 = x2;
                                    y3 = xm;
                                    q3 = (float)(sr[ix, 1] + sr[ix, 0]) / 2;

                                    x1 = ixxm;
                                    y1 = x50p;
                                    q1 = (r[ixxm, 0] + r[ixxm, xm]) / 2;

                                    x4 = ixxm;
                                    y4 = xm;
                                    q4 = r[ixxm, xm];

                                }
                                else
                                {
                                    ixx = ix * x;
                                    ixxm = ixx - 1;
                                    sxx = sx * x;
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
                                    float dist1 = MathF.Max(i + s - x, 0);
                                    ri[i, s] = (short)((ri[i, s] * dist1 + r[i, s] * ris[i, s]) / (dist1 + ris[i, s]) + 0.5);
                                    ris[i, s] = MathF.Min(dist1, ris[i, s]);
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
                                        sxx = sx * x;             //1    [2]
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
                                        ixx = ix * x;
                                        x4 = ix * x + x50p;//   1/4  2
                                        y4 = osmx;         //  
                                        q4 = sr[ix, osm];  //   [4]  3

                                        x3 = ixx + x;
                                        y3 = osmx;
                                        q3 = (r[(int)x3, nsm] + r[(int)x3, nsm - xm]) / 2;

                                        x1 = x4;
                                        y1 = y4 - x50p;
                                        q1 = (float)(sr[ix, osmm] + sr[ix, osm]) / 2;

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
                                        float dist2 = MathF.Max(nim - i + nsm - s - x, 0);
                                        ri[i, s] = (short)((ri[i, s] * dist2 + r[i, s] * ris[i, s]) / (ris[i, s] + dist2) + 0.5);
                                        ris[i, s] = MathF.Min(ris[i, s], dist2); 
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
                                            q2 = (float)(sr[1, sx] + sr[0, sx]) / 2; //allfloat?

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
                                            y2 = ns - x - 0.5f;                              //    1   
                                            q2 = (float)(sr[ix, osmm] + sr[ix, osm]) / 2;  //    4   [3]
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
                                            float dist2 = MathF.Max(i + nsm - s - x, 0);
                                            ri[i, s] = (short)((ri[i, s] * dist2 + r[i, s] * ris[i, s]) / (ris[i, s] + dist2) + 0.5);
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

            return BMPfromGray(rb, ni, ns); //AFTER ALL, visualisation all methods work ac++ video 
        }

        private Bitmap ScaleSeparateColor(Image img, int x, int ac)
        {
            int ni, ns, oi, os, xm, oim, osm, sxx, sxxm, ixx, ixxm, halfx;
            float x50p, x150p;
            bool xEven;
            xEven = (x % 2) == 0;
            x50p = x / 2f - 0.5f;
            halfx = x / 2;
            x150p = x * 1.5f - 0.5f;
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
            float[] rex = new float[3];
            float[] rex2 = new float[3];
            short[,,] r = new short[ni, ns, 3];
            short[,,] ri = new short[ni, ns, 3];
            float[,] ris = new float[ni, ns];
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
                    for (byte t = 0; t < 3; t++)
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

            float aax = (MathF.Log(x, 0.96f) + 127.5f) * (rex[0] + rex[1] + rex[2]) / 765; //0.96 - AntiAliasing separate, 0.99 - no AA, separate, 1.01 - average colors
            if (aax < 1) aax = 1;
            float aax2 = aax / 2;
            float[] rmaax2 = [rex2[0] - aax2, rex2[1] - aax2, rex2[2] - aax2];
            float[] rpaax2 = [rex2[0] + aax2, rex2[1] + aax2, rex2[2] + aax2];

            float x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4;

            int xs = oim / 2, ys = osm / 2, xsp = xs + 1, ysp = ys + 1;
            float xsx = xs * x + x50p;
            float xsxp = xsx + x;
            float ysx = ys * x + x50p;
            float ysxp = ysx + x;
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
                    short[] b3 = Bilinear3(i, s, xsx, ysx, xsxp, ysxp, sr[xs, ys, 0], sr[xsp, ys, 0], sr[xsp, ysp, 0], sr[xs, ysp, 0], sr[xs, ys, 1], sr[xsp, ys, 1], sr[xsp, ysp, 1], sr[xs, ysp, 1], sr[xs, ys, 2], sr[xsp, ys, 2], sr[xsp, ysp, 2], sr[xs, ysp, 2]);
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
                float dixxp = dixxm + x50p;
                Parallel.For(dixxm, dixx, i =>
                {
                    for (int s = iysx; s < ys2x; s++)
                    {
                        short[] b3 = Bilinear3(i, s, ixxm, ysx, dixxp, ysxp, (ri[ixxm, iysx, 0] + ri[ixxm, iysxp, 0]) / 2, sr[ix, ys, 0], sr[ix, ys + 1, 0], (ri[ixxm, iysxpi, 0] + ri[ixxm, iysxpp, 0]) / 2, (ri[ixxm, iysx, 1] + ri[ixxm, iysxp, 1]) / 2, sr[ix, ys, 1], sr[ix, ys + 1, 1], (ri[ixxm, iysxpi, 1] + ri[ixxm, iysxpp, 1]) / 2, (ri[ixxm, iysx, 2] + ri[ixxm, iysxp, 2]) / 2, sr[ix, ys, 2], sr[ix, ys + 1, 2], (ri[ixxm, iysxpi, 2] + ri[ixxm, iysxpp, 2]) / 2);
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
                float dixxm = dixx + x50p;
                Parallel.For(dixx, dixxp, i =>
                {
                    for (int s = iysx; s < ys2x; s++)
                    {
                        short[] b3 = Bilinear3(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, ys, 0], (ri[dixxp, iysx, 0] + ri[dixxp, iysxp, 0]) / 2, (ri[dixxp, iysxpi, 0] + ri[dixxp, iysxpp, 0]) / 2, sr[ix, ys + 1, 0], sr[ix, ys, 1], (ri[dixxp, iysx, 1] + ri[dixxp, iysxp, 1]) / 2, (ri[dixxp, iysxpi, 1] + ri[dixxp, iysxpp, 1]) / 2, sr[ix, ys + 1, 1], sr[ix, ys, 2], (ri[dixxp, iysx, 2] + ri[dixxp, iysxp, 2]) / 2, (ri[dixxp, iysxpi, 2] + ri[dixxp, iysxpp, 2]) / 2, sr[ix, ys + 1, 2]);
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
                float dsxxp = dsxxm + x50p;
                Parallel.For(ixsx, xs2x, i =>
                {
                    for (int s = dsxxm; s < dsxx; s++)
                    {
                        short[] b3 = Bilinear3(i, s, xsx, sxxm, xsxp, dsxxp, (ri[ixsx, sxxm, 0] + ri[ixsxp, sxxm, 0]) / 2, (ri[ixsxpi, sxxm, 0] + ri[ixsxpp, sxxm, 0]) / 2, sr[xs + 1, sx, 0], sr[xs, sx, 0], (ri[ixsx, sxxm, 1] + ri[ixsxp, sxxm, 1]) / 2, (ri[ixsxpi, sxxm, 1] + ri[ixsxpp, sxxm, 1]) / 2, sr[xs + 1, sx, 1], sr[xs, sx, 1], (ri[ixsx, sxxm, 2] + ri[ixsxp, sxxm, 2]) / 2, (ri[ixsxpi, sxxm, 2] + ri[ixsxpp, sxxm, 2]) / 2, sr[xs + 1, sx, 2], sr[xs, sx, 2]);
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
                float dsxxm = dsxx + x50p;

                Parallel.For(ixsx, xs2x, i =>
                {
                    for (int s = dsxx; s < sxxm; s++)
                    {
                        short[] b3 = Bilinear3(i, s, xsx, dsxxm, xsxp, sxxm, sr[xs, sx, 0], sr[xs + 1, sx, 0], (ri[ixsxpi, sxxm, 0] + ri[ixsxpp, sxxm, 0]) / 2, (ri[ixsx, sxxm, 0] + ri[ixsxp, sxxm, 0]) / 2, sr[xs, sx, 1], sr[xs + 1, sx, 1], (ri[ixsxpi, sxxm, 1] + ri[ixsxpp, sxxm, 1]) / 2, (ri[ixsx, sxxm, 1] + ri[ixsxp, sxxm, 1]) / 2, sr[xs, sx, 2], sr[xs + 1, sx, 2], (ri[ixsxpi, sxxm, 2] + ri[ixsxpp, sxxm, 2]) / 2, (ri[ixsx, sxxm, 2] + ri[ixsxp, sxxm, 2]) / 2);
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
                    float dsxxp = dsxxm + x50p;
                    int dixxm = ix * x;
                    ixxm = dixxm - 1;
                    int dixx = (ix + 1) * x;
                    int iixx = dixx - 1;
                    float dixxp = dixxm + x50p;

                    Parallel.For(dixxm, dixx, i =>
                    {
                        for (int s = dsxxm; s < dsxx; s++)
                        {
                            short[] b3 = Bilinear3(i, s, ixxm, sxxm, dixxp, dsxxp, ri[ixxm, sxxm, 0], (ri[dixxm, sxxm, 0] + ri[iixx, sxxm, 0]) / 2, sr[ix, sx, 0], (ri[ixxm, dsxxm, 0] + ri[ixxm, isxx, 0]) / 2, ri[ixxm, sxxm, 1], (ri[dixxm, sxxm, 1] + ri[iixx, sxxm, 1]) / 2, sr[ix, sx, 1], (ri[ixxm, dsxxm, 1] + ri[ixxm, isxx, 1]) / 2, ri[ixxm, sxxm, 2], (ri[dixxm, sxxm, 2] + ri[iixx, sxxm, 2]) / 2, sr[ix, sx, 2], (ri[ixxm, dsxxm, 2] + ri[ixxm, isxx, 2]) / 2);
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
                    float dsxxp = dsxxm + x50p;
                    int dixxm = ix * x;
                    ixxm = (ix + 1) * x;
                    int iixx = ixxm - 1;
                    float dixxp = dixxm + x50p;

                    Parallel.For(dixxm, ixxm, i =>
                    {
                        for (int s = dsxxm; s < sxxm; s++)
                        {
                            short[] b3 = Bilinear3(i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx, 0], (ri[ixxm, dsxxm, 0] + ri[ixxm, isxx, 0]) / 2, ri[ixxm, sxxm, 0], (ri[dixxm, sxxm, 0] + ri[iixx, sxxm, 0]) / 2, sr[ix, sx, 1], (ri[ixxm, dsxxm, 1] + ri[ixxm, isxx, 1]) / 2, ri[ixxm, sxxm, 1], (ri[dixxm, sxxm, 1] + ri[iixx, sxxm, 1]) / 2, sr[ix, sx, 2], (ri[ixxm, dsxxm, 2] + ri[ixxm, isxx, 2]) / 2, ri[ixxm, sxxm, 2], (ri[dixxm, sxxm, 2] + ri[iixx, sxxm, 2]) / 2);
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
                    float dsxxp = sxx + x50p;
                    int dixxm = ix * x;
                    int dixx = dixxm + x;
                    int iixx = dixx - 1;
                    float dixxp = dixxm + x50p;

                    Parallel.For(dixxm, dixx, i =>
                    {
                        for (int s = sxx; s < dsxx; s++)
                        {
                            short[] b3 = Bilinear3(i, s, dixxp, isxxm, dixx, dsxxp, (ri[dixxm, isxxm, 0] + ri[iixx, isxxm, 0]) / 2, ri[dixx, isxxm, 0], (ri[dixx, isxx, 0] + ri[dixx, sxx, 0]) / 2, sr[ix, sx, 0], (ri[dixxm, isxxm, 1] + ri[iixx, isxxm, 1]) / 2, ri[dixx, isxxm, 1], (ri[dixx, isxx, 1] + ri[dixx, sxx, 1]) / 2, sr[ix, sx, 1], (ri[dixxm, isxxm, 2] + ri[iixx, isxxm, 2]) / 2, ri[dixx, isxxm, 2], (ri[dixx, isxx, 2] + ri[dixx, sxx, 2]) / 2, sr[ix, sx, 2]);
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
                    float dsxxp = sxx + x50p;
                    ixx = ix * x;
                    int dixx = ixx + x;
                    ixxm = ixx - 1;
                    int iixx = dixx - 1;
                    float dixxp = ixx + x50p;

                    Parallel.For(ixx, dixx, i =>
                    {
                        for (int s = sxx; s < dsxx; s++)
                        {
                            short[] b3 = Bilinear3(i, s, ixxm, dsxxp, dixxp, dsxx, (ri[ixxm, sxx, 0] + ri[ixxm, isxx, 0]) / 2, sr[ix, sx, 0], (ri[ixx, dsxx, 0] + ri[iixx, dsxx, 0]) / 2, ri[ixxm, dsxx, 0], (ri[ixxm, sxx, 1] + ri[ixxm, isxx, 1]) / 2, sr[ix, sx, 1], (ri[ixx, dsxx, 1] + ri[iixx, dsxx, 1]) / 2, ri[ixxm, dsxx, 1], (ri[ixxm, sxx, 2] + ri[ixxm, isxx, 2]) / 2, sr[ix, sx, 2], (ri[ixx, dsxx, 2] + ri[iixx, dsxx, 2]) / 2, ri[ixxm, dsxx, 2]);
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
                float oimx = oim * x + x50p, osmx = osm * x + x50p;
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
                                rq4 = (float)(sr[ix, sx, 0] + sr[ix, sx - 1, 0]) / 2;
                                gq4 = (float)(sr[ix, sx, 1] + sr[ix, sx - 1, 1]) / 2;
                                bq4 = (float)(sr[ix, sx, 1] + sr[ix, sx - 1, 1]) / 2;

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
                                rq4 = (float)(sr[ix, 0, 0] + sr[ix, 1, 0]) / 2;
                                gq3 = r[ixxm, xm, 1];
                                gq2 = (r[ixxm, 0, 1] + r[ixxm, xm, 1]) / 2;
                                gq4 = (float)(sr[ix, 0, 1] + sr[ix, 1, 1]) / 2;
                                bq3 = r[ixxm, xm, 2];
                                bq2 = (r[ixxm, 0, 2] + r[ixxm, xm, 2]) / 2;
                                bq4 = (float)(sr[ix, 0, 2] + sr[ix, 1, 2]) / 2;
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
                                short[] b3 = Bilinear3(i, s, x1, y1, x2, y2, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1]; //AFTER ALL, nupkg .NET Standart 2.0, application & project
                                r[i, s, 2] = b3[2];
                                float dist1 = MathF.Max(nim - i + s - x, 0);
                                float dist2 = MathF.Max(MathF.Abs(ixsxpi - i) + MathF.Abs(iysxpi - s) - x, 0);
                                float sdist = dist1 + dist2;
                                float dis1sdist = dist1 / sdist;
                                float dis2sdist = dist2 / sdist;
                                ri[i, s, 0] = (short)(ri[i, s, 0] * dis1sdist + r[i, s, 0] * dis2sdist + 0.5);
                                ri[i, s, 1] = (short)(ri[i, s, 1] * dis1sdist + r[i, s, 1] * dis2sdist + 0.5);
                                ri[i, s, 2] = (short)(ri[i, s, 2] * dis1sdist + r[i, s, 2] * dis2sdist + 0.5);
                                ris[i, s] = MathF.Min(dist1, dist2);
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
                                    ixx = ix * x;
                                    ixxm = ixx - 1;
                                    x2 = ix * x + x50p;    //    1   [2]
                                    y2 = x50p;             //  
                                    rq2 = sr[ix, 0, 0];    //    4   2/3
                                    gq2 = sr[ix, 0, 1];
                                    bq2 = sr[ix, 0, 2];

                                    x3 = x2;
                                    y3 = xm;
                                    rq3 = (float)(sr[ix, 1, 0] + sr[ix, 0, 0]) / 2;
                                    gq3 = (float)(sr[ix, 1, 1] + sr[ix, 0, 1]) / 2;
                                    bq3 = (float)(sr[ix, 1, 2] + sr[ix, 0, 2]) / 2;

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
                                    ixx = ix * x;
                                    ixxm = ixx - 1;
                                    sxx = sx * x;
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
                                    short[] b3 = Quadrilateral3(i, s, x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4);

                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];
                                    float dist1 = MathF.Max(i + s - x, 0);
                                    float sdist = dist1 + ris[i, s];
                                    float risdist = ris[i, s] / sdist;
                                    float disdist = dist1 / sdist;
                                    ri[i, s, 0] = (short)(ri[i, s, 0] * disdist + r[i, s, 0] * risdist + 0.5);
                                    ri[i, s, 1] = (short)(ri[i, s, 1] * disdist + r[i, s, 1] * risdist + 0.5);
                                    ri[i, s, 2] = (short)(ri[i, s, 2] * disdist + r[i, s, 2] * risdist + 0.5);

                                    ris[i, s] = MathF.Min(dist1, ris[i, s]);
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
                                        sxx = sx * x;             //1    [2]
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
                                        ixx = ix * x;
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
                                        rq1 = (float)(sr[ix, osmm, 0] + sr[ix, osm, 0]) / 2;
                                        gq1 = (float)(sr[ix, osmm, 1] + sr[ix, osm, 1]) / 2;
                                        bq1 = (float)(sr[ix, osmm, 2] + sr[ix, osm, 2]) / 2;

                                        x2 = x3;
                                        y2 = y1;
                                        rq2 = r[(int)x2, (int)y2, 0];
                                        gq2 = r[(int)x2, (int)y2, 1];
                                        bq2 = r[(int)x2, (int)y2, 2];
                                    }
                                    else
                                    {
                                        x4 = ix * x;                    
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
                                        short[] b3 = Quadrilateral3(i, s, x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4);
                                        r[i, s, 0] = b3[0];
                                        r[i, s, 1] = b3[1];
                                        r[i, s, 2] = b3[2];
                                        float dist2 = MathF.Max(nim - i + nsm - s - x, 0);
                                        float sdist = ris[i, s] + dist2;
                                        float dist2sdist = dist2 / sdist;
                                        float rissdist = ris[i, s] / sdist;
                                        ri[i, s, 0] = (short)(ri[i, s, 0] * dist2sdist + r[i, s, 0] * rissdist + 0.5);
                                        ri[i, s, 1] = (short)(ri[i, s, 1] * dist2sdist + r[i, s, 1] * rissdist + 0.5);
                                        ri[i, s, 2] = (short)(ri[i, s, 2] * dist2sdist + r[i, s, 2] * rissdist + 0.5);
                                        ris[i, s] = MathF.Min(ris[i, s], dist2);
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
                                            rq2 = (float)(sr[1, sx, 0] + sr[0, sx, 0]) / 2;
                                            gq2 = (float)(sr[1, sx, 1] + sr[0, sx, 1]) / 2;
                                            bq2 = (float)(sr[1, sx, 2] + sr[0, sx, 2]) / 2;

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
                                            x2 += x50p;                                          //        2/3
                                            y2 = ns - x - 0.5f;                                  //    1   
                                            rq2 = (float)(sr[ix, osmm, 0] + sr[ix, osm, 0]) / 2; //    4   [3]
                                            gq2 = (float)(sr[ix, osmm, 1] + sr[ix, osm, 1]) / 2;
                                            bq2 = (float)(sr[ix, osmm, 2] + sr[ix, osm, 2]) / 2;

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
                                            short[] b3 = Quadrilateral3(i, s, x1, y1, x2, y2, x3, y3, x4, y4, rq1, rq2, rq3, rq4, gq1, gq2, gq3, gq4, bq1, bq2, bq3, bq4);
                                            r[i, s, 0] = b3[0];
                                            r[i, s, 1] = b3[1];
                                            r[i, s, 2] = b3[2];
                                            float dist2 = MathF.Max(i + nsm - s - x, 0);
                                            float sdist = ris[i, s] + dist2;
                                            float disdist = dist2 / sdist;
                                            float risdist = ris[i, s] / sdist;
                                            ri[i, s, 0] = (short)(ri[i, s, 0] * disdist + r[i, s, 0] * risdist + 0.5);
                                            ri[i, s, 1] = (short)(ri[i, s, 1] * disdist + r[i, s, 1] * risdist + 0.5);
                                            ri[i, s, 2] = (short)(ri[i, s, 2] * disdist + r[i, s, 2] * risdist + 0.5);
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
                    for (byte t = 0; t < 3; t++)
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

        static short[] Quadrilateral3(int x, int y, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, float rq1, float rq2, float rq3, float rq4, float gq1, float gq2, float gq3, float gq4, float bq1, float bq2, float bq3, float bq4)
        {
            float r1, r2, ry1, ry2, r12, r21, r34, r43, d21, d34, dry21;
            d21 = x2 - x1;
            d34 = x3 - x4;
            r21 = (x2 - x) / d21;
            r12 = (x - x1) / d21;
            r43 = (x - x4) / d34;
            r34 = (x3 - x) / d34;
            ry1 = r21 * y1 + r12 * y2;
            ry2 = r34 * y4 + r43 * y3;
            dry21 = ry2 - ry1; 
            float ry2y = (ry2 - y) / dry21;
            float yry1 = (y - ry1) / dry21;

            short[] res = new short[3];
            r1 = r21 * rq1 + r12 * rq2;
            r2 = r34 * rq4 + r43 * rq3;
            res[0] = (short)(ry2y * r1 + yry1 * r2 + 0.5);
            /*if (float.IsNaN(res[0])) //в каком случае - оптимизировать? dry21?
            {
                res[0] = (short)((r1 + r2) / 2 + 0.5);
            }*/

            r1 = r21 * gq1 + r12 * gq2;
            r2 = r34 * gq4 + r43 * gq3;
            res[1] = (short)(ry2y * r1 + yry1 * r2 + 0.5);

            r1 = r21 * bq1 + r12 * bq2;
            r2 = r34 * bq4 + r43 * bq3;
            res[2] = (short)(ry2y * r1 + yry1 * r2 + 0.5);

            return res;
        }

        static short Quadrilateral(int x, int y, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, float q1, float q2, float q3, float q4)
        {
            float r1, r2, ry1, ry2, r12, r21, r34, r43, d21, d34, dry21;
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
            /*if (float.IsNaN((ry2 - y) / dry21 * r1 + (y - ry1) / dry21 * r2)) ///why?
            {
                return (short)((r1 + r2) / 2 + 0.5);
            }*/
            return (short)((ry2 - y) / dry21 * r1 + (y - ry1) / dry21 * r2 + 0.5);
        }

        static short Bilinear(int x, int y, float x1, float y1, float x2, float y2, float q1, float q2, float q3, float q4)
        {
            float r1, r2, r12, r21, d21, y21;
            d21 = x2 - x1;
            r21 = x2 - x;
            r12 = x - x1;
            r1 = r21 * q1 + r12 * q2;
            r2 = r21 * q4 + r12 * q3;
            y21 = y2 - y1;
            return (short)(((y2 - y) / y21 * r1 + (y - y1) / y21 * r2) / d21 + 0.5);
        }

        static short[] Bilinear3(int x, int y, float x1, float y1, float x2, float y2, float q1r, float q2r, float q3r, float q4r, float q1g, float q2g, float q3g, float q4g, float q1b, float q2b, float q3b, float q4b)
        {
            float r1r, r2r, r1g, r2g, r1b, r2b, r12, r21, d21, y21;
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
            float y2yy21 = (y2 - y) / y21;
            float yy1y21 = (y - y1) / y21;
            return [(short)(y2yy21 * r1r + yy1y21 * r2r + 0.5), (short)(y2yy21 * r1g + yy1y21 * r2g + 0.5), (short)(y2yy21 * r1b + yy1y21 * r2b + 0.5)];
        }


        private Bitmap ScaleBilinearApproximationGray(Image img, int x, int ac)//after nupkg, application and project
        {
            if (ac < 1) ac = 1;
            int ni, ns, oi, os, xm, oim, osm, sxx, sxxm, ixx, ixxm;
            float halfx = x / 2f;
            float x50p = halfx - 0.5f;

            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            xm = x - 1;
            short[,] r = new short[ni, ns];
            short[,] ri = new short[ni, ns];
            float[,] ris = new float[ni, ns];
            byte[,] sr = GrayFromBMP(img, 0, 0, 0, 0, oi, os);

            int ki = (int)MathF.Ceiling(oim * ac / 100f);
            int ks = (int)MathF.Ceiling(osm * ac / 100f);
            int oiki = (int)(oim / (float)ki + 0.5f);

            const float k = -4f; //+8 - only Gibbs ringing, 0 - save derivative with Gibbs ringing; -8 - save mean value 
            for (int kki = 0; kki < ki * oiki; kki += oiki)//after ALL, -4 sub +8/2??? as clear Gibbs ringing???
            {
                ProgressText.Text = (kki * 100 / oim).ToString();
                for (int kks = 0; kks < ks * oiki; kks += oiki)
                {
                    int ixsx = kki * x;
                    int iysx = kks * x;
                    float xsx = ixsx + x50p;
                    float xsxp = xsx + x;
                    float ysx = iysx + x50p;
                    float ysxp = ysx + x;

                    int iysxp = iysx + xm;
                    int iysxpi = iysxp + 1;
                    int iysxpp = iysxpi + xm;
                    int ys2x = iysxpp + 1;
                    int ixsxp = ixsx + xm;
                    int ixsxpi = ixsxp + 1;
                    float cx = ixsxp + 0.5f;
                    float cy = iysxp + 0.5f;
                    int ixsxpp = ixsxpi + xm;
                    int xsp = kki + 1, ysp = kks + 1;

                    Parallel.For(ixsx, (kki + 2) * x, i => //center
                    {
                        for (int s = iysx; s < ys2x; s++)
                        {
                            r[i, s] = Bilinear(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks], sr[xsp, kks], sr[xsp, ysp], sr[kki, ysp]);
                            float dist = MathF.Pow(Dist4(i, s, cx, cy, halfx), k);
                            float distris = dist + ris[i, s];
                            ri[i, s] = (short)((r[i, s] * dist + ri[i, s] * ris[i, s]) / (dist + ris[i, s]) + 0.5f);
                            ris[i, s] = distris;
                        }
                    });

                    for (int ix = kki + 2; ix < oi; ix++) //right
                    {
                        int dixxm = ix * x;
                        ixxm = dixxm - 1;
                        int dixx = (ix + 1) * x;
                        float dixxp = dixxm + x50p;

                        Parallel.For(dixxm, dixx, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                r[i, s] = Bilinear(i, s, ixxm, ysx, dixxp, ysxp, (r[ixxm, iysx] + r[ixxm, iysxp]) / 2, sr[ix, kks], sr[ix, kks + 1], (r[ixxm, iysxpi] + r[ixxm, iysxpp]) / 2);
                                float dist = MathF.Pow(Dist2Right(i, s, cx, cy, halfx), k);
                                float distris = dist + ris[i, s];
                                ri[i, s] = (short)((r[i, s] * dist + ri[i, s] * ris[i, s]) / distris + 0.5f);
                                ris[i, s] = distris;
                            }
                        });
                    }

                    for (int ix = kki - 1; ix > -1; ix--) //left
                    {
                        int dixx = ix * x;
                        int dixxp = dixx + x;
                        float dixxm = dixx + x50p;

                        Parallel.For(dixx, dixxp, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                r[i, s] = Bilinear(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, kks], (r[dixxp, iysx] + r[dixxp, iysxp]) / 2, (r[dixxp, iysxpi] + r[dixxp, iysxpp]) / 2, sr[ix, kks + 1]);
                                float dist = MathF.Pow(Dist2Left(i, s, cx, cy, halfx), k);
                                float distris = dist + ris[i, s];
                                ri[i, s] = (short)((r[i, s] * dist + ri[i, s] * ris[i, s]) / distris + 0.5f);
                                ris[i, s] = distris;
                            }
                        });
                    }

                    int xs2x = ixsxpp + 1;
                    for (int sx = kks + 2; sx < os; sx++) //bottom
                    {
                        int dsxxm = sx * x;
                        sxxm = dsxxm - 1;
                        int dsxx = (sx + 1) * x;
                        float dsxxp = dsxxm + x50p;

                        Parallel.For(ixsx, xs2x, i =>
                        {
                            for (int s = dsxxm; s < dsxx; s++)
                            {
                                r[i, s] = Bilinear(i, s, xsx, sxxm, xsxp, dsxxp, (r[ixsx, sxxm] + r[ixsxp, sxxm]) / 2, (r[ixsxpi, sxxm] + r[ixsxpp, sxxm]) / 2, sr[kki + 1, sx], sr[kki, sx]);
                                float dist = MathF.Pow(Dist2Bottom(i, s, cx, cy, halfx), k);
                                float distris = dist + ris[i, s];
                                ri[i, s] = (short)((r[i, s] * dist + ri[i, s] * ris[i, s]) / distris + 0.5f);
                                ris[i, s] = distris;
                            }
                        });
                    }

                    for (int sx = kks - 1; sx > -1; sx--) //top
                    {
                        int dsxx = sx * x;
                        sxxm = dsxx + x;
                        float dsxxm = dsxx + x50p;

                        Parallel.For(ixsx, xs2x, i =>
                        {
                            for (int s = dsxx; s < sxxm; s++)
                            {
                                r[i, s] = Bilinear(i, s, xsx, dsxxm, xsxp, sxxm, sr[kki, sx], sr[xsp, sx], (r[ixsxpi, sxxm] + r[ixsxpp, sxxm]) / 2, (r[ixsx, sxxm] + r[ixsxp, sxxm]) / 2);
                                float dist = MathF.Pow(Dist2Top(i, s, cx, cy, halfx), k);
                                float distris = dist + ris[i, s];
                                ri[i, s] = (short)((r[i, s] * dist + ri[i, s] * ris[i, s]) / distris + 0.5f);
                                ris[i, s] = distris;
                            }
                        });
                    }
                    float cxcyx = cx + x + cy;
                    for (int sx = kks + 2; sx < os; sx++) //rightbottom
                    {
                        for (int ix = kki + 2; ix < oi; ix++)
                        {
                            int dsxxm = sx * x;
                            sxxm = dsxxm - 1;
                            int dsxx = (sx + 1) * x;
                            int isxx = dsxx - 1;
                            float dsxxp = dsxxm + x50p;
                            int dixxm = ix * x;
                            ixxm = dixxm - 1;
                            int dixx = (ix + 1) * x;
                            int iixx = dixx - 1;
                            float dixxp = dixxm + x50p;

                            Parallel.For(dixxm, dixx, i =>
                            {
                                for (int s = dsxxm; s < dsxx; s++)
                                {
                                    r[i, s] = Bilinear(i, s, ixxm, sxxm, dixxp, dsxxp, r[ixxm, sxxm], (r[dixxm, sxxm] + r[iixx, sxxm]) / 2, sr[ix, sx], (r[ixxm, dsxxm] + r[ixxm, isxx]) / 2);
                                    float dist = MathF.Pow(i - cxcyx + s, k);
                                    float distris = dist + ris[i, s];
                                    ri[i, s] = (short)((r[i, s] * dist + ri[i, s] * ris[i, s]) / distris + 0.5f);
                                    ris[i, s] = distris;
                                }
                            });
                        }
                    }

                    cxcyx = cx - x + cy;
                    for (int sx = kks - 1; sx > -1; sx--) //lefttop
                    {
                        for (int ix = kki - 1; ix > -1; ix--)
                        {
                            int dsxxm = sx * x;
                            sxxm = (sx + 1) * x;
                            int isxx = sxxm - 1;
                            float dsxxp = dsxxm + x50p;
                            int dixxm = ix * x;
                            ixxm = (ix + 1) * x;
                            int iixx = ixxm - 1;
                            float dixxp = dixxm + x50p;

                            Parallel.For(dixxm, ixxm, i =>
                            {
                                for (int s = dsxxm; s < sxxm; s++)
                                {
                                    r[i, s] = Bilinear(i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx], (r[ixxm, dsxxm] + r[ixxm, isxx]) / 2, r[ixxm, sxxm], (r[dixxm, sxxm] + r[iixx, sxxm]) / 2);
                                    float dist = MathF.Pow(cxcyx - i - s, k);
                                    float distris = dist + ris[i, s];
                                    ri[i, s] = (short)((r[i, s] * dist + ri[i, s] * ris[i, s]) / distris + 0.5f);
                                    ris[i, s] = distris;
                                }
                            });
                        }
                    }

                    cxcyx = cx - x - cy;
                    for (int sx = kks + 2; sx < os; sx++) //leftbottom
                    {
                        for (int ix = kki - 1; ix > -1; ix--)
                        {
                            sxx = sx * x;
                            int isxxm = sxx - 1;
                            int dsxx = (sx + 1) * x;
                            int isxx = dsxx - 1;
                            float dsxxp = sxx + x50p;
                            int dixxm = ix * x;
                            int dixx = dixxm + x;
                            int iixx = dixx - 1;
                            float dixxp = dixxm + x50p;

                            Parallel.For(dixxm, dixx, i =>
                            {
                                for (int s = sxx; s < dsxx; s++)
                                {
                                    r[i, s] = Bilinear(i, s, dixxp, isxxm, dixx, dsxxp, (r[dixxm, isxxm] + r[iixx, isxxm]) / 2, r[dixx, isxxm], (r[dixx, isxx] + r[dixx, sxx]) / 2, sr[ix, sx]);
                                    float dist = MathF.Pow(cxcyx - i + s, k);
                                    float distris = dist + ris[i, s];
                                    ri[i, s] = (short)((r[i, s] * dist + ri[i, s] * ris[i, s]) / distris + 0.5f);
                                    ris[i, s] = distris;
                                }
                            });
                        }
                    }

                    cxcyx = cy - x - cx;
                    for (int ix = kki + 2; ix < oi; ix++) //righttop
                    {
                        for (int sx = kks - 1; sx > -1; sx--)
                        {
                            sxx = sx * x;
                            int dsxx = sxx + x;
                            int isxx = dsxx - 1;
                            float dsxxp = sxx + x50p;
                            ixx = ix * x;
                            int dixx = ixx + x;
                            ixxm = ixx - 1;
                            int iixx = dixx - 1;
                            float dixxp = ixx + x50p;

                            Parallel.For(ixx, dixx, i =>
                            {
                                for (int s = sxx; s < dsxx; s++)
                                {
                                    r[i, s] = Bilinear(i, s, ixxm, dsxxp, dixxp, dsxx, (r[ixxm, sxx] + r[ixxm, isxx]) / 2, sr[ix, sx], (r[ixx, dsxx] + r[iixx, dsxx]) / 2, r[ixxm, dsxx]);
                                    float dist = MathF.Pow(cxcyx + i - s, k);
                                    float distris = dist + ris[i, s];
                                    ri[i, s] = (short)((r[i, s] * dist + ri[i, s] * ris[i, s]) / distris + 0.5f);
                                    ris[i, s] = distris;
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
                    if (ri[i, s] < 0.5f)
                        rb[i, s] = 0;
                    else if (ri[i, s] > 254.5) //after optimization, < 25 interp ? > 172 ??? 
                        rb[i, s] = 255;//after optimization, k=-8;k=4?*2??????? unite at 127.5
                    else
                        rb[i, s] = (byte)(ri[i, s] + 0.5f);
                }
            });

            return BMPfromGray(rb, ni, ns);
        }

        private Bitmap ScaleBilinearApproximationColor(Image img, int x, int ac)
        {
            if (ac < 1) ac = 1;
            int ni, ns, oi, os, xm, oim, osm, sxx, sxxm, ixx, ixxm;
            float halfx = (float)x / 2;
            float x50p = halfx - 0.5f;

            oi = img.Width;
            os = img.Height;
            ni = oi * x;
            ns = os * x;
            oim = oi - 1;
            osm = os - 1;
            xm = x - 1;
            short[,,] r = new short[ni, ns, 3];
            short[,,] ri = new short[ni, ns, 3];
            float[,] ris = new float[ni, ns];
            byte[,,] sr = RGBfromBMP(img, 0, 0, 0, 0, oi, os);

            float xs = oim / 2f, ys = osm / 2f;
            float nxs = (ni - 1) / 2f, nys = (ns - 1) / 2f;

            int ki = (int)MathF.Ceiling(oim * ac / 100f);
            int ks = (int)MathF.Ceiling(osm * ac / 100f);
            int oiki = (int)(oim / (float)ki + 0.5f);
                        
            for (int kki = 0; kki < ki * oiki; kki += oiki)
            {
                ProgressText.Text = ProgressText.Text = (kki * 100 / oim).ToString();
                for (int kks = 0; kks < ks * oiki; kks += oiki)
                {
                    float xsx = kki * x + x50p;
                    float xsxp = xsx + x;
                    float ysx = kks * x + x50p;
                    float ysxp = ysx + x;
                    int ys2x = (kks + 2) * x;

                    int iysx = kks * x;
                    int iysxp = iysx + xm;
                    int iysxpi = iysxp + 1;
                    int iysxpp = iysxpi + xm;

                    int ixsx = kki * x;
                    int ixsxp = ixsx + xm;
                    int ixsxpi = ixsxp + 1;
                    float cx = ixsxp + 0.5f;
                    float cy = iysxp + 0.5f;
                    int ixsxpp = ixsxpi + xm;
                    int xsp = kki + 1, ysp = kks + 1;

                    Parallel.For(ixsx, (kki + 2) * x, i =>
                    {
                        for (int s = iysx; s < ys2x; s++)
                        {
                            short[] b3 = Bilinear3(i, s, xsx, ysx, xsxp, ysxp, sr[kki, kks, 0], sr[xsp, kks, 0], sr[xsp, ysp, 0], sr[kki, ysp, 0], sr[kki, kks, 1], sr[xsp, kks, 1], sr[xsp, ysp, 1], sr[kki, ysp, 1], sr[kki, kks, 2], sr[xsp, kks, 2], sr[xsp, ysp, 2], sr[kki, ysp, 2]);
                            r[i, s, 0] = b3[0];
                            r[i, s, 1] = b3[1];
                            r[i, s, 2] = b3[2];
                            float dist = 1 / MathF.Pow(Dist4(i, s, cx, cy, halfx), 4);//
                            float sdist = (float)(dist + ris[i, s]);
                            float risdist = ris[i, s] / sdist;
                            float disdist = dist / sdist;
                            ri[i, s, 0] = (short)(r[i, s, 0] * disdist + ri[i, s, 0] * risdist + 0.5f);
                            ri[i, s, 1] = (short)(r[i, s, 1] * disdist + ri[i, s, 1] * risdist + 0.5f);
                            ri[i, s, 2] = (short)(r[i, s, 2] * disdist + ri[i, s, 2] * risdist + 0.5f);
                            ris[i, s] = sdist;
                        }
                    });

                    for (int ix = kki + 2; ix < oi; ix++)
                    {
                        int dixxm = ix * x;
                        ixxm = dixxm - 1;
                        int dixx = (ix + 1) * x;
                        float dixxp = dixxm + x50p;

                        Parallel.For(dixxm, dixx, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                short[] b3 = Bilinear3(i, s, ixxm, ysx, dixxp, ysxp, (r[ixxm, iysx, 0] + r[ixxm, iysxp, 0]) / 2, sr[ix, kks, 0], sr[ix, kks + 1, 0], (r[ixxm, iysxpi, 0] + r[ixxm, iysxpp, 0]) / 2, (r[ixxm, iysx, 1] + r[ixxm, iysxp, 1]) / 2, sr[ix, kks, 1], sr[ix, kks + 1, 1], (r[ixxm, iysxpi, 1] + r[ixxm, iysxpp, 1]) / 2, (r[ixxm, iysx, 2] + r[ixxm, iysxp, 2]) / 2, sr[ix, kks, 2], sr[ix, kks + 1, 2], (r[ixxm, iysxpi, 2] + r[ixxm, iysxpp, 2]) / 2);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1];
                                r[i, s, 2] = b3[2];
                                float dist = 1 / MathF.Pow(Dist4(i, s, cx, cy, halfx), 4);
                                float sdist = (float)(dist + ris[i, s]);
                                float risdist = ris[i, s] / sdist;
                                float disdist = dist / sdist;
                                ri[i, s, 0] = (short)(r[i, s, 0] * disdist + ri[i, s, 0] * risdist + 0.5f);
                                ri[i, s, 1] = (short)(r[i, s, 1] * disdist + ri[i, s, 1] * risdist + 0.5f);
                                ri[i, s, 2] = (short)(r[i, s, 2] * disdist + ri[i, s, 2] * risdist + 0.5f);
                                ris[i, s] = sdist;
                            }
                        });
                    }

                    for (int ix = kki - 1; ix > -1; ix--)
                    {
                        int dixx = ix * x;
                        int dixxp = dixx + x;
                        float dixxm = dixx + x50p;

                        Parallel.For(dixx, dixxp, i =>
                        {
                            for (int s = iysx; s < ys2x; s++)
                            {
                                short[] b3 = Bilinear3(i, s, dixxm, ysx, dixxp, ysxp, sr[ix, kks, 0], (r[dixxp, iysx, 0] + r[dixxp, iysxp, 0]) / 2, (r[dixxp, iysxpi, 0] + r[dixxp, iysxpp, 0]) / 2, sr[ix, kks + 1, 1], sr[ix, kks, 1], (r[dixxp, iysx, 1] + r[dixxp, iysxp, 1]) / 2, (r[dixxp, iysxpi, 1] + r[dixxp, iysxpp, 1]) / 2, sr[ix, kks + 1, 1], sr[ix, kks, 2], (r[dixxp, iysx, 2] + r[dixxp, iysxp, 2]) / 2, (r[dixxp, iysxpi, 2] + r[dixxp, iysxpp, 2]) / 2, sr[ix, kks + 1, 2]);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1];
                                r[i, s, 2] = b3[2];
                                float dist = 1 / MathF.Pow(Dist4(i, s, cx, cy, halfx), 4);
                                float sdist = (float)(dist + ris[i, s]);
                                float risdist = ris[i, s] / sdist;
                                float disdist = dist / sdist;
                                ri[i, s, 0] = (short)(r[i, s, 0] * disdist + ri[i, s, 0] * risdist + 0.5f);
                                ri[i, s, 1] = (short)(r[i, s, 1] * disdist + ri[i, s, 1] * risdist + 0.5f);
                                ri[i, s, 2] = (short)(r[i, s, 2] * disdist + ri[i, s, 2] * risdist + 0.5f);
                                ris[i, s] = sdist;
                            }
                        });
                    }

                    int xs2x = (kki + 2) * x;
                    for (int sx = kks + 2; sx < os; sx++)
                    {
                        int dsxxm = sx * x;
                        sxxm = dsxxm - 1;
                        int dsxx = (sx + 1) * x;
                        float dsxxp = dsxxm + x50p;

                        Parallel.For(ixsx, xs2x, i =>
                        {
                            for (int s = dsxxm; s < dsxx; s++)
                            {
                                short[] b3 = Bilinear3(i, s, xsx, sxxm, xsxp, dsxxp, (r[ixsx, sxxm, 0] + r[ixsxp, sxxm, 0]) / 2, (r[ixsxpi, sxxm, 0] + r[ixsxpp, sxxm, 0]) / 2, sr[kki + 1, sx, 0], sr[kki, sx, 0], (r[ixsx, sxxm, 1] + r[ixsxp, sxxm, 1]) / 2, (r[ixsxpi, sxxm, 1] + r[ixsxpp, sxxm, 1]) / 2, sr[kki + 1, sx, 1], sr[kki, sx, 1], (r[ixsx, sxxm, 2] + r[ixsxp, sxxm, 2]) / 2, (r[ixsxpi, sxxm, 2] + r[ixsxpp, sxxm, 2]) / 2, sr[kki + 1, sx, 2], sr[kki, sx, 2]);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1];
                                r[i, s, 2] = b3[2];
                                float dist = 1 / MathF.Pow(Dist4(i, s, cx, cy, halfx), 4);
                                float sdist = (float)(dist + ris[i, s]);
                                float risdist = ris[i, s] / sdist;
                                float disdist = dist / sdist;
                                ri[i, s, 0] = (short)(r[i, s, 0] * disdist + ri[i, s, 0] * risdist + 0.5f);
                                ri[i, s, 1] = (short)(r[i, s, 1] * disdist + ri[i, s, 1] * risdist + 0.5f);
                                ri[i, s, 2] = (short)(r[i, s, 2] * disdist + ri[i, s, 2] * risdist + 0.5f);
                                ris[i, s] = sdist;
                            }
                        });
                    }

                    for (int sx = kks - 1; sx > -1; sx--)
                    {
                        int dsxx = sx * x;
                        sxxm = dsxx + x;
                        float dsxxm = dsxx + x50p;

                        Parallel.For(ixsx, xs2x, i =>
                        {
                            for (int s = dsxx; s < sxxm; s++)
                            {
                                short[] b3 = Bilinear3(i, s, xsx, dsxxm, xsxp, sxxm, sr[kki, sx, 0], sr[xsp, sx, 0], (r[ixsxpi, sxxm, 0] + r[ixsxpp, sxxm, 0]) / 2, (r[ixsx, sxxm, 0] + r[ixsxp, sxxm, 0]) / 2, sr[kki, sx, 1], sr[xsp, sx, 1], (r[ixsxpi, sxxm, 1] + r[ixsxpp, sxxm, 1]) / 2, (r[ixsx, sxxm, 1] + r[ixsxp, sxxm, 1]) / 2, sr[kki, sx, 2], sr[xsp, sx, 2], (r[ixsxpi, sxxm, 2] + r[ixsxpp, sxxm, 2]) / 2, (r[ixsx, sxxm, 2] + r[ixsxp, sxxm, 2]) / 2);
                                r[i, s, 0] = b3[0];
                                r[i, s, 1] = b3[1];
                                r[i, s, 2] = b3[2];
                                float dist = 1 / MathF.Pow(Dist4(i, s, cx, cy, halfx), 4);
                                float sdist = (float)(dist + ris[i, s]);
                                float risdist = ris[i, s] / sdist;
                                float disdist = dist / sdist;
                                ri[i, s, 0] = (short)(r[i, s, 0] * disdist + ri[i, s, 0] * risdist + 0.5f);
                                ri[i, s, 1] = (short)(r[i, s, 1] * disdist + ri[i, s, 1] * risdist + 0.5f);
                                ri[i, s, 2] = (short)(r[i, s, 2] * disdist + ri[i, s, 2] * risdist + 0.5f);
                                ris[i, s] = sdist;
                            }
                        });
                    }

                    for (int sx = kks + 2; sx < os; sx++)
                    {
                        for (int ix = kki + 2; ix < oi; ix++)
                        {
                            int dsxxm = sx * x;
                            sxxm = dsxxm - 1;
                            int dsxx = (sx + 1) * x;
                            int isxx = dsxx - 1;
                            float dsxxp = dsxxm + x50p;
                            int dixxm = ix * x;
                            ixxm = dixxm - 1;
                            int dixx = (ix + 1) * x;
                            int iixx = dixx - 1;
                            float dixxp = dixxm + x50p;

                            Parallel.For(dixxm, dixx, i =>
                            {
                                for (int s = dsxxm; s < dsxx; s++)
                                {
                                    short[] b3 = Bilinear3(i, s, ixxm, sxxm, dixxp, dsxxp, r[ixxm, sxxm, 0], (r[dixxm, sxxm, 0] + r[iixx, sxxm, 0]) / 2, sr[ix, sx, 0], (r[ixxm, dsxxm, 0] + r[ixxm, isxx, 0]) / 2, r[ixxm, sxxm, 1], (r[dixxm, sxxm, 1] + r[iixx, sxxm, 1]) / 2, sr[ix, sx, 1], (r[ixxm, dsxxm, 1] + r[ixxm, isxx, 1]) / 2, r[ixxm, sxxm, 2], (r[dixxm, sxxm, 2] + r[iixx, sxxm, 2]) / 2, sr[ix, sx, 2], (r[ixxm, dsxxm, 2] + r[ixxm, isxx, 2]) / 2);
                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];
                                    float dist = 1 / MathF.Pow(Dist4(i, s, cx, cy, halfx), 4);
                                    float sdist = (float)(dist + ris[i, s]);
                                    float risdist = ris[i, s] / sdist;
                                    float disdist = dist / sdist;
                                    ri[i, s, 0] = (short)(r[i, s, 0] * disdist + ri[i, s, 0] * risdist + 0.5f);
                                    ri[i, s, 1] = (short)(r[i, s, 1] * disdist + ri[i, s, 1] * risdist + 0.5f);
                                    ri[i, s, 2] = (short)(r[i, s, 2] * disdist + ri[i, s, 2] * risdist + 0.5f);
                                    ris[i, s] = sdist;
                                }
                            });
                        }
                    }

                    for (int sx = kks - 1; sx > -1; sx--)
                    {
                        for (int ix = kki - 1; ix > -1; ix--)
                        {
                            int dsxxm = sx * x;
                            sxxm = (sx + 1) * x;
                            int isxx = sxxm - 1;
                            float dsxxp = dsxxm + x50p;
                            int dixxm = ix * x;
                            ixxm = (ix + 1) * x;
                            int iixx = ixxm - 1;
                            float dixxp = dixxm + x50p;

                            Parallel.For(dixxm, ixxm, i =>
                            {
                                for (int s = dsxxm; s < sxxm; s++)
                                {
                                    short[] b3 = Bilinear3(i, s, dixxp, dsxxp, ixxm, sxxm, sr[ix, sx, 0], (r[ixxm, dsxxm, 0] + r[ixxm, isxx, 0]) / 2, r[ixxm, sxxm, 0], (r[dixxm, sxxm, 0] + r[iixx, sxxm, 0]) / 2, sr[ix, sx, 1], (r[ixxm, dsxxm, 1] + r[ixxm, isxx, 1]) / 2, r[ixxm, sxxm, 1], (r[dixxm, sxxm, 1] + r[iixx, sxxm, 1]) / 2, sr[ix, sx, 2], (r[ixxm, dsxxm, 2] + r[ixxm, isxx, 2]) / 2, r[ixxm, sxxm, 2], (r[dixxm, sxxm, 2] + r[iixx, sxxm, 2]) / 2);
                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];
                                    float dist = 1 / MathF.Pow(Dist4(i, s, cx, cy, halfx), 4);
                                    float sdist = (float)(dist + ris[i, s]);
                                    float risdist = ris[i, s] / sdist;
                                    float disdist = dist / sdist;
                                    ri[i, s, 0] = (short)(r[i, s, 0] * disdist + ri[i, s, 0] * risdist + 0.5f);
                                    ri[i, s, 1] = (short)(r[i, s, 1] * disdist + ri[i, s, 1] * risdist + 0.5f);
                                    ri[i, s, 2] = (short)(r[i, s, 2] * disdist + ri[i, s, 2] * risdist + 0.5f);
                                    ris[i, s] = sdist;
                                }
                            });
                        }
                    }

                    for (int sx = kks + 2; sx < os; sx++)
                    {
                        for (int ix = kki - 1; ix > -1; ix--)
                        {
                            sxx = sx * x;
                            int isxxm = sxx - 1;
                            int dsxx = (sx + 1) * x;
                            int isxx = dsxx - 1;
                            float dsxxp = sxx + x50p;
                            int dixxm = ix * x;
                            int dixx = dixxm + x;
                            int iixx = dixx - 1;
                            float dixxp = dixxm + x50p;

                            Parallel.For(dixxm, dixx, i =>
                            {
                                for (int s = sxx; s < dsxx; s++)
                                {
                                    short[] b3 = Bilinear3(i, s, dixxp, isxxm, dixx, dsxxp, (r[dixxm, isxxm, 0] + r[iixx, isxxm, 0]) / 2, r[dixx, isxxm, 0], (r[dixx, isxx, 0] + r[dixx, sxx, 0]) / 2, sr[ix, sx, 0], (r[dixxm, isxxm, 1] + r[iixx, isxxm, 1]) / 2, r[dixx, isxxm, 1], (r[dixx, isxx, 1] + r[dixx, sxx, 1]) / 2, sr[ix, sx, 1], (r[dixxm, isxxm, 2] + r[iixx, isxxm, 2]) / 2, r[dixx, isxxm, 2], (r[dixx, isxx, 2] + r[dixx, sxx, 2]) / 2, sr[ix, sx, 2]);
                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];
                                    float dist = 1 / MathF.Pow(Dist4(i, s, cx, cy, halfx), 4);
                                    float sdist = (float)(dist + ris[i, s]);
                                    float risdist = ris[i, s] / sdist;
                                    float disdist = dist / sdist;
                                    ri[i, s, 0] = (short)(r[i, s, 0] * disdist + ri[i, s, 0] * risdist + 0.5f);
                                    ri[i, s, 1] = (short)(r[i, s, 1] * disdist + ri[i, s, 1] * risdist + 0.5f);
                                    ri[i, s, 2] = (short)(r[i, s, 2] * disdist + ri[i, s, 2] * risdist + 0.5f);
                                    ris[i, s] = sdist;
                                }
                            });
                        }
                    }

                    for (int ix = kki + 2; ix < oi; ix++)
                    {
                        for (int sx = kks - 1; sx > -1; sx--)
                        {
                            sxx = sx * x;
                            int dsxx = sxx + x;
                            int isxx = dsxx - 1;
                            float dsxxp = sxx + x50p;
                            ixx = ix * x;
                            int dixx = ixx + x;
                            ixxm = ixx - 1;
                            int iixx = dixx - 1;
                            float dixxp = ixx + x50p;

                            Parallel.For(ixx, dixx, i =>
                            {
                                for (int s = sxx; s < dsxx; s++)
                                {
                                    short[] b3 = Bilinear3(i, s, ixxm, dsxxp, dixxp, dsxx, (r[ixxm, sxx, 0] + r[ixxm, isxx, 0]) / 2, sr[ix, sx, 0], (r[ixx, dsxx, 0] + r[iixx, dsxx, 0]) / 2, r[ixxm, dsxx, 0], (r[ixxm, sxx, 1] + r[ixxm, isxx, 1]) / 2, sr[ix, sx, 1], (r[ixx, dsxx, 1] + r[iixx, dsxx, 1]) / 2, r[ixxm, dsxx, 1], (r[ixxm, sxx, 2] + r[ixxm, isxx, 2]) / 2, sr[ix, sx, 2], (r[ixx, dsxx, 2] + r[iixx, dsxx, 2]) / 2, r[ixxm, dsxx, 2]);
                                    float dist = 1 / MathF.Pow(Dist4(i, s, cx, cy, halfx), 4);
                                    float sdist = (float)(dist + ris[i, s]);
                                    float risdist = ris[i, s] / sdist;
                                    float disdist = dist / sdist;

                                    r[i, s, 0] = b3[0];
                                    r[i, s, 1] = b3[1];
                                    r[i, s, 2] = b3[2];

                                    ri[i, s, 0] = (short)(b3[0] * disdist + ri[i, s, 0] * risdist + 0.5f);
                                    ri[i, s, 1] = (short)(b3[1] * disdist + ri[i, s, 1] * risdist + 0.5f);
                                    ri[i, s, 2] = (short)(b3[2] * disdist + ri[i, s, 2] * risdist + 0.5f);
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
                    for (byte t = 0; t < 3; t++)
                    {
                        if (ri[i, s, t] < 0.5f)
                            rb[i, s, t] = 0;
                        else if (ri[i, s, t] > 254.5) //after optimization, k=8+ for pixelart?
                            rb[i, s, t] = 255;
                        else
                            rb[i, s, t] = (byte)(ri[i, s, t] + 0.5f);
                    }
                }
            });

            return BMPfromRGB(rb, ni, ns);
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
            if (Settings.Default.location != new Size(777, 777)) this.Location = (Point)Settings.Default.location;
            if (Left < 0) Left = 0;
            if (Screen.PrimaryScreen != null)
            {
                if (Left + Width > Screen.PrimaryScreen.Bounds.Width) Left = Screen.PrimaryScreen.Bounds.Width - Width;
                if (Top + Height > Screen.PrimaryScreen.Bounds.Height) Top = Screen.PrimaryScreen.Bounds.Height - Height;
            }
            if (Settings.Default.method > comboBox1.Items.Count - 1)
            {
                comboBox1.SelectedIndex = 0; 
            }
            else
            {
                comboBox1.SelectedIndex = Settings.Default.method;
            }

            switch (Settings.Default.gpu)
            {
                case 0:
                    checkBox1.CheckState = CheckState.Unchecked;//after optimization, separate -> smooth/rough
                    break;
                case 1:
                    checkBox1.CheckState = CheckState.Checked;
                    break;
                default:
                    checkBox1.CheckState = CheckState.Indeterminate;
                    break;
            }
            numericUpDown1.Value = Settings.Default.scale;
            switch (Settings.Default.channels)
            {
                case 3:
                    radioButton2.Checked = true;
                    break;
            }
            trackBar1.Value = Settings.Default.accuracy;
            checkBox2.Checked = Settings.Default.sound;

            toolTip1.SetToolTip(this, "Win+D and click desktop to minimize window\nDrag to move window\n\nbefore Upscale:\nRMB x2 to fit window in screen");
            toolTip1.SetToolTip(button5, "Discard changes and reload image");
            toolTip1.SetToolTip(button1, "Load raster image from file");
            toolTip1.SetToolTip(button2, "Save result as PNG file");
            toolTip1.SetToolTip(button3, "Start processing with selected parameters\nBe careful - the process can be very long and it will be impossible to cancel it except with the close button or through the task manager");
            toolTip1.SetToolTip(button4, "Close program and cancel processing\nIf this doesn't work, wait or kill process scaleSmooth from TaskManager (Ctrl+Shift+Esc)");
            toolTip1.SetToolTip(comboBox1, "Select method fo upscaling");
            toolTip1.SetToolTip(pictureBox5, "Select previous method fo upscaling");
            toolTip1.SetToolTip(pictureBox4, "Select next method fo upscaling");
            toolTip1.SetToolTip(checkBox1, "Try increase perfomance by using your videocard? (It can interrupt other non-system GPU processes)\n\nSquare - Auto decide\nCheck mark - Always\nEmpty - Never");
            toolTip1.SetToolTip(checkBox2, "Notify that processing is done by loud sound");
            toolTip1.SetToolTip(label6, "before Upscale:\nDrag to move window\nRMB x2 to fit window in screen");
            toolTip1.SetToolTip(pictureBox3, "Example of selected method\n\nbefore Upscale:\nDrag to move window\nRMB x2 to fit window in screen");
            toolTip1.SetToolTip(pictureBox2, "Example of bilinear interpolation\n\nbefore Upscale:\nDrag to move window\nRMB x2 to fit window in screen");
            toolTip1.SetToolTip(pictureBox1, "Image buffer\nShows input and output image\nAllows you to do several operations before saving total result\nWin+D and click desktop to minimize window\n\nbefore Upscale:\nDrag to move window\nRMB x2 to fit window in screen");
            toolTip1.SetToolTip(label1, "before Upscale:\nDrag to move window\nRMB x2 to fit window in screen");
            toolTip1.SetToolTip(numericUpDown1, "Write or select required scale factor by two axes");
            toolTip1.SetToolTip(radioButton1, "Processing 1 channel: Luma\ncalculated as Y=0.299*R+0.587*G+0.114*B");
            toolTip1.SetToolTip(radioButton2, "Processing 3 channels: R,G,B");
            toolTip1.SetToolTip(trackBar1, "Select Fast or Accurate variant or something in between\nSpace to default value");
            toolTip1.SetToolTip(label2, "Drag to move window\nRMB x2 to fit window in screen");
            toolTip1.SetToolTip(StopWatchLabel, "MMB to copy stopwatch value\n\nbefore Upscale:\nDrag to move window\nRMB x2 to fit window in screen");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopWatchLabel.Visible = false;
            switch (comboBox1.SelectedItem)
            {
                case "scaleSmooth"://afterALL, replace demo
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortSmooth);
                    label6.Text = "Most accurate for scenes where objects are completely in the image, but little bit blurred (much less than any interpolation) and grid structure is still visible\r\n\r\nVery fast - Slow, and you can process multiple images at the same time without losing speed";
                    checkBox1.Visible = false;
                    break;
                case "scaleSmoothContrast":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortSmoothContin);
                    label6.Text = "Very smooth, after reverse adjustment - most accurate for any scenes, but very contrast and grid structure is still visible\r\n\r\nSlow, but you can process multiple images at the same time without losing speed";
                    checkBox1.Visible = false;
                    break;
                case "scaleSmoothContinuous":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortSmoothContr);
                    label6.Text = "Most accurate for scenes where objects extend beyond the boundaries of the image, but little bit blurred (much less than any interpolation) and grid structure is still visible\r\n\r\nVery fast - Slow, and you can process multiple images at the same time without losing speed";
                    checkBox1.Visible = false;
                    break;
                case "boldScale":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortBold);
                    label6.Text = "Grid structure, little bit noisy and contrasty (for accuracy, subsequent reverse correction is desirable) and too small details may lost\r\n\r\nVery fast - Very very slow, but you can process multiple images at the same time without losing speed";
                    checkBox1.Visible = false;
                    break;
                case "contrastBoldScale":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortContrast);
                    label6.Text = "Perfect result, but too contrasty (for accuracy, subsequent reverse correction is required) and too small details are lost\r\n\r\nVery fast - Very very slow, but you can process multiple images at the same time without losing speed";
                    checkBox1.Visible = false;
                    break;
                case "scaleFurry":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortFurry);
                    label6.Text = "Beautiful and detailed result, but only if bigger version must be monochrome image (only pure black and white, or for color - only pure red, black, green, yellow, fuchsia, blue, cyan and white)\r\n\r\nVery slow - Very very slow, but you can process multiple images at the same time without losing speed";
                    checkBox1.Visible = false;
                    break;
                case "scaleRough":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortRough);
                    label6.Text = "Typographic raster stylization, but if bigger version must be monochrome image it gives acceptable upscaling\r\n\r\nVery slow, but you can process multiple images at the same time without losing speed";
                    checkBox1.Visible = false;
                    break;
                case "scaleSeparate":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortSeparate);//After all, translate github
                    label6.Text = "Gives beatiful, but almost monochrome result and there are Gibbs ringing artifacts (to avoid you can try several times x2-x4)\r\n\r\nVery very fast - fast, but you can't process multiple images at the same time without losing speed";
                    checkBox1.Visible = false;
                    break;
                case "scaleBilinearApproximation":
                    pictureBox3.Image = new Bitmap(ScaleSmooth.Properties.Resources.shortBilApprox);
                    label6.Text = "A clearly defined grid structure and may be present Gibbs ringing artifacts, but it's better than nearest neighbour\r\n\r\nVery very fast - Very slow, and you can't process multiple images at the same time without losing speed";
                    checkBox1.Visible = true;
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

        private void button5_Click(object sender, EventArgs e)
        {
            if (lastfile == "")
            {
                pictureBox1.Image = new Bitmap(ScaleSmooth.Properties.Resources.A8);
            }
            else
            {
                pictureBox1.Image = Image.FromFile(lastfile);
            }
            StopWatchLabel.Visible = false;
            label3.Text = pictureBox1.Image.Width + "x" + pictureBox1.Image.Height;
        }

        private void DragNDrop(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            PostMessage(this.Handle, 0x0112, 0xF012, 0);
        }

        private void ToScreen(object sender, MouseEventArgs e)
        {
            if (Left < 0) Left = 0;
            if (Screen.PrimaryScreen != null)
            {
                if (Left + Width > Screen.PrimaryScreen.Bounds.Width) Left = Screen.PrimaryScreen.Bounds.Width - Width;
                if (Top + Height > Screen.PrimaryScreen.Bounds.Height) Top = Screen.PrimaryScreen.Bounds.Height - Height;
            }
        }

        private void CopyStopwatch(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(((Label)sender).Text.Split(' ')[0]);
        }

        private void DefaultIs42(object sender, KeyPressEventArgs e)
        {
            trackBar1.Value = 42;
            label2.Text = "Fast                       Accurate";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int sp;
            if (trackBar1.Value < 42)
            {
                sp = trackBar1.Value * 9 / 42;
            }
            else
            {
                sp = (trackBar1.Value - 42) * 10 / 58 + 9;
            }
            label2.Text = "Fast " + new String(' ', sp) + trackBar1.Value + new String(' ', 19 - sp) + " Accurate";
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
            [PreserveSig]
            void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
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

        public static void SetValue(IntPtr windowHandle, float progressValue, float progressMax)
        {
            if (taskbarSupported) taskbarInstance.SetProgressValue(windowHandle, (ulong)progressValue, (ulong)progressMax);
        }
    }
}
