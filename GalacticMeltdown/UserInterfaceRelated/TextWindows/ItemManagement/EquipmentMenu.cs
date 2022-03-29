using System;
using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows.ItemManagement;

public class EquipmentMenu : TextWindow
{
    private Player _player;
    
    public EquipmentMenu(Player player)
    {
        _player = player;
        SetEquipmentLines();
        Controller = new ActionController(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Back, Close},
                {SelectionControl.Select, LineView.PressCurrent},
                {SelectionControl.Down, LineView.SelectNext},
                {SelectionControl.Up, LineView.SelectPrev},
            }));
    }

    private void SetEquipmentLines()
    {
        List<ListLine> lines = new();
        foreach (BodyPart bodyPart in Enum.GetValues<BodyPart>())
        {
            if (_player.Equipment[bodyPart] is null)
            {
                lines.Add(new TextLine($"{DataHolder.BodyPartName[bodyPart]}: nothing"));
            }
            else
            {
                lines.Add(new Button($"{DataHolder.BodyPartName[bodyPart]}: {_player.Equipment[bodyPart].Name}", "",
                    () => UnequipAndUpdate(bodyPart)));
            }
        }
        LineView.SetLines(lines);
    }

    private void UnequipAndUpdate(BodyPart bodyPart)
    {
        _player.Unequip(bodyPart);
        SetEquipmentLines();
    }
}