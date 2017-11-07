using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Config
{
    public class CredentialSettings
    {
        public string UrlBase { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public bool IsDefined
        {
            get
            {
                return !(string.IsNullOrEmpty(UrlBase) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword));
            }
        }
        
    }
}
