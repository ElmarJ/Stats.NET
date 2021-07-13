///////////////////////////////////////////////////////////////////////////////
// For information as to what this class does, see the Javadoc, below.       //
// Copyright (C) 2005 by Peter Spirtes, Richard Scheines, Joseph Ramsey,     //
// and Clark Glymour.                                                        //
//                                                                           //
// This program is free software; you can redistribute it and/or modify      //
// it under the terms of the GNU General Public License as published by      //
// the Free Software Foundation; either version 2 of the License, or         //
// (at your option) any later version.                                       //
//                                                                           //
// This program is distributed in the hope that it will be useful,           //
// but WITHOUT ANY WARRANTY; without even the implied warranty of            //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the             //
// GNU General Public License for more details.                              //
//                                                                           //
// You should have received a copy of the GNU General Public License         //
// along with this program; if not, write to the Free Software               //
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA //
///////////////////////////////////////////////////////////////////////////////

package edu.cmu.tetrad.test;

import edu.cmu.tetrad.bayes.BayesPm;
import edu.cmu.tetrad.bayes.MlBayesIm;
import edu.cmu.tetrad.data.DataParser;
import edu.cmu.tetrad.data.DataSavers;
import edu.cmu.tetrad.data.DelimiterType;
import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.graph.Dag;
import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.graph.GraphUtils;
import edu.cmu.tetrad.sem.SemIm;
import edu.cmu.tetrad.sem.SemPm;
import junit.framework.Test;
import junit.framework.TestCase;
import junit.framework.TestSuite;

import java.io.*;

/**
 * Tests data loaders against sample files.
 *
 * @author Joseph Ramsey
 * @version $Revision: 6059 $
 */
public class TestDataLoadersRoundtrip extends TestCase {
    public TestDataLoadersRoundtrip(String name) {
        super(name);
    }

    public void testContinuousRoundtrip() {
        try {
            Graph randomGraph = GraphUtils.createRandomDagC(5, 0, 8);
            SemPm semPm1 = new SemPm(randomGraph);
            SemIm semIm1 = new SemIm(semPm1);
            RectangularDataSet dataSet = semIm1.simulateData(10, false);
            System.out.println(dataSet);

            FileWriter fileWriter = new FileWriter("test_data/roundtrip.dat");
            Writer writer = new PrintWriter(fileWriter);
            DataSavers.saveContinuousData(dataSet, writer, ',');
            writer.close();

            File file = new File("test_data/roundtrip.dat");
            DataParser parser = new DataParser();
            parser.setDelimiter(DelimiterType.COMMA);
            RectangularDataSet _dataSet = parser.parseTabular(file);

            System.out.println(dataSet);
            System.out.println(_dataSet);
            assertTrue(dataSet.equals(_dataSet));
        }
        catch (IOException e) {
            e.printStackTrace();
            fail();
        }
    }

    public void testDiscreteRoundtrip() {
        try {
            for (int i = 0; i < 20; i++) {
                Graph randomGraph = GraphUtils.createRandomDagC(5, 0, 8);
                Dag dag = new Dag(randomGraph);
                BayesPm bayesPm1 = new BayesPm(dag);
                MlBayesIm bayesIm1 = new MlBayesIm(bayesPm1, MlBayesIm.RANDOM);
                RectangularDataSet dataSet = bayesIm1.simulateData(10, false);
                System.out.println(dataSet);

                FileWriter fileWriter =
                        new FileWriter("test_data/roundtrip.dat");
                Writer writer = new PrintWriter(fileWriter);
                DataSavers.saveDiscreteData(dataSet, writer, '\t');
                writer.close();

                File file = new File("test_data/roundtrip.dat");

                DataParser parser = new DataParser();
                parser.setKnownVariables(dataSet.getVariables());
                RectangularDataSet _dataSet = parser.parseTabular(file);

                System.out.println(_dataSet);
                assertTrue(dataSet.equals(_dataSet));
            }
        }
        catch (IOException e) {
            e.printStackTrace();
            fail(e.getMessage());
        }
    }

    /**
     * This method uses reflection to collect up all of the test methods from
     * this class and return them to
     *
     * the test runner.
     */
    public static Test suite() {

        // Edit the name of the class in the parens to match the name
        // of this class.
        return new TestSuite(TestDataLoadersRoundtrip.class);
    }
}

