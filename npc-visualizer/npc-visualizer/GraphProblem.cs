using System;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    abstract class GraphProblem : IReducible
    {
        public Graph G {get; protected set;}
        protected int[] solution;
        protected Literal[][] sat;
        public int Param { get; protected set;}
        protected int clauseCount;
        protected int[] satVarToVertex;
        protected int[,] indexToSatVar; // [vertex, param]

        public abstract Literal[][] ToSat();
        public abstract GraphProblem ToClique();
        public abstract GraphProblem ToColorability();
        public abstract GraphProblem ToDominatingSet();
        public abstract GraphProblem ToIndepSet();
        public abstract GraphProblem ToVertexCover();
        public abstract GraphProblem ToHamilCycle();
        public abstract int[] Solve();
        public virtual void DrawSolution()
        {
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] != -1)
                {
                    G.FindNode(solution[i].ToString()).Attr.FillColor = Color.Purple;
                }
            }
        }
    }
}
