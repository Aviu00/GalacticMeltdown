using System;
using System.IO;
using System.Xml;

namespace GalacticMeltdown.Data;

public abstract class XmlExtractor
{
    private readonly string _projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));

    protected XmlDocument GetXmlDocument(string name)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load($"{_projectDirectory}/Data/xml/{name}");
        return doc;
    }
}