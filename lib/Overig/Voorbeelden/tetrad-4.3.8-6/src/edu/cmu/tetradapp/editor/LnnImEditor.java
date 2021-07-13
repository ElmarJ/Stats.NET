package edu.cmu.tetradapp.editor;

import edu.cmu.tetrad.graph.*;
import edu.cmu.tetrad.sem.LnnIm;
import edu.cmu.tetrad.sem.ParamType;
import edu.cmu.tetrad.sem.Parameter;
import edu.cmu.tetrad.util.NumberFormatUtil;
import edu.cmu.tetradapp.model.LnnImWrapper;
import edu.cmu.tetradapp.util.DoubleTextField;
import edu.cmu.tetradapp.workbench.DisplayNode;
import edu.cmu.tetradapp.workbench.GraphNodeMeasured;
import edu.cmu.tetradapp.workbench.GraphWorkbench;

import javax.swing.*;
import javax.swing.border.TitledBorder;
import java.awt.*;
import java.awt.event.*;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.text.NumberFormat;

/**
 * Edits a SEM instantiated model.
 *
 * @author Donald Crimbchin
 * @author Joseph Ramsey
 */
public final class LnnImEditor extends JPanel {

    /**
     * The SemIm being edited.
     */
    private LnnIm lnnIm;

    /**
     * The graphical editor for the SemIm.
     */
    private LnnImGraphicalEditor lnnImGraphicalEditor;

    /**
     * Maximum number of free parameters for which statistics will be
     * calculated. (Calculating standard errors is high complexity.)
     */
    private int maxFreeParamsForStatistics = 100;

    /**
     * True iff covariance parameters are edited as correlations.
     */
    private boolean editCovariancesAsCorrelations = false;

    /**
     * True iff covariance parameters are edited as correlations.
     */
    private boolean editIntercepts = false;
    private JTabbedPane tabbedPane;
    private String graphicalEditorTitle = "Graphical Editor";
    private String tabularEditorTitle = "TabularEditor";
    private boolean editable = true;
    private JCheckBoxMenuItem meansItem;
    private JCheckBoxMenuItem interceptsItem;

    //========================CONSTRUCTORS===========================//

    public LnnImEditor(LnnIm lnnIm) {
        this(lnnIm, "Graphical Editor", "Tabular Editor");
    }

    /**
     * Constructs an editor for the given SemIm.
     */
    public LnnImEditor(LnnIm lnnIm, String graphicalEditorTitle,
                       String tabularEditorTitle) {
        if (lnnIm == null) {
            throw new NullPointerException("SemIm must not be null.");
        }

        this.lnnIm = lnnIm;
//        lnnIm.getLnnPm().getGraph().setShowErrorTerms(false);
        this.graphicalEditorTitle = graphicalEditorTitle;
        this.tabularEditorTitle = tabularEditorTitle;
        setLayout(new BorderLayout());

        tabbedPane = new JTabbedPane();

        tabbedPane.add(graphicalEditorTitle, graphicalEditor());

        add(tabbedPane, BorderLayout.CENTER);

        JMenuBar menuBar = new JMenuBar();
        JMenu file = new JMenu("File");
        menuBar.add(file);
        file.add(new SaveScreenshot(this, true, "Save Screenshot..."));
        file.add(new SaveComponentImage(lnnImGraphicalEditor.getWorkbench(),
                "Save Graph Image..."));

        JCheckBoxMenuItem covariances =
                new JCheckBoxMenuItem("Show covariances");
        JCheckBoxMenuItem correlations =
                new JCheckBoxMenuItem("Show correlations");

        ButtonGroup correlationGroup = new ButtonGroup();
        correlationGroup.add(covariances);
        correlationGroup.add(correlations);
        covariances.setSelected(true);

        JMenuItem errorTerms = new JMenuItem();

        // By default, hide the error terms.
        getSemGraph().setShowErrorTerms(false);

        if (getSemGraph().isShowErrorTerms()) {
            errorTerms.setText("Hide Error Terms");
        } else {
            errorTerms.setText("Show Error Terms");
        }

        errorTerms.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                JMenuItem menuItem = (JMenuItem) e.getSource();

                if ("Hide Error Terms".equals(menuItem.getText())) {
                    menuItem.setText("Show Error Terms");
                    getSemGraph().setShowErrorTerms(false);
                    graphicalEditor().resetLabels();
                } else if ("Show Error Terms".equals(menuItem.getText())) {
                    menuItem.setText("Hide Error Terms");
                    getSemGraph().setShowErrorTerms(true);
                    graphicalEditor().resetLabels();
                }
            }
        });

//        menuBar.add(graph);

        meansItem = new JCheckBoxMenuItem("Show means");
        interceptsItem = new JCheckBoxMenuItem("Show intercepts");

        ButtonGroup meansGroup = new ButtonGroup();
        meansGroup.add(meansItem);
        meansGroup.add(interceptsItem);
        meansItem.setSelected(true);

        JMenu params = new JMenu("Parameters");
        params.add(errorTerms);
        params.addSeparator();
        params.add(covariances);
        params.add(correlations);
        params.addSeparator();

        menuBar.add(params);

        add(menuBar, BorderLayout.NORTH);
    }

    private SemGraph getSemGraph() {
        return lnnIm.getLnnPm().getGraph();
    }

    /**
     * Constructs a new SemImEditor from the given OldSemEstimateAdapter.
     */
    public LnnImEditor(LnnImWrapper semImWrapper) {
        this(semImWrapper.getLnnIm());
    }

    /**
     * Returns the index of the currently selected tab. Used to construct a new
     * SemImEditor in the same state as a previous one.
     */
    public int getTabSelectionIndex() {
        return tabbedPane.getSelectedIndex();
    }

    /**
     * Sets a new SemIm to edit.
     */
    public void setSemIm(LnnIm lnnIm, int tabSelectionIndex,
                         int matrixSelection) {
        if (lnnIm == null) {
            throw new NullPointerException();
        }

        if (tabSelectionIndex < 0 || tabSelectionIndex >= 4) {
            throw new IllegalArgumentException(
                    "Tab selection must be 0, 1, 2, or 3: " + tabSelectionIndex);
        }

        if (matrixSelection < 0 || matrixSelection >= 4) {
            throw new IllegalArgumentException(
                    "Matrix selection must be 0, 1, 2, or 3: " + matrixSelection);
        }

        Graph oldGraph = this.lnnIm.getLnnPm().getGraph();

        this.lnnIm = lnnIm;
        GraphUtils.arrangeBySourceGraph(lnnIm.getLnnPm().getGraph(), oldGraph);

        this.lnnImGraphicalEditor = null;

        tabbedPane.removeAll();
        tabbedPane.add(getGraphicalEditorTitle(), graphicalEditor());

        tabbedPane.setSelectedIndex(tabSelectionIndex);
        tabbedPane.validate();
    }

    //========================PRIVATE METHODS===========================//

    private LnnIm getLnnIm() {
        return lnnIm;
    }

    private LnnImGraphicalEditor graphicalEditor() {
        if (this.lnnImGraphicalEditor == null) {
            this.lnnImGraphicalEditor = new LnnImGraphicalEditor(getLnnIm(),
                    this, this.maxFreeParamsForStatistics);
            this.lnnImGraphicalEditor.addPropertyChangeListener(
                    new PropertyChangeListener() {
                        public void propertyChange(PropertyChangeEvent evt) {
                            firePropertyChange(evt.getPropertyName(), null,
                                    null);
                        }
                    });
        }
        return this.lnnImGraphicalEditor;
    }

    public boolean isEditCovariancesAsCorrelations() {
        return editCovariancesAsCorrelations;
    }

    public boolean isEditIntercepts() {
        return editIntercepts;
    }

    private String getGraphicalEditorTitle() {
        return graphicalEditorTitle;
    }

    private String getTabularEditorTitle() {
        return tabularEditorTitle;
    }

    public boolean isEditable() {
        return editable;
    }

    public void setEditable(boolean editable) {
        graphicalEditor().setEditable(editable);
        this.editable = editable;
    }
}

/**
 * Edits the parameters of the SemIm using a graph workbench.
 */
final class LnnImGraphicalEditor extends JPanel {

    /**
     * Font size for parameter values in the graph.
     */
    private static Font SMALL_FONT = new Font("Dialog", Font.PLAIN, 10);

    /**
     * Background color of the edit panel when you click on the parameters.
     */
    private static Color LIGHT_YELLOW = new Color(255, 255, 215);

    /**
     * Formats numbers.
     */
    private NumberFormat nf = NumberFormatUtil.getInstance().getNumberFormat();

    /**
     * The SemIM being edited.
     */
    private LnnIm lnnIm;

    /**
     * Workbench for the graphical editor.
     */
    private GraphWorkbench workbench;

    /**
     * Stores the last active edge so that it can be reset properly.
     */
    private Object lastEditedObject = null;

    /**
     * This delay needs to be restored when the component is hidden.
     */
    private int savedTooltipDelay = 0;

    /**
     * The editor that sits inside the SemImEditor that allows the user to edit
     * the SemIm graphically.
     */
    private LnnImEditor editor = null;

    /**
     * True iff this graphical display is editable.
     */
    private boolean editable = true;

    /**
     * Constructs a LnnIm graphical editor for the given LnnIm.
     */
    public LnnImGraphicalEditor(LnnIm lnnIm, LnnImEditor editor,
                                int maxFreeParamsForStatistics) {
        this.lnnIm = lnnIm;
        this.editor = editor;

        setLayout(new BorderLayout());
        JScrollPane scroll = new JScrollPane(workbench());
        scroll.setPreferredSize(new Dimension(450, 450));

        add(scroll, BorderLayout.CENTER);
        setBorder(new TitledBorder("Click parameter values to edit"));

        ToolTipManager toolTipManager = ToolTipManager.sharedInstance();
        setSavedTooltipDelay(toolTipManager.getInitialDelay());

        // Laborious code that follows is intended to make sure tooltips come
        // almost immediately within the lnn im editor but more slowly outside.
        // Ugh.
        workbench().addComponentListener(new ComponentAdapter() {
            public void componentShown(ComponentEvent e) {
                resetLabels();
                ToolTipManager toolTipManager = ToolTipManager.sharedInstance();
                toolTipManager.setInitialDelay(100);
            }

            public void componentHidden(ComponentEvent e) {
                ToolTipManager toolTipManager = ToolTipManager.sharedInstance();
                toolTipManager.setInitialDelay(getSavedTooltipDelay());
            }
        });

        workbench().addMouseListener(new MouseAdapter() {
            public void mouseEntered(MouseEvent e) {
                if (workbench().contains(e.getPoint())) {

                    // Commenting out the resetLabels, since it seems to make
                    // people confused when they can't move the mouse away
                    // from the text field they are editing without the
                    // textfield disappearing. jdramsey 3/16/2005.
//                    resetLabels();
                    ToolTipManager toolTipManager =
                            ToolTipManager.sharedInstance();
                    toolTipManager.setInitialDelay(100);
                }
            }

            public void mouseExited(MouseEvent e) {
                if (!workbench().contains(e.getPoint())) {
                    ToolTipManager toolTipManager =
                            ToolTipManager.sharedInstance();
                    toolTipManager.setInitialDelay(getSavedTooltipDelay());
                }
            }
        });

        // Make sure the graphical editor reflects changes made to parameters
        // in other editors.
        addComponentListener(new ComponentAdapter() {
            public void componentShown(ComponentEvent e) {
                resetLabels();
            }
        });
    }

    //========================PRIVATE METHODS===========================//

    private void beginEdgeEdit(Edge edge) {
    }

    private void beginNodeEdit(Node node) {
//        finishEdit();
//
//        if (!isEditable()) {
//            return;
//        }
//
//        if (node.getNodeType() != NodeType.ERROR) {
//
//        }
//
//        Parameter parameter = getNodeParameter(node);
//        if (editor.isEditCovariancesAsCorrelations() &&
//                parameter.getType() == ParamType.VAR) {
//            return;
//        }
//
//        double d;
//        String prefix;
//        String postfix = "";
//
//        if (parameter.getType() == ParamType.MEAN) {
//            if (editor.isEditIntercepts()) {
//                d = semIm().getIntercept(node);
//                prefix = "B0_" + node.getName() + " = ";
//            } else {
//                d = semIm().getMean(node);
//                prefix = "Mean(" + node.getName() + ") = ";
//            }
//        } else {
//            d = Math.sqrt(semIm().getParamValue(parameter));
//            prefix = node.getName() + " ~ N(0,";
//            postfix = ")";
//        }
//
//        DoubleTextField field = new DoubleTextField(d, 7, NumberFormatUtil.getInstance().getNumberFormat());
//        field.setPreferredSize(new Dimension(60, 20));
//        field.addActionListener(new NodeActionListener(this, node));
//
//        field.addFocusListener(new FocusAdapter() {
//            public void focusLost(FocusEvent e) {
//                DoubleTextField field = (DoubleTextField) e.getSource();
//                field.grabFocus();
//            }
//        });
//
//        JLabel instruct = new JLabel("Press Enter when done");
//        instruct.setFont(SMALL_FONT);
//        instruct.setForeground(Color.GRAY);
//
//        Box b1 = Box.createHorizontalBox();
//        b1.add(new JLabel(prefix));
//        b1.add(field);
//        b1.add(new JLabel(postfix));
//
//        Box b2 = Box.createHorizontalBox();
//        b2.add(instruct);
//
//        JPanel editPanel = new JPanel();
//        editPanel.setLayout(new BoxLayout(editPanel, BoxLayout.Y_AXIS));
//        editPanel.setBackground(LIGHT_YELLOW);
//        editPanel.setBorder(BorderFactory.createLineBorder(Color.LIGHT_GRAY));
//        editPanel.add(b1);
//        editPanel.add(Box.createVerticalStrut(5));
//        editPanel.add(b2);
//
//        workbench().setNodeLabel(node, editPanel, 15, 2);
//        setLastEditedObject(node);
//
//        workbench().repaint();
//        field.grabFocus();
//        field.selectAll();
    }

    private void finishEdit() {
    }

    private LnnIm lnnIm() {
        return this.lnnIm;
    }

    private Graph graph() {
        return this.lnnIm().getLnnPm().getGraph();
    }

    private GraphWorkbench workbench() {
        if (this.getWorkbench() == null) {
            this.workbench = new GraphWorkbench(graph());
            this.getWorkbench().setAllowDoubleClickActions(false);
            this.getWorkbench().addPropertyChangeListener(
                    new PropertyChangeListener() {
                        public void propertyChange(PropertyChangeEvent evt) {
                            if ("BackgroundClicked".equals(
                                    evt.getPropertyName())) {
                                finishEdit();
                            }
                        }
                    });
            resetLabels();
            addMouseListenerToGraphNodesMeasured();
        }
        return getWorkbench();
    }

    public void resetLabels() {
        for (Object o : graph().getEdges()) {
            resetEdgeLabel((Edge) (o));
        }

        java.util.List<Node> nodes = graph().getNodes();

        for (Object node : nodes) {
            resetNodeLabel((Node) node);
        }

        workbench().repaint();
    }

    private void resetEdgeLabel(Edge edge) {
        Parameter parameter = getEdgeParameter(edge);

        if (parameter != null) {
            double val = lnnIm().getParamValue(parameter);

            JLabel label = new JLabel();

            if (parameter.getType() == ParamType.COVAR) {
                label.setForeground(Color.red);
            }

            label.setBackground(Color.white);
            label.setOpaque(true);
            label.setFont(SMALL_FONT);
            label.setText(" " + asString1(val) + " ");
            label.setToolTipText(parameter.getName() + " = " + asString1(val));
            label.addMouseListener(new EdgeMouseListener(edge, this));

            workbench().setEdgeLabel(edge, label);
        } else {
            workbench().setEdgeLabel(edge, null);
        }
    }

    private String asString1(double value) {
        if (Double.isNaN(value)) {
            return " * ";
        } else {
            return nf.format(value);
        }
    }

    private String asString2(double value) {
        if (Double.isNaN(value)) {
            return "*";
        } else {
            return nf.format(value);
        }
    }

    private void resetNodeLabel(Node node) {
    }

    /**
     * Returns the parameter for the given edge, or null if the edge does not
     * have a parameter associated with it in the model. The edge must be either
     * directed or bidirected, since it has to come from a SemGraph. For
     * directed edges, this method automatically adjusts if the user has changed
     * the endpoints of an edge X1 --> X2 to X1 <-- X2 and returns the correct
     * parameter.
     *
     * @throws IllegalArgumentException if the edge is neither directed nor
     *                                  bidirected.
     */
    public Parameter getEdgeParameter(Edge edge) {
        if (Edges.isDirectedEdge(edge)) {
            return lnnIm().getLnnPm().getCoefficientParameter(edge.getNode1(), edge.getNode2());
        } else if (Edges.isBidirectedEdge(edge)) {
            return lnnIm().getLnnPm().getCovarianceParameter(edge.getNode1(), edge.getNode2());
        }

        throw new IllegalArgumentException(
                "This is not a directed or bidirected edge: " + edge);
    }

    private void setEdgeValue(Edge edge, String text) {
    }

    private void setNodeValue(Node node, String text) {
    }

    private int getSavedTooltipDelay() {
        return savedTooltipDelay;
    }

    private void setSavedTooltipDelay(int savedTooltipDelay) {
        if (this.savedTooltipDelay == 0) {
            this.savedTooltipDelay = savedTooltipDelay;
        }
    }

    private void addMouseListenerToGraphNodesMeasured() {
        java.util.List nodes = graph().getNodes();

        for (Object node : nodes) {
            Object displayNode = workbench().getModelToDisplay().get(node);

            if (displayNode instanceof GraphNodeMeasured) {
                DisplayNode _displayNode = (DisplayNode) displayNode;
                _displayNode.setToolTipText(
                        getEquationOfNode(_displayNode.getModelNode())
                );
            }
        }
    }

    private String getEquationOfNode(Node node) {
        String eqn = node.getName() + " = B0_" + node.getName();

        SemGraph semGraph = lnnIm().getLnnPm().getGraph();
        java.util.List parentNodes = semGraph.getParents(node);

        for (Object parentNodeObj : parentNodes) {
            Node parentNode = (Node) parentNodeObj;
//            Parameter edgeParam = lnnIm().getLnnPm().getEdgeParameter(
//                    semGraph.getEdge(parentNode, node));
            Parameter edgeParam = getEdgeParameter(
                    semGraph.getDirectedEdge(parentNode, node));

            if (edgeParam != null) {
                eqn = eqn + " + " + edgeParam.getName() + "*" + parentNode;
            }
        }

        eqn = eqn + " + " + lnnIm().getLnnPm().getGraph().getExogenous(node);

        return eqn;
    }

    public GraphWorkbench getWorkbench() {
        return workbench;
    }

    private boolean isEditable() {
        return editable;
    }

    public void setEditable(boolean editable) {
        workbench().setAllowEdgeReorientations(editable);
//        workbench().setAllowMultipleSelection(editable);
//        workbench().setAllowNodeDragging(false);
        workbench().setAllowDoubleClickActions(editable);
        workbench().setAllowNodeEdgeSelection(editable);
        this.editable = editable;
    }

    final static class EdgeMouseListener extends MouseAdapter {
        private Edge edge;
        private LnnImGraphicalEditor editor;

        public EdgeMouseListener(Edge edge, LnnImGraphicalEditor editor) {
            this.edge = edge;
            this.editor = editor;
        }

        private Edge getEdge() {
            return edge;
        }

        private LnnImGraphicalEditor getEditor() {
            return editor;
        }

        public void mouseClicked(MouseEvent e) {
            getEditor().beginEdgeEdit(getEdge());
        }
    }

    final static class NodeMouseListener extends MouseAdapter {
        private Node node;
        private LnnImGraphicalEditor editor;

        public NodeMouseListener(Node node, LnnImGraphicalEditor editor) {
            this.node = node;
            this.editor = editor;
        }

        private Node getNode() {
            return node;
        }

        private LnnImGraphicalEditor getEditor() {
            return editor;
        }

        public void mouseClicked(MouseEvent e) {
            getEditor().beginNodeEdit(getNode());
        }
    }

    final static class EdgeActionListener implements ActionListener {
        private LnnImGraphicalEditor editor;
        private Edge edge;

        public EdgeActionListener(LnnImGraphicalEditor editor, Edge edge) {
            this.editor = editor;
            this.edge = edge;
        }

        public void actionPerformed(ActionEvent ev) {
            DoubleTextField doubleTextField = (DoubleTextField) ev.getSource();
            String s = doubleTextField.getText();
            getEditor().setEdgeValue(getEdge(), s);
        }

        private LnnImGraphicalEditor getEditor() {
            return editor;
        }

        private Edge getEdge() {
            return edge;
        }
    }

    final static class NodeActionListener implements ActionListener {
        private LnnImGraphicalEditor editor;
        private Node node;

        public NodeActionListener(LnnImGraphicalEditor editor, Node node) {
            this.editor = editor;
            this.node = node;
        }

        public void actionPerformed(ActionEvent ev) {
            DoubleTextField doubleTextField = (DoubleTextField) ev.getSource();
            String s = doubleTextField.getText();
            getEditor().setNodeValue(getNode(), s);
        }

        private LnnImGraphicalEditor getEditor() {
            return editor;
        }

        private Node getNode() {
            return node;
        }
    }
}

