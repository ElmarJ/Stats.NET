using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using SpssLib.FileParser;

namespace SpssLib.DataReader
{
    public class SpssDataReader: IDataReader
    {
        FileParser.SavFileParser parser;
        byte[][] currentRecord;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ///
        /// <param name="parser">   The parser. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpssDataReader(FileParser.SavFileParser parser)
        {
            this.parser = parser;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ///
        /// <param name="spssFileStream">   The spss file stream. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpssDataReader(Stream spssFileStream)
        {
            this.parser = new FileParser.SavFileParser(spssFileStream);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the information describing the file meta. </summary>
        ///
        /// <value> Information describing the file meta. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public MetaData FileMetaData
        {
            get
            {
                if (!parser.MetaDataParsed)
                    parser.ParseMetaData();
                return this.parser.MetaData;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Closes this object. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Close()
        {
            this.IsClosed = true;
            this.parser.Dispose();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the depth. </summary>
        ///
        /// <value> The depth. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int Depth
        {
            get { return 0; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the schema table. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ///
        /// <returns>   The schema table. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DataTable GetSchemaTable()
        {
            // throw new NotImplementedException();
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether this object is closed. </summary>
        ///
        /// <value> true if this object is closed, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsClosed
        {
            get;
            private set;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Next result. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool NextResult()
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Reads this object. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Read()
        {
            this.currentRecord = parser.ReadNextDataRecord();
            return (currentRecord != null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the number of records affected. </summary>
        ///
        /// <value> The number of records affected. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int RecordsAffected
        {
            get { return 0; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dispose of this object, cleaning up any resources it uses. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dispose of this object, cleaning up any resources it uses. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ///
        /// <param name="disposing">    true if resources should be disposed, false if not. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.parser != null)
                {
                    this.parser.Dispose();
                    this.parser = null;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the number of fields. </summary>
        ///
        /// <value> The number of fields. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public int FieldCount
        {
            get { return parser.Variables.Count; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a boolean. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ///
        /// <param name="i">    The index. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool GetBoolean(int i)
        {
            throw new NotSupportedException();
        }

        public byte GetByte(int i)
        {
            throw new NotSupportedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public char GetChar(int i)
        {
            throw new NotSupportedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public string GetDataTypeName(int i)
        {
            return this.parser.Variables[i].Type.ToString();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotSupportedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotSupportedException();
        }

        public double GetDouble(int i)
        {
            var value = BitConverter.ToDouble(currentRecord[i], 0);
            if (value == this.FileMetaData.InfoRecords.MachineFloatingPointInfoRecord.SystemMissingValue)
                throw new InvalidOperationException("Value is sysmis");
            return value;
        }

        public Type GetFieldType(int i)
        {
            if (this.parser.Variables[i].Type == SpssDataset.DataType.Numeric)
            {
                return typeof(double);
            }
            else
            {
                return typeof(string);
            }
        }

        public float GetFloat(int i)
        {
            throw new NotSupportedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        public short GetInt16(int i)
        {
            throw new NotSupportedException();
        }

        public int GetInt32(int i)
        {
            throw new NotSupportedException();
        }

        public long GetInt64(int i)
        {
            throw new NotSupportedException();
        }

        public string GetName(int i)
        {
            return this.parser.Variables[i].Name;
        }

        public int GetOrdinal(string name)
        {
            var variable =
                (from v in this.parser.Variables
                 where v.Name == name
                 select v)
                .FirstOrDefault();

            if (variable == null) throw new ArgumentException("Fieldname unknown", "name");

            return variable.Index;
        }

        public string GetString(int i)
        {
            throw new NotSupportedException();
        }

        public object GetValue(int i)
        {
            if(GetFieldType(i) == typeof(double))
            {
                var value =  this.GetDouble(i);
                if (value == this.FileMetaData.InfoRecords.MachineFloatingPointInfoRecord.SystemMissingValue)
                    return DBNull.Value;
                else
                    return value;
            }
            else
            {
                return this.GetString(i);
            }
        }

        public int GetValues(object[] values)
        {
            var record = parser.RecordToObjects(this.currentRecord).ToArray();
            record.CopyTo(values, 0);
            return record.Length;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Query if 'i' is database null. </summary>
        ///
        /// <remarks>   Elmar, 5-6-2010. </remarks>
        ///
        /// <param name="i">    The index. </param>
        ///
        /// <returns>   true if database null, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsDBNull(int i)
        {
            return (GetValue(i) is DBNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <value> The indexed item. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public object this[string name]
        {
            get
            {
                return GetValue(GetOrdinal(name));
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <value> The indexed item. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public object this[int i]
        {
            get { return GetValue(i); }
        }
    }
}
