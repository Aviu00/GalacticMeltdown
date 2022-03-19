using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

internal class WorldButton : Button
{
    public string Path;

    public WorldButton(LevelInfo info, Action<string> levelStarter)
        : base($"{info.Name}", $"seed: {info.Seed}", () => levelStarter(info.Path))
    {
        Path = info.Path;
    }
}

public class LevelMenu : Menu
{
    public LevelMenu()
    {
        LineView = new LineView();
        SetLevelButtons();
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.LevelMenu,
            new Dictionary<LevelMenuControl, Action>
            {
                {LevelMenuControl.SelectNext, LineView.SelectNext},
                {LevelMenuControl.SelectPrev, LineView.SelectPrev},
                {LevelMenuControl.Start, StartCurrentLevel},
                {LevelMenuControl.GoBack, Close},
                {LevelMenuControl.Create, OpenLevelCreationMenu},
                {LevelMenuControl.Delete, OpenLevelRemovalDialog}
            }));
    }

    private void SetLevelButtons()
    {
        
    }

    private void OpenLevelCreationMenu()
    {
        
    }

    private void OpenLevelRemovalDialog()
    {
        
    }

    private void StartCurrentLevel()
    {
        
    }

    private void TryStartLevel(string path)
    {
        
    }
}