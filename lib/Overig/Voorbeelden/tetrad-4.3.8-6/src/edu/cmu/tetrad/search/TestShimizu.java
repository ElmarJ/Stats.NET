package edu.cmu.tetrad.search;

import edu.cmu.tetrad.data.DataParser;
import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.graph.Edge;
import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.graph.Node;
import edu.cmu.tetrad.sem.ParamType;
import edu.cmu.tetrad.sem.Parameter;
import edu.cmu.tetrad.sem.SemIm;
import edu.cmu.tetrad.sem.SemPm;
import junit.framework.Test;
import junit.framework.TestCase;
import junit.framework.TestSuite;

import java.io.File;
import java.io.IOException;

/**
 * Tests the ability to get SEM and LNN models out of the Shimizu search.
 *
 * @author Joseph Ramsey
 */
public class TestShimizu extends TestCase {
    public TestShimizu(String name) {
        super(name);
    }

    public void test1() {
        try {
            DataParser parser = new DataParser();
            parser.setMaxIntegralDiscrete(0);

//            RectangularDataSet data = parser.parseTabular(new File("test_data/eigen4c.csv.dat"));
//            RectangularDataSet data = parser.parseTabular(new File("test_data/g1set.txt"));
//            data = new RegressionInterpolator().filter(data);
            RectangularDataSet data = parser.parseTabular(new File("test_data/SAHDMod.dat"));


            ShimizuResult result = Shimizu2006SearchCleanup.lingamDiscoveryDag(data);
            System.out.println(result);

            Graph graph = result.getGraph();
            SemPm semPm = new SemPm(graph);
            SemIm semIm = new SemIm(semPm);

            for (Parameter parameter : semPm.getParameters()) {
                if (parameter.getType() == ParamType.COEF) {
                    Node node1 = parameter.getNodeA();
                    Node node2 = parameter.getNodeB();
                    Edge edge = graph.getEdge(node1, node2);
                    double weight = result.getWeight(edge);
                    semIm.setEdgeCoef(node1, node2, weight);
                }
            }

            System.out.println(semIm);
        }
        catch (IOException e) {
            e.printStackTrace();
            fail("Couldn't load file.");
        }
    }


    public static Test suite() {
        return new TestSuite(TestShimizu.class);
    }
}
