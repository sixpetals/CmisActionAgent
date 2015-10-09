using Aegif.Makuranage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aegif.Makuranage.TriggerEngine {
    interface ITrigger {
        void PollingStartAsync();
        void PollingStop();

        event MakuraObjectEventHandler Changed;

    }

    public delegate void MakuraObjectEventHandler(object sender, MakuraDocumentEventArgs e);

    public class MakuraDocumentEventArgs : EventArgs {

        public MakuraDocument UpdatedDocument { get; set; }

        public String Path { get; set; } = "";

    }
}
