﻿using System;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    abstract class GraphProblem : IReducible
    {
        protected Graph g;
        protected int[] solution;
        protected Literal[][] sat;
        protected int param;
        protected int clauseCount;
        protected int[] satVarToVertex;
        protected int[,] indexToSatVar; // [vertex, param]

        public abstract Literal[][] ToSat();
        public abstract Tuple<Graph, int> ToClique();
        public abstract Tuple<Graph, int> ToColorability();
        public abstract Tuple<Graph, int> ToDominatingSet();
        public abstract Tuple<Graph, int> ToIndepSet();
        public abstract Tuple<Graph, int> ToVertexCover();
        public abstract Tuple<Graph, int> ToHamilCycle();
        public abstract int[] Solve();
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