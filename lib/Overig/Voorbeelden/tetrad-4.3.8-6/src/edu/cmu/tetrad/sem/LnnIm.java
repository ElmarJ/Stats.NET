package edu.cmu.tetrad.sem;

import cern.colt.matrix.DoubleMatrix2D;
import cern.colt.matrix.impl.SparseDoubleMatrix2D;
import edu.cmu.tetrad.data.ColtDataSet;
import edu.cmu.tetrad.data.ContinuousVariable;
import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.graph.Node;
import edu.cmu.tetrad.graph.NodeType;
import edu.cmu.tetrad.graph.SemGraph;
import edu.cmu.tetrad.util.*;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.rmi.MarshalledObject;
import java.util.*;

/**
 * This is an instantiated model (i.e. a model in which values for the
 * parameters are given) for a LNN (Linear Non-Normal). The underlying graph is
 * assumed to be acyclic. The parameters of the model are the linear
 * coefficients of the edges (a linear model is assumed over the variables)
 * together with the coefficients of the exogenous terms.
 *
 * @author Joseph Ramsey
 * @version $Revision$
 */
public final class LnnIm implements TetradSerializable {
    static final long serialVersionUID = 23L;

    /**
     * The Sem PM containing the graph and the parameters to be estimated. For
     * now a defensive copy of this is not being constructed, since it is not
     * used anywhere in the code except in the the constructor and in its
     * accessor method. It somebody changes it, it's their own fault, but it
     * won't affect this class.
     *
     * @serial Cannot be null.
     */
    private final LnnPm lnnPm;

    /**
     * The list of measured and latent variableNodes for the semPm.
     * (Unmodifiable.)
     *
     * @serial Cannot be null.
     */
    private final List<Node> variableNodes;

    /**
     * The list of measured variableNodes from the semPm. (Unmodifiable.)
     *
     * @serial Cannot be null.
     */
    private final List<Node> measuredNodes;

    /**
     * The list of free parameters (Unmodifiable). This must be in the same
     * order as this.freeMappings.
     *
     * @serial Cannot be null.
     */
    private List<Parameter> freeParameters;

    /**
     * The list of fixed parameters (Unmodifiable). This must be in the same
     * order as this.fixedMappings.
     *
     * @serial Cannot be null.
     */
    private List<Parameter> fixedParameters;

    /**
     * Matrix of edge coefficients. edgeCoef[i][j] is the coefficient of the
     * edge from getVariableNodes().get(i) to getVariableNodes().get(j), or 0.0
     * if this edge is not in the graph. The values of these may be changed, but
     * the array itself may not.
     *
     * @serial Cannot be null.
     */
    private DoubleMatrix2D edgeCoef;

    /**
     * Map from nodes to error distributions.
     */
    private Map<Node, Distribution> distributions;

    /**
     * The sample size.
     *
     * @serial Range >= 0.
     */
    private int sampleSize;

    /**
     * The list of freeMappings. This is an unmodifiable list. It is fixed (up
     * to order) by the SemPm. This must be in the same order as
     * this.freeParameters.
     *
     * @serial Cannot be null.
     */
    private List<LnnMapping> freeMappings;

    /**
     * The list of fixed parameters (Unmodifiable). This must be in the same
     * order as this.fixedParameters.
     *
     * @serial Cannot be null.
     */
    private List<LnnMapping> fixedMappings;

    /**
     * Stores the standard errors for the parameters.  May be null.
     *
     * @serial Can be null.
     */
    private double[] standardErrors;

    /**
     * True iff setting parameters to out-of-bound values throws exceptions.
     *
     * @serial Any value.
     */
    private boolean parameterBoundsEnforced = true;

    /**
     * True iff this SemIm is estimated.
     *
     * @serial Any value.
     */
    private boolean estimated = false;

    //=============================CONSTRUCTORS============================//

    public LnnIm(LnnPm lnnPm) {
        if (lnnPm == null) {
            throw new NullPointerException("Sem PM must not be null.");
        }

        this.lnnPm = new LnnPm(lnnPm);
        this.variableNodes =
                Collections.unmodifiableList(lnnPm.getVariableNodes());
        this.measuredNodes =
                Collections.unmodifiableList(lnnPm.getMeasuredNodes());

        int numVars = this.variableNodes.size();
        this.edgeCoef = new SparseDoubleMatrix2D(numVars, numVars);

        this.freeParameters = new ArrayList<Parameter>();
        this.freeMappings = new ArrayList<LnnMapping>();
        this.fixedParameters = new ArrayList<Parameter>();
        this.fixedMappings = new ArrayList<LnnMapping>();
        this.distributions = new HashMap<Node, Distribution>();

        processCoefficientParameters();
        processDistributionParameters();

        initializeCoefValues();
    }

    /**
     * Copy constructor.
     *
     * @throws RuntimeException if the given SemIm cannot be serialized and
     *                          deserialized correctly.
     */
    public LnnIm(LnnIm semIm) {
        try {

            // We make a deep copy of semIm and then copy all of its fields
            // into this SEM IM. Otherwise, it's just too HARD to make a deep copy!
            // (Complain, complain.) jdramsey 4/20/2005
            LnnIm _semIm = (LnnIm) new MarshalledObject(semIm).get();

            lnnPm = _semIm.lnnPm;
            variableNodes = _semIm.variableNodes;
            measuredNodes = _semIm.measuredNodes;
            freeParameters = _semIm.freeParameters;
            fixedParameters = _semIm.fixedParameters;
            edgeCoef = _semIm.edgeCoef.copy();
            sampleSize = _semIm.sampleSize;
            freeMappings = _semIm.freeMappings;
            fixedMappings = _semIm.fixedMappings;
            standardErrors = _semIm.standardErrors;
            parameterBoundsEnforced = _semIm.parameterBoundsEnforced;
            estimated = _semIm.estimated;
        }
        catch (IOException e) {
            throw new RuntimeException("SemIm could not be deep cloned.", e);
        }
        catch (ClassNotFoundException e) {
            throw new RuntimeException("SemIm could not be deep cloned.", e);
        }
    }

    public static LnnIm retainValues(LnnIm semIm, SemGraph graph) {
        LnnPm newLnnPm = new LnnPm(graph);
        LnnIm newSemIm = new LnnIm(newLnnPm);

        for (Parameter p1 : newSemIm.getLnnPm().getParameters()) {
            Node nodeA = semIm.getLnnPm().getGraph().getNode(p1.getNodeA().getName());
            Node nodeB = semIm.getLnnPm().getGraph().getNode(p1.getNodeB().getName());

            for (Parameter p2 : semIm.getLnnPm().getParameters()) {
                if (p2.getNodeA() == nodeA && p2.getNodeB() == nodeB && p2.getType() == p1.getType()) {
                    newSemIm.setParamValue(p1, semIm.getParamValue(p2));
                }
            }
        }

        newSemIm.sampleSize = semIm.sampleSize;
        return newSemIm;
    }

    /**
     * Generates a simple exemplar of this class to test serialization.
     *
     * @see edu.cmu.TestSerialization
     * @see edu.cmu.tetradapp.util.TetradSerializableUtils
     */
    public static LnnIm serializableInstance() {
        return new LnnIm(LnnPm.serializableInstance());
    }

    //==============================PUBLIC METHODS=========================//

    /**
     * Returns the Digraph which describes the causal structure of the Sem.
     */
    public LnnPm getLnnPm() {
        return this.lnnPm;
    }

    /**
     * Returns an array containing the current values for the free parameters,
     * in the order in which the parameters appear in getFreeParameters(). That
     * is, getFreeParamValues()[i] is the value for getFreeParameters()[i].
     */
    public double[] getFreeParamValues() {
        double[] paramValues = new double[freeMappings().size()];

        for (int i = 0; i < freeMappings().size(); i++) {
            LnnMapping mapping = freeMappings().get(i);
            paramValues[i] = mapping.getValue();
        }

        return paramValues;
    }

    /**
     * Sets the values of the free parameters (in the order in which they appear
     * in getFreeParameters()) to the values contained in the given array. That
     * is, params[i] is the value for getFreeParameters()[i].
     */
    public void setFreeParamValues(double[] params) {
        if (params.length != getNumFreeParams()) {
            throw new IllegalArgumentException("The array provided must be " +
                    "of the same length as the number of free parameters.");
        }

        for (int i = 0; i < freeMappings().size(); i++) {
            LnnMapping mapping = freeMappings().get(i);
            mapping.setValue(params[i]);
        }
    }

    /**
     * Gets the value of a single free parameter, or Double.NaN if the parameter
     * is not in this
     *
     * @throws IllegalArgumentException if the given parameter is not a free
     *                                  parameter in this model.
     */
    public double getParamValue(Parameter parameter) {
        if (parameter == null) {
            throw new NullPointerException();
        }

        if (getFreeParameters().contains(parameter)) {
            int index = getFreeParameters().indexOf(parameter);
            LnnMapping mapping = this.freeMappings.get(index);
            return mapping.getValue();
        }
        else if (getFixedParameters().contains(parameter)) {
            int index = getFixedParameters().indexOf(parameter);
            LnnMapping mapping = this.fixedMappings.get(index);
            return mapping.getValue();
        }

        throw new IllegalArgumentException(
                "Not a parameter in this model: " + parameter);
    }

    /**
     * Sets the value of a single free parameter to the given value.
     *
     * @throws IllegalArgumentException if the given parameter is not a free
     *                                  parameter in this model.
     */
    public void setParamValue(Parameter parameter, double value) {
        if (getFreeParameters().contains(parameter)) {
            // Note this assumes the freeMappings are in the same order as the
            // free parameters.
            int index = getFreeParameters().indexOf(parameter);
            LnnMapping mapping = this.freeMappings.get(index);
            mapping.setValue(value);
        }
        else {
            throw new IllegalArgumentException("That parameter cannot be set in " +
                    "this model: " + parameter);
        }
    }

    /**
     * Sets the value of a single free parameter to the given value.
     *
     * @throws IllegalArgumentException if the given parameter is not a free
     *                                  parameter in this model.
     */
    public void setFixedParamValue(Parameter parameter, double value) {
        if (!getFixedParameters().contains(parameter)) {
            throw new IllegalArgumentException(
                    "Not a fixed parameter in " + "this model: " + parameter);
        }

        // Note this assumes the fixedMappings are in the same order as the
        // fixed parameters.
        int index = getFixedParameters().indexOf(parameter);
        LnnMapping mapping = this.fixedMappings.get(index);
        mapping.setValue(value);
    }

    public double getCoef(Node x, Node y) {
        Parameter parameter = getLnnPm().getCoefficientParameter(x, y);
        return getParamValue(parameter);
    }

    public void setCoef(Node x, Node y, double value) {
        setParamValue(x, y, value);
    }

    public Distribution getDistribution(Node node) {
        return distributions.get(node);
    }

    /**
     * Sets the value of a single free parameter to the given value, where the
     * free parameter is specified by the endpoint nodes of its edge in the
     * graph. Note that coefficient parameters connect elements of
     * getVariableNodes(), whereas variance and covariance parameters connect
     * elements of getExogenousNodes(). (For variance parameters, nodeA and
     * nodeB are the same.)
     *
     * @throws IllegalArgumentException if the given parameter is not a free
     *                                  parameter in this model or if there is
     *                                  no parameter connecting nodeA with nodeB
     *                                  in this model.
     */
    public void setParamValue(Node nodeA, Node nodeB, double value) {
        Parameter parameter = null;

        if (nodeA == nodeB) {
            parameter = getLnnPm().getVarianceParameter(nodeA);
        }

        if (parameter == null) {
            parameter = getLnnPm().getCoefficientParameter(nodeA, nodeB);
        }

        if (parameter == null) {
            parameter = getLnnPm().getCovarianceParameter(nodeA, nodeB);
        }

        if (parameter == null) {
            throw new IllegalArgumentException("There is no parameter in " +
                    "model for an edge from " + nodeA + " to " + nodeB + ".");
        }

        if (!this.getFreeParameters().contains(parameter)) {
            throw new IllegalArgumentException(
                    "Not a free parameter in " + "this model: " + parameter);
        }

        setParamValue(parameter, value);
    }

    /**
     * Returns the (unmodifiable) list of free parameters in the model.
     */

    public List<Parameter> getFreeParameters() {
        return freeParameters;
    }


    /**
     * Returns the number of free parameters.
     */
    public int getNumFreeParams() {
        return getFreeParameters().size();
    }

    /**
     * Returns the (unmodifiable) list of fixed parameters in the model.
     */
    public List<Parameter> getFixedParameters() {
        return this.fixedParameters;
    }

    /**
     * Returns the number of free parameters.
     */
    public int getNumFixedParams() {
        return getFixedParameters().size();
    }

    /**
     * The list of measured and latent nodes for the semPm. (Unmodifiable.)
     */
    public List<Node> getVariableNodes() {
        return variableNodes;
    }

    /**
     * The list of measured nodes for the semPm. (Unmodifiable.)
     */
    public List<Node> getMeasuredNodes() {
        return measuredNodes;
    }

    /**
     * Returns the sample size (that is, the sample size of the CovarianceMatrix
     * provided at construction time).
     */
    public int getSampleSize() {
        return sampleSize;
    }

    /**
     * Returns a copy of the matrix of edge coefficients. Note that
     * edgeCoef[i][j] is the coefficient of the edge from
     * getVariableNodes().get(i) to getVariableNodes().get(j), or 0.0 if this
     * edge is not in the graph. The values of these may be changed, but the
     * array itself may not.
     */
    public DoubleMatrix2D getCoefMatrix() {
        return edgeCoef().copy();
    }

    /**
     * Iterates through all parameters, picking values for them from the
     * distributions that have been set for them.
     */
    private void initializeCoefValues() {
        for (LnnMapping fixedMapping : fixedMappings) {
            Parameter parameter = fixedMapping.getParameter();

            if (parameter.getType() == ParamType.COEF) {
                fixedMapping.setValue(initialValue(parameter));
            }
        }

        for (LnnMapping freeMapping : freeMappings) {
            Parameter parameter = freeMapping.getParameter();

            if (parameter.getType() == ParamType.COEF) {
                freeMapping.setValue(initialValue(parameter));
            }
        }
    }

    public boolean isParameterBoundsEnforced() {
        return parameterBoundsEnforced;
    }

    public void setParameterBoundsEnforced(boolean parameterBoundsEnforced) {
        this.parameterBoundsEnforced = parameterBoundsEnforced;
    }

    public boolean isEstimated() {
        return estimated;
    }

    public void setEstimated(boolean estimated) {
        this.estimated = estimated;
    }

    /**
     * This simulates data by picking random values for the exogenous terms and
     * percolating this information down through the SEM, assuming it is
     * acyclic. Fast for large simulations but hangs for cyclic models.
     */
    public RectangularDataSet simulateData(int sampleSize) {
        List<Node> variables = new LinkedList<Node>();
        List<Node> variableNodes = getVariableNodes();

        for (Node node : variableNodes) {
            ContinuousVariable var = new ContinuousVariable(node.getName());
            variables.add(var);
        }

        RectangularDataSet dataSet = new ColtDataSet(sampleSize, variables);

        // Create some index arrays to hopefully speed up the simulation.
        SemGraph graph = getLnnPm().getGraph();
        List<Node> tierOrdering = graph.getTierOrdering();

        int[] tierIndices = new int[variableNodes.size()];

        for (int i = 0; i < tierIndices.length; i++) {
            tierIndices[i] = variableNodes.indexOf(tierOrdering.get(i));
        }

        int[][] _parents = new int[variables.size()][];

        for (int i = 0; i < variableNodes.size(); i++) {
            Node node = variableNodes.get(i);
            List<Node> parents = graph.getParents(node);

            for (Iterator<Node> j = parents.iterator(); j.hasNext();) {
                Node _node = j.next();

                if (_node.getNodeType() == NodeType.ERROR) {
                    j.remove();
                }
            }

            _parents[i] = new int[parents.size()];

            for (int j = 0; j < parents.size(); j++) {
                Node _parent = parents.get(j);
                _parents[i][j] = variableNodes.indexOf(_parent);
            }
        }

        // Do the simulation.
        for (int row = 0; row < sampleSize; row++) {
            for (int i = 0; i < tierOrdering.size(); i++) {
                int col = tierIndices[i];

                Node node = tierOrdering.get(i);

                //adding the error term to current node
                Distribution distribution = getDistribution(node);
                double value = distribution.nextRandom();

                for (int j = 0; j < _parents[col].length; j++) {
                    int parent = _parents[col][j];
                    value += dataSet.getDouble(row, parent) *
                            edgeCoef.get(parent, col);
                }

                dataSet.setDouble(row, col, value);
            }
        }

        return dataSet;
    }

    /**
     * Returns a string representation of the Sem (pretty detailed).
     */
    public String toString() {
        StringBuffer buf = new StringBuffer();
        buf.append("\nSem");

        buf.append("\n\n\tVariable nodes:\n");
        buf.append("\t");
        buf.append(getVariableNodes());

        buf.append("\n\n\tmeasuredNodes:\n");
        buf.append("\t");
        buf.append(getMeasuredNodes());

        buf.append("\n\n\tedgeCoef:\n");
        buf.append(MatrixUtils.toString(edgeCoef().toArray()));

        buf.append("\n\n\tsampleSize = ");
        buf.append("\t");
        buf.append(this.sampleSize);

        buf.append("\n\n\tfree mappings:\n");
        for (int i = 0; i < this.freeMappings.size(); i++) {
            LnnMapping iMapping = this.freeMappings.get(i);
            buf.append("\n\t");
            buf.append(i);
            buf.append(". ");
            buf.append(iMapping);
        }

        buf.append("\n\n\tfixed mappings:\n");
        for (int i = 0; i < this.fixedMappings.size(); i++) {
            LnnMapping iMapping = this.fixedMappings.get(i);
            buf.append("\n\t");
            buf.append(i);
            buf.append(". ");
            buf.append(iMapping);
        }

        return buf.toString();
    }

    //==============================PRIVATE METHODS========================//

    private void processCoefficientParameters() {
        SemGraph graph = getLnnPm().getGraph();

        for (Parameter parameter : getLnnPm().getParameters()) {
            if (parameter.getType() == ParamType.COEF) {
                Node nodeA = graph.getVarNode(parameter.getNodeA());
                Node nodeB = graph.getVarNode(parameter.getNodeB());

                int i = getVariableNodes().indexOf(nodeA);
                int j = getVariableNodes().indexOf(nodeB);

                LnnMapping mapping =
                        new LnnMatrixMapping(this, parameter, edgeCoef(), i, j);

                if (parameter.isFixed()) {
                    fixedParameters.add(parameter);
                    fixedMappings.add(mapping);
                }
                else {
                    freeParameters.add(parameter);
                    freeMappings.add(mapping);
                }
            }

        }
    }

    private void processDistributionParameters() {
        SemGraph graph = getLnnPm().getGraph();

        for (Node node : graph.getNodes()) {
            if (node.getNodeType() == NodeType.ERROR) {
                continue;
            }

            DistributionType _type = lnnPm.getDistributionType(node);
            Distribution distribution = getDefaultDistribution(_type);
            distributions.put(node, distribution);

            List<Parameter> _parameters = lnnPm.getDistributionParameters(node);

            for (int i = 0; i < _parameters.size(); i++) {
                LnnDistributionMapping mapping = new LnnDistributionMapping(distribution, i, _parameters.get(i));
                freeParameters.add(_parameters.get(i));
                freeMappings.add(mapping);
            }
        }

    }

    /**
     * Returns a random value from the appropriate distribution for the given
     * parameter.
     */
    private double initialValue(Parameter parameter) {
        if (parameter.isInitializedRandomly()) {
            return parameter.getDistribution().nextRandom();
        }
        else {
            return parameter.getStartingValue();
        }
    }

    /**
     * Returns the (unmodifiable) list of parameters (type Param).
     */
    private List<LnnMapping> freeMappings() {
        return this.freeMappings;
    }

    private Distribution getDefaultDistribution(DistributionType distributionType) {
        if (distributionType == DistributionType.NORMAL) {
            return new NormalDistribution(0, 1);
        }
        else if (distributionType == DistributionType.UNIFORM) {
            return new UniformDistribution(-1, 1);
        }
        else if (distributionType == DistributionType.BETA) {
            return new BetaDistribution(.2, .3);
        } else if (distributionType == DistributionType.GAUSSIAN_POWER) {
            return new GaussianPower(2);
        }
        else if (distributionType == DistributionType.GAUSSIAN_POWER) {
            return new GaussianPower(2);
        }

        throw new IllegalArgumentException();
    }

    private DoubleMatrix2D edgeCoef() {
        return this.edgeCoef;
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

        if (lnnPm == null) {
            throw new NullPointerException();
        }

        if (variableNodes == null) {
            throw new NullPointerException();
        }

        if (measuredNodes == null) {
            throw new NullPointerException();
        }

        if (freeParameters == null) {
            throw new NullPointerException();
        }

        if (freeMappings == null) {
            throw new NullPointerException();
        }

        if (fixedParameters == null) {
            throw new NullPointerException();
        }

        if (fixedMappings == null) {
            throw new NullPointerException();
        }

        if (sampleSize < 0) {
            throw new IllegalArgumentException(
                    "Sample size out of range: " + sampleSize);
        }
    }
}
