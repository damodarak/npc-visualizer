using System;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    class IndepSet : GraphProblem
    {
        public IndepSet(Graph g, int param)
        {
            this.g = g;
            this.param = param;
        }
        public override Literal[][] ToSat()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(g);
            Clique cliq = new Clique(flippedGraph, param);
            return cliq.ToSat();
        }
        public override int[] Solve()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(g);
            Clique cliq = new Clique(flippedGraph, param);
            solution = cliq.Solve();

            return solution;
        }
        public override Tuple<Graph, int> ToClique()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(g);
            return new Tuple<Graph, int>(flippedGraph, param);
        }

        public override Tuple<Graph, int> ToColorability()
        {
            throw new NotImplementedException();
        }

        public override Tuple<Graph, int> ToDominatingSet()
        {
            throw new NotImplementedException();
        }

        public override Tuple<Graph, int> ToHamilCycle()
        {
            throw new NotImplementedException();
        }

        public override Tuple<Graph, int> ToIndepSet()
        {
            return new Tuple<Graph, int>(GraphUtilities.CopyGraph(g), param);
        }

        public override Tuple<Graph, int> ToVertexCover()
        {
            return new Tuple<Graph, int>(GraphUtilities.CopyGraph(g), g.NodeCount - param);
        }
    }
}
