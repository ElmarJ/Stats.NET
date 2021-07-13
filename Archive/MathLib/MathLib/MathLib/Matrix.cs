using System;
using System.Collections.Generic;
using System.Text;

namespace MathLib.Troep
{
    public class Matrix
    {
        #region Fields and Constants

        double[,] array;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="rowCount">The number of rows.</param>
        /// <param name="columnCount">The number of columns.</param>
        public Matrix(int rowCount, int columnCount)
        {
            array = new double[rowCount, columnCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    this.array[i, j] = 0;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="array">An array with the dimensions of the desired Matrix.</param>
        public Matrix(double[,] array)
        {
            this.array = (double[,])array.Clone();
        }

        #endregion

        #region Additional Static Constructor Methods

        /// <summary>
        /// Returns a Null-Matrix of the given dimensions
        /// </summary>
        /// <param name="rowCount">The number of rows.</param>
        /// <param name="columnCount">The number of columns.</param>
        /// <returns></returns>
        public static Matrix NullMatrix(int rowCount, int columnCount)
        {
            return new Matrix(rowCount, columnCount);
        }

        /// <summary>
        /// Returns an Identity-Matrix of the given dimensions.
        /// </summary>
        /// <param name="dimension">The number of rows and columns.</param>
        /// <returns></returns>
        public static Matrix IdentityMatrix(int dimension)
        {
            return ScalarMatrix(dimension, dimension, 1);
        }

        /// <summary>
        /// Returns an Unit-Matrix of the given dimensions.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        /// <returns></returns>
        public static Matrix UnitMatrix(int rows, int columns)
        {
            Matrix matrix = new Matrix(rows, columns);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    matrix[i, j] = 1;
            return matrix;
        }

        /// <summary>
        /// The function returns a Scalar Matrix of given dimensions
        /// and with a given scalar.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="cols">The number of columns.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        public static Matrix ScalarMatrix(int rowCount, int colCount, double scalar)
        {
            Matrix matrix = new Matrix(rowCount, colCount);
            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < colCount; j++)
                {
                    if (i == j)
                        matrix[i, j] = scalar;
                    else
                        matrix[i, j] = 0;
                }
            return matrix;
        }

        #endregion

        #region Operator, Conversion Overloads and Indexer

        public static Matrix operator -(Matrix matrix)
        { return matrix.Negative; }

        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        { return matrix1.Add(matrix2); }

        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        { return matrix1.Add(-matrix2); }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        { return matrix1.Multiply(matrix2); }

        public static Matrix operator *(Matrix matrix1, double frac)
        { return matrix1.Multiply(frac); }

        public static Matrix operator *(double frac, Matrix matrix1)
        { return matrix1.Multiply(frac); }

        public static Matrix operator *(Matrix matrix1, int number)
        { return matrix1.Multiply(number); }

        public static Matrix operator *(int number, Matrix matrix1)
        { return matrix1.Multiply(number); }

        public static Matrix operator /(Matrix matrix1, double frac)
        { return matrix1.Multiply(1 / frac); }
	
        public double this[int row, int column]
        {
            get
            {
                if (row < 0 || row > this.RowCount - 1 || column < 0 || column > this.ColumnCount - 1)
                    throw new IndexOutOfRangeException();
                return array[row, column];
            }

            set
            {
                if (row < 0 || row > this.RowCount - 1 || column < 0 || column > this.ColumnCount - 1)
                    throw new IndexOutOfRangeException();
                array[row, column] = value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Echelon form of the current matrix.
        /// </summary>
        public Matrix EchelonForm
        {
            get
            {
                return GetEchelonForm();
            }
        }

        /// <summary>
        /// The reduced echelon form of the current matrix.
        /// </summary>
        public Matrix ReducedEchelonForm
        {
            get { return this.GetReducedEchelonForm(); }
        }

        /// <summary>
        /// The inverse of the current matrix.
        /// </summary>
        public Matrix Inverse
        {
            get
            {
                if (this.Determinent == 0)
                    throw new InvalidOperationException("Inverse of a singular matrix is not possible");
                return (this.Adjoint / this.Determinent);
            }
        }

        /// <summary>
        /// The function returns the determinent of a Matrix object as double.
        /// </summary>
        public double Determinent
        {
            get
            {
                double det = 0;
                if (this.RowCount != this.ColumnCount)
                    throw new InvalidOperationException("Determinent of a non-square matrix doesn't exist");
                if (this.RowCount == 1)
                    return this[0, 0];
                for (int j = 0; j < this.ColumnCount; j++)
                    det += (this[0, j] * Minor(0, j).Determinent * (int)Math.Pow(-1, 0 + j));
                return det;
            }
        }

        /// <summary>
        /// The adjoint of the current matrix.
        /// </summary>
        public Matrix Adjoint
        {
            get
            {
                if (this.RowCount != this.ColumnCount)
                    throw new InvalidOperationException("Adjoint of a non-square matrix does not exists");
                Matrix adjointMatrix = new Matrix(this.RowCount, this.ColumnCount);
                for (int i = 0; i < this.RowCount; i++)
                    for (int j = 0; j < this.ColumnCount; j++)
                        adjointMatrix[i, j] = Math.Pow(-1, i + j) * this.Minor(i, j).Determinent;
                adjointMatrix = adjointMatrix.Transposed;
                return adjointMatrix;
            }
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>The number of columns.</value>
        public int ColumnCount
        {
            get { return this.array.GetLength(1); }
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        /// <value>The number of rows.</value>
        public int RowCount
        {
            get { return this.array.GetLength(0); }
        }

        /// <summary>
        /// Gets the transpose of the current matrix.
        /// </summary>
        public Matrix Transposed
        {
            get
            {
                Matrix TransposeMatrix = new Matrix(this.ColumnCount, this.RowCount);
                for (int i = 0; i < TransposeMatrix.RowCount; i++)
                    for (int j = 0; j < TransposeMatrix.ColumnCount; j++)
                        TransposeMatrix[i, j] = this[j, i];
                return TransposeMatrix;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterPriority>2</filterPriority>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < this.RowCount; i++)
            {
                // Add space if this row follows another row:
                if (i != 0) builder.Append(" ");

                // Open braces:
                builder.Append("{");

                // Add the values in this row:
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    if (j != 0) builder.Append(",");
                    builder.Append(this[i, j]);
                }

                // Close the braces:
                builder.Append("}");
            }
            return builder.ToString();
        }

        /// <summary>
        /// The function return the Minor of element[Row,Col] of a Matrix object 
        /// </summary>
        public Matrix Minor(int row, int column)
        {
            Matrix minor = new Matrix(this.RowCount - 1, this.ColumnCount - 1);
            int m = 0, n = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                if (i == row)
                    continue;
                n = 0;
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    if (j == column)
                        continue;
                    minor[m, n] = this[i, j];
                    n++;
                }
                m++;
            }
            return minor;
        }

        /// <summary>
        /// The function multiplies the given row of the current matrix object by a double 
        /// </summary>
        public void MultiplyRow(int row, double fraction)
        {
            for (int j = 0; j < this.ColumnCount; j++)
            {
                this[row, j] *= fraction;
            }
        }

        /// <summary>
        /// The function adds two rows for current matrix object
        /// It performs the following calculation:
        /// targetRow = targetRow + multiple*secondRow
        /// </summary>
        public void AddRow(int targetRow, int secondRow, double multiplier)
        {
            for (int j = 0; j < this.ColumnCount; j++)
                this[targetRow, j] += (this[secondRow, j] * multiplier);
        }

        /// <summary>
        /// The function interchanges two rows of the current matrix object
        /// </summary>
        public void InterchangeRow(int row1, int row2)
        {
            for (int j = 0; j < this.ColumnCount; j++)
            {
                double temp = this[row1, j];
                this[row1, j] = this[row2, j];
                this[row2, j] = temp;
            }
        }

        /// <summary>
        /// The function concatenates the current Matrix with a given matrix column-wise
        /// </summary>
        public Matrix Concatenate(Matrix matrix)
        {
            if (this.RowCount != matrix.RowCount)
                throw new ArgumentException("Concatenation not possible");
            Matrix result = new Matrix(this.RowCount, this.ColumnCount + matrix.ColumnCount);
            for (int i = 0; i < result.RowCount; i++)
                for (int j = 0; j < result.ColumnCount; j++)
                {
                    if (j < this.ColumnCount)
                        result[i, j] = this[i, j];
                    else
                        result[i, j] = matrix[i, j - this.ColumnCount];
                }
            return result;
        }

        #endregion

        #region Private Methods and Properties

        private Matrix GetEchelonForm()
        {
            try
            {
                Matrix echelonMatrix = new Matrix(this.array);
                for (int i = 0; i < this.RowCount; i++)
                {
                    if (echelonMatrix[i, i] == 0)	// if diagonal entry is zero, 
                        for (int j = i + 1; j < echelonMatrix.RowCount; j++)
                            if (echelonMatrix[j, i] != 0)	 //check if some below entry is non-zero
                                echelonMatrix.InterchangeRow(i, j);	// then interchange the two rows
                    if (echelonMatrix[i, i] == 0)	// if not found any non-zero diagonal entry
                        continue;	// increment i;
                    if (echelonMatrix[i, i] != 1)	// if diagonal entry is not 1 , 	
                        for (int j = i + 1; j < echelonMatrix.RowCount; j++)
                            if (echelonMatrix[j, i] == 1)	 //check if some below entry is 1
                                echelonMatrix.InterchangeRow(i, j);	// then interchange the two rows
                    echelonMatrix.MultiplyRow(i, 1 / echelonMatrix[i, i]);
                    for (int j = i + 1; j < echelonMatrix.RowCount; j++)
                        echelonMatrix.AddRow(j, i, -echelonMatrix[j, i]);
                }
                return echelonMatrix;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Matrix GetReducedEchelonForm()
        {
            try
            {
                Matrix reducedEchelonMatrix = new Matrix(this.array);
                for (int i = 0; i < this.RowCount; i++)
                {
                    if (reducedEchelonMatrix[i, i] == 0)	// if diagonal entry is zero, 
                        for (int j = i + 1; j < reducedEchelonMatrix.RowCount; j++)
                            if (reducedEchelonMatrix[j, i] != 0)	 //check if some below entry is non-zero
                                reducedEchelonMatrix.InterchangeRow(i, j);	// then interchange the two rows
                    if (reducedEchelonMatrix[i, i] == 0)	// if not found any non-zero diagonal entry
                        continue;	// increment i;
                    if (reducedEchelonMatrix[i, i] != 1)	// if diagonal entry is not 1 , 	
                        for (int j = i + 1; j < reducedEchelonMatrix.RowCount; j++)
                            if (reducedEchelonMatrix[j, i] == 1)	 //check if some below entry is 1
                                reducedEchelonMatrix.InterchangeRow(i, j);	// then interchange the two rows
                    reducedEchelonMatrix.MultiplyRow(i, 1 / reducedEchelonMatrix[i, i]);
                    for (int j = i + 1; j < reducedEchelonMatrix.RowCount; j++)
                        reducedEchelonMatrix.AddRow(j, i, -reducedEchelonMatrix[j, i]);
                    for (int j = i - 1; j >= 0; j--)
                        reducedEchelonMatrix.AddRow(j, i, -reducedEchelonMatrix[j, i]);
                }
                return reducedEchelonMatrix;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Matrix Negative
        {
            get { return this.Multiply(-1); }
        }

        private Matrix Add(Matrix matrix)
        {
            if (this.RowCount != matrix.RowCount || this.ColumnCount != matrix.ColumnCount)
                throw new ArgumentException();

            Matrix result = new Matrix(this.RowCount, this.ColumnCount);
            for (int i = 0; i < result.RowCount; i++)
                for (int j = 0; j < result.ColumnCount; j++)
                    result[i, j] = this[i, j] + matrix[i, j];
            return result;
        }

        private Matrix Multiply(Matrix matrix)
        {
            if (this.ColumnCount != matrix.RowCount)
                throw new ArgumentException();

            Matrix result = Matrix.NullMatrix(this.RowCount, matrix.ColumnCount);
            for (int i = 0; i < result.RowCount; i++)
                for (int j = 0; j < result.ColumnCount; j++)
                    for (int k = 0; k < this.ColumnCount; k++)
                        result[i, j] += this[i, k] * matrix[k, j];
            return result;
        }

        private Matrix Multiply(int scalar)
        {
            Matrix result = new Matrix(this.RowCount, this.ColumnCount);
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColumnCount; j++)
                    result[i, j] = this[i, j] * scalar;
            return result;
        }

        private Matrix Multiply(double fraction)
        {
            Matrix result = new Matrix(this.RowCount, this.ColumnCount);
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColumnCount; j++)
                    result[i, j] = this[i, j] * fraction;
            return result;
        }

        #endregion

    }
}
