# ToddHenderson.Maui.MacIdiom.Compatibility

Compatibility handlers for .NET MAUI applications running on Mac Catalyst with the native Mac user-interface idiom.

Some UIKit controls used by the standard .NET MAUI handlers are unavailable when a Mac Catalyst application uses the Mac UI idiom. This package supplies alternative handlers so common MAUI controls remain usable in that environment.

## Included handlers

| MAUI control | Mac Catalyst implementation |
| --- | --- |
| `Picker` | A button-backed menu that replaces the unsupported `UIPickerView` path |
| `Stepper` | Accessible decrement and increment buttons with a value display |
| `RefreshView` | A refresh button with an activity indicator |

Your existing XAML and control bindings do not need to change. The package replaces the handlers during application startup.

## Requirements

- .NET 10
- .NET MAUI 10
- A project targeting `net10.0-maccatalyst`

The package also contains `net10.0`, `net10.0-android`, and `net10.0-ios` assets so it can be referenced by a multi-targeted MAUI project. Registration is a no-op outside Mac Catalyst.

## Installation

Install the stable package from NuGet.org:

```bash
dotnet add package ToddHenderson.Maui.MacIdiom.Compatibility --version 0.0.3
```

Or add it directly to your project file:

```xml
<PackageReference Include="ToddHenderson.Maui.MacIdiom.Compatibility"
                  Version="0.0.3" />
```

Prerelease builds are published from the repository's `develop` branch. Pass `--prerelease` when you intentionally want to test the latest preview:

```bash
dotnet add package ToddHenderson.Maui.MacIdiom.Compatibility --prerelease
```

## Setup

Call `UseMacIdiomCompatibility()` while creating the MAUI app in `MauiProgram.cs`:

```csharp
using Maui.MacIdiom.Compatibility;

namespace MyMauiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMacIdiomCompatibility();

        return builder.Build();
    }
}
```

The call is safe in shared startup code. On Mac Catalyst it registers the compatibility handlers for `Picker`, `Stepper`, and `RefreshView`; on other target frameworks it returns the builder without registering handlers.

## Usage

Continue using standard MAUI controls. For example:

```xml
<VerticalStackLayout Padding="24" Spacing="16">
    <Picker Title="Choose an option"
            ItemsSource="{Binding Options}"
            SelectedItem="{Binding SelectedOption}" />

    <Stepper Minimum="0"
             Maximum="10"
             Increment="1"
             Value="{Binding Quantity}" />

    <RefreshView IsRefreshing="{Binding IsRefreshing}"
                 Command="{Binding RefreshCommand}">
        <CollectionView ItemsSource="{Binding Items}" />
    </RefreshView>
</VerticalStackLayout>
```

## Handler-registration order

`UseMacIdiomCompatibility()` registers handlers for all three supported controls. If your application installs another custom handler for `Picker`, `Stepper`, or `RefreshView`, the last registration for that control wins. Order the startup calls according to which handler you want to use.

## Versioning and release channels

- Commits published from `develop` produce public prerelease versions such as `0.0.3-preview.12`.
- Commits published from `main` produce stable versions such as `0.0.3`.

Package versions on NuGet.org are immutable. Use an explicit version in production applications and review changes before upgrading.

## Source and issues

Source code and issue tracking are available in the [GitHub repository](https://github.com/thenderson21/Maui.MacIdiom.Compatibility).

## License

Licensed under the [MIT License](LICENSE).
