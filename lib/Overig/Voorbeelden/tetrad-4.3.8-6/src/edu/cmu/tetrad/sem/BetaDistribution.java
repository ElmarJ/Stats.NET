package edu.cmu.tetrad.sem;

import cern.jet.random.Beta;
import cern.jet.random.engine.MersenneTwister;
import edu.cmu.tetrad.util.Distribution;
import edu.cmu.tetrad.util.PersistentRandomUtil;

/**
 * Wraps a beta _beta, allowing the values of its parameters to
 * be modified, generating random data.
 *
 * @author Joseph Ramsey
 */
public class BetaDistribution implements Distribution {
    static final long serialVersionUID = 23L;

    /**
     * The stored alpha value for the Beta _beta.
     */
    private double alpha = 0.5;

    /**
     * The stored beta value for the Beta _beta.
     */
    private double beta = 0.5;

    /**
     * The wrapped _beta.
     */
    private Beta _beta;

    /**
     * Constructs s new Beta _beta.
     * @param alpha The alpha value for the _beta.
     * @param beta The beta value for the _beta.
     */
    public BetaDistribution(double alpha, double beta) {
        // TODO Check these values.

        _beta = new Beta(alpha, beta, PersistentRandomUtil.getInstance().getRandomEngine());
        this.alpha = alpha;
        this.beta = beta;
    }

    /**
     * Generates a simple exemplar of this class to test serialization.
     *
     * @see edu.cmu.TestSerialization
     * @see edu.cmu.tetradapp.util.TetradSerializableUtils
     */
    public static BetaDistribution serializableInstance() {
        return new BetaDistribution(.5, .5);
    }

    public double nextRandom() {
        return _beta.nextDouble();
    }

    public void setParameter(int index, double value) {
        if (index == 0) {
            alpha = value;
        }
        else if (index == 1 && value >= 0) {
            beta = value;
        }
        else {
            throw new IllegalArgumentException("Illegal value: " + value);
        }

        _beta = new Beta(alpha, beta, new MersenneTwister());
    }

    public double getParameter(int index) {
        if (index == 0) {
            return alpha;
        }
        else if (index == 1) {
            return beta;
        }
        else {
            throw new IllegalArgumentException("Illegal index: " + index);
        }
    }

    public String getParameterName(int index) {
        if (index == 0) {
            return "Alpha";
        }
        else if (index == 1) {
            return "Beta";
        }
        else {
            throw new IllegalArgumentException("Not a parameter index: " + index);
        }
    }

    public int getNumParameters() {
        return 2;
    }


    public String getName() {
        return "Beta";
    }

    public String toString() {
        return "B(" + alpha + ", " + beta + ")";
    }
}
