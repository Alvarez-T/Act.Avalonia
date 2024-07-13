using System.Collections.Concurrent;
using Dotfy.Avalonia.UI.TemplatedControls;

namespace Dotfy.Avalonia.UI.Common;

internal static class OverlayDialogManager
{
    private static DialogOverlayHost? _defaultHost;
    private static readonly ConcurrentDictionary<string, DialogOverlayHost> Hosts = new();

    public static void RegisterHost(DialogOverlayHost host, string? id)
    {
        if (id == null)
        {
            if (_defaultHost != null)
            {
                throw new InvalidOperationException("Cannot register multiple OverlayDialogHost with empty HostId");
            }
            _defaultHost = host;
            return;
        }
        Hosts.TryAdd(id, host);
    }

    public static void UnregisterHost(string? id)
    {
        if (id is null)
        {
            _defaultHost = null;
            return;
        }
        Hosts.TryRemove(id, out _);
    }

    public static DialogOverlayHost? GetHost(string? id)
    {
        if (id is null)
        {
            return _defaultHost;
        }
        return Hosts.TryGetValue(id, out var host) ? host : null;
    }
}