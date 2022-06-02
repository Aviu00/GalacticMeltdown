using System.Collections.Generic;

namespace GalacticMeltdown.Data;

public record Language(string Code, string Name, List<string> InfoLines);

public static class Text
{
    public static Language English = new("en", "English", new List<string>
    {
        "Galactic Meltdown",
        "",
        "You are an astronaut, caught stranded on a damaged spaceship that has been taken over by hostile alien " +
        "forces. The only way you can survive is by making a daring escape through the teleport pad located " +
        "somewhere on the ship. You have to find a way out or you will be assimilated by terrible creatures and " +
        "become one of them. You have a limited amount of time, so all you can do is grab a few spare items on the " +
        "ship and make a run for it.",
        "",
        "This game is turn-based: each entity has some energy that it can use to perform actions during a turn.", 
        "After an entity performs an action, all other entities close to the player may perform an action as well.",
        "A turn finishes when no entities spend energy when performing their action.",
        "At the end of each turn all entities restore their energy.",
        "The amount restored is the maximum possible amount available to the entity.",
        "",
        "Key bindings",
        "Use 1-9 or arrows to move, hit, or open doors",
        "Use Escape to go back",
        "",
        "In game:",
        "    Esc: open pause menu",
        "    O: get a cursor for opening doors",
        "    P: get a cursor for picking up items",
        "    U: get a cursor to shoot",
        "    C: get a cursor for examining objects",
        "    D: wait for other creatures to move",
        "    S: skip a turn",
        "    E: open inventory",
        "    A: open ammo selection menu",
        "    /: open console",
        "",
        "Cursor:",
        "    L: toggle line",
        "    F: toggle focus",
        "    Enter: interact with an object",
        "",
        "Level menu:",
        "    C: create level",
        "    D: delete level",
        "",
        "Level creation dialog:",
        "    Enter: select text field to type in",
        "    Tab: create the level",
        "",
        "Inventory:",
        "    Enter: open item dialog",
        "    Q: open equipment menu",
        "    C: open category selection",
        "    /: open item search",
    });
}