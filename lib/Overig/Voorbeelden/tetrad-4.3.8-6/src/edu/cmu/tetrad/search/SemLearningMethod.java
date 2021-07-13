package edu.cmu.tetrad.search;

import java.util.List;

import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.graph.Edge;

/**
 * methods to learn SEMs, e.g. PC, Shimizu, etc
 *  
 * @author Gustavo
 *
 */
public interface SemLearningMethod {

	String getName();	//name of the SEM learning method
	
//	PatternWithParameters run(RectangularDataSet dataSet); //estimates a graph
//
//	//sets edgesToEvaluateCoeffs
//	PatternWithParameters run(RectangularDataSet dataSet, PatternWithParameters generatingDag);

	PatternWithParameters run(RectangularDataSet dataSet,
                              PatternWithParameters generatingDag);

}
