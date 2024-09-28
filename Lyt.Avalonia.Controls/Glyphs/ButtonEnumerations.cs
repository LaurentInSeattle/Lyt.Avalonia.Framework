namespace Lyt.Avalonia.Controls.Glyphs;

//  Tap.            Touch-down followed by touch-up inside. Applicable to most controls.

//  Long Press.     After control is held down for X seconds, activate the function without requiring the user to
//                  release the control (control would ‘auto-release’ on function change of state.
//                  Should be associated with a visual timer to show user how long the control needs to be held for.
//                  For functions that require adding some workflow friction to mitigate unintended events,
//                  like deleting user, unlocking touchpad.

//  Continuous.     Activate associated function while control is being pressed (on touch-down) and stop activation
//                  on touch-up. For functions that need continuous confirmation as a safety mitigation

// WhilePressing    if we decide to show sometime like a tooltips. I.e, we show a screen element, like a help text,
//                  while the user is pressing an icon.
//                  I currently have one of these on the boot screen (an (i) icon) to show SW version number and
//                  system ID(although I may get rid of that extra action and just show the detail onscreen
//                  because hiding it doesn’t really make sense to me).
//                  So there are 4. I’ll define all these and make sure they get documented in some sort of spec.

// Keyboard         Keyboard behavior allowing entry of diacritics by showing addition additional buttons after a medium 
//                  (not too long) press.                

public enum ButtonBehaviour
{
    Tap,            // Standard Touch Up or Mouse Up inside 
    LongPress,      // Long Touch Down or Mouse Up Down inside 
    Countdown,      // Similar to LongPress but the delay is longer and paratrizable and there is (usually) some visual feedback 
    Continuous,     // Command gets invoked repeatedly as long the control is maintained pressed.
    Keyboard,       // Medium timeout while pressed triggers an additional action, cancelled on touch up
                    // Triggers the appearance of additional popup keyboard
    PopupKeyboard,  // Behaviour of the buttons of the popup keyboard

    // this one below is not implemented yet, but seems to be a simplification of Continuous 
    WhilePressing,  
}

public enum ButtonTag
{
    None,

    CountdownBegin,
    CountdownCancel,
    CountdownComplete,

    CountinuousBegin,
    CountinuousContinue,
    CountinuousEnd,
}

public enum ButtonLayout
{
    IconOnly , 
    IconTextBelow, 
    IconTextRightSide,
    TextOnly,
}

public enum ButtonBackground
{
    None,
    BorderOnly,
    Rectangle,
    BorderlessRectangle,
}