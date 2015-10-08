using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Aegif.Makuranage.Models;
using Aegif.Makuranage.TriggerEngine;
using Aegif.Makuranage.ActionEngine;

namespace Aegif.Makuranage.ConsoleApplication {
    class Program {
        static void Main(string[] args) {
            var trigger = new LocalFileSystemTrigger();

            trigger.Changed += Trigger_Changed;
        }

        private static void Trigger_Changed(object sender, TransferObjectEventArgs e) {
            var action = new CmisUploadAction();
            action.Invole(e.TransferObject);
        }
    }
}
