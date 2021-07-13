package edu.cmu.tetradapp.model;

import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.graph.GraphUtils;
import edu.cmu.tetrad.search.ImpliedOrientation;
import edu.cmu.tetrad.search.MeekRules;
import edu.cmu.tetrad.search.VanderbiltMmhc;

/**
 * Extends AbstractAlgorithmRunner to produce a wrapper for the GES algorithm.
 *
 * @author Ricardo Silva
 */

public class MmhcRunner extends AbstractAlgorithmRunner implements GraphSource {
    static final long serialVersionUID = 23L;

    //============================CONSTRUCTORS============================//

    public MmhcRunner(DataWrapper dataWrapper, BasicSearchParams params) {
        super(dataWrapper, params);
    }

    /**
     * Generates a simple exemplar of this class to test serialization.
     *
     * @see edu.cmu.TestSerialization
     * @see edu.cmu.tetradapp.util.TetradSerializableUtils
     */
    public static MmhcRunner serializableInstance() {
        return new MmhcRunner(DataWrapper.serializableInstance(),
                BasicSearchParams.serializableInstance());
    }                                                     

    //============================PUBLIC METHODS==========================//

    /**
     * Executes the algorithm, producing (at least) a result workbench. Must be
     * implemented in the extending class.
     */

    public void execute() {
        VanderbiltMmhc search;
        Object source = getDataSet();

        search = new VanderbiltMmhc((RectangularDataSet) source, 3);

        Graph searchGraph = search.search();
        setResultGraph(searchGraph);
        GraphUtils.arrangeBySourceGraph(getResultGraph(), getSourceGraph());
    }

    public Graph getGraph() {
        return getResultGraph();
    }

    public boolean supportsKnowledge() {
        return true;
    }

    public ImpliedOrientation getMeekRules() {
        MeekRules rules = new MeekRules();
        rules.setKnowledge(getParams().getKnowledge());
        return rules;
    }
}
