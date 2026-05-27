using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace PushTEvolutionMvp
{
    public abstract class PushTBlockEvaluator : MonoBehaviour
    {
        public async Task<PushTGenomeEvaluation> EvaluateGenomeAsync(
            PushTBlockGenome genome,
            PushTEvolutionConfig config,
            int generationIndex,
            int genomeIndex,
            System.Random random)
        {
            var evaluation = new PushTGenomeEvaluation
            {
                generationIndex = generationIndex,
                genomeIndex = genomeIndex,
                genome = genome.Clone()
            };

            for (int episodeIndex = 0; episodeIndex < config.episodesPerGenome; episodeIndex++)
            {
                var episodeSeed = random.Next();
                var episode = await this.EvaluateEpisodeAsync(genome, episodeIndex, episodeSeed);
                evaluation.episodes.Add(episode);
            }

            evaluation.averageFitness = evaluation.episodes.Average(x => x.fitness);
            evaluation.successRate = evaluation.episodes.Count == 0
                ? 0f
                : evaluation.episodes.Count(x => x.success) / (float)evaluation.episodes.Count;

            evaluation.fitnessVariance = CalculateVariance(evaluation.episodes);
            return evaluation;
        }

        protected abstract Task<PushTEpisodeResult> EvaluateEpisodeAsync(
            PushTBlockGenome genome,
            int episodeIndex,
            int episodeSeed);

        private static float CalculateVariance(IReadOnlyList<PushTEpisodeResult> episodes)
        {
            if (episodes.Count == 0)
            {
                return 0f;
            }

            var mean = episodes.Average(x => x.fitness);
            var sumSquaredError = episodes.Sum(x =>
            {
                var delta = x.fitness - mean;
                return delta * delta;
            });

            return sumSquaredError / episodes.Count;
        }
    }
}
