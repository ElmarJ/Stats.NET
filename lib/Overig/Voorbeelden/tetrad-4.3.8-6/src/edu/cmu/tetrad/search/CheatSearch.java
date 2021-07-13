package edu.cmu.tetrad.search;

import java.util.List;

import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.graph.Edge;
import edu.cmu.tetrad.sem.SemEstimator;
import edu.cmu.tetrad.sem.SemIm;
import edu.cmu.tetrad.sem.SemPm;


/**
 * Not a search at all, but rather returns the true DAG with the coefficients reconstructed through regression.
 * 
 * @author Gustavo
 *
 */
public class CheatSearch implements SemLearningMethod {

	public String getName() {
		// TODO Auto-generated method stub
		return "true DAG";
	}


	/**
	 * makes a PatternWithParameters made from generatingDag.
	 */
	public PatternWithParameters run(RectangularDataSet dataSet, PatternWithParameters generatingDag) {
		PatternWithParameters estimatedTrueDag = null;
		try {
			SemPm semPmTrueDag = new SemPm(generatingDag.graph);
			SemEstimator estimatorTrueDag = new SemEstimator(dataSet,semPmTrueDag);
			estimatorTrueDag.estimate();
			SemIm semImTrueDag = estimatorTrueDag.getEstimatedSem();

			estimatedTrueDag = new PatternWithParameters(semImTrueDag,generatingDag.graph);
		}
		catch(Exception e){
			e.printStackTrace();
			System.out.println("Skip this pattern");    		
		}

		return estimatedTrueDag;
	}

	
	/**
	 * makes a PatternWithParameters made from generatingDag.
	 */
	public PatternWithParameters run(RectangularDataSet dataSet, List<Edge> edgesToEvaluateCoeffs, PatternWithParameters generatingDag) {
		return run(dataSet,generatingDag);
	}

}
