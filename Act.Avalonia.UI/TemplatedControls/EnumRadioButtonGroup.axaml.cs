using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Act.Avalonia.UI.TemplatedControls;

public class EnumRadioButtonGroup : ItemsControl
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<EnumRadioButtonGroup, Orientation>(nameof(Orientation));

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<StackPanel, double>(nameof(Spacing));

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public static readonly StyledProperty<Enum> EnumValueProperty =
        AvaloniaProperty.Register<EnumRadioButtonGroup, Enum>(nameof(EnumValue));

    public Enum EnumValue
    {
        get => GetValue(EnumValueProperty);
        set => SetValue(EnumValueProperty, value);
    }

    public static readonly AttachedProperty<Enum> EnumMemberValueProperty =
        AvaloniaProperty.RegisterAttached<EnumRadioButtonGroup, RadioButton, Enum>("EnumMemberValue");

    public static void SetEnumMemberValue(AvaloniaObject element, Enum value)
    {
        element.SetValue(EnumMemberValueProperty, value);
    }

    public static Enum GetEnumMemberValue(AvaloniaObject element)
    {
        return element.GetValue(EnumMemberValueProperty);
    }

    static EnumRadioButtonGroup()
    {
        AffectsMeasure<EnumRadioButtonGroup>(SpacingProperty);
        AffectsMeasure<EnumRadioButtonGroup>(OrientationProperty);

        EnumValueProperty.Changed.AddClassHandler<EnumRadioButtonGroup>((x, e) => x.OnEnumValueChanged(e));
        EnumMemberValueProperty.Changed.AddClassHandler<EnumRadioButtonGroup>((x, e) => x.OnEnumMemberValueChanged(e));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        AddHandler(ToggleButton.IsCheckedChangedEvent, OnRadioButtonCheckedChanged, RoutingStrategies.Bubble);
    }

    private void OnRadioButtonCheckedChanged(object? o, RoutedEventArgs e)
    {
        if (e.Source is RadioButton { IsChecked: true } radioButton)
        {
            EnumValue = GetEnumMemberValue(radioButton);
        }
    }

    private void OnEnumValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newValue = (Enum)e.NewValue!;
        foreach (var radioButton in this.GetLogicalChildren().OfType<RadioButton>())
        {
            var enumMemberValue = GetEnumMemberValue(radioButton);
            radioButton.IsChecked = newValue.Equals(enumMemberValue);
        }
    }

    private void OnEnumMemberValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is RadioButton { IsChecked: true })
        {
            EnumValue = (Enum)e.NewValue!;
        }
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not RadioButton;
    }
}