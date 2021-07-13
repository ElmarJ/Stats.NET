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

import edu.cmu.tetrad.data.Clusters;
import edu.cmu.tetrad.data.DataModel;
import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.session.SessionModel;
import edu.cmu.tetrad.util.Executable;

/**
 * Specifies the methods that all algorithm runners must implement. All
 * algorithm runners must know what their parameters are, must know what their
 * source graph is, and must know what their result graph is (if it has been
 * calculated).
 *
 * @author Joseph Ramsey
 * @version $Revision: 4524 $ $Date: 2005-03-08 12:28:12 -0500 (Tue, 08 Mar
 *          2005) $
 */
public interface MimRunner extends SessionModel, Executable {

    /**
     * Returns the data used to execute this algorithm. Might possibly be a
     * graph.
     */
    DataModel getData();

    /**
     * Returns the search parameters for this algorithm.
     */
    MimParams getParams();

    /**
     * Returns the graph from which data was originally generated, if such a
     * graph is available. Otherwise, returns null.
     */
    Graph getSourceGraph();

    /**
     * Returns the graph that results from executing the algorithm, if the
     * algorithm has been successfully executed.
     */
    Graph getResultGraph();

    /**
     * Returns the clusters that resulted from executing the algorithm, if the
     * algorithm was successfully executed.
     */
    Clusters getClusters();

    /**
     * Returns the resulting strucure graph (that is, graph over latents only),
     * if there is one; otherwise, null.
     */
    Graph getStructureGraph();

    /**
     * Executes the algorithm.
     */
    void execute() throws Exception;
}


