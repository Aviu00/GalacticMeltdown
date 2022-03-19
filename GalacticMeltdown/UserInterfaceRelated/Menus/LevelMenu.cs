using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Launchers;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

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
                {LevelMenuControl.Start, LineView.PressCurrent},
                {LevelMenuControl.GoBack, Close},
                {LevelMenuControl.Create, OpenLevelCreationMenu},
                {LevelMenuControl.Delete, OpenLevelRemovalDialog}
            }));
    }

    private void SetLevelButtons()
    {
        List<LevelInfo> levelInfos = FilesystemLevelManager.GetLevelInfo();
        List<Button> buttons = new(levelInfos.Select(levelInfo =>
            new Button($"{levelInfo.Name}", $"seed: {levelInfo.Seed}", () => TryStartLevel(levelInfo.Path))));
        LineView.SetLines(buttons.Cast<ListLine>().ToList());
    }

    private void OpenLevelCreationMenu()
    {
        
    }

    private void OpenLevelRemovalDialog()
    {
        
    }

    private void TryStartLevel(string path)
    {
        var (level, seed) = FilesystemLevelManager.GetLevel(path);
        if (level is null)
        {
            SetLevelButtons();
            // TODO: failure animation
            return;
        }

        DataHolder.CurrentSeed = seed;
        Game.StartLevel(level, path);
    }
}