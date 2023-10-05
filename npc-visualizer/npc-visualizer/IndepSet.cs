using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    class IndepSet : Problem
    {
        public IndepSet(Graph g, int param)
        {
            this.g = g;
            this.param = param;
        }
        public override Literal[][] ToSat()
        {
            Graph flippedGraph = Utilities.FlipEdges(g);
            Clique cliq = new Clique(flippedGraph, param);
            return cliq.ToSat();
        }
        public override int[] Solve()
        {
            Graph flippedGraph = Utilities.FlipEdges(g);
            Clique cliq = new Clique(flippedGraph, param);
            solution = cliq.Solve();

            return solution;
        }
        public override Graph ToClique()
        {
            Graph flippedGraph = Utilities.FlipEdges(g);
            return flippedGraph;
        }

        public override Graph ToColorability()
        {
            throw new NotImplementedException();
        }

        public override Graph ToDominatingSet()
        {
            throw new NotImplementedException();
        }

        public override Graph ToHamilPath()
        {
            throw new NotImplementedException();
        }

        public override Graph ToIndepSet()
        {
            return g;
        }

        public override Graph ToVertexCover()
        {
            throw new NotImplementedException();
        }
    }
}
