using Avalonia.Interactivity;
using Dotfy.Avalonia.UI.Common;

namespace Dotfy.Avalonia.UI.Events;

public class DialogLayerChangeEventArgs : RoutedEventArgs
{
    public DialogLayerChangeType ChangeType { get; }

    public DialogLayerChangeEventArgs(DialogLayerChangeType type)
    {
        ChangeType = type;
    }
    public DialogLayerChangeEventArgs(RoutedEvent routedEvent, DialogLayerChangeType type) : base(routedEvent)
    {
        ChangeType = type;
    }
}

