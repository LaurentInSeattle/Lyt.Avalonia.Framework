namespace Lyt.Avalonia.Interfaces.Localization;

public sealed record class LanguageChangedMessage(string? OldLanguageKey, string NewLanguageKey);
