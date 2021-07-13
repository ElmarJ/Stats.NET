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

package edu.cmu.tetrad.graph;

import junit.framework.Test;
import junit.framework.TestCase;
import junit.framework.TestSuite;

/**
 * Tests the functions of EndpointMatrixGraph and EdgeListGraph through the
 * Graph interface.
 *
 * @author Joseph Ramsey
 * @version $Revision: 5549 $ $Date: 2005-03-07 13:55:02 -0500 (Mon, 07 Mar
 *          2005) $
 */
public final class TestGraphUtils extends TestCase {

    /**
     * Standard constructor for JUnit test cases.
     */
    public TestGraphUtils(String name) {
        super(name);
    }

    public void testCreateRandomDag() {
        //        while (true) {
        Dag dag = GraphUtils.createRandomDag(50, 0, 50, 4, 3, 3, false);
        System.out.println(dag);
        //        }
    }

    //    public void rtestMaxPathLength() {
    //        int numTests = 10;
    //        int n = 40;
    //        int k = 80;
    //
    //        System.out.println("numTests = " + numTests);
    //        System.out.println("n = " + n);
    //        System.out.println("k = " + k);
    //
    //        int sum = 0;
    //        int min = Integer.MAX_VALUE;
    //        int max = 0;
    //
    //        for (int i = 0; i < numTests; i++) {
    //            Dag dag = GraphUtils.createRandomDagC(n, 0, k);
    //            List tiers = dag.getTiers();
    //            sum += tiers.size();
    //            if (tiers.size() < min) {
    //                min = tiers.size();
    //            }
    //            if (tiers.size() > max) {
    //                max = tiers.size();
    //            }
    //        }
    //
    //        double ave = sum / (double) numTests;
    //
    //        System.out.println("OLD: Min = " + min + ", Max = " + max +
    //                ", average = " + ave);
    //
    //        sum = max = 0;
    //        min = Integer.MAX_VALUE;
    //
    //        for (int i = 0; i < numTests; i++) {
    //            Dag dag = GraphUtils.createRandomDagB(n, 0, k, 0.0, 0.0, 0.0);
    //            List tiers = dag.getTiers();
    //            sum += tiers.size();
    //            if (tiers.size() < min) {
    //                min = tiers.size();
    //            }
    //            if (tiers.size() > max) {
    //                max = tiers.size();
    //            }
    //        }
    //
    //        ave = sum / (double) numTests;
    //
    //        System.out.println("1: Min = " + min + ", Max = " + max +
    //                ", average = " + ave);
    //
    //        sum = max = 0;
    //        min = Integer.MAX_VALUE;
    //        int totK = 0;
    //
    //        for (int i = 0; i < numTests; i++) {
    ////            System.out.print(".");
    //            Dag dag = GraphUtils.createRandomDagC(n, 0, k, 0.0, 0.0, 0.0);
    //            System.out.println("test " + (i + 1) + ": num edges = " + dag.getNumEdges());
    //            System.out.flush();
    //
    //            List tiers = dag.getTiers();
    //            sum += tiers.size();
    //            if (tiers.size() < min) {
    //                min = tiers.size();
    //            }
    //            if (tiers.size() > max) {
    //                max = tiers.size();
    //            }
    //
    //            totK += dag.getNumEdges();
    //        }
    //
    //        ave = sum / (double) numTests;
    //
    //        System.out.println("\n2: Min = " + min + ", Max = " + max +
    //                ", average = " + ave + ", avenumedges = " + totK / (double) numTests);
    //    }

    /**
     * This method uses reflection to collect up all of the test methods from
     * this class and return them to the test runner.
     */
    public static Test suite() {

        // Edit the name of the class in the parens to match the name
        // of this class.
        return new TestSuite(TestGraphUtils.class);
    }
}


