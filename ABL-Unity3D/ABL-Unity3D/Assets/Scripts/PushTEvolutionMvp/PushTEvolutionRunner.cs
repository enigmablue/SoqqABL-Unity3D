using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace PushTEvolutionMvp
{
    public class PushTEvolutionRunner : MonoBehaviour
    {
        public PushTEvolutionConfig config = new PushTEvolutionConfig();
        public PushTBlockEvaluator? evaluator;

        [TextArea] public string status = "Idle";
        public PushTEvolutionRunResult lastRun = new PushTEvolutionRunResult();

        [ContextMenu("Run Evolution")]
        public async void RunEvolution()
        {
            if (this.evaluator == null)
            {
                Debug.LogError("No PushTBlockEvaluator assigned.");
                return;
            }

            this.lastRun = await this.RunEvolutionAsync(this.evaluator);
        }

        public async Task<PushTEvolutionRunResult> RunEvolutionAsync(PushTBlockEvaluator assignedEvaluator)
        {
            ValidateConfig(this.config);

            var random = new System.Random(this.config.randomSeed);
            var population = CreateInitialPopulation(this.config, random);
            var runResult = new PushTEvolutionRunResult();

            for (int generationIndex = 0; generationIndex < this.config.generationCount; generationIndex++)
            {
                this.status = $"Evaluating generation {generationIndex + 1}/{this.config.generationCount}";

                var evaluations = await EvaluatePopulationAsync(
                    population,
                    assignedEvaluator,
                    this.config,
                    generationIndex,
                    random);

                evaluations.Sort((a, b) => b.averageFitness.CompareTo(a.averageFitness));
                var bestThisGeneration = evaluations[0];
                runResult.bestPerGeneration.Add(bestThisGeneration);

                if (runResult.bestOverall == null ||
                    bestThisGeneration.averageFitness > runResult.bestOverall.averageFitness)
                {
                    runResult.bestOverall = bestThisGeneration;
                }

                population = CreateNextPopulation(evaluations, this.config, random);

                Debug.Log(
                    $"Generation {generationIndex} best fitness: {bestThisGeneration.averageFitness:F3}, " +
                    $"success rate: {bestThisGeneration.successRate:P0}");
            }

            this.status = runResult.bestOverall == null
                ? "Finished with no result."
                : $"Finished. Best fitness: {runResult.bestOverall.averageFitness:F3}";

            return runResult;
        }

        private static List<PushTBlockGenome> CreateInitialPopulation(
            PushTEvolutionConfig config,
            System.Random random)
        {
            var population = new List<PushTBlockGenome>(config.populationSize);
            for (int i = 0; i < config.populationSize; i++)
            {
                population.Add(PushTBlockGenome.CreateRandom(config, random));
            }

            return population;
        }

        private static async Task<List<PushTGenomeEvaluation>> EvaluatePopulationAsync(
            IReadOnlyList<PushTBlockGenome> population,
            PushTBlockEvaluator evaluator,
            PushTEvolutionConfig config,
            int generationIndex,
            System.Random random)
        {
            var evaluations = new List<PushTGenomeEvaluation>(population.Count);
            for (int genomeIndex = 0; genomeIndex < population.Count; genomeIndex++)
            {
                var evaluation = await evaluator.EvaluateGenomeAsync(
                    population[genomeIndex],
                    config,
                    generationIndex,
                    genomeIndex,
                    random);
                evaluations.Add(evaluation);
            }

            return evaluations;
        }

        private static List<PushTBlockGenome> CreateNextPopulation(
            IReadOnlyList<PushTGenomeEvaluation> evaluations,
            PushTEvolutionConfig config,
            System.Random random)
        {
            var eliteCount = Mathf.Min(config.eliteCount, evaluations.Count);
            var elites = evaluations
                .Take(eliteCount)
                .Select(x => x.genome.Clone())
                .ToList();

            var nextPopulation = new List<PushTBlockGenome>(config.populationSize);

            foreach (var elite in elites)
            {
                nextPopulation.Add(elite.Clone());
            }

            while (nextPopulation.Count < config.populationSize)
            {
                var parent = elites[random.Next(elites.Count)].Clone();
                parent.Mutate(config, random);
                nextPopulation.Add(parent);
            }

            return nextPopulation;
        }

        private static void ValidateConfig(PushTEvolutionConfig config)
        {
            if (config.populationSize < 2)
            {
                throw new ArgumentException("Population size must be at least 2.");
            }

            if (config.eliteCount < 1 || config.eliteCount > config.populationSize)
            {
                throw new ArgumentException("Elite count must be between 1 and population size.");
            }

            if (config.generationCount < 1)
            {
                throw new ArgumentException("Generation count must be at least 1.");
            }

            if (config.episodesPerGenome < 1)
            {
                throw new ArgumentException("Episodes per genome must be at least 1.");
            }
        }
    }
}
