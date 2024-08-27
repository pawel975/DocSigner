using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DocSigner
{
    public class XadesSignedXml : SignedXml
    {
        public const string XmlDsigSignatureProperties = "http://uri.etsi.org/01903#SignedProperties";
        public const string XadesProofOfApproval = "http://uri.etsi.org/01903/v1.2.2#ProofOfApproval";
        public const string XadesPrefix = "xades";
        public const string XadesNamespaceUrl = "http://uri.etsi.org/01903/v1.3.2#";
        public XmlElement PropertiesNode { get; set; }

        private readonly List<DataObject> _dataObjects = new List<DataObject>();

        public XadesSignedXml(XmlDocument document) : base(document) { }

        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            if (string.IsNullOrEmpty(idValue))
                return null;

            var xmlElement = base.GetIdElement(document, idValue);
            if (xmlElement != null)
                return xmlElement;

            if (_dataObjects.Count == 0)
                return null;

            foreach (var dataObject in _dataObjects)
            {
                var nodeWithSameId = Helpers.FindNodeWithPrefix(dataObject.Data, idValue);
                if (nodeWithSameId != null)
                    return nodeWithSameId;
            }

            return null;
        }

        public new void AddObject(DataObject dataObject)
        {
            base.AddObject(dataObject);
            _dataObjects.Add(dataObject);
        }
    }
}
