using System;
using System.Collections;
using System.Collections.Generic;

namespace Metaheuristics
{
	public abstract class DiscreteTS
	{
		public bool LocalSearchEnabled { get; protected set; }
		public int TabuListLength { get; protected set; }
		public int NeighborChecks { get; protected set; }

		public int[] BestSolution { get; protected set; }
		public double BestFitness { get; protected set; }

		public DiscreteTS (int tabuListLength, int neighborChecks)
		{
			LocalSearchEnabled = false;
			TabuListLength = tabuListLength;
			NeighborChecks = neighborChecks;			

			BestSolution = null;
			BestFitness = 0;
		}

		// Generate the initial solution.
		protected abstract int[] InitialSolution();

		// Evaluate a solution.
		protected abstract double Fitness(int[] solution);

		// Get a new solution in the neighborhood of the given solution.
		protected abstract int[] GetNeighbor(int[] solution);
		
		// Get the tabu for two solutions.
		protected abstract Tuple<int, int> GetTabu(int[] current, int[] neighbor);
		
		public void Run(int timeLimit)
		{	
			int startTime = Environment.TickCount;
			int iterationStartTime = 0;
			int iterationTime = 0;
			int maxIterationTime = 0;	
			int[] initialSolution = null;
			double initialFitness = 0;
			int[] currentSolution = null;
			double currentFitness = 0;
			int[] nextSolution = null;
			double nextFitness = 0;
			Tuple<int, int> nextTabu = null;
			int[] neighbor = null;
			double neighborFitness = 0;

			LimitedQueue<Tuple<int,int>> tabuList = new LimitedQueue<Tuple<int, int>>(TabuListLength);

			currentSolution = InitialSolution();
			initialSolution = new int[currentSolution.Length];
			currentSolution.CopyTo(initialSolution, 0);
			currentFitness = Fitness(currentSolution);

			BestFitness = currentFitness;
			BestSolution = currentSolution;

			while (Environment.TickCount - startTime < timeLimit - maxIterationTime) {
				iterationStartTime = Environment.TickCount;
				int count = 0;
				nextSolution = null;
				nextFitness = int.MaxValue;
				nextTabu = null;
				bool success = false;
				
				// Finding the next movement.
				Tuple<int, int> tabu = new Tuple<int, int>(-1, -1);
				Tuple<int, int> lastTabu = new Tuple<int, int>(-1, -1);

				while (count < NeighborChecks){
					count ++;
					neighbor = GetNeighbor(currentSolution);
					tabu = GetTabu(currentSolution, neighbor);
					if(!tabuList.Contains(tabu) && tabu != lastTabu) {
						neighborFitness = Fitness(neighbor);
						if (nextFitness > neighborFitness) {
							nextSolution = neighbor;
							nextFitness = neighborFitness;
							nextTabu = tabu;
							success = true;
						}
						if (currentFitness > nextFitness) {
							break;
						}
					}
				}
				if (!success) {
					nextSolution = initialSolution;
					nextFitness = initialFitness;
				}
				
				// Aspiration.
				if (BestFitness > nextFitness) {
					tabuList.Clear();
					BestSolution = nextSolution;
					BestFitness = nextFitness;
				}

				tabuList.Enqueue(nextTabu);
				currentSolution = nextSolution;
				currentFitness = nextFitness;
				
				iterationTime = Environment.TickCount - iterationStartTime;
				maxIterationTime = (maxIterationTime < iterationTime) ? iterationTime : maxIterationTime;				
			}
		}
	}
}
