using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    public partial class Form1 : Form
    {
        Graph g;
        Microsoft.Msagl.GraphViewerGdi.GViewer viewer;
        Microsoft.Msagl.GraphViewerGdi.GViewer viewerRight;

        string firstNodeClicked = "";
        Edge selectedEdge = null;
        //gViewer1.NeedToCalculateLayout = false;

        public Form1()
        {
            InitializeComponent();
            InitGraphLayout();
        }

        private void InitGraphLayout()
        {
            //FIRST viewer

            //create a viewer object 
            viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
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

            //create a graph object 
            g = new Graph("graph");
            g.Directed = false;

            //create the graph content 
            GraphUtilities.AddEdge(g, "0", "1");
            GraphUtilities.AddEdge(g, "0", "2");
            GraphUtilities.AddEdge(g, "1", "2");
           
            g.FindNode("0").Attr.Shape = Shape.Circle;
            g.FindNode("1").Attr.Shape = Shape.Circle;
            g.FindNode("2").Attr.Shape = Shape.Circle;

            //bind the graph to the viewer 
            viewer.Graph = g;

            //associate the viewer with the form 
            viewer.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(viewer);

            // Handle single-click events for adding edges
            viewer.MouseUp += Viewer_MouseUp;

            // Handle delete button press
            viewer.KeyDown += Viewer_KeyDown;

            // SECOND VIEWER

            viewerRight = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            viewerRight = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            viewerRight.OutsideAreaBrush = System.Drawing.Brushes.White;
            viewerRight.UndoRedoButtonsVisible = false;
            viewerRight.EdgeInsertButtonVisible = false;
            viewerRight.SaveAsMsaglEnabled = false;
            viewerRight.SaveInVectorFormatEnabled = false;
            viewerRight.SaveAsImageEnabled = false;
            viewerRight.NavigationVisible = false;
            viewerRight.AllowDrop = false;
            viewerRight.InsertingEdge = false;
            viewerRight.SaveButtonVisible = false;

            Graph dummy = new Graph("dummy");
            dummy.Directed = false;
            dummy.AddNode("dummy").Attr.Shape = Shape.Circle;

            viewerRight.Graph = dummy;

            viewerRight.Dock = DockStyle.Fill;
            this.panel2.Controls.Add(viewerRight);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int from = comboBox2.SelectedIndex;
            int to = comboBox3.SelectedIndex;
            int param = (int)numericUpDown1.Value;

            GraphProblem problem; // default value, so the compiler doesn't scream
            GraphUtilities.ClearVertexColorAndEdgeStyle(g);
            switch (from)
            {
                case 0:
                    problem = new Clique(g, param);
                    break;
                case 1:
                    problem = new IndepSet(g, param);
                    break;
                case 2:
                    problem = new VertexCover(g, param);
                    break;
                case 3:
                    problem = new DominatingSet(g, param);
                    break;
                case 4:
                    problem = new Colorability(g, param);
                    break;
                case 5:
                    problem = new HamilCycle(g, param);
                    break;
                default:
                    return;
            }

            GraphProblem result;       
            switch (to)
            {
                case 0:
                    result = problem.ToClique();
                    break;
                case 1:
                    result = problem.ToIndepSet();
                    break;
                case 2:
                    result = problem.ToVertexCover();
                    break;
                case 3:
                    result = problem.ToDominatingSet();
                    break;
                case 4:
                    result = problem.ToColorability();
                    break;
                case 5:
                    result = problem.ToHamilCycle();
                    break;
                default:
                    return;
            }

            Graph reduction = result.G;
            int secondParam = result.Param;

            label1.Text = $"Param: {param}";
            label2.Text = $"Param: {secondParam}";
            viewerRight.Graph = reduction;
            viewer.Graph = g;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            selectedEdge = null;
            firstNodeClicked = "";

            if (g.NodeCount == 0)
            {
                return;
            }

            int index = comboBox1.SelectedIndex;
            int param = (int)numericUpDown1.Value;

            GraphProblem problem;
            GraphUtilities.ClearVertexColorAndEdgeStyle(g);
            switch (index)
            {
                case 0:
                    problem = new Clique(g, param);
                    break;
                case 1:
                    problem = new IndepSet(g, param);
                    break;
                case 2:
                    problem = new VertexCover(g, param);
                    break;
                case 3:
                    problem = new DominatingSet(g, param);
                    break;
                case 4:
                    problem = new Colorability(g, param);
                    break;
                case 5:
                    problem = new HamilCycle(g, param);
                    break;
                default:
                    return;
            }

            problem.Solve();
            problem.DrawSolution();

            viewer.Graph = g;
        }

        void addCompleteGraph(int vertexCount)
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

            int param = vertexCount;

            for (int i = 0; i < param; i++)
            {
                g.AddNode(i.ToString()).Attr.Shape = Shape.Circle;
            }
            for (int i = 0; i < param; i++)
            {
                for (int j = i + 1; j < param; j++)
                {
                    GraphUtilities.AddEdge(g, i.ToString(), j.ToString());
                }
            }

            viewer.Graph = g;
        }

        private void Viewer_MouseUp(object sender, MouseEventArgs e)
        {
            var gviewer = (Microsoft.Msagl.GraphViewerGdi.GViewer)sender;
            var dnode = gviewer.ObjectUnderMouseCursor as Microsoft.Msagl.GraphViewerGdi.DNode;
            var dedge = gviewer.ObjectUnderMouseCursor as Microsoft.Msagl.GraphViewerGdi.DEdge;

            if(dedge != null)
            {
                firstNodeClicked = "";
                selectedEdge = dedge.Edge;
                return;
            }

            if (dnode == null)
            {
                firstNodeClicked = "";
                selectedEdge = null;
                return;
            }

            if (firstNodeClicked == "")
            {
                firstNodeClicked = dnode.Node.LabelText;
                selectedEdge = null;
            }
            else
            {
                string dnodeLabel = dnode.Node.LabelText;
                Node n1 = g.FindNode(firstNodeClicked);
                Node n2 = g.FindNode(dnodeLabel);

                Edge e1 = GraphUtilities.EdgeById(g, firstNodeClicked + "_" + dnodeLabel);
                Edge e2 = GraphUtilities.EdgeById(g, dnodeLabel + "_" + firstNodeClicked);

                if (n1 != null && n2 != null && e1 == null && e2 == null && n1.Id != n2.Id && e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    int index1 = int.Parse(firstNodeClicked);
                    int index2 = int.Parse(dnodeLabel);

                    //edge is always from vertex with smaller index to vertex with higher index
                    if (index1 > index2)
                    {
                        string temp = firstNodeClicked;
                        firstNodeClicked = dnodeLabel;
                        dnodeLabel = temp;
                    }

                    GraphUtilities.AddEdge(g, firstNodeClicked, dnodeLabel);

                    GraphUtilities.ClearVertexColorAndEdgeStyle(g);
                    viewer.Graph = g;
                    firstNodeClicked = "";
                    selectedEdge = null;
                }
            }
        }

        private void Viewer_KeyDown(object sender, KeyEventArgs e)
        {
            // If not delete or NumPad
            if (e.KeyValue >= 97 && e.KeyValue <= 105)
            {
                firstNodeClicked = "";
                selectedEdge = null;
                addCompleteGraph(e.KeyValue - 96);
                return;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (g.NodeCount == 20)
                {
                    return;
                }
                selectedEdge = null;
                firstNodeClicked = "";
                g.AddNode(g.NodeCount.ToString()).Attr.Shape = Shape.Circle;
                GraphUtilities.ClearVertexColorAndEdgeStyle(g);
                viewer.Graph = g;
                return;
            }
            else if (e.KeyCode != Keys.Delete)
            {
                return;
            }

            if (selectedEdge != null)
            {
                g.RemoveEdge(selectedEdge);
                selectedEdge = null;
                firstNodeClicked = "";
                GraphUtilities.ClearVertexColorAndEdgeStyle(g);
                viewer.Graph = g;
            }
            else if (firstNodeClicked != "")
            {
                GraphUtilities.RemoveNode(ref g, firstNodeClicked);
                selectedEdge = null;
                firstNodeClicked = "";
                viewer.Graph = g;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graph map = new Graph();

            map.AddEdge("Sat", "3-Sat");
            map.AddEdge("Clique", "Sat");
            map.AddEdge("Independent Set", "Sat");
            map.AddEdge("Vertex Cover", "Sat");
            map.AddEdge("Dominating Set", "Sat");
            map.AddEdge("Coloring", "Sat");
            map.AddEdge("Hamiltonian Cycle", "Sat");

            map.AddEdge("Vertex Cover", "Dominating Set");
            map.AddEdge("Vertex Cover", "Hamiltonian Cycle").Attr.Color = Color.Red;

            map.AddEdge("Independent Set", "Clique");
            map.AddEdge("Clique", "Independent Set");

            map.AddEdge("Independent Set", "Vertex Cover");
            map.AddEdge("Vertex Cover", "Independent Set");

            map.AddEdge("3-Sat", "Independent Set");
            map.AddEdge("3-Sat", "Coloring");

            viewerRight.Graph = map;
        }
    }
}
