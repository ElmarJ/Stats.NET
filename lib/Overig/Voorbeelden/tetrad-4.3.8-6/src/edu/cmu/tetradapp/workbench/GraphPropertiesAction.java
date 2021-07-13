package edu.cmu.tetradapp.workbench;

import edu.cmu.tetrad.graph.*;
import edu.cmu.tetrad.util.JOptionUtils;

import javax.swing.*;
import java.awt.datatransfer.Clipboard;
import java.awt.datatransfer.ClipboardOwner;
import java.awt.datatransfer.Transferable;
import java.awt.event.ActionEvent;
import java.util.List;

/**
 * Puts up a panel showing some graph properties, e.g., number of nodes and
 * edges in the graph, etc.
 *
 * @author Joseph Ramsey jdramsey@andrew.cmu.edu
 */
public class GraphPropertiesAction extends AbstractAction implements ClipboardOwner {
    private GraphWorkbench workbench;

    /**
     * Creates a new copy subsession action for the given LayoutEditable and
     * clipboard.
     */
    public GraphPropertiesAction(GraphWorkbench workbench) {
        super("Graph Properties");
        this.workbench = workbench;
    }

    /**
     * Copies a parentally closed selection of session nodes in the frontmost
     * session editor to the clipboard.
     */
    public void actionPerformed(ActionEvent e) {
        Box b = Box.createVerticalBox();
        Graph graph = workbench.getGraph();
        Box b1 = Box.createHorizontalBox();
        b1.add(new JLabel("Number of nodes: "));
        b1.add(Box.createHorizontalGlue());
        b1.add(new JLabel(String.valueOf(graph.getNumNodes())));
        b.add(b1);

        int numLatents = 0;
        for (Node node : graph.getNodes()) {
            if (node.getNodeType() == NodeType.LATENT) {
                numLatents++;
            }
        }

        int maxIndegree = 0;
        for (Node node : graph.getNodes()) {
            int indegree = graph.getNodesInTo(node, Endpoint.ARROW).size();

            if (indegree > maxIndegree) {
                maxIndegree = indegree;
            }
        }

        int maxOutdegree = 0;
        for (Node node : graph.getNodes()) {
            int outdegree = graph.getNodesOutTo(node, Endpoint.ARROW).size();

            if (outdegree > maxOutdegree) {
                maxOutdegree = outdegree;
            }
        }

        int numDirectedEdges = 0;
        int numBidirectedEdges = 0;
        int numUndirectedEdges = 0;

        for (Edge edge : graph.getEdges()) {
            if (Edges.isDirectedEdge(edge)) numDirectedEdges++;
            else if (Edges.isBidirectedEdge(edge)) numBidirectedEdges++;
            else if (Edges.isUndirectedEdge(edge)) numUndirectedEdges++;
        }

        boolean cyclic = graph.existsDirectedCycle();
        List<Node> cycle = GraphUtils.directedCycle(graph);

        Box b2 = Box.createHorizontalBox();
        b2.add(new JLabel("Number of latents: "));
        b2.add(Box.createHorizontalGlue());
        b2.add(new JLabel(String.valueOf(numLatents)));
        b.add(b2);

        Box b3 = Box.createHorizontalBox();
        b3.add(new JLabel("Number of edges: "));
        b3.add(Box.createHorizontalGlue());
        b3.add(new JLabel(String.valueOf(graph.getNumEdges())));
        b.add(b3);

        Box b3a = Box.createHorizontalBox();
        b3a.add(new JLabel("Number of directed edges: "));
        b3a.add(Box.createHorizontalGlue());
        b3a.add(new JLabel(String.valueOf(numDirectedEdges)));
        b.add(b3a);

        Box b3b = Box.createHorizontalBox();
        b3b.add(new JLabel("Number of bidirected edges: "));
        b3b.add(Box.createHorizontalGlue());
        b3b.add(new JLabel(String.valueOf(numBidirectedEdges)));
        b.add(b3b);

        Box b3c = Box.createHorizontalBox();
        b3c.add(new JLabel("Number of undirected edges: "));
        b3c.add(Box.createHorizontalGlue());
        b3c.add(new JLabel(String.valueOf(numUndirectedEdges)));
        b.add(b3c);

        Box b4 = Box.createHorizontalBox();
        b4.add(new JLabel("Max Degree: "));
        b4.add(Box.createHorizontalGlue());
        b4.add(new JLabel(String.valueOf(graph.getConnectivity())));
        b.add(b4);

        Box b5 = Box.createHorizontalBox();
        b5.add(new JLabel("Max Indegree: "));
        b5.add(Box.createHorizontalGlue());
        b5.add(new JLabel(String.valueOf(maxIndegree)));
        b.add(b5);

        Box b6 = Box.createHorizontalBox();
        b6.add(new JLabel("Max Outdegree: "));
        b6.add(Box.createHorizontalGlue());
        b6.add(new JLabel(String.valueOf(maxOutdegree)));
        b.add(b6);

        Box b7 = Box.createHorizontalBox();
        b7.add(new JLabel("Cyclic?"));
        b7.add(Box.createHorizontalGlue());
        b7.add(new JLabel(cyclic ? "Yes" : "No"));
        b.add(b7);

        if (cyclic) {
            Box b8 = Box.createHorizontalBox();
            b8.add(new JLabel("Example cycle: "));
            b8.add(Box.createHorizontalGlue());
            b8.add(new JLabel(cycle.toString()));
            b.add(b8);
        }


        JOptionPane.showMessageDialog(JOptionUtils.centeringComp(), b,
                "Graph Properties", JOptionPane.PLAIN_MESSAGE);
    }

    /**
     * Required by the AbstractAction interface; does nothing.
     */
    public void lostOwnership(Clipboard clipboard, Transferable contents) {
    }


}
