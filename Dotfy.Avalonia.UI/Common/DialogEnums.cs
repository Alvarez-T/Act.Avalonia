namespace Dotfy.Avalonia.UI.Common;

public enum DialogMode
{
    Info,
    Warning,
    Error,
    Question,
    None,
    Success,
}

public enum DialogButton
{
    None,
    OK,
    OKCancel,
    YesNo,
    YesNoCancel,
}

public enum DialogResult
{
    Cancel,
    No,
    None,
    OK,
    Yes,
}

public enum DialogLayerChangeType
{
    BringForward,
    SendBackward,
    BringToFront,
    SendToBack
}