namespace Lyt.Avalonia.Interfaces.Profiler;

public interface IProfiler
{
    Task FullGcCollect(int delay = 0);

    int[] CollectionCounts();

    void MemorySnapshot(string comment = "", bool withGCCollect = true); 
}
