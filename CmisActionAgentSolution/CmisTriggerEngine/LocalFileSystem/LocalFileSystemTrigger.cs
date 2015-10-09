using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Aegif.Makuranage.Models;

namespace Aegif.Makuranage.TriggerEngine.LocalFileSystem {
    public class LocalFileSystemTrigger : ITrigger {


        public String BaseDirectoryPath { get; set; } = @"D:\Users\takuma.sugimoto\Desktop\Sample";

        public DirectoryInfo BaseDirectory {
            get { return new DirectoryInfo(BaseDirectoryPath); }
        }

        private FileSystemWatcher watcher;

 
        public LocalFileSystemTrigger() {

        }

        public void PollingStartAsync() {
            watcher = new FileSystemWatcher();
            watcher.Path = BaseDirectoryPath;

            watcher.NotifyFilter =
                (System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName);
            watcher.Filter = "";


            watcher.IncludeSubdirectories = true;
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Changed;

            watcher.EnableRaisingEvents = true;
        }

        public void PollingStop() {
            watcher.EnableRaisingEvents = false;
        }

        public event MakuraObjectEventHandler Changed;

        private void Watcher_Changed(object sender, FileSystemEventArgs e) {
            var filePath = e.FullPath;
            
            var file = new FileInfo(filePath);
            if(!file.Exists)return;

            var fileDir = new Uri(file.Directory.FullName);
            var baseDir = new Uri(BaseDirectory.FullName);

            var relativePath = baseDir.MakeRelativeUri(fileDir).ToString();

            var prefix = @"../";
            if ( relativePath.StartsWith(prefix)) {
                relativePath = relativePath.Substring(prefix.Length);
            }



            //var doc = GetMakuraDocument(file);
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
                eventArgs.Path = relativePath;

                Changed?.Invoke(sender, eventArgs);
            }
        }


         private MakuraDocument GetMakuraDocument(FileInfo file) {
            using (var fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                var content = new MakuraContentStream();
                content.FileName = file.Name;
                content.Length = fileStream.Length;
                content.MimeType = MimeMapping.GetMimeMapping(file.Name);
                content.Stream = fileStream;

                var doc = new MakuraDocument();
                doc.ContentStream = content;
                doc.Name = file.Name;
                return doc;
            }
        }


    }
}
