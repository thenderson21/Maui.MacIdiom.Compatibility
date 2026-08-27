#if MACCATALYST

using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Maui.MacIdiom.Compatibility.Handlers;

public sealed class MacRefreshViewHandler
    : ViewHandler<IRefreshView, MacRefreshView>
{
    public static readonly IPropertyMapper<
        IRefreshView,
        MacRefreshViewHandler> Mapper =
        new PropertyMapper<IRefreshView, MacRefreshViewHandler>(
            ViewMapper)
        {
            [nameof(IRefreshView.IsRefreshing)] = MapIsRefreshing
        };

    public MacRefreshViewHandler()
        : base(Mapper)
    {
    }

    protected override MacRefreshView CreatePlatformView()
    {
        return new MacRefreshView();
    }

    protected override void ConnectHandler(
        MacRefreshView platformView)
    {
        base.ConnectHandler(platformView);

        platformView.RefreshRequested += OnRefreshRequested;
    }

    protected override void DisconnectHandler(
        MacRefreshView platformView)
    {
        platformView.RefreshRequested -= OnRefreshRequested;

        base.DisconnectHandler(platformView);
    }

    private void OnRefreshRequested(
        object? sender,
        EventArgs e)
    {
        if (VirtualView is null || VirtualView.IsRefreshing)
        {
            return;
        }

        VirtualView.IsRefreshing = true;
    }

    private static void MapIsRefreshing(
        MacRefreshViewHandler handler,
        IRefreshView refreshView)
    {
        handler.PlatformView?.SetRefreshing(
            refreshView.IsRefreshing);
    }
}

public sealed class MacRefreshView : UIView
{
    private readonly UIButton _refreshButton;
    private readonly UIActivityIndicatorView _activityIndicator;

    public event EventHandler? RefreshRequested;

    public MacRefreshView()
    {
        _refreshButton = new UIButton(UIButtonType.System);

        _refreshButton.SetImage(
            UIImage.GetSystemImage("arrow.clockwise"),
            UIControlState.Normal);

        _refreshButton.AccessibilityLabel = "Refresh";

        _activityIndicator =
            new UIActivityIndicatorView(
                UIActivityIndicatorViewStyle.Medium);

        _refreshButton.TouchUpInside += OnRefreshClicked;

        AddSubview(_refreshButton);
        AddSubview(_activityIndicator);
    }

    private void OnRefreshClicked(
        object? sender,
        EventArgs e)
    {
        RefreshRequested?.Invoke(this, EventArgs.Empty);
    }

    public void SetRefreshing(bool refreshing)
    {
        _refreshButton.Hidden = refreshing;
        _refreshButton.Enabled = !refreshing;

        if (refreshing)
        {
            _activityIndicator.StartAnimating();
        }
        else
        {
            _activityIndicator.StopAnimating();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshButton.TouchUpInside -= OnRefreshClicked;
        }

        base.Dispose(disposing);
    }
}

#endif
