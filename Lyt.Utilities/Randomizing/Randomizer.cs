using Lyt.Avalonia.Interfaces.Random;

namespace Lyt.Utilities.Randomizing; 

public sealed class Randomizer : IRandomizer
{
    private readonly Random random;

    public Randomizer() => this.random = new Random(Environment.TickCount);       
    
    public float NextSingle() => this.random.NextSingle();

    public double NextDouble() => this.random.NextDouble();

    public int Next(int min, int max) => this.random.Next(min, max);

    public int Next(int max) => this.random.Next(max);

    public bool NextBool() => this.random.NextSingle() > 0.5f;

    public double NextDouble(double min, double max) => min + this.random.NextDouble() * (max - min); 

    /// <summary>Shuffles the specified list using the Fisher Yates algorithm.</summary>
    public void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = this.random.Next(0, 1 + i);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }
}
