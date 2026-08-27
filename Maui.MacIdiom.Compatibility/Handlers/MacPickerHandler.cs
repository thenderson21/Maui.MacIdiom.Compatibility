#if MACCATALYST

using Microsoft.Maui;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Maui.MacIdiom.Compatibility.Handlers;

/// <summary>
/// Mac-idiom-compatible implementation of a MAUI Picker.
///
/// UIPickerView is unsupported when Mac Catalyst runs using the Mac UI idiom,
/// so this handler uses UIButton + UIMenu instead.
/// </summary>
public sealed class MacPickerHandler : ViewHandler<IPicker, UIButton>
{
    public static readonly IPropertyMapper<IPicker, MacPickerHandler> Mapper =
        new PropertyMapper<IPicker, MacPickerHandler>(ViewMapper)
        {
            [nameof(IPicker.SelectedIndex)] = MapSelectedIndex,
            [nameof(IPicker.TextColor)] = MapTextColor,
            [nameof(IPicker.Title)] = MapTitle,
            [nameof(IPicker.TitleColor)] = MapTitleColor,
            [nameof(IPicker.Items)] = MapItems,
            [nameof(IPicker.Font)] = MapFont,
            [nameof(ITextAlignment.HorizontalTextAlignment)] =
                MapHorizontalTextAlignment,
            [nameof(ITextAlignment.VerticalTextAlignment)] =
                MapVerticalTextAlignment,
        };

    public MacPickerHandler()
        : base(Mapper)
    {
    }

    protected override UIButton CreatePlatformView()
    {
        return new UIButton(UIButtonType.System)
        {
            ShowsMenuAsPrimaryAction = true
        };
    }

    protected override void ConnectHandler(UIButton platformView)
    {
        base.ConnectHandler(platformView);

        UpdateMenu();
        UpdateTitle();
    }

    protected override void DisconnectHandler(UIButton platformView)
    {
        platformView.Menu = null;

        base.DisconnectHandler(platformView);
    }

    private void UpdateMenu()
    {
        if (PlatformView is null || VirtualView is null)
            return;

        var count = VirtualView.GetCount();

        if (count == 0)
        {
            PlatformView.Menu = null;
            UpdateTitle();
            return;
        }

        var actions = new UIAction[count];

        for (var index = 0; index < count; index++)
        {
            var itemIndex = index;
            var item = VirtualView.GetItem(index) ?? string.Empty;

            var action = UIAction.Create(
                item,
                null,
                null,
                _ =>
                {
                    VirtualView.SelectedIndex = itemIndex;
                });

            if (index == VirtualView.SelectedIndex)
                action.State = UIMenuElementState.On;

            actions[index] = action;
        }

        PlatformView.Menu = UIMenu.Create(actions);

        UpdateTitle();
    }

    private void UpdateTitle()
    {
        if (PlatformView is null || VirtualView is null)
            return;

        string text;

        if (VirtualView.SelectedIndex >= 0 &&
            VirtualView.SelectedIndex < VirtualView.GetCount())
        {
            text =
                VirtualView.GetItem(VirtualView.SelectedIndex)
                ?? string.Empty;
        }
        else
        {
            text = VirtualView.Title ?? string.Empty;
        }

        PlatformView.SetTitle(
            text,
            UIControlState.Normal);

        UpdateTitleColor();
    }

    private void UpdateTitleColor()
    {
        if (PlatformView is null || VirtualView is null)
            return;

        var color =
            VirtualView.SelectedIndex >= 0
                ? VirtualView.TextColor
                : VirtualView.TitleColor;

        if (color is not null)
        {
            PlatformView.SetTitleColor(
                color.ToPlatform(),
                UIControlState.Normal);
        }
    }

    private static void MapItems(
        MacPickerHandler handler,
        IPicker picker)
    {
        handler.UpdateMenu();
    }

    private static void MapSelectedIndex(
        MacPickerHandler handler,
        IPicker picker)
    {
        handler.UpdateMenu();
    }

    private static void MapTitle(
        MacPickerHandler handler,
        IPicker picker)
    {
        handler.UpdateTitle();
    }

    private static void MapTextColor(
        MacPickerHandler handler,
        IPicker picker)
    {
        handler.UpdateTitleColor();
    }

    private static void MapTitleColor(
        MacPickerHandler handler,
        IPicker picker)
    {
        handler.UpdateTitleColor();
    }

    private static void MapFont(
        MacPickerHandler handler,
        IPicker picker)
    {
        if (handler.PlatformView?.TitleLabel is null)
            return;

        var fontManager =
            handler.GetRequiredService<IFontManager>();

        handler.PlatformView.TitleLabel.Font =
            picker.Font.ToUIFont(fontManager);
    }

    private static void MapHorizontalTextAlignment(
        MacPickerHandler handler,
        IPicker picker)
    {
        if (handler.PlatformView is null)
            return;

        handler.PlatformView.HorizontalAlignment =
            picker.HorizontalTextAlignment switch
            {
                TextAlignment.Start =>
                    UIControlContentHorizontalAlignment.Leading,

                TextAlignment.End =>
                    UIControlContentHorizontalAlignment.Trailing,

                _ =>
                    UIControlContentHorizontalAlignment.Center
            };
    }

    private static void MapVerticalTextAlignment(
        MacPickerHandler handler,
        IPicker picker)
    {
        if (handler.PlatformView is null)
            return;

        handler.PlatformView.VerticalAlignment =
            picker.VerticalTextAlignment switch
            {
                TextAlignment.Start =>
                    UIControlContentVerticalAlignment.Top,

                TextAlignment.End =>
                    UIControlContentVerticalAlignment.Bottom,

                _ =>
                    UIControlContentVerticalAlignment.Center
            };
    }
}

#endif