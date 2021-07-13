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

package edu.cmu.tetrad.data;


/**
 * Returns a new data set in which cases in the given data set that have been
 * assigned multiplicies other than n = 1 are copied out n times. This increases
 * the number of rows in the data set.
 *
 * @author Joseph Ramsey
 * @version $Revision: 5003 $ $Date: 2006-01-09 11:04:56 -0500 (Mon, 09 Jan
 *          2006) $
 */
public final class CaseExpander implements DataFilter {
    public final RectangularDataSet filter(RectangularDataSet dataSet) {
//        if (dataSet.isDiscrete()) {
//            return expandDiscrete(dataSet);
//        }
//        else if (dataSet.isContinuous()) {
//            return expandContinuous(dataSet);
//        }

        return expand(dataSet);

//        throw new RuntimeException(
//                "Can only expand continuous or " + "discrete data sets.");
    }

    private static RectangularDataSet expand(RectangularDataSet dataSet) {
        int rows = -1;

        for (int i = 0; i < dataSet.getNumRows(); i++) {
            int caseMultiplier = dataSet.getMultiplier(i);

            for (int k = 0; k < caseMultiplier; k++) {
                ++rows;
            }
        }

        RectangularDataSet newDataSet =
                new ColtDataSet(rows, dataSet.getVariables());
        int cols = dataSet.getNumColumns();
        int index = -1;

        for (int i = 0; i < dataSet.getNumRows(); i++) {
            int caseMultiplier = dataSet.getMultiplier(i);

            for (int k = 0; k < caseMultiplier; k++) {
                ++index;

                for (int j = 0; j < cols; j++) {
                    if (dataSet.getVariable(j) instanceof ContinuousVariable) {
                        newDataSet.setDouble(index, j, dataSet.getDouble(i, j));
                    }
                    else if (dataSet.getVariable(j) instanceof DiscreteVariable) {
                        newDataSet.setInt(index, j, dataSet.getInt(i, j));
                    }
                    else {
                        throw new IllegalStateException("Expecting either a " +
                                "continuous or a discrete variable.");
                    }

                }
            }
        }

        return newDataSet;
    }

//    private static RectangularDataSet expandContinuous(
//            RectangularDataSet dataSet) {
//        int rows = -1;
//
//        for (int i = 0; i < dataSet.getNumRows(); i++) {
//            int caseMultiplier = dataSet.getMultiplier(i);
//
//            for (int k = 0; k < caseMultiplier; k++) {
//                ++rows;
//            }
//        }
//
//        RectangularDataSet newDataSet =
//                new ColtDataSet(rows, dataSet.getVariables());
//        int cols = dataSet.getNumColumns();
//        int index = -1;
//
//        for (int i = 0; i < dataSet.getNumRows(); i++) {
//            int caseMultiplier = dataSet.getMultiplier(i);
//
//            for (int k = 0; k < caseMultiplier; k++) {
//                ++index;
//
//                for (int j = 0; j < cols; j++) {
//                    newDataSet.setDouble(index, j, dataSet.getDouble(i, j));
//                }
//            }
//        }
//
//        return newDataSet;
//    }

//    private static RectangularDataSet expandDiscrete(
//            RectangularDataSet dataSet) {
//        int rows = -1;
//
//        for (int i = 0; i < dataSet.getNumRows(); i++) {
//            int caseMultiplier = dataSet.getMultiplier(i);
//
//            for (int k = 0; k < caseMultiplier; k++) {
//                ++rows;
//            }
//        }
//
//        RectangularDataSet newDataSet =
//                new ColtDataSet(rows, dataSet.getVariables());
//        int cols = dataSet.getNumColumns();
//        int index = -1;
//
//        for (int i = 0; i < dataSet.getNumRows(); i++) {
//            int caseMultiplier = dataSet.getMultiplier(i);
//
//            for (int k = 0; k < caseMultiplier; k++) {
//                ++index;
//
//                for (int j = 0; j < cols; j++) {
//                    newDataSet.setInt(index, j, dataSet.getInt(i, j));
//                }
//            }
//        }
//
//        return newDataSet;
//    }
}


