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

package edu.cmu.tetradapp.model;

import edu.cmu.tetrad.data.Knowledge;
import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.search.IndTestType;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.util.ArrayList;
import java.util.List;

/**
 * Stores the parameters needed for the PC search and wizard.
 *
 * @author Raul Salinas
 * @author Joseph Ramsey
 * @version 1.0
 */
public final class BasicSearchParams implements SearchParams {
    static final long serialVersionUID = 23L;

    /**
     * @serial Cannot be null.
     */
    private Knowledge knowledge = new Knowledge();

    /**
     * @serial Cannot be null. (?)
     */
    private List varNames;

    /**
     * @serial Cannot be null.
     */
    private IndTestParams indTestParams = new BasicIndTestParams();

    /**
     * @serial Cannot be null.
     */
    private IndTestType testType = IndTestType.DEFAULT;

    /**
     * @serial May be null.
     */
    private Graph sourceGraph;

    //=============================CONSTUCTORS===========================//

    /**
     * Constructs a new parameter object. Must have a blank constructor.
     */
    public BasicSearchParams() {
        this.varNames = new ArrayList();
    }

    /**
     * Generates a simple exemplar of this class to test serialization.
     *
     * @see edu.cmu.TestSerialization
     * @see edu.cmu.tetradapp.util.TetradSerializableUtils
     */
    public static BasicSearchParams serializableInstance() {
        return new BasicSearchParams();
    }

    //=============================PUBLIC METHODS========================//

    /**
     * Sets a new knowledge for the algorithm.
     */
    public void setKnowledge(Knowledge knowledge) {
        if (knowledge == null) {
            throw new NullPointerException("Cannot set a null knowledge.");
        }

        this.knowledge = new Knowledge(knowledge);
    }

    public Knowledge getKnowledge() {
        return new Knowledge(this.knowledge);
    }

    public IndTestParams getIndTestParams() {
        return this.indTestParams;
    }

    public void setIndTestParams2(IndTestParams indTestParams) {
        if (indTestParams == null) {
            throw new NullPointerException();
        }
        this.indTestParams = indTestParams;
    }

    public List getVarNames() {
        return this.varNames;
    }

    public void setVarNames(List varNames) {
        if (varNames == null) {
            throw new NullPointerException();
        }

        this.varNames = varNames;
    }

    public Graph getSourceGraph() {
        return this.sourceGraph;
    }

    public void setSourceGraph(Graph graph) {
        this.sourceGraph = graph;
    }

    public void setIndTestType(IndTestType testType) {
        if (testType == null) {
            throw new NullPointerException();
        }

        this.testType = testType;
    }

    public IndTestType getIndTestType() {
        return this.testType;
    }

    public String toString() {
        StringBuffer buf = new StringBuffer();
        buf.append("BasicSearchParams:");
        buf.append("\n\tindTestParams = ").append(indTestParams);

        return buf.toString();
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

        if (knowledge == null) {
            throw new NullPointerException();
        }

        if (varNames == null) {
            throw new NullPointerException();
        }

        if (indTestParams == null) {
            throw new NullPointerException();
        }

        if (testType == null) {
            throw new NullPointerException();
        }
    }
}


