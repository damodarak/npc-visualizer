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
            viewer.LayoutAlgorithmSettingsButtonVisible = false;
            viewer.SaveButtonVisible = false;

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
           
            g.FindNode("0").Attr.Shape = Shape.Circle;
            g.FindNode("1").Attr.Shape = Shape.Circle;
            g.FindNode("2").Attr.Shape = Shape.Circle;

            //bind the graph to the viewer 
            viewer.Graph = g;

            //associate the viewer with the form 
            viewer.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(viewer);

            // Handle double-click event on the viewer
            viewer.MouseDoubleClick += Viewer_MouseDoubleClick;

            // Handle single-click events for adding edges
            viewer.MouseUp += Viewer_MouseUp;

            // Handle delete button press
            viewer.KeyDown += Viewer_KeyDown;
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

            Problem problem;
            Utilities.ClearVertexColorAndEdgeStyle(g);
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
            selectedEdge = null;
            firstNodeClicked = "";

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

        private void Viewer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            selectedEdge = null;
            firstNodeClicked = "";

            var gviewer = (Microsoft.Msagl.GraphViewerGdi.GViewer)sender;
            var dnode = gviewer.ObjectUnderMouseCursor as Microsoft.Msagl.GraphViewerGdi.DNode;
            if(dnode != null )
            {
                return;
            }

            if (g.NodeCount == 20)
            {
                return;
            }
            g.AddNode(g.NodeCount.ToString()).Attr.Shape = Shape.Circle;
            Utilities.ClearVertexColorAndEdgeStyle(g);
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

                Edge e1 = Utilities.EdgeById(g, firstNodeClicked + "_" + dnodeLabel);
                Edge e2 = Utilities.EdgeById(g, dnodeLabel + "_" + firstNodeClicked);

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

                    Edge new_e = g.AddEdge(firstNodeClicked, dnodeLabel);
                    new_e.Attr.ArrowheadAtTarget = ArrowStyle.None;
                    new_e.Attr.Id = firstNodeClicked + "_" + dnodeLabel;

                    Utilities.ClearVertexColorAndEdgeStyle(g);
                    viewer.Graph = g;
                    firstNodeClicked = "";
                    selectedEdge = null;
                }
            }
        }

        private void Viewer_KeyDown(object sender, KeyEventArgs e)
        {
            //TODO: delete the vertex and lower the IDs of higher vertices 

            firstNodeClicked = "";

            // If not delete
            if (e.KeyCode != Keys.Delete)
            {
                return;
            }

            if(selectedEdge != null)
            {
                g.RemoveEdge(selectedEdge);
                selectedEdge = null;
                Utilities.ClearVertexColorAndEdgeStyle(g);
                viewer.Graph = g;
            }
        }
    }
}
