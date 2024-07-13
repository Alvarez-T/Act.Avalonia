using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace Dotfy.Avalonia.UI.Helpers;

public static class AvaloniaPropertyExtension
{
    public static void SetValue<TValue, TItem>(
        this AvaloniaProperty<TValue> property,
        TValue value,
        params TItem?[] objects)
        where TItem : AvaloniaObject
    {
        foreach (TItem? obj in objects)
            obj?.SetValue((AvaloniaProperty)property, (object)value, BindingPriority.LocalValue);
    }

    public static void AffectsPseudoClass<TControl>(
        this AvaloniaProperty<bool> property,
        string pseudoClass,
        RoutedEvent<RoutedEventArgs>? routedEvent = null)
        where TControl : Control
    {
        property.Changed.AddClassHandler<TControl, bool>((control, args) => { OnPropertyChanged(control, args, pseudoClass, routedEvent); });
    }

    private static void OnPropertyChanged<TControl, TArgs>(
        TControl control,
        AvaloniaPropertyChangedEventArgs<bool> args,
        string pseudoClass,
        RoutedEvent<TArgs>? routedEvent)
        where TControl : Control
        where TArgs : RoutedEventArgs, new()
    {
        PseudolassesExtensions.Set(control.Classes, pseudoClass, args.NewValue.Value);
        if (routedEvent is not null)
        {
            control.RaiseEvent(new TArgs() { RoutedEvent = routedEvent });
        }
    }
}