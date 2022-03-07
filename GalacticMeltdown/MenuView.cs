using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public class MenuView : View
{
    public override event ViewChangedEventHandler NeedRedraw;
    public override event CellsChangedEventHandler CellsChanged;
    private Stack<View> _menus = new();

    public override ViewCellData GetSymbol(int x, int y)
    {
        return _menus.Any() ? _menus.Peek().GetSymbol(x, y) : new ViewCellData(null, null);
    }

    public override void Resize(int width, int height)
    {
        base.Resize(width, height);
        foreach (var menu in _menus)
        {
            menu.Resize(width, height);
        }
    }

    public void OpenMenu(View menu, Dictionary<SelectionControl, Action> bindings)
    {
        _menus.Push(menu);
        menu.Resize(Width, Height);
        menu.NeedRedraw += SendRedraw;
        menu.CellsChanged += SendAnim;
        NeedRedraw?.Invoke(this);
        InputProcessor.AddBinding(DataHolder.CurrentBindings.Selection, bindings);
        Renderer.Redraw();
    }

    public void CloseMenu()
    {
        View menu = _menus.Pop();
        menu.NeedRedraw -= SendRedraw;
        menu.CellsChanged -= SendAnim;
        NeedRedraw?.Invoke(this);
        InputProcessor.RemoveLastBinding();
    }

    public void OpenBasicMenu(params Button[] buttons)
    {
        var buttonListView = new ButtonListView(buttons);
        OpenMenu(buttonListView,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Up, buttonListView.SelectPrev},
                {SelectionControl.Down, buttonListView.SelectNext},
                {SelectionControl.Select, buttonListView.PressCurrent}
            });
    }

    public void OpenLevelMenu()
    {
        var levelMenu = new LevelManagementView();
        OpenMenu(levelMenu,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Up, levelMenu.SelectPrev},
                {SelectionControl.Down, levelMenu.SelectNext},
                {SelectionControl.SwitchButtonGroup, levelMenu.SwitchButtonGroup},
                {SelectionControl.Select, levelMenu.PressCurrent},
                {SelectionControl.Back, CloseMenu}
            });
    }

    private void SendRedraw(View view) => NeedRedraw?.Invoke(this);

    private void SendAnim((View, HashSet<(int, int, ViewCellData)>) animInfo) =>
        CellsChanged?.Invoke((this, animInfo.Item2));
}