using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace DeOldifySharp
{
    public partial class FrmMain : Form
    {
        private ColorizeModel model = null;
        ModelParam modelParam = null;
        Bitmap __InputImage;
        String __ImageFileName;
        private Thread __ColorizationThread;

        public FrmMain()
        {
            InitializeComponent();
            modelParam = new ModelParam();
            this.btnColor.Click += StartColorizationThread;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            FrmSetting dlgSetting = new FrmSetting();
            var res = dlgSetting.ShowDialog();
            if (res != DialogResult.OK)
            {
            }            
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var OFD = new OpenFileDialog();
            OFD.Title = "Open";
            OFD.Filter = "Images (*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf)|*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf|All files|*.*";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                var filename = OFD.FileName;
                this.Text = $"DeOldify Dotnet: {filename}";
                OpenImage(OFD.FileName);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void chkAutoSave_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //model = new ColorizeModel(modelParam);
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            Enabled = false;
            model = new ColorizeModel(modelParam);
            Enabled = true;
            // Loaded Model: Full-Stable, Speed: SIMD
            lblModel.Text = $"Loaded Model: {model.Description()}";
        }

        /// <summary>
        /// Opening and image from location.
        /// </summary>
        /// <param name="sourceLocation">File or network location of an image.</param>
        private Boolean OpenImage(String sourceLocation)
        {

            __ImageFileName = "";            

            if (Uri.IsWellFormedUriString(sourceLocation, UriKind.Absolute))
            {
                DownloadImage(sourceLocation);
            }
            else
            {
                try
                {
                    //picGray.Image = new Bitmap(sourceLocation);
                    __InputImage = (Bitmap)System.Drawing.Image.FromFile(sourceLocation);
                    __ImageFileName = sourceLocation;
                    OpenImage(__InputImage, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message} ({ex.HResult})",
                                    Application.ProductName, 
                                    MessageBoxButtons.OK, 
                                    MessageBoxIcon.Error);                    
                    //__InputPictureBox.ResumeLayout();
                    return false;
                }
            }

            //AddLocationHistory(sourceLocation);

            return true;
        }

        /// <summary>
        /// Opening and image from Bitmap.
        /// </summary>
        /// <param name="sourceImage">Source Bitmap.</param>
        /// <param name="clear">Clear load. I.e. not been called during another opening process.</param>
        private Boolean OpenImage(Bitmap sourceImage, Boolean clear = true)
        {
            if (clear)
                __ImageFileName = "";

            try
            {
                // __InputImage = __PrepareInputImage(sourceImage); // too slow, unnecessary for most images
                __InputImage = sourceImage;
                picGray.Image = __InputImage;                
                picColor.Image = null;
                picGray.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0} ({1})", ex.Message, ex.HResult.ToString()),
                                Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }              

            return true;

        }

        /// <summary>
        /// Starting image downloading.
        /// </summary>
        /// <param name="url">Image URL.</param>
        private Image DownloadImage(String url)
        {
            /*
            StopColorizationThread(this, null);

            __StartButton.Click -= StartColorizationThread;
            __StartButton.Click += StopColorizationThread;

            __ProgressBar.Maximum = 100;
            __ProgressBar.Minimum = 0;
            UpdateProgress(0);

            ToggleControls(false);
            */
            picGray.LoadAsync(url);
            return null;

        }

        private void StartColorizationThread(object sender, EventArgs e)
        {

            StopColorizationThread(sender, e);
            if (__InputImage == null)
                return;
            this.BeginInvoke((MethodInvoker)delegate () {
                prgbColor.Maximum = 1000;
                prgbColor.Value = 0;
                //ToggleControls(false);
                btnColor.Click -= StartColorizationThread;
                btnColor.Click += StopColorizationThread;
            });

            __ColorizationThread = new Thread(() =>
            {
                try
                {

                    if (this.InvokeRequired)
                    {

                        this.BeginInvoke((MethodInvoker)delegate () {
                            Bitmap OutputImage = model.Colorize(this.__InputImage);
                            if (OutputImage != null)
                            {
                                this.picColor.Image = OutputImage;
                                //this.__TabControl.SelectedIndex = 1;
                            }
                        });
                    }
                }
                catch (System.Threading.ThreadInterruptedException ex)
                {
                    MessageBox.Show(String.Format("{0} ({1})", ex.Message, ex.HResult.ToString()),
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (System.Threading.Tasks.TaskSchedulerException ex)
                {
                    MessageBox.Show(String.Format("{0} ({1})", ex.Message, ex.HResult.ToString()),
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0} ({1})", ex.Message, ex.HResult.ToString()),
                                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    //  GC.Collect(); // bugs. will collect by its own eventually.
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke((MethodInvoker)delegate () {
                            btnColor.Click -= StopColorizationThread;
                            btnColor.Click += StartColorizationThread;
                            //ToggleControls(true);
                        });
                    }
                }
            });
            __ColorizationThread.Start();

        }

        private void StopColorizationThread(object sender, EventArgs e)
        {

            picGray.CancelAsync();

            if (__ColorizationThread != null)
            {
                __ColorizationThread.Interrupt();
                __ColorizationThread.Join();
            }

        }
    }
}
