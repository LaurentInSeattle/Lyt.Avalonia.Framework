namespace Lyt.Avalonia.Controls.Glyphs;

public partial class GlyphButton : UserControl, ICanSelect 
{
    // private const double KeyboardPressMilliseconds = 600.0;

    private const double ViewboxDefaultMargin = 4.0;
    private const double LongPressMilliseconds = 950.0;
    private const int KeyboardTimerIntervalMilliseconds = 75;
    private const int LongPressTimerIntervalMilliseconds = 75;
    private const int CountdownTimerIntervalMilliseconds = 100;
    private const int ContinuousTimerIntervalMilliseconds = 250;

    private DispatcherTimer? timer;
    private bool isOver;
    private DateTime pressedAt;
    private bool isPressed;
    private bool isLongPressActivated;
    private bool isContinuousEnded;
    private double viewboxMargin;
    private double glyphAngle;

    public GlyphButton()
    {
        this.InitializeComponent();
        this.eventingRectangle.PointerPressed += this.OnPointerPressed;
        this.eventingRectangle.PointerReleased += this.OnPointerReleased;
        this.eventingRectangle.PointerEntered += this.OnPointerEnter;
        this.eventingRectangle.PointerExited += this.OnPointerLeave;
        this.eventingRectangle.PointerMoved += this.OnPointerMoved;
        this.Loaded += this.OnLoaded;
    }

    ~GlyphButton()
    {
        this.eventingRectangle.PointerPressed -= this.OnPointerPressed;
        this.eventingRectangle.PointerReleased -= this.OnPointerReleased;
        this.eventingRectangle.PointerEntered -= this.OnPointerEnter;
        this.eventingRectangle.PointerExited -= this.OnPointerLeave;
        this.eventingRectangle.PointerMoved -= this.OnPointerMoved;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        //this.DismissPopupKeyboard();
        
        this.ChangeButtonBackground(this.ButtonBackground);
        this.ChangeLayout(this.Layout);
        this.ChangeBehaviour(this.Behaviour);
        this.ChangeTypography(this.Typography);
        this.icon.UpdateImage();
        this.UpdateVisualState();
        this.InvalidateVisual();

        this.Group?.Register(this); 
    }

    #region Layout Updates 

    private bool HasIcon
        => (this.icon is not null) &&
            (this.Layout == ButtonLayout.IconOnly ||
            this.Layout == ButtonLayout.IconTextRightSide ||
            this.Layout == ButtonLayout.IconTextBelow);

    private bool HasText
        => (this.textBlock is not null) &&
            (this.Layout == ButtonLayout.IconTextRightSide ||
            this.Layout == ButtonLayout.IconTextBelow ||
            this.Layout == ButtonLayout.TextOnly);

    private bool HasBackgroundRectangle
         => this.ButtonBackground == ButtonBackground.Rectangle ||
             this.ButtonBackground == ButtonBackground.BorderlessRectangle;

    private bool HasBackgroundBorder
        => this.ButtonBackground == ButtonBackground.Rectangle ||
            this.ButtonBackground == ButtonBackground.BorderOnly;

    // According to Forum discussion:
    // this.textBlock.Theme = value;
    // Does not work, because TextBlock is not a TemplatedControl ??? 
    private void ChangeTypography(ControlTheme typography)
    {
        this.textBlock.ApplyControlTheme(typography);
        this.textBlock.Text = this.Text;
    }

    private void ChangeBehaviour(ButtonBehaviour behaviour)
        => this.gridPopup.IsVisible = behaviour == ButtonBehaviour.Keyboard;

    private void ChangeLayout(ButtonLayout layout)
    {
        switch (layout)
        {
            default:
            case ButtonLayout.IconOnly:
                this.Text = string.Empty;
                this.textBlock.IsVisible = false; // Visibility.Hidden;
                this.icon.IsVisible = true; // Visibility.Visible;
                //this.mainGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Pixel);
                this.mainGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Pixel);
                this.rectangleBackground.SetValue(Grid.RowSpanProperty, 1);
                break;

            case ButtonLayout.IconTextBelow:
                this.icon.IsVisible = true; //Visibility.Visible;
                this.textBlock.IsVisible = true; //Visibility.Visible;
                this.textBlock.SetValue(Grid.RowProperty, 1);
                this.textBlock.SetValue(Grid.ColumnProperty, 0);
                this.textBlock.SetValue(Grid.ColumnSpanProperty, 1);
                this.textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                this.textBlock.Margin = new Thickness(0, 2, 0, 0);
                this.mainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
                break;

            case ButtonLayout.IconTextRightSide:
                this.mainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                this.mainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                this.mainGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Pixel);
                this.icon.IsVisible = true; //Visibility.Visible;
                this.icon.Margin = new Thickness(6);
                this.textBlock.IsVisible = true; //Visibility.Visible;
                this.border.SetValue(Grid.RowProperty, 0);
                this.border.SetValue(Grid.ColumnProperty, 1);
                this.textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                this.textBlock.SetValue(HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                this.textBlock.Margin = new Thickness(4, 0, 0, 0);
                break;

            case ButtonLayout.TextOnly:
                this.textBlock.IsVisible = true; //Visibility.Visible;
                this.viewBox.IsVisible = false; //Visibility.Hidden;
                this.icon.IsVisible = false; //Visibility.Hidden;
                this.border.SetValue(Grid.RowProperty, 0);
                this.border.SetValue(Grid.ColumnProperty, 0);
                this.textBlock.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                this.textBlock.SetValue(HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                this.textBlock.SetValue(MarginProperty, new Thickness(4));
                //this.mainGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Pixel);
                this.mainGrid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Pixel);
                break;
        }
    }

    private void ChangeButtonBackground(ButtonBackground newButtonBackground)
    {
        switch (newButtonBackground)
        {
            default:
            case ButtonBackground.None:
                this.viewboxMargin = GlyphButton.ViewboxDefaultMargin;
                this.viewBox.Margin = new Thickness(GlyphButton.ViewboxDefaultMargin);
                this.rectangleBackground.IsVisible = false; //  Visibility.Hidden;
                break;

            case ButtonBackground.BorderOnly:
                this.viewboxMargin = GlyphButton.ViewboxDefaultMargin + this.BackgroundBorderThickness;
                this.viewBox.Margin = new Thickness(this.viewboxMargin, 2.0 + this.viewboxMargin, this.viewboxMargin, this.viewboxMargin);
                this.rectangleBackground.IsVisible = true; // Visibility.Visible;
                this.rectangleBackground.Fill = Brushes.Transparent;
                break;

            case ButtonBackground.Rectangle:
                this.viewboxMargin = GlyphButton.ViewboxDefaultMargin + this.BackgroundBorderThickness;
                this.viewBox.Margin = new Thickness(this.viewboxMargin, 2.0 + this.viewboxMargin, this.viewboxMargin, this.viewboxMargin);
                this.rectangleBackground.IsVisible = true; // Visibility.Visible;
                this.rectangleBackground.Fill = Brushes.Transparent;// todo
                break;

            case ButtonBackground.BorderlessRectangle:
                this.viewboxMargin = GlyphButton.ViewboxDefaultMargin;
                this.viewBox.Margin = new Thickness(GlyphButton.ViewboxDefaultMargin);
                this.rectangleBackground.StrokeThickness = 0.0;
                this.rectangleBackground.IsVisible = true; // Visibility.Visible;
                this.rectangleBackground.Fill = Brushes.Transparent; // todo
                break;
        }
    }

    #endregion Layout Updates 

    #region Visual States 

    private bool IsHot => !this.isPressed && this.isOver && !this.IsDisabled; 

    private void UpdateVisualState()
    {
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
            else if (this.IsSelected)
            {
                this.SetSelectedVisualState();
            }
            else
            {
                this.SetNormalVisualState();
            }
        }
    }

    private void SetPressedVisualState()
    {
        this.eventingRectangle.Fill = Brushes.Transparent;
        var pressedColor = this.GeneralVisualState.Pressed; 
        if (this.HasIcon)
        {
            this.icon.Foreground = pressedColor;
            this.GlyphAngle = this.glyphAngle;
        }

        if (this.HasText)
        {
            this.textBlock.Foreground = pressedColor;
        }

        if (this.HasBackgroundRectangle)
        {
            this.rectangleBackground.Fill = this.BackgroundVisualState.Pressed; 
        }

        if (this.HasBackgroundBorder)
        {
            this.rectangleBackground.Stroke = this.BackgroundBorderVisualState.Pressed;
        }
    }

    private void SetHotVisualState()
    {
        this.eventingRectangle.Fill = Brushes.Transparent;
        var hotColor = this.GeneralVisualState.Hot;
        if (this.HasIcon)
        {
            this.icon.Foreground = hotColor;
            // Consider creating a property for this 'feature'...
            // feature that has been given some negative feedback.
            // this.GlyphAngle = this.glyphAngle + 3.0; 
            this.viewBox.Margin = new Thickness(this.viewboxMargin + 2.0);
        }

        if (this.HasText)
        {
            this.textBlock.Foreground = hotColor;
        }

        if (this.HasBackgroundRectangle)
        {
            this.rectangleBackground.Fill = this.BackgroundVisualState.Hot;
        }

        if (this.HasBackgroundBorder)
        {
            this.rectangleBackground.Stroke = this.BackgroundBorderVisualState.Hot; 
        }
    }

    private void SetNormalVisualState()
    {
        this.eventingRectangle.Fill = Brushes.Transparent;
        
        if (this.HasIcon)
        {
            this.icon.Foreground = this.GeneralVisualState.Normal;
            this.GlyphAngle = this.glyphAngle;
            this.viewBox.Margin = new Thickness(this.viewboxMargin);
        }

        if (this.HasText)
        {
            this.textBlock.Foreground = this.GeneralVisualState.Normal;
        }

        if (this.HasBackgroundRectangle)
        {
            this.rectangleBackground.Fill = this.BackgroundVisualState.Normal;
        }

        if (this.HasBackgroundBorder)
        {
            this.rectangleBackground.Stroke = this.BackgroundBorderVisualState.Normal;
        }
    }

    private void SetSelectedVisualState()
    {
        this.eventingRectangle.Fill = Brushes.Transparent;
        if (this.HasIcon)
        {
            this.icon.Foreground = this.GeneralVisualState.Selected; 
            this.viewBox.Margin = new Thickness(this.viewboxMargin);
            this.GlyphAngle = this.glyphAngle;
        }

        if (this.HasText)
        {
            this.textBlock.Foreground = this.GeneralVisualState.Selected;
        }

        if (this.HasBackgroundRectangle)
        {
            this.rectangleBackground.Fill = this.BackgroundVisualState.Selected;
        }

        if (this.HasBackgroundBorder)
        {
            this.rectangleBackground.Stroke = this.BackgroundBorderVisualState.Selected;
        }
    }

    private void SetDisabledVisualState()
    {
        var disabledColor = this.GeneralVisualState.Disabled;
        if (this.HasIcon)
        {
            this.icon.Foreground = disabledColor;
            this.GlyphAngle = this.glyphAngle;
        }

        if (this.HasText)
        {
            this.textBlock.Foreground = disabledColor;
        }


        if (this.HasBackgroundRectangle)
        {
            this.rectangleBackground.Fill = this.BackgroundVisualState.Disabled;
        }

        if (this.HasBackgroundBorder)
        {
            this.rectangleBackground.Stroke = this.BackgroundBorderVisualState.Disabled;
        }

        this.eventingRectangle.Fill = Brushes.Transparent; // this.isOver ? this.darkColor : Brushes.Transparent;
    }

    #endregion Visual States 

    #region Pointer Handling

    private void OnPointerEnter(object? sender, PointerEventArgs args)
    {
        if (this.IsSelected)
        {
            return;
        }

        if (this.eventingRectangle.IsPointerOver)
        {
            this.Enter();
        }
    }

    private void OnPointerLeave(object? sender, PointerEventArgs args)
    {
        if (this.IsSelected)
        {
            this.UpdateVisualState();
            return;
        }

        if (!this.eventingRectangle.IsPointerOver)
        {
            this.Leave();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        if (this.IsSelected)
        {
            return;
        }

        if (this.eventingRectangle.IsPointerInside(args))
        {
            this.Down();
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs args)
    {
        if (this.IsSelected)
        {
            return;
        }

        if (this.eventingRectangle.IsPointerInside(args))
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
        if (this.IsSelected || !this.isOver)
        {
            return; 
        }

        if (!this.eventingRectangle.IsPointerInside(args))
        {
            this.Leave();
        }
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
            return;
        }

        // Debug.WriteLine("Leave");
        this.isOver = false;
        this.isPressed = false;
        if ((this.Behaviour != ButtonBehaviour.Tap) || (this.Behaviour != ButtonBehaviour.PopupKeyboard))
        {
            this.StopTimer();
        }

        if (this.isLongPressActivated)
        {
            this.isLongPressActivated = false;
            this.IsDisabled = false;
        }

        if (this.Behaviour == ButtonBehaviour.Countdown)
        {
            this.ActivateCommand(new(), ButtonTag.CountdownCancel);
        }

        if (this.Behaviour == ButtonBehaviour.Continuous)
        {
            if (!this.isContinuousEnded)
            {
                this.ActivateCommand(new(), ButtonTag.CountinuousEnd);
            }
        }

        this.UpdateVisualState();
    }

    private void Down()
    {
        // Debug.WriteLine("Down");

        this.isPressed = true;
        if (this.isLongPressActivated)
        {
            this.isLongPressActivated = false;
            this.IsDisabled = false;
        }

        this.UpdateVisualState();
        if ((this.Behaviour != ButtonBehaviour.Tap) || (this.Behaviour != ButtonBehaviour.PopupKeyboard))
        {
            this.pressedAt = DateTime.Now;
            if (this.Behaviour == ButtonBehaviour.LongPress)
            {
                this.StartTimer(GlyphButton.LongPressTimerIntervalMilliseconds);
            }
            else if (this.Behaviour == ButtonBehaviour.Keyboard)
            {
                this.StartTimer(GlyphButton.KeyboardTimerIntervalMilliseconds);
            }
            else if (this.Behaviour == ButtonBehaviour.Countdown)
            {
                this.ActivateCommand(new(), ButtonTag.CountdownBegin);
                this.StartTimer(GlyphButton.CountdownTimerIntervalMilliseconds);
            }
            else if (this.Behaviour == ButtonBehaviour.Continuous)
            {
                this.ActivateCommand(new(), ButtonTag.CountinuousBegin);
                this.StartTimer(GlyphButton.ContinuousTimerIntervalMilliseconds);
            }
        }
    }

    private void Up(PointerReleasedEventArgs args)
    {
        // Debug.WriteLine("Up");

        this.isPressed = false;
        if (this.isLongPressActivated)
        {
            this.isLongPressActivated = false;
            this.IsDisabled = false;
        }

        this.UpdateVisualState();

        if (this.Behaviour == ButtonBehaviour.Tap)
        {
            this.ActivateCommand(args, ButtonTag.None);
        }
        #region Keyboards 

        //else if (this.Behaviour == ButtonBehaviour.PopupKeyboard)
        //{
        //    if (this.Tag is string tag)
        //    {
        //        if (this.IsShifted)
        //        {
        //            tag = tag.ToUpper();
        //        }

        //        this.ActivateCommand(new(), tag);
        //    }
        //    else
        //    {
        //        this.ActivateCommand(new());
        //    }

        //    if (this.ParentGlyphButton is not null)
        //    {
        //        this.ParentGlyphButton.DismissPopupKeyboard();
        //    }
        //}
        //else if (this.Behaviour == ButtonBehaviour.Keyboard)
        //{
        //    if (this.timer is not null)
        //    {
        //        // Timer is still running so this is just treated as a regular tap
        //        if (this.Tag is string tag)
        //        {
        //            if (this.IsShifted)
        //            {
        //                tag = tag.ToUpper();
        //            }

        //            this.ActivateCommand(new(), tag);
        //        }
        //        else
        //        {
        //            this.ActivateCommand(new());
        //        }

        //        this.StopTimer();
        //    }

        //    // If the timer is running this is just in case... If not we have to do that 
        //    this.DismissPopupKeyboard();
        //}
        #endregion Keyboards 
        //else if ((this.Behaviour == ButtonBehaviour.Countdown) && (this.timer is not null))
        //{
        //    this.ActivateCommand(new(), ButtonTag.CountdownCancel);
        //    this.StopTimer();
        //}
        else if (this.Behaviour == ButtonBehaviour.Continuous)
        {
            if (!this.isContinuousEnded) 
            {
                this.ActivateCommand(new(), ButtonTag.CountinuousEnd);
            }

            this.StopTimer();
        }
        else
        {
            this.StopTimer();
        }
    }

    #endregion Pointer Handling

    #region Timer

    private void StartTimer(int interval)
    {
        this.StopTimer();
        this.timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(interval),
            IsEnabled = true,
        };
        this.timer.Tick += this.OnTimerTick;
    }

    private void StopTimer()
    {
        if (this.timer is not null)
        {
            this.timer.IsEnabled = false;
            this.timer.Stop();
            this.timer.Tick -= this.OnTimerTick;
            this.timer = null;
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        double elapsed = (DateTime.Now - this.pressedAt).TotalMilliseconds;
        if ((this.Behaviour == ButtonBehaviour.LongPress) || (this.Behaviour == ButtonBehaviour.Countdown))
        {
            bool triggered =
                this.Behaviour == ButtonBehaviour.LongPress ?
                    (elapsed > GlyphButton.LongPressMilliseconds) :
                    (elapsed > this.Countdown);
            if (triggered)
            {
                this.StopTimer();
                if ( ! this.IsDisabled)
                {
                    ButtonTag tag =
                        this.Behaviour == ButtonBehaviour.LongPress ? ButtonTag.None : ButtonTag.CountdownComplete;
                    this.ActivateCommand(new(), tag);
                    this.IsDisabled = true;
                    this.isLongPressActivated = true;
                    this.UpdateVisualState();
                }
            }
        }
        else if (this.Behaviour == ButtonBehaviour.Keyboard)
        {
            //if (elapsed > GlyphButton.KeyboardPressMilliseconds)
            //{
            //    this.StopTimer();
            //    if (string.IsNullOrWhiteSpace(this.Keys))
            //    {
            //        this.ActivateCommand(new());
            //    }
            //    else
            //    {
            //        this.LaunchPopupKeyboard();
            //    }

            //    this.UpdateVisualState();
            //}
        }
        else if (this.Behaviour == ButtonBehaviour.Continuous)
        {
            this.ActivateCommand(new(), ButtonTag.CountinuousContinue);
        }
    }

    #endregion Timer

    #region Commanding 

    private void PreventMultipleClicks()
    {
        if (this.Behaviour != ButtonBehaviour.Tap)
        {
            // This should get activated only for the Tap behaviour 
            if (Debugger.IsAttached) { Debugger.Break(); }
            return;
        }

        this.IsEnabled = false;
        Task.Run(async () =>
            {
                await Task.Delay(250);
                Dispatcher.UIThread.Post((Action)delegate { this.IsEnabled = true; });
            });
    }

    private void ActivateCommand(RoutedEventArgs rea, ButtonTag buttonTag)
    {
        if (this.IsDisabled)
        {
            // This should never happen
            if (Debugger.IsAttached) { Debugger.Break(); }
            return;
        }

        bool activated = false; 
        // Give precedence to the Click handler if present 
        if (this.Click != null)
        {
            if (this.Behaviour == ButtonBehaviour.Tap)
            {
                this.PreventMultipleClicks();
            }

            this.Click.Invoke(this, rea);
            activated = true;
        }
        else if (this.Command != null)
        {
            // Pass along the button Tag if there is one for continuous and count down behaviours,
            // then the Button Tag if any, for keyboards and numeric pads 
            // and finally the command parameter which can be null if left unspecified 
            object? commandParameter =
                buttonTag != ButtonTag.None ?
                    buttonTag :
                    this.Tag is not null ? this.Tag : this.CommandParameter; 
            if (this.Command.CanExecute(commandParameter))
            {
                if (this.Behaviour == ButtonBehaviour.Tap)
                {
                    this.PreventMultipleClicks();
                }

                if ((this.Behaviour == ButtonBehaviour.Continuous) && (buttonTag == ButtonTag.CountinuousBegin))
                {
                    this.isContinuousEnded = false;
                }

                this.Command.Execute(commandParameter);
                activated = true;

                if ( (this.Behaviour == ButtonBehaviour.Continuous)&& ( buttonTag== ButtonTag.CountinuousEnd ) ) 
                {
                    this.isContinuousEnded = true;
                }
            }
        }

        if (activated && this.Group is not null)
        {
            this.isOver = false;
            this.Group.Select(this);
        }
    }

    #endregion Commanding 

    #region Popup Keyboards 

    // private PopupKeyboard? popupKeyboard;

    // public GlyphButton? ParentGlyphButton { get; set; }


    //private void LaunchPopupKeyboard()
    //{
    //    //this.popupKeyboard = new();
    //    //this.popupKeyboard.Create(this);
    //    //this.gridPopup.Children.Add(this.popupKeyboard);
    //    //this.gridPopup.Visibility = Visibility.Visible;
    //    //this.gridPopup.Width = this.popupKeyboard.Width;
    //    //this.gridPopup.Height = this.popupKeyboard.Height;
    //    //this.gridPopup.Margin =
    //    //    new Thickness(
    //    //        this.Width / 2.0 - this.popupKeyboard.Width / 2.0, -this.popupKeyboard.Height - 6.0,
    //    //        this.Width / 2.0 - this.popupKeyboard.Width / 2.0, -this.popupKeyboard.Height - 6.0);
    //}

    //private void DismissPopupKeyboard()
    //{
    //    //this.gridPopup.Visibility = Visibility.Collapsed;
    //    //this.gridPopup.Width = 0.0;
    //    //this.gridPopup.Height = 0.0;
    //    //if (this.popupKeyboard != null)
    //    //{
    //    //    this.gridPopup.Children.Remove(this.popupKeyboard);
    //    //    this.popupKeyboard = null;
    //    //}
    //}

    #endregion Popup Keyboards 
}
