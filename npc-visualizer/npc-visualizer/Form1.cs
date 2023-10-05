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
        int counter = 3;
        int[] solution;

        public Form1()
        {
            InitializeComponent();
            InitGraphLayout();
            this.edgeNode1.Maximum = counter - 1;
            this.edgeNode2.Maximum = counter - 1;
        }

        private void InitGraphLayout()
        {
            //create a viewer object 
            viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            viewer.EdgeInsertButtonVisible = false;
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.UndoRedoButtonsVisible = false;
            viewer.SaveAsMsaglEnabled = false;
            viewer.AllowDrop = false;
            //viewer.AutoValidate ?
            viewer.InsertingEdge = false;

            //create a graph object 
            g = new Graph("graph");

            g.Directed = false;

            //create the graph content 
            Edge e = g.AddEdge("0", "1");
            e.Attr.ArrowheadAtTarget = ArrowStyle.None;
            e.Attr.Id = "0_1";

            e = g.AddEdge("1", "2");   
            e.Attr.ArrowheadAtTarget = ArrowStyle.None;
            e.Attr.Id = "1_2";

            e = g.AddEdge("0", "2");
            e.Attr.ArrowheadAtTarget = ArrowStyle.None;
            e.Attr.Id = "0_2";

            //change vertex color
            //g.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
            //g.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.MistyRose;

            Node c = g.FindNode("2");
            //c.Attr.FillColor = Microsoft.Msagl.Drawing.Color.PaleGreen;
            c.Attr.Shape = Shape.Circle;
            g.FindNode("0").Attr.Shape = Shape.Circle;
            g.FindNode("1").Attr.Shape = Shape.Circle;

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
            if (counter == 20)
            {
                return;
            }
            g.AddNode((counter++).ToString()).Attr.Shape = Shape.Circle;
            Utilities.ClearVertexColor(g);
            viewer.Graph = g;

            this.edgeNode1.Maximum = counter - 1;
            this.edgeNode2.Maximum = counter - 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string vertex1 = edgeNode1.Text;
            string vertex2 = edgeNode2.Text;

            Node n1 = g.FindNode(vertex1);
            Node n2 = g.FindNode(vertex2);

            Edge e1 = Utilities.EdgeById(g, vertex1 + "_" + vertex2);
            Edge e2 = Utilities.EdgeById(g, vertex2 + "_" + vertex1);

            if (n1 != null && n2 != null && e1 == null && e2 == null && n1.Id != n2.Id)
            {
                int index1 = int.Parse(vertex1);
                int index2 = int.Parse(vertex2);

                //edge is always from vertex with smaller index to vertex with higher index
                if (index1 > index2)
                {
                    string temp = vertex1;
                    vertex1 = vertex2;
                    vertex2 = temp;
                }

                Edge new_e = g.AddEdge(vertex1, vertex2);
                new_e.Attr.ArrowheadAtTarget = ArrowStyle.None;
                new_e.Attr.Id = vertex1 + "_" + vertex2;

                Utilities.ClearVertexColor(g);
                viewer.Graph = g;
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (g.NodeCount == 0)
            {
                return;
            }

            int index = comboBox1.SelectedIndex;
            int param = (int)numericUpDown1.Value;

            Problem problem;
            Utilities.ClearVertexColor(g);
            switch (index)
            {
                case 0:
                    problem = new Clique(g, param);
                    problem.Solve();
                    problem.DrawSolution();
                    break;
                case 1:
                    problem = new IndepSet(g, param);
                    problem.Solve();
                    problem.DrawSolution();
                    break;
                case 2:
                    problem = new VertexCover(g, param);
                    problem.Solve();
                    problem.DrawSolution();
                    break;
                case 3:
                    problem = new DominatingSet(g, param);
                    problem.Solve();
                    problem.DrawSolution();
                    break;
                case 4:
                    problem = new Colorability(g, param);
                    problem.Solve();
                    problem.DrawSolution();
                    break;
                case 5:
                    problem = new HamilPath(g, param);
                    problem.Solve();
                    problem.DrawSolution();
                    break;
                default:
                    break;
            }
            
            viewer.Graph = g;
        }

        private void button4_Click(object sender, EventArgs e)
        {       
            while (g.EdgeCount > 0)
            {
                IEnumerable<Edge> edges = g.Edges;
                foreach (var ed in edges)
                {
                    g.RemoveEdge(ed);
                    break;
                }
            }

            while (g.NodeCount > 0)
            {
                IEnumerable<Node> nodes = g.Nodes;
                foreach (var nod in nodes)
                {
                    g.RemoveNode(nod);
                    break;
                }
            }

            int param = (int)numericUpDown2.Value;
            counter = param;

            for (int i = 0; i < param; i++)
            {
                g.AddNode(i.ToString()).Attr.Shape = Shape.Circle;
            }
            for (int i = 0; i < param; i++)
            {
                for (int j = i + 1; j < param; j++)
                {
                    Edge ed = g.AddEdge(i.ToString(), j.ToString());
                    ed.Attr.ArrowheadAtTarget = ArrowStyle.None;
                    ed.Attr.Id = i.ToString() + "_" + j.ToString();
                }
            }

            viewer.Graph = g;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (counter == 1)
            {
                return;
            }
            g.RemoveNode(g.FindNode((--counter).ToString()));
            Utilities.ClearVertexColor(g);
            viewer.Graph = g;

            this.edgeNode1.Maximum = counter - 1;
            this.edgeNode2.Maximum = counter - 1;
        }
    }
}
