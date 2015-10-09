
using Aegif.Makuranage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Aegif.Makuranage.TriggerEngine.WebSiteCapture {
    public class TimerCapturingTrigger : ITrigger {
        public event MakuraObjectEventHandler Changed;

        public void PollingStartAsync() {
            timerOn = true;
            WaitAsync();
        }

        public void PollingStop() {
            timerOn = false;
        }

        private bool timerOn;

        public async void WaitAsync() {
            timerOn = true;

            await Task.Run(() => {
                while (timerOn) {
                    if(DateTime.Now.Minute % 2 == 0) {
                        CaptureWebSite();
                    }
                        
                }
            });
        }

        private void CaptureWebSite() {
            var capture = new PageCapturer(@"http://localhost:8080/ui/repo/bedroom/");
            capture.Capture();
            capture.Captured += Capture_Captured;
        }

        private void Capture_Captured(object sender, WebPageCapturedEventArgs e) {
            var file = e.CapturedFile;

            using (var fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                var content = new MakuraContentStream();
                content.FileName = file.Name;
                content.Length = fileStream.Length;
                content.MimeType = MimeMapping.GetMimeMapping(file.Name);
                content.Stream = fileStream;

                var doc = new MakuraDocument();
                doc.ContentStream = content;
                doc.Name = file.Name;

                var eventArgs = new MakuraDocumentEventArgs();
                eventArgs.UpdatedDocument = doc;
                eventArgs.Path = "/CaptureImages";

                Changed?.Invoke(this, eventArgs);
            }
        }
    }
}
