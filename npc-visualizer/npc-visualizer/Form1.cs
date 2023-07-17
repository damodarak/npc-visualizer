using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    public partial class Form1 : Form
    {
        Graph g;
        Microsoft.Msagl.GraphViewerGdi.GViewer viewer;
        int counter = 0;

        public Form1()
        {
            InitializeComponent();

            //create a viewer object 
            viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            viewer.EdgeInsertButtonVisible = false;
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.UndoRedoButtonsVisible = false;

            //create a graph object 
            g = new Graph("graph");
            //create the graph content 
            g.AddEdge("A", "B").Attr.ArrowheadAtTarget = ArrowStyle.None;
            g.AddEdge("B", "C").Attr.ArrowheadAtTarget = ArrowStyle.None;
            g.AddEdge("A", "C").Attr.ArrowheadAtTarget = ArrowStyle.None;

            int[] b = { 1, 2, 3 };
            int eCount = g.EdgeCount;
            IEnumerable<Edge> edges = g.Edges;

            foreach (var edge in edges)
            {
                Console.WriteLine($"{edge.Source} ---- {edge.Target}");
            }

            g.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            g.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;

            Node c = g.FindNode("C");
            c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            c.Attr.Shape = Shape.Diamond;
            //bind the graph to the viewer 
            viewer.Graph = g;
            //associate the viewer with the form 
            this.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(viewer);
            this.ResumeLayout();
        }
    }
}
