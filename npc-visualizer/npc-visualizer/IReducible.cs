using System;

using Microsoft.SolverFoundation.Solvers;
using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    internal interface IReducible
    {
        Literal[][] ToSat();
        Tuple<Graph, int> ToClique();
        Tuple<Graph, int> ToColorability();
        Tuple<Graph, int> ToDominatingSet();
        Tuple<Graph, int> ToHamilCycle();
        Tuple<Graph, int> ToIndepSet();
        Tuple<Graph, int> ToVertexCover();
    }
}
