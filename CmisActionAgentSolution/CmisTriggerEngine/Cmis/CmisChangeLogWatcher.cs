
using DotCMIS.Client.Impl;
using DotCMIS.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aegif.Makuranage.TriggerEngine.Cmis {
    public class CmisChangeLogWatcher {

        public CmisChangeLogWatcher(CmisConnector conn, IChangeLogTokenProvider latestChangeLogTokenProvider) {
            this.connector = conn;
            this.LatestChangeLogTokenProvider = latestChangeLogTokenProvider;
        }

        public bool EnableRaisingEvents { get; set; }

        public event CmisChangeLogEventHandler Changed;

        public IChangeLogTokenProvider LatestChangeLogTokenProvider;

        public CmisConnector connector;

        public void Poling() {
            while (true) {
                if (!EnableRaisingEvents) return;

                var session = connector.CreateSession();
                var latestChangeLogToken = LatestChangeLogTokenProvider.GetChangeLogToken();
                var changeLogs = session.GetContentChanges(latestChangeLogToken, false, 10);
                if ( changeLogs.TotalNumItems > 0){
                    foreach (var cle in changeLogs.ChangeEventList) {
                        var doc = session.GetObject(cle.ObjectId) as Document;
                        if (doc == null) continue; //とりあえずドキュメントのみ

                        var evChangeType = WatcherChangeTypes.Changed;
                        if (cle.ChangeType == ChangeType.Created) {
                            evChangeType = WatcherChangeTypes.Created;
                        } else if (cle.ChangeType == ChangeType.Deleted) {
                            evChangeType = WatcherChangeTypes.Deleted;
                        }else if (cle.ChangeType == ChangeType.Updated) {
                            evChangeType = WatcherChangeTypes.Changed;
                        }

                        var evArg = new CmisChangeLogEventArgs(evChangeType, doc);
                        this.Changed(this, evArg);
                    }

                    LatestChangeLogTokenProvider.SetChangeLogToken(changeLogs.LatestChangeLogToken);

                }

            }
        }
    }

    public delegate void CmisChangeLogEventHandler(object sender, CmisChangeLogEventArgs e);


    public class CmisChangeLogEventArgs : EventArgs {


        public CmisChangeLogEventArgs(WatcherChangeTypes changeType, Document updateDoc) {
            UpdatedDocument = updateDoc;
        }

        public Document UpdatedDocument { get; private set; }

        public WatcherChangeTypes ChangeType { get; }

    }
}
