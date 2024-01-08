using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    internal interface IReducible
    {
        // If we want to extend the list of the NPC problems in this program then we simple add another function to the interface
        Literal[][] ToSat();
        Clique ToClique();
        Colorability ToColorability();
        DominatingSet ToDominatingSet();
        HamilCycle ToHamilCycle();
        IndepSet ToIndepSet();
        VertexCover ToVertexCover();
    }
}
