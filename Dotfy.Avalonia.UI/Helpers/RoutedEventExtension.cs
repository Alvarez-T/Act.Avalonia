using Avalonia.Interactivity;

namespace Dotfy.Avalonia.UI.Helpers;

public static class RoutedEventExtension
{
    public static void AddHandler<TArgs>(
        this RoutedEvent<TArgs> routedEvent,
        EventHandler<TArgs> handler,
        params Interactive?[] controls)
        where TArgs : RoutedEventArgs
    {
        foreach (Interactive? control in controls)
            control?.AddHandler<TArgs>(routedEvent, handler);
    }

    public static void RemoveHandler<TArgs>(
        this RoutedEvent<TArgs> routedEvent,
        EventHandler<TArgs> handler,
        params Interactive?[] controls)
        where TArgs : RoutedEventArgs
    {
        foreach (var t in controls)
        {
            t?.RemoveHandler(routedEvent, handler);
        }
    }
}