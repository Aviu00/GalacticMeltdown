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

internal class LevelButton : Button
{
    public string Name { get; }

    public LevelButton(LevelInfo levelInfo, Action<LevelInfo> levelStarter) 
        : base(levelInfo.Name, $"Seed: {levelInfo.Seed}", () => levelStarter(levelInfo))
    {
        Name = levelInfo.Name;
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
        List<Button> buttons = new(levelInfos.Select(levelInfo => new LevelButton(levelInfo, TryStartLevel)));
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
        YesNoDialog levelRemovalDialog = new(RemoveSelectedLevel, "Are you sure you want to remove this level?");
        UserInterface.AddChild(this, levelRemovalDialog);
        levelRemovalDialog.Open();
    }

    private void RemoveSelectedLevel(bool doIt)
    {
        if (!doIt) return;
        FilesystemLevelManager.RemoveLevel(((LevelButton) LineView.GetCurrent()).Name);
        SetLevelButtons();
    }

    private void CreateLevel(string name, int? seed)
    {
        seed ??= Random.Shared.Next();
        Level level= FilesystemLevelManager.CreateLevel(seed.Value, name);
        DataHolder.CurrentSeed = seed.Value;
        Game.StartLevel(level, name);
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