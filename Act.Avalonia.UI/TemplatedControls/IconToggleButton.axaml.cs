using Act.Avalonia.UI.Common;
using Act.Avalonia.UI.Helpers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Act.Avalonia.UI.TemplatedControls;

[PseudoClasses(":right", ":left", ":top", ":bottom", ":empty")]
public class IconToggleButton : ToggleButton
{
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<IconToggleButton, object?>(
   nameof(Icon));

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty = AvaloniaProperty.Register<IconToggleButton, IDataTemplate?>(
        nameof(IconTemplate));

    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public static readonly StyledProperty<bool> IsLoadingProperty = AvaloniaProperty.Register<IconToggleButton, bool>(
        nameof(IsLoading));

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public static readonly StyledProperty<Position> IconPlacementProperty = AvaloniaProperty.Register<IconToggleButton, Position>(
        nameof(IconPlacement), defaultValue: Position.Left);

    public Position IconPlacement
    {
        get => GetValue(IconPlacementProperty);
        set => SetValue(IconPlacementProperty, value);
    }

    static IconToggleButton()
    {
        IconPlacementProperty.Changed.AddClassHandler<IconToggleButton, Position>((o, e) =>
        {
            o.SetPlacement(e.NewValue.Value, o.Icon);
        });
        IconProperty.Changed.AddClassHandler<IconToggleButton, object?>((o, e) =>
        {
            o.SetPlacement(o.IconPlacement, e.NewValue.Value);
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var grid = e.NameScope.Find<Grid>("PART_Grid");
        if (grid is null)
            throw new NullReferenceException("The part \"PART_Grid\" was not founded in the template");

        SetGridDefinitions(grid);
        SetPlacement(IconPlacement, Icon);
    }

    private void SetPlacement(Position placement, object? icon)
    {
        this.ResetAllPseudoClasses();

        if (icon is null)
            PseudoClasses.Set(":empty", true);

        PseudoClasses.Set(GetPseudoClassByPosition(placement), true);

    }

    private void SetGridDefinitions(Grid grid)
    {
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();

        switch (IconPlacement)
        {
            case Position.Top:
                grid.RowDefinitions.AddRange([new(GridLength.Star), new(GridLength.Auto)]);
                grid.ColumnDefinitions.AddRange([new(GridLength.Star), new(GridLength.Auto)]);
                grid.IsEnabled = false;
                break;
            case Position.Bottom:
                grid.RowDefinitions.AddRange([new(GridLength.Auto), new(GridLength.Star)]);
                grid.ColumnDefinitions.AddRange([new(GridLength.Star), new(GridLength.Auto)]);
                break;
            case Position.Right:
                grid.RowDefinitions.AddRange([new(GridLength.Star), new(GridLength.Auto)]);
                grid.ColumnDefinitions.AddRange([new(GridLength.Star), new(GridLength.Auto)]);
                break;
            case Position.Left:
                grid.RowDefinitions.AddRange([new(GridLength.Star), new(GridLength.Auto)]);
                grid.ColumnDefinitions.AddRange([new(GridLength.Auto), new(GridLength.Star)]);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(IconPlacement), IconPlacement, null);
        }
    }

    private string GetPseudoClassByPosition(Position placement) => placement switch
    {
        Position.Left => ":left",
        Position.Right => ":right",
        Position.Top => ":top",
        Position.Bottom => ":bottom",
        _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, null)
    };
}