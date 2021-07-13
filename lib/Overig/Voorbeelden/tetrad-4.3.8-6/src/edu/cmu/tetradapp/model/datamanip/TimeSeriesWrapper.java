package edu.cmu.tetradapp.model.datamanip;

import edu.cmu.tetrad.data.*;
import edu.cmu.tetrad.graph.Node;
import edu.cmu.tetradapp.model.DataWrapper;

import java.util.ArrayList;
import java.util.List;

/**
 * @author Tyler
 * @version $Revision: 1.1 $ $Date: Jan 27, 2007 11:10:09 PM $
 */
public class TimeSeriesWrapper extends DataWrapper {
    static final long serialVersionUID = 23L;

    /**
     * Constructs a new time series dataset.
     *
     * @param data   - Previous data (from the parent node)
     * @param params - The parameters.
     */
    public TimeSeriesWrapper(DataWrapper data, TimeSeriesParams params) {
        DataModel model = data.getSelectedDataModel();
        if (!(model instanceof RectangularDataSet)) {
            throw new IllegalArgumentException("The data model must be a rectangular dataset");
        }
        model = createTimeSeriesData((RectangularDataSet) model, params.getNumOfTimeLags());
        this.setDataModel(model);
        this.setSourceGraph(data.getSourceGraph());
    }


    /**
     * Generates a simple exemplar of this class to test serialization.
     *
     * @see edu.cmu.TestSerialization
     * @see edu.cmu.tetradapp.util.TetradSerializableUtils
     */
    public static DataWrapper serializableInstance() {
        return new TimeSeriesWrapper(DataWrapper.serializableInstance(),
                TimeSeriesParams.serializableInstance());
    }

    //=============================== Private Methods =========================//


    /**
     * Creates new time series dataset from the given one (fixed to deal with mixed datasets)
     */
    private static DataModel createTimeSeriesData(RectangularDataSet data, int numOfLags) {
        List<Node> variables = data.getVariables();
        int dataSize = variables.size();
        int laggedRows = data.getNumRows() - numOfLags + 1;
        Knowledge knowledge = new Knowledge();
        Node[][] laggedNodes = new Node[numOfLags][dataSize];
        List<Node> newVariables = new ArrayList<Node>(numOfLags * dataSize + 1);
        for (int lag = 0; lag < numOfLags; lag++) {
            for (int col = 0; col < dataSize; col++) {
                Node node = variables.get(col);
                String varName = node.getName();
                Node laggedNode;
                if (node instanceof ContinuousVariable) {
                    laggedNode = new ContinuousVariable(varName + "." + (lag + 1));
                } else if (node instanceof DiscreteVariable) {
                    DiscreteVariable var = (DiscreteVariable) node;
                    laggedNode = new DiscreteVariable(var);
                    var.setName(varName + "." + (lag + 1));
                } else {
                    throw new IllegalStateException("Node must be either continuous or discrete");
                }
                newVariables.add(laggedNode);
                laggedNode.setCenter(80 * col + 50, 80 * (numOfLags - lag) + 50);
                laggedNodes[lag][col] = laggedNode;
                knowledge.addToTier(lag, laggedNode.getName());
            }
        }
        RectangularDataSet laggedData = new ColtDataSet(laggedRows, newVariables);
        for (int lag = 0; lag < numOfLags; lag++) {
            for (int col = 0; col < dataSize; col++) {
                for (int row = 0; row < laggedRows; row++) {
                    Node laggedNode = laggedNodes[lag][col];
                    if (laggedNode instanceof ContinuousVariable) {
                        double value = data.getDouble(row + lag, col);
                        laggedData.setDouble(row, col + lag * dataSize, value);
                    } else {
                        int value = data.getInt(row + lag, col);
                        laggedData.setInt(row, col + lag * dataSize, value);
                    }
                }
            }
        }

        knowledge.setDefaultToKnowledgeLayout(true);
        laggedData.setKnowledge(knowledge);
        return laggedData;
    }

}
