package edu.cmu.tetrad.search;

public class EvaluationResult {
	
	static class AdjacencyEvaluationResult implements PartialEvaluationResult {
		Integer errorsOfOmission;
		Integer errorsOfCommission;
		
		public AdjacencyEvaluationResult(Integer errorsOfOmission, Integer errorsOfCommission) {
			super();
			this.errorsOfOmission = errorsOfOmission;
			this.errorsOfCommission = errorsOfCommission;
		}

		public double loss() {
			return errorsOfOmission + errorsOfCommission;
		}
	
		public double[] values(){
			return new double[]{errorsOfOmission,errorsOfCommission,loss()};
		}
	}
	
	static class OrientationEvaluationResult implements PartialEvaluationResult {
		Integer nCorrect;
		Integer nIncorrect;
		Integer nUnoriented;
		
		public OrientationEvaluationResult(Integer correct, Integer incorrect, Integer unoriented) {
			super();
			this.nCorrect = correct;
			this.nIncorrect = incorrect;
			this.nUnoriented = unoriented;
		}

		public double[] values(){
			return new double[]{nCorrect,nIncorrect,nUnoriented};
		}
	
	}

	static class CoefficientEvaluationResult implements PartialEvaluationResult {
		Double totalCoeffErrorSq;
		Integer nEdgesEvaluated;

		public CoefficientEvaluationResult(Double totalCoeffErrorSq, Integer edgesEvaluated) {
			super();
			this.totalCoeffErrorSq = totalCoeffErrorSq;
			this.nEdgesEvaluated = edgesEvaluated;
		}
		
		double loss(){
			return totalCoeffErrorSq;
		}
		
		public double[] values(){
			return new double[]{totalCoeffErrorSq,nEdgesEvaluated,loss()};
		}
		
	}


	AdjacencyEvaluationResult adj;
	OrientationEvaluationResult ori;
	CoefficientEvaluationResult coeffAll;
	CoefficientEvaluationResult coeffSome;
	
	String name = null;

	
	public EvaluationResult(String methodName, AdjacencyEvaluationResult adj, OrientationEvaluationResult ori,
			CoefficientEvaluationResult coeffAll, CoefficientEvaluationResult coeffSome) {
		super();
		this.name = methodName;
		this.adj = adj;
		this.ori = ori;
		this.coeffAll = coeffAll;
		this.coeffSome = coeffSome;

	}		

}
