using System.Xml;

namespace GalacticMeltdown.Data;

public abstract class XmlExtractor
{
    protected XmlDocument GetXmlDocument(string name)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load($"{DataHolder.ProjectDirectory}/Data/xml/{name}");
        return doc;
    }
}