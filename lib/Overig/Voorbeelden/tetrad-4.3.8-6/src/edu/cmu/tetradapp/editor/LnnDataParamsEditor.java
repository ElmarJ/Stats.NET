package edu.cmu.tetradapp.editor;

import edu.cmu.tetrad.model.Params;
import edu.cmu.tetradapp.model.LnnDataParams;
import edu.cmu.tetradapp.util.IntTextField;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

/**
 * Edits the parameters for simulating data from OldSem nets.
 *
 * @author Joseph Ramsey
 * @version $Revision: 5985 $ $Date: 2006-01-08 23:44:17 -0500 (Sun, 08 Jan
 *          2006) $
 */
public class LnnDataParamsEditor extends JPanel implements ParameterEditor {

    /**
     * The parameters object being edited.
     */
    private LnnDataParams params = null;

    /**
     * Constructs a dialog to edit the given workbench OldLnn simulation
     * getMappings object.
     */
    public LnnDataParamsEditor() {
    }

    public void setParams(Params params) {
        if (params == null) {
            throw new NullPointerException();
        }

        this.params = (LnnDataParams) params;
    }

    public void setParentModels(Object[] parentModels) {
        // Do nothing.
    }

    public void setup() {

        // set up text and ties them to the parameters object being edited.
        IntTextField sampleSizeField = new IntTextField(getParams().getSampleSize(), 4);
        sampleSizeField.setFilter(new IntTextField.Filter() {
            public int filter(int value, int oldValue) {
                try {
                    getParams().setSampleSize(value);
                    return value;
                }
                catch (Exception e) {
                    return oldValue;
                }
            }
        });
        JCheckBox box = new JCheckBox("Include Latent Variables ");
        box.setHorizontalTextPosition(SwingConstants.LEFT);
        box.addActionListener(new ActionListener(){
            public void actionPerformed(ActionEvent e) {
                JCheckBox b = (JCheckBox)e.getSource();
                getParams().setIncludeLatents(b.isSelected());
            }
        });

        setLayout(new BorderLayout());

        // continue workbench construction.
        Box b6 = Box.createVerticalBox();
        Box b1 = Box.createHorizontalBox();
        Box hBox = Box.createHorizontalBox();

        b1.add(new JLabel("Sample size:  "));
        b1.add(Box.createHorizontalGlue());
        b1.add(sampleSizeField);

        hBox.add(box);
        hBox.add(Box.createHorizontalGlue());

        b6.add(b1);
    //    b6.add(Box.createVerticalStrut(5));
     //   b6.add(hBox);
        b6.add(Box.createVerticalGlue());
        add(b6, BorderLayout.CENTER);
    }

    public boolean mustBeShown() {
        return true;
    }

    /**
     * Returns the getMappings object being edited. (This probably should not be
     * public, but it is needed so that the textfields can edit the model.)
     *
     * @return the stored simulation parameters model.
     */
    private synchronized LnnDataParams getParams() {
        return this.params;
    }
}
