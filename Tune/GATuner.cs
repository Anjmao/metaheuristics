using System;
using System.Collections.Generic;

using Metaheuristics;

namespace Tune
{
	public class GATuner : Tuner
	{
		public GATuner(ITunableMetaheuristic metaheuristic, string dirInstances)
			: base(metaheuristic, dirInstances, 6, new int[] { 2000, 10000 }, 5)
		{
		}
		
		protected override IEnumerable<double[]> EnumerateParameters()
		{
			List<double> popFactors = new List<double>();
			for (int i = 5; i <= 51; i += 2) {
				popFactors.Add(i);
			}
			double[] mutProbabilities = new double[] { 0.1, 0.15, 0.2, 0.25, 0.3 };
			
			foreach (double popFactor in popFactors) {
				foreach (double mutProbability in mutProbabilities) {
					yield return new double[] { popFactor, mutProbability };
				}
			}
		}
	}
}
