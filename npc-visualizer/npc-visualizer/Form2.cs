using System;
using System.Windows.Forms;

using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            viewer.OutsideAreaBrush = System.Drawing.Brushes.White;
            viewer.UndoRedoButtonsVisible = false;
            viewer.EdgeInsertButtonVisible = false;
            viewer.SaveAsMsaglEnabled = false;
            viewer.SaveInVectorFormatEnabled = false;
            viewer.SaveAsImageEnabled = false;
            viewer.NavigationVisible = false;
            viewer.AllowDrop = false;
            viewer.InsertingEdge = false;
            viewer.SaveButtonVisible = false;

            Graph map = new Graph();

            map.AddEdge("Sat", "3-Sat");
            map.AddEdge("Clique", "Sat");
            map.AddEdge("Independent Set", "Sat");
            map.AddEdge("Vertex Cover", "Sat");
            map.AddEdge("Dominating Set", "Sat");
            map.AddEdge("Coloring", "Sat");
            map.AddEdge("Hamiltonian Cycle", "Sat");

            map.AddEdge("Vertex Cover", "Dominating Set");
            map.AddEdge("Vertex Cover", "Hamiltonian Cycle");

            map.AddEdge("Independent Set", "Clique");
            map.AddEdge("Clique", "Independent Set");

            map.AddEdge("Independent Set", "Vertex Cover");
            map.AddEdge("Vertex Cover", "Independent Set");

            map.AddEdge("3-Sat", "Independent Set");
            map.AddEdge("3-Sat", "Coloring");

            viewer.Graph = map;
            viewer.Dock = DockStyle.Fill;
            this.Controls.Add(viewer);
        }
    }
}
