using System.Collections.Generic;

namespace GalacticMeltdown.Data;

public static class MapData
{
    public static readonly Dictionary<string, TileTypeData> TileTypes;
    public static readonly Dictionary<string, ItemData> ItemTypes;
    public static readonly Dictionary<string, ILoot> LootTables;
    public static readonly Dictionary<string, NpcTypeData> EnemyTypes;
    public static readonly List<RoomType> RoomTypes;
    
    static MapData()
    {
        TileTypes = new TileTypesExtractor().TileTypes;
        EnemyTypes = new NpcTypesExtractor().NpcTypes;
        ItemTypes = new ItemTypesExtractor().ItemTypes;
        LootTables = new LootTableDataExtractor().LootTables;
        RoomTypes = new RoomTypesExtractor(TileTypes).Rooms;
    }
}