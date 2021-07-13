using System;
using System.Collections.Generic;
using System.Text;
using MathLib.Statistics.DataItems;

namespace MathLib.Statistics.Analysis
{

    public class GlsMultipleLinearRegression
    {
        private Variable dependentVar;
        private Variable[] independentVars;
        private Matrix omega;

        GlsMultipleLinearRegression(Variable dependentVar, Variable[] independentVars)
        {
            this.dependentVar = dependentVar;
            this.independentVars = independentVars.Clone();
        }

        /// <summary>
        /// Calculates beta by GLS:
        /// <pre>
        /// b=(X' Omega^-1 X)^-1X'Omega^-1 y
        /// </pre>
        /// </summary>
        /// <returns></returns>
        protected Matrix calculateBeta()
        {
            Matrix OI = omega.Inverse;
            Matrix XT = X.transpose();
            Matrix XTOIX = XT.multiply(OI).multiply(X);
            return XTOIX.inverse().multiply(XT).multiply(OI).multiply(Y);
        }

        /// <summary>
        /// Calculates the variance on the beta by GLS:
        /// <pre>
        /// Var(b)=(X' Omega^-1 X)^-1
        /// </pre>      
        /// </summary>
        /// <returns>The beta variance matrix</returns>
        protected Matrix calculateBetaVariance()
        {
            RealMatrix XTOIX = X.transpose().multiply(omega.inverse()).multiply(X);
            return XTOIX.inverse();
        }

        /// <summary>
        /// Calculates the Y variance.
        /// Calculates the variance on the y by GLS:
        /// <pre>
        /// Var(y)=Tr(u' Omega^-1 u)/(n-k)
        /// </pre>
        /// </summary>
        /// <returns>The Y variance</returns>

        protected double calculateYVariance()
        {
            RealMatrix u = calculateResiduals();
            RealMatrix sse = u.transpose().multiply(omega.inverse()).multiply(u);
            return sse.getTrace() / (X.getRowDimension() - X.getColumnDimension());
        }

    }

    class GLSMultipleLinearRegressionEstimatorn
    {
    }
}
