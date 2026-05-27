using System;
using UnityEngine;

namespace PushTEvolutionMvp
{
    [Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, this.min, this.max);
        }

        public float RandomValue(System.Random random)
        {
            return this.min + ((float)random.NextDouble() * (this.max - this.min));
        }
    }

    [Serializable]
    public class PushTEvolutionConfig
    {
        [Header("Search")]
        public int populationSize = 24;
        public int eliteCount = 6;
        public int generationCount = 20;
        public int episodesPerGenome = 5;
        public int randomSeed = 12345;

        [Header("Mutation")]
        [Range(0f, 1f)] public float mutationRate = 0.35f;
        [Range(0f, 1f)] public float mutationStrength = 0.15f;

        [Header("Genome Ranges")]
        public FloatRange widthRange = new FloatRange { min = 0.2f, max = 2.0f };
        public FloatRange heightRange = new FloatRange { min = 0.2f, max = 2.0f };
        public FloatRange depthRange = new FloatRange { min = 0.2f, max = 2.0f };
        public FloatRange massRange = new FloatRange { min = 0.1f, max = 10f };
        public FloatRange frictionRange = new FloatRange { min = 0f, max = 1f };
        public FloatRange bouncinessRange = new FloatRange { min = 0f, max = 1f };
        public FloatRange colorChannelRange = new FloatRange { min = 0f, max = 1f };
    }
}
