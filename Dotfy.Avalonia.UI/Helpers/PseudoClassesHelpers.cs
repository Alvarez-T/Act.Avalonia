using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace Dotfy.Avalonia.UI.Helpers;

public static class PseudoClassesHelpers
{
    public static IEnumerable<string> GetPseudoClassesNames<T>() where T : class
    {
        Type type = typeof(T);
        if (!type.IsDefined(typeof(PseudoClassesAttribute), false))
            return [];

        var pseudoClasses = new List<string>();

        var attributes = typeof(T).GetCustomAttributes(typeof(PseudoClassesAttribute), false);

        foreach (PseudoClassesAttribute attribute in attributes)
            pseudoClasses.AddRange(attribute.PseudoClasses);

        return pseudoClasses;
    }

    public static void ResetAllPseudoClasses(this StyledElement element)
    {
        var pseudoClasses = UnsafeAccessors.GetPseudoClasses(element);

        GetPseudoClassesNames<StyledElement>().Select(e =>
        {
            pseudoClasses.Set(e, false);
            return e;
        });
    }
}