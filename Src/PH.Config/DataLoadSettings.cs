using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Config
{
    public class DataLoadSettings
    {

        private CredentialSettings _credentials;
        
        public DataLoadSettings()
        {
            _credentials = new CredentialSettings();
       
        }
        public CredentialSettings Credentials
        {
            get { return _credentials; }
        }
        public int TeamId { get; set; }
        
        public int StoryPointFieldId { get; set; }
    }
}
