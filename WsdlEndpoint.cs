using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Web.Services.Description;
using WsdlDescription = System.Web.Services.Description.ServiceDescription;
using System.Xml;
using System.IO;
using System.Xml.Schema;

namespace Shared.Components.Behaviours
{
    //https://winterdom.com/2006/09/19/asampleiwsdlexportextensionforwcf
    public class WsdlEndpoint : IEndpointBehavior,IWsdlExportExtension
    {
        private string Wsdl = string.Empty;
        public WsdlEndpoint(string wsdl)
        {
            System.Diagnostics.Trace.WriteLine("WsdlEndpoint");

            Wsdl = wsdl;
        }
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
           
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            //not needed

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
           
        }

        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
            throw new NotImplementedException();
            // never called
        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
           /*
            System.Diagnostics.Trace.WriteLine("Count " + exporter.GeneratedWsdlDocuments.Count);

            foreach (WsdlDescription item in exporter.GeneratedWsdlDocuments)
            {
                System.Diagnostics.Trace.WriteLine("GeneratedWsdlDocument-name:" + item.Name);
                System.Diagnostics.Trace.WriteLine("GeneratedWsdlDocument-uri:" + item.RetrievalUrl);

                MemoryStream mem = new MemoryStream();
                item.Write(mem);
                mem.Position = 0;
                StreamReader rdr = new StreamReader(mem);
                System.Diagnostics.Trace.WriteLine(rdr.ReadToEnd());

                

            }

            foreach (XmlSchema schema in exporter.GeneratedXmlSchemas.Schemas())
            {
                System.Diagnostics.Trace.WriteLine("GeneratedXmlSchemas-Id:" + schema.Id);
                System.Diagnostics.Trace.WriteLine("GeneratedXmlSchemas-uri:" + schema.SourceUri);

                MemoryStream mem = new MemoryStream();
                schema.Write(mem);
                mem.Position = 0;
                StreamReader rdr = new StreamReader(mem);
                System.Diagnostics.Trace.WriteLine(rdr.ReadToEnd());



            }
            */
            RemoveAllWsdlDescription(exporter.GeneratedWsdlDocuments);
            RemoveAllSchemas(exporter.GeneratedXmlSchemas);

            string targetNamespace = String.Empty;
            WsdlDescription desc = null;

            using (FileStream wsdlfile = new FileStream(Wsdl, FileMode.Open, FileAccess.Read))
            {
                desc = WsdlDescription.Read(wsdlfile, false);
            }
                

            foreach (XmlSchema schema in desc.Types.Schemas)
            {
                MemoryStream mem = new MemoryStream();

                targetNamespace = schema.TargetNamespace;
                schema.Write(mem);
                mem.Position = 0;

              
                XmlSchema newschema = XmlSchema.Read(mem, null);
                exporter.GeneratedXmlSchemas.Add(newschema);


            }

            desc.Types.Schemas.Clear();

            string import = String.Format(@"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'> 
                      <xsd:import namespace='{0}' /> 
                    </xsd:schema>", targetNamespace);

            XmlSchema descSchema = XmlSchema.Read(new StringReader(import), null);
            desc.Types.Schemas.Add(descSchema);

            exporter.GeneratedWsdlDocuments.Add(desc);
           

        }

        public void Validate(ServiceEndpoint endpoint)
        {
            
        }

        private void RemoveAllSchemas(XmlSchemaSet set)
        {
            System.Collections.IEnumerator en = set.Schemas().GetEnumerator();

            while (en.MoveNext())
            {
               

                XmlSchema item = (XmlSchema)en.Current;
                

                set.Remove(item);
                //reset XmlSchemaSet as we have changed it
                en = set.Schemas().GetEnumerator();

            }

        }
        private void RemoveAllWsdlDescription(ServiceDescriptionCollection set)
        {
            System.Collections.IEnumerator en = set.GetEnumerator();

            while (en.MoveNext())
            {

                WsdlDescription item = (WsdlDescription)en.Current;

               

                set.Remove(item);
                //reset XmlSchemaSet as we have changed it
                en = set.GetEnumerator();

            }

        }
    }
}
