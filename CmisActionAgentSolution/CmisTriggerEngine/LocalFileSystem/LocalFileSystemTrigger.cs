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


        public bool EnableRaisingEvents {
            get {
                return watcher.EnableRaisingEvents;
            }

            set {
                watcher.EnableRaisingEvents = value;
            }
        }

 
        public LocalFileSystemTrigger() {
            watcher = new FileSystemWatcher();
            watcher.Path = BaseDirectoryPath;
            
            watcher.NotifyFilter =
                (System.IO.NotifyFilters.LastAccess
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
            watcher.Filter = "";

            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Changed;

            watcher.EnableRaisingEvents = true;
        }

        public event TransferObjectEventHandler Changed;

        private void Watcher_Changed(object sender, FileSystemEventArgs e) {
            var filePath = e.FullPath;
            
            //TODO: ベースパスからの相対パス作成、MakuraFolderの階層を作成し末端にぶら下げる
            var file = new FileInfo(filePath);
            
            var doc = GetMakuraDocument(file);
            var eventArgs = new TransferObjectEventArgs();
            eventArgs.TransferObject = doc;

            Changed?.Invoke(sender, eventArgs);
        }
        

        public void ParseUpdate(DirectoryInfo dir) {

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
