using System;
using GalacticMeltdown.Items;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class ItemButton : Button
{
    public Item StoredItem { get; }

    public ItemButton(Item item, Action<Item> openItemScreen, int? count = null) : base(item.Name,
        count is null ? "" : count.ToString(), () => openItemScreen(item))
    {
        StoredItem = item;
    }

    public override ViewCellData this[int x] 
    {
        get
        {
            return x switch
            {
                0 => new ViewCellData(StoredItem.SymbolData, StoredItem.BgColor ?? BgColor),
                1 => new ViewCellData(null, BgColor),
                _ => new ViewCellData((RenderedText[x - 2], TextColor), BgColor)
            };
        }
    }

    public override void SetWidth(int width)
    {
        base.SetWidth(width);
        RenderedText = UtilityFunctions.RenderText(width - 2, TextLeft, TextRight);
    }
}