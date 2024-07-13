﻿namespace Dotfy.Avalonia.UI.Common;

internal class DialogOverlayOptions
{
    internal static DialogOverlayOptions Default { get; } = new DialogOverlayOptions();
    public bool FullScreen { get; set; }
    public HorizontalPosition HorizontalAnchor { get; set; } = HorizontalPosition.Center;
    public VerticalPosition VerticalAnchor { get; set; } = VerticalPosition.Center;
    /// <summary>
    /// This attribute is only used when HorizontalAnchor is not Center
    /// </summary>
    public double? HorizontalOffset { get; set; } = null;
    /// <summary>
    /// This attribute is only used when VerticalAnchor is not Center
    /// </summary>
    public double? VerticalOffset { get; set; } = null;
    /// <summary>
    /// Only works for DefaultDialogControl
    /// </summary>
    public DialogMode Mode { get; set; } = DialogMode.None;
    /// <summary>
    /// Only works for DefaultDialogControl
    /// </summary>
    public DialogButton Buttons { get; set; } = DialogButton.OKCancel;
    /// <summary>
    /// Only works for DefaultDialogControl
    /// </summary>
    public string? Title { get; set; } = null;
    /// <summary>
    /// Only works for CustomDialogControl
    /// </summary>
    public bool ShowCloseButton { get; set; } = true;
    public bool CanLightDismiss { get; set; }
    public bool CanDragMove { get; set; } = true;
}