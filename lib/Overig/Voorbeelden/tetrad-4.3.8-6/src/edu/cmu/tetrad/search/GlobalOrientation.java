package edu.cmu.tetrad.search;

import edu.cmu.tetrad.data.Knowledge;
import edu.cmu.tetrad.graph.Graph;

/**
 * Does a global reorientation of the given undirected graph structure. Any
 * orientations in the graph are ignored; only the undirected structure is
 * paid attention to.
 *
 * @author Joseph Ramsey
 */
public interface GlobalOrientation {

    /**
     * Sets the knowledge.
     */
    void setKnowledge(Knowledge knowledge);

    /**
     * Globally reorients the graph.
     */
    void reorient(Graph graph);
}
