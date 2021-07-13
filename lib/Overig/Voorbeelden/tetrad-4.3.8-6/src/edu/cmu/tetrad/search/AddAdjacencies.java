package edu.cmu.tetrad.search;

import edu.cmu.tetrad.data.Knowledge;
import edu.cmu.tetrad.graph.Edge;
import edu.cmu.tetrad.graph.EdgeListGraph;
import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.graph.Node;
import edu.cmu.tetrad.util.ChoiceGenerator;
import edu.cmu.tetrad.util.SubsetGenerator;
import edu.cmu.tetrad.util.TetradLogger;

import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Set;

/**
 * Implements the "fast adjacency search" used in several causal algorithms in
 * this package. In the fast adjacency search, at a given stage of the search,
 * an edge X*-*Y is removed from the graph if X _||_ Y | S, where S is a subset
 * of size d either of adj(X) or of adj(Y), where d is the depth of the search.
 * The fast adjacency search performs this procedure for each pair of adjacent
 * edges in the graph and for each depth d = 0, 1, 2, ..., d1, where d1 is the
 * first such depth at which no edges can be removed. The interpretation of this
 * adjacency search is different for different algorithms, depending on the
 * assumptions of the algorithm. Sepsets are not collected, and no sepset
 * mapping is returned.
 *
 * @author Joseph Ramsey.
 * @version $Revision: 4591 $ $Date: 2006-01-20 13:34:55 -0500 (Fri, 20 Jan
 *          2006) $
 */
public class AddAdjacencies {

    /**
     * The search graph. It is assumed going in that all of the true adjacencies
     * of x are in this graph for every node x. It is hoped (i.e. true in the
     * large sample limit) that true adjacencies are never removed.
     */

    private Graph graph;

    /**
     * The independence test.
     */
    private IndependenceTest test;

    /**
     * Specification of which edges are forbidden or required.
     */
    private Knowledge knowledge;

    /**
     * The maximum number of variables conditioned on in any conditional
     * independence test. The value is -1 if depth is unlimited, or a
     * non-negative integer otherwise.
     */
    private int depth = Integer.MAX_VALUE;

    //==========================CONSTRUCTORS=============================//

    /**
     * Constructs a new FastAdjacencySearch.
     */
    public AddAdjacencies(Graph graph, IndependenceTest test) {
        this.graph = graph;
        this.test = test;
    }

    //==========================PUBLIC METHODS===========================//

    /**
     * Discovers all adjacencies in data.  The procedure is to remove edges in
     * the graph which connect pairs of variables which are independent
     * conditional on some other set of variables in the graph (the "sepset").
     * These are removed in tiers.  First, edges which are independent
     * conditional on zero other variables are removed, then edges which are
     * independent conditional on one other variable are removed, then two, then
     * three, and so on, until no more edges can be removed from the graph.  The
     * edges which remain in the graph after this procedure are the adjacencies
     * in the data.
     */
    public void addAdjacencies() {
        TetradLogger.getInstance().info("Starting Fast Adjacency Search.");

        // Remove edges forbidden both ways.
//        List<Edge> edges = graph.getEdges();
//
//        for (Edge edge1 : edges) {
//            String name1 = edge1.getNode1().getName();
//            String name2 = edge1.getNode2().getName();
//
//            if (getKnowledge().edgeForbidden(name1, name2) &&
//                    getKnowledge().edgeForbidden(name2, name1)) {
//                graph.removeEdge(edge1);
//            }
//        }

        TetradLogger.getInstance().info("Depth = " + ((getDepth() == Integer
                .MAX_VALUE) ? "Unlimited" : Integer.toString(getDepth())));

        int _depth = getDepth();

        if (_depth == -1) {
            _depth = Integer.MAX_VALUE;
        }

        addEdges(graph, test);

        TetradLogger.getInstance().info("Finishing Fast Adjacency Search.");
    }

    public int getDepth() {
        return depth;
    }

    public void setDepth(int depth) {
        if (depth < -1) {
            throw new IllegalArgumentException(
                    "Depth must be -1 (unlimited) or >= 0.");
        }

        this.depth = depth;
    }

    public Knowledge getKnowledge() {
        return knowledge;
    }

    public void setKnowledge(Knowledge knowledge) {
        if (knowledge == null) {
            throw new NullPointerException("Cannot set knowledge to null");
        }
        this.knowledge = knowledge;
    }

    //==============================PRIVATE METHODS======================/

    /**
     * Removes from the list of nodes any that cannot be parents of x given the
     * background knowledge.
     */
    private List<Node> possibleParents(Node x, Node y, List<Node> nodes,
                                       Knowledge knowledge) {
        List<Node> possibleParents = new LinkedList<Node>();
        String _x = x.getName();
        String _y = y.getName();

        for (Node z : nodes) {
            String _z = z.getName();

            if (possibleParentOf(_z, _x, _y, knowledge)) {
                possibleParents.add(z);
            }
        }

        return possibleParents;
    }

    /**
     * Returns true just in case z is a possible parent of both x and y, in the
     * sense that edges are not forbidden from z to either x or y, and edges are
     * not required from either x or y to z, according to background knowledge.
     */
    private boolean possibleParentOf(String z, String x, String y,
                                     Knowledge knowledge) {
        if (knowledge.edgeForbidden(z, x)) {
            return false;
        }

        if (knowledge.edgeForbidden(z, y)) {
            return false;
        }

        if (knowledge.edgeRequired(x, z)) {
            return false;
        }

        return !knowledge.edgeRequired(y, z);
    }

    /**
     * Performs one depth step of the adjacency search.
     *
     * @param graph     The search graph. This will be modified.
     * @param test      The independence test.
     */
    private void addEdges(Graph graph, IndependenceTest test) {
        Graph graphCopy = new EdgeListGraph(graph);
        List<Node> nodes = new LinkedList<Node>(graphCopy.getNodes());

        ChoiceGenerator cg1 = new ChoiceGenerator(nodes.size(), 2);
        int[] choice1;

        PAIR:
        while ((choice1 = cg1.next()) != null) {
            List<Node> pair = SearchGraphUtils.asList(choice1, nodes);
            Node x = pair.get(0);
            Node y = pair.get(1);

            if (graphCopy.isAdjacentTo(x, y)) {
                continue;
            }

            List<Node> adjx = graphCopy.getAdjacentNodes(x);
            List<Node> adjy = graphCopy.getAdjacentNodes(y);
            adjx.remove(y);
            adjy.remove(x);

            SubsetGenerator genx = new SubsetGenerator(adjx.size());
            boolean[] subsetx;

            while ((subsetx = genx.next()) != null) {
                List<Node> condx = new LinkedList<Node>();

                for (int i = 0; i < adjx.size(); i++) {
                    if (subsetx[i]) {
                        condx.add(adjx.get(i));
                    }
                }

                if (test.isIndependent(x, y, condx)) {
//                    if (graphCopy.isAdjacentTo(x, y)) {
//                        graph.removeEdge(x, y);
//                        System.out.println("Removing " + graph.getEdge(x, y));
//                    }

                    continue PAIR;
                }
            }

            SubsetGenerator geny = new SubsetGenerator(adjy.size());
            boolean[] subsety;

            while ((subsety = geny.next()) != null) {
                List<Node> condy = new LinkedList<Node>();

                for (int i = 0; i < adjy.size(); i++) {
                    if (subsety[i]) {
                        condy.add(adjy.get(i));
                    }
                }

                if (test.isIndependent(x, y, condy)) {
//                    if (graphCopy.isAdjacentTo(x, y)) {
//                        graph.removeEdge(x, y);
//                        System.out.println("Removing " + graph.getEdge(x, y));
//                    }

                    continue PAIR;
                }
            }

            if (graph.getEdge(x, y) == null) {
                graph.addUndirectedEdge(x, y);
                System.out.println("Added " + graph.getEdge(x, y));
            }
        }
    }

    /**
     * A slightly more conservative adjacency search. Should leave fewer
     * false positive adjacencies in the output.
     */
    private boolean searchAtDepth2(Graph graph, IndependenceTest test,
                                   Knowledge knowledge, int depth) {
        boolean more = false;
        List<Edge> edges = new LinkedList<Edge>(graph.getEdges());

        EDGE: for (Edge edge : edges) {
            Node x = edge.getNode1();
            Node y = edge.getNode2();

            Set<Node> set = new HashSet<Node>();
            set.addAll(graph.getAdjacentNodes(x));
            set.addAll(graph.getAdjacentNodes(y));
            set.remove(x);
            set.remove(y);

            List<Node> adjxy = new LinkedList<Node>(set);
            List<Node> ppxy = possibleParents(x, y, adjxy, knowledge);

            boolean noEdgeRequired =
                    knowledge.noEdgeRequired(x.getName(), y.getName());

            if (ppxy.size() >= depth) {
                ChoiceGenerator cg = new ChoiceGenerator(ppxy.size(), depth);
                int[] choice;

                while ((choice = cg.next()) != null) {
                    List<Node> condSet = SearchGraphUtils.asList(choice, ppxy);
                    boolean independent = test.isIndependent(x, y, condSet);

                    if (independent && noEdgeRequired) {
                        graph.removeEdge(x, y);
                        continue EDGE;
                    }
                }
            }

            if (graph.getAdjacentNodes(x).size() - 1 > depth) {
                more = true;
            }
        }

        return more;
    }
}
