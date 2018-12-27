using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Configuration;
namespace Shared.Components.Behaviours
{
    //https://winterdom.com/2006/09/19/asampleiwsdlexportextensionforwcf
    public class WsdlExtension: BehaviorExtensionElement
    {
        
        public override Type BehaviorType
        {
            get { return typeof(WsdlEndpoint); }
        }

        protected override object CreateBehavior()
        {
            // Create the  endpoint behavior that will insert the message  
            // inspector into the client runtime  
            return new WsdlEndpoint(Wsdl);
        }


        [ConfigurationProperty("Wsdl", DefaultValue = "", IsRequired = true)]
        public string Wsdl
        { 
            get { return (string)base["Wsdl"]; } 
            set { base["Wsdl"] = value; } 
        } 

    }
}
