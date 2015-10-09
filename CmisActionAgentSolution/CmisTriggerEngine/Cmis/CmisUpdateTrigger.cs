
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DotCMIS.Client.Impl;
using Aegif.Makuranage.Models;

namespace Aegif.Makuranage.TriggerEngine.Cmis {
    public class CmisUpdateTrigger : ITrigger {
        public event TransferObjectEventHandler Changed;

        public CmisConnector Connector { get;private set; }

        public bool EnableRaisingEvents {
            get {
                return watcher.EnableRaisingEvents;
            }

            set {
                watcher.EnableRaisingEvents = value;
            }
        }

        private CmisChangeLogWatcher watcher;

        public CmisUpdateTrigger(IChangeLogTokenProvider changeLogTokenProvider) {
            this.Connector = Connector = new CmisConnector();

            watcher = new CmisChangeLogWatcher(Connector, changeLogTokenProvider);
            watcher.Changed += Watcher_Changed;

            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, CmisChangeLogEventArgs e) {

            var doc = GetMakuraDocument(e.UpdatedDocument);
            var eventArgs = new TransferObjectEventArgs();
            eventArgs.TransferObject = doc;

            Changed?.Invoke(sender, eventArgs);
        }

        private MakuraDocument GetMakuraDocument(Document cmisDoc) {
            var content = new MakuraContentStream();
            content.FileName = cmisDoc.ContentStreamFileName;
            content.Length = cmisDoc.ContentStreamLength;
            content.MimeType = cmisDoc.ContentStreamMimeType;
            content.Stream = cmisDoc.GetContentStream().Stream;

            var doc = new MakuraDocument();
            doc.ContentStream = content;
            doc.Name = cmisDoc.Name;
            return doc;
        }
    }
}
