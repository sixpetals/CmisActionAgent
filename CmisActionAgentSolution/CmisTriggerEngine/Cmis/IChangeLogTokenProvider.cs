using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aegif.Makuranage.TriggerEngine.Cmis {
    public interface IChangeLogTokenProvider {
        string GetChangeLogToken();

        void SetChangeLogToken(String changeLog);
    }
}
