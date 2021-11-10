using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GalacticMeltdown
{
    public class TerrainData
    {
        public Dictionary<string, TerrainObject> Data { get; private set; }
        public TerrainData()
        {
            Data = new Dictionary<string, TerrainObject>();
            string projectDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            //Console.WriteLine("Dir: " + projectDirectory);
            XmlDocument doc = new XmlDocument();
            doc.Load($"{projectDirectory}/../../../data/xml/Terrain.xml");
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                string name = "";
                bool isWalkable = false;
                bool isTransparent = false;
                char symbol = ' ';
                ConsoleColor color = ConsoleColor.White;
                foreach (XmlNode locNode in node)
                {
                    switch (locNode.Name)
                    {
                        case "Name":
                            name = locNode.InnerText;
                            break;
                        case "Symbol":
                            symbol = Convert.ToChar(locNode.InnerText);
                            break;
                        case "IsWalkable":
                            isWalkable = Convert.ToBoolean(locNode.InnerText);
                            break;
                        case "IsTransparent":
                            isTransparent = Convert.ToBoolean(locNode.InnerText);
                            break;
                        case "Color":
                            color = Utility.Colors[locNode.InnerText];
                            break;
                    }
                }

                TerrainObject obj = new TerrainObject(name, symbol, color, isWalkable, isTransparent);
                Data.Add(name, obj);
            }
        }
        
        public struct TerrainObject
        {
            public string Name { get; }
            public char Symbol { get; }
            public bool IsWalkable { get; }
            public bool IsTransparent { get; }
            public ConsoleColor Color { get; }

            public TerrainObject(string name, char symbol, ConsoleColor color, bool isWalkable, bool isTransparent)
            {
                Symbol = symbol;
                Name = name;
                Color = color;
                IsTransparent = isTransparent;
                IsWalkable = isWalkable;
            }
        }
    }
}