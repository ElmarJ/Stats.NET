package edu.cmu.tetradapp.model;

import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.sem.LnnIm;
import edu.cmu.tetrad.session.SessionModel;
import edu.cmu.tetrad.util.TetradLogger;
import edu.cmu.tetrad.util.TetradLoggerConfig;

import java.io.IOException;
import java.io.ObjectInputStream;

/**
 * Wraps a Bayes Pm for use in the Tetrad application.
 *
 * @author Joseph Ramsey
 * @version $Revision: 5766 $ $Date: 2006-01-06 23:02:37 -0500 (Fri, 06 Jan
 *          2006) $
 */
public class LnnImWrapper implements SessionModel, GraphSource {
    static final long serialVersionUID = 23L;

    /**
     * @serial Can be null.
     */
    private String name;

    /**
     * @serial Cannot be null.
     */
    private final LnnIm lnnIm;

    //============================CONSTRUCTORS==========================//

    public LnnImWrapper(LnnPmWrapper lnnPmWrapper) {
        if (lnnPmWrapper == null) {
            throw new NullPointerException("SemPmWrapper must not be null.");
        }

        this.lnnIm = new LnnIm(lnnPmWrapper.getLnnPm());
        log(lnnIm);
    }

    /**
     * Generates a simple exemplar of this class to test serialization.
     *
     * @see edu.cmu.TestSerialization
     * @see edu.cmu.tetradapp.util.TetradSerializableUtils
     */
    public static LnnImWrapper serializableInstance() {
        return new LnnImWrapper(LnnPmWrapper.serializableInstance());
    }

    //===========================PUBLIC METHODS=========================//

    public LnnIm getLnnIm() {
        return this.lnnIm;
    }


    public Graph getGraph() {
        return lnnIm.getLnnPm().getGraph();
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    //======================== Private methods =======================//

    private void log(LnnIm im) {
        TetradLoggerConfig config = TetradLogger.getInstance().getTetradLoggerConfigForModel(this.getClass());
        if (config != null) {
            TetradLogger.getInstance().setTetradLoggerConfig(config);
            TetradLogger.getInstance().info("IM type = SEM");
            TetradLogger.getInstance().log("im", im.toString());
            TetradLogger.getInstance().reset();
        }
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

        if (lnnIm == null) {
            throw new NullPointerException();
        }
    }

}
