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
            this.sat = cliq.ToSat();
            this.solution = cliq.Solve();

            return this.solution;
        }
        public override Clique ToClique()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(G);
            return new Clique(flippedGraph, Param);
        }

        public override Colorability ToColorability()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToColorability();
        }

        public override DominatingSet ToDominatingSet()
        {
            VertexCover vertexCover = ToVertexCover();
            return vertexCover.ToDominatingSet();
        }

        public override HamilCycle ToHamilCycle()
        {
            VertexCover vertexCover = ToVertexCover();
            return vertexCover.ToHamilCycle();
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
