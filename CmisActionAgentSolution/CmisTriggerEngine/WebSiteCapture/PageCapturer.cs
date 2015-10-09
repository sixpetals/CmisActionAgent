using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aegif.Makuranage.TriggerEngine.WebSiteCapture {
    public class PageCapturer {

        private WebBrowser browser;

        public event WebPageCapturedEventHandler Captured;


        private String captureURL = @"http://localhost:8080/ui/repo/bedroom/";
        public String CaptureURL {
            get { return captureURL; }
            set { captureURL = value; }
        }

        private System.Drawing.Imaging.ImageFormat captureFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
        public System.Drawing.Imaging.ImageFormat CaptureFormat {
            get { return captureFormat; }
            set { captureFormat = value; }
        }

        private String saveLocation;
        public String SaveLocation {
            get { return saveLocation; }
            set { saveLocation = value; }
        }

        public PageCapturer(String pageURL) {
            CaptureURL = pageURL;


        }


        public void Capture() {
            var th = new Thread(() => {
                browser = new WebBrowser();
                //Register for the Document Completed Event
                browser.DocumentCompleted += webBrowserDocumentCompleted;
                browser.Navigate(CaptureURL);
                Application.Run();
            });

            //Set the threads appartment state to be singular
            th.SetApartmentState(ApartmentState.STA);

            //Start the thread
            th.Start();
        }


        void webBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            //Set the Width and Height of the browser to match the page size acording to the scroll bars
            int webPageHeight = browser.Document.Body.ScrollRectangle.Height;
            int webPageWidth = browser.Document.Body.ScrollRectangle.Width;

            try {
                using (var bmp = new Bitmap(webPageWidth, webPageHeight)) {
                    browser.Size = new Size(webPageWidth, webPageHeight);

                    //Hide scroll bars so they wont show in the image capture
                    browser.ScrollBarsEnabled = false;

                    //capture the image from browser
                    browser.DrawToBitmap(bmp, new Rectangle(browser.Location.X, browser.Location.Y, webPageWidth, webPageHeight));

                    //save image
                    var tempPath = Path.Combine(Path.GetTempPath(), "\\CaptureImage-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".jpg");
                    bmp.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                
                var capturedFile = new FileInfo(saveLocation);
                var args = new WebPageCapturedEventArgs(capturedFile, new Uri(this.CaptureURL));
                Captured?.Invoke(this, args);
            } catch {

            }
        }
    }

    public delegate void WebPageCapturedEventHandler(object sender, WebPageCapturedEventArgs e);


    public class WebPageCapturedEventArgs : EventArgs {


        public WebPageCapturedEventArgs(FileInfo capturedFile, Uri capturedUri) {
            CapturedFile = capturedFile;
            CapturedUri = capturedUri;
        }

        public FileInfo CapturedFile { get; private set; }

        public Uri CapturedUri { get;  private set; }

    }
}
