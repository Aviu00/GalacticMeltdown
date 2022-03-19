using System;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated.Rendering;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

internal class WorldButton : Button
{
    private LevelInfo _levelInfo;

    public WorldButton(LevelInfo info, Action<string> levelStarter)
        : base($"{info.Name}", $"seed: {info.Seed}", () => levelStarter(info.Path))
    {
    }
}

public class LevelMenu : Menu
{
    public LevelMenu()
    {
        
    }

    private void OpenLevelCreationMenu()
    {
        
    }

    private void OpenLevelRemovalDialog()
    {
        
    }

    private void TryStartLevel(string path)
    {
        
    }
}