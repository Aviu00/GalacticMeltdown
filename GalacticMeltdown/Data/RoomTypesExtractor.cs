using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace GalacticMeltdown.Data;
using TerrainInformation = Dictionary<char, (string tileId, string lootId, int lootChance, double gain, int limit)>;

public class RoomTypesExtractor : XmlExtractor
{
    private const int ChunkSize = DataHolder.ChunkSize;

    public readonly List<RoomTypes> Rooms;

    private readonly Dictionary<string, TileTypeData> _tileTypes;

    public RoomTypesExtractor(Dictionary<string, TileTypeData> tileTypes)
    {
        _tileTypes = tileTypes;
        Rooms = new List<RoomTypes>();
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
            Rooms.Add(new RoomTypes(interior, type, chance, rotationalSymmetry, centralSymmetry));
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
            string lootId = null;
            int lootChance = 100;
            double gain = 0;
            int limit = 100;
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
                    case "loot_id":
                        lootId = attribute.InnerText;
                        break;
                    case "loot_chance":
                        lootChance = Convert.ToInt32(attribute.InnerText);
                        break;
                    case "gain":
                        gain = Convert.ToDouble(attribute.InnerText, CultureInfo.InvariantCulture);
                        break;
                    case "limit":
                        limit = Convert.ToInt32(attribute.InnerText);
                        break;
                }
            }

            terrainInfo.Add(symbol, (tileId, lootId, lootChance, gain, limit));
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
            int limit = 100;
            double gain = 0;
            if (terrainInfo is not null && terrainInfo.ContainsKey(c))
            {
                data = _tileTypes[terrainInfo[c].tileId];
                lootTableId = terrainInfo[c].lootId;
                lootTableChance = terrainInfo[c].lootChance;
                gain = terrainInfo[c].gain;
                limit = terrainInfo[c].limit;
            }
            else
            {
                data = _tileTypes.Values.First(tileType => tileType.Symbol == c);
            }
            roomInterior[i, 23 - j] = new TileInformation(data, lootTableId, lootTableChance, gain, limit);
            i++;
        }

        return roomInterior;
    }
}

public record RoomTypes(TileInformation[,] RoomInterior, int Type, int Chance, bool RotationalSymmetry,
    bool CentralSymmetry);

public struct TileInformation
{
    public TileTypeData TileTypeData;
    public readonly string LootId;
    public readonly int LootChance;
    public readonly double Gain;
    public readonly int Limit;

    public TileInformation(TileTypeData tileTypeData, string lootId, int lootChance, double gain, int limit)
    {
        TileTypeData = tileTypeData;
        LootId = lootId;
        LootChance = lootChance;
        Gain = gain;
        Limit = limit;
    }
}