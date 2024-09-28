using Avalonia.Controls;

namespace Lyt.Avalonia.Controls.Glyphs; 

public interface ICanSelect
{
    bool IsSelected { get; set; }
}

public partial class SelectionGroup : UserControl
{
    public SelectionGroup() => this.InitializeComponent();

    public readonly List<ICanSelect> Members = [];

    public void Clear()
    {
        foreach (var member in this.Members)
        {
                member.IsSelected = false;
        }
    }

    public void Register(ICanSelect selectable)
    {
        if (this.Members.Contains(selectable))
        {
            return; 
        }

        this.Members.Add(selectable);
    } 

    public void Select(ICanSelect selectable)
    {
        if(selectable.IsSelected)
        {
            return; 
        }

        foreach (var member in this.Members)
        {
            if ( member != selectable )
            { 
                member.IsSelected = false;
            }
        } 

        selectable.IsSelected = true;
    } 
}
