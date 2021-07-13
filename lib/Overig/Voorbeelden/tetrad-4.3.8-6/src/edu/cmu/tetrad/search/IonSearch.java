
package edu.cmu.tetrad.search;

import edu.cmu.tetrad.graph.*;
import edu.cmu.tetrad.util.TetradLogger;


import java.util.*;

/**
 * Implements the ION algorithm by David Danks, for merging causal PAGS.
 *
 * @author Tyler Gibson, Trevor Burns
 */
public class IonSearch {

    /**
     * The input that are being intergrated.
     */
    private List<Pag> input = new ArrayList<Pag>();


    /**
     * The output graphs.
     */
    private List<Pag> output = new ArrayList<Pag>();


    /**
     * Set of non-adjacent nodes.
     */
    private Set<NodePair> notadjacent = new HashSet<NodePair>();


    /**
     * All the variables involved in the search.
     */
    private List<Node> variables = new ArrayList<Node>();


    public IonSearch(List<Graph> pags) {
        for (Graph graph : pags) {
            if (graph instanceof Pag) {
                this.input.add((Pag) graph);
            } else {
                this.input.add(new Pag(graph));
            }
        }
        for (Pag pag : input) {
            for (Node node : pag.getNodes()) {
                if (!variables.contains(node)) {
                    this.variables.add(node);
                }
            }
        }
    }


    //============================= Public Methods ============================//


    public List<Graph> search() {
        TetradLogger.getInstance().info("Starting ION Search.");
        logGraphs("\nInitial Pags: ", this.input);

        Pag graph = new Pag(this.variables);
        graph.fullyConnect(Endpoint.CIRCLE);
        // Part 2.
        removeAdjacencies(graph);
        transferOrientations(graph);
        this.output.add(graph);

        // Part 3.
        List<Set<IonIndependenceFacts>> sepAndAssoc = findSepAndAssoc();
        Set<IonIndependenceFacts> separations = sepAndAssoc.get(0);
        Set<IonIndependenceFacts> associations = sepAndAssoc.get(1);

        Map<Collection<Node>, List<PossibleDConnectingPath>> paths =
                new HashMap<Collection<Node>, List<PossibleDConnectingPath>>();

        for (IonIndependenceFacts fact : separations) {
            // Part 3.a
            for (Pag pag : this.output) {
                List<PossibleDConnectingPath> dConnections = new ArrayList<PossibleDConnectingPath>();
                for (Collection<Node> conditions : fact.getZ())
                    dConnections.addAll(PossibleDConnectingPath.findDConnectingPaths
                            (pag, fact.getX(), fact.getY(), conditions));
                for (PossibleDConnectingPath path : dConnections) {
                    List<PossibleDConnectingPath> p = paths.get(path.getConditions());
                    if (p == null) {
                        p = new LinkedList<PossibleDConnectingPath>();
                        p.add(path);
                        paths.put(path.getConditions(), p);
                    } else {
                        p.add(path);
                    }
                }
            }

            // Part 3.b

            List possibleChanges = findChanges(paths);

            Collection<GraphChange> hittingSets = IonHittingSet.findHittingSet(possibleChanges);

            ArrayList<Pag> changedPags = new ArrayList<Pag> ();
            for (Pag pag : this.output) {
                for (GraphChange gc : hittingSets) {
                    Pag changed = gc.applyTo(pag);

                    //Part 3.c

                    if (changed != null && !predictsFalseIndependence(associations, changed)
                            && !hasCircularPath(changed))
                        changedPags.add(changed);
                }
            }

            this.output = changedPags;
        }

        // Part 4
        // Needs to be implemented



        // Part 5

        logGraphs("\nReturning input:", this.output);
        return new ArrayList<Graph>(this.output);
    }



    //============================= Private Methods =============================//



    /**
     * Returns all the triples in the graph that can be either oriented as a collider or non-collider.
     */
    private static Set<Triple> getAllTriples(Pag pag){
        Set<Triple> triples = new HashSet<Triple>();
        for(Edge edge : pag.getEdges()){
            if(isUndirected(edge)){
                Node y = edge.getNode2();
                for(Edge adjEdge : pag.getEdges(y)){
                    if(isUndirected(adjEdge)){
                        if(!pag.isUnderlineTriple(edge.getNode1(), y, adjEdge.getNode2())){
                            triples.add(new Triple(edge.getNode1(), y, adjEdge.getNode2()));
                        }
                    }
                }
            }
        }
        return triples;
    }


    /**
     * Finds the association or seperation sets for every pair of nodes.
     */
    private List<Set<IonIndependenceFacts>> findSepAndAssoc() {
        Set<IonIndependenceFacts> separations = new HashSet<IonIndependenceFacts>();
        Set<IonIndependenceFacts> associations = new HashSet<IonIndependenceFacts>();
        Set<NodePair> allNodes = makeAllPairs(this.variables);

        for (NodePair pair : allNodes) {
            Node x = pair.getFirst();
            Node y = pair.getSecond();

            List<Node> variables = new ArrayList<Node>(this.variables);
            variables.remove(x);
            variables.remove(y);

            List<Set<Node>> subsets = SearchGraphUtils.powerSet(variables);

            IonIndependenceFacts indep = new IonIndependenceFacts(x, y, new HashSet<List<Node>>());
            IonIndependenceFacts assoc = new IonIndependenceFacts(x, y, new HashSet<List<Node>>());
            boolean addIndep = false;
            boolean addAssoc = false;

            for (Pag pag : input) {
                for (Set<Node> subset : subsets) {
                    if (containsAll(pag, subset, pair) ) {
                        if (pag.isDSeparatedFrom(x, y, new ArrayList<Node>(subset))) {
                            if (this.notadjacent.contains(pair)) {
                                addIndep = true;
                                indep.addMoreZ(new ArrayList<Node>(subset));
                            }
                        }
                        else {
                            addAssoc = true;
                            assoc.addMoreZ(new ArrayList<Node>(subset));
                        }
                    }
                }
            }
            if (addIndep) separations.add(indep);
            if (addAssoc) associations.add(assoc);
        }

        List<Set<IonIndependenceFacts>> ret = new ArrayList<Set<IonIndependenceFacts>> (2);
        ret.add(0,separations);
        ret.add(1,associations);
        return ret;
    }


    /**
     * States whether the given graph contains the nodes in the given set and the
     * node pair.
     */
    private static boolean containsAll(Graph g, Set<Node> nodes, NodePair pair) {
        if (!g.containsNode(pair.getFirst()) || !g.containsNode(pair.getSecond())) {
            return false;
        }
        for (Node node : nodes) {
            if (!g.containsNode(node)) {
                return false;
            }
        }
        return true;
    }


    /**
     * Remove all X,Y such that X,Y are not adjacent in some input PAG.  Assumes the given graph
     * is fully connected etc.
     */
    private void removeAdjacencies(Graph graph) {
        for (Edge edge : graph.getEdges()) {
            for (Graph input : this.input) {
                if (!input.containsNode(edge.getNode1()) || !input.containsNode(edge.getNode2())) {
                    continue;
                }
                if (!input.isAdjacentTo(edge.getNode1(), edge.getNode2())) {
                    graph.removeEdge(edge);
                    NodePair nodePair = new NodePair(edge.getNode1(), edge.getNode2());
                    this.notadjacent.add(nodePair);
                    TetradLogger.getInstance().details("Added to NotAdj : " + nodePair);
                }
            }
        }
    }


    /**
     * Transfers edge info from the input to the given graph.
     */
    private void transferOrientations(Pag graph) {
        Map<Edge, Edge> edgeMap = new HashMap<Edge, Edge>();
        Set<Edge> conflict = new HashSet<Edge>();
        // remove non-adjecent edges and transfer edge orientation.
        for (Edge sourceEdge : graph.getEdges()) {
            Node node1 = sourceEdge.getNode1();
            Node node2 = sourceEdge.getNode2();
            for (Graph pag : this.input) {
                if (!pag.containsNode(node1) || !pag.containsNode(node2)) {
                    continue;
                }
                Edge edge = pag.getEdge(node1, node2);
                if (edge == null) {
                    graph.removeEdge(sourceEdge);
                    NodePair nodePair = new NodePair(node1, node2);
                    this.notadjacent.add(nodePair);
                    TetradLogger.getInstance().details("Added to NotAdj: " + nodePair);
                } else if (isPartiallyDirected(edge) && !conflict.contains(edge)) {
                    Edge previous = edgeMap.get(sourceEdge);
                    if (previous != null && isConflict(previous, edge)) {
                        conflict.add(edge);
                        sourceEdge.setEndpoint1(Endpoint.CIRCLE);
                        sourceEdge.setEndpoint2(Endpoint.CIRCLE);
                    } else {
                        Edge clone = new Edge(sourceEdge);
                        if (sourceEdge.getNode1().equals(edge.getNode1())) {
                            sourceEdge.setEndpoint1(edge.getEndpoint1());
                            sourceEdge.setEndpoint2(edge.getEndpoint2());
                        } else {
                            sourceEdge.setEndpoint1(edge.getEndpoint2());
                            sourceEdge.setEndpoint2(edge.getEndpoint1());
                        }
                        TetradLogger.getInstance().details("Oriented edge " + clone + " to " + sourceEdge);
                        edgeMap.put(clone, new Edge(sourceEdge));
                    }
                }
            }
        }
        // Now deal with underline orientations.
        for (Graph g : this.input) {
            if (!(g instanceof Pag)) {
                continue;
            }
            Pag pag = (Pag) g;
            for (Triple triple : pag.getUnderLineTriples()) {
                if (matchesTriple(graph, triple)) {
                    graph.addUnderlineTriple(triple);
                }
            }
        }
    }


    /**
     * States whether there is a conflict between the previous edge and the current one.
     */
    private static boolean isConflict(Edge previous, Edge current) {
        Node node1 = previous.getNode1();
        if (previous.getEndpoint1() != Endpoint.CIRCLE && previous.getEndpoint1() != current.getProximalEndpoint(node1)) {
            return true;
        }
        Node node2 = previous.getNode2();
        return current.getEndpoint2() != Endpoint.CIRCLE && previous.getEndpoint2() != current.getProximalEndpoint(node2);
    }


    /**
     * True if the given graph contains nodes that match the triple (three adjacent nodes).
     */
    private static boolean matchesTriple(Graph graph, Triple triple) {
        if (!graph.containsNode(triple.getX()) || !graph.containsNode(triple.getY()) ||
                !graph.containsNode(triple.getZ())) {
            return false;
        }
        Edge edge1 = graph.getEdge(triple.getX(), triple.getY());
        Edge edge2 = graph.getEdge(triple.getY(), triple.getZ());
        if (edge1 == null || edge2 == null) {
            return false;
        }
        //noinspection SimplifiableIfStatement
        if (edge1.getEndpoint1() != Endpoint.CIRCLE || edge1.getEndpoint2() != Endpoint.CIRCLE) {
            return false;
        }
        return !(edge2.getEndpoint1() != Endpoint.CIRCLE || edge2.getEndpoint2() != Endpoint.CIRCLE);
    }


    /**
     * States whether the edge is "partially directed", such as A 0-> B (this may have
     * a real name?)
     */
    private static boolean isPartiallyDirected(Edge edge) {
        Endpoint end1 = edge.getEndpoint1();
        Endpoint end2 = edge.getEndpoint2();
        return end1 == Endpoint.ARROW || end1 == Endpoint.TAIL ||
                end2 == Endpoint.ARROW || end2 == Endpoint.TAIL;
    }


    /**
     * Checks endpoints to determine whether edge is undirected
     */
    private static boolean isUndirected(Edge edge){
        return edge.getEndpoint1() == Endpoint.CIRCLE && edge.getEndpoint2() == Endpoint.CIRCLE;
    }


    private static void logGraphs(String message, List<? extends Graph> graphs) {
        if (message != null) {
            TetradLogger.getInstance().log("graph", message);
        }
        for (Graph graph : graphs) {
            TetradLogger.getInstance().log("graph", graph.toString());
        }
    }


    /**
     * Checks given pag against a set of necessary associations to determine if the pag
     * implies an indepedence where one is known to not exist.
     */
    private static boolean predictsFalseIndependence (Set<IonIndependenceFacts> associations, Pag pag) {
        for (IonIndependenceFacts assocFact : associations)
            for (List<Node> conditioningSet : assocFact.getZ())
                if ( pag.isDSeparatedFrom(
                        assocFact.getX(), assocFact.getY() , conditioningSet ) )
                    return true;
        return false;
    }


    /**
     * Returns whether the given Pag contains any circular triples
     */
    private static boolean hasCircularPath (Pag pag) {
        Set<Triple> triples = getAllTriples(pag);
        for (Triple trip : triples) {
            Edge one = pag.getEdge(trip.getX(), trip.getY());
            Edge two = pag.getEdge(trip.getY(), trip.getZ());
            Edge three = pag.getEdge(trip.getZ(), trip.getZ());
            if (one.getDistalEndpoint(trip.getX()).equals(Endpoint.ARROW)
                    && two.getDistalEndpoint(trip.getY()).equals(Endpoint.ARROW)
                    && three.getDistalEndpoint(trip.getZ()).equals(Endpoint.ARROW) )
                return true;
        }
        return false;
    }


    /**
     * Creates a set of NodePairs of all possible pairs of nodes from given
     * list of nodes.
     */
    private static Set<NodePair> makeAllPairs (List<Node> nodes) {
        Set<NodePair> allNodes = new HashSet<NodePair>();
        for (int i = 0 ; i<nodes.size() ; i++)
            for (int j = i+1 ; j < nodes.size() ; j++)
                allNodes.add(new NodePair(nodes.get(i), nodes.get(j)));

        return allNodes;
    }


    /**
     * Given a map between sets of conditioned on variables and lists of PossibleDConnectingPaths,
     * finds all the possible GraphChanges which could be used to block said paths
     */
    private static List findChanges (Map<Collection<Node>, List<PossibleDConnectingPath>> paths) {
        List<Set<GraphChange>> pagChanges = new ArrayList<Set<GraphChange>>();

        Set<Map.Entry<Collection<Node>, List<PossibleDConnectingPath>>> entries = paths.entrySet();

        /* Loop through each entry, ie each conditioned set of variables. */
        for (Map.Entry<Collection<Node>, List<PossibleDConnectingPath>> entry : entries) {
            Collection<Node> conditions = entry.getKey();
            List<PossibleDConnectingPath> dConnecting = entry.getValue();

            /* loop through each path */
            for (PossibleDConnectingPath possible : dConnecting ) {
                List<Node> possPath = possible.getPath();

                /* Created with 2*# of paths as appoximation. might have to increase size once */
                Set<GraphChange> pathChanges = new HashSet<GraphChange>(2 * possPath.size());

                /* find those conditions which are not along the path (used in colider) */
                List<Node> outsidePath = new ArrayList<Node>(conditions.size());
                for (Node condition : conditions) {
                    if (!possPath.contains(condition))
                        outsidePath.add(condition);
                }

                /* Walk through path, node by node */
                for (int i = 0; i < possPath.size() - 1; i++) {
                    Node current = possPath.get(i);
                    Node next = possPath.get(i + 1);
                    GraphChange gc;

                    /* for each pair of nodes, add the operation to remove their edge */
                    gc = new GraphChange();
                    gc.addRemove(possible.getPag().getEdge(current, next));
                    pathChanges.add(gc);

                    /* for each triple centered on a node which is an element of the conditioning
                     * set, add the operation to orient as a nonColider around that node */
                    if (conditions.contains(current) && i > 0) {
                        gc = new GraphChange();
                        Triple nonColider = new Triple( possPath.get(i - 1), current, next);
                        gc.addNonCollider(nonColider);
                        pathChanges.add(gc);
                    }

                    /* for each node on the path not in the conditioning set, make a colider. It
                     * is necessary though to ensure that there are no paths implying that a
                     * conditioned variable (even outside the path) is a decendant of a colider */
                    if ((!conditions.contains(current)) && i > 0) {
                        Triple colider = new Triple( possPath.get(i - 1), current, next);

                        if ( possible.getPag().isUnderlineTriple(possPath.get(i-1), current, next) )
                            continue;

                        /* Simple case, no conditions outside the path, so just add colider */
                        if (outsidePath.size() == 0) {
                            gc = new GraphChange();
                            gc.addCollider(colider);
                            pathChanges.add(gc);
                            continue;
                        }

                        /* ensure nondecendency in possible path between current and each conditioned
                         * variable outside the path */
                        for (Node outside : outsidePath) {

                            /* list of possible decendant paths */
                            List<PossibleDConnectingPath> decendantPaths
                                    = PossibleDConnectingPath.findDConnectingPaths
                                    (possible.getPag(), current, outside, new ArrayList<Node>());

                            /* loop over each possible path which might indicate decendency */
                            for (PossibleDConnectingPath decendantPDCPath : decendantPaths) {
                                List<Node> decendantPath = decendantPDCPath.getPath();

                                /* walk down path checking orientation (path may already
                                 * imply non-decendency) and creating changes if need be*/
                                boolean impliesDecendant = true;
                                Set<GraphChange> colideChanges = new HashSet<GraphChange>();
                                for (int j = 0; j < decendantPath.size() - 1; j++) {
                                    Node from = decendantPath.get(j);
                                    Node to = decendantPath.get(+1);
                                    Edge currentEdge = possible.getPag().getEdge(from, to);

                                    if (currentEdge.getEndpoint1().equals(Endpoint.ARROW)) {
                                        impliesDecendant = false;
                                        break;
                                    }

                                    gc = new GraphChange();
                                    gc.addCollider(colider);
                                    gc.addRemove(currentEdge);
                                    colideChanges.add(gc);

                                    gc = new GraphChange();
                                    gc.addCollider(colider);
                                    gc.addOrient(to, from);
                                    colideChanges.add(gc);
                                }
                                if (impliesDecendant)
                                    pathChanges.addAll(colideChanges);
                            }
                        }
                    }
                }

                pagChanges.add(pathChanges);
            }
        }
        return pagChanges;
    }


    /**
     * Exactly the same as edu.cmu.tetrad.graph.IndependenceFact excepting this class allows
     * for multiple conditioning sets to be associated with a single pair of nodes, which is
     * necessary for the proper ordering of iterations in the ION search.
     */
    private final class IonIndependenceFacts {
        private Node x;
        private Node y;
        private Collection<List<Node>> z;

        /**
         * Constructs a triple of nodes.
         */
        public IonIndependenceFacts(Node x, Node y, Collection<List<Node>> z) {
            if (x == null || y == null || z == null) {
                throw new NullPointerException();
            }

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public final Node getX() {
            return x;
        }

        public final Node getY() {
            return y;
        }

        public final Collection<List<Node>> getZ() {
            return z;
        }

        public void addMoreZ(List<Node> moreZ) {
            z.add(moreZ);
        }

        public final int hashCode() {
            int hash = 17;
            hash += 19 * x.hashCode() * y.hashCode();
            hash += 23 * z.hashCode();
            return hash;
        }

        public final boolean equals(Object obj) {
            if (!(obj instanceof IonIndependenceFacts)) {
                return false;
            }

            IonIndependenceFacts fact = (IonIndependenceFacts) obj;
            return (x.equals(fact.x) && y.equals(fact.y) &&
                    z.equals( fact.z))
                    || (x.equals(fact.y) & y.equals(fact.x) &&
                    z.equals( fact.z));
        }

        public String toString() {
            return "I(" + x + ", " + y + " | " + z + ")";
        }
    }
}
