
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
        public event MakuraObjectEventHandler Changed;

        public CmisConnector Connector { get;private set; }

        private CmisChangeLogWatcher watcher;

        public CmisUpdateTrigger(IChangeLogTokenProvider changeLogTokenProvider) {
            var conn = new CmisConnector();

            /*
            conn.User = "takuma.sugimoto";
            conn.AtomPubUrl = "https://avenue.aegif.jp:11222/alfresco/api/-default-/public/cmis/versions/1.1/atom";
            conn.Password = "ias0jou8";
            conn.RepositoryId = "-default-";
            */

            Connector = conn;
            watcher = new CmisChangeLogWatcher(Connector, changeLogTokenProvider);
            watcher.Changed += Watcher_Changed;

        }

        public void PollingStartAsync() {
            watcher.PollingStartAsync();
        }

        public void PollingStop() {
            watcher.PollingStop();
        }


        private void Watcher_Changed(object sender, CmisChangeLogEventArgs e) {

            var doc = GetMakuraDocument(e.UpdatedDocument);
            var eventArgs = new MakuraDocumentEventArgs();
            eventArgs.UpdatedDocument = doc;

            //マルチファイリング負対応
            var path = e.UpdatedDocument.Paths.FirstOrDefault();
            eventArgs.Path = path ?? String.Empty;

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
