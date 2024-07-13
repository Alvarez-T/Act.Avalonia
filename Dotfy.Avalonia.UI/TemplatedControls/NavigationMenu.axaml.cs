using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Dotfy.Avalonia.UI.Common;

namespace Dotfy.Avalonia.UI.TemplatedControls
{
    public class NavigationMenu : TemplatedControl
    {
        public static readonly StyledProperty<double> HeaderMinHeightProperty =
            AvaloniaProperty.Register<Control, double>(nameof(HeaderMinHeight));

        public double HeaderMinHeight
        {
            get => GetValue(HeaderMinHeightProperty);
            set => SetValue(HeaderMinHeightProperty, value);
        }

        public static readonly StyledProperty<NavigationMenuDisplayMode> NavigationMenuDisplayModeProperty =
            AvaloniaProperty.Register<NavigationMenu, NavigationMenuDisplayMode>(nameof(NavigationMenuDisplayMode), NavigationMenuDisplayMode.FixedCompact);

        public NavigationMenuDisplayMode NavigationMenuDisplayMode
        {
            get => GetValue(NavigationMenuDisplayModeProperty);
            set => SetValue(NavigationMenuDisplayModeProperty, value);
        }

    }
}
