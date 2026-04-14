namespace ReplaySystem.Core
{
    public interface IRng
    {
        int Seed { get; }
        float Range(float min, float max);
        int Range(int min, int max);
        float Value { get; }
    }
}