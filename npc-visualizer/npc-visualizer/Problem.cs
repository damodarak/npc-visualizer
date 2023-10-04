using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    abstract class Problem
    {
        protected Graph g;
        protected int[] solution;
        protected Literal[][] sat;
        protected int param;
        protected int clauseCount;
        protected int[] satVarToVertex;
        protected Dictionary<int, int> indexToSatVar;

        protected abstract void ToSat();
        public abstract Graph ToClique();
        public abstract Graph ToColorability();
        public abstract Graph ToDominatingSet();
        public abstract Graph ToIndepSet();
        public abstract Graph ToVertexCover();
        public abstract Graph ToHamilPath();
        public abstract void Solve();
        protected abstract void DefineClauses();
        public virtual void DrawSolution()
        {
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] != -1)
                {
                    g.FindNode(solution[i].ToString()).Attr.FillColor = Color.Purple;
                }
            }
        }    
    }
}
