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
            new Button($"{levelInfo.Name}", $"seed: {levelInfo.Seed}", () => TryStartLevel(levelInfo.Path))));
        LineView.SetLines(buttons.Cast<ListLine>().ToList());
    }

    private void OpenLevelCreationMenu()
    {
        LevelCreationDialog creationDialog = new(CreateLevel);
        UserInterface.AddChild(this, creationDialog);
        creationDialog.Open();
    }

    private void OpenLevelRemovalDialog()
    {
        
    }

    private void CreateLevel(string name, int? seed)
    {
        string path = FilesystemLevelManager.CreateLevel(seed ?? Random.Shared.Next(), name);
        var (level, loadedSeed) = FilesystemLevelManager.GetLevel(path);
        DataHolder.CurrentSeed = loadedSeed;
        Game.StartLevel(level, path);
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