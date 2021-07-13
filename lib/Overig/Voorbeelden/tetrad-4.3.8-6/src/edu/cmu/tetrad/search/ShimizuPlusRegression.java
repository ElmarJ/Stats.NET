package edu.cmu.tetrad.search;

import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.sem.SemEstimator;
import edu.cmu.tetrad.sem.SemIm;
import edu.cmu.tetrad.sem.SemPm;


/**
 * uses Shimizu to get the graph, and regression to get the coefficients
 */
public class ShimizuPlusRegression extends Shimizu2006Search {

	ShimizuPlusRegression(double alpha){
		super(alpha);
	}

	public PatternWithParameters run(RectangularDataSet dataSet) {
		//if alpha is different OR we don't have a cached Shimizu-graph
		// create a new Shimizu-graph
		if (isGraphUsed()||getAlpha()!=super.getLastAlpha()){ 
			PatternWithParameters shimizuGraph = lingamDiscovery_DAG(dataSet);
			return PatternWithParameters.regress(dataSet, shimizuGraph.graph);
		}
		else { //use the cached Shimizu graph
			Shimizu2006Search.setIsGraphUsed(true);
			return PatternWithParameters.regress(dataSet, lastShimizuGraph.graph);		
		}	
	}

	public PatternWithParameters run(RectangularDataSet dataSet, PatternWithParameters generatingDag) {
		return run(dataSet);
	}
	
	public String getName() {
		return "Shimizu(alpha="+getAlpha()+")+Regression";
	}
	
	
}
