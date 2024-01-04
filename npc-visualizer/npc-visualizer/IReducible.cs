using System;

using Microsoft.SolverFoundation.Solvers;
using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    internal interface IReducible
    {
        Literal[][] ToSat();
        GraphProblem ToClique();
        GraphProblem ToColorability();
        GraphProblem ToDominatingSet();
        GraphProblem ToHamilCycle();
        GraphProblem ToIndepSet();
        GraphProblem ToVertexCover();
    }
}
