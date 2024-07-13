namespace Dotfy.Avalonia.UI.Common;

public interface IDialogContext
{
    public void Close();
    public event EventHandler<object?>? RequestClose;
}