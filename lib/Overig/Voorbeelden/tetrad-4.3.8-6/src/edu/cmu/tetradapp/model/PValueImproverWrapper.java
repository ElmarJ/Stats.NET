package edu.cmu.tetradapp.model;

import edu.cmu.tetrad.search.*;
import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.data.Knowledge;
import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.graph.GraphUtils;
import edu.cmu.tetrad.sem.SemIm;

import java.beans.PropertyChangeListener;
import java.beans.PropertyChangeEvent;
import java.util.List;
import java.util.ArrayList;

/**
 * Extends AbstractAlgorithmRunner to produce a wrapper for the GES algorithm.
 *
 * @author Ricardo Silva
 */

public class PValueImproverWrapper extends AbstractAlgorithmRunner implements
        GraphSource {
    static final long serialVersionUID = 23L;
    private String name;
    private SemIm semIm;
    private transient List<PropertyChangeListener> listeners;
    private DataWrapper dataWrapper;
    private GesParams params;
    private SemIm estSem;

    //============================CONSTRUCTORS============================//

    public PValueImproverWrapper(SemImWrapper semImWrapper,
                                DataWrapper dataWrapper,
                                GesParams params) {
        super(dataWrapper, params);
        this.dataWrapper = dataWrapper;
        this.params = params;
        this.semIm = semImWrapper.getSemIm();
    }

    public PValueImproverWrapper(SemEstimatorWrapper semEstimatorWrapper,
                                DataWrapper dataWrapper,
                                GesParams params) {
        super(dataWrapper, params);
        this.dataWrapper = dataWrapper;
        this.params = params;
        this.semIm = semEstimatorWrapper.getEstimatedSemIm();
    }

    /**
     * Generates a simple exemplar of this class to test serialization.
     *
     * @see edu.cmu.TestSerialization
     * @see edu.cmu.tetradapp.util.TetradSerializableUtils
     */
    public static PValueImproverWrapper serializableInstance() {
        return new PValueImproverWrapper(SemImWrapper.serializableInstance(),
                DataWrapper.serializableInstance(),
                GesParams.serializableInstance());
    }

    //============================PUBLIC METHODS==========================//


    public void setName(String name) {
        this.name = name;
    }

    public String getName() {
        return this.name;
    }

    /**
     * Executes the algorithm, producing (at least) a result workbench. Must be
     * implemented in the extending class.
     */

    public void execute() {
        Object source = dataWrapper.getSelectedDataModel();

        if (!(source instanceof RectangularDataSet)) {
            throw new RuntimeException("Sem Score Search requires a rectangular " +
                    "dataset.");
        }

        RectangularDataSet dataSet = (RectangularDataSet) source;
        Knowledge knowledge = params.getKnowledge();
        PValueImprover search = new PValueImprover(semIm, dataSet, knowledge);
        search.setKnowledge(knowledge);

        fireGraphChange(semIm.getSemPm().getGraph());

        this.estSem = search.search();

        Graph graph = estSem.getSemPm().getGraph();
        GraphUtils.arrangeBySourceGraph(graph, getSourceGraph());
        setResultGraph(graph);
    }

    public boolean supportsKnowledge() {
        return true;
    }

    public ImpliedOrientation getMeekRules() {
        MeekRules rules = new MeekRules();
        rules.setKnowledge(params.getKnowledge());
        return rules;
    }

    private boolean isAggressivelyPreventCycles() {
        return params.isAggressivelyPreventCycles();
    }

    public void addPropertyChangeListener(PropertyChangeListener l) {
        if (!getListeners().contains(l)) getListeners().add(l);
    }

    private void fireGraphChange(Graph graph) {
        for (PropertyChangeListener l : getListeners()) {
            l.propertyChange(new PropertyChangeEvent(this, "graph", null, graph));
        }
    }

    public SemIm getSemIm() {
        return semIm;
    }

    public SemIm getEstSem() {
        return estSem;
    }

    public Graph getGraph() {
        return estSem.getSemPm().getGraph();
    }

    private List<PropertyChangeListener> getListeners() {
        if (listeners == null) {
            listeners = new ArrayList<PropertyChangeListener>();
        }
        return listeners;
    }
}
