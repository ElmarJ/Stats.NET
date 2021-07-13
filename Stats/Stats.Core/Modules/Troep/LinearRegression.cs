using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;

namespace MathLib.Statistics.Troep
{
    class LinearRegression
    {
        public struct Results
        {
            public int SampleSize;
            public double SigmaError;
            public double XRangeL;
            public double XRangeH;
            public double YRangeL;
            public double YRangeH;
            public double StandardError;
            public double a;
            public double b;
            public double XStdDev;
            public double YStdDev;
            public double XMean;
            public double YMean;
            public double PearsonsR;
            public double t;

            public string ToHTML()
            {
                string template = @"
                        y = {0} + {1}x
                        Sample size = {3}
                        X Mean = {4}
                        X Range = {5} - {6}
                        X STDDEV = {7}
                        Y Mean = {8}
                        Y Range = {9} - {10}
                        t = {11}
                        r = {12}
                        r² = {13}";

                StringBuilder builder = new StringBuilder();
                builder.AppendFormat(
                    CultureInfo.CurrentCulture,
                    template,
                    this.a, this.b,
                    this.SampleSize,
                    this.XMean,
                    this.XRangeL, this.XRangeH,
                    this.XStdDev,
                    this.YMean,
                    this.YRangeL,
                    this.YRangeH);
                if (this.PearsonsR * this.PearsonsR < .25)
                {
                    builder.AppendLine("Low r² scores represent LOW correlation between variables !");
                }
                builder.AppendFormat("Total Error = {0}", this.SigmaError);
                builder.AppendLine();
                builder.AppendFormat("Standard Error sqrt((y - y')² / n); = {0}", this.StandardError);
                builder.AppendLine();
                if (this.StandardError > this.YStdDev)
                {
                    builder.AppendLine("THIS IS NOT A REASONABLE PREDICTION AS THE STANDARD ERROR IS LARGER THAN THE FIRST STANDARD DEVIATION OF THE Y VARIABLE");
                }
                return builder.ToString();
            }
        }

        public static Results Regress(DataSet ds, DataColumn xColumn, DataColumn yColumn)
        {
            double sigmax = 0.0;
            double sigmay = 0.0;
            double sigmaxx = 0.0;
            double sigmayy = 0.0;
            double sigmaxy = 0.0;
            double x;
            double y;
            double n = 0;

            Results ret = new Results();

            if (ds != null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    x = double.Parse(dr[xColumn].ToString());
                    y = double.Parse(dr[yColumn].ToString());
                    if (x > ret.XRangeH) ret.XRangeH = x;
                    if (x < ret.XRangeL) ret.XRangeL = x;
                    if (y > ret.YRangeH) ret.YRangeH = y;
                    if (y < ret.YRangeL) ret.YRangeL = y;
                    sigmax += x;
                    sigmaxx += x * x;
                    sigmay += y;
                    sigmayy += y * y;
                    sigmaxy += x * y;
                    n++;
                }
                ret.b = (n * sigmaxy - sigmax * sigmay) / (n * sigmaxx - sigmax * sigmax);
                ret.a = (sigmay - ret.b * sigmax) / n;
                ret.SampleSize = (int)n;

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    //calculate distances for each point (residual)
                    y = double.Parse(dr[yColumn].ToString());
                    x = double.Parse(dr[xColumn].ToString());
                    double yprime = ret.a + ret.b * x; //prediction
                    double Residual = y - yprime;
                    ret.SigmaError += Residual * Residual;
                }
                ret.XMean = sigmax / n;
                ret.YMean = sigmay / n;
                ret.XStdDev = Math.Sqrt(((double)n * sigmaxx - sigmax * sigmax) / ((double)n * (double)n - 1.0));
                ret.YStdDev = Math.Sqrt(((double)n * sigmayy - sigmay * sigmay) / ((double)n * (double)n - 1.0));
                ret.StandardError = Math.Sqrt(ret.SigmaError / ret.SampleSize);
                double ssx = sigmaxx - ((sigmax * sigmax) / n);
                double ssy = sigmayy - ((sigmay * sigmay) / n);
                double ssxy = sigmaxy - ((sigmax * sigmay) / n);
                ret.PearsonsR = ssxy / Math.Sqrt(ssx * ssy);
                ret.t = ret.PearsonsR / Math.Sqrt((1 - (ret.PearsonsR * ret.PearsonsR)) / (n - 2));
            }
            return ret;
        }
    }

}
