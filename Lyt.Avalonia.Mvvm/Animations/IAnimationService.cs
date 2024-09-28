namespace Lyt.Avalonia.Mvvm.Interfaces.Animations;

public interface IAnimationService
{
    void FadeIn(Control control, double durationSeconds = 1.5, Action? onAnimationCompleted = null);

    void FadeOut(Control control, double durationSeconds = 1.5, Action? onAnimationCompleted = null);

    CancellationTokenSource StartAnimation(
        Control animatableElement,
        AvaloniaProperty dependencyProperty,
        double toValue,
        double animationDurationSeconds,
        IterationCount iterationCount,
        PlaybackDirection playbackDirection = PlaybackDirection.Normal,
        FillMode fillMode = FillMode.Forward,
        Action? onAnimationCompleted = null);

    void CancelAnimation(CancellationTokenSource cancellationTokenSource);
}
