using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace DocSigner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string XmlToSignPath = args[0];
            string SignedXmlOutputDir = args[1];
            string certPath = args[2];

            // Read cert
            X509Certificate2 cert2 = new X509Certificate2(certPath);
            RSA rsaPrivateKey = cert2.GetRSAPrivateKey();

            // Read input xml
            XmlDocument xmlDocument = new();
            xmlDocument.Load(new XmlTextReader(XmlToSignPath));

            
            // Create Signature
            XmlElement xmlDigitalSignature = Signature.SignWithXAdES(cert2, xmlDocument);

            // Attach Signature to document
            xmlDocument.DocumentElement?.AppendChild(xmlDocument.ImportNode(xmlDigitalSignature, true));

            // Write whole document with signature into xml file
            XmlTextWriter xmltw = new XmlTextWriter(Path.Combine(SignedXmlOutputDir, "signedFile.xml"), new UTF8Encoding(false));
            xmlDocument.WriteTo(xmltw);
            xmltw.Close();
        }
    }
}
