#if MACCATALYST

using UIKit;

namespace Maui.MacIdiom.Compatibility.Platform;

/// <summary>
/// Provides helpers for detecting the current Mac Catalyst UI idiom.
/// </summary>
internal static class MacIdiom
{
    /// <summary>
    /// Gets a value indicating whether the application is currently
    /// running using the native Mac user interface idiom.
    /// </summary>
    public static bool IsMac
    {
        get
        {
            var viewController =
                Microsoft.Maui.ApplicationModel.Platform
                    .GetCurrentUIViewController();

            return viewController?
                       .TraitCollection
                       .UserInterfaceIdiom ==
                   UIUserInterfaceIdiom.Mac;
        }
    }
}

#endif
