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
            init_graph_layout();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.AddNode((counter++).ToString()).Attr.Shape = Shape.Circle;
            viewer.Graph = g;
        }

        private void init_graph_layout()
        {
            //create a viewer object 
            viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            viewer.EdgeInsertButtonVisible = false;
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.UndoRedoButtonsVisible = false;

            //create a graph object 
            g = new Graph("graph");

            g.Directed = false;

            //create the graph content 
            Edge e = g.AddEdge("-1", "-2");
            e.Attr.ArrowheadAtTarget = ArrowStyle.None;
            e.Attr.Id = "-1_-2";

            e = g.AddEdge("-2", "-3");   
            e.Attr.ArrowheadAtTarget = ArrowStyle.None;
            e.Attr.Id = "-2_-3";

            e = g.AddEdge("-1", "-3");
            e.Attr.ArrowheadAtTarget = ArrowStyle.None;
            e.Attr.Id = "-1_-3";

            //IEnumerable<Edge> edges = g.Edges;

            //foreach (var edge in edges)
            //{
            //    Console.WriteLine($"{edge.Source} ---- {edge.Target}");
            //}

            //change vertex color
            //g.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            //g.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;

            Node c = g.FindNode("-3");
            //c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            c.Attr.Shape = Shape.Circle;
            g.FindNode("-1").Attr.Shape = Shape.Circle;
            g.FindNode("-2").Attr.Shape = Shape.Circle;

            //bind the graph to the viewer 
            viewer.Graph = g;
            //associate the viewer with the form 
            this.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(viewer);
            this.ResumeLayout();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string t1 = textBox1.Text;
            string t2 = textBox2.Text;

            Node n1 = g.FindNode(t1);
            Node n2 = g.FindNode(t2);

            Edge e1 = EdgeById(t1 + "_" + t2);
            Edge e2 = EdgeById(t2 + "_" + t1);

            if (n1 != null && n2 != null && e1 == null && e2 == null)
            {
                Edge new_e = g.AddEdge(t1, t2);
                new_e.Attr.ArrowheadAtTarget = ArrowStyle.None;
                new_e.Attr.Id = t1 + "_" + t2;

                viewer.Graph = g;
            }
        }

        private Edge EdgeById(string id)
        {
            IEnumerable<Edge> edges = g.Edges;

            string[] vertices = id.Split('_');

            foreach (Edge edge in edges)
            {
                if((edge.Source == vertices[0] && edge.Target == vertices[1]) || 
                    (edge.Target == vertices[0] && edge.Source == vertices[1]))
                {
                    return edge;
                }
            }

            return null;
        }
    }
}
