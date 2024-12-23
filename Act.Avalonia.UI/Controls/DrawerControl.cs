﻿using Act.Avalonia.UI.Common;
using Act.Avalonia.UI.Events;
using Act.Avalonia.UI.Helpers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Act.Avalonia.UI.Controls;

[TemplatePart(PART_CloseButton, typeof(Button))]
public abstract class DrawerControl : OverlayFeedbackElement
{
    public const string PART_CloseButton = "PART_CloseButton";

    protected internal Button? _closeButton;

    public static readonly StyledProperty<Position> PositionProperty =
        AvaloniaProperty.Register<DrawerControl, Position>(
            nameof(Position), defaultValue: Position.Right);

    public Position Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public static readonly StyledProperty<bool> IsOpenProperty = AvaloniaProperty.Register<DrawerControl, bool>(
        nameof(IsOpen));

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty =
        AvaloniaProperty.Register<DrawerControl, bool>(
            nameof(IsCloseButtonVisible), defaultValue: true);

    public bool IsCloseButtonVisible
    {
        get => GetValue(IsCloseButtonVisibleProperty);
        set => SetValue(IsCloseButtonVisibleProperty, value);
    }

    protected internal bool CanLightDismiss { get; set; }

    static DrawerControl()
    {
        DataContextProperty.Changed.AddClassHandler<DrawerControl, object?>((o, e) => o.OnDataContextChange(e));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Button.ClickEvent.RemoveHandler(OnCloseButtonClick, _closeButton);
        _closeButton = e.NameScope.Find<Button>(PART_CloseButton);
        Button.ClickEvent.AddHandler(OnCloseButtonClick, _closeButton);
    }

    private void OnDataContextChange(AvaloniaPropertyChangedEventArgs<object?> args)
    {
        if (args.OldValue.Value is IDialogContext oldContext)
        {
            oldContext.RequestClose -= OnContextRequestClose;
        }
        if (args.NewValue.Value is IDialogContext newContext)
        {
            newContext.RequestClose += OnContextRequestClose;
        }
    }

    private void OnContextRequestClose(object sender, object? e)
    {
        RaiseEvent(new ResultEventArgs(ClosedEvent, e));
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e) => Close();

    public override void Close()
    {
        if (DataContext is IDialogContext context)
        {
            context.Close();
        }
        else
        {
            RaiseEvent(new ResultEventArgs(ClosedEvent, null));
        }
    }
}