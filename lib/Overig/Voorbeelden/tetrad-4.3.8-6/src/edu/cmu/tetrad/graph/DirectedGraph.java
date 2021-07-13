///////////////////////////////////////////////////////////////////////////////
// For information as to what this class does, see the Javadoc, below.       //
// Copyright (C) 2005 by Peter Spirtes, Richard Scheines, Joseph Ramsey,     //
// and Clark Glymour.                                                        //
//                                                                           //
// This program is free software; you can redistribute it and/or modify      //
// it under the terms of the GNU General Public License as published by      //
// the Free Software Foundation; either version 2 of the License, or         //
// (at your option) any later version.                                       //
//                                                                           //
// This program is distributed in the hope that it will be useful,           //
// but WITHOUT ANY WARRANTY; without even the implied warranty of            //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the             //
// GNU General Public License for more details.                              //
//                                                                           //
// You should have received a copy of the GNU General Public License         //
// along with this program; if not, write to the Free Software               //
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA //
///////////////////////////////////////////////////////////////////////////////

package edu.cmu.tetrad.graph;

import edu.cmu.tetrad.util.TetradSerializable;

import java.beans.PropertyChangeListener;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.util.Arrays;
import java.util.List;
import java.util.Set;

/**
 * A graph that permits only directed edges, with no loops, at most one edge per
 * node pair, and with measured or latent node only.
 *
 * @author Joseph Ramsey
 * @version $Revision: 5967 $ $Date: 2006-01-09 13:18:07 -0500 (Mon, 09 Jan
 *          2006) $
 * @see MeasuredLatentOnly
 * @see DirectedEdgesOnly
 */
public final class DirectedGraph implements TetradSerializable, Graph {
    static final long serialVersionUID = 23L;

    private static final GraphConstraint[] constraints = {
            new MeasuredLatentOnly(), new AtMostOneEdgePerPair(),
            new NoEdgesToSelf(), new DirectedEdgesOnly()};

    /**
     * @serial
     */
    private final Graph graph = new EdgeListGraph();

    //=========================CONSTRUCTORS==========================//

    /**
     * Constructs a new blank DG.
     */
    public DirectedGraph() {
        List<GraphConstraint> constraints1 = Arrays.asList(constraints);

        for (Object aConstraints1 : constraints1) {
            addGraphConstraint((GraphConstraint) aConstraints1);
        }
    }

    /**
     * Constructs a new DG based on the given graph.
     *
     * @param graph the graph to base the new DG on.
     * @throws IllegalArgumentException if the given graph cannot be converted
     *                                  into a DG for some reason.
     */
    public DirectedGraph(Graph graph) throws IllegalArgumentException {
        List<GraphConstraint> constraints1 = Arrays.asList(constraints);

        for (Object aConstraints1 : constraints1) {
            addGraphConstraint((GraphConstraint) aConstraints1);
        }

        transferNodesAndEdges(graph);
    }

    /**
     * Generates a simple exemplar of this class to test serialization.
     *
     * @see edu.cmu.TestSerialization
     * @see edu.cmu.tetradapp.util.TetradSerializableUtils
     */
    public static DirectedGraph serializableInstance() {
        return new DirectedGraph();
    }

    //==========================PUBLIC METHODS=======================//

    public final void transferNodesAndEdges(Graph graph)
            throws IllegalArgumentException {
        this.graph.transferNodesAndEdges(graph);
    }

    public void markAmbiguous(Triple triple) {
        graph.markAmbiguous(triple);
    }

    public boolean isAmbiguous(Triple triple) {
        return graph.isAmbiguous(triple);
    }

    public void fullyConnect(Endpoint endpoint) {
        graph.fullyConnect(endpoint);
    }

    public void reorientAllWith(Endpoint endpoint) {
        graph.reorientAllWith(endpoint);
    }

    public Endpoint[][] getEndpointMatrix() {
        return graph.getEndpointMatrix();
    }

    public List<Node> getAdjacentNodes(Node node) {
        return graph.getAdjacentNodes(node);
    }

    public List<Node> getNodesInTo(Node node, Endpoint endpoint) {
        return graph.getNodesInTo(node, endpoint);
    }

    public List<Node> getNodesOutTo(Node node, Endpoint n) {
        return graph.getNodesOutTo(node, n);
    }

    public List<Node> getNodes() {
        return graph.getNodes();
    }

    public boolean removeEdge(Node node1, Node node2) {
        return graph.removeEdge(node1, node2);
    }

    public boolean removeEdges(Node node1, Node node2) {
        return graph.removeEdges(node1, node2);
    }

    public boolean isAdjacentTo(Node nodeX, Node nodeY) {
        return graph.isAdjacentTo(nodeX, nodeY);
    }

    public boolean setEndpoint(Node node1, Node node2, Endpoint endpoint) {
        return graph.setEndpoint(node1, node2, endpoint);
    }

    public Endpoint getEndpoint(Node node1, Node node2) {
        return graph.getEndpoint(node1, node2);
    }

    public boolean equals(Object o) {
        return graph.equals(o);
    }

    public Graph subgraph(List<Node> nodes) {
        return graph.subgraph(nodes);
    }

    public boolean existsDirectedPathFromTo(Node node1, Node node2) {
        return graph.existsDirectedPathFromTo(node1, node2);
    }

    public boolean existsSemiDirectedPathFromTo(Node node1, Set<Node> nodes) {
        return graph.existsSemiDirectedPathFromTo(node1, nodes);
    }

    public boolean addDirectedEdge(Node nodeA, Node nodeB) {
        return graph.addDirectedEdge(nodeA, nodeB);
    }

    public boolean addUndirectedEdge(Node nodeA, Node nodeB) {
        return graph.addUndirectedEdge(nodeA, nodeB);
    }

    public boolean addNondirectedEdge(Node nodeA, Node nodeB) {
        return graph.addNondirectedEdge(nodeA, nodeB);
    }

    public boolean addPartiallyOrientedEdge(Node nodeA, Node nodeB) {
        return graph.addPartiallyOrientedEdge(nodeA, nodeB);
    }

    public boolean addBidirectedEdge(Node nodeA, Node nodeB) {
        return graph.addBidirectedEdge(nodeA, nodeB);
    }

    public boolean addEdge(Edge edge) {
        return graph.addEdge(edge);
    }

    public boolean addNode(Node node) {
        return graph.addNode(node);
    }

    public void addPropertyChangeListener(PropertyChangeListener l) {
        graph.addPropertyChangeListener(l);
    }

    public boolean containsEdge(Edge edge) {
        return graph.containsEdge(edge);
    }

    public boolean containsNode(Node node) {
        return graph.containsNode(node);
    }

    public List<Edge> getEdges() {
        return graph.getEdges();
    }

    public List<Edge> getEdges(Node node) {
        return graph.getEdges(node);
    }

    public List<Edge> getEdges(Node node1, Node node2) {
        return graph.getEdges(node1, node2);
    }

    public Node getNode(String name) {
        return graph.getNode(name);
    }

    public int getNumEdges() {
        return graph.getNumEdges();
    }

    public int getNumNodes() {
        return graph.getNumNodes();
    }

    public int getNumEdges(Node node) {
        return graph.getNumEdges(node);
    }

    public List<GraphConstraint> getGraphConstraints() {
        return graph.getGraphConstraints();
    }

    public boolean isGraphConstraintsChecked() {
        return graph.isGraphConstraintsChecked();
    }

    public void setGraphConstraintsChecked(boolean checked) {
        graph.setGraphConstraintsChecked(checked);
    }

    public boolean removeEdge(Edge edge) {
        return graph.removeEdge(edge);
    }

    public boolean removeEdges(List<Edge> edges) {
        return graph.removeEdges(edges);
    }

    public boolean removeNode(Node node) {
        return graph.removeNode(node);
    }

    public void clear() {
        graph.clear();
    }

    public boolean removeNodes(List<Node> nodes) {
        return graph.removeNodes(nodes);
    }

    public boolean existsDirectedCycle() {
        return graph.existsDirectedCycle();
    }

    public boolean isDirectedFromTo(Node node1, Node node2) {
        return graph.isDirectedFromTo(node1, node2);
    }

    public boolean isUndirectedFromTo(Node node1, Node node2) {
        return graph.isUndirectedFromTo(node1, node2);
    }

    public boolean defVisible(Edge edge) {
        return graph.defVisible(edge);
    }

    public boolean defNonCollider(Node node1, Node node2, Node node3) {
        return graph.defNonCollider(node1, node2, node3);
    }

    public boolean defCollider(Node node1, Node node2, Node node3) {
        return graph.defCollider(node1, node2, node3);
    }

    public boolean existsTrek(Node node1, Node node2) {
        return graph.existsTrek(node1, node2);
    }

    public List<Node> getChildren(Node node) {
        return graph.getChildren(node);
    }

    public int getConnectivity() {
        return graph.getConnectivity();
    }

    public List<Node> getDescendants(List<Node> nodes) {
        return graph.getDescendants(nodes);
    }

    public Edge getEdge(Node node1, Node node2) {
        return graph.getEdge(node1, node2);
    }

    public Edge getDirectedEdge(Node node1, Node node2) {
        return graph.getDirectedEdge(node1, node2);
    }

    public List<Node> getParents(Node node) {
        return graph.getParents(node);
    }

    public int getIndegree(Node node) {
        return graph.getIndegree(node);
    }

    public int getOutdegree(Node node) {
        return graph.getOutdegree(node);
    }

    public boolean isAncestorOf(Node node1, Node node2) {
        return graph.isAncestorOf(node1, node2);
    }

    public boolean possibleAncestor(Node node1, Node node2) {
        return graph.possibleAncestor(node1, node2);
    }

    public List<Node> getAncestors(List<Node> nodes) {
        return graph.getAncestors(nodes);
    }

    public boolean isChildOf(Node node1, Node node2) {
        return graph.isChildOf(node1, node2);
    }

    public boolean isDescendentOf(Node node1, Node node2) {
        return graph.isDescendentOf(node1, node2);
    }

    public boolean defNonDescendent(Node node1, Node node2) {
        return graph.defNonDescendent(node1, node2);
    }

    public boolean isDConnectedTo(Node node1, Node node2,
            List<Node> conditioningNodes) {
        return graph.isDConnectedTo(node1, node2, conditioningNodes);
    }

    public boolean isDSeparatedFrom(Node node1, Node node2, List<Node> z) {
        return graph.isDSeparatedFrom(node1, node2, z);
    }

    public boolean possDConnectedTo(Node node1, Node node2, List<Node> z) {
        return graph.possDConnectedTo(node1, node2, z);
    }

    public boolean existsInducingPath(Node node1, Node node2,
            Set<Node> observedNodes, Set<Node> conditioningNodes) {
        return graph.existsInducingPath(node1, node2, observedNodes,
                conditioningNodes);
    }

    public boolean isParentOf(Node node1, Node node2) {
        return graph.isParentOf(node1, node2);
    }

    public boolean isProperAncestorOf(Node node1, Node node2) {
        return graph.isProperAncestorOf(node1, node2);
    }

    public boolean isProperDescendentOf(Node node1, Node node2) {
        return graph.isProperDescendentOf(node1, node2);
    }

    public boolean isExogenous(Node node) {
        return graph.isExogenous(node);
    }

    public String toString() {
        return graph.toString();
    }

    public boolean addGraphConstraint(GraphConstraint gc) {
        return graph.addGraphConstraint(gc);
    }

    /**
     * Adds semantic checks to the default deserialization method. This method
     * must have the standard signature for a readObject method, and the body of
     * the method must begin with "s.defaultReadObject();". Other than that, any
     * semantic checks can be specified and do not need to stay the same from
     * version to version. A readObject method of this form may be added to any
     * class, even if Tetrad sessions were previously saved out using a version
     * of the class that didn't include it. (That's what the
     * "s.defaultReadObject();" is for. See J. Bloch, Effective Java, for help.
     *
     * @throws java.io.IOException
     * @throws ClassNotFoundException
     */
    private void readObject(ObjectInputStream s)
            throws IOException, ClassNotFoundException {
        s.defaultReadObject();

        if (graph == null) {
            throw new NullPointerException();
        }

    }
}


