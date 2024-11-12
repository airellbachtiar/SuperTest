using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using Resources.Solver;

namespace TrafficSim.Generated
{
    public partial class PandISimulator
    {
        private readonly DecomposingSolver _solver = new();


        private void InitComponents()
        {
            CarGreen.Resistance = 1;
            CarYellow.Resistance = 1;
            CarRed.Resistance = 1;

            PedGreen.Resistance = 1;
            PedRed.Resistance = 1;

        }

        private double[] SolveEquations(DenseMatrix matrix, DenseVector rhs, List<(int row, int col)> potentialNonZero)
        {
            return _solver.Solve(matrix, rhs, potentialNonZero);
        }

        private void CalculationStepFinished()
        {
        }
    }
}
