using UnityEngine;
using ReplaySystem.Core;

namespace ReplaySystem.Recording
{
    public class FixedRng : IRng
    {
        private System.Random _random;
        public int Seed { get; private set; }

        public FixedRng(int seed)
        {
            Seed = seed;
            _random = new System.Random(seed);
        }

        public float Range(float min, float max)
        {
            return min + (float)_random.NextDouble() * (max - min);
        }

        public int Range(int min, int max)
        {
            return _random.Next(min, max);
        }

        public float Value => (float)_random.NextDouble();
    }
}