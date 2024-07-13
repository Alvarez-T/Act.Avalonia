using Avalonia.Interactivity;

namespace Dotfy.Avalonia.UI.Events;

public class ResultEventArgs : RoutedEventArgs
{
    public object? Result { get; set; }

    public ResultEventArgs(object? result)
    {
        Result = result;
    }

    public ResultEventArgs(RoutedEvent routedEvent, object? result) : base(routedEvent)
    {
        Result = result;
    }
}