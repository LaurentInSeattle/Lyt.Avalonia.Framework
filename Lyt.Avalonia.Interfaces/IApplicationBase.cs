using Lyt.Avalonia.Interfaces.Model;

namespace Lyt.Avalonia.Interfaces;

public interface IApplicationBase
{
    IEnumerable<IModel> GetModels();
    
    Task Shutdown ();
}

