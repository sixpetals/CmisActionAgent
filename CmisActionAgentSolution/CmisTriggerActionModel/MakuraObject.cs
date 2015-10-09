using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aegif.Makuranage.Models {
    public class MakuraObject {
        public String Name;
    }


    public class MakuraDocument : MakuraObject {
        public MakuraContentStream ContentStream { get; set; } 
    }

    public class MakuraFolder : MakuraObject {
        public List<MakuraObject> MakuraObjects {
            get {
                var list = new List<MakuraObject>();
                list.AddRange(Folders);
                list.AddRange(Documents);
                return list;
            }
        }

        public List<MakuraDocument> Documents = new List<MakuraDocument>();
        public List<MakuraFolder> Folders = new List<MakuraFolder>();
    }

    public class MakuraContentStream  {
        public long? Length { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public Stream Stream { get; set; }
    }
}
