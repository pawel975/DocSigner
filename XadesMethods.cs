using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DocSigner
{
    public class XadesMethods
    {
        public static DataObject CreateXadesBesDataObject(string xadesSignedPropertiesId)
        {
            // Create a new DataObject
            DataObject dataObject = new();
            XmlDocument doc = dataObject.GetXml().OwnerDocument;

            // Create QualifyingProperties element with namespace
            XmlElement qualifyingProperties = doc.CreateElement("xades:QualifyingProperties", "http://uri.etsi.org/ts/101948/v1.2.1/");
            doc.AppendChild(qualifyingProperties);

            // Create SignedProperties element with namespace
            XmlElement signedProperties = doc.CreateElement("xades:SignedProperties", "http://uri.etsi.org/ts/101948/v1.2.1/");
            signedProperties.SetAttribute("Id", $"ID-{xadesSignedPropertiesId}");
            qualifyingProperties.AppendChild(signedProperties);

            // Create SignedSignatureProperties element with namespace
            XmlElement signedSignatureProperties = doc.CreateElement("xades:SignedSignatureProperties", "http://uri.etsi.org/ts/101948/v1.2.1/");
            signedProperties.AppendChild(signedSignatureProperties);

            // Add specific properties to SignedSignatureProperties (e.g., creation time, signer name)
            XmlElement creationTime = doc.CreateElement("xades:SigningTime");
            creationTime.InnerText = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            signedSignatureProperties.AppendChild(creationTime);

            // Create SignedDataObjectProperties element with namespace
            XmlElement signedDataObjectProperties = doc.CreateElement("xades:SignedDataObjectProperties", "http://uri.etsi.org/ts/101948/v1.2.1/");
            signedProperties.AppendChild(signedDataObjectProperties);

            // Add specific properties to SignedDataObjectProperties (e.g., data object format, timestamps)

            // Create UnsignedProperties element with namespace (if needed)
            // XmlElement unsignedProperties = dataObject.Document.CreateElement("xades:UnsignedProperties", "http://uri.etsi.org/ts/101948/v1.2.1/");
            // qualifyingProperties.AppendChild(unsignedProperties);

            dataObject.Data = doc.ChildNodes;

            return dataObject;
        }

        public static byte[] ComputeHash(XmlDocument xmlDoc)
        {
            // Replace with your desired hash algorithm (e.g., SHA256)
            using SHA256 hashAlgorithm = SHA256.Create();
            XmlWriterSettings settings = new();

            settings.Encoding = Encoding.UTF8;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (MemoryStream memoryStream = new MemoryStream()) // Create a MemoryStream
            {
                using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
                {
                    xmlDoc.WriteTo(writer);
                    writer.Flush();
                    return hashAlgorithm.ComputeHash(memoryStream.ToArray()); // Use memoryStream.ToArray() to get the byte array
                }
            }
        }
    }
}

