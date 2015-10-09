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

        public void Invoke(String path, MakuraDocument doc) {
            var session = conn.CreateSession();

            var targetFolder = SetTargetFolder(path, session);
            Upload(targetFolder, doc);
        }

        private static IFolder SetTargetFolder(string path, ISession session) {
            //目的の場所までフォルダを掘りながら下る
            var currentFolder = session.GetRootFolder();
            var folderNames = path.Split('/');

            foreach (var targetFolderName in folderNames) {
                if (String.IsNullOrEmpty(targetFolderName)) continue;
                var existFolder = currentFolder.GetChildren().OfType<IFolder>().FirstOrDefault(p => p.Name == targetFolderName);
                if (existFolder == null) {
                    var properties = new Dictionary<string, object>();
                    properties[PropertyIds.Name] = targetFolderName;
                    properties[PropertyIds.ObjectTypeId] = "cmis:folder";
                    currentFolder = currentFolder.CreateFolder(properties);
                } else {
                    currentFolder = existFolder;
                }

            }

            return currentFolder;
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



