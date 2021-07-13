package edu.cmu.tetradapp.model;

import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.session.SessionModel;

/**
 * Wraps a data model so that a random sample will automatically be drawn on
 * construction from a LnnIm.
 *
 * @author Joseph Ramsey jdramsey@andrew.cmu.edu
 * @version $Revision: 5766 $
 */
public class LnnDataWrapper extends DataWrapper implements SessionModel {
    static final long serialVersionUID = 23L;

    //==============================CONSTRUCTORS=============================//

    public LnnDataWrapper(LnnImWrapper wrapper, LnnDataParams params) {
        int sampleSize = params.getSampleSize();
        RectangularDataSet columnDataModel =
                wrapper.getLnnIm().simulateData(sampleSize);
        this.setDataModel(columnDataModel);
        this.setSourceGraph(wrapper.getLnnIm().getLnnPm().getGraph());
    }

    /**
     * Generates a simple exemplar of this class to test serialization.
     *
     * @see edu.cmu.TestSerialization
     * @see edu.cmu.tetradapp.util.TetradSerializableUtils
     */
    public static DataWrapper serializableInstance() {
        return new LnnDataWrapper(LnnImWrapper.serializableInstance(),
                LnnDataParams.serializableInstance());
    }
}
