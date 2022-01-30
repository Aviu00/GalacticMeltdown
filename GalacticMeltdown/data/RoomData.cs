using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GalacticMeltdown.data;

public class RoomData
{
    public List<(int rarity, int exitCount, Room room)> Rooms { get; private set; }

    private readonly Dictionary<char, string> _defaultTiles = new()
    {
        {'.', "floor"},
        {'#', "wall"}
    };

    public RoomData()
    {
        Rooms = new List<(int rarity, int exitCount, Room room)>();
        
        string projectDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        XmlDocument doc = new XmlDocument();
        doc.Load($"{projectDirectory}/../../../data/xml/Rooms.xml");
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            TerrainData.TerrainObject[,] pattern = new TerrainData.TerrainObject[24, 24];
            foreach (XmlNode locNode in node)
            {
                switch (locNode.Name)
                {
                    case "Pattern":
                        Console.WriteLine(locNode.InnerText);
                        pattern = ConvertPattern(locNode.InnerText);
                        break;
                }
            }
            Rooms.Add((0, 4, new Room(pattern)));
        }
    }

    private TerrainData.TerrainObject[,] ConvertPattern(string stringPattern)
    {
        int i = 0;
        int j = -1;
        TerrainData.TerrainObject[,] terrainObjects = new TerrainData.TerrainObject[24, 24];
        foreach (char c in stringPattern)
        {
            switch (c)
            {
                case ' ':
                    continue;
                case '\n':
                    i = 0;
                    j++;
                    continue;
            }
            
            TerrainData.TerrainObject data = GameManager.TerrainData.Data[_defaultTiles[c]];
            terrainObjects[i, j] = data;
            i++;
        }

        return terrainObjects;
    }
        
    public readonly struct Room
    {
        public TerrainData.TerrainObject[,] Pattern { get; }

        public Room(TerrainData.TerrainObject[,] pattern)
        {
            Pattern = pattern;
        }
    }
}