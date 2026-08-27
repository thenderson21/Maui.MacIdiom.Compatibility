#if MACCATALYST

using CoreGraphics;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Maui.MacIdiom.Compatibility.Handlers;

public sealed class MacStepperHandler
    : ViewHandler<IStepper, MacStepperView>
{
    public static readonly IPropertyMapper<
        IStepper,
        MacStepperHandler> Mapper =
        new PropertyMapper<IStepper, MacStepperHandler>(ViewMapper)
        {
            [nameof(IStepper.Minimum)] = MapMinimum,
            [nameof(IStepper.Maximum)] = MapMaximum,
            [nameof(IStepper.Interval)] = MapInterval,
            [nameof(IStepper.Value)] = MapValue
        };

    public MacStepperHandler()
        : base(Mapper)
    {
    }

    protected override MacStepperView CreatePlatformView()
    {
        return new MacStepperView();
    }

    protected override void ConnectHandler(
        MacStepperView platformView)
    {
        base.ConnectHandler(platformView);

        platformView.DecrementRequested += OnDecrementRequested;
        platformView.IncrementRequested += OnIncrementRequested;

        UpdateView();
    }

    protected override void DisconnectHandler(
        MacStepperView platformView)
    {
        platformView.DecrementRequested -= OnDecrementRequested;
        platformView.IncrementRequested -= OnIncrementRequested;

        base.DisconnectHandler(platformView);
    }

    private void OnDecrementRequested(
        object? sender,
        EventArgs e)
    {
        if (VirtualView is null)
        {
            return;
        }

        var value = Math.Max(
            VirtualView.Minimum,
            VirtualView.Value - VirtualView.Interval);

        VirtualView.Value = value;
    }

    private void OnIncrementRequested(
        object? sender,
        EventArgs e)
    {
        if (VirtualView is null)
        {
            return;
        }

        var value = Math.Min(
            VirtualView.Maximum,
            VirtualView.Value + VirtualView.Interval);

        VirtualView.Value = value;
    }

    private void UpdateView()
    {
        if (VirtualView is null || PlatformView is null)
        {
            return;
        }

        PlatformView.Update(
            VirtualView.Value,
            VirtualView.Minimum,
            VirtualView.Maximum);
    }

    private static void MapMinimum(
        MacStepperHandler handler,
        IStepper stepper)
    {
        handler.UpdateView();
    }

    private static void MapMaximum(
        MacStepperHandler handler,
        IStepper stepper)
    {
        handler.UpdateView();
    }

    private static void MapInterval(
        MacStepperHandler handler,
        IStepper stepper)
    {
        handler.UpdateView();
    }

    private static void MapValue(
        MacStepperHandler handler,
        IStepper stepper)
    {
        handler.UpdateView();
    }
}

public sealed class MacStepperView : UIView
{
    private readonly UIButton _decrementButton;
    private readonly UIButton _incrementButton;
    private readonly UILabel _valueLabel;

    public event EventHandler? DecrementRequested;
    public event EventHandler? IncrementRequested;

    public MacStepperView()
    {
        _decrementButton = new UIButton(UIButtonType.System);
        _incrementButton = new UIButton(UIButtonType.System);
        _valueLabel = new UILabel();

        _decrementButton.SetTitle(
            "−",
            UIControlState.Normal);

        _incrementButton.SetTitle(
            "+",
            UIControlState.Normal);

        _decrementButton.AccessibilityLabel = "Decrease";
        _incrementButton.AccessibilityLabel = "Increase";

        _valueLabel.TextAlignment = UITextAlignment.Center;

        AddSubview(_decrementButton);
        AddSubview(_valueLabel);
        AddSubview(_incrementButton);

        _decrementButton.TouchUpInside += OnDecrementClicked;
        _incrementButton.TouchUpInside += OnIncrementClicked;
    }

    public void Update(
        double value,
        double minimum,
        double maximum)
    {
        _valueLabel.Text = FormatValue(value);

        _decrementButton.Enabled = value > minimum;
        _incrementButton.Enabled = value < maximum;

        _valueLabel.AccessibilityValue =
            _valueLabel.Text;
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();

        nfloat buttonWidth = 28;
        nfloat spacing = 6;

        var height = Bounds.Height;

        _decrementButton.Frame = new CGRect(
            0,
            0,
            buttonWidth,
            height);

        _incrementButton.Frame = new CGRect(
            Bounds.Width - buttonWidth,
            0,
            buttonWidth,
            height);

        _valueLabel.Frame = new CGRect(
            buttonWidth + spacing,
            0,
            Math.Max(
                0,
                Bounds.Width -
                (buttonWidth * 2) -
                (spacing * 2)),
            height);
    }

    public override CGSize SizeThatFits(CGSize size)
    {
        return new CGSize(110, 28);
    }

    private static string FormatValue(double value)
    {
        if (Math.Abs(value % 1) < double.Epsilon)
        {
            return ((long)value).ToString();
        }

        return value.ToString("0.##");
    }

    private void OnDecrementClicked(
        object? sender,
        EventArgs e)
    {
        DecrementRequested?.Invoke(
            this,
            EventArgs.Empty);
    }

    private void OnIncrementClicked(
        object? sender,
        EventArgs e)
    {
        IncrementRequested?.Invoke(
            this,
            EventArgs.Empty);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _decrementButton.TouchUpInside -=
                OnDecrementClicked;

            _incrementButton.TouchUpInside -=
                OnIncrementClicked;
        }

        base.Dispose(disposing);
    }
}

#endif