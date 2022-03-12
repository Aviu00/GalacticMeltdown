using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GalacticMeltdown.Data;
using TerrainInformation = Dictionary<char, (string tileId, string lootTableId, int lootTableChance)>;

public class RoomDataExtractor : XmlExtractor
{
    private const int ChunkSize = DataHolder.ChunkSize;

    public readonly List<Room> Rooms;

    private readonly Dictionary<string, TileTypeData> _tileTypes;

    public RoomDataExtractor(Dictionary<string, TileTypeData> tileTypes)
    {
        _tileTypes = tileTypes;
        Rooms = new List<Room>();
        ParseDocument("Rooms.xml");
    }

    private void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            string stringPattern = "";
            int type = 0;
            int chance = 100;
            bool rotationalSymmetry = false;
            bool centralSymmetry = false;
            TerrainInformation terrainInfo = null;
            foreach (XmlNode locNode in node)
            {
                switch (locNode.Name)
                {
                    case "Pattern":
                        stringPattern = locNode.InnerText;
                        break;
                    case "Chance":
                        chance = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "Type":
                        type = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "Terrain":
                        terrainInfo = ParseTerrain(locNode);
                        break;
                    case "RotationalSymmetry":
                        rotationalSymmetry = Convert.ToBoolean(locNode.InnerText);
                        break;
                    case "CentralSymmetry":
                        centralSymmetry = Convert.ToBoolean(locNode.InnerText);
                        break;
                }
            }

            TileInformation[,] interior = ConvertPattern(stringPattern, terrainInfo);
            Rooms.Add(new Room(interior, type, chance, rotationalSymmetry, centralSymmetry));
        }
    }
    
    private TerrainInformation ParseTerrain(XmlNode terrainNode)
    {
        TerrainInformation terrainInfo = new();
        foreach (XmlNode node in terrainNode)
        {
            if (node.Attributes is null) continue;
            char symbol = ' ';
            string tileId = "";
            string lootTableId = null;
            int lootTableChance = 0;
            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "char":
                        symbol = Convert.ToChar(attribute.InnerText);
                        break;
                    case "tile_id":
                        tileId = attribute.InnerText;
                        break;
                    case "loot_table_id":
                        lootTableId = attribute.InnerText;
                        break;
                    case "loot_table_chance":
                        lootTableChance = Convert.ToInt32(attribute.InnerText);
                        break;
                }
            }

            terrainInfo.Add(symbol, (tileId, lootTableId, lootTableChance));
        }

        return terrainInfo;
    }

    private TileInformation[,] ConvertPattern(string stringPattern, TerrainInformation terrainInfo)
    {
        int i = 0;
        int j = -1;
        var roomInterior = new TileInformation[ChunkSize - 1, ChunkSize - 1];
        foreach (char c in stringPattern)
        {
            switch (c)
            {
                case ' ' or '\r':
                    continue;
                case '\n':
                    i = 0;
                    j++;
                    continue;
            }

            TileTypeData data;
            string lootTableId = null;
            int lootTableChance = 0;
            if (terrainInfo is not null && terrainInfo.ContainsKey(c))
            {
                data = _tileTypes[terrainInfo[c].tileId];
                lootTableId = terrainInfo[c].lootTableId;
                lootTableChance = terrainInfo[c].lootTableChance;
            }
            else
            {
                data = _tileTypes.Values.First(tileType => tileType.Symbol == c);
            }
            roomInterior[i, 23 - j] = new TileInformation(data, lootTableId, lootTableChance);
            i++;
        }

        return roomInterior;
    }
}

public readonly record struct Room(TileInformation[,] RoomInterior, int Type, int Chance, bool RotationalSymmetry,
    bool CentralSymmetry);

public struct TileInformation
{
    public TileTypeData TileTypeData;
    public readonly string LootTableId;
    public readonly int LootTableChance;

    public TileInformation(TileTypeData tileTypeData, string lootTableId, int lootTableChance)
    {
        TileTypeData = tileTypeData;
        LootTableId = lootTableId;
        LootTableChance = lootTableChance;
    }
}