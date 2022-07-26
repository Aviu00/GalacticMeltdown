using System;
using GalacticMeltdown.Actors;

namespace GalacticMeltdown.Events;

public class EquipmentChangeEventArgs : EventArgs
{
    public Equipment Equipment { get; }

    public EquipmentChangeEventArgs(Equipment equipment)
    {
        Equipment = equipment;
    }
}