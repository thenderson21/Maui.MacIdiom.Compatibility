using Microsoft.Maui.Hosting;

#if MACCATALYST
using Maui.MacIdiom.Compatibility.Handlers;
using Microsoft.Maui.Controls;
#endif

namespace Maui.MacIdiom.Compatibility;

/// <summary>
/// Extension methods for enabling Mac UI idiom compatibility.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Enables compatibility handlers for controls that are unsupported
    /// by UIKit when Mac Catalyst is running in the native Mac UI idiom.
    /// </summary>
    /// <param name="builder">The MAUI application builder.</param>
    /// <returns>The application builder.</returns>
    public static MauiAppBuilder UseMacIdiomCompatibility(
        this MauiAppBuilder builder)
    {
#if MACCATALYST
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<Picker, MacPickerHandler>();
            handlers.AddHandler<Stepper, MacStepperHandler>();
            handlers.AddHandler<RefreshView, MacRefreshViewHandler>();
        });
#endif

        return builder;
    }
}
