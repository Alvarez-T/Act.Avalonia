using Act.Avalonia.UI.Common;
using Act.Avalonia.UI.Helpers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;

namespace Act.Avalonia.UI.TemplatedControls
{
    [PseudoClasses(":top", ":bottom")]
    public class IconRadioButton : RadioButton
    {
        public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<IconRadioButton, object?>(
            nameof(Icon));

        public object? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty = AvaloniaProperty.Register<IconRadioButton, IDataTemplate?>(
            nameof(IconTemplate));

        public IDataTemplate? IconTemplate
        {
            get => GetValue(IconTemplateProperty);
            set => SetValue(IconTemplateProperty, value);
        }

        public static readonly StyledProperty<bool> IsLoadingProperty = AvaloniaProperty.Register<IconRadioButton, bool>(
            nameof(IsLoading));

        public bool IsLoading
        {
            get => GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly StyledProperty<VerticalPosition> IconPlacementProperty = AvaloniaProperty.Register<IconRadioButton, VerticalPosition>(
            nameof(IconPlacement), defaultValue: VerticalPosition.Top);

        public VerticalPosition IconPlacement
        {
            get => GetValue(IconPlacementProperty);
            set => SetValue(IconPlacementProperty, value);
        }

        static IconRadioButton()
        {
            IconPlacementProperty.Changed.AddClassHandler<IconRadioButton, VerticalPosition>((o, e) =>
            {
                o.SetPlacement(e.NewValue.Value, o.Icon);
            });
            IconProperty.Changed.AddClassHandler<IconRadioButton, object?>((o, e) =>
            {
                o.SetPlacement(o.IconPlacement, e.NewValue.Value);
            });
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            SetPlacement(IconPlacement, Icon);
        }

        private void SetPlacement(VerticalPosition placement, object? icon)
        {
            this.ResetAllPseudoClasses();

            if (icon is null)
                throw new NullReferenceException("Icon cannot be null");

            PseudoClasses.Set(GetPseudoClassByPosition(placement), true);

        }

        private string GetPseudoClassByPosition(VerticalPosition placement) => placement switch
        {
            VerticalPosition.Center => ":top",
            VerticalPosition.Top => ":top",
            VerticalPosition.Bottom => ":bottom",
            _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, null)
        };

        protected override void OnIsCheckedChanged(RoutedEventArgs e)
        {
            base.OnIsCheckedChanged(e);
        }
    }
}
