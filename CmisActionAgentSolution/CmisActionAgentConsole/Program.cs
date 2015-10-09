using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Aegif.Makuranage.Models;
using Aegif.Makuranage.TriggerEngine;
using Aegif.Makuranage.ActionEngine;
using Aegif.Makuranage.ActionEngine.Cmis;
using Aegif.Makuranage.TriggerEngine.LocalFileSystem;
using Aegif.Makuranage.TriggerEngine.Cmis;
using Aegif.Makuranage.TriggerEngine.WebSiteCapture;

namespace Aegif.Makuranage.ConsoleApplication {
    class Program {
        static void Main(string[] args) {
            var timeCaptureTrigger = new TimerCapturingTrigger();
            timeCaptureTrigger.Changed += Trigger_Changed;
            timeCaptureTrigger.PollingStartAsync();
            Console.WriteLine("時間ごとのWebキャプチャを開始します。何かキーを押すと終了します。");
            Console.ReadKey();


            var localFSTrigger = new LocalFileSystemTrigger();
            localFSTrigger.Changed += Trigger_Changed;
            localFSTrigger.PollingStartAsync();
            Console.WriteLine("ローカルファイルシステムの監視を開始します。何かキーを押すと終了します。");
            Console.ReadKey();


            var provider = new SettingsChangeLogTokenProvider();
            var cmisChangeLogTrigger = new CmisUpdateTrigger(provider);
            cmisChangeLogTrigger.Changed += Trigger_Changed;
            cmisChangeLogTrigger.PollingStartAsync();
            Console.WriteLine("CMISリポジトリの監視を開始します。何かキーを押すと終了します。");
            Console.ReadKey();


        }

        private static void Trigger_Changed(object sender, MakuraDocumentEventArgs e) {
            var action = new CmisUploadAction();
            action.Invoke(e.Path, e.UpdatedDocument);
        }
    }
}
