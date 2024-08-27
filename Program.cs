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

            // Create signature
            SignedXml signedXml = new SignedXml(xmlDocument) { SigningKey = rsaPrivateKey };

            // Create File Reference
            Reference reference = new Reference() { Uri = "" };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);

            // Create a KeyInfo object
            KeyInfo keyInfo = new KeyInfo();
            RSA publicKey = cert2.GetRSAPublicKey();
            keyInfo.AddClause(new RSAKeyValue(publicKey));
            signedXml.KeyInfo = keyInfo;

            // Create Xades Reference
            string xadesSignedPropertiesId = Guid.NewGuid().ToString();
            Reference xadesReference = new() { Uri = $"#ID-{xadesSignedPropertiesId}" };

            // Attach xades Reference 
            signedXml.AddReference(xadesReference);

            // Attach xades
            signedXml.AddObject(XadesMethods.CreateXadesBesDataObject(xadesSignedPropertiesId));

            // Compute signature
            signedXml.ComputeSignature();

            Console.WriteLine(signedXml.CheckSignature());

            // Assign signature to document
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            xmlDocument.DocumentElement?.AppendChild(xmlDocument.ImportNode(xmlDigitalSignature, true));
            Console.WriteLine(signedXml.CheckSignature());

            //if (xmlDocument.FirstChild is XmlDeclaration)
            //{
            //    xmlDocument.RemoveChild(xmlDocument.FirstChild);
            //}

            XmlTextWriter xmltw = new XmlTextWriter(Path.Combine(SignedXmlOutputDir, "signedFile.xml"), new UTF8Encoding(false));
            xmlDocument.WriteTo(xmltw);
            xmltw.Close();
        }
    }
}
