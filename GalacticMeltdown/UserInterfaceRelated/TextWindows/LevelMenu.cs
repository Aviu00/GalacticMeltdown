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

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

internal class LevelButton : Button
{
    public string Path { get; }

    public LevelButton(LevelInfo levelInfo, Action<LevelInfo> levelStarter) 
        : base(levelInfo.Name, $"Seed: {levelInfo.Seed}", () => levelStarter(levelInfo))
    {
        Path = levelInfo.Path;
    }
}

public class LevelMenu : TextWindow
{
    private bool _noLevels;
    
    public LevelMenu()
    {
        SetLevelButtons();
        Controller = new ActionController(UtilityFunctions.JoinDictionaries(KeyBindings.LevelMenu,
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
            _noLevels = true;
            LineView.SetLines(new List<ListLine>
            {
                new TextLine("Press C to create a level")
            });
            return;
        }

        _noLevels = false;
        List<Button> buttons = new(levelInfos.OrderBy(info => info.Name)
            .Select(levelInfo => new LevelButton(levelInfo, TryStartLevel)));
        LineView.SetLines(buttons.Cast<ListLine>().ToList(), true);

        void TryStartLevel(LevelInfo levelInfo)
        {
            Level level = FilesystemLevelManager.GetLevel(levelInfo.Path);
            if (level is null)
            {
                SetLevelButtons();
                return;
            }

            Game.StartLevel(level, levelInfo.Path);
        }
    }

    private void OpenLevelCreationMenu()
    {
        LevelCreationDialog creationDialog = new(CreateLevel);
        UserInterface.AddChild(this, creationDialog);
        creationDialog.Open();
        
        void CreateLevel(string name, int? seed)
        {
            seed ??= Random.Shared.Next();
            Level level = FilesystemLevelManager.CreateLevel(seed.Value, name, out string path);
            Game.StartLevel(level, path);
        }
    }

    private void OpenLevelRemovalDialog()
    {
        if (_noLevels) return;
        YesNoDialog levelRemovalDialog = new(RemoveSelectedLevel, "Are you sure you want to remove this level?");
        UserInterface.AddChild(this, levelRemovalDialog);
        levelRemovalDialog.Open();
        
        void RemoveSelectedLevel(bool doIt)
        {
            if (!doIt) return;
            FilesystemLevelManager.RemoveLevel(((LevelButton) LineView.GetCurrent()).Path);
            SetLevelButtons();
        }
    }
}