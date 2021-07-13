package edu.cmu.tetrad.search;

import cern.jet.random.Normal;

public class GaussianConfidenceInterval {
    private double alpha; //but is it a significance test?

	public GaussianConfidenceInterval(double alpha){
		this.alpha = alpha;
	}
	
	/**
	 * two-sided confidence interval
	 * @param mean
	 * @param sampleSd
	 * @return whether we reject the hypothesis that 'value' was sampled from this normal distribution
	 * with confidence alpha
	 */
	public boolean test(double value, double mean, double sampleSd, int sampleSize){
		double estPopSd = sampleSd;
		double score = new Normal(mean, estPopSd, null).cdf(value);
//		System.out.println("score = " + score);
		return (score < alpha/2) || (score > 1-alpha/2);
	}
}
