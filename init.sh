#!/usr/bin/env bash

set -euo pipefail

PROJECT="Maui.MacIdiom.Compatibility"

if [[ ! -d "$PROJECT" ]]; then
    echo "Error: Project directory '$PROJECT' does not exist."
    exit 1
fi

cd "$PROJECT"

mkdir -p Extensions
mkdir -p Handlers
mkdir -p Platform

write_if_missing() {
    local file="$1"

    if [[ -e "$file" ]]; then
        echo "Skipping existing file: $file"
        return
    fi

    cat > "$file"
    echo "Created: $file"
}

write_if_missing "Platform/MacIdiom.cs" <<'EOF'
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
EOF

write_if_missing "Handlers/MacPickerHandler.cs" <<'EOF'
#if MACCATALYST

using Microsoft.Maui.Handlers;
using UIKit;

namespace Maui.MacIdiom.Compatibility.Handlers;

/// <summary>
/// Mac Catalyst handler used to provide a Mac-idiom-compatible
/// implementation of the MAUI Picker control.
/// </summary>
public class MacPickerHandler : PickerHandler
{
    protected override void ConnectHandler(MauiPicker platformView)
    {
        base.ConnectHandler(platformView);

        // TODO:
        // Detect MacIdiom.IsMac and replace the unsupported UIPickerView
        // interaction with a Mac-compatible menu or popover.
    }

    protected override void DisconnectHandler(MauiPicker platformView)
    {
        // TODO:
        // Remove any Mac-specific event handlers here.

        base.DisconnectHandler(platformView);
    }
}

#endif
EOF

write_if_missing "Handlers/MacStepperHandler.cs" <<'EOF'
#if MACCATALYST

using Microsoft.Maui.Handlers;
using UIKit;

namespace Maui.MacIdiom.Compatibility.Handlers;

/// <summary>
/// Mac Catalyst handler used to provide a Mac-idiom-compatible
/// implementation of the MAUI Stepper control.
/// </summary>
public class MacStepperHandler : StepperHandler
{
    protected override void ConnectHandler(UIStepper platformView)
    {
        base.ConnectHandler(platformView);

        // TODO:
        // UIStepper is unsupported in the native Mac idiom.
        // Replace this handler's platform implementation with a
        // Mac-compatible minus/value/plus control.
    }

    protected override void DisconnectHandler(UIStepper platformView)
    {
        base.DisconnectHandler(platformView);
    }
}

#endif
EOF

write_if_missing "Handlers/MacRefreshViewHandler.cs" <<'EOF'
#if MACCATALYST

using Microsoft.Maui.Handlers;

namespace Maui.MacIdiom.Compatibility.Handlers;

/// <summary>
/// Mac Catalyst handler used to provide a Mac-idiom-compatible
/// implementation of the MAUI RefreshView control.
/// </summary>
public class MacRefreshViewHandler : RefreshViewHandler
{
    protected override void ConnectHandler(
        MauiRefreshView platformView)
    {
        base.ConnectHandler(platformView);

        // TODO:
        // UIRefreshControl is unsupported in the native Mac idiom.
        // Provide a Mac-compatible refresh mechanism.
    }

    protected override void DisconnectHandler(
        MauiRefreshView platformView)
    {
        base.DisconnectHandler(platformView);
    }
}

#endif
EOF

write_if_missing "Extensions/MauiAppBuilderExtensions.cs" <<'EOF'
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
EOF

echo
echo "Maui.MacIdiom.Compatibility scaffold created."
echo
echo "Add this to MauiProgram.cs:"
echo
echo "    builder"
echo "        .UseMauiApp<App>()"
echo "        .UseMacIdiomCompatibility();"