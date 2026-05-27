using System;
using System.Collections.Generic;

namespace PushTEvolutionMvp
{
    [Serializable]
    public class PushTEpisodeResult
    {
        public int episodeIndex;
        public int episodeSeed;
        public float fitness;
        public bool success;
        public float completionTimeSeconds;
        public float stabilityPenalty;
        public string notes = "";
    }

    [Serializable]
    public class PushTGenomeEvaluation
    {
        public int generationIndex;
        public int genomeIndex;
        public PushTBlockGenome genome = new PushTBlockGenome();
        public float averageFitness;
        public float fitnessVariance;
        public float successRate;
        public List<PushTEpisodeResult> episodes = new List<PushTEpisodeResult>();
    }

    [Serializable]
    public class PushTEvolutionRunResult
    {
        public List<PushTGenomeEvaluation> bestPerGeneration = new List<PushTGenomeEvaluation>();
        public PushTGenomeEvaluation? bestOverall;
    }
}
