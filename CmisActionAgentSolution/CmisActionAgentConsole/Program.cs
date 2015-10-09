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

namespace Aegif.Makuranage.ConsoleApplication {
    class Program {
        static void Main(string[] args) {
            var localFSTrigger = new LocalFileSystemTrigger();
            localFSTrigger.Changed += Trigger_Changed;
            localFSTrigger.EnableRaisingEvents = true;
            Console.WriteLine("ローカルファイルシステムの監視を開始します。何かキーを押すと終了します。");
            Console.Read();
            localFSTrigger.EnableRaisingEvents = false;

            var provider = new SettingsChangeLogTokenProvider();
            var cmisChangeLogTrigger = new CmisUpdateTrigger(provider);
            cmisChangeLogTrigger.Changed += Trigger_Changed;
            cmisChangeLogTrigger.EnableRaisingEvents = true;
            Console.WriteLine("CMISリポジトリの監視を開始します。何かキーを押すと終了します。");
            Console.Read();
            cmisChangeLogTrigger.EnableRaisingEvents = false;

        }

        private static void Trigger_Changed(object sender, MakuraDocumentEventArgs e) {
            var action = new CmisUploadAction();
            action.Invoke(e.Path, e.UpdatedDocument);
        }
    }
}
