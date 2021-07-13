package edu.cmu.tetrad.graph;

import edu.cmu.tetrad.util.TetradSerializable;

import java.beans.PropertyChangeListener;
import java.io.PrintStream;
import java.util.List;
import java.util.Set;

/**
 * Created by IntelliJ IDEA. User: jdramsey Date: Jan 25, 2007 Time: 12:52:10 PM
 * To change this template use File | Settings | File Templates.
 */
public interface IDag extends Graph, TetradSerializable {
    boolean addBidirectedEdge(Node node1, Node node2);

    boolean addEdge(Edge edge);

    boolean addDirectedEdge(Node node1, Node node2);

    boolean addGraphConstraint(GraphConstraint gc);

    boolean addPartiallyOrientedEdge(Node node1, Node node2);

    boolean addNode(Node node);

    void addPropertyChangeListener(PropertyChangeListener l);

    boolean addUndirectedEdge(Node node1, Node node2);

    boolean addNondirectedEdge(Node node1, Node node2);

    void clear();

    boolean containsEdge(Edge edge);

    boolean containsNode(Node node);

    boolean defNonDescendent(Node node1, Node node2);

    boolean existsDirectedCycle();

    boolean defVisible(Edge edge);

    boolean defNonCollider(Node node1, Node node2, Node node3);

    boolean defCollider(Node node1, Node node2, Node node3);

    boolean existsTrek(Node node1, Node node2);

    boolean equals(Object o);

    boolean existsDirectedPathFromTo(Node node1, Node node2);

    boolean existsSemiDirectedPathFromTo(Node node1, Set<Node> nodes);

    boolean existsInducingPath(Node node1, Node node2,
            Set<Node> observedNodes, Set<Node> conditioningNodes);

    void fullyConnect(Endpoint endpoint);

    Endpoint getEndpoint(Node node1, Node node2);

    Endpoint[][] getEndpointMatrix();

    List<Node> getAdjacentNodes(Node node);

    List<Node> getNodesInTo(Node node, Endpoint endpoint);

    List<Node> getNodesOutTo(Node node, Endpoint n);

    List<Node> getNodes();

    List<Edge> getEdges();

    List<Edge> getEdges(Node node);

    List<Edge> getEdges(Node node1, Node node2);

    Node getNode(String name);

    int getNumEdges();

    int getNumNodes();

    int getNumEdges(Node node);

    List<GraphConstraint> getGraphConstraints();

    List<List<Node>> getTiers();

    List<Node> getChildren(Node node);

    int getConnectivity();

    List<Node> getDescendants(List<Node> nodes);

    Edge getEdge(Node node1, Node node2);

    Edge getDirectedEdge(Node node1, Node node2);

    List<Node> getParents(Node node);

    int getIndegree(Node node);

    int getOutdegree(Node node);

    List<Node> getTierOrdering();

    boolean isAdjacentTo(Node nodeX, Node nodeY);

    boolean isAncestorOf(Node node1, Node node2);

    boolean isDirectedFromTo(Node node1, Node node2);

    boolean isUndirectedFromTo(Node node1, Node node2);

    boolean isGraphConstraintsChecked();

    boolean isParentOf(Node node1, Node node2);

    boolean isProperAncestorOf(Node node1, Node node2);

    boolean isProperDescendentOf(Node node1, Node node2);

    boolean isExogenous(Node node);

    boolean isDConnectedTo(Node node1, Node node2,
            List<Node> conditioningNodes);

    boolean isDSeparatedFrom(Node node1, Node node2, List<Node> z);

    boolean isChildOf(Node node1, Node node2);

    boolean isDescendentOf(Node node1, Node node2);

    void printTiers(PrintStream out);

    void printTierOrdering();

    boolean removeEdge(Node node1, Node node2);

    boolean removeEdges(Node node1, Node node2);

    boolean setEndpoint(Node node1, Node node2, Endpoint endpoint);

    Graph subgraph(List<Node> nodes);

    void setGraphConstraintsChecked(boolean checked);

    boolean removeEdge(Edge edge);

    boolean removeEdges(List<Edge> edges);

    boolean removeNode(Node node);

    boolean removeNodes(List<Node> nodes);

    void reorientAllWith(Endpoint endpoint);

    boolean possibleAncestor(Node node1, Node node2);

    List<Node> getAncestors(List<Node> nodes);

    boolean possDConnectedTo(Node node1, Node node2, List<Node> z);

    void transferNodesAndEdges(Graph graph)
            throws IllegalArgumentException;

    void markAmbiguous(Triple triple);

    boolean isAmbiguous(Triple triple);

    String toString();
}
