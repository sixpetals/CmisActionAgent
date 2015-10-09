using Aegif.Makuranage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aegif.Makuranage.TriggerEngine {
    interface ITrigger {
        event TransferObjectEventHandler Changed;
        bool EnableRaisingEvents {get;set;}
    }

    public delegate void TransferObjectEventHandler(object sender, TransferObjectEventArgs e);

    public class TransferObjectEventArgs : EventArgs {
        public MakuraObject TransferObject { get; set; }
    }
}
