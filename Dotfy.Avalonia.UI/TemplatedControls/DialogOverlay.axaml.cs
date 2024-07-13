using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Dotfy.Avalonia.UI.Common;
using Dotfy.Avalonia.UI.Controls;
using Dotfy.Avalonia.UI.Events;
using Dotfy.Avalonia.UI.Helpers;

namespace Dotfy.Avalonia.UI.TemplatedControls;

[TemplatePart(PART_CloseButton, typeof(Button))]
[TemplatePart(PART_TitleArea, typeof(Panel))]
[PseudoClasses(PC_Modal, PC_FullScreen)]
public class DialogOverlay : OverlayFeedbackElement
{
    public const string PART_CloseButton = "PART_CloseButton";
    public const string PART_TitleArea = "PART_TitleArea";
    public const string PC_Modal = ":modal";
    public const string PC_FullScreen = ":full-screen";

    internal HorizontalPosition HorizontalAnchor { get; set; } = HorizontalPosition.Center;
    internal VerticalPosition VerticalAnchor { get; set; } = VerticalPosition.Center;
    internal HorizontalPosition ActualHorizontalAnchor { get; set; }
    internal VerticalPosition ActualVerticalAnchor { get; set; }
    internal double? HorizontalOffset { get; set; }
    internal double? VerticalOffset { get; set; }
    internal double? HorizontalOffsetRatio { get; set; }
    internal double? VerticalOffsetRatio { get; set; }
    internal bool CanLightDismiss { get; set; }
    internal bool IsCloseButtonVisible { get; set; }

    private bool _isFullScreen;

    public static readonly DirectProperty<DialogOverlay, bool> IsFullScreenProperty = AvaloniaProperty.RegisterDirect<DialogOverlay, bool>(
        nameof(IsFullScreen), o => o.IsFullScreen, (o, v) => o.IsFullScreen = v);

    public bool IsFullScreen
    {
        get => _isFullScreen;
        set => SetAndRaise(IsFullScreenProperty, ref _isFullScreen, value);
    }

    protected internal Button? _closeButton;
    private Panel? _titleArea;

    #region Layer Management

    public static readonly RoutedEvent<DialogLayerChangeEventArgs> LayerChangedEvent =
        RoutedEvent.Register<DialogOverlay, DialogLayerChangeEventArgs>(
            nameof(LayerChanged), RoutingStrategies.Bubble);

    public event EventHandler<DialogLayerChangeEventArgs> LayerChanged
    {
        add => AddHandler(LayerChangedEvent, value);
        remove => RemoveHandler(LayerChangedEvent, value);
    }

    public void UpdateLayer(object? o)
    {
        if (o is DialogLayerChangeType t)
        {
            RaiseEvent(new DialogLayerChangeEventArgs(LayerChangedEvent, t));
        }
    }

    #endregion

    #region DragMove AttachedPropert

    public static readonly AttachedProperty<bool> CanDragMoveProperty =
        AvaloniaProperty.RegisterAttached<DialogOverlay, InputElement, bool>("CanDragMove");

    public static void SetCanDragMove(InputElement obj, bool value) => obj.SetValue(CanDragMoveProperty, value);
    public static bool GetCanDragMove(InputElement obj) => obj.GetValue(CanDragMoveProperty);

    private static void OnCanDragMoveChanged(InputElement arg1, AvaloniaPropertyChangedEventArgs<bool> arg2)
    {
        if (arg2.NewValue.Value)
        {
            arg1.AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Bubble);
            arg1.AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Bubble);
            arg1.AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Bubble);
        }
        else
        {
            arg1.RemoveHandler(PointerPressedEvent, OnPointerPressed);
            arg1.RemoveHandler(PointerMovedEvent, OnPointerMoved);
            arg1.RemoveHandler(PointerReleasedEvent, OnPointerReleased);
        }

        void OnPointerPressed(InputElement sender, PointerPressedEventArgs e)
        {
            if (sender.FindLogicalAncestorOfType<DialogOverlay>() is { } dialog)
            {
                e.Source = dialog;
            }
        }

        void OnPointerMoved(InputElement sender, PointerEventArgs e)
        {
            if (sender.FindLogicalAncestorOfType<DialogOverlay>() is { } dialog)
            {
                e.Source = dialog;
            }
        }

        void OnPointerReleased(InputElement sender, PointerReleasedEventArgs e)
        {
            if (sender.FindLogicalAncestorOfType<DialogOverlay>() is { } dialog)
            {
                e.Source = dialog;
            }
        }
    }

    #endregion

    #region Close AttachedProperty

    public static readonly AttachedProperty<bool> CanCloseProperty =
        AvaloniaProperty.RegisterAttached<DialogOverlay, InputElement, bool>("CanClose");

    public static void SetCanClose(InputElement obj, bool value) => obj.SetValue(CanCloseProperty, value);
    public static bool GetCanClose(InputElement obj) => obj.GetValue(CanCloseProperty);
    private static void OnCanCloseChanged(InputElement arg1, AvaloniaPropertyChangedEventArgs<bool> arg2)
    {
        if (arg2.NewValue.Value)
        {
            arg1.AddHandler(PointerPressedEvent, (InputElement sender, PointerPressedEventArgs e) =>
            {
                if (sender.FindLogicalAncestorOfType<DialogOverlay>() is { } dialog)
                {
                    dialog.Close();
                }
            }, 
        RoutingStrategies.Bubble);
        }

    }
    #endregion

    static DialogOverlay()
    {
        CanDragMoveProperty.Changed.AddClassHandler<InputElement, bool>(OnCanDragMoveChanged);
        CanCloseProperty.Changed.AddClassHandler<InputElement, bool>(OnCanCloseChanged);
        IsFullScreenProperty.AffectsPseudoClass<DialogOverlay>(PC_FullScreen);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _titleArea = e.NameScope.Find<Panel>(PART_TitleArea);
        if (GetCanDragMove(this))
        {
            _titleArea?.RemoveHandler(PointerMovedEvent, OnTitlePointerMove);
            _titleArea?.RemoveHandler(PointerPressedEvent, OnTitlePointerPressed);
            _titleArea?.RemoveHandler(PointerReleasedEvent, OnTitlePointerRelease);

            _titleArea?.AddHandler(PointerMovedEvent, OnTitlePointerMove, RoutingStrategies.Bubble);
            _titleArea?.AddHandler(PointerPressedEvent, OnTitlePointerPressed, RoutingStrategies.Bubble);
            _titleArea?.AddHandler(PointerReleasedEvent, OnTitlePointerRelease, RoutingStrategies.Bubble);
        }
        else
        {
            if (_titleArea is not null) _titleArea.IsHitTestVisible = false;
        }

        Button.ClickEvent.RemoveHandler(OnCloseButtonClick, _closeButton);
        _closeButton = e.NameScope.Find<Button>(PART_CloseButton);
        Button.ClickEvent.AddHandler(OnCloseButtonClick, _closeButton);

        if (_closeButton is not null)
        {
            _closeButton.IsVisible = IsCloseButtonVisible;
        }
    }

    private void OnTitlePointerPressed(InputElement sender, PointerPressedEventArgs e)
    {
        e.Source = this;
    }

    private void OnTitlePointerMove(InputElement sender, PointerEventArgs e)
    {
        e.Source = this;
    }

    private void OnTitlePointerRelease(InputElement sender, PointerReleasedEventArgs e)
    {
        e.Source = this;
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs args) => Close();

    internal void SetAsModal(bool modal)
    {
        PseudoClasses.Set(PC_Modal, modal);
    }

    public override void Close()
    {
        if (DataContext is IDialogContext context)
        {
            context.Close();
        }
        else
        {
            OnElementClosing(this, null);
        }
    }
}