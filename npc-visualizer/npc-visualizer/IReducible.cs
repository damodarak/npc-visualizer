using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    internal interface IReducible
    {
        Literal[][] ToSat();
        Clique ToClique();
        Colorability ToColorability();
        DominatingSet ToDominatingSet();
        HamilCycle ToHamilCycle();
        IndepSet ToIndepSet();
        VertexCover ToVertexCover();
    }
}
