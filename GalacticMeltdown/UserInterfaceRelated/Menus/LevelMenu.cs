using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Launchers;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
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
        if (!levelInfos.Any())
        {
            LineView.SetLines(new List<ListLine>
            {
                new TextLine("Press C to create a level")
            });
            return;
        }
        List<Button> buttons = new(levelInfos.Select(levelInfo =>
            new Button($"{levelInfo.Name}", $"seed: {levelInfo.Seed}", () => TryStartLevel(levelInfo))));
        LineView.SetLines(buttons.Cast<ListLine>().ToList());
    }

    private void OpenLevelCreationMenu()
    {
        int seed = Random.Shared.Next(0, 1000000000);
        Level level = FilesystemLevelManager.CreateLevel(seed, "Test");
        Game.StartLevel(level, "Test");
    }

    private void OpenLevelRemovalDialog()
    {
        
    }

    private void TryStartLevel(LevelInfo levelInfo)
    {
        Level level = FilesystemLevelManager.GetLevel(levelInfo.Name);
        if (level is null)
        {
            SetLevelButtons();
            // TODO: failure animation
            return;
        }

        DataHolder.CurrentSeed = levelInfo.Seed;
        Game.StartLevel(level, levelInfo.Name);
    }
}