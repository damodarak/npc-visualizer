using System;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    abstract class GraphProblem : IReducible
    {
        public Graph G {get; protected set;}
        public bool HasSolution { get { return solution != null; } }
        protected int[] solution = null;
        protected Literal[][] sat;
        public int Param { get; protected set;}
        protected int clauseCount;
        protected int[] satVarToVertex;
        protected int[,] indexToSatVar; // [vertex, param]

        public abstract Literal[][] ToSat();
        public abstract Clique ToClique();
        public abstract Colorability ToColorability();
        public abstract DominatingSet ToDominatingSet();
        public abstract IndepSet ToIndepSet();
        public abstract VertexCover ToVertexCover();
        public abstract HamilCycle ToHamilCycle();
        public abstract int[] Solve();
        public virtual void DrawSolution()
        {
            if (solution == null)
            {
                return;
            }

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
