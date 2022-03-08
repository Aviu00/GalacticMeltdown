using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.Events;
using GalacticMeltdown.Frontend;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown.Views;

public class MenuView : View
{
    private Stack<View> _menus = new();

    public override event EventHandler NeedRedraw;
    public override event EventHandler<CellChangeEventArgs> CellsChanged;

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

    private void OpenMenu(View menu, Dictionary<SelectionControl, Action> bindings)
    {
        _menus.Push(menu);
        menu.Resize(Width, Height);
        menu.NeedRedraw += SendRedraw;
        menu.CellsChanged += SendAnim;
        InputProcessor.AddBinding(DataHolder.CurrentBindings.Selection, bindings);
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void CloseMenu()
    {
        View menu = _menus.Pop();
        menu.NeedRedraw -= SendRedraw;
        menu.CellsChanged -= SendAnim;
        InputProcessor.RemoveLastBinding();
        NeedRedraw?.Invoke(this, EventArgs.Empty);
    }

    private void SendRedraw(object view, EventArgs _) => NeedRedraw?.Invoke(view, EventArgs.Empty);

    private void SendAnim(object sender, CellChangeEventArgs e) => CellsChanged?.Invoke(sender, e);
}