using System;
using UnityEngine;

namespace PushTEvolutionMvp
{
    public enum PushTShapeType
    {
        Box,
        Cylinder,
        Capsule
    }

    [Serializable]
    public class PushTBlockGenome
    {
        public PushTShapeType shapeType = PushTShapeType.Box;
        public float width = 1f;
        public float height = 1f;
        public float depth = 1f;
        public float mass = 1f;
        public float friction = 0.5f;
        public float bounciness = 0f;
        public Color color = Color.red;

        public PushTBlockGenome Clone()
        {
            return new PushTBlockGenome
            {
                shapeType = this.shapeType,
                width = this.width,
                height = this.height,
                depth = this.depth,
                mass = this.mass,
                friction = this.friction,
                bounciness = this.bounciness,
                color = this.color
            };
        }

        public static PushTBlockGenome CreateRandom(PushTEvolutionConfig config, System.Random random)
        {
            return new PushTBlockGenome
            {
                shapeType = (PushTShapeType)random.Next(0, Enum.GetValues(typeof(PushTShapeType)).Length),
                width = config.widthRange.RandomValue(random),
                height = config.heightRange.RandomValue(random),
                depth = config.depthRange.RandomValue(random),
                mass = config.massRange.RandomValue(random),
                friction = config.frictionRange.RandomValue(random),
                bounciness = config.bouncinessRange.RandomValue(random),
                color = new Color(
                    config.colorChannelRange.RandomValue(random),
                    config.colorChannelRange.RandomValue(random),
                    config.colorChannelRange.RandomValue(random),
                    1f)
            };
        }

        public void Mutate(PushTEvolutionConfig config, System.Random random)
        {
            if (ShouldMutate(config, random))
            {
                this.shapeType = (PushTShapeType)random.Next(0, Enum.GetValues(typeof(PushTShapeType)).Length);
            }

            this.width = MutateFloat(this.width, config.widthRange, config, random);
            this.height = MutateFloat(this.height, config.heightRange, config, random);
            this.depth = MutateFloat(this.depth, config.depthRange, config, random);
            this.mass = MutateFloat(this.mass, config.massRange, config, random);
            this.friction = MutateFloat(this.friction, config.frictionRange, config, random);
            this.bounciness = MutateFloat(this.bounciness, config.bouncinessRange, config, random);

            if (ShouldMutate(config, random))
            {
                this.color.r = config.colorChannelRange.Clamp(
                    this.color.r + RandomDelta(config.colorChannelRange, config, random));
            }

            if (ShouldMutate(config, random))
            {
                this.color.g = config.colorChannelRange.Clamp(
                    this.color.g + RandomDelta(config.colorChannelRange, config, random));
            }

            if (ShouldMutate(config, random))
            {
                this.color.b = config.colorChannelRange.Clamp(
                    this.color.b + RandomDelta(config.colorChannelRange, config, random));
            }
        }

        private static float MutateFloat(
            float value,
            FloatRange range,
            PushTEvolutionConfig config,
            System.Random random)
        {
            if (!ShouldMutate(config, random))
            {
                return value;
            }

            return range.Clamp(value + RandomDelta(range, config, random));
        }

        private static float RandomDelta(
            FloatRange range,
            PushTEvolutionConfig config,
            System.Random random)
        {
            var maxDelta = (range.max - range.min) * config.mutationStrength;
            var sample = (float)random.NextDouble() * 2f - 1f;
            return sample * maxDelta;
        }

        private static bool ShouldMutate(PushTEvolutionConfig config, System.Random random)
        {
            return random.NextDouble() < config.mutationRate;
        }
    }
}
