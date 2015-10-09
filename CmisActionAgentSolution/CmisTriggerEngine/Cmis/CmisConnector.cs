using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aegif.Makuranage.TriggerEngine.Cmis
{
    public class CmisConnector
    {

        public String User { get; set; } = "admin";

        public String Password { get; set; } = "admin";

        public String RepositoryId { get; set; } = "bedroom";

        public String AtomPubUrl { get; set; } = "http://trial.nemakiware.com:8080/core/atom/bedroom/";
        
        public CmisConnector() {

        }

        public ISession CreateSession() {
            var parameters = new Dictionary<string, string>();

            parameters[SessionParameter.BindingType] = BindingType.AtomPub;
            parameters[SessionParameter.AtomPubUrl] = AtomPubUrl;
            parameters[SessionParameter.User] = User;
            parameters[SessionParameter.Password] = Password;
            parameters[SessionParameter.RepositoryId] = RepositoryId;

            var factory = SessionFactory.NewInstance();
            var session = factory.CreateSession(parameters);
            return session;
        }
    }
}
