namespace Lyt.Avalonia.Controls.Toggle;

public partial class ToggleSwitch : UserControl
{
    private bool isOver;
    private bool isPressed;

    public ToggleSwitch()
    {
        this.InitializeComponent();
        this.eventingRectangle.PointerPressed += this.OnPointerPressed;
        this.eventingRectangle.PointerReleased += this.OnPointerReleased;
        this.eventingRectangle.PointerEntered += this.OnPointerEnter;
        this.eventingRectangle.PointerExited += this.OnPointerLeave;
        this.eventingRectangle.PointerMoved += this.OnPointerMoved;
        this.Loaded += this.OnLoaded;
    }

    ~ToggleSwitch()
    {
        this.eventingRectangle.PointerPressed -= this.OnPointerPressed;
        this.eventingRectangle.PointerReleased -= this.OnPointerReleased;
        this.eventingRectangle.PointerEntered -= this.OnPointerEnter;
        this.eventingRectangle.PointerExited -= this.OnPointerLeave;
        this.eventingRectangle.PointerMoved -= this.OnPointerMoved;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        this.ChangeTypography(this.Typography);
        this.switchEllipse.HorizontalAlignment =
            this.Value ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        this.UpdateVisualState();
        this.InvalidateVisual();
    }

    // According to Forum discussion:
    // this.textBlock.Theme = value;
    // Does not work, because TextBlock is not a TemplatedControl ??? 
    private void ChangeTypography(ControlTheme typography)
    {
        this.trueTextBlock.ApplyControlTheme(typography);
        this.trueTextBlock.Text = this.TrueText;
        this.falseTextBlock.ApplyControlTheme(typography);
        this.falseTextBlock.Text = this.FalseText;
    }

    #region Visual States 

    private bool IsHot => !this.isPressed && this.isOver && !this.IsDisabled;

    private void UpdateVisualState()
    {
        if ((this.GeneralVisualState is null) || 
            (this.BackgroundVisualState is null) || 
            (this.BackgroundBorderVisualState is null))
        {
            return;
        }

        this.eventingRectangle.Fill = Brushes.Transparent;
        if (this.isPressed && !this.IsDisabled)
        {
            this.SetPressedVisualState();
        }
        else if (this.IsHot)
        {
            this.SetHotVisualState();
        }
        else
        {
            if (this.IsDisabled)
            {
                this.SetDisabledVisualState();
            }
            else
            {
                this.SetNormalVisualState();
            }
        }
    }

    private void SetPressedVisualState()
    {
        var pressedColor = this.GeneralVisualState.Pressed;
        var disabledColor = this.GeneralVisualState.Disabled;
        this.switchEllipse.Fill = pressedColor;
        if (this.Value)
        {
            this.trueTextBlock.Foreground = disabledColor;
            this.falseTextBlock.Foreground = pressedColor;
        }
        else
        {
            this.trueTextBlock.Foreground = pressedColor;
            this.falseTextBlock.Foreground = disabledColor;
        }

        this.rectangleBackground.Fill = this.BackgroundVisualState.Pressed;
        this.rectangleBackground.Stroke = this.BackgroundBorderVisualState.Pressed;
    }

    private void SetHotVisualState()
    {
        var hotColor = this.GeneralVisualState.Hot;
        var disabledColor = this.GeneralVisualState.Disabled;
        this.switchEllipse.Fill = hotColor;
        if (this.Value)
        {
            this.trueTextBlock.Foreground = hotColor;
            this.falseTextBlock.Foreground = disabledColor;
        }
        else
        {
            this.trueTextBlock.Foreground = disabledColor;
            this.falseTextBlock.Foreground = hotColor;
        }

        this.rectangleBackground.Fill = this.BackgroundVisualState.Hot;
        this.rectangleBackground.Stroke = this.BackgroundBorderVisualState.Hot;
    }

    private void SetNormalVisualState()
    {
        var normalColor = this.GeneralVisualState.Normal;
        var disabledColor = this.GeneralVisualState.Disabled;
        this.switchEllipse.Fill = normalColor;
        if (this.Value)
        {
            this.trueTextBlock.Foreground = normalColor;
            this.falseTextBlock.Foreground = disabledColor;
        }
        else
        {
            this.trueTextBlock.Foreground = disabledColor;
            this.falseTextBlock.Foreground = normalColor;
        }

        this.rectangleBackground.Fill = this.BackgroundVisualState.Normal;
        this.rectangleBackground.Stroke = this.BackgroundBorderVisualState.Normal;
    }

    private void SetDisabledVisualState()
    {
        var disabledColor = this.GeneralVisualState.Disabled;
        this.trueTextBlock.Foreground = disabledColor;
        this.falseTextBlock.Foreground = disabledColor;
        this.switchEllipse.Fill = disabledColor;
        this.rectangleBackground.Fill = this.BackgroundVisualState.Disabled;
        this.rectangleBackground.Stroke = this.BackgroundBorderVisualState.Disabled;
    }

    #endregion Visual States 

    #region Pointer Handling

    private void OnPointerEnter(object? sender, PointerEventArgs args)
    {
        // Debug.WriteLine("Pointer Enter");
        if (this.eventingRectangle.IsPointerOver)
        {
            this.Enter();
        }
    }

    private void OnPointerLeave(object? sender, PointerEventArgs args)
    {
        // Debug.WriteLine("Pointer Leave");
        // Debugger.Break();
        if (!this.eventingRectangle.IsPointerOver)
        {
            this.Leave();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        // Debug.WriteLine("Pointer Pressed");
        if (this.eventingRectangle.IsPointerOver)
        {
            this.Down();
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs args)
    {
        // Debug.WriteLine("Pointer Released");
        if (this.eventingRectangle.IsPointerOver)
        {
            this.Up(args);
        }
        else
        {
            this.Leave();
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs args)
    {
        // Debug.WriteLine("Pointer Moved");
        if (!this.isOver)
        {
            // Debug.WriteLine("Not Over: ignore");
            return;
        }

        if (!this.eventingRectangle.IsPointerInside(args))
        {
            // Debug.WriteLine("Pointer outside : leave");
            this.Leave();
        }

        // Debug.WriteLine("Pointer inside : ignore");
    }

    private void Enter()
    {
        // Debug.WriteLine("Enter");
        this.isOver = true;
        this.UpdateVisualState();
    }

    private void Leave()
    {
        bool needToLeave = this.isOver || this.isPressed;
        if (!needToLeave)
        {
            // Debug.WriteLine("No need to Leave, return");
            return;
        }

        // Debug.WriteLine("Leave");
        this.isOver = false;
        this.isPressed = false;
        this.UpdateVisualState();
    }

    private void Down()
    {
        // Debug.WriteLine("Down");
        this.isPressed = true;
        this.UpdateVisualState();
    }

    private void Up(PointerReleasedEventArgs args)
    {
        // Debug.WriteLine("Up");
        if (!this.eventingRectangle.IsPointerInside(args))
        {
            // Debug.WriteLine("Pointer outside : leave");
            this.Leave();
        }
        else
        {
            this.isPressed = false;
            this.UpdateVisualState();
            this.ActivateCommand(args);
        }
    }

    #endregion Pointer Handling

    #region Commanding 

    private void PreventMultipleClicks()
    {
        this.IsEnabled = false;
        Task.Run(async () =>
        {
            await Task.Delay(250);
            Dispatcher.UIThread.Post((Action)delegate { this.IsEnabled = true; });
        });
    }

    private void ActivateCommand(RoutedEventArgs rea)
    {
        if (this.IsDisabled)
        {
            // This should never happen
            if (Debugger.IsAttached) { Debugger.Break(); }
            return;
        }

        this.Value = !this.Value;
        this.UpdateVisualState();

        // Give precedence to the Click handler if present 
        if (this.Click != null)
        {
            this.PreventMultipleClicks();
            this.Click.Invoke(this, rea);
        }
        else if (this.Command != null)
        {
            object? tag = this.Value;
            if (this.Command.CanExecute(tag))
            {
                this.PreventMultipleClicks();
                this.Command.Execute(tag);
            }
        }
    }

    #endregion Commanding 
}
