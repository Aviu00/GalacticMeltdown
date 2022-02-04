using System.IO;
using System.Reflection;
using System.Xml;

namespace GalacticMeltdown.data;

public abstract class XmlExtractor
{
    private readonly string _projectDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
    protected XmlDocument GetXmlDocument(string name)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load($"{_projectDirectory}/../../../data/xml/{name}");
        return doc;
    }
}