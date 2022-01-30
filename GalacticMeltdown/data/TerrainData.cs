using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GalacticMeltdown.data
{
    public class TerrainData
    {
        public Dictionary<string, TileData> Data { get; private set; }
        public TerrainData()
        {
            Data = new Dictionary<string, TileData>();
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

                TileData obj = new TileData(name, symbol, color, isWalkable, isTransparent);
                Data.Add(name, obj);
            }
        }
        
        public readonly struct TileData
        {
            public string Name { get; }
            public char Symbol { get; }
            public bool IsWalkable { get; }
            public bool IsTransparent { get; }
            public ConsoleColor Color { get; }

            public TileData(string name, char symbol, ConsoleColor color, bool isWalkable, bool isTransparent)
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