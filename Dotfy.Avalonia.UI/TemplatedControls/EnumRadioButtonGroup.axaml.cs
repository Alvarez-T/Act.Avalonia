using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Dotfy.Avalonia.UI.Helpers;
using System;
using Avalonia.Interactivity;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Dotfy.Avalonia.UI.TemplatedControls;

public class EnumRadioButtonGroup : ItemsControl
{
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
        EnumValueProperty.Changed.AddClassHandler<EnumRadioButtonGroup>((x, e) => x.OnEnumValueChanged(e));
        EnumMemberValueProperty.Changed.AddClassHandler<EnumRadioButtonGroup>((x, e) => x.OnEnumMemberValueChanged(e));
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();


    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
       // AddHandler(IconRadioButton.IsCheckedChangedEvent, OnRadioButtonCheckedChanged, RoutingStrategies.Bubble);
        foreach (var child in this.GetLogicalChildren().OfType<RadioButton>())
        {
            child.IsCheckedChanged += OnRadioButtonCheckedChanged;
        }
    }

    private void OnRadioButtonCheckedChanged(object? o, RoutedEventArgs e)
    {
        if (o is RadioButton { IsChecked: true } radioButton)
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

