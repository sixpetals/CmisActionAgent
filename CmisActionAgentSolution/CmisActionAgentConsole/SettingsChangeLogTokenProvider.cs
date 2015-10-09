using Aegif.Makuranage.TriggerEngine.Cmis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aegif.Makuranage.ConsoleApplication {
    public class SettingsChangeLogTokenProvider : IChangeLogTokenProvider {
        public string GetChangeLogToken() {
            return Properties.Settings.Default.LatestChangeToken;
        }

        public void SetChangeLogToken(string changeLog) {
            Properties.Settings.Default.LatestChangeToken = changeLog;
            Properties.Settings.Default.Save();
        }
    }
}
