using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using MathLib.Statistics.DataItems;

namespace MathLib.Statistics.Troep
{
    class MultipleLinearRegression
    {

        // *********************** COPYRIGHT © 1997 STEFAN WANER *********
        // *********************** ALL RIGHTS RESERVED *********************

        const int N = 0;		// number of data points entered 
        const int maxN = 16;	// maximum number of data points possible
        const int M = 4; 		// number of independent variables 
        decimal[,] X;
        decimal[] Y;
        int SX = 0;
        int SY = 0;
        int SXX = 0;
        int SXY = 0;
        int SYY = 0;
        int m = 0; 		// degree of polymonial regression
        bool abort = false;

        Variable var1;
        Variable var2;

        public MultipleLinearRegression(Variable variable1, Variable variable2)
        {
            X = makeArray2(M, maxN);
        }

        decimal[] regrCoeff = new decimal[M];
        int sigDig = 10;	// accuracy 

        private decimal[,] makeArray2(int X, int Y)
        {
            decimal[,] result = new decimal[X + 1, Y + 1];
            for (int count = 0; count <= X + 1; count++)
                for (int count2 = 0; count2 <= Y + 1; count2++)
                    result[count, count2] = 0;
            return result;
        } // makeArray2

        private decimal det(decimal[,] A)
        {
            decimal sum = 0;
            int Length = A.Length - 1;
            // formal length of a matrix is one bigger
            if (Length == 1) return (A[1, 1]);
            else
            {
                int factor = 1;
                for (int i = 1; i <= Length; i++)
                {
                    if (A[1, i] != 0)
                    {
                        // create the minor
                        decimal[,] minor = makeArray2(Length - 1, Length - 1);
                        int theColumn;
                        for (int m = 1; m <= Length - 1; m++) // columns
                        {
                            if (m < i) theColumn = m;
                            else theColumn = m + 1;
                            for (int n = 1; n <= Length - 1; n++)
                            {
                                minor[n, m] = A[n + 1, theColumn];
                                // alert(minor[n][m]);
                            } // n
                        } // m
                        // compute its determinant
                        sum = sum + A[1, i] * factor * det(minor);
                    }
                    factor = -factor;	// alternating sum
                } // end i
            } // recursion
            return (sum);
        } // end determinant

        private decimal[,] inverse(decimal[,] A)
        {
            int Length = A.Length - 1;
            decimal[,] B = makeArray2(Length, Length);  // inverse
            decimal d = det(A);
            if (d == 0) System.Diagnostics.Debug.Write("singular matrix--check data");
            else
            {
                for (int i = 1; i <= Length; i++)
                {
                    for (int j = 1; j <= Length; j++)
                    {
                        // create the minor
                        decimal[,] minor = makeArray2(Length - 1, Length - 1);
                        int theColumn;
                        int theRow;
                        for (int m = 1; m <= Length - 1; m++) // columns
                        {
                            if (m < j) theColumn = m;
                            else theColumn = m + 1;
                            for (int n = 1; n <= Length - 1; n++)
                            {
                                if (n < i) theRow = n;
                                else theRow = n + 1;
                                minor[n, m] = A[theRow, theColumn];
                                // alert(minor[n][m]);
                            } // n
                        } // m
                        // inverse entry
                        double temp = (i + j) / 2;
                        int factor;
                        if (temp == Math.Round(temp)) factor = 1;
                        else factor = -1;

                        B[j, i] = det(minor) * factor / d;


                    } // j

                } // end i
            } // recursion
            return (B);
        } // end inverse

        public decimal shiftRight(decimal theNumber, int k)
        {
            int k2 = 0;
            if (k == 0) return (theNumber);
            else
            {
                k2 = 1;
                if (theNumber < 0) theNumber = -theNumber;
                for (int i = 1; i <= theNumber; i++)
                {
                    k2 = k2 * 10;
                }
            }
            if (k > 0)
            { return (k2 * theNumber); }
            else
            { return (theNumber / k2); }
        }

        decimal roundSigDig(decimal theNumber, decimal numDigits)
        {
            if (theNumber == 0) return (0);
            else if (Math.Abs(theNumber) < (decimal)0.000000000001) return (0);
            // warning: ignores numbers less than 10^(-12)
            else
            {
                int k = (int)Math.Floor(Math.Log((double)Math.Abs(theNumber)) / Math.Log(10)) - (int)numDigits;
                decimal k2 = shiftRight(Math.Round(shiftRight(Math.Abs(theNumber), -k)), k);
                if (theNumber > 0) return (k2);
                else return (-k2);
            } // end else
        }
    }
}















/* 
 function buildxy()  {
 e = 2.718281828459045;
 pi = 3.141592653589793;	
 abort = false;
 with (Math)
     {
     N = 0; 		// number of data points
     var searching = true;
     var numvariables = 4;
     if (document.theForm[4+6*1].value == "") numvariables = 1;
     else if (document.theForm[5+6*1].value == "") numvariables = 2;
     else if (document.theForm[6+6*1].value == "") numvariables = 3;
     else numvariables = 4;
		
     for (var i = 0; i <= 15; i++)			// arrays start at 0
         {
		
         theString1 = stripSpaces(document.theForm[3+6*i].value);	
         if (theString1 == "") searching = false;
         theString2 = stripSpaces(document.theForm[4+6*i].value);	
         if ( (numvariables >= 2) && (theString2 == "") ) searching = false;
         theString3 = stripSpaces(document.theForm[5+6*i].value);	
         if ( (numvariables >= 3) && (theString3 == "") ) searching = false;
         theString4 = stripSpaces(document.theForm[6+6*i].value);	
         if ( (numvariables >= 4) && (theString4 == "") ) searching = false;
			
         if ( (searching) && (!abort) )
             { 
             N++;
             X[1][N] = eval(theString1);
             if (numvariables >= 2) X[2][N] = eval(theString2); 
             if (numvariables >= 3) X[3][N] = eval(theString3); 
             if (numvariables >= 4) X[4][N] = eval(theString4);
             theString = stripSpaces(document.theForm[7+6*i].value);
             if (theString == "") {alert("You have not entered a y-value for data point number "+N); abort = true}
             else Y[N] = eval(theString);
             }
	
         } // of i = 1 to 15
     } // end of with math
 M = numvariables;		// the numer of active variables
 if (!abort)
     {
     if (N == 0) {alert("Enter data first"); abort = true} 
     else if (N < M+1) {alert("You have entered too few data points"); abort = true}
     } // if !abort
 }

function linregr()
 {
 if (!abort) {
     e = 2.718281828459045;
     pi = 3.141592653589793;
     var k;
     var i;
     var j;
     var sum;
		
     B = new makeArray(M+1);
     P = new makeArray2(M+1, M+1);
     invP = new makeArray2(M+1, M+1);
     var mtemp = M+1;
//		if (N < M+1) alert("your need at least "+ mtemp +" points");
     with (Math)
         {
         // First define the matrices B and P
         for (i = 1; i <=  N; i++) X[0][i] = 1;
         for (i = 1; i <= M+1; i++)
             {
             sum = 0;
             for (k = 1; k <= N; k++) sum = sum + X[i-1][k]*Y[k];
             B[i] = sum;
				
             for (j = 1; j <= M+1; j++) 
                 { 
                 sum = 0;
                 for (k = 1; k <= N; k++) sum = sum + X[i-1][k]*X[j-1][k];
                 P[i][j] = sum;
                 }
             } // i
// begin debug display
// invP = inverse(P);
// var count = 0;
// for(i = 1; i<=M+1; i++) 
//	{
//	for (j = 1; j <=M+1; j++)
//		{count++;
//		document.theForm2[count-1].value = invP[i][j];}
//	}
// for (j = 1; j <=M+1; j++)
//		{count++;
//		document.theForm2[count-1].value = B[j];}
// end debug display

         invP = inverse(P);	
         for (k = 0; k <= M; k++)
             {
             sum = 0;
	
             for (j = 1; j <= M+1; j++)
                 {
                 sum = sum + invP[k+1][j]*B[j];
                 } // j 
// alert("here");
             regrCoeff[k] = sum;
             } // k
         } // end of with math
     } // end of if not abort
 }

function calc(){
	
 var num = calc.arguments[0];

 //**********

 // Option 1	
 if (num == 1)
     {
     }

 // Option 2
 else  if (num == 2)
     {
		
     }
	
 // Option 3
 else  if (num == 3)
     {
     }


 // Option 4
else  if (num == 4)
 {
 } // of this option

 // Option 5 linear regression
 else  if (num == 5)
 {
 with (Math)
     {
     buildxy();
// alert("here");
     linregr();
     var output = "y = " + roundSigDig(regrCoeff[0], sigDig);
     for (i = 1; i <=M; i++)
         output += " + " + roundSigDig(regrCoeff[i], sigDig) + "x" + i ;
     document.theForm.output.value = output;
	
     // now post the predicted values
     for ( i = 0; i <= N-1; i++)			// arrays start at 0
         {
         y = regrCoeff[0];
         for (j = 1; j <=M; j++) y +=regrCoeff[j]*X[j][i+1];
         document.theForm[8+6*i].value = roundSigDig(y, sigDig);
         } // of i = 1 to 10
     // now blank out the rest of the predicted values
     for (i = N; i <= maxN-1; i++) document.theForm[8+6*i].value = "";
     } // end of with math
 } // end option 5

 // Option 6
 else  if (num == 6)
     {
     }
			
}
*/

