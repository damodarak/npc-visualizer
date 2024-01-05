using System;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    class IndepSet : GraphProblem
    {
        public IndepSet(Graph g, int param)
        {
            this.G = g;
            this.Param = param;
        }
        public override Literal[][] ToSat()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(G);
            Clique cliq = new Clique(flippedGraph, Param);
            return cliq.ToSat();
        }
        public override int[] Solve()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(G);
            Clique cliq = new Clique(flippedGraph, Param);
            solution = cliq.Solve();

            return solution;
        }
        public override Clique ToClique()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(G);
            return new Clique(flippedGraph, Param);
        }

        public override Colorability ToColorability()
        {
            throw new NotImplementedException();
        }

        public override DominatingSet ToDominatingSet()
        {
            throw new NotImplementedException();
        }

        public override HamilCycle ToHamilCycle()
        {
            throw new NotImplementedException();
        }

        public override IndepSet ToIndepSet()
        {
            return new IndepSet(GraphUtilities.CopyGraph(G), Param);
        }

        public override VertexCover ToVertexCover()
        {
            return new VertexCover(GraphUtilities.CopyGraph(G), G.NodeCount - Param);
        }
    }
}
