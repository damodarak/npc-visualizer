using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace npc_visualizer
{
    class HamilPath : Problem
    {
        int[] satVarToIndex;
        public HamilPath(Graph g, int param)
        {
            this.g = g;
            this.param = param;
        }
        public override Literal[][] ToSat()
        {
            satVarToIndex = new int[g.NodeCount * g.NodeCount];
            indexToSatVar = new Dictionary<int, int>();

            CreateMapping();
            ClauseCount();

            sat = new Literal[clauseCount][];
            DefineClauses();

            return sat;
        }
        public override int[] Solve()
        {
            return null;
        }

        void CreateMapping()
        {

        }
        void ClauseCount()
        {

        }
        void DefineClauses()
        {

        }
        public override Graph ToClique()
        {
            throw new NotImplementedException();
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
            return g;
        }
        public override Graph ToIndepSet()
        {
            throw new NotImplementedException();
        }
        public override Graph ToVertexCover()
        {
            throw new NotImplementedException();
        }
    }
}
