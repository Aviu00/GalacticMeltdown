using System.Xml;

namespace GalacticMeltdown.Data;

public abstract class XmlExtractor
{
    protected XmlDocument GetXmlDocument(string name)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load($"{FileSystemInfo.ProjectDirectory}/Data/xml/{name}");
        return doc;
    }
}