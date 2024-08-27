using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DocSigner
{
    public static class Helpers
    {
        public static XmlElement FindNodeWithPrefix(XmlNodeList nodeList, string idValue)
        {
            foreach (XmlNode node in nodeList)
            {
                var parts = node.Name.Split(':'); //Because name has format "prefix:name"
                string name = parts.Length >= 2 ? parts[1] : parts[0];
                if (name == idValue) return (XmlElement)node;

                if (!node.HasChildNodes) continue;

                var foundNode = FindNodeWithPrefix(node.ChildNodes, idValue);

                if (foundNode != null) return foundNode;
            }
            return null;
        }
    }
}
