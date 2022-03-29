using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class AmmoDialog : ChoiceDialog<string>
{
    public AmmoDialog(Player player, Action<string> send) : base(GetChoices(player), send, "Choose ammo type")
    {
    }

    private static List<(string text, string choice)> GetChoices(Player player)
    {
        var availableAmmoTypes = Enum.GetValues<ItemCategory>()
            .SelectMany(category => player.Inventory)
            .DistinctBy(item => item.Id)
            .Where(item => ((WeaponItem) player.Equipment[BodyPart.Hands]).AmmoTypes.ContainsKey(item.Id));
        var choices = new List<(string text, string choice)> {("Unset ammo", null)};
        choices.AddRange(availableAmmoTypes.Select(item => (item.Name, item.Id)).OrderBy(tuple => tuple.Name));
        return choices;
    }
}