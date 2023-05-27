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

namespace test_graph_drawing
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

        private void button1_Click(object sender, EventArgs e)
        {
            g.AddNode((counter++).ToString());
            viewer.Graph = g;
            this.SuspendLayout();
            this.panel1.Controls.Add(viewer);
            this.ResumeLayout();
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
                this.SuspendLayout();
                this.panel1.Controls.Add(viewer);
                this.ResumeLayout();
            }
        }
    }
}
