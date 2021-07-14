using System;
using System.Collections.Generic;
using System.Text;

namespace MathLib.Statistics.Troep
{
    class Statistics
    {
        private double[] list;

        public Statistics(params double[] list)
        {
            this.list = list;
        }

        public Statistics(IEnumerable<double> list)
        {
            this.list = (double[])list;
        }

        public void Update(params double[] list)
        {
            this.list = list;
        }
        

        public double Mode
        {
            get
            {
                try
                {
                    double[] i = new double[this.list.Length];
                    list.CopyTo(i, 0);
                    Statistics.Sort(i);
                    double valMode = i[0], helpValMode = i[0];
                    int oldCounter = 0, newCounter = 0;
                    int j = 0;
                    for (; j <= i.Length - 1; j++)
                        if (i[j] == helpValMode) newCounter++;
                        else if (newCounter > oldCounter)
                        {
                            oldCounter = newCounter;
                            newCounter = 1;
                            helpValMode = i[j];
                            valMode = i[j - 1];
                        }
                        else if (newCounter == oldCounter)
                        {
                            valMode = double.NaN;
                            helpValMode = i[j];
                            newCounter = 1;
                        }
                        else
                        {
                            helpValMode = i[j];
                            newCounter = 1;
                        }
                    if (newCounter > oldCounter) valMode = i[j - 1];
                    else if (newCounter == oldCounter) valMode = double.NaN;
                    return valMode;
                }
                catch (DivideByZeroException)
                {
                    return double.NaN;
                }
            }
        }

        public int Length
        {
            get
            {
                return this.list.Length;
            }
        }

        public double Min
        {
            get
            {
                double minimum = double.PositiveInfinity;
                foreach (double item in this.list)
                    if (item < minimum) minimum = item;
                return minimum;
            }
        }

        public double Max
        {
            get
            {
                double maximum = double.NegativeInfinity;
                foreach (double item in this.list)
                    if (item > maximum) maximum = item;
                return maximum;
            }
        }

        public double Quarter1
        {
            get { return InnerQuantile(0.25); }
        }

        public double Median
        {
            get { return InnerQuantile(0.5); }
        }

        public double Quarter3
        {
            get { return InnerQuantile(0.75); }
        }

        public double Mean
        {
            get
            {
                try
                {
                    double sum = 0;
                    foreach (double item in this.list)
                        sum += item;
                    return sum / this.list.Length;
                }
                catch (DivideByZeroException)
                {
                    return double.NaN;
                }
            }
        }

        public double Range
        {
            get
            {
                double minimum = this.Min;
                double maximum = this.Max;
                return (maximum - minimum);
            }
        }

        public double IntemediateQuartile
        {
            get { return this.Quarter3 - this.Quarter1; }
        }

        public double MiddleOfRange
        {
            get
            {
                double minimum = Min;
                double maximum = Max;
                return (minimum + maximum) / 2;
            }
        }

        public double Var
        {
            get
            {
                try
                {
                    double s = 0;
                    foreach (double item in list)
                        s += Math.Pow(item, 2);
                    return (s - this.list.Length * Math.Pow(Mean, 2)) / (this.list.Length - 1);
                }
                catch (DivideByZeroException)
                {
                    return double.NaN;
                }
            }
        }

        public double Sigma()
        {
            return Math.Sqrt(this.Var);
        }

        public double Yule
        {
            get
            {
                try
                {
                    return ((this.Quarter3 - this.Median) - (this.Median - this.Quarter1)) / (this.Quarter3 - this.Quarter1);
                }
                catch (DivideByZeroException)
                {
                    return double.NaN;
                }
            }
        }

        public double Z(double member)
        {
            try
            {
                if (this.Exist(member))
                    return (member - this.Mean) / this.Sigma();
                else return double.NaN;
            }
            catch (DivideByZeroException)
            {
                return double.NaN;
            }
        }

        public double Covariance(Statistics s)
        {
            try
            {
                if (this.Length != s.Length) return double.NaN;
                int len = this.Length;
                double sum_mul = 0;
                for (int i = 0; i <= len - 1; i++)
                    sum_mul += (this.list[i] * s.list[i]);
                return (sum_mul - len * this.Mean * s.Mean) / (len - 1);
            }
            catch (DivideByZeroException)
            {
                return double.NaN;
            }
        }

        public double R(Statistics design)
        {
            try
            {
                return this.Covariance(design) / (this.Sigma() * design.Sigma());
            }
            catch (DivideByZeroException)
            {
                return double.NaN;
            }
        }

        public double A(Statistics design)
        {
            try
            {
                return this.Covariance(design) / (Math.Pow(design.Sigma(), 2));
            }
            catch (DivideByZeroException)
            {
                return double.NaN;
            }
        }

        public double B(Statistics design)
        {
            return this.Mean - this.A(design) * design.Mean;
        }

        private double InnerQuantile(double i)
        {
            try
            {
                double[] j = new double[this.list.Length];
                this.list.CopyTo(j, 0);
                Sort(j);
                if (Math.Ceiling(this.list.Length * i) == this.list.Length * i)
                    return (j[(int)(this.list.Length * i - 1)] + j[(int)(this.list.Length * i)]) / 2;
                else return j[((int)(Math.Ceiling(this.list.Length * i))) - 1];
            }
            catch (DivideByZeroException)
            {
                return double.NaN;
            }
        }

        private static void Sort(double[] i)
        {
            double[] temp = new double[i.Length];
            Statistics.MergeSort(i, temp, 0, i.Length - 1);
        }

        private static void MergeSort(double[] source, double[] temp, int left, int right)
        {
            int mid;
            if (left < right)
            {
                mid = (left + right) / 2;
                Statistics.MergeSort(source, temp, left, mid);
                Statistics.MergeSort(source, temp, mid + 1, right);
                Statistics.Merge(source, temp, left, mid + 1, right);
            }
        }

        private static void Merge(double[] source, double[] temp, int left, int mid, int right)
        {
            int i, leftEnd, numElements, tmpPos;
            leftEnd = mid - 1;
            tmpPos = left;
            numElements = right - left + 1;
            while ((left <= leftEnd) && (mid <= right))
            {
                if (source[left] <= source[mid])
                {
                    temp[tmpPos] = source[left];
                    tmpPos++;
                    left++;
                }
                else
                {
                    temp[tmpPos] = source[mid];
                    tmpPos++;
                    mid++;
                }
            }
            while (left <= leftEnd)
            {
                temp[tmpPos] = source[left];
                left++;
                tmpPos++;
            }
            while (mid <= right)
            {
                temp[tmpPos] = source[mid];
                mid++;
                tmpPos++;
            }
            for (i = 1; i <= numElements; i++)
            {
                source[right] = temp[right];
                right--;
            }
        }

        private bool Exist(double item)
        {
            bool isExist = false;
            int i = 0;
            while (i <= this.list.Length - 1 && !isExist)
            {
                isExist = (this.list[i] == item);
                i++;
            }
            return isExist;
        }

    }
}