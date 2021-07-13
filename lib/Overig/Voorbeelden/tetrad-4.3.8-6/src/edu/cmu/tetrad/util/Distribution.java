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

package edu.cmu.tetrad.util;

/**
 * Interface for a random distribution, in which the values of individual
 * parameters can be set, and from which random values can be drawn
 * without replacement.
 *
 * @author Joseph Ramsey jdramsey@andrew.cmu.edu
 * @version $Revision: 6017 $
 */
public interface Distribution extends TetradSerializable {
    static final long serialVersionUID = 23L;

    /**
     * Returns the number of parameters.
     * @return The number of parameters.
     */
    int getNumParameters();

    String getName();
    
    /**
     * Sets the index'th parameter to the given value.
     * @param index The index of the parameter.
     * @param value The value of the parameter.
     */
    void setParameter(int index, double value);

    /**
     * Returns the value of the index'th parameter.
     * @param index The index of the parameter.
     * @return The value of the parameter.
     */
    double getParameter(int index);

    /**
     * The name of the index'th parameter, for display purposes.
     * @param index The index of the parameter.
     * @return The name of the index'th parameter.
     */
    String getParameterName(int index);

    /**
     * Returns a new random value drawn from the underlying distribution.
     * @return The next random number drawn from the distribution.
     */
    double nextRandom();
}

