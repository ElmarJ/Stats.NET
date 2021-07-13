package edu.cmu.tetrad.search;

import java.util.HashMap;
import java.util.List;
import java.util.Vector;

import edu.cmu.tetrad.data.RectangularDataSet;
import edu.cmu.tetrad.graph.Dag;
import edu.cmu.tetrad.graph.Edge;
import edu.cmu.tetrad.graph.Graph;
import edu.cmu.tetrad.graph.GraphUtils;
import edu.cmu.tetrad.graph.Node;
import edu.cmu.tetrad.search.EvaluationResult.AdjacencyEvaluationResult;
import edu.cmu.tetrad.search.EvaluationResult.CoefficientEvaluationResult;
import edu.cmu.tetrad.search.EvaluationResult.OrientationEvaluationResult;
import edu.cmu.tetrad.sem.SemEstimator;
import edu.cmu.tetrad.sem.SemIm;
import edu.cmu.tetrad.sem.SemPm;


//Dag plus edge weights     //later include distributions for the error terms
public class PatternWithParameters{
	//a Dag has a list of edges
	//therefore, Hashmap from edges to weights

	public Graph graph;
	
	public String generatingMethodName = null;
	public String getGeneratingMethodName(){
		return generatingMethodName;
	}
	
	public HashMap<Edge,Double> weightHash;

//	public Dag patDag = null; //only non-null when graph is a pattern

	/*
	 * estimate the weights for the nodes that have all parents determined.
	 */
	//it would have been more efficient to only regression on the nodes that matter
	public PatternWithParameters(SemIm semIm, Graph truePattern) {
//		Graph g = (truePattern==null) ? semIm.getSemPm().getGraph() : truePattern;
//		this.graph = g;
//		weightHash = new HashMap<Edge,Double>();
		this(truePattern);

		//make the SemIm

		//estimate the weights for the nodes that have all parents determined.
		for (Node node : this.graph.getNodes()){
			if (GraphUtils.allAdjacenciesAreDirected(node,graph)){	//if we know the set of parents of 'node'				

				//steal the coefficients from the SemIm
				for (Edge edge : this.graph.getEdges(node)){
					double semImWeight = semIm.getEdgeCoef(edge);
					this.weightHash.put(edge, semImWeight);
				}
			}
		}
		this.graph = graph;
	}

	public PatternWithParameters(Graph graph) {
		this.graph = graph;
		weightHash = new HashMap<Edge,Double>();
	}

	public String toString(){ //iterate through the edges and print their weight too
		String str="";
		for (Edge edge : graph.getEdges()){
			str+=edge.toString();
			str+="   " + weightHash.get(edge) + "\n";				
		}
		return str;
	}


	int errorsOfOmission=0;
	int errorsOfCommission=0;

	public AdjacencyEvaluationResult evalAdjacency(Dag genDag){
		//for each edge in this DAG, check whether it is in genDag. If it isn't, that's an error of
		//commission.
		for (Edge thisEdge : this.graph.getEdges()){
			System.out.print("thisEdge = " + thisEdge);

			//is it in this DAG?
			Edge genEdge = getCorrespondingEdge(genDag, thisEdge);
			System.out.println(", genEdge = " + genEdge);

			boolean adjCorrect = (genEdge!=null);
			if (!adjCorrect){
				errorsOfCommission++;
			}			
		}

		//for each edge in genDag, check whether it is in this DAG. If it isn't, that's an error of
		//omission.
		for (Edge genEdge : genDag.getEdges()){
			System.out.print("genEdge = " + genEdge);

			//is it in this DAG?
			Edge thisEdge = getCorrespondingEdge(this.graph, genEdge);
			System.out.println(", thisEdge = " + thisEdge);

			boolean adjCorrect = (thisEdge!=null);
			if (!adjCorrect){
				errorsOfOmission++;
			}			
		}			
		return new AdjacencyEvaluationResult(errorsOfOmission,errorsOfCommission);
	}


	public void printAdjacencyEvaluation() {
		System.out.println("== Results of evaluating adjacency ==");
		System.out.println("errorsOfOmission = " + errorsOfOmission);
		System.out.println("errorsOfCommission = " + errorsOfCommission);
	}		


	int oriEvaluated=0;
	int oriCorrect=0;
	int oriIncorrect=0;
	int oriUndirected=0;
	List<Edge> correctOrientationEdges;
	//evaluating orientations
	//should only evaluate on the adjacencies that are correct
	public OrientationEvaluationResult evalOrientations(Dag genDag){
		correctOrientationEdges = new Vector();

		for (Edge genEdge : genDag.getEdges()){

			Edge thisEdge = getCorrespondingEdge(this.graph, genEdge);
			System.out.print("genEdge = " + genEdge);
			System.out.println(", thisEdge = " + thisEdge);

			//skip the ones that are not adjacent
			if (thisEdge==null)
				continue;

			oriEvaluated++;

			if (thisEdge.isDirected()){ //directed: compare direction
				if (getCorrespondingDirectedEdge(this.graph, genEdge)!=null){
					oriCorrect++;
					correctOrientationEdges.add(thisEdge);
				}
				else{
					oriIncorrect++;
				}

			}
			else{ //undirected, do nothing
				oriUndirected++;
			}
		}
		System.out.print("\n");
		return new OrientationEvaluationResult(oriCorrect,oriIncorrect,oriUndirected);
	}


	public void printOrientationEvaluation() {
		System.out.println("== Results of evaluating orientation ==");
		System.out.println("oriCorrect = " + oriCorrect + "  oriIncorrect = " + oriIncorrect + "  oriUndirected = " + oriUndirected );
		System.out.println("oriEvaluated = " + oriEvaluated);
	}



	//evaluating coefficients
	double totalCoeffErrorSq;

	//evaluate every node-pair
	public CoefficientEvaluationResult evalCoeffs(PatternWithParameters genDag) {
		totalCoeffErrorSq=0;

		List<Node> nodes = graph.getNodes();
		for (int i=0; i<nodes.size(); i++){ //iterating through each node pair
			Node node1 = nodes.get(i);
			Node realNode1 = getCorrespondingNode(genDag.graph, node1);
			for (int j=0; j<i; j++){
				Node node2 = nodes.get(j);	
				Node realNode2 = getCorrespondingNode(genDag.graph, node2);
				
				System.out.println("node1 = " + node1 + "  node2 = " + node2);				
				double coeff12 = getDirectedEdgeCoeff(node1,node2);
				double realCoeff12 = genDag.getDirectedEdgeCoeff(realNode1,realNode2);
				double err12 = java.lang.Math.pow(coeff12 - realCoeff12, 2);
				System.out.println("err12 = " + err12);

				double coeff21 = getDirectedEdgeCoeff(node2,node1);
				double realCoeff21 = genDag.getDirectedEdgeCoeff(realNode2,realNode1);
				double err21 = java.lang.Math.pow(coeff21 - realCoeff21, 2);
				System.out.println("err21 = " + err21);
				
				double error = err12 + err21;
				System.out.println("error = " + error);
				
				totalCoeffErrorSq+=error;
			}
		}
		
		return new CoefficientEvaluationResult(totalCoeffErrorSq, null);
	}
	
	
	//we call this, passing the edges that PC evaluates
	/**
	 * evalute coefficients for some node pairs
	 * @param genDag
	 * @param edges edges from the pattern returned by PC-search
	 */
	public CoefficientEvaluationResult evalCoeffsForNodePairs(PatternWithParameters genDag, List<Edge> edges) {

		totalCoeffErrorSq=0;

		//turn them into 'graph' edges	
		for (Edge edge : edges){
			Node node1Edges = edge.getNode1();
			Node node2Edges = edge.getNode2();

			System.out.println("node1Edges = " + node1Edges + "  node2Edges = " + node2Edges);				
			Node node1this = getCorrespondingNode(this.graph, node1Edges);
			Node node2this = getCorrespondingNode(this.graph, node2Edges);
			double coeff12 = getDirectedEdgeCoeff(node1this,node2this);
			Node node1gen = getCorrespondingNode(genDag.graph, node1Edges);
			Node node2gen = getCorrespondingNode(genDag.graph, node2Edges);
			double realCoeff12 = genDag.getDirectedEdgeCoeff(node1gen,node2gen);
			double err12 = java.lang.Math.pow(coeff12 - realCoeff12, 2);
			System.out.println("err12 = " + err12);

			double coeff21 = getDirectedEdgeCoeff(node2this,node1this);
			double realCoeff21 = genDag.getDirectedEdgeCoeff(node2gen,node1gen);
			double err21 = java.lang.Math.pow(coeff21 - realCoeff21, 2);
			System.out.println("err21 = " + err21);
			
			double error = err12 + err21;
			System.out.println("error = " + error);
			
			totalCoeffErrorSq+=error;
		}
		return new CoefficientEvaluationResult(totalCoeffErrorSq,edges.size());
	}

	
	
	private double getDirectedEdgeCoeff(Node node1, Node node2) {
		double result;
		Edge edge = graph.getDirectedEdge(node1, node2);
		if (edge==null)
			result = 0;
		else
			result = weightHash.get(edge);  //weightHash is null!
		return result;
	}

	//should only evaluate those whose orientation is correct
	public void evalCoeffsCorrectOrientation(PatternWithParameters genDag) {
		List<Edge> edgesToEvaluate;
//		if (patDag!=null) //we use it
//		{
//		edgesToEvaluate = new Vector();
//		//add only the patDag edges whose orientation is correct
//		for (Edge patDagEdge : patDag.getEdges()){
//		Edge genEdge = getCorrespondingEdge(genDag.graph,patDagEdge);

//		if (genEdge!=null && oriAgrees(patDagEdge,genEdge))
//		edgesToEvaluate.add(getCorrespondingEdge(this.graph,patDagEdge));
//		}
//		}
//		else 
		edgesToEvaluate = correctOrientationEdges;

		System.out.println("correctOrientationEdges = " + correctOrientationEdges);
		for (Edge edge : edgesToEvaluate){
			double thisCoeff = this.weightHash.get(edge);
			Edge genEdge = getCorrespondingEdge(genDag.graph, edge);
			double genCoeff = genDag.weightHash.get(genEdge);
			double diff = thisCoeff - genCoeff;
			System.out.println("thisEdge " + edge + ": " + thisCoeff + "   err = " + diff);
			totalCoeffErrorSq+=java.lang.Math.pow(diff, 2);
		}

	}

	//either both point to the left or both point to the right
	private boolean oriAgrees(Edge edge1, Edge edge2) {
		int count=0;
		System.out.println();
		if (edge1.pointingLeft(edge1.getEndpoint1(), edge1.getEndpoint2()))
			count++;			
		if (edge2.pointingLeft(edge1.getEndpoint1(), edge2.getEndpoint2()))
			count++;
		return (count%2)==0;
	}

	public void printCoefficientEvaluation() {
		System.out.println("== Results of evaluating coefficients ==");
		System.out.println("totalCoeffErrorSq = " + totalCoeffErrorSq);
	}

	public static Node getCorrespondingNode(Graph graph, Node node){
		String nodeName = node.getName();
		Node node1 = graph.getNode(nodeName);
		return node1;
	}
	
	//returns the edge of graph corresponding to edge
	public static Edge getCorrespondingEdge(Graph graph, Edge edge){
//		System.out.println("entered getCorrespondingEdge: edge = " + edge);
		Node node1 = getCorrespondingNode(graph, edge.getNode1());
		Node node2 = getCorrespondingNode(graph, edge.getNode2());
		Edge result = graph.getEdge(node1, node2);
		return result;
	}

	//returns the directed edge of graph corresponding to edge
	public static Edge getCorrespondingDirectedEdge(Graph graph, Edge edge){
		if (edge==null)
			return null;
		else {
			String nodeName1 = edge.getNode1().getName();
			String nodeName2 = edge.getNode2().getName();
			Node node1 = graph.getNode(nodeName1);
			Node node2 = graph.getNode(nodeName2);		
			Edge result = graph.getDirectedEdge(node1, node2);
			return result;
		}
	}



	//does the graph have an edge similar to 'edge'?
	private static boolean hasCorrespondingAdjacency(Graph graph, Edge edge) {
		Edge corrEdge = getCorrespondingEdge(graph, edge);
		return corrEdge!=null;
	}

	private static boolean directionAgrees(Graph graph, Edge edge) {
		String edgeDirection = (edge.toString().indexOf(">")==-1) ? "left" : "right";

		String nodeName1 = edge.getNode1().getName();
		String nodeName2 = edge.getNode2().getName();
		Node node1 = graph.getNode(nodeName1);
		Node node2 = graph.getNode(nodeName2);		
		Edge graphEdge = graph.getEdge(node1, node2);

		String graphEdgeDirection = (graphEdge.toString().indexOf(">")==-1) ? "left" : "right";

		return edgeDirection.equals(graphEdgeDirection);
	}
	
	/**
	 * creates a PatternWithParameters by running a regression, given a graph and data
	 * @param dataSet
	 * @param graph
	 * @return
	 */
	public static PatternWithParameters regress(RectangularDataSet dataSet, Graph graph){
		SemPm semPmEstDag = new SemPm(graph);
		SemEstimator estimatorEstDag = new SemEstimator(dataSet,semPmEstDag);
		estimatorEstDag.estimate();
		SemIm semImEstDag = estimatorEstDag.getEstimatedSem();
		PatternWithParameters estimatedGraph = new PatternWithParameters(semImEstDag,graph);
		return estimatedGraph;		
	}


	
}
