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

        public Form1()
        {
            InitializeComponent();
            InitGraphLayout();
        }

        private void InitGraphLayout()
        {
            // FIRST viewer

            // Create and configurate the viewer object 
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

            // Create a graph object 
            g = new Graph("graph");
            g.Directed = false;

            // Create simple graph content 
            GraphUtilities.AddEdge(g, "0", "1");
            GraphUtilities.AddEdge(g, "0", "2");
            GraphUtilities.AddEdge(g, "1", "2");
           
            g.FindNode("0").Attr.Shape = Shape.Circle;
            g.FindNode("1").Attr.Shape = Shape.Circle;
            g.FindNode("2").Attr.Shape = Shape.Circle;

            // Bind the graph to the viewer 
            viewer.Graph = g;

            // Associate the viewer with the form 
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
            dummy.AddNode("0").Attr.Shape = Shape.Circle;

            viewerRight.Graph = dummy;

            viewerRight.Dock = DockStyle.Fill;
            this.panel2.Controls.Add(viewerRight);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Indices of the problems
            int from = comboBox2.SelectedIndex;
            int to = comboBox3.SelectedIndex;

            int param = (int)numericUpDown1.Value;

            // Param larger than the g.NodeCount doesn't make any sense and might create some bugs
            if (param > g.NodeCount)
            {
                param = g.NodeCount;
            }

            GraphProblem problem;
            GraphUtilities.ClearVertexColorAndEdgeStyle(g);
            cleanRightViewerAndInfoParams();

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

            problem.Solve();
            problem.DrawSolution();

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

            if (problem.HasSolution)
            {
                result.Solve();
                result.DrawSolution();
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
            cleanRightViewerAndInfoParams();

            selectedEdge = null;
            firstNodeClicked = "";

            // No need to calculate anything
            if (g.NodeCount == 0)
            {
                return;
            }

            int index = comboBox1.SelectedIndex;
            int param = (int)numericUpDown1.Value;

            if (param > g.NodeCount)
            {
                param = g.NodeCount;
            }

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

        void cleanRightViewerAndInfoParams()
        {
            label1.Text = "Param: -1";
            label2.Text = "Param: -1";

            Graph right = new Graph();
            right.Directed = false;
            viewerRight.Graph = right;
        }

        void addCompleteGraph(int vertexCount)
        {
            cleanRightViewerAndInfoParams();

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

            for (int i = 0; i < vertexCount; i++)
            {
                g.AddNode(i.ToString()).Attr.Shape = Shape.Circle;
            }
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = i + 1; j < vertexCount; j++)
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

            // Clicked on an edge
            if(dedge != null)
            {
                firstNodeClicked = "";
                selectedEdge = dedge.Edge;
                return;
            }

            // Clicked on empty space
            if (dnode == null)
            {
                firstNodeClicked = "";
                selectedEdge = null;
                return;
            }

            // Clicked on a node
            if (firstNodeClicked == "")
            {
                firstNodeClicked = dnode.Node.LabelText;
                selectedEdge = null;
            }
            else
            {
                // Second clicked indicates that the user wants to connect the nodes in graph
                string dnodeLabel = dnode.Node.LabelText;
                Node n1 = g.FindNode(firstNodeClicked);
                Node n2 = g.FindNode(dnodeLabel);

                Edge e1 = GraphUtilities.EdgeById(g, firstNodeClicked + "_" + dnodeLabel);
                Edge e2 = GraphUtilities.EdgeById(g, dnodeLabel + "_" + firstNodeClicked);

                // Vertices exist and aren't connected and second click was with the RMB
                if (n1 != null && n2 != null && e1 == null && e2 == null && n1.Id != n2.Id && e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    int index1 = int.Parse(firstNodeClicked);
                    int index2 = int.Parse(dnodeLabel);

                    GraphUtilities.AddEdge(g, firstNodeClicked, dnodeLabel);

                    GraphUtilities.ClearVertexColorAndEdgeStyle(g);
                    cleanRightViewerAndInfoParams();
                    viewer.Graph = g;
                    firstNodeClicked = "";
                    selectedEdge = null;
                }
            }
        }

        private void Viewer_KeyDown(object sender, KeyEventArgs e)
        {
            // Add complete graph with 0-9 vertices
            if (e.KeyValue >= 97 && e.KeyValue <= 105)
            {
                firstNodeClicked = "";
                selectedEdge = null;
                addCompleteGraph(e.KeyValue - 96);
                return;
            }
            // Add another vertex to the graph
            else if (e.KeyCode == Keys.Enter)
            {
                if (g.NodeCount == 20) // Maximum amount
                {
                    return;
                }

                selectedEdge = null;
                firstNodeClicked = "";
                g.AddNode(g.NodeCount.ToString()).Attr.Shape = Shape.Circle;
                GraphUtilities.ClearVertexColorAndEdgeStyle(g);
                cleanRightViewerAndInfoParams();
                viewer.Graph = g;
                return;
            }
            else if (e.KeyCode != Keys.Delete)
            {
                return;
            }

            // Remove Edge from graph
            if (selectedEdge != null)
            {
                g.RemoveEdge(selectedEdge);
                selectedEdge = null;
                firstNodeClicked = "";
                GraphUtilities.ClearVertexColorAndEdgeStyle(g);
                cleanRightViewerAndInfoParams();
                viewer.Graph = g;
            }
            // Remove Node from graph
            else if (firstNodeClicked != "")
            {
                GraphUtilities.RemoveNode(ref g, firstNodeClicked);
                cleanRightViewerAndInfoParams();
                selectedEdge = null;
                firstNodeClicked = "";
                viewer.Graph = g;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Show the graph of reductions
            Form2 map = new Form2();
            map.Show();
        }

        // Copy graph from the right panel to the left panel
        private void button4_Click(object sender, EventArgs e)
        {
            if (viewerRight.Graph.NodeCount > 20) return;

            g = GraphUtilities.CopyGraph(viewerRight.Graph);
            cleanRightViewerAndInfoParams();
            viewer.Graph = g;
        }
    }
}
