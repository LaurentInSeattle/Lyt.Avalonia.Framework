namespace Lyt.Avalonia.Interfaces.Model;

public sealed class ModelUpdateMessage(IModel model, string? propertyName = "", string? methodName = "")
{
    public IModel Model { get; private set; } = model;

    public string? PropertyName { get; private set; } = propertyName;

    public string? MethodName { get; private set; } = methodName;
}
