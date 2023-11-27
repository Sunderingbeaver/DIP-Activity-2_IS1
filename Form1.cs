using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using AForge.Video.DirectShow;
using AForge.Video;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Bitmap pre; //pre = pb1(Original Image) post = pb2(edited green background) result = pb3(Fused)
        Bitmap post;
        Bitmap result;
        String path;
        private bool isCamera = false;
        private bool isRunning = false;
        private bool isFiltered = false;
        private FilterInfoCollection devices;
        private VideoCaptureDevice video;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            post = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            for(int x = 0; x <= (pictureBox1.Width - 1); x++)
            {
                for (int y = 0; y <= (pictureBox1.Height - 1); y++)
                {
                    Color color = pre.GetPixel(x, y);   
                    color = Color.FromArgb(255, (color.R), (color.G), (color.B));
                    post.SetPixel((pictureBox1.Width - 1) - x, y, color);
                    pictureBox2.Image = post;
                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            
        }


        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                pre = new Bitmap(open.FileName);
                pictureBox1.Image= pre;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                post = new Bitmap(pre.Width, pre.Height);
                for (int x = 0; x <= (pre.Width - 1); x++)
                {
                    for (int y = 0; y <= (pre.Height - 1); y++)
                    {
                        Color color = pre.GetPixel(x, y);
                        int c = (color.R + color.G + color.B) / 3;
                        color = Color.FromArgb(c, c, c);
                        post.SetPixel(x, y, color);
                        pictureBox3.Image = post;
                        pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
            else
            {
                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                post = new Bitmap(pre.Width, pre.Height);
                for (int x = 0; x <= (pre.Width - 1); x++)
                {
                    for (int y = 0; y <= (pre.Height - 1); y++)
                    {
                        Color color = pre.GetPixel(x, y);
                        color = Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
                        post.SetPixel(x, y, color);
                        pictureBox2.Image = post;
                        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
            else
            {

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Color pixel;
            int intensity;
            int[] histogram = new int[256];
            post = new Bitmap(pictureBox2.Width, pictureBox2.Height);

            for (int x = 0; x < pre.Width; x++)
            {
                for (int y = 0; y < pre.Height; y++)
                {
                    pixel = pre.GetPixel(x, y);
                    intensity = (int)(pixel.R + pixel.G + pixel.B) / 3;

                    histogram[intensity]++;
                }
            }
            using (Graphics g = Graphics.FromImage(post))
            {
                int maxValue = 0;
                foreach (int value in histogram)
                {
                    if (value > maxValue)
                        maxValue = value;
                }

                for (int i = 0; i < 256; i++)
                {
                    int barHeight = (int)((double)histogram[i] / maxValue * post.Height);
                    g.DrawLine(Pens.Black, i, post.Height, i, post.Height - barHeight);
                }
            }

            pictureBox2.Image = post;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                pre = new Bitmap(open.FileName);
                pictureBox1.Image = pre;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private bool isValid(string s)
        {
            try
            {
                using (var bitmap = new Bitmap(s))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                Image img = Sepia((Bitmap)pictureBox1.Image.Clone());
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox2.Image = img;
            } else
            {

            }
        }


        private void button7_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.ShowDialog();
            path = save.FileName;
            post.Save(path + ".jpg");
        }
        private Image Sepia(Bitmap b)
        {
            Color pixel;
            post = new Bitmap(pre.Width, pre.Height);

            for (int x = 0; x < pre.Width; x++)
            {
                for (int y = 0; y < pre.Height; y++)
                {
                    pixel = pre.GetPixel(x, y);
                    int r = pixel.R;
                    int g = pixel.G;
                    int bb = pixel.B;

                    int tr = (int)(0.393 * r + 0.769 * g + 0.189 * bb);
                    int tg = (int)(0.349 * r + 0.686 * g + 0.168 * bb);
                    int tb = (int)(0.272 * r + 0.534 * g + 0.131 * bb);

                    if (tr > 255)
                    {
                        r = 255;
                    }
                    else
                    {
                        r = tr;
                    }

                    if (tg > 255)
                    {
                        g = 255;
                    }
                    else
                    {
                        g = tg;
                    }

                    if (tb > 255)
                    {
                        bb = 255;
                    }
                    else
                    {
                        bb = tb;
                    }

                    post.SetPixel(x, y, Color.FromArgb(r, g, bb));
                }
            }

            return post;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                post = new Bitmap(open.FileName);
                pictureBox2.Image = post;
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            if (pictureBox2 != null)
            {
                pictureBox3.Image = subtract(pre);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap b = pre;
            Color gr = Color.FromArgb(0, 0, 255);
            int ggreen = (gr.R + gr.G + gr.B) / 3;
            int threshold = 10;
            result = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            Image image = pre, image2 = post;
            pre = resize(image, 178, 200);
            post = resize(image2, 178, 200);
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    Color pixel = pre.GetPixel(x, y);
                    Color backpixel = post.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subractvalue = Math.Abs(grey - ggreen);
                    if (subractvalue < threshold)
                    {
                        result.SetPixel(x, y, backpixel);
                    }
                    else
                    {
                        result.SetPixel(x, y, pixel);
                    }
                }
            }
            Image resultr = result;
            result = resize(resultr, (pictureBox3.Height - 1), (pictureBox3.Width - 1));
            pictureBox3.Image = result;
        }
        private Bitmap subtract(Bitmap b)
        {
            Color gr = Color.FromArgb(0, 0, 255);
            int ggreen = (gr.R + gr.G + gr.B) / 3;
            int threshold = 10;
            result = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            Image image = pre, image2 = post;
            pre = resize(image, 178, 200);
            post = resize(image2, 178, 200);
            for (int x = 0; x < result.Width; x++)
            {
                for (int y = 0; y < result.Height; y++)
                {
                    Color pixel = pre.GetPixel(x, y);
                    Color backpixel = post.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subractvalue = Math.Abs(grey - ggreen);
                    if (subractvalue < threshold)
                    {
                        result.SetPixel(x, y, backpixel);
                    }
                    else
                    {
                        result.SetPixel(x, y, pixel);
                    }
                }
            }
            Image resultr = result;
            result = resize(resultr, (pictureBox3.Height-1), (pictureBox3.Width-1));
            return result;
        }
        private Bitmap resize(Image image, int height, int width)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(image, 0, 0, width, height);
            }
            return result;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            StartCameraView();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            StopCameraView();
        }
        //camera shiz
        private void videoNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Image cameraFrame = (Image)eventArgs.Frame.Clone();

            pictureBox1.Invoke((MethodInvoker)delegate
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = cameraFrame;
                if (!isFiltered)
                {
                    pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox3.Image = cameraFrame;
                }
            });

        }
        private void StartCameraView()
        {
            devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (devices.Count > 0)
            {
                video = new VideoCaptureDevice(devices[0].MonikerString);
                video.NewFrame += videoNewFrame;
                video.Start();
                isRunning = true;
            }
            else
            {
                MessageBox.Show("No video devices found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopCameraView()
        {
            
                video.SignalToStop();
                video.WaitForStop();
                isRunning = false;
           
        }
    }
}
