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

package edu.cmu.tetradapp.app;

import edu.cmu.tetrad.util.JOptionUtils;

import javax.swing.*;
import java.awt.datatransfer.Clipboard;
import java.awt.datatransfer.ClipboardOwner;
import java.awt.datatransfer.Transferable;
import java.awt.event.ActionEvent;

/**
 * Deletes a selection of session nodes from the frontmost session editor.
 *
 * @author Joseph Ramsey jdramsey@andrew.cmu.edu
 * @version $Revision: 4524 $ $Date: 2006-01-06 15:57:49 -0500 (Fri, 06 Jan
 *          2006) $
 */
final class DeleteSubsessionAction extends AbstractAction
        implements ClipboardOwner {

    /**
     * The desktop containing the target session editor.
     */
    private TetradDesktop desktop;

    /**
     * Creates a new delete subsession action for the given desktop and
     * clipboard.
     */
    public DeleteSubsessionAction() {
        super("Delete");
    }

    /**
     * Copies a parentally closed selection of session nodes in the frontmost
     * session editor to the clipboard.
     */
    public void actionPerformed(ActionEvent e) {
        SessionEditor sessionEditor = this.desktop.getFrontmostSessionEditor();
        SessionEditorWorkbench graph = sessionEditor.getSessionWorkbench();

        int ret = JOptionPane.showConfirmDialog(JOptionUtils.centeringComp(),
                "Delete nodes?", "Confirm", JOptionPane.OK_CANCEL_OPTION);

        if (ret == JOptionPane.OK_OPTION) {
            graph.deleteSelectedObjects();
        }
    }

    /**
     * Notifies this object that it is no longer the owner of the contents of
     * the clipboard.
     *
     * @param clipboard the clipboard that is no longer owned
     * @param contents  the contents which this owner had placed on the
     *                  clipboard
     */
    public void lostOwnership(Clipboard clipboard, Transferable contents) {
    }
}

