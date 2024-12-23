﻿using Avalonia;

namespace Act.Avalonia.UI.Attaches;

internal class ClassAttach
{
    static ClassAttach()
    {
        ClassesProperty.Changed.AddClassHandler<StyledElement>(OnClassesChanged);
    }

    public static readonly AttachedProperty<string> ClassesProperty =
        AvaloniaProperty.RegisterAttached<ClassAttach, StyledElement, string>("Classes");

    public static void SetClasses(AvaloniaObject obj, string value) => obj.SetValue(ClassesProperty, value);
    public static string GetClasses(AvaloniaObject obj) => obj.GetValue(ClassesProperty);

    private static void OnClassesChanged(StyledElement sender, AvaloniaPropertyChangedEventArgs value)
    {
        string classes = value.GetNewValue<string>();
        sender.Classes.Clear();
        sender.Classes.Add(classes);
    }
}