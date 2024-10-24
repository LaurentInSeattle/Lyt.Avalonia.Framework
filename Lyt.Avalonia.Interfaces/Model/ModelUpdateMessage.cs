namespace Lyt.Avalonia.Interfaces.Model;

public sealed record class ModelUpdateMessage(
    IModel Model, string? PropertyName = "", string? MethodName = "") { }
