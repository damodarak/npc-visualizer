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
        protected Literal[][] sat; // CNF formula that represents the graph problem
        public int Param { get; protected set;}
        protected int clauseCount; // this.sat.Length
        protected int[] satVarToVertex; // If there is a solution then we can reconstruct the solution of the graph
        protected int[,] indexToSatVar; // [vertex, param]

        // Each class that inherits from this class needs to implement some basic contracts of IReducible plus some functions from this class
        public abstract Literal[][] ToSat();
        public abstract Clique ToClique();
        public abstract Colorability ToColorability();
        public abstract DominatingSet ToDominatingSet();
        public abstract IndepSet ToIndepSet();
        public abstract VertexCover ToVertexCover();
        public abstract HamilCycle ToHamilCycle();
        public abstract int[] Solve();

        // Function to display solution of graph problems where we are looking for a subset of the Vertices
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
