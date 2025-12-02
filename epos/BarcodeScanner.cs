using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Windows.Forms;
using ZXing;

namespace epos
{
    public class BarcodeScanner : IDisposable
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private readonly BarcodeReader reader;

        
        private string lastCode = null;
        private DateTime lastCodeTime = DateTime.MinValue;

        
        private readonly PictureBox previewBox;

        public event Action<string> BarcodeDetected;

        public BarcodeScanner(PictureBox preview = null)
        {
            previewBox = preview;

            reader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true
            };

            
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        public string[] GetCameraNames()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            string[] names = new string[videoDevices.Count];

            for (int i = 0; i < videoDevices.Count; i++)
                names[i] = videoDevices[i].Name;

            return names;
        }

        
        public void Start(int cameraIndex = 0)
        {
            Stop();

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
                throw new Exception("Neboli nájdené žiadne kamery.");

            if (cameraIndex < 0 || cameraIndex >= videoDevices.Count)
                cameraIndex = 0;

            videoSource = new VideoCaptureDevice(videoDevices[cameraIndex].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
        }

        public void Stop()
        {
            if (videoSource != null)
            {
                videoSource.NewFrame -= VideoSource_NewFrame;

                if (videoSource.IsRunning)
                    videoSource.SignalToStop();

                videoSource = null;
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (Bitmap bmp = (Bitmap)eventArgs.Frame.Clone())
                {
                    
                    if (previewBox != null)
                    {
                        
                        if (previewBox.InvokeRequired)
                        {
                            previewBox.BeginInvoke(new Action(() =>
                            {
                                previewBox.Image?.Dispose();
                                previewBox.Image = (Bitmap)bmp.Clone();
                            }));
                        }
                        else
                        {
                            previewBox.Image?.Dispose();
                            previewBox.Image = (Bitmap)bmp.Clone();
                        }
                    }

                    
                    var result = reader.Decode(bmp);
                    if (result != null)
                    {
                        string code = result.Text;

                        
                        if (code == lastCode && (DateTime.Now - lastCodeTime).TotalSeconds < 2)
                            return;

                        lastCode = code;
                        lastCodeTime = DateTime.Now;

                        BarcodeDetected?.Invoke(code);
                    }
                }
            }
            catch
            {
                
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
