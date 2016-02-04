using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Elmah.Extensions
{
    public class FallbackErrorLogSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            IList<IDictionary> childNodes = new List<IDictionary>();

            for(int i = 0; i < section.ChildNodes.Count; ++i)
            {
                var currChildNode = section.ChildNodes.Item(i);
                if (currChildNode.NodeType != XmlNodeType.Element) continue;
                CheckForChildNodes(currChildNode);
                childNodes.Add(GetHashtable(currChildNode));
            }

            if(childNodes.Count < 1)
            {
                throw new ConfigurationErrorsException("No error loggers defined.");
            }

            Hashtable hashtable = new Hashtable();
            hashtable["childrenLogsConfiguration"] = childNodes;
            hashtable["type"] = typeof(FallbackErrorLog).AssemblyQualifiedName.ToString();

            return hashtable;
        }

        private IDictionary GetHashtable(XmlNode node)
        {
            Hashtable hashtable = new Hashtable();
            foreach (XmlAttribute xmlAttribute in node.Attributes)
            {
                hashtable[xmlAttribute.Name] = xmlAttribute.Value;
            }

            return hashtable;
        }

        private void CheckForChildNodes(XmlNode node)
        {
            if (node.HasChildNodes)
            {
                throw new ConfigurationErrorsException("Child nodes not allowed.", node.FirstChild);
            }
        }
    }
}
