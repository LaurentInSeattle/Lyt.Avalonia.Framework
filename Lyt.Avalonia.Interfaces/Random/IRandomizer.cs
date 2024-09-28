namespace Lyt.Avalonia.Interfaces.Random; 

public interface IRandomizer
{
    int Next(int min, int max);

    int Next(int max);

    bool NextBool();

    float NextSingle ();

    double NextDouble(double min, double max);

    double NextDouble();

    void Shuffle<T> (IList<T> list);
}
