using System.Threading;

namespace Lyt.Avalonia.Mvvm.Animations;

public sealed class AnimationService : IAnimationService
{
#pragma warning disable CA2211 // Non-constant fields should not be visible
    // Cannot be made const to use as a default parameter.
    public static IterationCount OneIteration = IterationCount.Parse("1");
#pragma warning restore CA2211 

    private Dictionary<Control, CancellationTokenSource>? fadeOutAnimations; 

    public void FadeIn(Control control, double durationSeconds = 1.5, Action? onAnimationCompleted = null)
    {
        // Cancel FadeOut if any ongoing 
        if (this.fadeOutAnimations is not null)
        {
            if (this.fadeOutAnimations.TryGetValue(control, out CancellationTokenSource? cancellationTokenSource))
            {
                if (cancellationTokenSource is not null) // && !cancellationTokenSource.IsCancellationRequested) 
                {
                    Debug.WriteLine("Cancelling Fade Out"); 
                    cancellationTokenSource.Cancel();
                    Debug.WriteLine("Fade Out entry removed");
                    this.fadeOutAnimations.Remove(control);
                }
            }
        }

        Debug.WriteLine("Fade IN begins");
        this.StartAnimation(
            control, Control.OpacityProperty, 1.0, durationSeconds,
            OneIteration, PlaybackDirection.Normal, FillMode.Forward, onAnimationCompleted);
    }

    public void FadeOut(Control control, double durationSeconds = 1.5, Action? onAnimationCompleted = null)
    {
        Debug.WriteLine("Fade OUT begins");
        CancellationTokenSource cancellationTokenSource = 
            this.StartAnimation(control, Control.OpacityProperty, 0.0, durationSeconds,
                OneIteration, PlaybackDirection.Normal, FillMode.Forward, 
                ()=>
                {
                    Debug.WriteLine("Fade Out Complete");
                    if (this.fadeOutAnimations is not null)
                    {
                        if (this.fadeOutAnimations.TryGetValue(control, out CancellationTokenSource? cancellationTokenSource))
                        {
                            Debug.WriteLine("Fade Out entry removed");
                            this.fadeOutAnimations.Remove(control);
                        }
                    }

                    Dispatch.OnUiThread(() => { onAnimationCompleted?.Invoke(); }, DispatcherPriority.Send);
                });

        // Save the CancellationTokenSource should we fade in soon 
        if (this.fadeOutAnimations is null)
        {
            this.fadeOutAnimations = [];
        }

        if (this.fadeOutAnimations.ContainsKey(control))
        {
            Debug.WriteLine("Fade Out entry removed");
            this.fadeOutAnimations.Remove(control);
        }

        this.fadeOutAnimations.Add(control, cancellationTokenSource); 
    } 

    public void CancelAnimation(CancellationTokenSource cancellationTokenSource)
        => cancellationTokenSource.Cancel();

    public CancellationTokenSource StartAnimation(
        Control control,
        AvaloniaProperty avaloniaProperty,
        double toValue,
        double animationDurationSeconds,
        IterationCount iterationCount,
        PlaybackDirection playbackDirection = PlaybackDirection.Normal,
        FillMode fillMode = FillMode.Forward,
        Action? onAnimationCompleted = null)
    {
        object? propertyValue = control.GetValue(avaloniaProperty);
        if (propertyValue is not double fromValue)
        {
            throw new NotSupportedException("Animate only double properties");
        }

        CancellationTokenSource cancellationTokenSource = new();
        CancellationToken cancellationToken = cancellationTokenSource.Token;

        Animation animation = new()
        {
            Duration = TimeSpan.FromSeconds(animationDurationSeconds),
            IterationCount = iterationCount,
            PlaybackDirection = playbackDirection,
            FillMode = fillMode,
            Children =
                {
                    new KeyFrame
                    {
                        Cue= new Cue(0.0),
                        Setters = { new Setter(avaloniaProperty, fromValue) }
                    },
                    new KeyFrame
                    {
                        Cue= new Cue(1.0),
                        Setters = { new Setter(avaloniaProperty, toValue) }
                    }
                }
        };

        var animationTask = animation.RunAsync(control, cancellationToken);
        animationTask.ContinueWith(task =>
        {
            if (task.IsCompleted && onAnimationCompleted is not null)
            {
                // Invoke completion delegate if present,
                // Important => Most likely, this is NOT running on the UI thread, so: dispatch 
                Dispatch.OnUiThread(() => { onAnimationCompleted.Invoke(); }, DispatcherPriority.Send);
            }

            // Do not leak the cancellation Token Source, always dispose it.
            cancellationTokenSource.Dispose();
        });

        return cancellationTokenSource;
    }
}
