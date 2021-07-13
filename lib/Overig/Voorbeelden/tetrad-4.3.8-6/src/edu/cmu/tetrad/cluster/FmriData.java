package edu.cmu.tetrad.cluster;

import cern.colt.matrix.DoubleMatrix2D;
import cern.colt.matrix.DoubleMatrix1D;
import cern.colt.matrix.impl.DenseDoubleMatrix2D;

import java.util.*;
import java.io.*;

/**
 * Performs various utility operations for the fMRI data files we have.
 *
 * @author Joseph Ramsey
 */
public class FmriData {

    /**
     * The original data that was loaded, in case the user wants to revert to
     * it.
     */
    private DoubleMatrix2D originalData;

    /**
     * The transformed data;
     */
    private DoubleMatrix2D timeSeries;

    /**
     * The spatial data.
     */
    private DoubleMatrix2D xyzData;

    //===========================CONSTRUCTOR==========================//

    private FmriData() {
    }

    public static FmriData loadDataSetHead(String path, int numRecords) throws IOException {
//                    File file = new File("C:/work/proj/other/r1/r1_63.txt");
        DoubleMatrix2D data = ClusterUtils.loadMatrix(path, numRecords, 403, true, false);
        FmriData fmriData = new FmriData();

        fmriData.originalData = data;
        fmriData.timeSeries = ClusterUtils.restrictToTimeSeries(data);
        fmriData.xyzData = ClusterUtils.restrictToXyz(data);

        return fmriData;
    }

    //==========================PUBLIC METHODS==========================//

    public DoubleMatrix2D getTimeSeries() {
        return timeSeries;
    }

    public DoubleMatrix2D getXyzData() {
        return xyzData;
    }

    public int getNumPoints() {
        return timeSeries.rows();
    }

    public void convertToMeanCentered() {
        this.timeSeries = ClusterUtils.convertToMeanCentered(timeSeries);
    }

    public void imposeTimeSeriesMinimum(double minimum) {
        List<Integer> rows = new ArrayList<Integer>();

        I:
        for (int i = 0; i < timeSeries.rows(); i++) {
            for (int j = 0; j < timeSeries.columns(); j++) {
                if (timeSeries.get(i, j) < minimum) {
                    continue I;
                }
            }

            rows.add(i);
        }

        int[] _rows = new int[rows.size()];
        for (int i = 0; i < rows.size(); i++) _rows[i] = rows.get(i);

        restrictToRows(rows);
    }

    public void imposeMinimumOnTimeSeriesMaximum(double minimum) {
        List<Integer> rows = new ArrayList<Integer>();

        double maximum = Double.NEGATIVE_INFINITY;

        for (int i = 0; i < timeSeries.rows(); i++) {
            for (int j = 0; j < timeSeries.columns(); j++) {
                if (timeSeries.get(i, j) > maximum) {
                    maximum = timeSeries.get(i, j);
                }
            }

            if (maximum > minimum) {
                rows.add(i);
            }
        }

        int[] _rows = new int[rows.size()];
        for (int i = 0; i < rows.size(); i++) _rows[i] = rows.get(i);

        restrictToRows(rows);
    }

    public void restrictToRows(List<Integer> rows) {
        timeSeries = ClusterUtils.restrictToRows(timeSeries, rows);
        xyzData = ClusterUtils.restrictToRows(xyzData, rows);
    }

    /**
     * Creates a datasets restricted to rows/points whose score is >= the given
     * cutoff, restricted to the specified columns.
     */
    public void restrictToAboveThreshold(DoubleMatrix1D scores, double cutoff) {
        List<Integer> includedRows = ClusterUtils.getAboveThresholdRows(scores, cutoff, timeSeries);
        restrictToRows(includedRows);
    }

    /**
     * Creates a dataset which includes the indicated top fraction of points--
     * that is, it sorts the points low to high according <code>getScore</code>
     * and returns the top <code>topFraction</code> of them.
     */
    public void restrictToTopFractionScores(DoubleMatrix1D scores, double topFraction) {
        List<Integer> rows = ClusterUtils.getTopFractionScoreRows(scores,
                topFraction, timeSeries);
        restrictToRows(rows);
    }

    /**
     * Converts the times series to that for each time swipe the top fraction
     * scoring voxels are labeled 1.0 and the rest 0.0.
     */
    public void convertToTopFractionThresholdByTime(double topFraction) {
        DoubleMatrix2D timeSeries2 = timeSeries.like();
        timeSeries2.assign(0);

        for (int j = 0; j < timeSeries.columns(); j++) {
            if (j % 50 == 0) System.out.println(j);

            List<Integer> points = ClusterUtils.getTopFractionScoreRows(
                    timeSeries.viewColumn(j), topFraction, timeSeries);

            for (int i = 0; i < points.size(); i++) {
                timeSeries2.set(points.get(i), j, 1.0);
            }
        }

        timeSeries = timeSeries2;
    }

    /**
     * Converts the times series to that for each time swipe the top fraction
     * scoring voxels are labeled 1.0 and the rest 0.0.
     */
    public void convertToTopThresholdByTime(double threshold) {
        DoubleMatrix2D timeSeries2 = timeSeries.like();
        timeSeries2.assign(0);

        for (int j = 0; j < timeSeries.columns(); j++) {
            if (j % 50 == 0) System.out.println(j);

            List<Integer> points = ClusterUtils.getAboveThresholdRows(
                    timeSeries.viewColumn(j), threshold, timeSeries);

            for (int i = 0; i < points.size(); i++) {
                timeSeries2.set(points.get(i), j, 1.0);
            }
        }

        timeSeries = timeSeries2;
    }

    /**
     * Standardizes the time series, allowing clustering using squared error
     * loss to be over correlation space.
     */
    public void convertToStandardized() {
        timeSeries = ClusterUtils.convertToStandardized(timeSeries);
    }

    /**
     * Converts the time series to a Z stat map.
     */
    public void convertToSeriesZScores() {
        timeSeries = ClusterUtils.convertToSeriesZScores(timeSeries);
    }

    /**
     * Prints the clusters to Standard.out.
     */
    public void printClusters(List<List<Integer>> clusters) {
        for (int k = 0; k < clusters.size(); k++) {
            List<Integer> _cluster = clusters.get(k);
            Collections.sort(_cluster);
            int[] rows = new int[_cluster.size()];

            for (int m = 0; m < _cluster.size(); m++) {
                rows[m] = _cluster.get(m);
            }

            int[] cols = new int[3];

            for (int m = 0; m < 3; m++) {
                cols[m] = m;
            }

            DoubleMatrix2D selection = xyzData.viewSelection(rows, cols);

            System.out.println("Cluster " + k);
            ClusterUtils.printXyzExtents(selection);
            ClusterUtils.printPcaTranslations(selection, k);

            System.out.println();
            System.out.println("Centroid: " + timeSeries.viewRow(k));
            System.out.println();
        }
    }

    /**
     * Prints the XYZ extents of the data--that is, the minimum and maximum of
     * each dimension.
     */
    public void printXyzExtents() {
        ClusterUtils.printXyzExtents(xyzData);
    }

    public void writeClustersToGnuPlotFile(String path, List<List<Integer>> clusters)
            throws FileNotFoundException {
        ClusterUtils.writerClustersToGnuPlotFile(xyzData, clusters, path);
    }

    public void writeClustersToGnuPlotFile(String path, List<List<Integer>> clusters,
                                           List<List<Integer>> colors)
            throws FileNotFoundException {
        ClusterUtils.writerClustersToGnuPlotFile(xyzData, clusters, colors, path);
    }

    /**
     * Reverts to the original data that was loaded.
     */
    public void revert() {
        timeSeries = ClusterUtils.restrictToTimeSeries(originalData);
        xyzData = ClusterUtils.restrictToXyz(originalData);
    }

    //======================== PRIVATE METHODS ===========================//

}
