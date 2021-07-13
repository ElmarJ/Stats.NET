package edu.cmu.tetrad.search;

import java.io.File;
import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Vector;

import edu.cmu.tetrad.data.ColtDataSet;
import edu.cmu.tetrad.data.DataParser;
import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.graph.Dag;
import edu.cmu.tetrad.graph.Edge;
import edu.cmu.tetrad.graph.Endpoint;
import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.graph.GraphNode;
import edu.cmu.tetrad.graph.GraphUtils;
import edu.cmu.tetrad.graph.Node;
import edu.cmu.tetrad.search.EvaluationResult.AdjacencyEvaluationResult;
import edu.cmu.tetrad.search.EvaluationResult.CoefficientEvaluationResult;
import edu.cmu.tetrad.search.EvaluationResult.OrientationEvaluationResult;
import edu.cmu.tetrad.search.fastica.math.Matrix;

import cern.colt.function.DoubleFunction;
import cern.colt.list.DoubleArrayList;
import cern.colt.matrix.DoubleMatrix1D;
import cern.colt.matrix.DoubleMatrix2D;
import cern.colt.matrix.impl.DenseDoubleMatrix1D;
import cern.colt.matrix.impl.DenseDoubleMatrix2D;
//import edu.cmu.tetrad.search.FastIca;
import edu.cmu.tetrad.sem.*;
import edu.cmu.tetrad.util.Distribution;
import edu.cmu.tetrad.util.GaussianPower;
import edu.cmu.tetrad.util.MatrixUtils;
import edu.cmu.tetrad.util.NormalDistribution;
import edu.cmu.tetrad.util.UniformDistribution;

public class TestFastIca {

//	public TestCats(String name) {
//		super(name);
//	}



	//DoubleMatrix2D inVectors = readData("C:/Program Files/R/R-2.4.1/data.txt");//generateBimodal2DGaussian(100); //inVectors1;
//	DoubleMatrix2D inVectors = readData("C:/Program Files/R/R-2.4.1/data1000-2.txt");
	//DoubleMatrix2D inVectors = readData("C:/Program Files/R/R-2.4.1/data-5000.txt");
	//DoubleMatrix2D inVectors = readData("C:/Program Files/R/R-2.4.1/data-20000.txt");
	//DoubleMatrix2D inVectors = readData("C:/Program Files/R/R-2.4.1/data1000-3.txt");
	//DoubleMatrix2D inVectors = readData("C:/Program Files/R/R-2.4.1/data20000-3.txt");
	//DoubleMatrix2D inVectors = readData("C:/Program Files/R/R-2.4.1/data20000-4.txt");


	//runSwappingExperiment();
	//runSwappingTest();


	public double[][] sampleMatrix(){
//		double[][] a = {{12,1,10},{3,5,7},{17,9,2}};
//		double[][] a = {{12,1,10},{17,9,2},{3,5,7}};
//		double[][] a = {{2,5,10,11},{3,3,5,1},{5,4,8,6},{6,3,5,12}};
		double[][] a = {{2,5,10,11,7},{3,3,5,1,2},{5,4,8,6,11},{6,3,5,12,10},{9,2,4,5,9}};

		return a;
	}





//	DoubleMatrix2D mat = convertToColt(a);
//	System.out.println("mat = " + mat);

//	MatCost bestMat = bestAssignment(mat);

//	System.out.println("bestMat.matrix = " + bestMat.matrix);
//	System.out.println("bestMat.cost = " + bestMat.cost);
//	System.out.println("entered = " + entered);


//	public static void testMain() {
//		DoubleMatrix2D inVectors;
//
////		inVectors = example_line(3000);
////		inVectors = example_Y(3000);
//		inVectors = example_completeDag(5000);
//
//		List<Node> nodes = makeNodeList(inVectors.rows());
//		ColtDataSet dataSet = Shimizu2006Search.makeDataSet(inVectors.viewDice(), nodes);
////		csda_B(inVectors);
//
//		ColtDataSet bHatData = Shimizu2006Search.lingamDiscovery_Bhat(dataSet);
//
//		DoubleMatrix2D Bhat = bHatData.getDoubleMatrix().viewDice();
//		System.out.println("Bhat = " + Bhat);
//
//		// Diagonal
//		assertEquals(0, Bhat.get(0, 0), 0.001);
//		assertEquals(0, Bhat.get(1, 1), 0.001);
//		assertEquals(0, Bhat.get(1, 1), 0.001);
//		assertEquals(0, Bhat.get(1, 1), 0.001);
//
//		// Top triangle.
//		assertEquals(0, Bhat.get(0, 1), 0.05);
//		assertEquals(0, Bhat.get(0, 2), 0.05);
//		assertEquals(0, Bhat.get(0, 3), 0.05);
//		assertEquals(0, Bhat.get(2, 3), 0.05);
//		assertEquals(0, Bhat.get(3, 3), 0.05);
//
//		// Bottom triangle
//		assertEquals(2, Bhat.get(1, 0), 0.2);
//		assertEquals(3, Bhat.get(2, 0), 0.2);
//		assertEquals(0, Bhat.get(2, 1), 0.2);
//		assertEquals(1, Bhat.get(3, 0), 0.2);
//		assertEquals(1, Bhat.get(3, 1), 0.2);
//		assertEquals(-1, Bhat.get(3, 2), 0.2);
//
//	}


	//creates a list of nodes in simple order
	public static List<Node> makeNodeList(int nVars) {
		List<Node> nodes = new Vector<Node>();

		//taken from UniformGraphGenerator.getDag
		for (int i = 1; i <= nVars; i++) {
			GraphNode node = new GraphNode("X" + i);
			nodes.add(node);
		}

		return nodes;
	}



	//Dag with edge weights
	private static PatternWithParameters makeRandomDagWithParms(Dag dag) {
		PatternWithParameters dwp = new PatternWithParameters(dag);

		List<Edge> edges = dag.getEdges();

		//for each edge, add a weight to the hash
		for (int i=0; i<edges.size(); i++){
			Edge edge = edges.get(i);
//			double w = 0.0;
			double w = 1.0;
			dwp.weightHash.put(edge, w);    		
		}

		return dwp;
	}


	//Dag with a slot for node values, i.e. instantiations
	public static class DagWithParmsAndValues {

		public PatternWithParameters dwp;

		public HashMap<Node,Double> valueHash;

		public DagWithParmsAndValues(PatternWithParameters dwp){
			this.dwp = dwp;
			valueHash = new HashMap<Node,Double>();
		}
	}



	public static void main(String[] args){
//		testMain();
		compareIcaTetrad();
//		runMultipleTimes();
	}
	
	
	/**
	 * test whether ICA gives the same answer each time
	 *
	 */
	public static void runMultipleTimes(){
		RectangularDataSet dataSet = readData("C:\\Documents and Settings\\Administrator\\Desktop\\causality\\joe-data.txt");
		for (int i=0; i<10; i++){
			Vector<SemLearningMethod> methods = new Vector<SemLearningMethod>();
			methods.add(new Shimizu2006Search(0.5));
			doRun(dataSet, null, methods);//true, false, false);
		}
	}


	//compares Shimizu 2006's method to Tetrad
	private static void compareIcaTetrad() {

		Distribution beta05_05 = new BetaDistribution(0.5,0.5);
		Distribution beta2_2 = new BetaDistribution(2,2);
		Distribution u = new UniformDistribution(-1,1);
		Distribution gp025 = new GaussianPower(0.25);
		Distribution gp05 = new GaussianPower(0.5);
		Distribution gp09 = new GaussianPower(0.9);
		Distribution gp095 = new GaussianPower(0.95);
		Distribution gp098 = new GaussianPower(0.98);
		Distribution gp1 = new GaussianPower(1);
		Distribution g = new NormalDistribution(0,1);

		Distribution gp102 = new GaussianPower(1.02);
		Distribution gp105 = new GaussianPower(1.05);
		Distribution gp11 = new GaussianPower(1.1);
		Distribution gp12 = new GaussianPower(1.2);
		Distribution gp13 = new GaussianPower(1.3);
		Distribution gp14 = new GaussianPower(1.4);
		Distribution gp15 = new GaussianPower(1.5);
		Distribution gp16 = new GaussianPower(1.6);
		Distribution gp17 = new GaussianPower(1.7);
		Distribution gp18 = new GaussianPower(1.8);
		Distribution gp19 = new GaussianPower(1.9);
		Distribution gp2 = new GaussianPower(2);
		Distribution gp25 = new GaussianPower(2.5);
		Distribution gp3 = new GaussianPower(3);
		Distribution gp35 = new GaussianPower(3.5);
		Distribution gp4 = new GaussianPower(4);
		
		Distribution beta0203 = new BetaDistribution(0.2,0.3);

		for (int nNodes : new int[]{10})
			for (int maxNoParents : new int[]{3}) //setting this to 1 prevents colliders
				for (int graphRun=0; graphRun<1; graphRun++) //number of datasets per generating graph
					//generate DAG here

					//we could use the same DAG with different distributions and sample sizes
					//instead of generating a new one on each run
					//to do that, we would make generatingDag here and pass it to doRun()


					for (int sampleSize : new int[]{25, 50}) //{15,20,40,70,100,150,200}
						for (Distribution distr : new Distribution[]{gp2}) {
							int nRuns = 10;
							//g, gp105, gp11, gp12, gp13, gp14, gp15, 
							List<HashMap<SemLearningMethod,EvaluationResult>> evalTable;
							
							//evalTable should have a HashTable of methods
							//for each method, it assigns an EvaluationResult
							
							//AbstractMatrix2D evTable = new DenseAbstractMatrix2D();
							
							List<SemLearningMethod> methods = new Vector<SemLearningMethod>();
//							methods.add(new PcSearch(0.025));
//							methods.add(new PcSearch(0.05));
							methods.add(new PcSearch(0.10));
//							methods.add(new PcSearch(0.20));
//							methods.add(new PcSearch(0.40));

//							methods.add(new Shimizu2006Search(0.025));
//							methods.add(new Shimizu2006Search(0.05));
							methods.add(new Shimizu2006Search(0.10));
//							methods.add(new Shimizu2006Search(0.20));
//							methods.add(new Shimizu2006Search(0.40));
//
//							methods.add(new ShimizuPlusRegression(0.10));
//							methods.add(new ShimizuPlusRegression(0.10));
							
							methods.add(new CheatSearch());
							
							evalTable = doRuns(nNodes,sampleSize,maxNoParents,distr, methods, nRuns);

							printRunHeader(sampleSize,maxNoParents,(GaussianPower) distr);
							makeComparison(evalTable,methods);
						}
	}
	
	//aggregate identical runs into the same data
	
	//coeff estimates, 
	
	//log into a file

	

	/**
	 * print statistics for each run
	 * evalTable contains all the results for a given setting
	 * @param methods 
	 */
	private static void makeComparison(List<HashMap<SemLearningMethod,EvaluationResult>> evalTable,
			List<SemLearningMethod> methods) {
		
//		List<String> methods = new Vector<String>();
//		for (EvaluationResult eval : evalTable.get(0)){
////			methods.add(eval.name);
//			System.out.print(eval.name+____);
//		}		
//		System.out.println("");
		

		//first, change the traversal order to iterate through property, and inside that iterate through runs
		
		//construct the whole table of values as a DoubleMatrix2D, and pass that to the new method
		//this method prints the table, and the summary statistics.

		int i; //run-number
		int propsPerMethod;
		
		i=0; 
		propsPerMethod = 3;
		
		DoubleMatrix2D adjTable = new DenseDoubleMatrix2D(evalTable.size(),propsPerMethod*methods.size());

		for (HashMap<SemLearningMethod,EvaluationResult> run : evalTable){ //each run in the evalTable corresponds to the list of methods

			int nMethod = 0;
			//for each method, show omission, commission and loss
			for(SemLearningMethod method : methods){
				EvaluationResult eval = run.get(method);
				adjTable.set(i, 0 + propsPerMethod*nMethod, eval.adj.errorsOfOmission);
				adjTable.set(i, 1 + propsPerMethod*nMethod, eval.adj.errorsOfCommission);
				adjTable.set(i, 2 + propsPerMethod*nMethod, eval.adj.loss());
				nMethod++;
			}
			
			i++;
		}
		printTable(propsPerMethod, adjTable, methods, "adjacency (errOmission,errCommission,loss)");
		

		///////////////////////////////////////////////////
		

		i=0; 
		propsPerMethod = 1;
		
		DoubleMatrix2D allCoeffTable = new DenseDoubleMatrix2D(evalTable.size(),propsPerMethod*methods.size());

		for (HashMap<SemLearningMethod,EvaluationResult> run : evalTable){ //each run in the evalTable corresponds to the list of methods

			int nMethod = 0;
			//for each method, show omission, commission and loss
			for(SemLearningMethod method : methods){
				EvaluationResult eval = run.get(method);
				allCoeffTable.set(i, 0 + propsPerMethod*nMethod, eval.coeffAll.loss());
				nMethod++;
			}
			
			i++;
		}
		printTable(propsPerMethod, allCoeffTable, methods, "all coefficients (total_loss)");


		////////////////////////////////////////////////////////
		

//		i=0; 
//		propsPerMethod = 2;
//		
//		DoubleMatrix2D someCoeffsTable = new DenseDoubleMatrix2D(evalTable.size(),propsPerMethod*methods.size());
//
//		for (HashMap<SemLearningMethod,EvaluationResult> run : evalTable){ //each run in the evalTable corresponds to the list of methods
//
//			int nMethod = 0;
//			//for each method, show omission, commission and loss
//			for(SemLearningMethod method : methods){
//				EvaluationResult eval = run.get(method);
//				someCoeffsTable.set(i, 0 + propsPerMethod*nMethod, eval.coeffSome.loss());
//				someCoeffsTable.set(i, 0 + propsPerMethod*nMethod, eval.coeffSome.nEdgesEvaluated);
//				nMethod++;
//			}
//			
//			i++;
//		}
//		printTable(propsPerMethod, someCoeffsTable, methods, "some coefficients (loss, nEdgesEvaluated)");

	}




	/**
	 * prints a table. Each row is a run. Each run contains methods and each method contains properties.
	 * @param propsPerMethod number of properties recorded for each method
	 * @param table
	 * @param methods 
	 */
	private static void printTable(int propsPerMethod, DoubleMatrix2D table, List<SemLearningMethod> methods,
			String propsEvaluated) {

		int props = table.columns();
		int runs = table.rows();
		
		List<Double> means = new Vector<Double>();
		List<Double> sds = new Vector<Double>();

		//calculate statistics
		for (int j=0; j<table.columns(); j++){//for each column

			DoubleArrayList dal = new DoubleArrayList(table.viewColumn(j).toArray()); //calculate summary statistics for the column
			double mean = cern.jet.stat.Descriptive.mean(dal);
			means.add(mean);
			double variance = cern.jet.stat.Descriptive.sampleVariance(dal, mean);
			double sd = cern.jet.stat.Descriptive.sampleStandardDeviation(runs, variance);
			sds.add(sd);
		}
				
		System.out.println("************************************************************************");
		System.out.println("*** comparing runs for this setting: "+propsEvaluated+" ***");
		System.out.println("************************************************************************");

		String s="";
		for (SemLearningMethod method : methods){
			s += method.getName() + __;
		}
		System.out.println(s);
		
		for (int i=0; i<runs; i++){ //print each entry
			s = "#" + i + ":" + __; //reset s
			for (int j=0; j<props; j++)
				s += nextEntryString(table.get(i, j),j,propsPerMethod);

			System.out.println(s);
		}
		
		//print summary statistics for each column

		System.out.println(stringMultiply("-",methods.size()*12));

		s="mean:   "+_;		
		for (int j=0; j<props; j++)
			s += nextEntryString(means.get(j),j,propsPerMethod);
		
		System.out.println(s);

		s="stdev:  "+_;		
		for (int j=0; j<props; j++)
			s += nextEntryString(sds.get(j),j,propsPerMethod);
		
		System.out.println(s);

		
	}




	static String _ = "     ";
	static String __ = _ + _;
	static String ___ = _ + _ + _;
	static String ____ = _ + _ + _ + _;
	static String METHOD_SEPARATOR = "|"+_;

	/**
	 * returns the string to be added for the next entry on the line
	 * count the number of spaces
	 * @param entry
	 * @param column
	 * @param propsPerMethod
	 * @return
	 */
	private static String nextEntryString(double entry, int column, int propsPerMethod) {
		
		DecimalFormat form = new DecimalFormat("0.000E00");
		
		String s="";
		if (column%propsPerMethod==0) //add an extra space at the end of each method
			s += METHOD_SEPARATOR;

		//double d = Math.round(entry*1000.0) / 1000.0;
		String d = form.format(entry);

		int dSize = (""+d).length();

		// we add 10 - dSize spaces
		
		s += d + stringMultiply(" ",10-dSize);
		return s;
	}




	/**
	 *  stringMultiply("x",3) --> "xxx"
	 * @param string
	 * @param n
	 * @return
	 */
	private static String stringMultiply(String str, int n) {
		String s="";
		for (int i=0; i<n; i++){
			s+=str;
		}
		return s;
	}





	/**
	 * print header
	 * generate the random generating DAG
	 * pass them to the next function
	 * 
	 * should we vary sampleSize within the same DAG?
	 */
	private static List<HashMap<SemLearningMethod,EvaluationResult>> doRuns(int nNodes, int sampleSize, int maxNoParents, Distribution distr,
			List<SemLearningMethod> methods, int nRuns) {

		List<HashMap<SemLearningMethod,EvaluationResult>> evalTable = new Vector<HashMap<SemLearningMethod,EvaluationResult>>(); //rows are methods, columns are runs
		
		for (int run=0; run<nRuns; run++){
			
			printRunHeader(sampleSize,maxNoParents,(GaussianPower) distr,run);

			///////////////////////////////////
			// generate DAG
			///////////////////////////////////

			Dag dag = GraphUtils.createRandomDag(nNodes, 0, nNodes, nNodes, maxNoParents, nNodes, false);
//			Dag dag = twoNodeDag();
			System.out.println("generating DAG = " + dag);

			PatternWithParameters generatingDag = makeRandomDagWithParms(dag);
			HashMap<SemLearningMethod,EvaluationResult> evalSet = doRun(generatingDag, sampleSize, distr, methods);

			evalTable.add(evalSet);
		}
		
		return evalTable;
	}

	/**
	 * generates simulated data, using 'generatingDag', 'distr' and sampleSize
	 * passes this data to the next function.
	 * @param generatingDag generating DAG for this SEM model
	 * @param sampleSize 
	 * @param distr distribution of the noise terms
	 * @param runIca whether to run Shimizu
	 * @param runPc whether to run PC
	 */
	private static HashMap<SemLearningMethod,EvaluationResult> doRun(PatternWithParameters generatingDag, int sampleSize,
			Distribution distr, List<SemLearningMethod> methods) { //boolean runIca, boolean runPc) {
		
		///////////////////////////////////
		// simulate
		///////////////////////////////////

		DoubleMatrix2D inVectors = simulate(generatingDag,sampleSize,distr); 

		System.out.println("inVectors (truncated) = " + truncate(inVectors));
//		System.out.println("inVectors = " + inVectors.viewDice());

		//wrapper: ColtDataGraph = data matrix + variable names
		ColtDataSet dataSet = Shimizu2006Search.makeDataSet(inVectors.viewDice(), generatingDag.graph.getNodes());
		return doRun(dataSet, generatingDag, methods); //runIca, runPc, true);
		
	}

	
	public static List<Edge> edgesToEvaluateCoeffs;
	


	/**
	 *  Analyze dataSet with different methods (ICA and/or PC), and evaluate each method.
	 *  for each method, run it, evaluate it, and record the results.
	 * @param dataSet
	 * @param generatingDag
	 * @param runIca
	 * @param runPc
	 */
	//pass a list of methods, e.g. PC, Shimizu(alpha=0.2), Shimizu(alpha=0.5)
	private static HashMap<SemLearningMethod,EvaluationResult> doRun(RectangularDataSet dataSet, PatternWithParameters generatingDag,
			List<SemLearningMethod> methods){
			
//			boolean runShimizu, boolean runPc, boolean evaluate) {
		
		HashMap<SemLearningMethod,EvaluationResult> evals = new HashMap<SemLearningMethod,EvaluationResult>();
		
//		List<Edge> edgesToEvaluateCoeffs = null;

//		EvaluationResult pcEval = null;
//		EvaluationResult pcTrueEval = null;
//		EvaluationResult shimizuEval = null;

		
		//need to capture the edgesToEvaluateCoeffs
//		edgesToEvaluateCoeffs = evaluatableEdges(pattern); //apply algorithm to 'pattern'
//		System.out.println("edgesToEvaluateCoeffs = " + edgesToEvaluateCoeffs);

		//must run PC search first, so that 'edgesToEvaluateCoeffs' gets set correctly
		for (SemLearningMethod method : methods){
			System.out.println("437: before, edgesToEvaluateCoeffs = " + edgesToEvaluateCoeffs);

			System.out.println("");
			System.out.println("****************************************************");
	    	System.out.println("*************** Running "+method.getName()+" ****************");
			System.out.println("****************************************************");
			
			PatternWithParameters estimatedGraph = method.run(dataSet,generatingDag);
			System.out.println("437: after, edgesToEvaluateCoeffs = " + edgesToEvaluateCoeffs);
			EvaluationResult eval = evaluateGraph(estimatedGraph, generatingDag, method.getName(), true, true, true, edgesToEvaluateCoeffs);
			evals.put(method, eval);
		}
		
		// 4 methods
		// PC graph + regression
		// true graph + regression
		// Shimizu(0.1)
		// Shimizu(0.5)
		
		
//		////////////////////////////////////////
//		// run PC search
//		////////////////////////////////////////
//
//		if (runPc){
//			double alpha = 0.05;
//			Graph pattern = runPc(dataSet, alpha);
//			System.out.println("pattern = " + pattern);
//
//			try{
//				PatternToDagSearch search = new PatternToDagSearch(new Pattern(pattern));
//				Dag patDag = search.patternToDagMeek();				//turn into a DAG
//				System.out.println("patDag = " + patDag);
//
//				//creating an IM from estimated DAG
//				SemPm semPmEstDag = new SemPm(patDag);
//				SemEstimator estimatorEstDag = new SemEstimator(dataSet,semPmEstDag);
//				estimatorEstDag.estimate();
//				SemIm semImEstDag = estimatorEstDag.getEstimatedSem(); //this is our real estimated model
//
//				SemPm semPmTrueDag = new SemPm(generatingDag.graph);
//				SemEstimator estimatorTrueDag = new SemEstimator(dataSet,semPmTrueDag);
//				estimatorTrueDag.estimate();
//				SemIm semImTrueDag = estimatorTrueDag.getEstimatedSem();
//
//				edgesToEvaluateCoeffs = evaluatableEdges(pattern); //apply algorithm to 'pattern'
//				System.out.println("edgesToEvaluateCoeffs = " + edgesToEvaluateCoeffs);
//				
//				PatternWithParameters estimatedGraph = new PatternWithParameters(semImEstDag,pattern);
//
//				if (evaluate){
//					pcEval = evaluateGraph(estimatedGraph, generatingDag, "PC graph - PC estimation", true, true, true, edgesToEvaluateCoeffs);
//					evals.add(pcEval);
//				}
//				PatternWithParameters estimatedTrueDag = new PatternWithParameters(semImTrueDag,generatingDag.graph);
//				if (evaluate){
//					pcTrueEval = evaluateGraph(estimatedTrueDag, generatingDag, "true graph - PC estimation", true, true, true, edgesToEvaluateCoeffs);
//					evals.add(pcTrueEval);
//				}
//			}
//			catch(Exception e){
//				e.printStackTrace();
//				System.out.println("Skip this pattern");    		
//			}
//		}
//
//		///////////////////////////////////
//		// run Shimizu
//		///////////////////////////////////
//
//		if (runShimizu){
//			double alpha = 0.50;
//			PatternWithParameters shimizuDag = Shimizu2006Search.lingamDiscovery_DAG(dataSet,alpha);
//			System.out.println("shimizuDag = " + shimizuDag);
//			if (evaluate) {
//				shimizuEval = evaluateGraph(shimizuDag, generatingDag, "Shimizu2006(alpha="+alpha+")", true, true, true, edgesToEvaluateCoeffs);
//				evals.add(shimizuEval);
//			}
//		}

//		if (runPc && runShimizu){ //compare who did better
//			double pcAdjLoss = pcEval.adj.loss();
//			double shimizuAdjLoss = shimizuEval.adj.loss();
//
//			//COMPARE ADJACENCY
//			printComparisonSingleRun("adjacency",pcEval,shimizuEval,pcAdjLoss,shimizuAdjLoss);
//
//			//COMPARE ORIENTATION
//
//			//COMPARE SOME COEFFICIENTS
//			double pcCoeffSomeLoss = pcEval.coeffSome.loss();
//			double shimizuCoeffSomeLoss = shimizuEval.coeffSome.loss();
//			printComparisonSingleRun("some coefficients",pcEval,shimizuEval,pcCoeffSomeLoss,shimizuCoeffSomeLoss);
//
//		}

		return evals;
	}

	private static void printComparisonSingleRun(String comparedAttribute,
			EvaluationResult eval1, EvaluationResult eval2, double loss1, double loss2) {
		String s;
		double difference = loss1 - loss2;
		if (difference!=0)
			s = ((difference > 0) ? eval2 : eval1 ).name + " did better by " + difference;
		else
			s = eval1.name + " and " + eval2.name + " tied!";
		
		System.out.println(comparedAttribute + ": " + "PC loss = " + loss1 + "  Shimizu loss = " + loss2 + ". " + 
				s);
	}







	public static List<Edge> getDirectedEdges(Graph g){
		List<Edge> edges = new ArrayList();

		for (Edge edge : g.getEdges())
			if (edge.isDirected())
				edges.add(edge);

		return edges;
	}

	public static double globalCoeffErrorScoreTrue=0;
	public static int globalNEdgesEvaluatedTrue=0;


	private static Dag twoNodeDag() {
		Node node1 = new GraphNode("X1");
		Node node2 = new GraphNode("X2");
		List<Node> nodes = new ArrayList<Node>();
		nodes.add(node1);
		nodes.add(node2);		
		Dag dag = new Dag(nodes);
		Edge edge = new Edge(node1,node2,Endpoint.TAIL,Endpoint.ARROW);
//		Edge edge = new Edge(node2,node1,Endpoint.TAIL,Endpoint.ARROW);
		dag.addEdge(edge);
		return dag;
	}

	private static void printRunHeader(int sampleSize, int maxNoParents, GaussianPower distr) {
		System.out.println("");
		System.out.println("############################################################################");
		System.out.println("########sampleSize = " + sampleSize + ", maxParents = " + maxNoParents + ", distr = " + distr.getName() + " ####");
		System.out.println("############################################################################");
	}

	
	private static void printRunHeader(int sampleSize, int maxNoParents, GaussianPower distr, int runN) {
		System.out.println("");
		System.out.println("############################################################################");
		System.out.println("############################################################################");
		System.out.println("########sampleSize = " + sampleSize + ", maxParents = " + maxNoParents + ", distr = " + distr.getName() + ", run = " + runN + " ####");
		System.out.println("############################################################################");
		System.out.println("############################################################################");
	}



	//    PC:
//	* how often arrow-heads were added (commission)
//	* how often they were left out (omission)
//	* how often adjacencies were added
//	* how often they were left out

//	* turn the pattern into a DAG. Estimate the coefficients.
//	** if the adjacency was directed in the pattern, we evaluate it

//	** if the adjacency was undirected, say UNKNOWN



	//does the graph have an edge similar to 'edge'?
	private static boolean hasCorrespondingEdge(Graph graph, Edge edge) {
		String nodeName1 = edge.getNode1().getName();
		String nodeName2 = edge.getNode2().getName();
		Node node1 = graph.getNode(nodeName1);
		Node node2 = graph.getNode(nodeName2);

		Edge corrEdge = graph.getDirectedEdge(node1, node2);

		return corrEdge!=null;
	}



//	public static int oriCorrect;
	public static int totalOriCorrect=0;
	public static int totalOriEvaluated=0;
	private static int totalErrorsOfOmission=0;
	private static int totalErrorsOfCommission=0;

//	ICA:
//	* which orientations are wrong
//	* how far off the coefficients are
		

	/**
	 * this method evaluates coefficients twice: first time, for every node-pair, the second time for a subset
	 * of the node-pairs.
	 * @param edgesToEvaluateCoeffs the edges for which we evaluate coefficients the second time
	 */
	private static EvaluationResult evaluateGraph(PatternWithParameters graph,
			PatternWithParameters generatingDag, String methodName,
			boolean evalAdjacency, boolean evalOrientation, boolean evalCoeffs, List<Edge> edgesToEvaluateCoeffs) {

		//add to the results of methodName
		//if this object does not yet exist, the 'add' method will create it.
		//HashMap.get(methodName)
				
		System.out.println("");
		System.out.println("****************************************************");
		System.out.println("************* Evaluating "+methodName+" ***************");
		System.out.println("****************************************************");
		
		AdjacencyEvaluationResult adj = null;
		OrientationEvaluationResult ori = null;
		CoefficientEvaluationResult coeffAll = null;
		CoefficientEvaluationResult coeffSome = null;
		
		//modifies errorsOmission, errorsCommission
		if (evalAdjacency){
			adj = graph.evalAdjacency((Dag) generatingDag.graph);
			graph.printAdjacencyEvaluation();
			totalErrorsOfOmission+=graph.errorsOfOmission;
			totalErrorsOfCommission+=graph.errorsOfCommission;
		}

		//should only evaluate on the adjacencies that are correct
		if (evalOrientation){
			ori = graph.evalOrientations((Dag) generatingDag.graph);
			graph.printOrientationEvaluation();
			totalOriCorrect += graph.oriCorrect;
			totalOriEvaluated += graph.oriEvaluated;
		}
		
		if (evalCoeffs){
			System.out.println("== evaluating all node pairs ==");
			coeffAll = graph.evalCoeffs(generatingDag);
			graph.printCoefficientEvaluation();
			
			if (edgesToEvaluateCoeffs!=null){
				System.out.println("== evaluating some node pairs ==");
				coeffSome = graph.evalCoeffsForNodePairs(generatingDag,edgesToEvaluateCoeffs);
			}
			graph.printCoefficientEvaluation();
		}
		
		return new EvaluationResult(methodName,adj,ori,coeffAll,coeffSome);
		
	}



	private static String truncate(DoubleMatrix2D mat) {
		int MAX_LENGTH = 15;
		int n = (mat.columns()>MAX_LENGTH) ? MAX_LENGTH : mat.columns();
		DoubleMatrix2D res = new DenseDoubleMatrix2D(mat.rows(),n);
		for(int j=0; j<n; j++)
			res.viewColumn(j).assign(mat.viewColumn(j));
		return res.toString();
	}



	//what simulate () should do:
	// errorTerms <- nodes with zero parents;
	// generate values for nodes in errorTerms
	// generate values for the other nodes (breadth-first traversal: by highest depth)
	// return the matrix
	//EdgeListGraph has nodes and edges

	//simulate once
	public static DoubleMatrix1D simulate(PatternWithParameters dwp, Distribution distribution) {

		DagWithParmsAndValues dpv = new DagWithParmsAndValues(dwp);

		Dag dag = (Dag) dwp.graph;
		HashMap<Edge,Double> weightHash = dwp.weightHash;
		HashMap<Node,Double> valueHash = dpv.valueHash;


		//first generate values for the error terms
		List<Node> exogenousTerms = dag.getExogenousTerms();
		for (int i=0; i<exogenousTerms.size(); i++){
			Node node = exogenousTerms.get(i);
			double value = distribution.nextRandom();
			//double value = (new java.util.Random()).nextGaussian();
			valueHash.put(node, value);
		}

//		System.out.println("after adding roots, valueHash = " + valueHash);


		//for each child
		//assuming parents have values
		//
		//for each (parent
		List<Node> nodes = dwp.graph.getNodes();


		List<Node> nodesWithoutValue = setMinus(nodes, exogenousTerms);
		int count=0;
		wLoop: while(!allHaveValues(valueHash,nodes)&&count<20){ //while some node does not have values
			fLoop: for (int i=0; i<nodesWithoutValue.size(); i++){ //try to add value to node
//				System.out.println("i = " + i + ":  valueHash = " + valueHash);
//				System.out.println("        nodesWithoutValue = " + nodesWithoutValue);

				Node node = nodesWithoutValue.get(i);
//				System.out.println("        node = " + node);

				List<Node> parents = dag.getParents(node);
//				System.out.println("               parents = " + parents);

				if (allHaveValues(valueHash,parents)){
//					System.out.println("                all parents have values!");
					double nodeValue = distribution.nextRandom();  //double nodeValue = generateUniform(-1,1);
					//double nodeValue = (new java.util.Random()).nextGaussian();

					for (int j=0; j<parents.size();j++){
						Node parent = parents.get(j);
						double parentValue = valueHash.get(parent);
						Edge edge = dag.getDirectedEdge(parent, node);
						//System.out.println("edge from "+ node + " to " + parent + " is " + dag.getDirectedEdge(node, parent));
						//System.out.println("edge from "+ parent + " to " + node + " is " + dag.getDirectedEdge(parent, node));

						double edgeWeight = weightHash.get(edge);
						nodeValue += edgeWeight*parentValue;
					}
					dpv.valueHash.put(node, nodeValue);
					nodesWithoutValue.remove(node);
					i--;
				}
				else{
//					System.out.println("                some parents don't have values!");
					continue fLoop;
				}
			}
		count++;
		}

		DoubleMatrix1D vector = new DenseDoubleMatrix1D(dwp.graph.getNumNodes());

		for (int i=0; i<nodes.size(); i++){ //add each node's value
			Node node = nodes.get(i);
			//System.out.println("node = " + node);
			vector.set(i, dpv.valueHash.get(node));
		}

//		System.out.println("simulate returning vector = " + vector);
		return vector;
	}




	private static List<Node> setMinus(List<Node> l1, List<Node> l2) {
		List<Node> result = new Vector<Node>(l1);
		for (int i=0; i<l2.size(); i++){
			result.remove(l2.get(i));
		}
		return result;
	}



	public static DoubleMatrix2D simulate(PatternWithParameters dwp, int n, Distribution distribution) {		        	
		DoubleMatrix2D vectors = new DenseDoubleMatrix2D(dwp.graph.getNumNodes(),n);
		for(int j=0; j<n; j++){
			DoubleMatrix1D vector = simulate(dwp, distribution);
			vectors.viewColumn(j).assign(vector);        		
		}
		return vectors;
	}

	//do all nodes have a value in weightHash?
	private static boolean allHaveValues(HashMap weightHash, List<Node> nodes) {
		for (int i=0; i<nodes.size(); i++){
			Node node = nodes.get(i);
			if (weightHash.get(node)==null)
				return false;
		}
		return true;
	}


	private static DoubleMatrix2D example_line(int n) {

		DoubleMatrix1D e1 = generateUniform(0,1,n);
		DoubleMatrix1D e2 = generateUniform(0,1,n);
		DoubleMatrix1D e3 = generateUniform(0,1,n);

		DoubleMatrix1D x1 = e1;
		DoubleMatrix1D x2 = linearCombination(x1,1,e2,0.07);
		DoubleMatrix1D x3 = linearCombination(x2,1,e3,0.05);

		DoubleMatrix1D[] observedVars = {x1,x2,x3};

		return combine(observedVars);
	}


	private static DoubleMatrix2D example_Y(int n) {
		DoubleMatrix1D e1 = generateUniform(0,1,n);
		DoubleMatrix1D e2 = generateUniform(0,1,n);
		DoubleMatrix1D e3 = generateUniform(0,1,n);
		DoubleMatrix1D e4 = generateUniform(0,1,n);
		DoubleMatrix1D e5 = generateUniform(0,1,n);

		DoubleMatrix1D x1 = e1;
		DoubleMatrix1D x2 = e2;
		DoubleMatrix1D x3 = linearCombination(linearCombination(x1,1,x2,2),1,e3,1);
		DoubleMatrix1D x4 = linearCombination(x3,1.5,e4,1);

		DoubleMatrix1D x5 = linearCombination(x1,1,e5,1);

		DoubleMatrix1D[] observedVars = {x1,x2,x3,x4,x5};

		return combine(observedVars);

	}

	private static DoubleMatrix2D example_completeDag(int n) {
		//DoubleMatrix1D[] vars;
		//double[] ws;

		DoubleMatrix1D e1 = generateUniform(0,1,n);
		DoubleMatrix1D e2 = generateUniform(0,1,n);
		DoubleMatrix1D e3 = generateUniform(0,1,n);
		DoubleMatrix1D e4 = generateUniform(0,1,n);

		DoubleMatrix1D x1 = e1;

		DoubleMatrix1D[] vars2 = {x1,e2}; double[] ws2 = {2.0,1.0};
		DoubleMatrix1D x2 = linearCombination(vars2, ws2);

		DoubleMatrix1D[] vars3 = {x1,x2,e3}; double[] ws3 = {3.0,0.0,1.0};
		DoubleMatrix1D x3 = linearCombination(vars3, ws3);

		DoubleMatrix1D[] vars4 = {x1,x2,x3,e4}; double[] ws4 = {1.0,1.0,-1.0,1.0};
		DoubleMatrix1D x4 = linearCombination(vars4, ws4);

		DoubleMatrix1D[] observedVars = {x1,x2,x3,x4};
//		DoubleMatrix1D[] observedVars = {x4,x3,x2,x1};   //


		return combine(observedVars);

	}

	private static DoubleMatrix2D example_completeDagSG(int n) {
		//DoubleMatrix1D[] vars;
		//double[] ws;

		DoubleMatrix1D e1 = generateSquaredGaussian(n);
		DoubleMatrix1D e2 = generateSquaredGaussian(n);
		DoubleMatrix1D e3 = generateSquaredGaussian(n);
		DoubleMatrix1D e4 = generateSquaredGaussian(n);

		DoubleMatrix1D x1 = e1;

		DoubleMatrix1D[] vars2 = {x1,e2}; double[] ws2 = {2.0,1.0};
		DoubleMatrix1D x2 = linearCombination(vars2, ws2);

		DoubleMatrix1D[] vars3 = {x1,x2,e3}; double[] ws3 = {3.0,0.0,1.0};
		DoubleMatrix1D x3 = linearCombination(vars3, ws3);

		DoubleMatrix1D[] vars4 = {x1,x2,x3,e4}; double[] ws4 = {1.0,1.0,-1.0,1.0};
		DoubleMatrix1D x4 = linearCombination(vars4, ws4);

		DoubleMatrix1D[] observedVars = {x1,x2,x3,x4};
//		DoubleMatrix1D[] observedVars = {x4,x3,x2,x1};   //


		return combine(observedVars);

	}

	private static DoubleMatrix2D example_Straight(int n) {
		DoubleMatrix1D e1 = generateFourthGaussian(n);
		DoubleMatrix1D e2 = generateAlmostGaussian(n);

		DoubleMatrix1D x1 = e1;
		DoubleMatrix1D[] vars2 = {x1,e2}; double[] ws2 = {1.0,1.0};
		DoubleMatrix1D x2 = linearCombination(vars2, ws2);

		DoubleMatrix1D[] observedVars = {x1,x2};
		return combine(observedVars);
	}



	private static DoubleMatrix2D example_Reverse(int n) {
		DoubleMatrix1D e1 = generateAlmostGaussian(n);
		DoubleMatrix1D e2 = generateFourthGaussian(n);

		DoubleMatrix1D x1 = e1;
		DoubleMatrix1D[] vars2 = {x1,e2}; double[] ws2 = {1.0,1.0};
		DoubleMatrix1D x2 = linearCombination(vars2, ws2);

		DoubleMatrix1D[] observedVars = {x1,x2};
		return combine(observedVars);
	}



//	private static DoubleMatrix2D combine(DoubleMatrix1D vec1, DoubleMatrix1D vec2) {
//	DoubleMatrix2D resultMatrix = new DenseDoubleMatrix2D(2, vec1.size());
//	resultMatrix.viewRow(0).assign(vec1);
//	resultMatrix.viewRow(1).assign(vec2);
//	return resultMatrix;
//	}

	private static DoubleMatrix2D combine(DoubleMatrix1D[] vecs) {
		DoubleMatrix2D resultMatrix = new DenseDoubleMatrix2D(vecs.length, vecs[0].size());
		for (int i=0; i<vecs.length; i++){ //for each vector
			resultMatrix.viewRow(i).assign(vecs[i]);
		}
		return resultMatrix;
	}

	private static DoubleMatrix1D linearCombination(DoubleMatrix1D a, double aw, DoubleMatrix1D b, double bw) {
		DoubleMatrix1D resultMatrix = new DenseDoubleMatrix1D(a.size());
		for (int i=0; i<a.size(); i++){
			resultMatrix.set(i, aw*a.get(i) + bw*b.get(i));
		}
		return resultMatrix;
	}

	private static DoubleMatrix1D linearCombination(DoubleMatrix1D[] vecs, double[] weights) {
		//the elements of vecs must be vectors of the same size
		DoubleMatrix1D resultMatrix = new DenseDoubleMatrix1D(vecs[0].size());

		for (int i=0; i<vecs[0].size(); i++){ //each entry
			double sum=0;
			for (int j=0; j<vecs.length; j++){ //for each vector
				sum+=vecs[j].get(i)*weights[j];
			}
			resultMatrix.set(i, sum);
		}
		return resultMatrix;
	}



	private static DoubleMatrix2D linearCombination(DoubleMatrix2D a, double aw, DoubleMatrix2D b, double bw) {

		for (int i=0; i<a.rows(); i++){
			//System.out.println("b.get("+i+","+i+") = " + b.get(i,i));
		}


		DoubleMatrix2D resultMatrix = new DenseDoubleMatrix2D(a.rows(), a.columns());
		for (int i=0; i<a.rows(); i++){
			for (int j=0; j<a.columns(); j++){
				resultMatrix.set(i,j, aw*a.get(i,j) + bw*b.get(i,j));
				if (i==j){
//					System.out.println("entry (" + i + ","+ i+ ")   is the sum of ");
//					System.out.println("aw*a.get("+i+","+j+") = " + aw*a.get(i,j) + " and");
//					System.out.println("bw*b.get("+i+","+j+") = " + bw*b.get(i,j));
//					System.out.println(", which is " + (aw*a.get(i,j) + bw*b.get(i,j)));
//					System.out.println("");
//					System.out.println("bw = " + bw);
//					System.out.println("b.get("+i+","+j+") = " + b.get(i,j));
//					System.out.println("");
//					System.out.println("");

				}

			}
		}
		return resultMatrix ;
	}





	//reads a matrix written by R
	private static RectangularDataSet readData(String filename) {
		DataParser p = new DataParser();
		
		File inFile = new File(filename);

//		String thisLine;
		try {
			return p.parseTabular(inFile);
		}
//		Vector v0 = new Vector();
//		Vector v1 = new Vector();
//
//			FileInputStream fin =  new FileInputStream(inFile);
//			BufferedReader myInput = new BufferedReader(new InputStreamReader(fin));
//
//			myInput.readLine(); //throw out first line
//
//			while ((thisLine = myInput.readLine()) != null) {
//				//System.out.println("thisLine = " + thisLine);
//				String[] spl = thisLine.trim().split(" +"); //" +" means "any number of spaces"
//				//System.out.println("spl[0] = " + spl[0] + "     spl[1] = " + spl[1] );
//				v0.add(new Double(spl[1]));
//				v1.add(new Double(spl[2]));
//			}
//		}
		catch (Exception e) {
			e.printStackTrace();
		}

//		//String[] spl = bufferStr.split(" ");
//		////System.out.println("spl = " + spl);
//		//v0.add(spl[0]);
//		//v1.add(spl[1]);
//
//		DoubleMatrix2D dataMatrix = new DenseDoubleMatrix2D(2,v0.size());
//		for (int i=0; i<v0.size(); i++){
//			dataMatrix.set(0, i, (Double) v0.get(i));
//			dataMatrix.set(1, i, (Double) v1.get(i));
//		}
//
//		return dataMatrix;
		return null;
	}


	public static DoubleMatrix2D generateBimodal2DGaussian(int n) {
		DoubleMatrix2D vectors = new DenseDoubleMatrix2D(2,n);
		double x, y;

		for (int i=0; i<n; i++){
			if(Math.random()<0.5){
				x = Gaussian_invcdf(Math.random()) - 10;
				y = Gaussian_invcdf(Math.random());
			}
			else{
				x = Gaussian_invcdf(Math.random()) + 10;
				y = Gaussian_invcdf(Math.random());
			}
			vectors.set(0,i,x);
			vectors.set(1,i,y);
		}
		return vectors;
	}



	public static DoubleMatrix2D generate2DGaussian(int n) {
		DoubleMatrix2D vectors = new DenseDoubleMatrix2D(2,n);
		java.util.Random r = new java.util.Random();
		for (int i=0; i<n; i++){
//			double x = Gaussian_invcdf(Math.random());
//			double y = Gaussian_invcdf(Math.random());
//			vectors.set(0,i,x);
//			vectors.set(1,i,y);
			vectors.set(0, i, r.nextGaussian());
			vectors.set(1, i, r.nextGaussian());
		}
		return vectors;
	}


	public static DoubleMatrix1D generateAlmostGaussian(int n) {
		DoubleMatrix1D points = new DenseDoubleMatrix1D(n);
		java.util.Random r = new java.util.Random();

		for (int i=0; i<n; i++){
			if (r.nextDouble()<.95)
				points.set(i, r.nextGaussian());
			else
				points.set(i, r.nextGaussian()+2);
		}

		return points;
	}



	public static DoubleMatrix1D generateGaussian(int n) {
		DoubleMatrix1D points = new DenseDoubleMatrix1D(n);
		java.util.Random r = new java.util.Random();

		for (int i=0; i<n; i++)
			points.set(i, r.nextGaussian());

		return points;
	}


	public static DoubleMatrix1D generateSquaredGaussian(int n) {
		DoubleMatrix1D points = new DenseDoubleMatrix1D(n);
		java.util.Random r = new java.util.Random();

		for (int i=0; i<n; i++)
			points.set(i, Math.pow(r.nextGaussian(),2));

		return points;
	}


	public static DoubleMatrix1D generateFourthGaussian(int n) {
		DoubleMatrix1D points = new DenseDoubleMatrix1D(n);
		java.util.Random r = new java.util.Random();

		for (int i=0; i<n; i++)
			points.set(i, Math.pow(r.nextGaussian(),4));

		return points;
	}

	public static double generateUniform(double a, double b) {
		double length = b - a;
		double x = a + length*Math.random();
		return x;
	}


	public static DoubleMatrix1D generateUniform(double a, double b, int n) {
		DoubleMatrix1D points = new DenseDoubleMatrix1D(n);
		double length = b - a;

		for (int i=0; i<n; i++){
			double x = a + length*Math.random();
			points.set(i, x);
		}
		return points;
	}


	//d is in the range [0,1]
	private static double Gaussian_invcdf(double x) {

		//at .5398, the fn is at 0.1; at .9987, the fn is at 3.0
		double[] table = { 0.0000, 0.0398, 0.0793, 0.1179, 0.1554, 0.1915, 0.2257, 0.2580, 0.2881,
				0.3159, 0.3413, 0.3643, 0.3849, 0.4032, 0.4192, 0.4332, 0.4452, 0.4554, 0.4641,
				0.4713, 0.4772, 0.4821, 0.4861, 0.4893, 0.4918, 0.4938, 0.4953, 0.4965, 0.4974,
				0.4981, 0.4987, 0.5}; //assuming that at 3.1

		//identify where d falls in the table.
		if (x>0.5){
			int index = getLastSmallerIndex(x-0.5, table);

			//returns a value between 0.1*index and 0.1*(index+1)
			double interpol = interpolate(0.5+table[index],0.5+table[index+1],
					index*0.1,(index+1)*0.1,x); //weight

			System.out.println("x = "+ x +"    interpol = " + interpol);

			return interpol;
		}
		else{
			double ix = 1-x;

			int index = getLastSmallerIndex(ix-0.5, table);

			double interpol = interpolate(0.5+table[index],0.5+table[index+1],
					index*0.1,(index+1)*0.1,ix); //weight

			System.out.println("x = "+ x +"    -interpol = " + -interpol);
			return -interpol;
		}

	}


	//does a linear interpolation, assuming x1 < x < x2
	//returns the y-value corresponding to 'x'.
	private static double interpolate(double x1, double x2, double y1, double y2, double x) {
		//0.1*index is between d and e
		double dx = x2 - x1;
		double dy = y2 - y1;
		double slope = dy/dx;
		return y1 + slope*(x - x1);
	}


	private static int getLastSmallerIndex(double d, double[] table) {
		for (int i=0; i<table.length; i++)
			if (table[i]>d)
				return i - 1;
		return -1;
	}



	public static final class Invert implements DoubleFunction {
		public double apply(double d){
			return 1/d;
		}
	}


//	private static void testColt(DoubleMatrix2D inVectors) {
//	try{
//	System.out.println("");
//	System.out.println("================================================");
//	System.out.println("================================================");
//	System.out.println("============= testing FastIca Colt  ============");
//	//System.out.println("inVectors = " + inVectors);
//	long sTime = (new Date()).getTime();
//	FastIca fica = new FastIca(inVectors, inVectors.rows());
//	long eTime = (new Date()).getTime();
//	System.out.println("Colt-based ICA took " + (eTime-sTime) + "ms");
//	System.out.println("\nfica.getICVectors(): " + fica.getICVectors());
//	System.out.println("\nWeight matrix: " + fica.getWeightMatrix());
//	System.out.println("\nMixing matrix: " + fica.getMixingMatrix());
//	System.out.println("\nNormalized mixing matrix: " + normalizeDiagonal(fica.getMixingMatrix()));

//	}
//	catch(Exception e){
//	e.printStackTrace();
//	}		

//	}


	public static DoubleMatrix2D convertToColt(double[][] vectors) {
		int m = Matrix.getNumOfRows(vectors);
		int n = Matrix.getNumOfColumns(vectors);

		DoubleMatrix2D mat = new DenseDoubleMatrix2D(m,n);
		for (int i=0; i<m; i++)
			for (int j=0; j<n; j++)
				mat.set(i, j, vectors[i][j]);

		return mat;
	}

	public static double[] convert(DoubleMatrix1D vector) {
		int n = vector.size();
		double[] v = new double[n];
		for (int i=0; i<n; i++)
			v[i] = vector.get(i);		
		return v;
	}

	public static double[][] convert(DoubleMatrix2D inVectors) {
		if (inVectors==null) return null;

		int m = inVectors.rows();
		int n = inVectors.columns();

		double[][] inV = new double[m][n];
		for (int i=0; i<m; i++)
			for (int j=0; j<n; j++)
				inV[i][j] = inVectors.get(i, j);

		return inV;
	}


}
