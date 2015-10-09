using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aegif.Makuranage.Models;
using DotCMIS;
using System.IO;
using DotCMIS.Data.Impl;
using DotCMIS.Client;
using Aegif.Makuranage.ActionEngine.Cmis;

namespace Aegif.Makuranage.ActionEngine.Cmis {
    public class CmisUploadAction : IAction {

        public String path { get; set; }
        public CmisConnector conn { get; set; }

        public CmisUploadAction() {
            this.conn = conn = new CmisConnector();
        }

        public void Invoke(MakuraObject obj) {
            var session = conn.CreateSession();
            var folder = session.GetRootFolder();


            Upload(folder, obj);
        }

        private void Upload(IFolder uploadFolder, MakuraObject obj) {
            if(obj is MakuraDocument) {
                Upload(uploadFolder, obj as MakuraDocument);
            }else if (obj is MakuraFolder) {
                var makuraFolder = obj as MakuraFolder;
                var newTargetFolder = Upload(uploadFolder, makuraFolder);
                makuraFolder.MakuraObjects.ForEach(p => Upload(newTargetFolder, p));
            }
        }


        private IFolder Upload(IFolder uploadFolder, MakuraFolder folder) {
            var existFolder = uploadFolder.GetChildren().OfType<IFolder>().FirstOrDefault(p => p.Name == folder.Name);
            if (existFolder == null) {
                var properties = new Dictionary<string, object>();
                properties[PropertyIds.Name] = folder.Name;
                properties[PropertyIds.ObjectTypeId] = "cmis:folder";
                return uploadFolder.CreateFolder(properties);
            } else {
                return existFolder;
            }

        }


        private IDocument Upload(IFolder uploadFolder, MakuraDocument document) {
            var cmisDocumentStream = document.ContentStream.ConvertCmis();
            var existDoc = uploadFolder.GetChildren().OfType<IDocument>().FirstOrDefault(p => p.Name == document.Name);

            if (existDoc == null) {
                var properties = new Dictionary<string, object>();
                properties[PropertyIds.Name] = document.Name;
                properties[PropertyIds.ObjectTypeId] = "cmis:document";

                return uploadFolder.CreateDocument(properties, cmisDocumentStream, DotCMIS.Enums.VersioningState.Minor);
            } else {
                existDoc.SetContentStream(cmisDocumentStream,true);
                return existDoc;
            }
        }


    }

    public static class Util {
        public static ContentStream ConvertCmis(this MakuraContentStream makura) {
            var contentStream = new ContentStream();
            contentStream.FileName = makura.FileName;
            contentStream.MimeType = makura.MimeType;
            contentStream.Length = makura.Length;
            contentStream.Stream = makura.Stream;
            return contentStream;
        }

    }
}



