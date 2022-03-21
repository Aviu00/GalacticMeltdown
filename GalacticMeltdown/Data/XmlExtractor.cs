using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GalacticMeltdown.Data;

public abstract class XmlExtractor
{
    private readonly string _projectDirectory = Environment.CurrentDirectory;

    protected XmlDocument GetXmlDocument(string name)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load($"{_projectDirectory}/Data/xml/{name}");
        return doc;
    }
}