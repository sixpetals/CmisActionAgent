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

namespace Aegif.Makuranage.ConsoleApplication {
    class Program {
        static void Main(string[] args) {
            var trigger = new LocalFileSystemTrigger();
            trigger.Changed += Trigger_Changed;
            trigger.EnableRaisingEvents = true;
            Console.WriteLine("ローカルファイルシステムの監視を開始します。何かキーを押すと終了します。");
            Console.Read();
            trigger.EnableRaisingEvents = false;
        }

        private static void Trigger_Changed(object sender, TransferObjectEventArgs e) {
            var action = new CmisUploadAction();
            action.Invoke(e.TransferObject);
        }
    }
}
