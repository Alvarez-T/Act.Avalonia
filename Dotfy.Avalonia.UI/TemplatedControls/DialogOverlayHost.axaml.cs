using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Utilities;
using Dotfy.Avalonia.UI.Common;
using Dotfy.Avalonia.UI.Controls;
using Dotfy.Avalonia.UI.Events;

namespace Dotfy.Avalonia.UI.TemplatedControls;

public class DialogOverlayHost : Canvas
{
    private static readonly Animation _maskAppearAnimation;
    private static readonly Animation _maskDisappearAnimation;

    private readonly List<DialogPair> _layers = new List<DialogPair>(10);
    private Point _lastPoint;

    private int _modalCount = 0;

    public static readonly DirectProperty<DialogOverlayHost, bool> HasModalProperty = AvaloniaProperty.RegisterDirect<DialogOverlayHost, bool>(
        nameof(HasModal), o => o.HasModal);
    private bool _hasModal;
    public bool HasModal
    {
        get => _hasModal;
        private set => SetAndRaise(HasModalProperty, ref _hasModal, value);
    }

    public bool IsAnimationDisabled { get; set; }

    public Thickness SnapThickness { get; set; } = new Thickness(0);

    static DialogOverlayHost()
    {
        ClipToBoundsProperty.OverrideDefaultValue<DialogOverlayHost>(true);
        _maskAppearAnimation = CreateOpacityAnimation(true);
        _maskDisappearAnimation = CreateOpacityAnimation(false);
    }

    private static Animation CreateOpacityAnimation(bool appear)
    {
        var animation = new Animation();
        animation.FillMode = FillMode.Forward;
        var keyFrame1 = new KeyFrame { Cue = new Cue(0.0) };
        keyFrame1.Setters.Add(new Setter() { Property = OpacityProperty, Value = appear ? 0.0 : 1.0 });
        var keyFrame2 = new KeyFrame { Cue = new Cue(1.0) };
        keyFrame2.Setters.Add(new Setter() { Property = OpacityProperty, Value = appear ? 1.0 : 0.0 });
        animation.Children.Add(keyFrame1);
        animation.Children.Add(keyFrame2);
        animation.Duration = TimeSpan.FromSeconds(0.2);
        return animation;
    }

    public string? HostId { get; set; }

    public DataTemplates DialogDataTemplates { get; set; } = new DataTemplates();

    public static readonly StyledProperty<IBrush?> OverlayMaskBrushProperty =
        AvaloniaProperty.Register<DialogOverlayHost, IBrush?>(
            nameof(OverlayMaskBrush));

    public IBrush? OverlayMaskBrush
    {
        get => GetValue(OverlayMaskBrushProperty);
        set => SetValue(OverlayMaskBrushProperty, value);
    }

    private PureRectangle CreateOverlayMask(bool modal, bool canCloseOnClick)
    {
        PureRectangle rec = new()
        {
            Width = this.Bounds.Width,
            Height = this.Bounds.Height,
            IsVisible = true,
        };
        if (modal)
        {
            rec[!PureRectangle.BackgroundProperty] = this[!OverlayMaskBrushProperty];
        }
        else if (canCloseOnClick)
        {
            rec.SetCurrentValue(PureRectangle.BackgroundProperty, Brushes.Transparent);
        }
        if (canCloseOnClick)
        {
            rec.AddHandler(PointerReleasedEvent, ClickMaskToCloseDialog);
        }
        return rec;
    }

    private void ClickMaskToCloseDialog(object sender, PointerReleasedEventArgs e)
    {
        if (sender is PureRectangle border)
        {
            var layer = _layers.FirstOrDefault(a => a.Mask == border);
            if (layer is not null)
            {
                layer.Element.Close();
                border.RemoveHandler(PointerReleasedEvent, ClickMaskToCloseDialog);
            }
        }
    }

    protected sealed override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        OverlayDialogManager.RegisterHost(this, HostId);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        while (_layers.Count > 0)
        {
            _layers[0].Element.Close();
        }
        OverlayDialogManager.UnregisterHost(HostId);
        base.OnDetachedFromVisualTree(e);
    }


    protected sealed override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        for (int i = 0; i < _layers.Count; i++)
        {
            if (_layers[i].Mask is { } rect)
            {
                rect.Width = this.Bounds.Width;
                rect.Height = this.Bounds.Height;
            }
            if (_layers[i].Element is DialogOverlay d)
            {
                ResetDialogPosition(d, e.NewSize);
            }
            else if (_layers[i].Element is DrawerControl drawer)
            {
                ResetDrawerPosition(drawer, e.NewSize);
            }
        }
    }

    private void ResetZIndices()
    {
        int index = 0;
        for (int i = 0; i < _layers.Count; i++)
        {
            if (_layers[i].Mask is { } mask)
            {
                mask.ZIndex = index;
                index++;
            }
            if (_layers[i].Element is { } dialog)
            {
                dialog.ZIndex = index;
                index++;
            }
        }
    }

    internal IDataTemplate? GetDataTemplate(object? o)
    {
        if (o is null) return null;
        IDataTemplate? result = null;
        var templates = this.DialogDataTemplates;
        result = templates.FirstOrDefault(a => a.Match(o));
        if (result != null) return result;
        var keys = this.Resources.Keys;
        foreach (var key in keys)
        {
            if (Resources.TryGetValue(key, out var value) && value is IDataTemplate t)
            {
                result = t;
                break;
            }
        }
        return result;
    }

    internal T? Recall<T>()
    {
        var element = _layers.LastOrDefault(a => a.Element.Content?.GetType() == typeof(T));
        return element?.Element.Content is T t ? t : default;
    }

    private static void ResetDialogPosition(DialogOverlay control, Size newSize)
    {
        if (control.IsFullScreen)
        {
            control.Width = newSize.Width;
            control.Height = newSize.Height;
            SetLeft(control, 0);
            SetTop(control, 0);
            return;
        }
        control.MaxWidth = newSize.Width;
        control.MaxHeight = newSize.Height;
        var width = newSize.Width - control.Bounds.Width;
        var height = newSize.Height - control.Bounds.Height;
        var newLeft = width * control.HorizontalOffsetRatio ?? 0;
        var newTop = height * control.VerticalOffsetRatio ?? 0;
        if (control.ActualHorizontalAnchor == HorizontalPosition.Left)
        {
            newLeft = 0;
        }
        if (control.ActualHorizontalAnchor == HorizontalPosition.Right)
        {
            newLeft = newSize.Width - control.Bounds.Width;
        }
        if (control.ActualVerticalAnchor == VerticalPosition.Top)
        {
            newTop = 0;
        }
        if (control.ActualVerticalAnchor == VerticalPosition.Bottom)
        {
            newTop = newSize.Height - control.Bounds.Height;
        }
        SetLeft(control, Math.Max(0.0, newLeft));
        SetTop(control, Math.Max(0.0, newTop));
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (e.Source is DialogOverlay item)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var p = e.GetPosition(this);
                var left = p.X - _lastPoint.X;
                var top = p.Y - _lastPoint.Y;
                left = MathUtilities.Clamp(left, 0, Bounds.Width - item.Bounds.Width);
                top = MathUtilities.Clamp(top, 0, Bounds.Height - item.Bounds.Height);
                SetLeft(item, left);
                SetTop(item, top);
            }
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.Source is DialogOverlay item)
        {
            _lastPoint = e.GetPosition(item);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (e.Source is DialogOverlay item)
        {
            AnchorAndUpdatePositionInfo(item);
        }
    }

    internal void AddDialog(DialogOverlay control)
    {
        PureRectangle? mask = null;
        if (control.CanLightDismiss)
        {
            mask = CreateOverlayMask(false, control.CanLightDismiss);
        }
        if (mask is not null)
        {
            Children.Add(mask);
        }
        this.Children.Add(control);
        _layers.Add(new DialogPair(mask, control, false));
        if (control.IsFullScreen)
        {
            control.Width = Bounds.Width;
            control.Height = Bounds.Height;
        }
        control.MaxWidth = Bounds.Width;
        control.MaxHeight = Bounds.Height;
        control.Measure(this.Bounds.Size);
        control.Arrange(new Rect(control.DesiredSize));
        SetToPosition(control);
        control.AddHandler(OverlayFeedbackElement.ClosedEvent, OnDialogControlClosing);
        control.AddHandler(DialogOverlay.LayerChangedEvent, OnDialogLayerChanged);
        ResetZIndices();
    }

    private async void OnDialogControlClosing(object sender, object? e)
    {
        if (sender is DialogOverlay control)
        {
            var layer = _layers.FirstOrDefault(a => a.Element == control);
            if (layer is null) return;
            _layers.Remove(layer);

            control.RemoveHandler(OverlayFeedbackElement.ClosedEvent, OnDialogControlClosing);
            control.RemoveHandler(DialogOverlay.LayerChangedEvent, OnDialogLayerChanged);

            Children.Remove(control);

            if (layer.Mask is not null)
            {
                Children.Remove(layer.Mask);

                if (layer.Modal)
                {
                    _modalCount--;
                    HasModal = _modalCount > 0;
                    if (!IsAnimationDisabled)
                    {
                        await _maskDisappearAnimation.RunAsync(layer.Mask);
                    }
                }
            }

            ResetZIndices();
        }
    }

    /// <summary>
    ///  Add a dialog as a modal dialog to the host
    /// </summary>
    /// <param name="control"></param>
    internal void AddModalDialog(DialogOverlay control)
    {
        var mask = CreateOverlayMask(true, control.CanLightDismiss);
        _layers.Add(new DialogPair(mask, control));
        control.SetAsModal(true);
        ResetZIndices();
        this.Children.Add(mask);
        this.Children.Add(control);
        if (control.IsFullScreen)
        {
            control.Width = Bounds.Width;
            control.Height = Bounds.Height;
        }
        control.MaxWidth = Bounds.Width;
        control.MaxHeight = Bounds.Height;
        control.Measure(this.Bounds.Size);
        control.Arrange(new Rect(control.DesiredSize));
        SetToPosition(control);
        control.AddHandler(OverlayFeedbackElement.ClosedEvent, OnDialogControlClosing);
        control.AddHandler(DialogOverlay.LayerChangedEvent, OnDialogLayerChanged);
        if (!IsAnimationDisabled)
        {
            _maskAppearAnimation.RunAsync(mask);
        }
        _modalCount++;
        HasModal = _modalCount > 0;
        control.IsClosed = false;
    }

    // Handle dialog layer change event
    private void OnDialogLayerChanged(object sender, DialogLayerChangeEventArgs e)
    {
        if (sender is not DialogOverlay control)
            return;
        var layer = _layers.FirstOrDefault(a => a.Element == control);
        if (layer is null) return;
        int index = _layers.IndexOf(layer);
        _layers.Remove(layer);
        int newIndex = index;
        switch (e.ChangeType)
        {
            case DialogLayerChangeType.BringForward:
                newIndex = MathUtilities.Clamp(index + 1, 0, _layers.Count);
                break;
            case DialogLayerChangeType.SendBackward:
                newIndex = MathUtilities.Clamp(index - 1, 0, _layers.Count);
                break;
            case DialogLayerChangeType.BringToFront:
                newIndex = _layers.Count;
                break;
            case DialogLayerChangeType.SendToBack:
                newIndex = 0;
                break;
        }

        _layers.Insert(newIndex, layer);
        ResetZIndices();
    }

    private void SetToPosition(DialogOverlay? control)
    {
        if (control is null) return;
        double left = GetLeftPosition(control);
        double top = GetTopPosition(control);
        SetLeft(control, left);
        SetTop(control, top);
        AnchorAndUpdatePositionInfo(control);
    }

    private void AnchorAndUpdatePositionInfo(DialogOverlay control)
    {
        control.ActualHorizontalAnchor = HorizontalPosition.Center;
        control.ActualVerticalAnchor = VerticalPosition.Center;
        double left = GetLeft(control);
        double top = GetTop(control);
        double right = Bounds.Width - left - control.Bounds.Width;
        double bottom = Bounds.Height - top - control.Bounds.Height;
        if (top < SnapThickness.Top)
        {
            SetTop(control, 0);
            control.ActualVerticalAnchor = VerticalPosition.Top;
            control.VerticalOffsetRatio = 0;
        }
        if (bottom < SnapThickness.Bottom)
        {
            SetTop(control, Bounds.Height - control.Bounds.Height);
            control.ActualVerticalAnchor = VerticalPosition.Bottom;
            control.VerticalOffsetRatio = 1;
        }
        if (left < SnapThickness.Left)
        {
            SetLeft(control, 0);
            control.ActualHorizontalAnchor = HorizontalPosition.Left;
            control.HorizontalOffsetRatio = 0;
        }
        if (right < SnapThickness.Right)
        {
            SetLeft(control, Bounds.Width - control.Bounds.Width);
            control.ActualHorizontalAnchor = HorizontalPosition.Right;
            control.HorizontalOffsetRatio = 1;
        }
        left = GetLeft(control);
        top = GetTop(control);
        right = Bounds.Width - left - control.Bounds.Width;
        bottom = Bounds.Height - top - control.Bounds.Height;

        control.HorizontalOffsetRatio = (left + right) == 0 ? 0 : left / (left + right);
        control.VerticalOffsetRatio = (top + bottom) == 0 ? 0 : top / (top + bottom);
    }

    private double GetLeftPosition(DialogOverlay control)
    {
        double left = 0;
        double offset = Math.Max(0, control.HorizontalOffset ?? 0);
        left = this.Bounds.Width - control.Bounds.Width;
        if (control.HorizontalAnchor == HorizontalPosition.Center)
        {
            left *= 0.5;
            (double min, double max) = MathUtilities.GetMinMax(0, Bounds.Width * 0.5);
            left = MathUtilities.Clamp(left, min, max);
        }
        else if (control.HorizontalAnchor == HorizontalPosition.Left)
        {
            (double min, double max) = MathUtilities.GetMinMax(0, offset);
            left = MathUtilities.Clamp(left, min, max);
        }
        else if (control.HorizontalAnchor == HorizontalPosition.Right)
        {
            double leftOffset = Bounds.Width - control.Bounds.Width - offset;
            leftOffset = Math.Max(0, leftOffset);
            if (control.HorizontalOffset.HasValue)
            {
                left = MathUtilities.Clamp(left, 0, leftOffset);
            }
        }
        return left;
    }

    private double GetTopPosition(DialogOverlay control)
    {
        double top = 0;
        double offset = Math.Max(0, control.VerticalOffset ?? 0);
        top = this.Bounds.Height - control.Bounds.Height;
        if (control.VerticalAnchor == VerticalPosition.Center)
        {
            top *= 0.5;
            (double min, double max) = MathUtilities.GetMinMax(0, Bounds.Height * 0.5);
            top = MathUtilities.Clamp(top, min, max);
        }
        else if (control.VerticalAnchor == VerticalPosition.Top)
        {
            top = MathUtilities.Clamp(top, 0, offset);
        }
        else if (control.VerticalAnchor == VerticalPosition.Bottom)
        {
            var topOffset = Math.Max(0, Bounds.Height - control.Bounds.Height - offset);
            top = MathUtilities.Clamp(top, 0, topOffset);
        }
        return top;
    }

    internal async void AddDrawer(DrawerControl control)
    {
        PureRectangle? mask = null;
        if (control.CanLightDismiss)
        {
            mask = CreateOverlayMask(false, true);
        }
        _layers.Add(new DialogPair(mask, control));
        ResetZIndices();
        if (mask is not null) this.Children.Add(mask);
        this.Children.Add(control);
        control.Measure(this.Bounds.Size);
        control.Arrange(new Rect(control.DesiredSize));
        SetDrawerPosition(control);
        control.AddHandler(OverlayFeedbackElement.ClosedEvent, OnDrawerControlClosing);
        var animation = CreateAnimation(control.Bounds.Size, control.Position, true);
        if (IsAnimationDisabled)
        {
            ResetDrawerPosition(control, this.Bounds.Size);
        }
        else
        {
            if (mask is null)
            {
                await animation.RunAsync(control);
            }
            else
            {
                await Task.WhenAll(animation.RunAsync(control), _maskAppearAnimation.RunAsync(mask));
            }
        }
    }

    internal async void AddModalDrawer(DrawerControl control)
    {
        PureRectangle? mask = CreateOverlayMask(true, control.CanLightDismiss);
        _layers.Add(new DialogPair(mask, control));
        this.Children.Add(mask);
        this.Children.Add(control);
        ResetZIndices();
        control.Measure(this.Bounds.Size);
        control.Arrange(new Rect(control.DesiredSize));
        SetDrawerPosition(control);
        _modalCount++;
        HasModal = _modalCount > 0;
        control.AddHandler(OverlayFeedbackElement.ClosedEvent, OnDrawerControlClosing);
        var animation = CreateAnimation(control.Bounds.Size, control.Position);
        if (IsAnimationDisabled)
        {
            ResetDrawerPosition(control, this.Bounds.Size);
        }
        else
        {
            await Task.WhenAll(animation.RunAsync(control), _maskAppearAnimation.RunAsync(mask));
        }
    }

    private void SetDrawerPosition(DrawerControl control)
    {
        if (control.Position is Position.Left or Position.Right)
        {
            control.Height = this.Bounds.Height;
        }
        if (control.Position is Position.Top or Position.Bottom)
        {
            control.Width = this.Bounds.Width;
        }
    }

    private static void ResetDrawerPosition(DrawerControl control, Size newSize)
    {
        if (control.Position == Position.Right)
        {
            control.Height = newSize.Height;
            SetLeft(control, newSize.Width - control.Bounds.Width);
        }
        else if (control.Position == Position.Left)
        {
            control.Height = newSize.Height;
            SetLeft(control, 0);
        }
        else if (control.Position == Position.Top)
        {
            control.Width = newSize.Width;
            SetTop(control, 0);
        }
        else
        {
            control.Width = newSize.Width;
            SetTop(control, newSize.Height - control.Bounds.Height);
        }
    }

    private Animation CreateAnimation(Size elementBounds, Position position, bool appear = true)
    {
        // left or top.
        double source = 0;
        double target = 0;
        if (position == Position.Left)
        {
            source = appear ? -elementBounds.Width : 0;
            target = appear ? 0 : -elementBounds.Width;
        }

        if (position == Position.Right)
        {
            source = appear ? Bounds.Width : Bounds.Width - elementBounds.Width;
            target = appear ? Bounds.Width - elementBounds.Width : Bounds.Width;
        }

        if (position == Position.Top)
        {
            source = appear ? -elementBounds.Height : 0;
            target = appear ? 0 : -elementBounds.Height;
        }

        if (position == Position.Bottom)
        {
            source = appear ? Bounds.Height : Bounds.Height - elementBounds.Height;
            target = appear ? Bounds.Height - elementBounds.Height : Bounds.Height;
        }

        var targetProperty = position == Position.Left || position == Position.Right ? Canvas.LeftProperty : Canvas.TopProperty;
        var animation = new Animation();
        animation.Easing = new CubicEaseOut();
        animation.FillMode = FillMode.Forward;
        var keyFrame1 = new KeyFrame() { Cue = new Cue(0.0) };
        keyFrame1.Setters.Add(new Setter()
        { Property = targetProperty, Value = source });
        var keyFrame2 = new KeyFrame() { Cue = new Cue(1.0) };
        keyFrame2.Setters.Add(new Setter()
        { Property = targetProperty, Value = target });
        animation.Children.Add(keyFrame1);
        animation.Children.Add(keyFrame2);
        animation.Duration = TimeSpan.FromSeconds(0.3);
        return animation;
    }

    private async void OnDrawerControlClosing(object sender, ResultEventArgs e)
    {
        if (sender is DrawerControl control)
        {
            var layer = _layers.FirstOrDefault(a => a.Element == control);
            if (layer is null) return;
            _layers.Remove(layer);
            control.RemoveHandler(OverlayFeedbackElement.ClosedEvent, OnDialogControlClosing);
            control.RemoveHandler(DialogOverlay.LayerChangedEvent, OnDialogLayerChanged);
            if (layer.Mask is not null)
            {
                _modalCount--;
                HasModal = _modalCount > 0;
                layer.Mask.RemoveHandler(PointerPressedEvent, ClickMaskToCloseDialog);
                if (!IsAnimationDisabled)
                {
                    var disappearAnimation = CreateAnimation(control.Bounds.Size, control.Position, false);
                    await Task.WhenAll(disappearAnimation.RunAsync(control), _maskDisappearAnimation.RunAsync(layer.Mask));
                }
                Children.Remove(layer.Mask);
            }
            else
            {
                if (!IsAnimationDisabled)
                {
                    var disappearAnimation = CreateAnimation(control.Bounds.Size, control.Position, false);
                    await disappearAnimation.RunAsync(control);
                }
            }
            Children.Remove(control);
            ResetZIndices();
        }
    }

    private class DialogPair
    {
        internal PureRectangle? Mask { get; }
        internal OverlayFeedbackElement Element { get; }
        internal bool Modal { get; }

        public DialogPair(PureRectangle? mask, OverlayFeedbackElement element, bool modal = true)
        {
            Mask = mask;
            Element = element;
            Modal = modal;
        }
    }
}
