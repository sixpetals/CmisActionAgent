using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aegif.Makuranage.Models;

namespace Aegif.Makuranage.ActionEngine {
    public interface IAction{
        void Invoke(String path, MakuraDocument doc);
    }
}
