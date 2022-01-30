using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GalacticMeltdown.data
{
    public class TerrainData
    {
        public Dictionary<string, TileData> Data { get; }
        public TerrainData()
        {
            // Temporary. TODO: decide whether to create a class for each tile type or specify them using strings 
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

                TileData tileData = new TileData(symbol, color, isWalkable, isTransparent);
                Data.Add(name, tileData);
            }
        }
        
        public readonly struct TileData
        {
            public char Symbol { get; }
            public bool IsWalkable { get; }
            public bool IsTransparent { get; }
            public ConsoleColor Color { get; }

            public TileData(char symbol, ConsoleColor color, bool isWalkable, bool isTransparent)
            {
                Symbol = symbol;
                Color = color;
                IsTransparent = isTransparent;
                IsWalkable = isWalkable;
            }
        }
    }
}