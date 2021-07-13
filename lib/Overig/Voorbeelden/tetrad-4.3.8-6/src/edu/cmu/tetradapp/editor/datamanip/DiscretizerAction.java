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

package edu.cmu.tetradapp.editor.datamanip;

import edu.cmu.tetrad.data.DataModel;
import edu.cmu.tetradapp.editor.DataEditor;

import javax.swing.*;
import java.awt.event.ActionEvent;

/**
 * Discretizes selected columns in a data set.
 *
 * @author Joseph Ramsey
 */
public final class DiscretizerAction extends AbstractAction {

    /**
     * The data editor.                         -
     */
    private DataEditor dataEditor;

    /**
     * Creates new action to discretize columns.
     */
    public DiscretizerAction(DataEditor editor) {
        super("Discretize Selected Columns");

        if (editor == null) {
            throw new NullPointerException();
        }

        this.dataEditor = editor;
    }

    /**
     * Performs the action of loading a session from a file.
     */
    public void actionPerformed(ActionEvent e) {
        DataModel selectedDataModel = dataEditor.getSelectedDataModel();

//        if (selectedDataModel instanceof RectangularDataSet) {
//            List<Node> selectedVariables = new LinkedList<Node>();
//
//            RectangularDataSet dataSet = (RectangularDataSet) selectedDataModel;
//            int numColumns = dataSet.getNumColumns();
//
//            for (int i = 0; i < numColumns; i++) {
//                Node variable = dataSet.getVariable(i);
//
//                if (dataSet.isSelected(variable)) {
//                    selectedVariables.add(variable);
//                }
//            }
//
//            if (dataSet.getNumRows() == 0) {
//                JOptionPane.showMessageDialog(JOptionUtils.centeringComp(),
//                        "Data set is empty.");
//                return;
//            }
//
//            if (selectedVariables.isEmpty()) {
//                selectedVariables.addAll(dataSet.getVariables());
//            }
//
//            DataWrapper dataWrapper = dataEditor.getDataWrapper();
//            Map discretizationSpecs = dataWrapper.getDiscretizationSpecs();
//            final DiscretizationParamsEditor editor = new DiscretizationParamsEditor(
//                    dataSet, selectedVariables, discretizationSpecs);
//
//            JComponent comp = JOptionUtils.centeringComp();
//            JFrame ancestor = (JFrame) SwingUtilities.getAncestorOfClass(
//                    JFrame.class, comp);
//
//
//            final JDialog dialog =
//                    new JDialog(ancestor, "Column Discretizer", true);
//            dialog.getContentPane().add(editor, BorderLayout.CENTER);
//            dialog.pack();
//            dialog.setResizable(false);
//            dialog.setLocationRelativeTo(ancestor);
//
//            editor.addPropertyChangeListener(new PropertyChangeListener() {
//                public void propertyChange(PropertyChangeEvent evt) {
//                    if ("cancel".equals(evt.getPropertyName())) {
//                        dialog.setVisible(false);
//                        dialog.dispose();
//                    }
//                    else if ("discretize".equals(evt.getPropertyName())) {
//                        dialog.setVisible(false);
//                        dialog.dispose();
//
//                        RectangularDataSet dataSet =
//                                editor.getDiscretizedDataSet();
//                        DataModelList list = new DataModelList();
//                        list.add(dataSet);
//                        getDataEditor().reset(list);
//                        getDataEditor().selectLastTab();
//                    }
//                }
//            });
//
//            dialog.setVisible(true);
//        }
//        else {
//            JOptionPane.showMessageDialog(JOptionUtils.centeringComp(),
//                    "Requires a tabular data set.");
//        }
    }

    private DataEditor getDataEditor() {
        return dataEditor;
    }
}


