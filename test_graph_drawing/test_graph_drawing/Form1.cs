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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace test_graph_drawing
{
    public partial class Form1 : Form
    {
        Graph g;
        Microsoft.Msagl.GraphViewerGdi.GViewer viewer;
        int counter = 0;
        string firstNodeClicked = "";

        public Form1()
        {
            InitializeComponent();

            //create a viewer object 
            viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            viewer.EdgeInsertButtonVisible = false;
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.UndoRedoButtonsVisible = false;
            viewer.OutsideAreaBrush = System.Drawing.Brushes.White;
            viewer.ToolBarIsVisible = false;

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
            this.panel1.Controls.Add(viewer);
            viewer.Dock = DockStyle.Fill;

            // Handle double-click event on the viewer
            viewer.MouseDoubleClick += Viewer_MouseDoubleClick;

            // handel single-click for adding edges
            viewer.MouseUp += Viewer_MouseUp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.AddNode((counter++).ToString());
            viewer.Graph = g;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string t1 = textBox1.Text;
            string t2 = textBox2.Text;

            Node n1 = g.FindNode(t1);
            Node n2 = g.FindNode(t2);

            if(n1 != null && n2 != null)
            {
                g.AddEdge(t1, t2).Attr.ArrowheadAtTarget = ArrowStyle.None;
                viewer.Graph = g;
            }
        }

        private void Viewer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string nodeName = g.NodeCount.ToString();
            g.AddNode(nodeName).Attr.Shape = Shape.Circle;
            viewer.Graph = g;
        }

        private void Viewer_MouseUp(object sender, MouseEventArgs e)
        {
            var gviewer = (Microsoft.Msagl.GraphViewerGdi.GViewer)sender;
            var dnode = gviewer.ObjectUnderMouseCursor as Microsoft.Msagl.GraphViewerGdi.DNode;
            //var dnode = gviewer.SelectedObject as Microsoft.Msagl.GraphViewerGdi.DNode;
            if (dnode == null)
            {
                firstNodeClicked = "";
                return;
            }
            if (firstNodeClicked == "")
            {
                firstNodeClicked = dnode.Node.LabelText;
            }
            else
            {
                g.AddEdge(firstNodeClicked, dnode.Node.LabelText).Attr.ArrowheadAtTarget = ArrowStyle.None;
                firstNodeClicked = "";
                viewer.Graph = g;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Graph new_g = new Graph();
            foreach (var nod in g.Nodes)
            {
                new_g.AddNode(nod.Label.Text).Attr.Shape = Shape.Circle;
            }

            g = new_g;

            viewer.Graph = new_g;
        }
    }
}
