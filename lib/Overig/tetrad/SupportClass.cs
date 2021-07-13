//
// In order to convert some functionality to Visual C#, the Java Language Conversion Assistant
// creates "support classes" that duplicate the original functionality.  
//
// Support classes replicate the functionality of the original code, but in some cases they are 
// substantially different architecturally. Although every effort is made to preserve the 
// original architecture of the application in the converted project, the user should be aware that 
// the primary goal of these support classes is to replicate functionality, and that at times 
// the architecture of the resulting solution may differ somewhat.
//

using System;

	/// <summary>
	/// This interface should be implemented by any class whose instances are intended 
	/// to be executed by a thread.
	/// </summary>
	public interface IThreadRunnable
	{
		/// <summary>
		/// This method has to be implemented in order that starting of the thread causes the object's 
		/// run method to be called in that separately executing thread.
		/// </summary>
		void Run();
	}

	/*******************************/
	/// <summary>
	/// This class is used to encapsulate a source of Xml code in an single class.
	/// </summary>
	public class XmlSourceSupport
	{
		private System.IO.Stream bytes;
		private System.IO.StreamReader characters;
		private System.String uri;

		/// <summary>
		/// Constructs an empty XmlSourceSupport instance.
		/// </summary>
		public XmlSourceSupport()
		{
			bytes = null;
			characters = null;
			uri = null;
		}

		/// <summary>
		/// Constructs a XmlSource instance with the specified source System.IO.Stream.
		/// </summary>
		/// <param name="stream">The stream containing the document.</param>
		public XmlSourceSupport(System.IO.Stream stream)
		{
			bytes = stream;
			characters = null;
			uri = null;
		}

		/// <summary>
		/// Constructs a XmlSource instance with the specified source System.IO.StreamReader.
		/// </summary>
		/// <param name="reader">The reader containing the document.</param>
		public XmlSourceSupport(System.IO.StreamReader reader)
		{
			bytes = null;
			characters = reader;
			uri = null;
		}

		/// <summary>
		/// Construct a XmlSource instance with the specified source Uri string.
		/// </summary>
		/// <param name="source">The source containing the document.</param>
		public XmlSourceSupport(System.String source)
		{
			bytes = null;
			characters = null;
			uri = source;
		}

		/// <summary>
		/// Represents the source Stream of the XmlSource.
		/// </summary>
		public System.IO.Stream Bytes	
		{
			get
			{
				return bytes;
			}
			set
			{
				bytes = value;
			}
		}

		/// <summary>
		/// Represents the source StreamReader of the XmlSource.
		/// </summary>
		public System.IO.StreamReader Characters
		{
			get
			{
				return characters;
			}
			set
			{
				characters = value;
			}
		}

		/// <summary>
		/// Represents the source URI of the XmlSource.
		/// </summary>
		public System.String Uri
		{
			get
			{
				return uri;
			}
			set
			{
				uri = value;
			}
		}
	}

/// <summary>
/// Contains conversion support elements such as classes, interfaces and static methods.
/// </summary>
public class SupportClass
{
	/// <summary>
	/// Writes the exception stack trace to the received stream
	/// </summary>
	/// <param name="throwable">Exception to obtain information from</param>
	/// <param name="stream">Output sream used to write to</param>
	public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream)
	{
		stream.Write(throwable.StackTrace);
		stream.Flush();
	}

	/*******************************/
	/// <summary>
	/// Reads the serialized fields written by the DefaultWriteObject method.
	/// </summary>
	/// <param name="info">SerializationInfo parameter from the special deserialization constructor.</param>
	/// <param name="context">StreamingContext parameter from the special deserialization constructor</param>
	/// <param name="instance">Object to deserialize.</param>
	public static void DefaultReadObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Object instance)
	{                       
		System.Type thisType = instance.GetType();
		System.Reflection.MemberInfo[] mi = System.Runtime.Serialization.FormatterServices.GetSerializableMembers(thisType, context);
		for (int i = 0 ; i < mi.Length; i++) 
		{
			System.Reflection.FieldInfo fi = (System.Reflection.FieldInfo) mi[i];
			fi.SetValue(instance, info.GetValue(fi.Name, fi.FieldType));
		}
	}
	/*******************************/
	/// <summary>
	/// Writes the serializable fields to the SerializationInfo object, which stores all the data needed to serialize the specified object object.
	/// </summary>
	/// <param name="info">SerializationInfo parameter from the GetObjectData method.</param>
	/// <param name="context">StreamingContext parameter from the GetObjectData method.</param>
	/// <param name="instance">Object to serialize.</param>
	public static void DefaultWriteObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Object instance)
	{                       
		System.Type thisType = instance.GetType();
		System.Reflection.MemberInfo[] mi = System.Runtime.Serialization.FormatterServices.GetSerializableMembers(thisType, context);
		for (int i = 0 ; i < mi.Length; i++) 
		{
			info.AddValue(mi[i].Name, ((System.Reflection.FieldInfo) mi[i]).GetValue(instance));
		}
	}


	/*******************************/
	/// <summary>
	/// The class performs token processing in strings
	/// </summary>
	public class Tokenizer: System.Collections.IEnumerator
	{
		/// Position over the string
		private long currentPos = 0;

		/// Include demiliters in the results.
		private bool includeDelims = false;

		/// Char representation of the String to tokenize.
		private char[] chars = null;
			
		//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character and the form-feed character
		private string delimiters = " \t\n\r\f";		

		/// <summary>
		/// Initializes a new class instance with a specified string to process
		/// </summary>
		/// <param name="source">String to tokenize</param>
		public Tokenizer(System.String source)
		{			
			this.chars = source.ToCharArray();
		}

		/// <summary>
		/// Initializes a new class instance with a specified string to process
		/// and the specified token delimiters to use
		/// </summary>
		/// <param name="source">String to tokenize</param>
		/// <param name="delimiters">String containing the delimiters</param>
		public Tokenizer(System.String source, System.String delimiters):this(source)
		{			
			this.delimiters = delimiters;
		}


		/// <summary>
		/// Initializes a new class instance with a specified string to process, the specified token 
		/// delimiters to use, and whether the delimiters must be included in the results.
		/// </summary>
		/// <param name="source">String to tokenize</param>
		/// <param name="delimiters">String containing the delimiters</param>
		/// <param name="includeDelims">Determines if delimiters are included in the results.</param>
		public Tokenizer(System.String source, System.String delimiters, bool includeDelims):this(source,delimiters)
		{
			this.includeDelims = includeDelims;
		}	


		/// <summary>
		/// Returns the next token from the token list
		/// </summary>
		/// <returns>The string value of the token</returns>
		public System.String NextToken()
		{				
			return NextToken(this.delimiters);
		}

		/// <summary>
		/// Returns the next token from the source string, using the provided
		/// token delimiters
		/// </summary>
		/// <param name="delimiters">String containing the delimiters to use</param>
		/// <returns>The string value of the token</returns>
		public System.String NextToken(System.String delimiters)
		{
			//According to documentation, the usage of the received delimiters should be temporary (only for this call).
			//However, it seems it is not true, so the following line is necessary.
			this.delimiters = delimiters;

			//at the end 
			if (this.currentPos == this.chars.Length)
				throw new System.ArgumentOutOfRangeException();
			//if over a delimiter and delimiters must be returned
			else if (   (System.Array.IndexOf(delimiters.ToCharArray(),chars[this.currentPos]) != -1)
				     && this.includeDelims )                	
				return "" + this.chars[this.currentPos++];
			//need to get the token wo delimiters.
			else
				return nextToken(delimiters.ToCharArray());
		}

		//Returns the nextToken wo delimiters
		private System.String nextToken(char[] delimiters)
		{
			string token="";
			long pos = this.currentPos;

			//skip possible delimiters
			while (System.Array.IndexOf(delimiters,this.chars[currentPos]) != -1)
				//The last one is a delimiter (i.e there is no more tokens)
				if (++this.currentPos == this.chars.Length)
				{
					this.currentPos = pos;
					throw new System.ArgumentOutOfRangeException();
				}
			
			//getting the token
			while (System.Array.IndexOf(delimiters,this.chars[this.currentPos]) == -1)
			{
				token+=this.chars[this.currentPos];
				//the last one is not a delimiter
				if (++this.currentPos == this.chars.Length)
					break;
			}
			return token;
		}

				
		/// <summary>
		/// Determines if there are more tokens to return from the source string
		/// </summary>
		/// <returns>True or false, depending if there are more tokens</returns>
		public bool HasMoreTokens()
		{
			//keeping the current pos
			long pos = this.currentPos;
			
			try
			{
				this.NextToken();
			}
			catch (System.ArgumentOutOfRangeException)
			{				
				return false;
			}
			finally
			{
				this.currentPos = pos;
			}
			return true;
		}

		/// <summary>
		/// Remaining tokens count
		/// </summary>
		public int Count
		{
			get
			{
				//keeping the current pos
				long pos = this.currentPos;
				int i = 0;
			
				try
				{
					while (true)
					{
						this.NextToken();
						i++;
					}
				}
				catch (System.ArgumentOutOfRangeException)
				{				
					this.currentPos = pos;
					return i;
				}
			}
		}

		/// <summary>
		///  Performs the same action as NextToken.
		/// </summary>
		public System.Object Current
		{
			get
			{
				return (Object) this.NextToken();
			}		
		}		
		
		/// <summary>
		//  Performs the same action as HasMoreTokens.
		/// </summary>
		/// <returns>True or false, depending if there are more tokens</returns>
		public bool MoveNext()
		{
			return this.HasMoreTokens();
		}
		
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void  Reset()
		{
			;
		}			
	}
	/*******************************/
	/// <summary>
	/// This class manages array operations.
	/// </summary>
	public class ArraySupport
	{
		/// <summary>
		/// Compares the entire members of one array whith the other one.
		/// </summary>
		/// <param name="array1">The array to be compared.</param>
		/// <param name="array2">The array to be compared with.</param>
		/// <returns>True if both arrays are equals otherwise it returns false.</returns>
		/// <remarks>Two arrays are equal if they contains the same elements in the same order.</remarks>
		public static bool Equals(System.Array array1, System.Array array2)
		{
			bool result = false;
			if ((array1 == null) && (array2 == null))
				result = true;
			else if ((array1 != null) && (array2 != null))
			{
				if (array1.Length == array2.Length)
				{
					int length = array1.Length;
					result = true;
					for (int index = 0; index < length; index++)
					{
						if (!(array1.GetValue(index).Equals(array2.GetValue(index))))
						{
							result = false;
							break;
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Fills the array with an specific value from an specific index to an specific index.
		/// </summary>
		/// <param name="array">The array to be filled.</param>
		/// <param name="fromindex">The first index to be filled.</param>
		/// <param name="toindex">The last index to be filled.</param>
		/// <param name="val">The value to fill the array with.</param>
		public static void Fill(System.Array array, System.Int32 fromindex, System.Int32 toindex, System.Object val)
		{
			System.Object Temp_Object = val;
			System.Type elementtype = array.GetType().GetElementType();
			if (elementtype != val.GetType())
				Temp_Object = System.Convert.ChangeType(val, elementtype);
			if (array.Length == 0)
				throw (new System.NullReferenceException());
			if (fromindex > toindex)
				throw (new System.ArgumentException());
			if ((fromindex < 0) || ((System.Array)array).Length < toindex)
				throw (new System.IndexOutOfRangeException());
			for (int index = (fromindex > 0) ? fromindex-- : fromindex; index < toindex; index++)
				array.SetValue(Temp_Object, index);
		}

		/// <summary>
		/// Fills the array with an specific value.
		/// </summary>
		/// <param name="array">The array to be filled.</param>
		/// <param name="val">The value to fill the array with.</param>
		public static void Fill(System.Array array, System.Object val)
		{
			Fill(array, 0, array.Length, val);
		}
	}


	/*******************************/
	/// <summary>
	/// Implements number format functions
	/// </summary>
	[Serializable]
	public class TextNumberFormat
	{

		//Current localization number format infomation
		private System.Globalization.NumberFormatInfo numberFormat;
		//Enumeration of format types that can be used
		private enum formatTypes { General, Number, Currency, Percent };
		//Current format type used in the instance
		private int numberFormatType;
		//Indicates if grouping is being used
		private bool groupingActivated;
		//Current separator used
		private System.String separator;
		//Number of maximun digits in the integer portion of the number to represent the number
		private int maxIntDigits;
		//Number of minimum digits in the integer portion of the number to represent the number
		private int minIntDigits;
		//Number of maximun digits in the fraction portion of the number to represent the number
		private int maxFractionDigits;
		//Number of minimum digits in the integer portion of the number to represent the number
		private int minFractionDigits;

		/// <summary>
		/// Initializes a new instance of the object class with the default values
		/// </summary>
		public TextNumberFormat()
		{
			this.numberFormat      = new System.Globalization.NumberFormatInfo();
			this.numberFormatType  = (int)TextNumberFormat.formatTypes.General;
			this.groupingActivated = true;
			this.separator = this.GetSeparator( (int)TextNumberFormat.formatTypes.General );
			this.maxIntDigits = 127;
			this.minIntDigits = 1;
			this.maxFractionDigits = 3;
			this.minFractionDigits = 0;
		}

		/// <summary>
		/// Sets the Maximum integer digits value. 
		/// </summary>
		/// <param name="newValue">the new value for the maxIntDigits field</param>
		public void setMaximumIntegerDigits(int newValue)
		{
			maxIntDigits = newValue;
			if (newValue <= 0)
			{
				maxIntDigits = 0;
				minIntDigits = 0;
			}
			else if (maxIntDigits < minIntDigits)
			{
				minIntDigits = maxIntDigits;
			}
		}

		/// <summary>
		/// Sets the minimum integer digits value. 
		/// </summary>
		/// <param name="newValue">the new value for the minIntDigits field</param>
		public void setMinimumIntegerDigits(int newValue)
		{
			minIntDigits = newValue;
			if (newValue <= 0)
			{
				minIntDigits = 0;
			}
			else if (maxIntDigits < minIntDigits)
			{
				maxIntDigits = minIntDigits;
			}
		}

		/// <summary>
		/// Sets the maximum fraction digits value. 
		/// </summary>
		/// <param name="newValue">the new value for the maxFractionDigits field</param>
		public void setMaximumFractionDigits(int newValue)
		{
			maxFractionDigits = newValue;
			if (newValue <= 0)
			{
				maxFractionDigits = 0;
				minFractionDigits = 0;
			}
			else if (maxFractionDigits < minFractionDigits)
			{
				minFractionDigits = maxFractionDigits;
			}
		}
		
		/// <summary>
		/// Sets the minimum fraction digits value. 
		/// </summary>
		/// <param name="newValue">the new value for the minFractionDigits field</param>
		public void setMinimumFractionDigits(int newValue)
		{
			minFractionDigits = newValue;
			if (newValue <= 0)
			{
				minFractionDigits = 0;
			}
			else if (maxFractionDigits < minFractionDigits)
			{
				maxFractionDigits = minFractionDigits;
			}
		}

		/// <summary>
		/// Initializes a new instance of the class with the specified number format
		/// and the amount of fractional digits to use
		/// </summary>
		/// <param name="theType">Number format</param>
		/// <param name="digits">Number of fractional digits to use</param>
		private TextNumberFormat(TextNumberFormat.formatTypes theType, int digits)
		{
			this.numberFormat      = System.Globalization.NumberFormatInfo.CurrentInfo;
			this.numberFormatType  = (int)theType;
			this.groupingActivated = true;
			this.separator = this.GetSeparator( (int)theType );
			this.maxIntDigits = 127;
			this.minIntDigits = 1;
			this.maxFractionDigits = 3;
			this.minFractionDigits = 0;
		}

		/// <summary>
		/// Initializes a new instance of the class with the specified number format,
		/// uses the system's culture information,
		/// and assigns the amount of fractional digits to use
		/// </summary>
		/// <param name="theType">Number format</param>
		/// <param name="cultureNumberFormat">Represents information about a specific culture including the number formatting</param>
		/// <param name="digits">Number of fractional digits to use</param>
		private TextNumberFormat(TextNumberFormat.formatTypes theType, System.Globalization.CultureInfo cultureNumberFormat, int digits)
		{
			this.numberFormat      = cultureNumberFormat.NumberFormat;
			this.numberFormatType  = (int)theType;
			this.groupingActivated = true;
			this.separator = this.GetSeparator( (int)theType );
			this.maxIntDigits = 127;
			this.minIntDigits = 1;
			this.maxFractionDigits = 3;
			this.minFractionDigits = 0;
		}

		/// <summary>
		/// Returns an initialized instance of the TextNumberFormat object
		/// using number representation.
		/// </summary>
		/// <returns>The object instance</returns>
		public static TextNumberFormat getTextNumberInstance()
		{
			TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Number, 3);
			return instance;
		}

		/// <summary>
		/// Returns an initialized instance of the TextNumberFormat object
		/// using currency representation.
		/// </summary>
		/// <returns>The object instance</returns>
		public static TextNumberFormat getTextNumberCurrencyInstance()
		{
			TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Currency, 3);
			return instance.setToCurrencyNumberFormatDefaults(instance);
		}

		/// <summary>
		/// Returns an initialized instance of the TextNumberFormat object
		/// using percent representation.
		/// </summary>
		/// <returns>The object instance</returns>
		public static TextNumberFormat getTextNumberPercentInstance()
		{
			TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Percent, 3);
			return instance.setToPercentNumberFormatDefaults(instance);
		}

		/// <summary>
		/// Returns an initialized instance of the TextNumberFormat object
		/// using number representation, it uses the culture format information provided.
		/// </summary>
		/// <param name="culture">Represents information about a specific culture</param>
		/// <returns>The object instance</returns>
		public static TextNumberFormat getTextNumberInstance(System.Globalization.CultureInfo culture)
		{
			TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Number, culture, 3);
			return instance;
		}

		/// <summary>
		/// Returns an initialized instance of the TextNumberFormat object
		/// using currency representation, it uses the culture format information provided.
		/// </summary>
		/// <param name="culture">Represents information about a specific culture</param>
		/// <returns>The object instance</returns>
		public static TextNumberFormat getTextNumberCurrencyInstance(System.Globalization.CultureInfo culture)
		{
			TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Currency, culture, 3);
			return instance.setToCurrencyNumberFormatDefaults(instance);
		}

		/// <summary>
		/// Returns an initialized instance of the TextNumberFormat object
		/// using percent representation, it uses the culture format information provided.
		/// </summary>
		/// <param name="culture">Represents information about a specific culture</param>
		/// <returns>The object instance</returns>
		public static TextNumberFormat getTextNumberPercentInstance(System.Globalization.CultureInfo culture)
		{
			TextNumberFormat instance = new TextNumberFormat(TextNumberFormat.formatTypes.Percent, culture, 3);
            return instance.setToPercentNumberFormatDefaults(instance);
		}

		/// <summary>
		/// Clones the object instance
		/// </summary>
		/// <returns>The cloned object instance</returns>
		public System.Object Clone()
		{
			return (System.Object)this;
		}

		/// <summary>
		/// Determines if the received object is equal to the
		/// current object instance
		/// </summary>
		/// <param name="textNumberObject">TextNumber instance to compare</param>
		/// <returns>True or false depending if the two instances are equal</returns>
		public override bool Equals(Object obj) 
		{
			// Check for null values and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) 
				return false;
			SupportClass.TextNumberFormat param = (SupportClass.TextNumberFormat)obj;
			return (numberFormat == param.numberFormat) && (numberFormatType == param.numberFormatType) 
				&& (groupingActivated == param.groupingActivated) && (separator == param.separator) 
				&& (maxIntDigits == param.maxIntDigits)	&& (minIntDigits == param.minIntDigits) 
				&& (maxFractionDigits == param.maxFractionDigits) && (minFractionDigits == param.minFractionDigits);
		}

		
		/// <summary>
		/// Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current Object</returns>
		public override int GetHashCode()
		{
			return numberFormat.GetHashCode() ^ numberFormatType ^ groupingActivated.GetHashCode() 
				 ^ separator.GetHashCode() ^ maxIntDigits ^ minIntDigits ^ maxFractionDigits ^ minFractionDigits;
		}

		/// <summary>
		/// Formats a number with the current formatting parameters
		/// </summary>
		/// <param name="number">Source number to format</param>
		/// <returns>The formatted number string</returns>
		public System.String FormatDouble(double number)
		{
			if (this.groupingActivated)
			{
				return SetIntDigits(number.ToString(this.GetCurrentFormatString() + this.GetNumberOfDigits( number ), this.numberFormat));
			}
			else
			{
				return SetIntDigits((number.ToString(this.GetCurrentFormatString() + this.GetNumberOfDigits( number ) , this.numberFormat)).Replace(this.separator,""));
			}
		}
		
		/// <summary>
		/// Formats a number with the current formatting parameters
		/// </summary>
		/// <param name="number">Source number to format</param>
		/// <returns>The formatted number string</returns>
		public System.String FormatLong(long number)
		{			
			if (this.groupingActivated)
			{
				return SetIntDigits(number.ToString(this.GetCurrentFormatString() + this.minFractionDigits , this.numberFormat));
			}
			else
			{
				return SetIntDigits((number.ToString(this.GetCurrentFormatString() + this.minFractionDigits , this.numberFormat)).Replace(this.separator,""));
			}
		}
		
		
		/// <summary>
		/// Formats the number according to the specified number of integer digits 
		/// </summary>
		/// <param name="number">The number to format</param>
		/// <returns></returns>
		private System.String SetIntDigits(String number)
		{			
			String decimals = "";
			String fraction = "";
			int i = number.IndexOf(this.numberFormat.NumberDecimalSeparator);
			if (i > 0)
			{
				fraction = number.Substring(i);
				decimals = number.Substring(0,i).Replace(this.numberFormat.NumberGroupSeparator,"");
			}
			else decimals = number.Replace(this.numberFormat.NumberGroupSeparator,"");
			decimals = decimals.PadLeft(this.MinIntDigits,'0');
			if ((i = decimals.Length - this.MaxIntDigits) > 0) decimals = decimals.Remove(0,i);
			if (this.groupingActivated) 
			{
				for (i = decimals.Length;i > 3;i -= 3)
				{
					decimals = decimals.Insert(i - 3,this.numberFormat.NumberGroupSeparator);
				}
			}
			decimals = decimals + fraction;
			if (decimals.Length == 0) return "0";
			else return decimals;
		}

		/// <summary>
		/// Gets the list of all supported cultures
		/// </summary>
		/// <returns>An array of type CultureInfo that represents the supported cultures</returns>
		public static System.Globalization.CultureInfo[] GetAvailableCultures()
		{
			return System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures);
		}

		/// <summary>
		/// Obtains the current format representation used
		/// </summary>
		/// <returns>A character representing the string format used</returns>
		private System.String GetCurrentFormatString()
		{
			System.String currentFormatString = "n";  //Default value
			switch (this.numberFormatType)
			{
				case (int)TextNumberFormat.formatTypes.Currency:
					currentFormatString = "c";
					break;

				case (int)TextNumberFormat.formatTypes.General:
					currentFormatString = "n";
					break;

				case (int)TextNumberFormat.formatTypes.Number:
					currentFormatString = "n";
					break;

				case (int)TextNumberFormat.formatTypes.Percent:
					currentFormatString = "p";
					break;
			}
			return currentFormatString;
		}

		/// <summary>
		/// Retrieves the separator used, depending on the format type specified
		/// </summary>
		/// <param name="numberFormatType">formatType enumarator value to inquire</param>
		/// <returns>The values of character separator used </returns>
		private System.String GetSeparator(int numberFormatType)
		{
			System.String separatorItem = " ";  //Default Separator

			switch (numberFormatType)
			{
				case (int)TextNumberFormat.formatTypes.Currency:
					separatorItem = this.numberFormat.CurrencyGroupSeparator;
					break;

				case (int)TextNumberFormat.formatTypes.General:
					separatorItem = this.numberFormat.NumberGroupSeparator;
					break;

				case (int)TextNumberFormat.formatTypes.Number:
					separatorItem = this.numberFormat.NumberGroupSeparator;
					break;

				case (int)TextNumberFormat.formatTypes.Percent:
					separatorItem = this.numberFormat.PercentGroupSeparator;
					break;
			}
			return separatorItem;
		}

		/// <summary>
		/// Boolean value stating if grouping is used or not
		/// </summary>
		public bool GroupingUsed
		{
			get
			{
				return (this.groupingActivated);
			}
			set
			{
				this.groupingActivated = value;
			}
		}

		/// <summary>
		/// Minimum number of integer digits to use in the number format
		/// </summary>
		public int MinIntDigits
		{
			get
			{
				return this.minIntDigits;
			}
			set
			{
				this.minIntDigits = value;
			}
		}

		/// <summary>
		/// Maximum number of integer digits to use in the number format
		/// </summary>
		public int MaxIntDigits
		{
			get
			{
				return this.maxIntDigits;
			}
			set
			{
				this.maxIntDigits = value;
			}
		}

		/// <summary>
		/// Minimum number of fraction digits to use in the number format
		/// </summary>
		public int MinFractionDigits
		{
			get
			{
				return this.minFractionDigits;
			}
			set
			{
				this.minFractionDigits = value;
			}
		}

		/// <summary>
		/// Maximum number of fraction digits to use in the number format
		/// </summary>
		public int MaxFractionDigits
		{
			get
			{
				return this.maxFractionDigits;
			}
			set
			{
				this.maxFractionDigits = value;
			}
		}

		/// <summary>
		/// Sets the values of minFractionDigits and maxFractionDigits to the currency standard
		/// </summary>
		/// <param name="format">The TextNumberFormat instance to set</param>
		/// <returns>The TextNumberFormat with corresponding the default values</returns>
		private TextNumberFormat setToCurrencyNumberFormatDefaults( TextNumberFormat format )
		{
			format.maxFractionDigits = 2;
			format.minFractionDigits = 2;
			return format;
		}

		/// <summary>
		/// Sets the values of minFractionDigits and maxFractionDigits to the percent standard
		/// </summary>
		/// <param name="format">The TextNumberFormat instance to set</param>
		/// <returns>The TextNumberFormat with corresponding the default values</returns>
		private TextNumberFormat setToPercentNumberFormatDefaults( TextNumberFormat format )
		{
			format.maxFractionDigits = 0;
			format.minFractionDigits = 0;
			return format;
		}

		/// <summary>
		/// Gets the number of fraction digits thats must be used by the format methods
		/// </summary>
		/// <param name="number">The double number</param>
		/// <returns>The number of fraction digits to use</returns>
		private int GetNumberOfDigits( Double number )
		{
			int counter = 0;
			double temp = System.Math.Abs(number);
			while ( (temp % 1) > 0 )
			{
				temp *= 10;
				counter++;
			}
			return (counter < this.minFractionDigits) ? this.minFractionDigits : (( counter < this.maxFractionDigits ) ? counter : this.maxFractionDigits); 
		}
	}
	/*******************************/
	public delegate void PropertyChangeEventHandler(System.Object sender, PropertyChangingEventArgs e);

	/// <summary>
	/// EventArgs for support to the contrained properties.
	/// </summary>
	public class PropertyChangingEventArgs : System.ComponentModel.PropertyChangedEventArgs
	{   
		private System.Object oldValue;
		private System.Object newValue;

		/// <summary>
		/// Initializes a new PropertyChangingEventArgs instance.
		/// </summary>
		/// <param name="propertyName">Property name that fire the event.</param>
		public PropertyChangingEventArgs(System.String propertyName) : base(propertyName)
		{
		}

		/// <summary>
		/// Initializes a new PropertyChangingEventArgs instance.
		/// </summary>
		/// <param name="propertyName">Property name that fire the event.</param>
		/// <param name="oldVal">Property value to be replaced.</param>
		/// <param name="newVal">Property value to be set.</param>
		public PropertyChangingEventArgs(System.String propertyName, System.Object oldVal, System.Object newVal) : base(propertyName)
		{
			this.oldValue = oldVal;
			this.newValue = newVal;
		}

		/// <summary>
		/// Gets or sets the old value of the event.
		/// </summary>
		public System.Object OldValue
		{
			get
			{
				return this.oldValue;
			}
			set
			{
				this.oldValue = value;
			}
		}
	        
		/// <summary>
		/// Gets or sets the new value of the event.
		/// </summary>
		public System.Object NewValue
		{
			get
			{
				return this.newValue;
			}
			set
			{
				this.newValue = value;
			}
		}
	}


	/*******************************/
	/// <summary>
	/// This class provides functionality not found in .NET collection-related interfaces.
	/// </summary>
	public class ICollectionSupport
	{
		/// <summary>
		/// Adds a new element to the specified collection.
		/// </summary>
		/// <param name="c">Collection where the new element will be added.</param>
		/// <param name="obj">Object to add.</param>
		/// <returns>true</returns>
		public static bool Add(System.Collections.ICollection c, System.Object obj)
		{
			bool added = false;
			//Reflection. Invoke either the "add" or "Add" method.
			System.Reflection.MethodInfo method;
			try
			{
				//Get the "add" method for proprietary classes
				method = c.GetType().GetMethod("Add");
				if (method == null)
					method = c.GetType().GetMethod("add");
				int index = (int) method.Invoke(c, new System.Object[] {obj});
				if (index >=0)	
					added = true;
			}
			catch (System.Exception e)
			{
				throw e;
			}
			return added;
		}

		/// <summary>
		/// Adds all of the elements of the "c" collection to the "target" collection.
		/// </summary>
		/// <param name="target">Collection where the new elements will be added.</param>
		/// <param name="c">Collection whose elements will be added.</param>
		/// <returns>Returns true if at least one element was added, false otherwise.</returns>
		public static bool AddAll(System.Collections.ICollection target, System.Collections.ICollection c)
		{
			System.Collections.IEnumerator e = new System.Collections.ArrayList(c).GetEnumerator();
			bool added = false;

			//Reflection. Invoke "addAll" method for proprietary classes
			System.Reflection.MethodInfo method;
			try
			{
				method = target.GetType().GetMethod("addAll");

				if (method != null)
					added = (bool) method.Invoke(target, new System.Object[] {c});
				else
				{
					method = target.GetType().GetMethod("Add");
					while (e.MoveNext() == true)
					{
						bool tempBAdded =  (int) method.Invoke(target, new System.Object[] {e.Current}) >= 0;
						added = added ? added : tempBAdded;
					}
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			return added;
		}

		/// <summary>
		/// Removes all the elements from the collection.
		/// </summary>
		/// <param name="c">The collection to remove elements.</param>
		public static void Clear(System.Collections.ICollection c)
		{
			//Reflection. Invoke "Clear" method or "clear" method for proprietary classes
			System.Reflection.MethodInfo method;
			try
			{
				method = c.GetType().GetMethod("Clear");

				if (method == null)
					method = c.GetType().GetMethod("clear");

				method.Invoke(c, new System.Object[] {});
			}
			catch (System.Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Determines whether the collection contains the specified element.
		/// </summary>
		/// <param name="c">The collection to check.</param>
		/// <param name="obj">The object to locate in the collection.</param>
		/// <returns>true if the element is in the collection.</returns>
		public static bool Contains(System.Collections.ICollection c, System.Object obj)
		{
			bool contains = false;

			//Reflection. Invoke "contains" method for proprietary classes
			System.Reflection.MethodInfo method;
			try
			{
				method = c.GetType().GetMethod("Contains");

				if (method == null)
					method = c.GetType().GetMethod("contains");

				contains = (bool)method.Invoke(c, new System.Object[] {obj});
			}
			catch (System.Exception e)
			{
				throw e;
			}

			return contains;
		}

		/// <summary>
		/// Determines whether the collection contains all the elements in the specified collection.
		/// </summary>
		/// <param name="target">The collection to check.</param>
		/// <param name="c">Collection whose elements would be checked for containment.</param>
		/// <returns>true id the target collection contains all the elements of the specified collection.</returns>
		public static bool ContainsAll(System.Collections.ICollection target, System.Collections.ICollection c)
		{						
			System.Collections.IEnumerator e =  c.GetEnumerator();

			bool contains = false;

			//Reflection. Invoke "containsAll" method for proprietary classes or "Contains" method for each element in the collection
			System.Reflection.MethodInfo method;
			try
			{
				method = target.GetType().GetMethod("containsAll");

				if (method != null)
					contains = (bool)method.Invoke(target, new Object[] {c});
				else
				{					
					method = target.GetType().GetMethod("Contains");
					while (e.MoveNext() == true)
					{
						if ((contains = (bool)method.Invoke(target, new Object[] {e.Current})) == false)
							break;
					}
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}

			return contains;
		}

		/// <summary>
		/// Removes the specified element from the collection.
		/// </summary>
		/// <param name="c">The collection where the element will be removed.</param>
		/// <param name="obj">The element to remove from the collection.</param>
		public static bool Remove(System.Collections.ICollection c, System.Object obj)
		{
			bool changed = false;

			//Reflection. Invoke "remove" method for proprietary classes or "Remove" method
			System.Reflection.MethodInfo method;
			try
			{
				method = c.GetType().GetMethod("remove");

				if (method != null)
					method.Invoke(c, new System.Object[] {obj});
				else
				{
					method = c.GetType().GetMethod("Contains");
					changed = (bool)method.Invoke(c, new System.Object[] {obj});
					method = c.GetType().GetMethod("Remove");
					method.Invoke(c, new System.Object[] {obj});
				}
			}
			catch (System.Exception e)
			{
				throw e;
			}

			return changed;
		}

		/// <summary>
		/// Removes all the elements from the specified collection that are contained in the target collection.
		/// </summary>
		/// <param name="target">Collection where the elements will be removed.</param>
		/// <param name="c">Elements to remove from the target collection.</param>
		/// <returns>true</returns>
		public static bool RemoveAll(System.Collections.ICollection target, System.Collections.ICollection c)
		{
			System.Collections.ArrayList al = ToArrayList(c);
			System.Collections.IEnumerator e = al.GetEnumerator();

			//Reflection. Invoke "removeAll" method for proprietary classes or "Remove" for each element in the collection
			System.Reflection.MethodInfo method;
			try
			{
				method = target.GetType().GetMethod("removeAll");

				if (method != null)
					method.Invoke(target, new System.Object[] {al});
				else
				{
					method = target.GetType().GetMethod("Remove");
					System.Reflection.MethodInfo methodContains = target.GetType().GetMethod("Contains");

					while (e.MoveNext() == true)
					{
						while ((bool) methodContains.Invoke(target, new System.Object[] {e.Current}) == true)
							method.Invoke(target, new System.Object[] {e.Current});
					}
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			return true;
		}

		/// <summary>
		/// Retains the elements in the target collection that are contained in the specified collection
		/// </summary>
		/// <param name="target">Collection where the elements will be removed.</param>
		/// <param name="c">Elements to be retained in the target collection.</param>
		/// <returns>true</returns>
		public static bool RetainAll(System.Collections.ICollection target, System.Collections.ICollection c)
		{
			System.Collections.IEnumerator e = new System.Collections.ArrayList(target).GetEnumerator();
			System.Collections.ArrayList al = new System.Collections.ArrayList(c);

			//Reflection. Invoke "retainAll" method for proprietary classes or "Remove" for each element in the collection
			System.Reflection.MethodInfo method;
			try
			{
				method = c.GetType().GetMethod("retainAll");

				if (method != null)
					method.Invoke(target, new System.Object[] {c});
				else
				{
					method = c.GetType().GetMethod("Remove");

					while (e.MoveNext() == true)
					{
						if (al.Contains(e.Current) == false)
							method.Invoke(target, new System.Object[] {e.Current});
					}
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}

			return true;
		}

		/// <summary>
		/// Returns an array containing all the elements of the collection.
		/// </summary>
		/// <returns>The array containing all the elements of the collection.</returns>
		public static System.Object[] ToArray(System.Collections.ICollection c)
		{	
			int index = 0;
			System.Object[] objects = new System.Object[c.Count];
			System.Collections.IEnumerator e = c.GetEnumerator();

			while (e.MoveNext())
				objects[index++] = e.Current;

			return objects;
		}

		/// <summary>
		/// Obtains an array containing all the elements of the collection.
		/// </summary>
		/// <param name="objects">The array into which the elements of the collection will be stored.</param>
		/// <returns>The array containing all the elements of the collection.</returns>
		public static System.Object[] ToArray(System.Collections.ICollection c, System.Object[] objects)
		{	
			int index = 0;

			System.Type type = objects.GetType().GetElementType();
			System.Object[] objs = (System.Object[]) Array.CreateInstance(type, c.Count );

			System.Collections.IEnumerator e = c.GetEnumerator();

			while (e.MoveNext())
				objs[index++] = e.Current;

			//If objects is smaller than c then do not return the new array in the parameter
			if (objects.Length >= c.Count)
				objs.CopyTo(objects, 0);

			return objs;
		}

		/// <summary>
		/// Converts an ICollection instance to an ArrayList instance.
		/// </summary>
		/// <param name="c">The ICollection instance to be converted.</param>
		/// <returns>An ArrayList instance in which its elements are the elements of the ICollection instance.</returns>
		public static System.Collections.ArrayList ToArrayList(System.Collections.ICollection c)
		{
			System.Collections.ArrayList tempArrayList = new System.Collections.ArrayList();
			System.Collections.IEnumerator tempEnumerator = c.GetEnumerator();
			while (tempEnumerator.MoveNext())
				tempArrayList.Add(tempEnumerator.Current);
			return tempArrayList;
		}
	}


	/*******************************/
	/// <summary>
	/// Provides functionality for classes that implements the IList interface.
	/// </summary>
	public class IListSupport
	{
		/// <summary>
		/// Ensures the capacity of the list to be greater or equal than the specified.
		/// </summary>
		/// <param name="list">The list to be checked.</param>
		/// <param name="capacity">The expected capacity.</param>
		public static void EnsureCapacity(System.Collections.ArrayList list, int capacity)
		{
			if (list.Capacity < capacity) list.Capacity = 2 * list.Capacity;
			if (list.Capacity < capacity) list.Capacity = capacity;
		}

		/// <summary>
		/// Adds all the elements contained into the specified collection, starting at the specified position.
		/// </summary>
		/// <param name="index">Position at which to add the first element from the specified collection.</param>
		/// <param name="list">The list used to extract the elements that will be added.</param>
		/// <returns>Returns true if all the elements were successfuly added. Otherwise returns false.</returns>
		public static bool AddAll(System.Collections.IList list, int index, System.Collections.ICollection c)
		{
			bool result = false;
			if (c != null)
			{
				System.Collections.IEnumerator tempEnumerator = new System.Collections.ArrayList(c).GetEnumerator();
				int tempIndex = index;

				while (tempEnumerator.MoveNext())
				{
					list.Insert(tempIndex++, tempEnumerator.Current);
					result = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns an enumerator of the collection starting at the specified position.
		/// </summary>
		/// <param name="index">The position to set the iterator.</param>
		/// <returns>An IEnumerator at the specified position.</returns>
		public static System.Collections.IEnumerator GetEnumerator(System.Collections.IList list, int index)
		{
			if ((index < 0) || (index > list.Count)) 
				throw new System.IndexOutOfRangeException();			

			System.Collections.IEnumerator tempEnumerator = list.GetEnumerator();
			if (index > 0)
			{
				int i=0;
				while ((tempEnumerator.MoveNext()) && (i < index - 1))
					i++;
			}
			return tempEnumerator;
		}
	}


	/*******************************/
	/// <summary>
	/// SupportClass for the HashSet class.
	/// </summary>
	[Serializable]
	public class HashSetSupport : System.Collections.ArrayList, SetSupport
	{
		public HashSetSupport() : base()
		{	
		}

		public HashSetSupport(System.Collections.ICollection c) 
		{
			this.AddAll(c);
		}

		public HashSetSupport(int capacity) : base(capacity)
		{
		}

		/// <summary>
		/// Adds a new element to the ArrayList if it is not already present.
		/// </summary>		
		/// <param name="obj">Element to insert to the ArrayList.</param>
		/// <returns>Returns true if the new element was inserted, false otherwise.</returns>
		new public virtual bool Add(System.Object obj)
		{
			bool inserted;

			if ((inserted = this.Contains(obj)) == false)
			{
				base.Add(obj);
			}

			return !inserted;
		}

		/// <summary>
		/// Adds all the elements of the specified collection that are not present to the list.
		/// </summary>
		/// <param name="c">Collection where the new elements will be added</param>
		/// <returns>Returns true if at least one element was added, false otherwise.</returns>
		public bool AddAll(System.Collections.ICollection c)
		{
			System.Collections.IEnumerator e = new System.Collections.ArrayList(c).GetEnumerator();
			bool added = false;

			while (e.MoveNext() == true)
			{
				if (this.Add(e.Current) == true)
					added = true;
			}

			return added;
		}
		
		/// <summary>
		/// Returns a copy of the HashSet instance.
		/// </summary>		
		/// <returns>Returns a shallow copy of the current HashSet.</returns>
		public override System.Object Clone()
		{
			return base.MemberwiseClone();
		}
	}


	/*******************************/
	/// <summary>
	/// Represents a collection ob objects that contains no duplicate elements.
	/// </summary>	
	public interface SetSupport : System.Collections.ICollection, System.Collections.IList
	{
		/// <summary>
		/// Adds a new element to the Collection if it is not already present.
		/// </summary>
		/// <param name="obj">The object to add to the collection.</param>
		/// <returns>Returns true if the object was added to the collection, otherwise false.</returns>
		new bool Add(System.Object obj);

		/// <summary>
		/// Adds all the elements of the specified collection to the Set.
		/// </summary>
		/// <param name="c">Collection of objects to add.</param>
		/// <returns>true</returns>
		bool AddAll(System.Collections.ICollection c);
	}


	/*******************************/
	/// <summary>
	/// Converts the specified collection to its string representation.
	/// </summary>
	/// <param name="c">The collection to convert to string.</param>
	/// <returns>A string representation of the specified collection.</returns>
	public static System.String CollectionToString(System.Collections.ICollection c)
	{
		System.Text.StringBuilder s = new System.Text.StringBuilder();
		
		if (c != null)
		{
		
			System.Collections.ArrayList l = new System.Collections.ArrayList(c);

			bool isDictionary = (c is System.Collections.BitArray || c is System.Collections.Hashtable || c is System.Collections.IDictionary || c is System.Collections.Specialized.NameValueCollection || (l.Count > 0 && l[0] is System.Collections.DictionaryEntry));
			for (int index = 0; index < l.Count; index++) 
			{
				if (l[index] == null)
					s.Append("null");
				else if (!isDictionary)
					s.Append(l[index]);
				else
				{
					isDictionary = true;
					if (c is System.Collections.Specialized.NameValueCollection)
						s.Append(((System.Collections.Specialized.NameValueCollection)c).GetKey (index));
					else
						s.Append(((System.Collections.DictionaryEntry) l[index]).Key);
					s.Append("=");
					if (c is System.Collections.Specialized.NameValueCollection)
						s.Append(((System.Collections.Specialized.NameValueCollection)c).GetValues(index)[0]);
					else
						s.Append(((System.Collections.DictionaryEntry) l[index]).Value);

				}
				if (index < l.Count - 1)
					s.Append(", ");
			}
			
			if(isDictionary)
			{
				if(c is System.Collections.ArrayList)
					isDictionary = false;
			}
			if (isDictionary)
			{
				s.Insert(0, "{");
				s.Append("}");
			}
			else 
			{
				s.Insert(0, "[");
				s.Append("]");
			}
		}
		else
			s.Insert(0, "null");
		return s.ToString();
	}

	/// <summary>
	/// Tests if the specified object is a collection and converts it to its string representation.
	/// </summary>
	/// <param name="obj">The object to convert to string</param>
	/// <returns>A string representation of the specified object.</returns>
	public static System.String CollectionToString(System.Object obj)
	{
		System.String result = "";

		if (obj != null)
		{
			if (obj is System.Collections.ICollection)
				result = CollectionToString((System.Collections.ICollection)obj);
			else
				result = obj.ToString();
		}
		else
			result = "null";

		return result;
	}
	/*******************************/
	/// <summary>
	/// SupportClass for the SortedSet interface.
	/// </summary>
	public interface SortedSetSupport : SetSupport
	{
		/// <summary>
		/// Returns a portion of the list whose elements are less than the limit object parameter.
		/// </summary>
		/// <param name="l">The list where the portion will be extracted.</param>
		/// <param name="limit">The end element of the portion to extract.</param>
		/// <returns>The portion of the collection whose elements are less than the limit object parameter.</returns>
		SortedSetSupport HeadSet(System.Object limit);

		/// <summary>
		/// Returns a portion of the list whose elements are greater that the lowerLimit parameter less than the upperLimit parameter.
		/// </summary>
		/// <param name="l">The list where the portion will be extracted.</param>
		/// <param name="limit">The start element of the portion to extract.</param>
		/// <param name="limit">The end element of the portion to extract.</param>
		/// <returns>The portion of the collection.</returns>
		SortedSetSupport SubSet(System.Object lowerLimit, System.Object upperLimit);

		/// <summary>
		/// Returns a portion of the list whose elements are greater than the limit object parameter.
		/// </summary>
		/// <param name="l">The list where the portion will be extracted.</param>
		/// <param name="limit">The start element of the portion to extract.</param>
		/// <returns>The portion of the collection whose elements are greater than the limit object parameter.</returns>
		SortedSetSupport TailSet(System.Object limit);
	}


	/*******************************/
	/// <summary>
	/// SupportClass for the TreeSet class.
	/// </summary>
	[Serializable]
	public class TreeSetSupport : System.Collections.ArrayList, SetSupport, SortedSetSupport
	{
		private System.Collections.IComparer comparator = System.Collections.Comparer.Default;

		public TreeSetSupport() : base()
		{
		}

		public TreeSetSupport(System.Collections.ICollection c) : base()
		{
			this.AddAll(c);
		}

		public TreeSetSupport(System.Collections.IComparer c) : base()
		{
			this.comparator = c;
		}

		/// <summary>
		/// Gets the IComparator object used to sort this set.
		/// </summary>
		public System.Collections.IComparer Comparator
		{
			get
			{
				return this.comparator;
			}
		}

		/// <summary>
		/// Adds a new element to the ArrayList if it is not already present and sorts the ArrayList.
		/// </summary>
		/// <param name="obj">Element to insert to the ArrayList.</param>
		/// <returns>TRUE if the new element was inserted, FALSE otherwise.</returns>
		new public bool Add(System.Object obj)
		{
			bool inserted;
			if ((inserted = this.Contains(obj)) == false)
			{
				base.Add(obj);
				this.Sort(this.comparator);
			}
			return !inserted;
		}

		/// <summary>
		/// Adds all the elements of the specified collection that are not present to the list.
		/// </summary>		
		/// <param name="c">Collection where the new elements will be added</param>
		/// <returns>Returns true if at least one element was added to the collection.</returns>
		public bool AddAll(System.Collections.ICollection c)
		{
			System.Collections.IEnumerator e = new System.Collections.ArrayList(c).GetEnumerator();
			bool added = false;
			while (e.MoveNext() == true)
			{
				if (this.Add(e.Current) == true)
					added = true;
			}
			this.Sort(this.comparator);
			return added;
		}

		/// <summary>
		/// Determines whether an element is in the the current TreeSetSupport collection. The IComparer defined for 
		/// the current set will be used to make comparisons between the elements already inserted in the collection and 
		/// the item specified.
		/// </summary>
		/// <param name="item">The object to be locatet in the current collection.</param>
		/// <returns>true if item is found in the collection; otherwise, false.</returns>
		public override bool Contains(System.Object item)
		{
			System.Collections.IEnumerator tempEnumerator = this.GetEnumerator();
			while (tempEnumerator.MoveNext())
				if (this.comparator.Compare(tempEnumerator.Current, item) == 0)
					return true;
			return false;
		}

		/// <summary>
		/// Returns a portion of the list whose elements are less than the limit object parameter.
		/// </summary>
		/// <param name="limit">The end element of the portion to extract.</param>
		/// <returns>The portion of the collection whose elements are less than the limit object parameter.</returns>
		public SortedSetSupport HeadSet(System.Object limit)
		{
			SortedSetSupport newList = new TreeSetSupport();
			for (int i = 0; i < this.Count; i++)
			{
				if (this.comparator.Compare(this[i], limit) >= 0)
					break;
				newList.Add(this[i]);
			}
			return newList;
		}

		/// <summary>
		/// Returns a portion of the list whose elements are greater that the lowerLimit parameter less than the upperLimit parameter.
		/// </summary>
		/// <param name="lowerLimit">The start element of the portion to extract.</param>
		/// <param name="upperLimit">The end element of the portion to extract.</param>
		/// <returns>The portion of the collection.</returns>
		public SortedSetSupport SubSet(System.Object lowerLimit, System.Object upperLimit)
		{
			SortedSetSupport newList = new TreeSetSupport();
			int i = 0;
			while (this.comparator.Compare(this[i], lowerLimit) < 0)
				i++;
			for (; i < this.Count; i++)
			{
				if (this.comparator.Compare(this[i], upperLimit) >= 0)
					break;
				newList.Add(this[i]);
			}
			return newList;
		}

		/// <summary>
		/// Returns a portion of the list whose elements are greater than the limit object parameter.
		/// </summary>
		/// <param name="limit">The start element of the portion to extract.</param>
		/// <returns>The portion of the collection whose elements are greater than the limit object parameter.</returns>
		public SortedSetSupport TailSet(System.Object limit)
		{
			SortedSetSupport newList = new TreeSetSupport();
			int i = 0;
			while (this.comparator.Compare(this[i], limit) < 0)
				i++;
			for (; i < this.Count; i++)
				newList.Add(this[i]);
			return newList;
		}
	}


	/*******************************/
	/// <summary>
	/// The ProgressMonitorSupport is used to report on the status of a time-consuming task.
	/// </summary>
	public class ProgressMonitorSupport : System.Windows.Forms.Form
	{
		/// <summary>
		/// Visual components of the form.
		/// </summary>
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button cancelButton;

		/// <summary>
		/// Internal properties
		/// </summary>
		private System.Timers.Timer decideToPopup;
		private double millisToDecideToPopup;
		private System.Timers.Timer toPopup;
		private double millisToPopup;		
		private int progress;
		private System.Windows.Forms.Panel messagePanel;
		private System.Windows.Forms.Label noteLabel;
		private bool canceled;
		private System.Windows.Forms.Control parentWindow;
		private System.String note;

		/// <summary>
		/// A monitor to close the window when the task is done.
		/// </summary>
		private System.Timers.Timer monitor;
		private System.Windows.Forms.PictureBox infoIconPictureBox; 

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates a new progress monitor to report on the status of a time-consuming task.
		/// </summary>
		/// <param name="parent">Parent window of the progress monitor dialog.</param>
		/// <param name="message">A fixed message that always displays the same.</param>
		/// <param name="note">Variable message that can change at the task progresses.</param>
		/// <param name="minimum">The minimum value of the taskbar.</param>
		/// <param name="maximum">The maximum value of the taskbar.</param>
		public ProgressMonitorSupport(System.Windows.Forms.Control parent, System.Object message, System.String note, int minimum, int maximum)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.millisToDecideToPopup = 500;
			this.millisToPopup = 2000;
			this.parentWindow = parent;

			this.decideToPopup = new System.Timers.Timer(this.millisToDecideToPopup);
			this.decideToPopup.AutoReset = false;
			this.decideToPopup.Elapsed += new System.Timers.ElapsedEventHandler(decideToPopup_Elapsed);
			this.decideToPopup.Start();

			this.toPopup = new System.Timers.Timer(this.millisToPopup);
			this.toPopup.AutoReset = false;
			this.toPopup.Elapsed += new System.Timers.ElapsedEventHandler(toPopup_Elapsed);
			this.toPopup.Start();

			this.monitor = new System.Timers.Timer(500);
			this.monitor.AutoReset = true;
			this.monitor.Elapsed += new System.Timers.ElapsedEventHandler(monitor_Elapsed);
			this.monitor.Start();

			this.progressBar.Minimum = minimum;
			this.progressBar.Maximum = maximum;

			this.Location = new System.Drawing.Point((parent.Width - this.Width) / 2, (parent.Height - this.Height) / 2);
				
			this.noteLabel.Text = note;
			this.note = note;
			this.messageLook(message);
	            
			this.canceled = false;			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.noteLabel = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.messagePanel = new System.Windows.Forms.Panel();
			this.infoIconPictureBox = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(42, 105);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(306, 16);
			this.progressBar.TabIndex = 0;
			// 
			// noteLabel
			// 
			this.noteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.noteLabel.ForeColor = System.Drawing.SystemColors.Highlight;
			this.noteLabel.Location = new System.Drawing.Point(64, 69);
			this.noteLabel.Name = "noteLabel";
			this.noteLabel.Size = new System.Drawing.Size(312, 24);
			this.noteLabel.TabIndex = 1;
			this.noteLabel.Text = "messageLabel";
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(160, 129);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(64, 24);
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// messagePanel
			// 
			this.messagePanel.Location = new System.Drawing.Point(64, 8);
			this.messagePanel.Name = "messagePanel";
			this.messagePanel.Size = new System.Drawing.Size(312, 48);
			this.messagePanel.TabIndex = 5;
			// 
			// infoIconPictureBox
			// 
			this.infoIconPictureBox.Location = new System.Drawing.Point(8, 16);
			this.infoIconPictureBox.Name = "infoIconPictureBox";
			this.infoIconPictureBox.Size = new System.Drawing.Size(48, 48);
			this.infoIconPictureBox.TabIndex = 6;
			this.infoIconPictureBox.TabStop = false;
			// 
			// ProgressMonitorSupport
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(384, 166);
			this.Controls.Add(this.infoIconPictureBox);
			this.Controls.Add(this.messagePanel);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.noteLabel);
			this.Controls.Add(this.progressBar);
			this.Name = "ProgressMonitorSupport";
			this.Text = "Progress...";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Get or Sets the time that the monitor waits before decide if it needs to show the dialog.
		/// </summary>
		public virtual int DecideToPopup
		{
			get
			{
				return (int)this.millisToDecideToPopup;
			}
			set
			{
				this.millisToDecideToPopup = value;
				this.decideToPopup = new System.Timers.Timer(this.millisToDecideToPopup);
			}
		}

		/// <summary>
		/// Gets or Sets the time that the monitor waits before show the dialog.
		/// </summary>
		public virtual int ToPopup
		{
			get
			{
				return (int)this.millisToPopup;
			}
			set
			{
				this.millisToPopup = value;
				this.toPopup = new System.Timers.Timer(this.millisToPopup);
			}
		}

		/// <summary>
		/// Sets the progress of the taskbar.
		/// </summary>
		public virtual int Progress
		{
			set
			{
				this.progress = value;
			}
		}

		/// <summary>
		/// Gets a value indicating if the task was canceled
		/// </summary>
		public virtual bool Canceled
		{
			get
			{
				return this.canceled;
			}
		}

		/// <summary>
		/// Gets or sets the variable note that describes the progress of the task.
		/// </summary>
		public virtual System.String Note
		{
			get
			{
				return this.note;
			}
			set
			{
				this.note = value;			
				this.noteLabel.Text = this.note;
			}
		}

		/// <summary>
		/// The maximum value of the range for the progress bar.
		/// </summary>
		public virtual int Maximum
		{
			get
			{
				return this.progressBar.Maximum;
			}
			set
			{
				this.progressBar.Maximum = value;
			}
		}

		/// <summary>
		/// The minimum value of the range for the progress bar.
		/// </summary>
		public virtual int Minimum
		{
			get
			{
				return this.progressBar.Minimum;
			}
			set
			{
				this.progressBar.Minimum = value;
			}
		}

		private void messageLook(System.Object message)
		{
			if (message is System.Array)
			{
				System.Object[] array = (System.Object[])message;
				if (array.Length < 4)
				{
					for (int index = 0; index < array.Length; index++)
						this.messageLook(array[index]);
				}
			}
			else if (message is System.Drawing.Image)
			{
				System.Windows.Forms.PictureBox image = new System.Windows.Forms.PictureBox();
				image.Image = (System.Drawing.Image)message;
				this.messagePanel.Controls.Add(image);
				image.Dock = System.Windows.Forms.DockStyle.Fill;
			}
			else if (message is System.String)
			{
				System.Windows.Forms.Label label = new System.Windows.Forms.Label();
				label.Font = new System.Drawing.Font("Arial", 10.0f);
				label.Text = (System.String)message;
				this.messagePanel.Controls.Add(label);
				label.Dock = System.Windows.Forms.DockStyle.Bottom;
			}
			else
			{
				System.Windows.Forms.Label label = new System.Windows.Forms.Label();
				label.Font = new System.Drawing.Font("Arial", 10.0f);
				label.Text = message.ToString();
				this.messagePanel.Controls.Add(label);
				label.Dock = System.Windows.Forms.DockStyle.Bottom;
			}
		}

		private void decideToPopup_Elapsed(System.Object sender, System.Timers.ElapsedEventArgs e)
		{
			int progressMade = (this.progress * 100) / this.progressBar.Maximum;
			int limitTime = (int) ((this.millisToDecideToPopup * 100) / this.millisToPopup);
			this.decideToPopup.Stop();
			if (progressMade > limitTime)
				this.Close();
		}

		private void toPopup_Elapsed(System.Object sender, System.Timers.ElapsedEventArgs e)
		{
			this.ShowDialog(this.parentWindow);
			this.toPopup.Stop();
		}

		private void cancelButton_Click(System.Object sender, System.EventArgs e)
		{
			this.Hide();
			this.canceled = true;
		}

		private void monitor_Elapsed(System.Object sender, System.Timers.ElapsedEventArgs e)
		{
			// If the task is done just close the form.
			if (this.progress >= this.progressBar.Maximum)
				this.Close();
			else
				this.progressBar.Value = this.progress;
		}
	}


	/*******************************/
	/// <summary>
	/// Defines a method to handle basic events and convert the Action interface
	/// </summary>
	[Serializable]
	public abstract class ActionSupport
	{
		private System.Drawing.Image icon;
		private System.String description;

		/// <summary>
		/// Creates a new ActionSupport.
		/// </summary>		
		public ActionSupport()
		{
			this.description = null;	
			this.icon = null;		
		}


		/// <summary>
		/// Creates a new ActionSupport.
		/// </summary>
		/// <param name="description">The description for this Action</param>
		/// <param name="icon">The icon for this Action</param>
		public ActionSupport(System.String description, System.Drawing.Image icon)
		{
			this.description = description;
			this.icon = icon;
		}

		/// <summary>
		/// Creates a new ActionSupport
		/// </summary>
		/// <param name="description">The description for this Action</param>
		public ActionSupport(System.String description)
		{
			this.description = description;	
			this.icon = null;		
		}

		/// <summary>
		/// .NET version for the actionPerformed method.
		/// </summary>
		/// <param name="event_sender">The event raising object.</param>
		/// <param name="eventArgs">The arguments for the event.</param>
		public abstract void actionPerformed(System.Object event_sender, System.EventArgs eventArgs);

		/// <summary>
		/// The icon this Action provides for controls that use it.
		/// </summary>
		public System.Drawing.Image Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				this.icon = value;                
			}
		}

		/// <summary>
		/// The text this Action provides for controls that use it.
		/// </summary>
		public System.String Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}
	}


	/*******************************/
	/// <summary>
	/// This class supports basic functionality of the JOptionPane class.
	/// </summary>
	public class OptionPaneSupport 
	{
		/// <summary>
		/// This method finds the form which contains an specific control.
		/// </summary>
		/// <param name="control">The control which we need to find its form.</param>
		/// <returns>The form which contains the control</returns>
		public static System.Windows.Forms.Form GetFrameForComponent(System.Windows.Forms.Control control)
		{
			System.Windows.Forms.Form result = null;
			if (control == null)return null;
			if (control is System.Windows.Forms.Form)
				result = (System.Windows.Forms.Form)control;
			else if (control.Parent != null)
				result = GetFrameForComponent(control.Parent);
			return result;
		}

		/// <summary>
		/// This method finds the MDI container form which contains an specific control.
		/// </summary>
		/// <param name="control">The control which we need to find its MDI container form.</param>
		/// <returns>The MDI container form which contains the control.</returns>
		public static System.Windows.Forms.Form GetDesktopPaneForComponent(System.Windows.Forms.Control control)
		{
			System.Windows.Forms.Form result = null;
			if (control == null)return null;
			if (control.GetType().IsSubclassOf(typeof(System.Windows.Forms.Form)))
				if (((System.Windows.Forms.Form)control).IsMdiContainer)
					result = (System.Windows.Forms.Form)control;
				else if (((System.Windows.Forms.Form)control).IsMdiChild)
					result = GetDesktopPaneForComponent(((System.Windows.Forms.Form)control).MdiParent);
				else if (control.Parent != null)
					result = GetDesktopPaneForComponent(control.Parent);
			return result;
		}
		
		/// <summary>
		/// This method retrieves the message that is contained into the object that is sended by the user.
		/// </summary>
		/// <param name="control">The control which we need to find its form.</param>
		/// <returns>The form which contains the control</returns>
		public static System.String GetMessageForObject(System.Object message)
		{
			System.String result = "";
			if (message == null)
			  return result;
			else 
		 	  result = message.ToString();
			return result;
		}


		/// <summary>
		/// This method displays a dialog with a Yes, No, Cancel buttons and a question icon.
		/// </summary>
		/// <param name="parent">The component which will be the owner of the dialog.</param>
		/// <param name="message">The message to be displayed; if it isn't an String it displays the return value of the ToString() method of the object.</param>
		/// <returns>The integer value which represents the button pressed.</returns>
		public static int ShowConfirmDialog(System.Windows.Forms.Control parent, System.Object message)
		{
			return ShowConfirmDialog(parent, message,"Select an option...", (int)System.Windows.Forms.MessageBoxButtons.YesNoCancel,
				(int)System.Windows.Forms.MessageBoxIcon.Question);
		}

		/// <summary>
		/// This method displays a dialog with specific buttons and a question icon.
		/// </summary>
		/// <param name="parent">The component which will be the owner of the dialog.</param>
		/// <param name="message">The message to be displayed; if it isn't an String it displays the result value of the ToString() method of the object.</param>
		/// <param name="title">The title for the message dialog.</param>
		/// <param name="optiontype">The set of buttons to be displayed in the message box; defined by the MessageBoxButtons enumeration.</param>
		/// <returns>The integer value which represents the button pressed.</returns>
		public static int ShowConfirmDialog(System.Windows.Forms.Control parent, System.Object message,
			System.String title,int optiontype)
		{
			return ShowConfirmDialog(parent, message, title, optiontype, (int)System.Windows.Forms.MessageBoxIcon.Question);
		}

		/// <summary>
		/// This method displays a dialog with specific buttons and specific icon.
		/// </summary>
		/// <param name="parent">The component which will be the owner of the dialog.</param>
		/// <param name="message">The message to be displayed; if it isn't an String it displays the return value of the ToString() method of the object.</param>
		/// <param name="title">The title for the message dialog.</param>
		/// <param name="optiontype">The set of buttons to be displayed in the message box; defined by the MessageBoxButtons enumeration.</param>
		/// <param name="messagetype">The messagetype defines the icon to be displayed in the message box.</param>
		/// <returns>The integer value which represents the button pressed.</returns>
		public static int ShowConfirmDialog(System.Windows.Forms.Control parent, System.Object message,
			System.String title, int optiontype, int messagetype)
		{
			return (int)System.Windows.Forms.MessageBox.Show(GetFrameForComponent(parent), GetMessageForObject(message), title,
				(System.Windows.Forms.MessageBoxButtons)optiontype, (System.Windows.Forms.MessageBoxIcon)messagetype);
		}

		/// <summary>
		/// This method displays a simple MessageBox.
		/// </summary>
		/// <param name="parent">The component which will be the owner of the dialog.</param>
		/// <param name="message">The message to be displayed; if it isn't an String it displays result value of the ToString() method of the object.</param>
		public static void ShowMessageDialog(System.Windows.Forms.Control parent, System.Object message)
		{
			ShowMessageDialog(parent, message, "Message", (int)System.Windows.Forms.MessageBoxIcon.Information);
		}

		/// <summary>
		/// This method displays a simple MessageBox with a specific icon.
		/// </summary>
		/// <param name="parent">The component which will be the owner of the dialog.</param>
		/// <param name="message">The message to be displayed; if it isn't an String it displays result value of the ToString() method of the object.</param>
		/// <param name="title">The title for the message dialog.</param>
		/// <param name="messagetype">The messagetype defines the icon to be displayed in the message box.</param>
		public static void ShowMessageDialog(System.Windows.Forms.Control parent, System.Object message,
			System.String title, int messagetype)
		{
			System.Windows.Forms.MessageBox.Show(GetFrameForComponent(parent), GetMessageForObject(message), title,
				System.Windows.Forms.MessageBoxButtons.OK, (System.Windows.Forms.MessageBoxIcon)messagetype);
		}
	}


	/*******************************/
	/// <summary>
	/// Retrieves the names of the fonts on the current context.
	/// </summary>
	/// <returns>A string array containing the names of the Fonts.</returns>
	public static System.String[] FontNames()
	{
		System.String[] fontArray;
		System.Drawing.FontFamily[] families = System.Drawing.FontFamily.Families;
		fontArray = new System.String[families.Length];
		for(int i = 0; i < families.Length; i++)
			fontArray[i] = families[i].Name;
		return fontArray;
	}

	/// <summary>
	/// Retrieves the name of the availables fonts for the specified culture.
	/// </summary>
	/// <param name="culture">The desired culture from which the fonts will be extracted.</param>
	/// <returns>A string array containing the names of the fonts.</returns>
	public static System.String[] FontNames(System.Globalization.CultureInfo culture)
	{
		System.String[] fontArray;
		System.Drawing.FontFamily[] families = System.Drawing.FontFamily.Families;
		fontArray = new System.String[families.Length];
		for(int i = 0; i < families.Length; i++)
			fontArray[i] = families[i].GetName(culture.LCID);
		return fontArray;
	}


	/*******************************/
	/// <summary>
	/// Support for creation and modification of combo box elements
	/// </summary>
	public class ComboBoxSupport
	{
		/// <summary>
		/// Creates a new ComboBox control with the specified items.
		/// </summary>
		/// <param name="items">Items to insert into the combo box</param>
		/// <returns>A new combo box that contains the specified items</returns>
		public static System.Windows.Forms.ComboBox CreateComboBox( System.Object[] items )
		{
			System.Windows.Forms.ComboBox combo = new System.Windows.Forms.ComboBox();
			combo.Items.AddRange( items );
			combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			return combo;
		}

		/// <summary>
		/// Sets the items property of the specified combo with the items specified.
		/// </summary>
		/// <param name="combo">ComboBox to be modified</param>
		/// <param name="items">Items to insert into the combo box</param>
		public static void SetComboBox( System.Windows.Forms.ComboBox combo , System.Object[] items )
		{
			combo.Items.AddRange( items );
			combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		}

		/// <summary>
		/// Creates a new ComboBox control with the specified collection of items.
		/// </summary>
		/// <param name="items">Items to insert into the combo box</param>
		/// <returns>A new combo box that contains the specified items collection of items</returns>
		public static System.Windows.Forms.ComboBox CreateComboBox( System.Collections.ArrayList items )
		{
			return ComboBoxSupport.CreateComboBox( items.ToArray() );
		}

		/// <summary>
		/// Sets the items property of the specified combo with the items collection specified.
		/// </summary>
		/// <param name="combo">ComboBox to be modified</param>
		/// <param name="items">Collection of items to insert into the combo box</param>
		public static void SetComboBox( System.Windows.Forms.ComboBox combo , System.Collections.ArrayList items )
		{
			ComboBoxSupport.SetComboBox( combo, items.ToArray() );
		}

		/// <summary>
		/// Returns an array that contains the selected item of the specified combo box
		/// </summary>
		/// <param name="combo">The combo box from which the selected item is returned</param>
		/// <returns>An array that contains the selected item of the combo box</returns>
		public static System.Object[] GetSelectedObjects( System.Windows.Forms.ComboBox combo )
		{
			System.Object[] selectedObjects = new System.Object[1];
			selectedObjects[0] = combo.SelectedItem;
			return selectedObjects;
		}

		/// <summary>
		/// Returns a value indicating if the text portion of the specified combo box is editable
		/// </summary>
		/// <param name="combo">The combo box from to check</param>
		/// <returns>True if the text portion of the combo box is editable, false otherwise</returns>
		public static bool IsEditable( System.Windows.Forms.ComboBox combo )
		{
			return !( combo.DropDownStyle == System.Windows.Forms.ComboBoxStyle.DropDownList );
		}
		
		/// <summary>
		/// Create a TextBox object using the ComboBox object received as parameter.
		/// </summary>
		/// <param name="comboBox"></param>
		/// <returns></returns>
		public static System.Windows.Forms.TextBox GetEditComponent(System.Windows.Forms.ComboBox comboBox)
		{
			System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
			textBox.Text = comboBox.Text;
			return textBox;
		}
	}
	/*******************************/
	/// <summary>
	/// Class used to store and retrieve an object command specified as a String.
	/// </summary>
	public class CommandManager
	{
		/// <summary>
		/// Private Hashtable used to store objects and their commands.
		/// </summary>
		private static System.Collections.Hashtable Commands = new System.Collections.Hashtable();

		/// <summary>
		/// Sets a command to the specified object.
		/// </summary>
		/// <param name="obj">The object that has the command.</param>
		/// <param name="cmd">The command for the object.</param>
		public static void SetCommand(System.Object obj, System.String cmd)
		{
			if (obj != null)
			{
				if (Commands.Contains(obj))
					Commands[obj] = cmd;
				else
					Commands.Add(obj, cmd);
			}
		}

		/// <summary>
		/// Gets a command associated with an object.
		/// </summary>
		/// <param name="obj">The object whose command is going to be retrieved.</param>
		/// <returns>The command of the specified object.</returns>
		public static System.String GetCommand(System.Object obj)
		{
			System.String result = "";
			if (obj != null)
				result = System.Convert.ToString(Commands[obj]);
			return result;
		}



		/// <summary>
		/// Checks if the Control contains a command, if it does not it sets the default
		/// </summary>
		/// <param name="button">The control whose command will be checked</param>
		public static void CheckCommand(System.Windows.Forms.ButtonBase button)
		{
			if (button != null)
			{
				if (GetCommand(button).Equals(""))
					SetCommand(button, button.Text);
			}
		}

		/// <summary>
		/// Checks if the Control contains a command, if it does not it sets the default
		/// </summary>
		/// <param name="button">The control whose command will be checked</param>
		public static void CheckCommand(System.Windows.Forms.MenuItem menuItem)
		{
			if (menuItem != null)
			{
				if (GetCommand(menuItem).Equals(""))
					SetCommand(menuItem, menuItem.Text);
			}
		}

		/// <summary>
		/// Checks if the Control contains a command, if it does not it sets the default
		/// </summary>
		/// <param name="button">The control whose command will be checked</param>
		public static void CheckCommand(System.Windows.Forms.ComboBox comboBox)
		{
			if (comboBox != null)
			{
				if (GetCommand(comboBox).Equals(""))
					SetCommand(comboBox,"comboBoxChanged");
			}
		}

	}
	/*******************************/
	/// <summary>
	/// Deserializes an object, or an entire graph of connected objects, and returns the object intance
	/// </summary>
	/// <param name="binaryReader">Reader instance used to read the object</param>
	/// <returns>The object instance</returns>
	public static System.Object Deserialize(System.IO.BinaryReader binaryReader)
	{
		System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		return formatter.Deserialize(binaryReader.BaseStream);
	}

	/*******************************/
/// <summary>
/// Provides support for DateFormat
/// </summary>
public class DateTimeFormatManager
{
	static public DateTimeFormatHashTable manager = new DateTimeFormatHashTable();

	/// <summary>
	/// Hashtable class to provide functionality for dateformat properties
	/// </summary>
	public class DateTimeFormatHashTable :System.Collections.Hashtable 
	{
		/// <summary>
		/// Sets the format for datetime.
		/// </summary>
		/// <param name="format">DateTimeFormat instance to set the pattern</param>
		/// <param name="newPattern">A string with the pattern format</param>
		public void SetDateFormatPattern(System.Globalization.DateTimeFormatInfo format, System.String newPattern)
		{
			if (this[format] != null)
				((DateTimeFormatProperties) this[format]).DateFormatPattern = newPattern;
			else
			{
				DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
				tempProps.DateFormatPattern  = newPattern;
				Add(format, tempProps);
			}
		}

		/// <summary>
		/// Gets the current format pattern of the DateTimeFormat instance
		/// </summary>
		/// <param name="format">The DateTimeFormat instance which the value will be obtained</param>
		/// <returns>The string representing the current datetimeformat pattern</returns>
		public System.String GetDateFormatPattern(System.Globalization.DateTimeFormatInfo format)
		{
			if (this[format] == null)
				return "d-MMM-yy";
			else
				return ((DateTimeFormatProperties) this[format]).DateFormatPattern;
		}
		
		/// <summary>
		/// Sets the datetimeformat pattern to the giving format
		/// </summary>
		/// <param name="format">The datetimeformat instance to set</param>
		/// <param name="newPattern">The new datetimeformat pattern</param>
		public void SetTimeFormatPattern(System.Globalization.DateTimeFormatInfo format, System.String newPattern)
		{
			if (this[format] != null)
				((DateTimeFormatProperties) this[format]).TimeFormatPattern = newPattern;
			else
			{
				DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
				tempProps.TimeFormatPattern  = newPattern;
				Add(format, tempProps);
			}
		}

		/// <summary>
		/// Gets the current format pattern of the DateTimeFormat instance
		/// </summary>
		/// <param name="format">The DateTimeFormat instance which the value will be obtained</param>
		/// <returns>The string representing the current datetimeformat pattern</returns>
		public System.String GetTimeFormatPattern(System.Globalization.DateTimeFormatInfo format)
		{
			if (this[format] == null)
				return "h:mm:ss tt";
			else
				return ((DateTimeFormatProperties) this[format]).TimeFormatPattern;
		}

		/// <summary>
		/// Internal class to provides the DateFormat and TimeFormat pattern properties on .NET
		/// </summary>
		class DateTimeFormatProperties
		{
			public System.String DateFormatPattern = "d-MMM-yy";
			public System.String TimeFormatPattern = "h:mm:ss tt";
		}
	}	
}
	/*******************************/
	/// <summary>
	/// Gets the DateTimeFormat instance and date instance to obtain the date with the format passed
	/// </summary>
	/// <param name="format">The DateTimeFormat to obtain the time and date pattern</param>
	/// <param name="date">The date instance used to get the date</param>
	/// <returns>A string representing the date with the time and date patterns</returns>
	public static System.String FormatDateTime(System.Globalization.DateTimeFormatInfo format, System.DateTime date)
	{
		System.String timePattern = DateTimeFormatManager.manager.GetTimeFormatPattern(format);
		System.String datePattern = DateTimeFormatManager.manager.GetDateFormatPattern(format);
		return date.ToString(datePattern + " " + timePattern, format);            
	}

	/*******************************/
	/// <summary>
	/// Support Methods for FileDialog class. Note that several methods receive a DirectoryInfo object, but it won't be used in all cases.
	/// </summary>
	public class FileDialogSupport
	{
		/// <summary>
		/// Creates an OpenFileDialog open in a given path.
		/// </summary>
		/// <param name="path">Path to be opened by the OpenFileDialog.</param>
		/// <returns>A new instance of OpenFileDialog.</returns>
		public static System.Windows.Forms.OpenFileDialog CreateOpenFileDialog(System.IO.FileInfo path)
		{
			System.Windows.Forms.OpenFileDialog temp_fileDialog = new System.Windows.Forms.OpenFileDialog();
			temp_fileDialog.InitialDirectory = path.Directory.FullName;
			return temp_fileDialog;
		}

		/// <summary>
		/// Creates an OpenFileDialog open in a given path.
		/// </summary>
		/// <param name="path">Path to be opened by the OpenFileDialog.</param>
		/// <param name="directory">Directory to get the path from.</param>
		/// <returns>A new instance of OpenFileDialog.</returns>
		public static System.Windows.Forms.OpenFileDialog CreateOpenFileDialog(System.IO.FileInfo path, System.IO.DirectoryInfo directory)
		{
			System.Windows.Forms.OpenFileDialog temp_fileDialog = new System.Windows.Forms.OpenFileDialog();
			temp_fileDialog.InitialDirectory = path.Directory.FullName;
			return temp_fileDialog;
		}

		/// <summary>
		/// Creates a OpenFileDialog open in a given path.
		/// </summary>		
		/// <returns>A new instance of OpenFileDialog.</returns>
		public static System.Windows.Forms.OpenFileDialog CreateOpenFileDialog()
		{
			System.Windows.Forms.OpenFileDialog temp_fileDialog = new System.Windows.Forms.OpenFileDialog();
			temp_fileDialog.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);			
			return temp_fileDialog;
		}

		/// <summary>
		/// Creates an OpenFileDialog open in a given path.
		/// </summary>
		/// <param name="path">Path to be opened by the OpenFileDialog</param>
		/// <returns>A new instance of OpenFileDialog.</returns>
		public static System.Windows.Forms.OpenFileDialog CreateOpenFileDialog (System.String path)
		{
			System.Windows.Forms.OpenFileDialog temp_fileDialog = new System.Windows.Forms.OpenFileDialog();
			temp_fileDialog.InitialDirectory = path;
			return temp_fileDialog;
		}

		/// <summary>
		/// Creates an OpenFileDialog open in a given path.
		/// </summary>
		/// <param name="path">Path to be opened by the OpenFileDialog.</param>
		/// <param name="directory">Directory to get the path from.</param>
		/// <returns>A new instance of OpenFileDialog.</returns>
		public static System.Windows.Forms.OpenFileDialog CreateOpenFileDialog(System.String path, System.IO.DirectoryInfo directory)
		{
			System.Windows.Forms.OpenFileDialog temp_fileDialog = new System.Windows.Forms.OpenFileDialog();
			temp_fileDialog.InitialDirectory = path;
			return temp_fileDialog;
		}

		/// <summary>
		/// Modifies an instance of OpenFileDialog, to open a given directory.
		/// </summary>
		/// <param name="fileDialog">OpenFileDialog instance to be modified.</param>
		/// <param name="path">Path to be opened by the OpenFileDialog.</param>
		/// <param name="directory">Directory to get the path from.</param>
		public static void SetOpenFileDialog(System.Windows.Forms.FileDialog fileDialog, System.String path, System.IO.DirectoryInfo directory)
		{
			fileDialog.InitialDirectory = path;
		}

		/// <summary>
		/// Modifies an instance of OpenFileDialog, to open a given directory.
		/// </summary>
		/// <param name="fileDialog">OpenFileDialog instance to be modified.</param>
		/// <param name="path">Path to be opened by the OpenFileDialog</param>
		public static void SetOpenFileDialog(System.Windows.Forms.FileDialog fileDialog, System.IO.FileInfo path)
		{
			fileDialog.InitialDirectory = path.Directory.FullName;
		}

		/// <summary>
		/// Modifies an instance of OpenFileDialog, to open a given directory.
		/// </summary>
		/// <param name="fileDialog">OpenFileDialog instance to be modified.</param>
		/// <param name="path">Path to be opened by the OpenFileDialog.</param>
		public static void SetOpenFileDialog(System.Windows.Forms.FileDialog fileDialog, System.String path)
		{
			fileDialog.InitialDirectory = path;
		}

		///
		///  Use the following static methods to create instances of SaveFileDialog.
		///  By default, JFileChooser is converted as an OpenFileDialog, the following methods
		///  are provided to create file dialogs to save files.
		///	
		
		
		/// <summary>
		/// Creates a SaveFileDialog.
		/// </summary>		
		/// <returns>A new instance of SaveFileDialog.</returns>
		public static System.Windows.Forms.SaveFileDialog CreateSaveFileDialog()
		{
			System.Windows.Forms.SaveFileDialog temp_fileDialog = new System.Windows.Forms.SaveFileDialog();
			temp_fileDialog.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);			
			return temp_fileDialog;
		}

		/// <summary>
		/// Creates an SaveFileDialog open in a given path.
		/// </summary>
		/// <param name="path">Path to be opened by the SaveFileDialog.</param>
		/// <returns>A new instance of SaveFileDialog.</returns>
		public static System.Windows.Forms.SaveFileDialog CreateSaveFileDialog(System.IO.FileInfo path)
		{
			System.Windows.Forms.SaveFileDialog temp_fileDialog = new System.Windows.Forms.SaveFileDialog();
			temp_fileDialog.InitialDirectory = path.Directory.FullName;
			return temp_fileDialog;
		}

		/// <summary>
		/// Creates an SaveFileDialog open in a given path.
		/// </summary>
		/// <param name="path">Path to be opened by the SaveFileDialog.</param>
		/// <param name="directory">Directory to get the path from.</param>
		/// <returns>A new instance of SaveFileDialog.</returns>
		public static System.Windows.Forms.SaveFileDialog CreateSaveFileDialog(System.IO.FileInfo path, System.IO.DirectoryInfo directory)
		{
			System.Windows.Forms.SaveFileDialog temp_fileDialog = new System.Windows.Forms.SaveFileDialog();
			temp_fileDialog.InitialDirectory = path.Directory.FullName;
			return temp_fileDialog;
		}

		/// <summary>
		/// Creates a SaveFileDialog open in a given path.
		/// </summary>
		/// <param name="directory">Directory to get the path from.</param>
		/// <returns>A new instance of SaveFileDialog.</returns>
		public static System.Windows.Forms.SaveFileDialog CreateSaveFileDialog(System.IO.DirectoryInfo directory)
		{
			System.Windows.Forms.SaveFileDialog temp_fileDialog = new System.Windows.Forms.SaveFileDialog();
			temp_fileDialog.InitialDirectory = directory.FullName;
			return temp_fileDialog;
		}

		/// <summary>
		/// Creates an SaveFileDialog open in a given path.
		/// </summary>
		/// <param name="path">Path to be opened by the SaveFileDialog</param>
		/// <returns>A new instance of SaveFileDialog.</returns>
		public static System.Windows.Forms.SaveFileDialog CreateSaveFileDialog (System.String path)
		{
			System.Windows.Forms.SaveFileDialog temp_fileDialog = new System.Windows.Forms.SaveFileDialog();
			temp_fileDialog.InitialDirectory = path;
			return temp_fileDialog;
		}

		/// <summary>
		/// Creates an SaveFileDialog open in a given path.
		/// </summary>
		/// <param name="path">Path to be opened by the SaveFileDialog.</param>
		/// <param name="directory">Directory to get the path from.</param>
		/// <returns>A new instance of SaveFileDialog.</returns>
		public static System.Windows.Forms.SaveFileDialog CreateSaveFileDialog(System.String path, System.IO.DirectoryInfo directory)
		{
			System.Windows.Forms.SaveFileDialog temp_fileDialog = new System.Windows.Forms.SaveFileDialog();
			temp_fileDialog.InitialDirectory = path;
			return temp_fileDialog;
		}
	}
	/*******************************/
	/// <summary>
	/// Writes an object to the specified Stream
	/// </summary>
	/// <param name="stream">The target Stream</param>
	/// <param name="objectToSend">The object to be sent</param>
	public static void Serialize(System.IO.Stream stream, System.Object objectToSend)
	{
		System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		formatter.Serialize(stream, objectToSend);
	}

	/// <summary>
	/// Writes an object to the specified BinaryWriter
	/// </summary>
	/// <param name="stream">The target BinaryWriter</param>
	/// <param name="objectToSend">The object to be sent</param>
	public static void Serialize(System.IO.BinaryWriter binaryWriter, System.Object objectToSend)
	{
		System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		formatter.Serialize(binaryWriter.BaseStream, objectToSend);
	}

	/*******************************/
	/// <summary>
	/// Creates a GraphicsPath from two Int Arrays with a specific number of points.
	/// </summary>
	/// <param name="xPoints">Int Array to set the X points of the GraphicsPath</param>
	/// <param name="yPoints">Int Array to set the Y points of the GraphicsPath</param>
	/// <param name="pointsNumber">Number of points to add to the GraphicsPath</param>
	/// <returns>A new GraphicsPath</returns>
	public static System.Drawing.Drawing2D.GraphicsPath CreateGraphicsPath(int[] xPoints, int[] yPoints, int pointsNumber)
	{
		System.Drawing.Drawing2D.GraphicsPath tempGraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
		if (pointsNumber == 2)
			tempGraphicsPath.AddLine(xPoints[0], yPoints[0], xPoints[1], yPoints[1]);
		else
		{
			System.Drawing.Point[] tempPointArray = new System.Drawing.Point[pointsNumber];
			for (int index = 0; index < pointsNumber; index++)
				tempPointArray[index] = new System.Drawing.Point(xPoints[index], yPoints[index]);

			tempGraphicsPath.AddPolygon(tempPointArray);
		}
		return tempGraphicsPath;
	}

	/*******************************/
	/// <summary>
	/// Give functions to obtain information of graphic elements
	/// </summary>
	public class GraphicsManager
	{
		//Instance of GDI+ drawing surfaces graphics hashtable
		static public GraphicsHashTable manager = new GraphicsHashTable();

		/// <summary>
		/// Creates a new Graphics object from the device context handle associated with the Graphics
		/// parameter
		/// </summary>
		/// <param name="oldGraphics">Graphics instance to obtain the parameter from</param>
		/// <returns>A new GDI+ drawing surface</returns>
		public static System.Drawing.Graphics CreateGraphics(System.Drawing.Graphics oldGraphics)
		{
			System.Drawing.Graphics createdGraphics;
			System.IntPtr hdc = oldGraphics.GetHdc();
			createdGraphics = System.Drawing.Graphics.FromHdc(hdc);
			oldGraphics.ReleaseHdc(hdc);
			return createdGraphics;
		}

		/// <summary>
		/// This method draws a Bezier curve.
		/// </summary>
		/// <param name="graphics">It receives the Graphics instance</param>
		/// <param name="array">An array of (x,y) pairs of coordinates used to draw the curve.</param>
		public static void Bezier(System.Drawing.Graphics graphics, int[] array)
		{
			System.Drawing.Pen pen;
			pen = GraphicsManager.manager.GetPen(graphics);
			try
			{
				graphics.DrawBezier(pen, array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7]);
			}
			catch(System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException(e.ToString());
			}
		}

		/// <summary>
		/// Gets the text size width and height from a given GDI+ drawing surface and a given font
		/// </summary>
		/// <param name="graphics">Drawing surface to use</param>
		/// <param name="graphicsFont">Font type to measure</param>
		/// <param name="text">String of text to measure</param>
		/// <returns>A point structure with both size dimentions; x for width and y for height</returns>
		public static System.Drawing.Point GetTextSize(System.Drawing.Graphics graphics, System.Drawing.Font graphicsFont, System.String text)
		{
			System.Drawing.Point textSize;
			System.Drawing.SizeF tempSizeF;
			tempSizeF = graphics.MeasureString(text, graphicsFont);
			textSize = new System.Drawing.Point();
			textSize.X = (int) tempSizeF.Width;
			textSize.Y = (int) tempSizeF.Height;
			return textSize;
		}

		/// <summary>
		/// Gets the text size width and height from a given GDI+ drawing surface and a given font
		/// </summary>
		/// <param name="graphics">Drawing surface to use</param>
		/// <param name="graphicsFont">Font type to measure</param>
		/// <param name="text">String of text to measure</param>
		/// <param name="width">Maximum width of the string</param>
		/// <param name="format">StringFormat object that represents formatting information, such as line spacing, for the string</param>
		/// <returns>A point structure with both size dimentions; x for width and y for height</returns>
		public static System.Drawing.Point GetTextSize(System.Drawing.Graphics graphics, System.Drawing.Font graphicsFont, System.String text, System.Int32 width, System.Drawing.StringFormat format)
		{
			System.Drawing.Point textSize;
			System.Drawing.SizeF tempSizeF;
			tempSizeF = graphics.MeasureString(text, graphicsFont, width, format);
			textSize = new System.Drawing.Point();
			textSize.X = (int) tempSizeF.Width;
			textSize.Y = (int) tempSizeF.Height;
			return textSize;
		}

		/// <summary>
		/// Gives functionality over a hashtable of GDI+ drawing surfaces
		/// </summary>
		public class GraphicsHashTable:System.Collections.Hashtable 
		{
			/// <summary>
			/// Gets the graphics object from the given control
			/// </summary>
			/// <param name="control">Control to obtain the graphics from</param>
			/// <returns>A graphics object with the control's characteristics</returns>
			public System.Drawing.Graphics GetGraphics(System.Windows.Forms.Control control)
			{
				System.Drawing.Graphics graphic;
				if (control.Visible == true)
				{
					graphic = control.CreateGraphics();
					SetColor(graphic, control.ForeColor);
					SetFont(graphic, control.Font);
				}
				else
				{
					graphic = null;
				}
				return graphic;
			}

			/// <summary>
			/// Sets the background color property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given background color.
			/// </summary>
			/// <param name="graphic">Graphic element to search or add</param>
			/// <param name="color">Background color to set</param>
			public void SetBackColor(System.Drawing.Graphics graphic, System.Drawing.Color color)
			{
				if (this[graphic] != null)
					((GraphicsProperties) this[graphic]).BackColor = color;
				else
				{
					GraphicsProperties tempProps = new GraphicsProperties();
					tempProps.BackColor = color;
					Add(graphic, tempProps);
				}
			}

			/// <summary>
			/// Gets the background color property to the given graphics object in the hashtable. If the element doesn't exist, then it returns White.
			/// </summary>
			/// <param name="graphic">Graphic element to search</param>
			/// <returns>The background color of the graphic</returns>
			public System.Drawing.Color GetBackColor(System.Drawing.Graphics graphic)
			{
				if (this[graphic] == null)
					return System.Drawing.Color.White;
				else
					return ((GraphicsProperties) this[graphic]).BackColor;
			}

			/// <summary>
			/// Sets the text color property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given text color.
			/// </summary>
			/// <param name="graphic">Graphic element to search or add</param>
			/// <param name="color">Text color to set</param>
			public void SetTextColor(System.Drawing.Graphics graphic, System.Drawing.Color color)
			{
				if (this[graphic] != null)
					((GraphicsProperties) this[graphic]).TextColor = color;
				else
				{
					GraphicsProperties tempProps = new GraphicsProperties();
					tempProps.TextColor = color;
					Add(graphic, tempProps);
				}
			}

			/// <summary>
			/// Gets the text color property to the given graphics object in the hashtable. If the element doesn't exist, then it returns White.
			/// </summary>
			/// <param name="graphic">Graphic element to search</param>
			/// <returns>The text color of the graphic</returns>
			public System.Drawing.Color GetTextColor(System.Drawing.Graphics graphic) 
			{
				if (this[graphic] == null)
					return System.Drawing.Color.White;
				else
					return ((GraphicsProperties) this[graphic]).TextColor;
			}

			/// <summary>
			/// Sets the GraphicBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given GraphicBrush.
			/// </summary>
			/// <param name="graphic">Graphic element to search or add</param>
			/// <param name="brush">GraphicBrush to set</param>
			public void SetBrush(System.Drawing.Graphics graphic, System.Drawing.SolidBrush brush) 
			{
				if (this[graphic] != null)
					((GraphicsProperties) this[graphic]).GraphicBrush = brush;
				else
				{
					GraphicsProperties tempProps = new GraphicsProperties();
					tempProps.GraphicBrush = brush;
					Add(graphic, tempProps);
				}
			}
			
			/// <summary>
			/// Sets the GraphicBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given GraphicBrush.
			/// </summary>
			/// <param name="graphic">Graphic element to search or add</param>
			/// <param name="brush">GraphicBrush to set</param>
			public void SetPaint(System.Drawing.Graphics graphic, System.Drawing.Brush brush) 
			{
				if (this[graphic] != null)
					((GraphicsProperties) this[graphic]).PaintBrush = brush;
				else
				{
					GraphicsProperties tempProps = new GraphicsProperties();
					tempProps.PaintBrush = brush;
					Add(graphic, tempProps);
				}
			}
			
			/// <summary>
			/// Sets the GraphicBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given GraphicBrush.
			/// </summary>
			/// <param name="graphic">Graphic element to search or add</param>
			/// <param name="color">Color to set</param>
			public void SetPaint(System.Drawing.Graphics graphic, System.Drawing.Color color) 
			{
				System.Drawing.Brush brush = new System.Drawing.SolidBrush(color);
				if (this[graphic] != null)
					((GraphicsProperties) this[graphic]).PaintBrush = brush;
				else
				{
					GraphicsProperties tempProps = new GraphicsProperties();
					tempProps.PaintBrush = brush;
					Add(graphic, tempProps);
				}
			}


			/// <summary>
			/// Gets the HatchBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Blank.
			/// </summary>
			/// <param name="graphic">Graphic element to search</param>
			/// <returns>The HatchBrush setting of the graphic</returns>
			public System.Drawing.Drawing2D.HatchBrush GetBrush(System.Drawing.Graphics graphic)
			{
				if (this[graphic] == null)
					return new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Plaid,System.Drawing.Color.Black,System.Drawing.Color.Black);
				else
					return new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Plaid,((GraphicsProperties) this[graphic]).GraphicBrush.Color,((GraphicsProperties) this[graphic]).GraphicBrush.Color);
			}
			
			/// <summary>
			/// Gets the HatchBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Blank.
			/// </summary>
			/// <param name="graphic">Graphic element to search</param>
			/// <returns>The Brush setting of the graphic</returns>
			public System.Drawing.Brush GetPaint(System.Drawing.Graphics graphic)
			{
				if (this[graphic] == null)
					return new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Plaid,System.Drawing.Color.Black,System.Drawing.Color.Black);
				else
					return ((GraphicsProperties) this[graphic]).PaintBrush;
			}

			/// <summary>
			/// Sets the GraphicPen property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given Pen.
			/// </summary>
			/// <param name="graphic">Graphic element to search or add</param>
			/// <param name="pen">Pen to set</param>
			public void SetPen(System.Drawing.Graphics graphic, System.Drawing.Pen pen) 
			{
				if (this[graphic] != null)
					((GraphicsProperties) this[graphic]).GraphicPen = pen;
				else
				{
					GraphicsProperties tempProps = new GraphicsProperties();
					tempProps.GraphicPen = pen;
					Add(graphic, tempProps);
				}
			}

			/// <summary>
			/// Gets the GraphicPen property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Black.
			/// </summary>
			/// <param name="graphic">Graphic element to search</param>
			/// <returns>The GraphicPen setting of the graphic</returns>
			public System.Drawing.Pen GetPen(System.Drawing.Graphics graphic)
			{
				if (this[graphic] == null)
					return System.Drawing.Pens.Black;
				else
					return ((GraphicsProperties) this[graphic]).GraphicPen;
			}

			/// <summary>
			/// Sets the GraphicFont property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given Font.
			/// </summary>
			/// <param name="graphic">Graphic element to search or add</param>
			/// <param name="Font">Font to set</param>
			public void SetFont(System.Drawing.Graphics graphic, System.Drawing.Font font) 
			{
				if (this[graphic] != null)
					((GraphicsProperties) this[graphic]).GraphicFont = font;
				else
				{
					GraphicsProperties tempProps = new GraphicsProperties();
					tempProps.GraphicFont = font;
					Add(graphic,tempProps);
				}
			}

			/// <summary>
			/// Gets the GraphicFont property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Microsoft Sans Serif with size 8.25.
			/// </summary>
			/// <param name="graphic">Graphic element to search</param>
			/// <returns>The GraphicFont setting of the graphic</returns>
			public System.Drawing.Font GetFont(System.Drawing.Graphics graphic)
			{
				if (this[graphic] == null)
					return new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
				else
					return ((GraphicsProperties) this[graphic]).GraphicFont;
			}

			/// <summary>
			/// Sets the color properties for a given Graphics object. If the element doesn't exist, then it adds the graphic element to the hashtable with the color properties set with the given value.
			/// </summary>
			/// <param name="graphic">Graphic element to search or add</param>
			/// <param name="color">Color value to set</param>
			public void SetColor(System.Drawing.Graphics graphic, System.Drawing.Color color) 
			{
				if (this[graphic] != null)
				{
					((GraphicsProperties) this[graphic]).GraphicPen.Color = color;
					((GraphicsProperties) this[graphic]).GraphicBrush.Color = color;
					((GraphicsProperties) this[graphic]).color = color;
				}
				else
				{
					GraphicsProperties tempProps = new GraphicsProperties();
					tempProps.GraphicPen.Color = color;
					tempProps.GraphicBrush.Color = color;
					tempProps.color = color;
					Add(graphic,tempProps);
				}
			}

			/// <summary>
			/// Gets the color property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Black.
			/// </summary>
			/// <param name="graphic">Graphic element to search</param>
			/// <returns>The color setting of the graphic</returns>
			public System.Drawing.Color GetColor(System.Drawing.Graphics graphic) 
			{
				if (this[graphic] == null)
					return System.Drawing.Color.Black;
				else
					return ((GraphicsProperties) this[graphic]).color;
			}

			/// <summary>
			/// This method gets the TextBackgroundColor of a Graphics instance
			/// </summary>
			/// <param name="graphic">The graphics instance</param>
			/// <returns>The color value in ARGB encoding</returns>
			public System.Drawing.Color GetTextBackgroundColor(System.Drawing.Graphics graphic)
			{
				if (this[graphic] == null)
					return System.Drawing.Color.Black;
				else 
				{ 
					return ((GraphicsProperties) this[graphic]).TextBackgroundColor;
				}
			}

			/// <summary>
			/// This method set the TextBackgroundColor of a Graphics instace
			/// </summary>
			/// <param name="graphic">The graphics instace</param>
			/// <param name="color">The System.Color to set the TextBackgroundColor</param>
			public void SetTextBackgroundColor(System.Drawing.Graphics graphic, System.Drawing.Color color) 
			{
				if (this[graphic] != null)
				{
					((GraphicsProperties) this[graphic]).TextBackgroundColor = color;								
				}
				else
				{
					GraphicsProperties tempProps = new GraphicsProperties();
					tempProps.TextBackgroundColor = color;				
					Add(graphic,tempProps);
				}
			}

			/// <summary>
			/// Structure to store properties from System.Drawing.Graphics objects
			/// </summary>
			class GraphicsProperties
			{
				public System.Drawing.Color TextBackgroundColor = System.Drawing.Color.Black;
				public System.Drawing.Color color = System.Drawing.Color.Black;
				public System.Drawing.Color BackColor = System.Drawing.Color.White;
				public System.Drawing.Color TextColor = System.Drawing.Color.Black;
				public System.Drawing.SolidBrush GraphicBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
				public System.Drawing.Brush PaintBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
				public System.Drawing.Pen   GraphicPen = new System.Drawing.Pen(System.Drawing.Color.Black);
				public System.Drawing.Font  GraphicFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			}
		}
	}

	/*******************************/
	/// <summary>
	/// Contains methods to get an set a ToolTip
	/// </summary>
	public class ToolTipSupport
	{
		static System.Windows.Forms.ToolTip supportToolTip = new System.Windows.Forms.ToolTip();

		/// <summary>
		/// Get the ToolTip text for the specific control parameter.
		/// </summary>
		/// <param name="control">The control with the ToolTip</param>
		/// <returns>The ToolTip Text</returns>
		public static System.String getToolTipText(System.Windows.Forms.Control control)
		{
			return(supportToolTip.GetToolTip(control));
		}
		 
		/// <summary>
		/// Set the ToolTip text for the specific control parameter.
		/// </summary>
		/// <param name="control">The control to set the ToolTip</param>
		/// <param name="text">The text to show on the ToolTip</param>
		public static void setToolTipText(System.Windows.Forms.Control control, System.String text)
		{
			supportToolTip.SetToolTip(control,text);
		}
	}

	/*******************************/
	/// <summary>
	/// This class provides some useful methods for calculate operations with components.
	/// </summary>
	public class SwingUtilsSupport
	{
		/// <summary>
		/// Calculates the intersection between two rectangles.
		/// </summary>
		/// <param name="X">The X coordinate of the first rectangle.</param>
		/// <param name="Y">The Y coordinate of the first rectangle.</param>
		/// <param name="width">The width of the first rectangle.</param>
		/// <param name="height">The height of the first rectangle.</param>
		/// <param name="rectangle">The second rectangle used to make the intersection.</param>
		/// <returns>The Rectangle results from the intersection operation.</returns>
		public static System.Drawing.Rectangle ComputeIntersection(int X, int Y, int width, int height, System.Drawing.Rectangle rectangle)
		{
			return System.Drawing.Rectangle.Intersect(new System.Drawing.Rectangle(X, Y, width, height), rectangle);
		}

		/// <summary>
		/// Calculates the union of two rectangles.
		/// </summary>
		/// <param name="X">The X coordinate of the first rectangle.</param>
		/// <param name="Y">The Y coordinate of the first rectangle.</param>
		/// <param name="width">The width of the first rectangle.</param>
		/// <param name="height">The height of the first rectangle.</param>
		/// <param name="rectangle">The second rectangle used to make the union.</param>
		/// <returns>The Rectangle results from the union operation.</returns>
		public static System.Drawing.Rectangle ComputeUnion(int X, int Y, int width, int height, System.Drawing.Rectangle rectangle)
		{
			return System.Drawing.Rectangle.Union(new System.Drawing.Rectangle(X, Y, width, height), rectangle);
		}

		/// <summary>
		/// Takes the Point coordinate from the screen and translate into component's coordinate.
		/// </summary>
		/// <param name="pointSource">The Point that represents the coordinates from the screen
		/// and will be translated to component's coordinates.</param>
		/// <param name="component">The component used to calculate the new coordinates of the Point.</param>
		public static void PointFromScreen(ref System.Drawing.Point pointSource, System.Windows.Forms.Control component)
		{			
			pointSource.X = (((System.Windows.Forms.Control)component).PointToClient(pointSource)).X;
			pointSource.Y = (((System.Windows.Forms.Control)component).PointToClient(pointSource)).Y;			
		}
		
		/// <summary>
		/// Takes the Point coordinate from the component and translates it into screen's coordinates.
		/// </summary>
		/// <param name="pointSource">The Point that represents the coordinates from the component
		/// and will be translated to screen's coordinates.</param>
		/// <param name="component">The component used to calculate the new coordinates of the point.</param>
		public static void PointToScreen(ref System.Drawing.Point pointSource, System.Windows.Forms.Control component)
		{
			pointSource.X = (((System.Windows.Forms.Control)component).PointToScreen(pointSource)).X;
			pointSource.Y = (((System.Windows.Forms.Control)component).PointToScreen(pointSource)).Y;
		}

		/// <summary>
		/// Calculates if the first rectangle contains the second one.
		/// </summary>
		/// <param name="rectangle1">The first rectangle.</param>
		/// <param name="rectangle2">The second rectangle.</param>
		/// <returns>True if the first rectangle contains the second, otherwise false.</returns>
		public static bool RectangleContains(System.Drawing.Rectangle rectangle1, System.Drawing.Rectangle rectangle2)
		{
			return (((rectangle1.X + rectangle1.Width) >= (rectangle2.X + rectangle2.Width)) &&
					((rectangle1.Y + rectangle1.Height) >= (rectangle2.Y + rectangle2.Height)));
		}

		/// <summary>
		/// Calculates if the MouseEvent was fired by the left mouse button.
		/// </summary>
		/// <param name="mouseEvent">The MouseEvent of origin.</param>
		/// <returns>True if the the MouseEvent was generated by the left mouse button, otherwise false.</returns>
		public static bool IsMouseLeft(System.Windows.Forms.MouseEventArgs mouseEvent)
		{
			return (mouseEvent.Button == System.Windows.Forms.MouseButtons.Left);
		}

		/// <summary>
		/// Calculates if the MouseEvent was fired by the right mouse button.
		/// </summary>
		/// <param name="mouseEvent">The MouseEvent of origin.</param>
		/// <returns>True if the the MouseEvent was generated by the right mouse button, otherwise false.</returns>
		public static bool IsMouseRight(System.Windows.Forms.MouseEventArgs mouseEvent)
		{
			return (mouseEvent.Button == System.Windows.Forms.MouseButtons.Right);
		}

		/// <summary>
		/// Calculates if the MouseEvent was fired by the middle mouse button.
		/// </summary>
		/// <param name="mouseEvent">The MouseEvent of origin.</param>
		/// <returns>True if the the MouseEvent was generated by the middle mouse button, otherwise false.</returns>
		public static bool IsMouseMiddle(System.Windows.Forms.MouseEventArgs mouseEvent)
		{
			return (mouseEvent.Button == System.Windows.Forms.MouseButtons.Middle);
		}
	}


	/*******************************/
	/// <summary>
	/// Method used to obtain the underlying type of an object to make the correct property call.
	/// The method is used when setting values to a property.
	/// </summary>
	/// <param name="tempObject">Object instance received.</param>
	/// <param name="propertyName">Property name to work on.</param>
	/// <param name="newValue">Object containing the value to assing.</param>
	/// <returns>The return value of the property assignment.</returns>
	public static System.Object SetPropertyAsVirtual(System.Object tempObject, System.String propertyName, System.Object newValue)
	{
		System.Type type = tempObject.GetType();
		System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);
		propertyInfo.SetValue(tempObject, newValue, null);
		try
		{
			return propertyInfo.GetValue(tempObject, null);
		}
		catch(Exception e)
		{
			throw e.InnerException;
		}
	}


	/*******************************/
	/// <summary>
	/// Method used to obtain the underlying type of an object to make the correct property call.
	/// The method is used when getting values from a property.
	/// </summary>
	/// <param name="tempObject">Object instance received</param>
	/// <param name="propertyName">Property name to obtain value</param>
	/// <returns>The return value of the property</returns>
	public static System.Object GetPropertyAsVirtual(System.Object tempObject, System.String propertyName)
	{
		System.Type type = tempObject.GetType();
		System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);
		try
		{
			return propertyInfo.GetValue(tempObject, null);
		}
		catch(Exception e)
		{
			throw e.InnerException;
		}
	}


	/*******************************/
/// <summary>
/// Contains methods to construct customized Buttons
/// </summary>
public class ButtonSupport
{
	/// <summary>
	/// Creates a popup style Button with an specific text.	
	/// </summary>
	/// <param name="label">The text associated with the Button</param>
	/// <returns>The new Button</returns>
	public static System.Windows.Forms.Button CreateButton(System.String label)
	{			
		System.Windows.Forms.Button tempButton = new System.Windows.Forms.Button();
		tempButton.Text = label;
		tempButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
		return tempButton;
	}

	/// <summary>
	/// Sets the an specific text for the Button
	/// </summary>
	/// <param name="Button">The button to be set</param>
	/// <param name="label">The text associated with the Button</param>
	public static void SetButton(System.Windows.Forms.ButtonBase Button, System.String label)
	{
		Button.Text = label;
		Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
	}

	/// <summary>
	/// Creates a Button with an specific text and style.
	/// </summary>
	/// <param name="label">The text associated with the Button</param>
	/// <param name="style">The style of the Button</param>
	/// <returns>The new Button</returns>
	public static System.Windows.Forms.Button CreateButton(System.String label, int style)
	{
		System.Windows.Forms.Button tempButton = new System.Windows.Forms.Button();
		tempButton.Text = label;
		setStyle(tempButton,style);
		return tempButton;
	}

	/// <summary>
	/// Sets the specific Text and Style for the Button
	/// </summary>
	/// <param name="Button">The button to be set</param>
	/// <param name="label">The text associated with the Button</param>
	/// <param name="style">The style of the Button</param>
	public static void SetButton(System.Windows.Forms.ButtonBase Button, System.String label, int style)
	{
		Button.Text = label;
		setStyle(Button,style);
	}

	/// <summary>
	/// Creates a standard style Button that contains an specific text and/or image
	/// </summary>
	/// <param name="control">The control to be contained analized to get the text and/or image for the Button</param>
	/// <returns>The new Button</returns>
	public static System.Windows.Forms.Button CreateButton(System.Windows.Forms.Control control)
	{
		System.Windows.Forms.Button tempButton = new System.Windows.Forms.Button();
		if(control.GetType().FullName == "System.Windows.Forms.Label")
		{
			tempButton.Image = ((System.Windows.Forms.Label)control).Image;
			tempButton.Text = ((System.Windows.Forms.Label)control).Text;
			tempButton.ImageAlign = ((System.Windows.Forms.Label)control).ImageAlign;
			tempButton.TextAlign = ((System.Windows.Forms.Label)control).TextAlign;
		}
		else
		{
			if(control.GetType().FullName == "System.Windows.Forms.PictureBox")//Tentative to see maps of UIGraphic
			{
				tempButton.Image = ((System.Windows.Forms.PictureBox)control).Image;
				tempButton.ImageAlign = ((System.Windows.Forms.Label)control).ImageAlign;
			}else
				tempButton.Text = control.Text;
		}
		tempButton.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
		return tempButton;
	}

	/// <summary>
	/// Sets an specific text and/or image to the Button
	/// </summary>
	/// <param name="Button">The button to be set</param>
	/// <param name="control">The control to be contained analized to get the text and/or image for the Button</param>
	public static void SetButton(System.Windows.Forms.ButtonBase Button,System.Windows.Forms.Control control)
	{
		if(control.GetType().FullName == "System.Windows.Forms.Label")
		{
			Button.Image = ((System.Windows.Forms.Label)control).Image;
			Button.Text = ((System.Windows.Forms.Label)control).Text;
			Button.ImageAlign = ((System.Windows.Forms.Label)control).ImageAlign;
			Button.TextAlign = ((System.Windows.Forms.Label)control).TextAlign;
		}
		else
		{
			if(control.GetType().FullName == "System.Windows.Forms.PictureBox")//Tentative to see maps of UIGraphic
			{
				Button.Image = ((System.Windows.Forms.PictureBox)control).Image;
				Button.ImageAlign = ((System.Windows.Forms.Label)control).ImageAlign;
			}
			else
				Button.Text = control.Text;
		}
		Button.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
	}

	/// <summary>
	/// Creates a Button with an specific control and style
	/// </summary>
	/// <param name="control">The control to be contained by the button</param>
	/// <param name="style">The style of the button</param>
	/// <returns>The new Button</returns>
	public static System.Windows.Forms.Button CreateButton(System.Windows.Forms.Control control, int style)
	{
		System.Windows.Forms.Button tempButton = CreateButton(control);
		setStyle(tempButton,style);
		return tempButton;
	}

	/// <summary>
	/// Sets an specific text and/or image to the Button
	/// </summary>
	/// <param name="Button">The button to be set</param>
	/// <param name="control">The control to be contained by the button</param>
	/// <param name="style">The style of the button</param>
	public static void SetButton(System.Windows.Forms.ButtonBase Button,System.Windows.Forms.Control control,int style)
	{
		SetButton(Button,control);
		setStyle(Button,style);
	}

	/// <summary>
	/// Sets the style of the Button
	/// </summary>
	/// <param name="Button">The Button that will change its style</param>
	/// <param name="style">The new style of the Button</param>
	/// <remarks> 
	/// If style is 0 then sets a popup style to the Button, otherwise sets a standard style to the Button.
	/// </remarks>
	public static void setStyle(System.Windows.Forms.ButtonBase Button, int style)
	{
		if (  (style == 0 ) || (style ==  67108864) || (style ==  33554432) ) 
			Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
		else if ( (style == 2097152) || (style == 1048576) ||  (style == 16777216 ) )
				Button.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
		else 
			throw new System.ArgumentException("illegal style: " + style);		
	}

	/// <summary>
	/// Selects the Button
	/// </summary>
	/// <param name="Button">The Button that will change its style</param>
	/// <param name="select">It determines if the button woll be selected</param>
	/// <remarks> 
	/// If select is true thebutton will be selected, otherwise not.
	/// </remarks>
	public static void setSelected(System.Windows.Forms.ButtonBase Button, bool select)
	{
		if (select)
			Button.Select();
	}

	/// <summary>
	/// Receives a Button instance and sets the Text and Image properties.
	/// </summary>
	/// <param name="buttonInstance">Button instance to be set.</param>
	/// <param name="buttonText">Value to be set to Text.</param>
	/// <param name="icon">Value to be set to Image.</param>
	public static void SetStandardButton (System.Windows.Forms.ButtonBase buttonInstance, System.String buttonText , System.Drawing.Image icon )
	{
		buttonInstance.Text = buttonText;
		buttonInstance.Image = icon;
	}

	/// <summary>
	/// Creates a Button with a given text.
	/// </summary>
	/// <param name="buttonText">The text to be displayed in the button.</param>
	/// <returns>The new created button with text</returns>
	public static System.Windows.Forms.Button CreateStandardButton (System.String buttonText)
	{
		System.Windows.Forms.Button newButton = new System.Windows.Forms.Button();
		newButton.Text = buttonText;
		return newButton;
	}

	/// <summary>
	/// Creates a Button with a given image.
	/// </summary>
	/// <param name="buttonImage">The image to be displayed in the button.</param>
	/// <returns>The new created button with an image</returns>
	public static System.Windows.Forms.Button CreateStandardButton (System.Drawing.Image buttonImage)
	{
		System.Windows.Forms.Button newButton = new System.Windows.Forms.Button();
		newButton.Image = buttonImage;
		return newButton;
	}

	/// <summary>
	/// Creates a Button with a given image and a text.
	/// </summary>
	/// <param name="buttonText">The text to be displayed in the button.</param>
	/// <param name="buttonImage">The image to be displayed in the button.</param>
	/// <returns>The new created button with text and image</returns>
	public static System.Windows.Forms.Button CreateStandardButton (System.String buttonText, System.Drawing.Image buttonImage)
	{
		System.Windows.Forms.Button newButton = new System.Windows.Forms.Button();
		newButton.Text = buttonText;
		newButton.Image = buttonImage;
		return newButton;
	}
}
	/*******************************/
	/// <summary>
	/// Support class for creation of Forms behaving like Dialog windows.
	/// </summary>
	public class DialogSupport
	{
		/// <summary>
		/// Creates a dialog Form.
		/// </summary>
		/// <returns>The new dialog Form instance.</returns>
		public static System.Windows.Forms.Form CreateDialog()
		{
			System.Windows.Forms.Form tempForm = new System.Windows.Forms.Form();
			tempForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			tempForm.ShowInTaskbar = false;
			return tempForm;
		}

		/// <summary>
		/// Sets dialog like properties to a Form.
		/// </summary>
		/// <param name="formInstance">Form instance to be modified.</param>
		public static void SetDialog(System.Windows.Forms.Form formInstance)
		{
			formInstance.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			formInstance.ShowInTaskbar = false;
		}

		/// <summary>
		/// Creates a dialog Form with an owner.
		/// </summary>
		/// <param name="owner">The form to be set as owner of the newly created one.</param>
		/// <returns>A new dialog Form.</returns>
		public static System.Windows.Forms.Form CreateDialog(System.Windows.Forms.Form owner)
		{
			System.Windows.Forms.Form tempForm = new System.Windows.Forms.Form();
			tempForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			tempForm.ShowInTaskbar = false;
			tempForm.Owner = owner;
			return tempForm;
		}

		/// <summary>
		/// Sets dialog like properties and an owner to a Form.
		/// </summary>
		/// <param name="formInstance">Form instance to be modified.</param>
		/// <param name="owner">The form to be set as owner of the newly created one.</param>
		public static void SetDialog(System.Windows.Forms.Form formInstance, System.Windows.Forms.Form owner)
		{
			formInstance.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			formInstance.ShowInTaskbar = false;
			formInstance.Owner = owner;
		}

		
		/// <summary>
		/// Creates a dialog Form with an owner and a text property.
		/// </summary>
		/// <param name="owner">The form to be set as owner of the newly created one.</param>
		/// <param name="text">The title text for the form.</param>
		/// <returns>The new dialog Form.</returns>
		public static System.Windows.Forms.Form CreateDialog(System.Windows.Forms.Form owner, System.String text)
		{
			System.Windows.Forms.Form tempForm = new System.Windows.Forms.Form();
			tempForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			tempForm.ShowInTaskbar = false;
			tempForm.Owner = owner;
			tempForm.Text = text;
			return tempForm;
		}
				
		/// <summary>
		/// Sets dialog like properties, an owner and a text property to a Form.
		/// </summary>
		/// <param name="formInstance">Form instance to be modified.</param>
		/// <param name="owner">The form to be set as owner of the newly created one.</param>
		/// <param name="text">The title text for the form.</param>
		public static void SetDialog(System.Windows.Forms.Form formInstance, System.Windows.Forms.Form owner, System.String text)
		{
			formInstance.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			formInstance.ShowInTaskbar = false;
			formInstance.Owner = owner;
			formInstance.Text = text;
		}

			
		/// <summary>
		/// This method sets or unsets a resizable border style to a Form.
		/// </summary>
		/// <param name="formInstance">The form to be modified.</param>
		/// <param name="sizable">Boolean value to be set.</param>
		public static void SetSizable(System.Windows.Forms.Form formInstance, bool sizable)
		{
			if (sizable)
			{
				formInstance.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			}
			else
			{
				formInstance.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			}
		}
	}


	/*******************************/
	/// <summary>
	/// Recieves a form and an integer value representing the operation to perform when the closing 
	/// event is fired.
	/// </summary>
	/// <param name="form">The form that fire the event.</param>
	/// <param name="operation">The operation to do while the form is closing.</param>
	public static void CloseOperation(System.Windows.Forms.Form form, int operation)
	{
		switch (operation)
		{
			case 0:
				break;
			case 1:
				form.Hide();
				break;
			case 2:
				form.Dispose();
				break;
			case 3:
				form.Dispose();
				System.Windows.Forms.Application.Exit();
				break;
		}
	}


	/*******************************/
	/// <summary>
	/// This class has static methods to manage collections.
	/// </summary>
	public class CollectionsSupport
	{
		/// <summary>
		/// Copies the IList to other IList.
		/// </summary>
		/// <param name="SourceList">IList source.</param>
		/// <param name="TargetList">IList target.</param>
		public static void Copy(System.Collections.IList SourceList, System.Collections.IList TargetList)
		{
			for (int i = 0; i < SourceList.Count; i++)
				TargetList[i] = SourceList[i];
		}

		/// <summary>
		/// Replaces the elements of the specified list with the specified element.
		/// </summary>
		/// <param name="List">The list to be filled with the specified element.</param>
		/// <param name="Element">The element with which to fill the specified list.</param>
		public static void Fill(System.Collections.IList List, System.Object Element)
		{
			for(int i = 0; i < List.Count; i++)
				List[i] = Element;      
		}

		/// <summary>
		/// This class implements System.Collections.IComparer and is used for Comparing two String objects by evaluating 
		/// the numeric values of the corresponding Char objects in each string.
		/// </summary>
		class CompareCharValues : System.Collections.IComparer
		{
			public int Compare(System.Object x, System.Object y)
			{
				return System.String.CompareOrdinal((System.String)x, (System.String)y);
			}
		}

		/// <summary>
		/// Obtain the maximum element of the given collection with the specified comparator.
		/// </summary>
		/// <param name="Collection">Collection from which the maximum value will be obtained.</param>
		/// <param name="Comparator">The comparator with which to determine the maximum element.</param>
		/// <returns></returns>
		public static System.Object Max(System.Collections.ICollection Collection, System.Collections.IComparer Comparator)
		{
			System.Collections.ArrayList tempArrayList;

			if (((System.Collections.ArrayList)Collection).IsReadOnly)		
				throw new System.NotSupportedException();

			if ((Comparator == null) || (Comparator is System.Collections.Comparer)) 
			{
				try
				{	
					tempArrayList = new System.Collections.ArrayList(Collection);
					tempArrayList.Sort();
				} 
				catch(System.InvalidOperationException e) 
				{
					throw new System.InvalidCastException(e.Message);
				}			
				return (System.Object)tempArrayList[Collection.Count - 1];
			}
			else
			{
				try
				{				
					tempArrayList = new System.Collections.ArrayList(Collection);
					tempArrayList.Sort(Comparator);
				} 
				catch(System.InvalidOperationException e) 
				{
					throw new System.InvalidCastException(e.Message);
				}			
				return (System.Object)tempArrayList[Collection.Count - 1];
			}
		}
		
		/// <summary>
		/// Obtain the minimum element of the given collection with the specified comparator.
		/// </summary>
		/// <param name="Collection">Collection from which the minimum value will be obtained.</param>
		/// <param name="Comparator">The comparator with which to determine the minimum element.</param>
		/// <returns></returns>
		public static System.Object Min(System.Collections.ICollection Collection, System.Collections.IComparer Comparator)
		{
			System.Collections.ArrayList tempArrayList;

			if (((System.Collections.ArrayList)Collection).IsReadOnly)		
				throw new System.NotSupportedException();

			if ((Comparator == null) || (Comparator is System.Collections.Comparer)) 
			{
				try 	
				{				
					tempArrayList = new System.Collections.ArrayList(Collection);
					tempArrayList.Sort();
				} 
				catch(System.InvalidOperationException e) 
				{
					throw new System.InvalidCastException(e.Message);
				}			
				return (System.Object)tempArrayList[0];
			}
			else
			{			
				try 	
				{				
					tempArrayList = new System.Collections.ArrayList(Collection);
					tempArrayList.Sort(Comparator);
				} 
				catch(System.InvalidOperationException e) 
				{
					throw new System.InvalidCastException(e.Message);
				}			
				return (System.Object)tempArrayList[0];
			}
		}
		
		
		/// <summary>
		/// Sorts an IList collections
		/// </summary>
		/// <param name="list">The System.Collections.IList instance that will be sorted</param>
		/// <param name="Comparator">The Comparator criteria, null to use natural comparator.</param>
		public static void Sort(System.Collections.IList list, System.Collections.IComparer Comparator)
		{
			if (((System.Collections.ArrayList)list).IsReadOnly)		
				throw new System.NotSupportedException();

			if ((Comparator == null) || (Comparator is System.Collections.Comparer)) 
			{
				try 	
				{				
					((System.Collections.ArrayList)list).Sort();
				} 
				catch(System.InvalidOperationException e) 
				{
					throw new System.InvalidCastException(e.Message);
				}			
			}
			else
			{			
				try 	
				{				
					((System.Collections.ArrayList)list).Sort(Comparator);
				} 
				catch(System.InvalidOperationException e) 
				{
					throw new System.InvalidCastException(e.Message);
				}			
			}
		}

		/// <summary>
		/// Shuffles the list randomly.
		/// </summary>
		/// <param name="List">The list to be shuffled.</param>
		public static void Shuffle(System.Collections.IList List)
		{
			System.Random RandomList = new System.Random(unchecked((int)System.DateTime.Now.Ticks));
			Shuffle(List, RandomList);
		}

		/// <summary>
		/// Shuffles the list randomly.
		/// </summary>
		/// <param name="List">The list to be shuffled.</param>
		/// <param name="RandomList">The random to use to shuffle the list.</param>
		public static void Shuffle(System.Collections.IList List, System.Random RandomList)
		{
			System.Object source = null;
			int  target = 0;

			for (int i = 0; i < List.Count; i++)
			{
				target  = RandomList.Next(List.Count);
				source  = (System.Object)List[i];
				List[i] = List[target];
				List[target] = source;
			}
		}
	}


	/*******************************/
	/// <summary>
	/// Support class used to handle threads
	/// </summary>
	public class ThreadClass : IThreadRunnable
	{
		/// <summary>
		/// The instance of System.Threading.Thread
		/// </summary>
		private System.Threading.Thread threadField;
	      
		/// <summary>
		/// Initializes a new instance of the ThreadClass class
		/// </summary>
		public ThreadClass()
		{
			threadField = new System.Threading.Thread(new System.Threading.ThreadStart(Run));
		}
	 
		/// <summary>
		/// Initializes a new instance of the Thread class.
		/// </summary>
		/// <param name="Name">The name of the thread</param>
		public ThreadClass(System.String Name)
		{
			threadField = new System.Threading.Thread(new System.Threading.ThreadStart(Run));
			this.Name = Name;
		}
	      
		/// <summary>
		/// Initializes a new instance of the Thread class.
		/// </summary>
		/// <param name="Start">A ThreadStart delegate that references the methods to be invoked when this thread begins executing</param>
		public ThreadClass(System.Threading.ThreadStart Start)
		{
			threadField = new System.Threading.Thread(Start);
		}
	 
		/// <summary>
		/// Initializes a new instance of the Thread class.
		/// </summary>
		/// <param name="Start">A ThreadStart delegate that references the methods to be invoked when this thread begins executing</param>
		/// <param name="Name">The name of the thread</param>
		public ThreadClass(System.Threading.ThreadStart Start, System.String Name)
		{
			threadField = new System.Threading.Thread(Start);
			this.Name = Name;
		}
	      
		/// <summary>
		/// This method has no functionality unless the method is overridden
		/// </summary>
		public virtual void Run()
		{
		}
	      
		/// <summary>
		/// Causes the operating system to change the state of the current thread instance to ThreadState.Running
		/// </summary>
		public virtual void Start()
		{
			threadField.Start();
		}
	      
		/// <summary>
		/// Interrupts a thread that is in the WaitSleepJoin thread state
		/// </summary>
		public virtual void Interrupt()
		{
			threadField.Interrupt();
		}
	      
		/// <summary>
		/// Gets the current thread instance
		/// </summary>
		public System.Threading.Thread Instance
		{
			get
			{
				return threadField;
			}
			set
			{
				threadField = value;
			}
		}
	      
		/// <summary>
		/// Gets or sets the name of the thread
		/// </summary>
		public System.String Name
		{
			get
			{
				return threadField.Name;
			}
			set
			{
				if (threadField.Name == null)
					threadField.Name = value; 
			}
		}
	      
		/// <summary>
		/// Gets or sets a value indicating the scheduling priority of a thread
		/// </summary>
		public System.Threading.ThreadPriority Priority
		{
			get
			{
				return threadField.Priority;
			}
			set
			{
				threadField.Priority = value;
			}
		}
	      
		/// <summary>
		/// Gets a value indicating the execution status of the current thread
		/// </summary>
		public bool IsAlive
		{
			get
			{
				return threadField.IsAlive;
			}
		}
	      
		/// <summary>
		/// Gets or sets a value indicating whether or not a thread is a background thread.
		/// </summary>
		public bool IsBackground
		{
			get
			{
				return threadField.IsBackground;
			} 
			set
			{
				threadField.IsBackground = value;
			}
		}
	      
		/// <summary>
		/// Blocks the calling thread until a thread terminates
		/// </summary>
		public void Join()
		{
			threadField.Join();
		}
	      
		/// <summary>
		/// Blocks the calling thread until a thread terminates or the specified time elapses
		/// </summary>
		/// <param name="MiliSeconds">Time of wait in milliseconds</param>
		public void Join(long MiliSeconds)
		{
			lock(this)
			{
				threadField.Join(new System.TimeSpan(MiliSeconds * 10000));
			}
		}
	      
		/// <summary>
		/// Blocks the calling thread until a thread terminates or the specified time elapses
		/// </summary>
		/// <param name="MiliSeconds">Time of wait in milliseconds</param>
		/// <param name="NanoSeconds">Time of wait in nanoseconds</param>
		public void Join(long MiliSeconds, int NanoSeconds)
		{
			lock(this)
			{
				threadField.Join(new System.TimeSpan(MiliSeconds * 10000 + NanoSeconds * 100));
			}
		}
	      
		/// <summary>
		/// Resumes a thread that has been suspended
		/// </summary>
		public void Resume()
		{
			threadField.Resume();
		}
	      
		/// <summary>
		/// Raises a ThreadAbortException in the thread on which it is invoked, 
		/// to begin the process of terminating the thread. Calling this method 
		/// usually terminates the thread
		/// </summary>
		public void Abort()
		{
			threadField.Abort();
		}
	      
		/// <summary>
		/// Raises a ThreadAbortException in the thread on which it is invoked, 
		/// to begin the process of terminating the thread while also providing
		/// exception information about the thread termination. 
		/// Calling this method usually terminates the thread.
		/// </summary>
		/// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted</param>
		public void Abort(System.Object stateInfo)
		{
			lock(this)
			{
				threadField.Abort(stateInfo);
			}
		}
	      
		/// <summary>
		/// Suspends the thread, if the thread is already suspended it has no effect
		/// </summary>
		public void Suspend()
		{
			threadField.Suspend();
		}
	      
		/// <summary>
		/// Obtain a String that represents the current Object
		/// </summary>
		/// <returns>A String that represents the current Object</returns>
		public override System.String ToString()
		{
			return "Thread[" + Name + "," + Priority.ToString() + "," + "" + "]";
		}
	     
		/// <summary>
		/// Gets the currently running thread
		/// </summary>
		/// <returns>The currently running thread</returns>
		public static ThreadClass Current()
		{
			ThreadClass CurrentThread = new ThreadClass();
			CurrentThread.Instance = System.Threading.Thread.CurrentThread;
			return CurrentThread;
		}
	}


	/*******************************/
	/// <summary>
	/// Converts an array of sbytes to an array of bytes
	/// </summary>
	/// <param name="sbyteArray">The array of sbytes to be converted</param>
	/// <returns>The new array of bytes</returns>
	public static byte[] ToByteArray(sbyte[] sbyteArray)
	{
		byte[] byteArray = null;

		if (sbyteArray != null)
		{
			byteArray = new byte[sbyteArray.Length];
			for(int index=0; index < sbyteArray.Length; index++)
				byteArray[index] = (byte) sbyteArray[index];
		}
		return byteArray;
	}

	/// <summary>
	/// Converts a string to an array of bytes
	/// </summary>
	/// <param name="sourceString">The string to be converted</param>
	/// <returns>The new array of bytes</returns>
	public static byte[] ToByteArray(System.String sourceString)
	{
		return System.Text.UTF8Encoding.UTF8.GetBytes(sourceString);
	}

	/// <summary>
	/// Converts a array of object-type instances to a byte-type array.
	/// </summary>
	/// <param name="tempObjectArray">Array to convert.</param>
	/// <returns>An array of byte type elements.</returns>
	public static byte[] ToByteArray(System.Object[] tempObjectArray)
	{
		byte[] byteArray = null;
		if (tempObjectArray != null)
		{
			byteArray = new byte[tempObjectArray.Length];
			for (int index = 0; index < tempObjectArray.Length; index++)
				byteArray[index] = (byte)tempObjectArray[index];
		}
		return byteArray;
	}

	/*******************************/
	/// <summary>
	/// Write an array of bytes int the FileStream specified.
	/// </summary>
	/// <param name="FileStreamWrite">FileStream that must be updated.</param>
	/// <param name="Source">Array of bytes that must be written in the FileStream.</param>
	public static void WriteOutput(System.IO.FileStream FileStreamWrite, sbyte[] Source)
	{
		FileStreamWrite.Write(ToByteArray(Source), 0, Source.Length);
	}


	/*******************************/
	/// <summary>
	/// Receives a byte array and returns it transformed in an sbyte array
	/// </summary>
	/// <param name="byteArray">Byte array to process</param>
	/// <returns>The transformed array</returns>
	public static sbyte[] ToSByteArray(byte[] byteArray)
	{
		sbyte[] sbyteArray = null;
		if (byteArray != null)
		{
			sbyteArray = new sbyte[byteArray.Length];
			for(int index=0; index < byteArray.Length; index++)
				sbyteArray[index] = (sbyte) byteArray[index];
		}
		return sbyteArray;
	}

	/*******************************/
	/// <summary>
	/// Checks if a file have write permissions
	/// </summary>
	/// <param name="file">The file instance to check</param>
	/// <returns>True if have write permissions otherwise false</returns>
public static bool FileCanWrite(System.IO.FileInfo file)
{
return (System.IO.File.GetAttributes(file.FullName) & System.IO.FileAttributes.ReadOnly) != System.IO.FileAttributes.ReadOnly;
}

	/*******************************/
	/// <summary>
	/// This class contains static methods to manage tab controls.
	/// </summary>
	public class TabControlSupport
	{
		/// <summary>
		/// Create a new instance of TabControl and set the alignment property.
		/// </summary>
		/// <param name="alignment">The alignment property value.</param>
		/// <returns>New TabControl instance.</returns>
		public static System.Windows.Forms.TabControl CreateTabControl( System.Windows.Forms.TabAlignment alignment)
		{
			System.Windows.Forms.TabControl tabcontrol = new System.Windows.Forms.TabControl();
			tabcontrol.Alignment = alignment;
			return tabcontrol;
		}

		/// <summary>
		/// Set the alignment property to an instance of TabControl .
		/// </summary>
		/// <param name="tabcontrol">An instance of TabControl.</param>
		/// <param name="alignment">The alignment property value.</param>
		public static void SetTabControl( System.Windows.Forms.TabControl tabcontrol, System.Windows.Forms.TabAlignment alignment)
		{
			tabcontrol.Alignment = alignment;
		}

		/// <summary>
		/// Method to add TabPages into the TabControl object.
		/// </summary>
		/// <param name="tabControl">The TabControl to be modified.</param>
		/// <param name="component">A component to be added into the new TabControl.</param>
		public static System.Windows.Forms.Control AddTab(System.Windows.Forms.TabControl tabControl, System.Windows.Forms.Control component)
		{
			System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage();
			tabPage.Controls.Add(component);
			tabControl.TabPages.Add(tabPage);
			return component;
		}
	
		/// <summary>
		/// Method to add TabPages into the TabControl object.
		/// </summary>
		/// <param name="tabControl">The TabControl to be modified.</param>
		/// <param name="TabLabel">The label for the new TabPage.</param>
		/// <param name="component">A component to be added into the new TabControl.</param>
		public static System.Windows.Forms.Control AddTab(System.Windows.Forms.TabControl tabControl, System.String tabLabel, System.Windows.Forms.Control component)
		{
			System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage(tabLabel);
			tabPage.Controls.Add(component);
			tabControl.TabPages.Add(tabPage);
			return component;
		}

		/// <summary>
		/// Method to add TabPages into the TabControl object.
		/// </summary>
		/// <param name="tabControl">The TabControl to be modified.</param>
		/// <param name="component">A component to be added into the new TabControl.</param>
		/// <param name="constraints">The object that should be displayed in the tab but won't because of limitations</param>		
		public static void AddTab(System.Windows.Forms.TabControl tabControl, System.Windows.Forms.Control component, System.Object constraints)
		{
			System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage();
			if (constraints is System.String) 
			{
				tabPage.Text = (String)constraints;
			}
			tabPage.Controls.Add(component);
			tabControl.TabPages.Add(tabPage);
		}

		/// <summary>
		/// Method to add TabPages into the TabControl object.
		/// </summary>
		/// <param name="tabControl">The TabControl to be modified.</param>
		/// <param name="TabLabel">The label for the new TabPage.</param>
		/// <param name="constraints">The object that should be displayed in the tab but won't because of limitations</param>
		/// <param name="component">A component to be added into the new TabControl.</param>
		public static void AddTab(System.Windows.Forms.TabControl tabControl, System.String tabLabel, System.Object constraints, System.Windows.Forms.Control component)
		{
			System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage(tabLabel);
			tabPage.Controls.Add(component);
			tabControl.TabPages.Add(tabPage);
		}

		/// <summary>
		/// Method to add TabPages into the TabControl object.
		/// </summary>
		/// <param name="tabControl">The TabControl to be modified.</param>
		/// <param name="tabLabel">The label for the new TabPage.</param>
		/// <param name="image">Background image for the TabPage.</param>
		/// <param name="component">A component to be added into the new TabControl.</param>
		public static void AddTab(System.Windows.Forms.TabControl tabControl, System.String tabLabel, System.Drawing.Image image, System.Windows.Forms.Control component)
		{
			System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage(tabLabel);			
			tabPage.BackgroundImage = image;
			tabPage.Controls.Add(component);
			tabControl.TabPages.Add(tabPage);			
		}
	}


	/*******************************/
	/// <summary>
	/// The SplitterPanel its a panel with two controls separated by a movable splitter.
	/// </summary>
	public class SplitterPanelSupport : System.Windows.Forms.Panel
	{
		private System.Windows.Forms.Control firstControl;
		private System.Windows.Forms.Control secondControl;
		private System.Windows.Forms.Splitter splitter;
		private System.Windows.Forms.Orientation orientation;
		private int splitterSize;
		private int splitterLocation;
		private int lastSplitterLocation;

		//Default controls
		private System.Windows.Forms.Control defaultFirstControl;
		private System.Windows.Forms.Control defaultSecondControl;

		/// <summary>
		/// Creates a SplitterPanel with Horizontal orientation and two buttons as the default
		/// controls. The default size of the splitter is set to 5.
		/// </summary>
		public SplitterPanelSupport() : base()
		{
			System.Windows.Forms.Button button1 = new System.Windows.Forms.Button();
			System.Windows.Forms.Button button2 = new System.Windows.Forms.Button();
			button1.Text = "button1";
			button2.Text = "button2";
				
			this.lastSplitterLocation = -1;
			this.orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitterSize = 5;

			this.defaultFirstControl  = button1;
			this.defaultSecondControl = button2;
			this.firstControl  = this.defaultFirstControl;
			this.secondControl = this.defaultSecondControl;
			this.splitterLocation = this.firstControl.Size.Width;
			this.splitter = new System.Windows.Forms.Splitter();
			this.SuspendLayout();

			//
			// panel1
			//
			this.Controls.Add(this.splitter);
			this.Controls.Add(this.firstControl);
			this.Controls.Add(this.secondControl);
				
			// 
			// firstControl
			// 
			this.firstControl.Dock = System.Windows.Forms.DockStyle.Left;
			this.firstControl.Name = "firstControl";
			this.firstControl.TabIndex = 0;
				
			// 
			// secondControl
			//
			this.secondControl.Name = "secondControl";
			this.secondControl.TabIndex = 1;
			this.secondControl.Size = new System.Drawing.Size((this.Size.Width - this.firstControl.Size.Width) + this.splitterSize, this.Size.Height);
			this.secondControl.Location = new System.Drawing.Point((this.firstControl.Location.X + this.firstControl.Size.Width + this.splitterSize), 0);

			// 
			// splitter
			//			
			this.splitter.Name = "splitter";
			this.splitter.TabIndex = 2;
			this.splitter.TabStop = false;
			this.splitter.MinExtra = 10;
			this.splitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitter.Size = new System.Drawing.Size(this.splitterSize, this.Size.Height);
			this.splitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(splitter_SplitterMoved);
				
			this.SizeChanged += new System.EventHandler(SplitterPanel_SizeChanged);
			this.ResumeLayout(false);
		}

		/// <summary>
		/// Creates a new SplitterPanelSupport with two buttons as default controls and the specified
		/// splitter orientation.
		/// </summary>
		/// <param name="newOrientation">The orientation of the SplitterPanel.</param>
		public SplitterPanelSupport(int newOrientation) : this()
		{
			this.SplitterOrientation = (System.Windows.Forms.Orientation) newOrientation;
		}

		/// <summary>
		/// Creates a new SplitterPanelSupport with the specified controls and orientation.
		/// </summary>
		/// <param name="newOrientation">The orientation of the SplitterPanel.</param>
		/// <param name="first">The first control of the panel, left-top control.</param>
		/// <param name="second">The second control of the panel, right-botton control.</param>
		public SplitterPanelSupport(int newOrientation, System.Windows.Forms.Control first, System.Windows.Forms.Control second) : this(newOrientation)
		{
			this.FirstControl  = first;
			this.SecondControl = second;
		}


		/// <summary>
		/// Creates a new SplitterPanelSupport with the specified controls and orientation.
		/// </summary>		
		/// <param name="first">The first control of the panel, left-top control.</param>
		/// <param name="second">The second control of the panel, right-botton control.</param>
		public SplitterPanelSupport(System.Windows.Forms.Control first, System.Windows.Forms.Control second) : this()
		{
			this.FirstControl  = first;
			this.SecondControl = second;
		}

		/// <summary>
		/// Adds a control to the SplitterPanel in the first available position.
		/// </summary>		
		/// <param name="control">The control to by added.</param>
		public void Add(System.Windows.Forms.Control control)
		{
			if(FirstControl == defaultFirstControl)
				FirstControl = control;
			else if(SecondControl == defaultSecondControl) 
				SecondControl = control;
		}

		/// <summary>
		/// Adds a control to the SplitterPanel in the specified position.
		/// </summary>		
		/// <param name="control">The control to by added.</param>
		/// <param name="position">The position to add the control in the SpliterPanel.</param>
		public void Add(System.Windows.Forms.Control control, SpliterPosition position)
		{
			if(position == SpliterPosition.First)
				FirstControl = control;
			else if(position == SpliterPosition.Second) 
				SecondControl = control;
		}

		/// <summary>
		/// Defines the possible positions of a control in a SpliterPanel.
		/// </summary>		
		public enum SpliterPosition
		{
			First,
			Second,
		}

		/// <summary>
		/// Gets the specified component.
		/// </summary>
		/// <param name="name">the name of the part of the component to get: "nw": first control, 
		/// "se": second control, "splitter": splitter.</param>
		/// <returns>returns the specified component.</returns>
		public virtual System.Windows.Forms.Control GetComponent(System.String name)
		{
			if (name == "nw")
				return this.FirstControl;
			else if (name == "se")
				return this.SecondControl;
			else if (name == "splitter")
				return this.splitter;
			else return null;
		}

		/// <summary>
		/// First control of the panel. When orientation is Horizontal then this is the left control
		/// when the orientation is Vertical then this is the top control.
		/// </summary>
		public virtual System.Windows.Forms.Control FirstControl
		{
			get
			{
				return this.firstControl;
			}
			set
			{
				this.Controls.Remove(this.firstControl);
				if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
					value.Dock = System.Windows.Forms.DockStyle.Left;
				else
					value.Dock = System.Windows.Forms.DockStyle.Top;
				value.Size = this.firstControl.Size;
				this.firstControl = value;
				this.Controls.Add(this.firstControl);
			}
		}

		/// <summary>
		/// The second control of the panel. Right control when the panel is Horizontal oriented and
		/// botton control when the SplitterPanel orientation is Vertical.
		/// </summary>
		public virtual System.Windows.Forms.Control SecondControl
		{
			get
			{
				return this.secondControl;
			}
			set
			{
				this.Controls.Remove(this.secondControl);
				value.Size = this.secondControl.Size;
				value.Location = this.secondControl.Location;
				this.secondControl = value;
				this.Controls.Add(this.secondControl);
			}
		}

		/// <summary>
		/// The orientation of the SplitterPanel. Specifies how the controls are aligned.
		/// Left to right using Horizontal orientation or top to botton with Vertical orientation.
		/// </summary>
		public virtual System.Windows.Forms.Orientation SplitterOrientation
		{
			get
			{
				return this.orientation;
			}
			set
			{
				if (value != this.orientation)
				{
					this.orientation = value;
					if (value == System.Windows.Forms.Orientation.Vertical)
					{
						int lastWidth = this.firstControl.Size.Width;
						this.firstControl.Dock = System.Windows.Forms.DockStyle.Top;
						this.firstControl.Size = new System.Drawing.Size(this.Width, lastWidth);
						this.splitter.Dock = System.Windows.Forms.DockStyle.Top;
					}
					else
					{
						int lastHeight = this.firstControl.Size.Height;
						this.firstControl.Dock = System.Windows.Forms.DockStyle.Left;
						this.firstControl.Size = new System.Drawing.Size(lastHeight, this.Height);
						this.splitter.Dock = System.Windows.Forms.DockStyle.Left;
					}
					this.ResizeSecondControl();
				}
			}
		}

		/// <summary>
		/// Specifies the location of the Splitter in the panel.
		/// </summary>
		public virtual int SplitterLocation
		{
			get
			{
				return this.splitterLocation;
			}
			set
			{
				if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
					this.firstControl.Size = new System.Drawing.Size(value, this.Height);
				else
					this.firstControl.Size = new System.Drawing.Size(this.Width, value);					
				this.ResizeSecondControl();
				this.lastSplitterLocation = this.splitterLocation;
				this.splitterLocation = value;
			}
		}

		/// <summary>
		/// The last location of the splitter on the panel.
		/// </summary>
		public virtual int LastSplitterLocation
		{
			get
			{
				return this.lastSplitterLocation;
			}
			set
			{
				this.lastSplitterLocation = value;
			}
		}

		/// <summary>
		/// Specifies the size of the splitter divider.
		/// </summary>
		public virtual int SplitterSize
		{
			get
			{
				return this.splitterSize;
			}
			set
			{
				this.splitterSize = value;
				if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
					this.splitter.Size = new System.Drawing.Size(this.splitterSize, this.Size.Height);
				else
					this.splitter.Size = new System.Drawing.Size(this.Size.Width, this.splitterSize);
				this.ResizeSecondControl();
			}
		}

		/// <summary>
		/// The minimum location of the splitter on the panel.
		/// </summary>
		/// <returns>The minimum location value for the splitter.</returns>
		public virtual int GetMinimumLocation()
		{
			return this.splitter.MinSize;
		}

		/// <summary>
		/// The maximum location of the splitter on the panel.
		/// </summary>
		/// <returns>The maximum location value for the splitter.</returns>
		public virtual int GetMaximumLocation()
		{
			if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
				return this.Width - ( this.SplitterSize / 2 );
			else
				return this.Height - ( this.SplitterSize / 2 );
		}

		/// <summary>
		/// Adds a control to splitter panel.
		/// </summary>
		/// <param name="controlToAdd">The control to add.</param>
		/// <param name="dockStyle">The dock style for the control, left-top, or botton-right.</param>
		/// <param name="index">The index of the control in the panel control list.</param>
		protected virtual void AddControl(System.Windows.Forms.Control controlToAdd, System.Object dockStyle, int index)
		{
			if (dockStyle is System.String)
			{
				System.String dock = (System.String)dockStyle;
				if (dock == "botton" || dock == "right")
					this.SecondControl = controlToAdd;
				else if (dock == "top" || dock == "left")
					this.FirstControl  = controlToAdd;
				else
					throw new System.ArgumentException("Cannot add control: unknown constraint: " + dockStyle.ToString());
				this.Controls.SetChildIndex(controlToAdd, index);
			}
			else
				throw new System.ArgumentException("Cannot add control: unknown constraint: " + dockStyle.ToString());
		}

		/// <summary>
		/// Removes the specified control from the panel.
		/// </summary>
		/// <param name="controlToRemove">The control to remove.</param>
		public virtual void RemoveControl(System.Windows.Forms.Control controlToRemove)
		{
			if (this.Contains(controlToRemove))
			{
				this.Controls.Remove(controlToRemove);
				if (this.firstControl == controlToRemove)
					this.secondControl.Dock = System.Windows.Forms.DockStyle.Fill;
				else
					this.firstControl.Dock = System.Windows.Forms.DockStyle.Fill;
			}
		}

		/// <summary>
		/// Remove the control identified by the specified index.
		/// </summary>
		/// <param name="index">The index of the control to remove.</param>
		public virtual void RemoveControl(int index)
		{
			try 
			{
				this.Controls.RemoveAt(index);
				if (this.firstControl != null)
					if (this.Controls.Contains(this.firstControl))
						this.firstControl.Dock = System.Windows.Forms.DockStyle.Fill;
					else if (this.secondControl != null && (this.Controls.Contains(this.secondControl)))
						this.secondControl.Dock = System.Windows.Forms.DockStyle.Fill;
			} // Compatibility with the conversion assistant.
			catch (System.ArgumentOutOfRangeException)
			{
				throw new System.IndexOutOfRangeException("No such child: " + index);
			}
		}
			
		/// <summary>
		/// Changes the location of the splitter in the panel as a percentage of the panel's size.
		/// </summary>
		/// <param name="proportion">The percentage from 0.0 to 1.0.</param>
		public virtual void SetLocationProportional(double proportion)
		{
			if ((proportion > 0.0) && (proportion < 1.0))
				this.SplitterLocation = (int)((this.orientation == System.Windows.Forms.Orientation.Horizontal) ? (proportion * this.Width) : (proportion * this.Height));
			else
				throw new System.ArgumentException("Proportional location must be between 0.0 and 1.0");
		}

		private void splitter_SplitterMoved(System.Object sender, System.Windows.Forms.SplitterEventArgs e)
		{
			this.lastSplitterLocation = this.splitterLocation;
			if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
				this.splitterLocation = this.firstControl.Width;
			else
				this.splitterLocation = this.firstControl.Height;
			this.ResizeSecondControl();
		}

		private void SplitterPanel_SizeChanged(System.Object sender, System.EventArgs e)
		{
			this.ResizeSecondControl();
		}

		private void ResizeSecondControl()
		{
			if (this.orientation == System.Windows.Forms.Orientation.Horizontal)
			{
				this.secondControl.Size = new System.Drawing.Size((this.Width - (this.firstControl.Size.Width + this.splitterSize)), this.Size.Height);
				this.secondControl.Location = new System.Drawing.Point((this.firstControl.Size.Width + this.splitterSize), 0);
			}
			else
			{
				this.secondControl.Size = new System.Drawing.Size(this.Size.Width, (this.Size.Height - (this.firstControl.Size.Height + this.splitterSize)));
				this.secondControl.Location = new System.Drawing.Point(0, (this.firstControl.Size.Height + this.splitterSize));
			}
		}
	}


	/*******************************/
	/// <summary>
	/// Support class for the ListSelectionModel class.
	/// </summary>
	public class ListSelectionModelSupport : System.Windows.Forms.ListBox
	{

		/// <summary>
		/// Private field to store the first index argument from the most recent call to SetSelectionInterval(), AddSelectionInterval() or RemoveSelectionInterval().
		/// </summary>
		protected int anchor = -1;
		/// <summary>
		/// Private field to store the second index argument from the most recent call to SetSelectionInterval(), AddSelectionInterval() or RemoveSelectionInterval().
		/// </summary>
		protected int lead = -1;
		/// <summary>
		/// Private boolean field valueIsAdjusting. Included to provide functional equivalence.
		/// </summary>
		protected bool valueIsAdjusting = false;

		/// <summary>
		/// Default class constructor.
		/// </summary>
		public ListSelectionModelSupport() : base()
		{
		}


		/// <summary>
		/// Adds an interval to the selection.
		/// </summary>
		/// <param name="index0">Start of the interval.</param>
		/// <param name="index1">End of the interval.</param>
		public virtual void AddSelectionInterval(int index0, int index1) 
		{
			int start = System.Math.Min(index0,index1);
			int end = System.Math.Max(index0,index1);
			this.anchor = index0;
			this.lead = index1;
			if (start >= this.Items.Count) return;
			for (int i = start; i <= end; i++) 
			{
				this.SetSelected(i, true);
			}	
		}

		/// <summary>
		/// Clears the selection set.
		/// </summary>
		public virtual void ClearSelection() 
		{
			base.ClearSelected();
		}


		/// <summary>
		/// Return the first index argument from the most recent call to SetSelectionInterval(), AddSelectionInterval() or RemoveSelectionInterval().
		/// </summary>
		public virtual int GetAnchorSelectionIndex() 
		{
			return anchor;
		}


		/// <summary>
		/// Return the second index argument from the most recent call to SetSelectionInterval(), AddSelectionInterval() or RemoveSelectionInterval().
		/// </summary>
		public virtual int GetLeadSelectionIndex() 
		{
			return lead;
		}


		/// <summary>
		/// Returns the last selected index or -1 if the selection is empty.
		/// </summary>
		public virtual int GetMaxSelectionIndex() 
		{
			if (this.SelectedIndices.Count == 0)
				return -1;
			else
				return this.SelectedIndices[this.SelectedIndices.Count - 1];
		}


		/// <summary>
		/// Returns the first selected index or -1 if the selection is empty.
		/// </summary>
		public virtual int GetMinSelectionIndex() 
		{
			if (this.SelectedIndices.Count == 0)
				return -1;
			else
				return this.SelectedIndices[0];
		}

	
		/// <summary>
		/// Set the anchor selection index.
		/// </summary>
		/// <param name="index"></param>
		public virtual void SetAnchorSelectionIndex(int index) 
		{
			anchor = index;
		}


		/// <summary>
		/// Set the lead selection index.
		/// </summary>
		/// <param name="index"></param>
		public virtual void SetLeadSelectionIndex(int index) 
		{
			lead = index;
		}
	

		/// <summary>
		/// Remove the indices in the interval index0,index1 (inclusive).
		/// </summary>
		/// <param name="index0">Start of the interval.</param>
		/// <param name="index1">End of the interval.</param>
		public virtual void RemoveIndexInterval(int index0, int index1) 
		{
			int start = System.Math.Min(index0,index1);
			int end = System.Math.Max(index0,index1);
			if (start >= this.Items.Count) return;
			for (int i = start; i <= end; i++) 
			{
				this.SetSelected(i, false);
			}
		}


		/// <summary>
		/// Change the selection to be the set difference of the current selection and the indices between index0 and index1 inclusive.
		/// </summary>
		/// <param name="index0">Start of the interval.</param>
		/// <param name="index1">End of the interval.</param>
		public virtual void RemoveSelectionInterval(int index0, int index1) 
		{
			int start = System.Math.Min(index0,index1);
			int end = System.Math.Max(index0,index1);
			this.anchor = index0;
			this.lead = index1;
			if (start >= this.Items.Count) return;
			for (int i = start; i <= end; i++) 
			{
				this.SetSelected(i, false);
			}
		}


		/// <summary>
		/// Returns true if the value is undergoing a series of changes.
		/// </summary>
		public virtual bool GetValueIsAdjusting() 
		{
			return valueIsAdjusting;
		}


		/// <summary>
		/// This property is true if upcoming changes to the value of the model should be considered a single event.
		/// </summary>
		/// <param name="valueIsAdjusting"></param>
		public virtual void SetValueIsAdjusting(bool valueIsAdjusting) 
		{
			this.valueIsAdjusting = valueIsAdjusting;
		}


		/// <summary>
		/// Change the selection to be between index0 and index1 inclusive.
		/// </summary>
		/// <param name="index0">Start of the interval.</param>
		/// <param name="index1">End of the interval.</param>
		public virtual void SetSelectionInterval(int index0, int index1) 
		{
			this.ClearSelected();
			int start = System.Math.Min(index0,index1);
			int end = System.Math.Max(index0,index1);
			this.anchor = index0;
			this.lead = index1;
			if (start >= this.Items.Count) return;
			for (int i = start; i <= end; i++) 
			{
				this.SetSelected(i, false);
			}
		}

		/// <summary>
		/// Creates a shallow copy of the current Object.
		/// </summary>
		/// <returns>A shallow copy of the current Object.</returns>
		public virtual System.Object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	/*******************************/
	/// <summary>
	/// Gets the selected items in a ListBox instance.
	/// </summary>
	/// <param name="listbox">A listbox to get its selected items.</param>
	/// <returns>An object array with the selected items.</returns>
	public static System.Object[] GetSelectedItems(System.Windows.Forms.ListBox listbox)
	{
		System.Object[] selectedvalues = new System.Object[listbox.SelectedItems.Count];
		listbox.SelectedItems.CopyTo(selectedvalues, 0);
		return selectedvalues;
	}


	/*******************************/
	/// <summary>
	/// This class contains methods that supports list management operations in 
	/// ListBox.ObjectCollection instances.
	/// </summary>
	public class ListBoxObjectCollectionSupport
	{
		/// <summary>
		/// Gets the index of the first occurence of the specified element after the specified index.
		/// </summary>
		/// <param name="items">The collection of objects.</param>
		/// <param name="element">The element to search.</param>
		/// <param name="index">First index of the searching.</param>
		/// <returns>The index where the element was found or -1 if it wasn't.</returns>
		public static int IndexOf(System.Windows.Forms.ListBox.ObjectCollection items, System.Object element, int index)
		{
			for (int itemIndex = index; itemIndex < items.Count; itemIndex++)
				if (items[itemIndex] == element)
					return itemIndex;
			return -1;
		}

		/// <summary>
		/// Returns the last element of the collection of objects specified.
		/// </summary>
		/// <param name="items">The collection of objects.</param>
		/// <returns>The last item of the collection.</returns>
		public static System.Object LastElement(System.Windows.Forms.ListBox.ObjectCollection items)
		{
			if (items.Count == 0)
				throw new System.ArgumentOutOfRangeException();
			return items[items.Count - 1];
		}

		/// <summary>
		/// Gets the last index before the specified index of the specified element.
		/// </summary>
		/// <param name="items">The collection of objects.</param>
		/// <param name="element">The element to search.</param>
		/// <param name="index">Last index of the search.</param>
		/// <returns>Last index before the specified index of the element.</returns>
		public static int LastIndexOf(System.Windows.Forms.ListBox.ObjectCollection items, System.Object element, int index)
		{
			for (int itemIndex = index; itemIndex >= 0; itemIndex--)
				if (items[itemIndex] == element)
					return itemIndex;
			return -1;
		}

		/// <summary>
		/// Gets the index of the last occurrence of the specified element.
		/// </summary>
		/// <param name="items">The collection of elements.</param>
		/// <param name="element">The element to search.</param>
		/// <returns>Index of the last ocurrence of the element.</returns>
		public static int LastIndexOf(System.Windows.Forms.ListBox.ObjectCollection items, System.Object element)
		{
			for (int itemIndex = items.Count; itemIndex >= 0; itemIndex--)
				if (items[itemIndex] == element)
					return itemIndex;
			return -1;
		}

		/// <summary>
		/// Deletes specified range of elements in the specified collection of items.
		/// </summary>
		/// <param name="items">The collection of objects.</param>
		/// <param name="fromIndex">Minimum index of the range.</param>
		/// <param name="toIndex">Maximum index of the range.</param>
		public static void RemoveRange(System.Windows.Forms.ListBox.ObjectCollection items, int fromIndex, int toIndex)
		{
			for (int itemIndex = toIndex; itemIndex >= fromIndex; itemIndex--)
			{
				if (itemIndex >= items.Count)
					throw new System.IndexOutOfRangeException(itemIndex + " >= " + items.Count);
				else if (itemIndex < 0)
					throw new System.IndexOutOfRangeException("Array index out of range: " + itemIndex);
				else
					items.RemoveAt(itemIndex);
			}
		}

		/// <summary>
		/// Gets an array representation of the specified collection of objects.
		/// </summary>
		/// <param name="items">The collection of objects.</param>
		/// <returns>An array with all the elements in the collection.</returns>
		public static System.Object[] ToArray(System.Windows.Forms.ListBox.ObjectCollection items)
		{
			System.Object[] result = new System.Object[items.Count];
			items.CopyTo(result, 0);
			return result;
		}
	}


	/*******************************/
	/// <summary>
	/// This class has static methods for manage CheckBox and RadioButton controls.
	/// </summary>
	public class CheckBoxSupport
	{

		/// <summary>
		/// Receives a CheckBox instance and sets the specific text and style.
		/// </summary>
		/// <param name="checkBoxInstance">CheckBox instance to be set.</param>
		/// <param name="text">The text to set Text property.</param>
		/// <param name="style">The style to be used to set the threeState property.</param>
		public static void SetCheckBox(System.Windows.Forms.CheckBox checkBoxInstance, System.String text, int style)
		{
			checkBoxInstance.Text = text;			
			if (style == 2097152)
				checkBoxInstance.ThreeState = true;
		}

		/// <summary>
		/// Returns a new CheckBox instance with the specified text
		/// </summary>
		/// <param name="text">The text to create the CheckBox with</param>
		/// <returns>A new CheckBox instance</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.String text)
		{
			System.Windows.Forms.CheckBox tempCheck = new System.Windows.Forms.CheckBox();
			tempCheck.Text = text;
			return tempCheck;
		}

		/// <summary>
		/// Creates a CheckBox with a specified image.
		/// </summary>
		/// <param name="icon">The image to be used with the CheckBox.</param>
		/// <returns>A new CheckBox instance with an image.</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.Drawing.Image icon)
		{
			System.Windows.Forms.CheckBox tempCheckBox = new System.Windows.Forms.CheckBox();
			tempCheckBox.Image = icon;
			return tempCheckBox;
		}

		/// <summary>
		/// Creates a CheckBox with a specified label and image.
		/// </summary>
		/// <param name="text">The text to be used as label.</param>
		/// <param name="icon">The image to be used with the CheckBox.</param>
		/// <returns>A new CheckBox instance with a label and an image.</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.String text, System.Drawing.Image icon)
		{
			System.Windows.Forms.CheckBox tempCheckBox = new System.Windows.Forms.CheckBox();
			tempCheckBox.Text = text;
			tempCheckBox.Image = icon;
			return tempCheckBox;
		}

		/// <summary>
		/// Returns a new CheckBox instance with the specified text and state
		/// </summary>
		/// <param name="text">The text to create the CheckBox with</param>
		/// <param name="checkedStatus">Indicates if the Checkbox is checked or not</param>
		/// <returns>A new CheckBox instance</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.String text, bool checkedStatus)
		{
			System.Windows.Forms.CheckBox tempCheckBox = new System.Windows.Forms.CheckBox();
			tempCheckBox.Text = text;
			tempCheckBox.Checked = checkedStatus;
			return tempCheckBox;
		}

		/// <summary>
		/// Creates a CheckBox with a specified image and checked if checkedStatus is true.
		/// </summary>
		/// <param name="icon">The image to be used with the CheckBox.</param>
		/// <param name="checkedStatus">Value to be set to Checked property.</param>
		/// <returns>A new CheckBox instance.</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.Drawing.Image icon, bool checkedStatus)
		{
			System.Windows.Forms.CheckBox tempCheckBox = new System.Windows.Forms.CheckBox();
			tempCheckBox.Image = icon;
			tempCheckBox.Checked = checkedStatus;
			return tempCheckBox;
		}

		/// <summary>
		/// Creates a CheckBox with label, image and checked if checkedStatus is true.
		/// </summary>
		/// <param name="text">The text to be used as label.</param>
		/// <param name="icon">The image to be used with the CheckBox.</param>
		/// <param name="checkedStatus">Value to be set to Checked property.</param>
		/// <returns>A new CheckBox instance.</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.String text, System.Drawing.Image icon, bool checkedStatus)
		{
			System.Windows.Forms.CheckBox tempCheckBox = new System.Windows.Forms.CheckBox();
			tempCheckBox.Text = text;
			tempCheckBox.Image = icon;
			tempCheckBox.Checked = checkedStatus;
			return tempCheckBox;
		}

		/// <summary>
		/// Creates a CheckBox with a specific control.
		/// </summary>
		/// <param name="control">The control to be added to the CheckBox.</param>
		/// <returns>The new CheckBox with the specific control.</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.Windows.Forms.Control control)
		{
			System.Windows.Forms.CheckBox tempCheckBox = new System.Windows.Forms.CheckBox();
			tempCheckBox.Controls.Add(control);
			control.Location = new System.Drawing.Point(0, 18);
			return tempCheckBox;
		}

		/// <summary>
		/// Creates a CheckBox with the specific control and style.
		/// </summary>
		/// <param name="control">The control to be added to the CheckBox.</param>
		/// <param name="style">The style to be used to set the threeState property.</param>
		/// <returns>The new CheckBox with the specific control and style.</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.Windows.Forms.Control control, int style)
		{
			System.Windows.Forms.CheckBox tempCheckBox = new System.Windows.Forms.CheckBox();
			tempCheckBox.Controls.Add(control);
			control.Location = new System.Drawing.Point(0, 18);
			if (style == 2097152)
				tempCheckBox.ThreeState = true;
			return tempCheckBox;
		}

		/// <summary>
		/// Returns a new RadioButton instance with the specified text in the specified panel.
		/// </summary>
		/// <param name="text">The text to create the RadioButton with.</param>
		/// <param name="checkedStatus">Indicates if the RadioButton is checked or not.</param>
		/// <param name="panel">The panel where the RadioButton will be placed.</param>
		/// <returns>A new RadioButton instance.</returns>
		/// <remarks>If the panel is null the third param is ignored</remarks>
		public static System.Windows.Forms.RadioButton CreateRadioButton(System.String text, bool checkedStatus, System.Windows.Forms.Panel panel)
		{
			System.Windows.Forms.RadioButton tempCheckBox = new System.Windows.Forms.RadioButton();
			tempCheckBox.Text = text;
			tempCheckBox.Checked= checkedStatus;
			if (panel != null)
				panel.Controls.Add(tempCheckBox);
			return tempCheckBox;
		}

		/// <summary>
		/// Receives a CheckBox instance and sets the Text and Image properties.
		/// </summary>
		/// <param name="checkBoxInstance">CheckBox instance to be set.</param>
		/// <param name="text">Value to be set to Text property.</param>
		/// <param name="icon">Value to be set to Image property.</param>
		public static void SetCheckBox(System.Windows.Forms.CheckBox checkBoxInstance, System.String text, System.Drawing.Image icon)
		{
			checkBoxInstance.Text = text;
			checkBoxInstance.Image = icon;
		}

		/// <summary>
		/// Receives a CheckBox instance and sets the Text and Checked properties.
		/// </summary>
		/// <param name="checkBoxInstance">CheckBox instance to be set</param>
		/// <param name="text">Value to be set to Text</param>
		/// <param name="checkedStatus">Value to be set to Checked property.</param>
		public static void SetCheckBox(System.Windows.Forms.CheckBox checkBoxInstance, System.String text, bool checkedStatus)
		{
			checkBoxInstance.Text = text;
			checkBoxInstance.Checked = checkedStatus;
		}

		/// <summary>
		/// Receives a CheckBox instance and sets the Image and Checked properties.
		/// </summary>
		/// <param name="checkBoxInstance">CheckBox instance to be set.</param>
		/// <param name="icon">Value to be set to Image property.</param>
		/// <param name="checkedStatus">Value to be set to Checked property.</param>
		public static void SetCheckBox(System.Windows.Forms.CheckBox checkBoxInstance, System.Drawing.Image icon, bool checkedStatus)
		{
			checkBoxInstance.Image = icon;
			checkBoxInstance.Checked = checkedStatus;
		}

		/// <summary>
		/// Receives a CheckBox instance and sets the Text, Image and Checked properties.
		/// </summary>
		/// <param name="checkBoxInstance">CheckBox instance to be set.</param>
		/// <param name="text">Value to be set to Text property.</param>
		/// <param name="icon">Value to be set to Image property.</param>
		/// <param name="checkedStatus">Value to be set to Checked property.</param>
		public static void SetCheckBox(System.Windows.Forms.CheckBox checkBoxInstance, System.String text, System.Drawing.Image icon, bool checkedStatus)
		{
			checkBoxInstance.Text = text;
			checkBoxInstance.Image = icon;
			checkBoxInstance.Checked = checkedStatus;
		}
		
		/// <summary>
		/// Receives a CheckBox instance and sets the control specified.
		/// </summary>
		/// <param name="checkBoxInstance">CheckBox instance to be set.</param>
		/// <param name="control">The control assiciated with the CheckBox</param>
		public static void SetCheckBox(System.Windows.Forms.CheckBox checkBoxInstance, System.Windows.Forms.Control control)
		{
			checkBoxInstance.Controls.Add(control);
			control.Location = new System.Drawing.Point(0, 18);
		}

		/// <summary>
		/// Receives a CheckBox instance and sets the specific control and style.
		/// </summary>
		/// <param name="checkBoxInstance">CheckBox instance to be set.</param>
		/// <param name="control">The control assiciated with the CheckBox.</param>
		/// <param name="style">The style to be used to set the threeState property.</param>
		public static void SetCheckBox(System.Windows.Forms.CheckBox checkBoxInstance, System.Windows.Forms.Control control, int style)
		{
			checkBoxInstance.Controls.Add(control);
			control.Location = new System.Drawing.Point(0, 18);
			if (style == 2097152)
				checkBoxInstance.ThreeState = true;
		}

		/// <summary>
		/// Receives an instance of a RadioButton and sets its Text and Checked properties.
		/// </summary>
		/// <param name="RadioButtonInstance">The instance of RadioButton to be set.</param>
		/// <param name="text">The text to set Text property.</param>
		/// <param name="checkedStatus">Indicates if the RadioButton is checked or not.</param>
		public static void SetCheckbox(System.Windows.Forms.RadioButton radioButtonInstance, System.String text, bool checkedStatus)
		{
			radioButtonInstance.Text = text;
			radioButtonInstance.Checked = checkedStatus;
		}

		/// <summary>
		/// Receives an instance of a RadioButton and sets its Text and Checked properties and its containing panel
		/// </summary>
		/// <param name="CheckBoxInstance">The instance of RadioButton to be set</param>
		/// <param name="text">The text to set Text property</param>
		/// <param name="checkedStatus">Indicates if the RadioButton is checked or not</param>
		/// <param name="panel">The panel where the RadioButton will be placed</param>
		/// <remarks>If the panel is null the third param is ignored</remarks>
		public static void SetRadioButton(System.Windows.Forms.RadioButton radioButtonInstance, System.String text, bool checkedStatus, System.Windows.Forms.Panel panel)
		{
			radioButtonInstance.Text = text;
			radioButtonInstance.Checked = checkedStatus;
			if (panel != null)
				panel.Controls.Add(radioButtonInstance);
		}
		
		/// <summary>
		/// Creates a CheckBox with a specified text label and style.
		/// </summary>
		/// <param name="text">The text to be used as label.</param>
		/// <param name="style">The style to be used to set the threeState property.</param>
		/// <returns>A new CheckBox instance.</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.String text, int style)
		{
			System.Windows.Forms.CheckBox checkBox = new System.Windows.Forms.CheckBox();
			checkBox.Text = text;
			if (style == 2097152)
				checkBox.ThreeState = true;
			return checkBox;
		}
		
		/// <summary>
		/// Receives a CheckBox instance and sets the Text and ThreeState properties.
		/// </summary>
		/// <param name="checkBox">CheckBox instance to be set.</param>
		/// <param name="text">Value to be set to Text property.</param>
		/// <param name="style">The style to be used to set the threeState property.</param>
		public static void setCheckBox(System.Windows.Forms.CheckBox checkBox, System.String text, int style)
		{
			checkBox.Text = text;
			if (style == 2097152)
				checkBox.ThreeState = true;
		}
		
		/// <summary>
		/// Creates a CheckBox with a specified text label, image and style.
		/// </summary>
		/// <param name="text">The text to be used as label.</param>
		/// <param name="icon">The image to be used with the CheckBox.</param>
		/// <param name="style">The style to be used to set the threeState property.</param>
		/// <returns>A new CheckBox instance.</returns>
		public static System.Windows.Forms.CheckBox CreateCheckBox(System.String text, System.Drawing.Image icon, int style)
		{
			System.Windows.Forms.CheckBox checkBox = new System.Windows.Forms.CheckBox();
			checkBox.Text = text;
			checkBox.Image = icon;
			if (style == 2097152)
				checkBox.ThreeState = true;
			return checkBox;
		}
		
		/// <summary>
		/// Receives a CheckBox instance and sets the Text, Image and ThreeState properties.
		/// </summary>
		/// <param name="checkBox">CheckBox instance to be set.</param>
		/// <param name="text">Value to be set to Text property.</param>
		/// <param name="icon">Value to be set to Image property.</param>
		/// <param name="style">The style to be used to set the threeState property.</param>
		public static void setCheckBox(System.Windows.Forms.CheckBox checkBox, System.String text, System.Drawing.Image icon, int style)
		{
			checkBox.Text = text;
			checkBox.Image = icon;
			if (style == 2097152)
				checkBox.ThreeState = true;
		}
		
		/// <summary>
		/// The SetIndeterminate method is used to sets or clear the CheckState of the CheckBox component.
		/// </summary>
		/// <param name="indeterminate">The value to the Indeterminate state. If true, the state is set; otherwise, it is cleared.</param>
		/// <param name="checkBox">The CheckBox component to be modified.</param>
		/// <returns></returns>
		public static System.Windows.Forms.CheckBox SetIndeterminate(bool indeterminate, System.Windows.Forms.CheckBox checkBox)
		{
			if (indeterminate)
				checkBox.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			else if (checkBox.Checked)
				checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
			else
				checkBox.CheckState = System.Windows.Forms.CheckState.Unchecked;
			return checkBox;
		}
	}

	/*******************************/
	/// <summary>
	/// This method works as a handler for the Control.Layout event, it arranges the controls into the container 
	/// control in a left-to-right orientation.
	/// The location of each control will be calculated according the number of them in the container. 
	/// The corresponding alignment, the horizontal and vertical spacing between the inner controls are specified
	/// as an array of object values in the Tag property of the container.
	/// </summary>
	/// <param name="event_sender">The container control in which the controls will be relocated.</param>
	/// <param name="eventArgs">Data of the event.</param>
	public static void FlowLayoutResize(System.Object event_sender, System.Windows.Forms.LayoutEventArgs eventArgs)
	{
		System.Windows.Forms.Control container = (System.Windows.Forms.Control) event_sender;
		if (container.Tag is System.Array)
		{
			System.Object[] items = (System.Object[]) container.Tag;
			if (items.Length == 3)
			{
				container.SuspendLayout();

				int width = container.Width;
				int height = container.Height;
				if (!(container is System.Windows.Forms.ScrollableControl))
				{
					width = container.DisplayRectangle.Width;
					height = container.DisplayRectangle.Height;
				}
				else
					if (container is System.Windows.Forms.Form)
					{
						width = ((System.Windows.Forms.Form) container).ClientSize.Width;
						height = ((System.Windows.Forms.Form) container).ClientSize.Height;
					}
				System.Drawing.ContentAlignment alignment = (System.Drawing.ContentAlignment) items[0];
				int horizontal = (int) items[1];
				int vertical = (int) items[2];

				// Split controls in several rows
				System.Collections.ArrayList rows = new System.Collections.ArrayList();
				System.Collections.ArrayList list = new System.Collections.ArrayList();
				int tempWidth = 0;
				int tempHeight = 0;
				int totalHeight = 0;
				for (int index = 0; index < container.Controls.Count; index++)
				{
					if (tempHeight < container.Controls[index].Height)
						tempHeight = container.Controls[index].Height;

					list.Add(container.Controls[index]);

					if (index == 0) tempWidth = container.Controls[0].Width;

					if (index == container.Controls.Count - 1)
					{
						rows.Add(list);
						totalHeight += tempHeight + vertical;
					}
					else
					{
						tempWidth += horizontal + container.Controls[index + 1].Width;
						if (tempWidth >= width - horizontal * 2)
						{
							rows.Add(list);
							totalHeight += tempHeight + vertical;
							tempHeight = 0;
							list = new System.Collections.ArrayList();
							tempWidth = container.Controls[index + 1].Width;
						}
					}
				}
				totalHeight -= vertical;

				// Break out alignment coordinates
				int h = 0;
				int cx = 0;
				int cy = 0;
				if (((int) alignment & 0x00F) > 0)
				{
					h = (int) alignment;
					cy = 1;
				}
				if (((int) alignment & 0x0F0) > 0)
				{
					h = (int) alignment >> 4;
					cy = 2;
				}
				if (((int) alignment & 0xF00) > 0)
				{
					h = (int) alignment >> 8;
					cy = 3;
				}
				if (h == 1) cx = 1;
				if (h == 2) cx = 2;
				if (h == 4) cx = 3;

				int ypos = vertical;
				if (cy == 2) ypos = height / 2 - totalHeight / 2;
				if (cy == 3) ypos = height - totalHeight - vertical;
				foreach (System.Collections.ArrayList row in rows)
				{
					int maxHeight = PlaceControls(row, width, cx, ypos, horizontal);
					ypos += vertical + maxHeight;
				}
				container.ResumeLayout();
			}
		}
	}

	private static int PlaceControls(System.Collections.ArrayList controls, int width, int cx, int ypos, int horizontal)
	{
		int count = controls.Count;
		int controlsWidth = 0;
		int maxHeight = 0;
		foreach (System.Windows.Forms.Control control in controls)
		{
			controlsWidth += control.Width;
			if (maxHeight < control.Height) maxHeight = control.Height;
		}
		controlsWidth += horizontal * (count - 1);

		// Start x point
		int xpos = 0;
		if (cx == 1) xpos = horizontal; // Left
		if (cx == 2) xpos = width / 2 - controlsWidth / 2; // Center
		if (cx == 3) xpos = width - horizontal - controlsWidth; // Right

		// Place controls
		int x = xpos;
		foreach (System.Windows.Forms.Control control in controls)
		{
			int y = ypos + (maxHeight / 2) - control.Height / 2;
			control.Location = new System.Drawing.Point(x, y);
			x += control.Width + horizontal;
		}
		return maxHeight;
	}


	/*******************************/
	/// <summary>
	/// Provides overloaded methods to create and set values to an instance of System.Drawing.Pen.
	/// </summary>
	public class StrokeConsSupport
	{
		/// <summary>
		/// Creates an instance of System.Drawing.Pen with the default SolidBrush black.
		/// And then set the parameters into their corresponding properties.
		/// </summary>
		/// <param name="width">The width of the stroked line.</param>
		/// <param name="cap">The DashCap end of line style.</param>
		/// <param name="join">The LineJoin style.</param>
		/// <returns>A new instance with the values set.</returns>
		public static System.Drawing.Pen CreatePenInstance(float width, int cap, int join)
		{
			System.Drawing.Pen tempPen = new System.Drawing.Pen(System.Drawing.Brushes.Black,width);
			tempPen.StartCap = (System.Drawing.Drawing2D.LineCap)  cap;
			tempPen.EndCap = (System.Drawing.Drawing2D.LineCap) cap;
			tempPen.LineJoin = (System.Drawing.Drawing2D.LineJoin)join;
			return tempPen;
		}

		/// <summary>
		/// Creates an instance of System.Drawing.Pen with the default SolidBrush black.
		/// And then set the parameters into their corresponding properties.
		/// </summary>
		/// <param name="width">The width of the stroked line.</param>
		/// <param name="cap">The DashCap end of line style.</param>
		/// <param name="join">The LineJoin style.</param>
		/// <param name="miterlimit">The limit of the line.</param>
		/// <returns>A new instance with the values set.</returns>
		public static System.Drawing.Pen CreatePenInstance(float width, int cap, int join, float miterlimit)
		{
			System.Drawing.Pen tempPen = new System.Drawing.Pen(System.Drawing.Brushes.Black,width);
			tempPen.StartCap = (System.Drawing.Drawing2D.LineCap)  cap;
			tempPen.EndCap = (System.Drawing.Drawing2D.LineCap) cap;
			tempPen.LineJoin = (System.Drawing.Drawing2D.LineJoin)join;
			tempPen.MiterLimit = miterlimit;
			return tempPen;
		}

		/// <summary>
		/// Creates an instance of System.Drawing.Pen with the default SolidBrush black.
		/// And then set the parameters into their corresponding properties.
		/// </summary>
		/// <param name="width">The width of the stroked line.</param>
		/// <param name="cap">The DashCap end of line style.</param>
		/// <param name="join">The LineJoin style.</param>
		/// <param name="miterlimit">The limit of the line.</param>
		/// <param name="dashPattern">The array to use to make the dash.</param>
		/// <param name="dashOffset">The space between each dash.</param>
		/// <returns>A new instance with the values set.</returns>
		public static System.Drawing.Pen CreatePenInstance(float width, int cap, int join, float miterlimit,float[] dashPattern, float dashOffset)
		{
			System.Drawing.Pen tempPen = new System.Drawing.Pen(System.Drawing.Brushes.Black,width);
			tempPen.StartCap = (System.Drawing.Drawing2D.LineCap)  cap;
			tempPen.EndCap = (System.Drawing.Drawing2D.LineCap) cap;
			tempPen.LineJoin = (System.Drawing.Drawing2D.LineJoin)join;
			tempPen.MiterLimit = miterlimit;
			tempPen.DashPattern = dashPattern;
			tempPen.DashOffset = dashOffset;
			return tempPen;
		}

		/// <summary>
		/// Sets a Pen instance with the corresponding DashCap and LineJoin values.
		/// </summary>
		/// <param name="cap">The DashCap end of line style.</param>
		/// <param name="join">The LineJoin style.</param>
		/// <returns>A new instance with the values set.</returns>
		public static void SetPen(System.Drawing.Pen tempPen, int cap, int join)
		{
			tempPen.StartCap = (System.Drawing.Drawing2D.LineCap)  cap;
			tempPen.EndCap = (System.Drawing.Drawing2D.LineCap) cap;
			tempPen.LineJoin = (System.Drawing.Drawing2D.LineJoin)join;
		}

		/// <summary>
		/// Sets a Pen instance with the corresponding DashCap, LineJoin and MiterLimit values.
		/// </summary>
		/// <param name="cap">The DashCap end of line style.</param>
		/// <param name="join">The LineJoin style.</param>
		/// <param name="miterlimit">The limit of the line.</param>
		public static void SetPen(System.Drawing.Pen tempPen, int cap, int join, float miterlimit)
		{
			tempPen.StartCap = (System.Drawing.Drawing2D.LineCap)  cap;
			tempPen.EndCap = (System.Drawing.Drawing2D.LineCap) cap;
			tempPen.LineJoin = (System.Drawing.Drawing2D.LineJoin)join;
			tempPen.MiterLimit = miterlimit;
		}

		/// <summary>
		/// Sets a Pen instance with the corresponding DashCap, LineJoin, MiterLimit, DashPattern and 
		/// DashOffset values.
		/// </summary>
		/// <param name="cap">The DashCap end of line style.</param>
		/// <param name="join">The LineJoin style.</param>
		/// <param name="miterlimit">The limit of the line.</param>
		/// <param name="dashPattern">The array to use to make the dash.</param>
		/// <param name="dashOffset">The space between each dash.</param>
		public static void SetPen(System.Drawing.Pen tempPen, float width, int cap, int join, float miterlimit, float[] dashPattern, float dashOffset)
		{
			tempPen.StartCap = (System.Drawing.Drawing2D.LineCap)  cap;
			tempPen.EndCap = (System.Drawing.Drawing2D.LineCap) cap;
			tempPen.LineJoin = (System.Drawing.Drawing2D.LineJoin)join;
			tempPen.MiterLimit = miterlimit;
			tempPen.DashPattern = dashPattern;
			tempPen.DashOffset = dashOffset;
		}
	}

	/*******************************/
	/// <summary>
	/// Returns true if nothing is selected
	/// </summary>
	/// <param name="listbox">The listbox to check if something is selected</param>
	/// <returns>Returns true if nothing is selected false otherwise</returns>
	public static bool IsSelectionEmpty(System.Windows.Forms.ListBox listbox)
	{
		return (listbox.SelectedItem == null);
	}


	/*******************************/
	/// <summary>
	/// Contains methods to construct ProgressBar objects
	/// </summary>
	public class ProgressBarSupport
	{ 
		/// <summary>
		/// Creates a ProgressBar with the specified range and an initial position of 0.
		/// </summary>
		/// <param name="maxRange">The maximum range value of the control. The control's range is from 0 to range.</param>
		/// <returns>The new ProgressBar</returns>
		public static System.Windows.Forms.ProgressBar CreateProgress(int maxRange)
		{
			System.Windows.Forms.ProgressBar tempProgress = new System.Windows.Forms.ProgressBar();
			tempProgress.Maximum = maxRange;
			return tempProgress;
		}

		/// <summary>
		/// Creates a ProgressBar with the specified range and initial position.
		/// </summary>
		/// <param name="minRange">The minimum range value of the ProgressBar.</param>
		/// <param name="maxRange">The maximum range value of the ProgressBar.</param>
		/// <returns>The new ProgressBar</returns>
		public static System.Windows.Forms.ProgressBar CreateProgress(int minRange, int maxRange)
		{
			System.Windows.Forms.ProgressBar tempProgress = CreateProgress(maxRange);
			tempProgress.Minimum = minRange;
			return tempProgress;
		}
	}

	/*******************************/
	public class FormSupport
	{
		/// <summary>
		/// Creates a Form instance and sets the property Text to the specified parameter.
		/// </summary>
		/// <param name="Text">Value for the Form property Text</param>
		/// <returns>A new Form instance</returns>
		public static System.Windows.Forms.Form CreateForm(System.String text)
		{
			System.Windows.Forms.Form tempForm;
			tempForm = new System.Windows.Forms.Form();
			tempForm.Text = text;
			return tempForm;
		}

		/// <summary>
		/// Creates a Form instance and sets the property Text to the specified parameter.
		/// Adds the received control to the Form's Controls collection in the position 0.
		/// </summary>
		/// <param name="text">Value for the Form Text property.</param>
		/// <param name="control">Control to be added to the Controls collection.</param>
		/// <returns>A new Form instance</returns>
		public static System.Windows.Forms.Form CreateForm(System.String text, System.Windows.Forms.Control control )
		{
			System.Windows.Forms.Form tempForm;
			tempForm = new System.Windows.Forms.Form();
			tempForm.Text = text;
			tempForm.Controls.Add( control );	
			tempForm.Controls.SetChildIndex( control, 0 );
			return tempForm;
		}
		
		
		/// <summary>
		/// Creates a Form instance and sets the property Owner to the specified parameter.
		/// This also sets the FormBorderStyle and ShowInTaskbar properties.
		/// </summary>
		/// <param name="Owner">Value for the Form property Owner</param>
		/// <returns>A new Form instance</returns>
		public static System.Windows.Forms.Form CreateForm(System.Windows.Forms.Form owner)
		{
			System.Windows.Forms.Form tempForm;
			tempForm = new System.Windows.Forms.Form();
			tempForm.Owner = owner;
			tempForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			tempForm.ShowInTaskbar = false;
			return tempForm;
		}


		/// <summary>
		/// Creates a Form instance and sets the property Owner to the specified parameter.
		/// Sets the FormBorderStyle and ShowInTaskbar properties.
		/// Also add the received Control to the Form's Controls collection in the position 0.
		/// </summary>
		/// <param name="owner">Value for the Form property Owner</param>
		/// <param name="header">Control to be added to the Form's Controls collection</param>
		/// <returns>A new Form instance</returns>
		public static System.Windows.Forms.Form CreateForm(System.Windows.Forms.Form owner, System.Windows.Forms.Control header)
		{
			System.Windows.Forms.Form tempForm;
			tempForm = new System.Windows.Forms.Form();
			tempForm.Owner = owner;
			tempForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			tempForm.ShowInTaskbar = false;
			tempForm.Controls.Add( header );
			tempForm.Controls.SetChildIndex( header, 0 );
			return tempForm;
		}

		/// <summary>
		/// Creates a Form instance and sets the FormBorderStyle property.
		/// </summary>
		/// <param name="title">The title of the Form</param>
		/// <param name="resizable">Boolean value indicating if the Form is or not resizable</param>
		/// <returns>A new Form instance</returns>
		public static System.Windows.Forms.Form CreateForm(System.String title,bool resizable)
		{
			System.Windows.Forms.Form tempForm;
			tempForm = new System.Windows.Forms.Form();
			tempForm.Text = title;
			if(resizable)
				tempForm.FormBorderStyle  = System.Windows.Forms.FormBorderStyle.Sizable;
			else
				tempForm.FormBorderStyle  = System.Windows.Forms.FormBorderStyle.FixedSingle;
			tempForm.TopLevel = false;
			tempForm.MaximizeBox = false;
			tempForm.MinimizeBox = false;
			return tempForm;
		}

		/// <summary>
		/// Creates a Form instance and sets the FormBorderStyle property.
		/// </summary>
		/// <param name="title">The title of the Form</param>
		/// <param name="resizable">Boolean value indicating if the Form is or not resizable</param>
		/// <param name="maximizable">Boolean value indicating if the Form displays the maximaze box</param>
		/// <returns>A new Form instance</returns>
		public static System.Windows.Forms.Form CreateForm(System.String title,bool resizable, bool maximizable)
		{
			System.Windows.Forms.Form tempForm;
			tempForm = new System.Windows.Forms.Form();
			tempForm.Text = title;
			if(resizable)
				tempForm.FormBorderStyle  = System.Windows.Forms.FormBorderStyle.Sizable;
			else
				tempForm.FormBorderStyle  = System.Windows.Forms.FormBorderStyle.FixedSingle;
			tempForm.TopLevel = false;
			tempForm.MaximizeBox = maximizable;
			tempForm.MinimizeBox = false;
			return tempForm;
		}

		/// <summary>
		/// Creates a Form instance and sets the FormBorderStyle property.
		/// </summary>
		/// <param name="title">The title of the Form</param>
		/// <param name="resizable">Boolean value indicating if the Form is or not resizable</param>
		/// <param name="maximizable">Boolean value indicating if the Form displays the maximaze box</param>
		/// <param name="minimizable">Boolean value indicating if the Form displays the minimaze box</param>
		/// <returns>A new Form instance</returns>
		public static System.Windows.Forms.Form CreateForm(System.String title,bool resizable, bool maximizable, bool minimizable)
		{
			System.Windows.Forms.Form tempForm;
			tempForm = new System.Windows.Forms.Form();
			tempForm.Text = title;
			if(resizable)
				tempForm.FormBorderStyle  = System.Windows.Forms.FormBorderStyle.Sizable;
			else
				tempForm.FormBorderStyle  = System.Windows.Forms.FormBorderStyle.FixedSingle;
			tempForm.TopLevel = false;
			tempForm.MaximizeBox = maximizable;
			tempForm.MinimizeBox = minimizable;
			return tempForm;
		}

		/// <summary>
		/// Receives a Form instance and sets the property Owner to the specified parameter.
		/// This also sets the FormBorderStyle and ShowInTaskbar properties.
		/// </summary>
		/// <param name="formInstance">Form instance to be set</param>
		/// <param name="Owner">Value for the Form property Owner</param>
		public static void SetForm(System.Windows.Forms.Form formInstance, System.Windows.Forms.Form owner)
		{
			formInstance.Owner = owner;
			formInstance.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			formInstance.ShowInTaskbar = false;
		}

		/// <summary>
		/// Receives a Form instance, sets the Text property and adds a Control.
		/// The received Control is added in the position 0 of the Form's Controls collection.
		/// </summary>
		/// <param name="formInstance">Form instance to be set</param>
		/// <param name="text">Value to be set to the Text property.</param>
		/// <param name="control">Control to add to the Controls collection.</param>
		public static void SetForm(System.Windows.Forms.Form formInstance, System.String text, System.Windows.Forms.Control control )
		{
			formInstance.Text = text;
			formInstance.Controls.Add( control );	
			formInstance.Controls.SetChildIndex( control, 0 );
		}
		
		/// <summary>
		/// Receives a Form instance and sets the property Owner to the specified parameter.
		/// Sets the FormBorderStyle and ShowInTaskbar properties.
		/// Also adds the received Control to the Form's Controls collection in position 0.
		/// </summary>
		/// <param name="formInstance">Form instance to be set</param>
		/// <param name="owner">Value for the Form property Owner</param>
		/// <param name="header">The Control to be added to the Controls collection.</param>
		public static void SetForm(System.Windows.Forms.Form formInstance, System.Windows.Forms.Form owner, System.Windows.Forms.Control header)
		{
			formInstance.Owner = owner;
			formInstance.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			formInstance.ShowInTaskbar = false;
			formInstance.Controls.Add( header );
			formInstance.Controls.SetChildIndex( header, 0 );
		}
	}
	/*******************************/
	/// <summary>
	/// Calculates the ascent of the font, using the GetCellAscent and GetEmHeight methods
	/// </summary>
	/// <param name="font">The Font instance used to obtain the Ascent</param>
	/// <returns>The ascent of the font</returns>
	public static int GetAscent(System.Drawing.Font font)
	{		
		System.Drawing.FontFamily fontFamily = font.FontFamily;
		int ascent = fontFamily.GetCellAscent(font.Style);
		int ascentPixel = (int)font.Size * ascent / fontFamily.GetEmHeight(font.Style);
		return ascentPixel;
	}

	/*******************************/
	/// <summary>
	/// Converts an angle in radians to degrees.
	/// </summary>
	/// <param name="angleInRadians">Double value of angle in radians to convert.</param>
	/// <returns>The value of the angle in degrees.</returns>
	public static double RadiansToDegrees(double angleInRadians)
	{
		double valueDegrees = 360 / (2 * System.Math.PI) ;
		return angleInRadians * valueDegrees;
	}

	/*******************************/
	/// <summary>
	/// Calculates the descent of the font, using the GetCellDescent and GetEmHeight
	/// </summary>
	/// <param name="font">The Font instance used to obtain the Descent</param>
	/// <returns>The Descent of the font </returns>
	public static int GetDescent(System.Drawing.Font font)
	{		
		System.Drawing.FontFamily fontFamily = font.FontFamily;
		int descent = fontFamily.GetCellDescent(font.Style);
		int descentPixel = (int) font.Size * descent / fontFamily.GetEmHeight(font.Style);
		return descentPixel;
	}

	/*******************************/
	/// <summary>
	/// Method used to obtain the underlying type of an object to make the correct method call.
	/// </summary>
	/// <param name="tempObject">Object instance received.</param>
	/// <param name="method">Method name to invoke.</param>
	/// <param name="parameters">Object array containing the method parameters.</param>
	/// <returns>The return value of the method called with the proper parameters.</returns>
	public static System.Object InvokeMethodAsVirtual(System.Object tempObject, System.String method, System.Object[] parameters)
	{
		System.Reflection.MethodInfo methodInfo;
		System.Type type = tempObject.GetType();
		if (parameters != null)
		{
			System.Type[] types = new System.Type[parameters.Length];
			for (int index = 0; index < parameters.Length; index++)
				types[index] = parameters[index].GetType();
			methodInfo = type.GetMethod(method, types);
		}
		else methodInfo = type.GetMethod(method);
		try
		{
			return methodInfo.Invoke(tempObject, parameters);
		}
		catch (System.Exception exception)
		{
			throw exception.InnerException;
		}
	}

	/*******************************/
	/// <summary>
	/// This class contains support methods to work with ButtonGroup.
	/// </summary>
	public class ButtonGroupSupport
	{
		/// <summary>
		/// Indicates whether an specific button is selected or not within a panel
		/// </summary>
		/// <param name="theArrayList">A reference to the penel where we will look in.</param>
		/// <param name="theButton">The button we want to know if it is selected.</param>
		/// <returns>A boolean value indicating if the button is selected within the panel.</returns>
		public static bool IsSelected(System.Collections.ArrayList theArrayList, System.Windows.Forms.ButtonBase theButton)
		{
			if(theArrayList.Contains(theButton))
			{
				return (theButton.Focused)? true : false;
			}
			return false;
		}

		/// <summary>
		/// Sets or remove the focus of an specific button within a panel.
		/// </summary>
		/// <param name="theArrayList">A reference to the penel whitch contains the button.</param>
		/// <param name="theButton">The button we want to pass the focus to.</param>
		/// <param name="theValue">A boolean value indicating whether the button should get the focus or lose it.</param>
		public static void SetSelected(System.Collections.ArrayList theArrayList, System.Windows.Forms.ButtonBase theButton, bool theValue)
		{																											 
			if(theArrayList.Contains(theButton))
			{
				while(theArrayList.GetEnumerator().MoveNext())
				{
					if(((System.Windows.Forms.Control)theArrayList.GetEnumerator().Current).Equals(theButton))
					{
						if(theValue == true)
							((System.Windows.Forms.Control)theArrayList.GetEnumerator().Current).Select();
						else
							((System.Windows.Forms.Control)theArrayList.GetEnumerator().Current).FindForm().Select();
					}
				}
			}
		}
	
		/// <summary>
		/// Gets a reference to the button witch contains the focus within a panel.
		/// </summary>
		/// <param name="theArrayList">A reference to the penel whitch contains the button.</param>
		/// <returns> A reference to the button whitch contains the focus within the panel. Null if none</returns>
		
		public static System.Windows.Forms.ButtonBase GetSelection(System.Collections.ArrayList theArrayList)
		{																											 
			while(theArrayList.GetEnumerator().MoveNext()){
					if(((System.Windows.Forms.Control)theArrayList.GetEnumerator().Current).Focused)
						return (theArrayList.GetEnumerator().Current is System.Windows.Forms.ButtonBase)?
							((System.Windows.Forms.ButtonBase)theArrayList.GetEnumerator().Current) :
							null;
				}
			return null;
		}
	}

	/*******************************/
	//Provides access to a static System.Random class instance
	static public System.Random Random = new System.Random();

	/*******************************/
	/// <summary>
	/// This class contains support methods to work with PointF struct.
	/// </summary>
	public class PointFSupport
	{
		/// <summary>
		/// Returns the distance between two specified points.
		/// </summary>
		/// <param name="point1">The first point.</param>
		/// <param name="point2">The second point.</param>
		/// <returns>Returns the distance between two specified points.</returns>
		public static double Distance(System.Drawing.PointF point1, System.Drawing.PointF point2)
		{
			return Distance(point1.X, point1.Y, point2.X, point2.Y);
		}

		/// <summary>
		/// Returns the distance between two specified points.
		/// </summary>
		/// <param name="point1">The first point.</param>
		/// <param name="x">The x-coordinate of the second point.</param>
		/// <param name="y">The y-coordinate of the second point.</param>
		/// <returns>Returns the distance between two specified points.</returns>
		public static double Distance(System.Drawing.PointF point1, float x, float y)
		{
			return Distance(point1.X, point1.Y, x, y);
		}

		/// <summary>
		/// Returns the distance between two specified points
		/// </summary>
		/// <param name="x1">The x-coordinate of the first point</param>
		/// <param name="y1">The y-coordinate of the first point</param>
		/// <param name="x2">The x-coordinate of the second point</param>
		/// <param name="y2">The y-coordinate of the second point</param>
		/// <returns>Returns the distance between two specified points</returns>
		public static double Distance(float x1, float y1, float x2, float y2)
		{
			//The Pythagorean Theorem: a^2 + b^2 = c^2
			float a = System.Math.Max(x1, x2) - System.Math.Min(x1, x2);
			float b = System.Math.Max(y1, y2) - System.Math.Min(y1, y2);
			double c = System.Math.Pow(a, 2) + System.Math.Pow(b, 2);
			return System.Math.Sqrt(c);
		}

		/// <summary>
		/// Returns the square distance between two specified points.
		/// </summary>
		/// <param name="point1">The first point.</param>
		/// <param name="point2">The second point.</param>
		/// <returns>Returns the square distance between two specified points.</returns>
		public static double DistanceSqrt(System.Drawing.PointF point1, System.Drawing.PointF point2)
		{
			return DistanceSqrt(point1.X, point1.Y, point2.X, point2.Y);
		}

		/// <summary>
		/// Returns the square distance between two specified points.
		/// </summary>
		/// <param name="point1">The first point.</param>
		/// <param name="x">The x-coordinate of the second point.</param>
		/// <param name="y">The y-coordinate of the second point.</param>
		/// <returns>Returns the square distance between two specified points.</returns>
		public static double DistanceSqrt(System.Drawing.PointF point1, float x, float y)
		{
			return DistanceSqrt(point1.X, point1.Y, x, y);
		}

		/// <summary>
		/// Returns the square distance between two specified points.
		/// </summary>
		/// <param name="x1">The x-coordinate of the first point.</param>
		/// <param name="y1">The y-coordinate of the first point.</param>
		/// <param name="x2">The x-coordinate of the second point.</param>
		/// <param name="y2">The y-coordinate of the second point.</param>
		/// <returns>Returns the square distance between two specified points.</returns>
		public static double DistanceSqrt(float x1, float y1, float x2, float y2)
		{
			return System.Math.Pow(Distance(x1, y1, x2, y2), 2);
		}
	}


	/*******************************/
	/// <summary>
	/// Gets the current System properties.
	/// </summary>
	/// <returns>The current system properties.</returns>
	public static System.Collections.Specialized.NameValueCollection GetProperties()
	{
		System.Collections.Specialized.NameValueCollection properties = new System.Collections.Specialized.NameValueCollection();
		System.Collections.ArrayList keys = new System.Collections.ArrayList(System.Environment.GetEnvironmentVariables().Keys);
		System.Collections.ArrayList values = new System.Collections.ArrayList(System.Environment.GetEnvironmentVariables().Values);
		for (int count = 0; count < keys.Count; count++)
			properties.Add(keys[count].ToString(), values[count].ToString());
		return properties;
	}


	/*******************************/
	/// <summary>
	/// Represents the methods to support some operations over files.
	/// </summary>
	public class FileSupport
	{
		/// <summary>
		/// Creates a new empty file with the specified pathname.
		/// </summary>
		/// <param name="path">The abstract pathname of the file</param>
		/// <returns>True if the file does not exist and was succesfully created</returns>
		public static bool CreateNewFile(System.IO.FileInfo path)
		{
			if (path.Exists)
			{
				return false;
			}
			else
			{
                System.IO.FileStream createdFile = path.Create();
                createdFile.Close();
				return true;
			}
		}

		/// <summary>
		/// Compares the specified object with the specified path
		/// </summary>
		/// <param name="path">An abstract pathname to compare with</param>
		/// <param name="file">An object to compare with the given pathname</param>
		/// <returns>A value indicating a lexicographically comparison of the parameters</returns>
		public static int CompareTo(System.IO.FileInfo path, System.Object file)
		{
			if( file is System.IO.FileInfo )
			{
				System.IO.FileInfo fileInfo = (System.IO.FileInfo)file;
				return path.FullName.CompareTo( fileInfo.FullName );
			}
			else
			{
				throw new System.InvalidCastException();
			}
		}

		/// <summary>
		/// Returns an array of abstract pathnames representing the files and directories of the specified path.
		/// </summary>
		/// <param name="path">The abstract pathname to list it childs.</param>
		/// <returns>An array of abstract pathnames childs of the path specified or null if the path is not a directory</returns>
		public static System.IO.FileInfo[] GetFiles(System.IO.FileInfo path)
		{
			if ( (path.Attributes & System.IO.FileAttributes.Directory) > 0 )
			{																 
				String[] fullpathnames = System.IO.Directory.GetFileSystemEntries(path.FullName);
				System.IO.FileInfo[] result = new System.IO.FileInfo[fullpathnames.Length];
				for(int i = 0; i < result.Length ; i++)
					result[i] = new System.IO.FileInfo(fullpathnames[i]);
				return result;
			}
			else return null;
		}

		/// <summary>
		/// Creates an instance of System.Uri class with the pech specified
		/// </summary>
		/// <param name="path">The abstract path name to create the Uri</param>
		/// <returns>A System.Uri instance constructed with the specified path</returns>
		public static System.Uri ToUri(System.IO.FileInfo path)
		{
			System.UriBuilder uri = new System.UriBuilder();
			uri.Path = path.FullName;
			uri.Host = String.Empty;
			uri.Scheme = System.Uri.UriSchemeFile;
			return uri.Uri;
		}

		/// <summary>
		/// Returns true if the file specified by the pathname is a hidden file.
		/// </summary>
		/// <param name="file">The abstract pathname of the file to test</param>
		/// <returns>True if the file is hidden, false otherwise</returns>
		public static bool IsHidden(System.IO.FileInfo file)
		{
			return ((file.Attributes & System.IO.FileAttributes.Hidden) > 0); 
		}

		/// <summary>
		/// Sets the read-only property of the file to true.
		/// </summary>
		/// <param name="file">The abstract path name of the file to modify</param>
		public static bool SetReadOnly(System.IO.FileInfo file)
		{
			try 
			{
				file.Attributes = file.Attributes | System.IO.FileAttributes.ReadOnly;
				return true;
			}
			catch (System.Exception exception)
			{
				String exceptionMessage = exception.Message;
				return false;
			}
		}

		/// <summary>
		/// Sets the last modified time of the specified file with the specified value.
		/// </summary>
		/// <param name="file">The file to change it last-modified time</param>
		/// <param name="date">Total number of miliseconds since January 1, 1970 (new last-modified time)</param>
		/// <returns>True if the operation succeeded, false otherwise</returns>
		public static bool SetLastModified(System.IO.FileInfo file, long date)
		{
			try 
			{
				long valueConstant = (new System.DateTime(1969, 12, 31, 18, 0, 0)).Ticks;
				file.LastWriteTime = new System.DateTime( (date * 10000L) + valueConstant );
				return true;
			}
			catch (System.Exception exception)
			{
				String exceptionMessage = exception.Message;
				return false;
			}
		}
	}
	/*******************************/
	/// <summary>Reads a number of characters from the current source Stream and writes the data to the target array at the specified index.</summary>
	/// <param name="sourceStream">The source Stream to read from.</param>
	/// <param name="target">Contains the array of characteres read from the source Stream.</param>
	/// <param name="start">The starting index of the target array.</param>
	/// <param name="count">The maximum number of characters to read from the source Stream.</param>
	/// <returns>The number of characters read. The number will be less than or equal to count depending on the data available in the source Stream. Returns -1 if the end of the stream is reached.</returns>
	public static System.Int32 ReadInput(System.IO.Stream sourceStream, sbyte[] target, int start, int count)
	{
		// Returns 0 bytes if not enough space in target
		if (target.Length == 0)
			return 0;

		byte[] receiver = new byte[target.Length];
		int bytesRead   = sourceStream.Read(receiver, start, count);

		// Returns -1 if EOF
		if (bytesRead == 0)	
			return -1;
                
		for(int i = start; i < start + bytesRead; i++)
			target[i] = (sbyte)receiver[i];
                
		return bytesRead;
	}

	/// <summary>Reads a number of characters from the current source TextReader and writes the data to the target array at the specified index.</summary>
	/// <param name="sourceTextReader">The source TextReader to read from</param>
	/// <param name="target">Contains the array of characteres read from the source TextReader.</param>
	/// <param name="start">The starting index of the target array.</param>
	/// <param name="count">The maximum number of characters to read from the source TextReader.</param>
	/// <returns>The number of characters read. The number will be less than or equal to count depending on the data available in the source TextReader. Returns -1 if the end of the stream is reached.</returns>
	public static System.Int32 ReadInput(System.IO.TextReader sourceTextReader, sbyte[] target, int start, int count)
	{
		// Returns 0 bytes if not enough space in target
		if (target.Length == 0) return 0;

		char[] charArray = new char[target.Length];
		int bytesRead = sourceTextReader.Read(charArray, start, count);

		// Returns -1 if EOF
		if (bytesRead == 0) return -1;

		for(int index=start; index<start+bytesRead; index++)
			target[index] = (sbyte)charArray[index];

		return bytesRead;
	}

	/*******************************/
	/// <summary>
	/// Checks if the giving File instance is a directory or file, and returns his Length
	/// </summary>
	/// <param name="file">The File instance to check</param>
	/// <returns>The length of the file</returns>
	public static long FileLength(System.IO.FileInfo file)
	{
		if (file.Exists)
			return file.Length;
		else 
			return 0;
	}

	/*******************************/
	/// <summary>
	/// This class contains support methods to work with GraphicsPath and Ellipses.
	/// </summary>
	public class Ellipse2DSupport
	{
		/// <summary>
		/// Creates a object and adds an ellipse to it.
		/// </summary>
		/// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.</param>
		/// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.</param>
		/// <param name="width">The width of the bounding rectangle that defines the ellipse.</param>
		/// <param name="height">The height of the bounding rectangle that defines the ellipse.</param>
		/// <returns>Returns a GraphicsPath object containing an ellipse.</returns>
		public static System.Drawing.Drawing2D.GraphicsPath CreateEllipsePath(float x, float y, float width, float height)
		{
			System.Drawing.Drawing2D.GraphicsPath ellipsePath = new System.Drawing.Drawing2D.GraphicsPath();
			ellipsePath.AddEllipse(x, y, width, height);
			return ellipsePath;
		}

		/// <summary>
		/// Resets the x-coordinate of the ellipse path contained in the specified GraphicsPath object.
		/// </summary>
		/// <param name="ellipsePath">The GraphicsPath object that will be set.</param>
		/// <param name="x">The new x-coordinate.</param>
		public static void SetX(System.Drawing.Drawing2D.GraphicsPath ellipsePath, float x)
		{
			System.Drawing.RectangleF rectangle = ellipsePath.GetBounds();
			rectangle.X = x;
			ellipsePath.Reset();
			ellipsePath.AddEllipse(rectangle);
		}

		/// <summary>
		/// Resets the y-coordinate of the ellipse path contained in the specified GraphicsPath object.
		/// </summary>
		/// <param name="ellipsePath">The GraphicsPath object that will be set.</param>
		/// <param name="y">The new y-coordinate.</param>
		public static void SetY(System.Drawing.Drawing2D.GraphicsPath ellipsePath, float y)
		{
			System.Drawing.RectangleF rectangle = ellipsePath.GetBounds();
			rectangle.Y = y;
			ellipsePath.Reset();
			ellipsePath.AddEllipse(rectangle);
		}

		/// <summary>
		/// Resets the width of the ellipse path contained in the specified GraphicsPath object.
		/// </summary>
		/// <param name="ellipsePath">The GraphicsPath object that will be set.</param>
		/// <param name="width">The new width.</param>
		public static void SetWidth(System.Drawing.Drawing2D.GraphicsPath ellipsePath, float width)
		{
			System.Drawing.RectangleF rectangle = ellipsePath.GetBounds();
			rectangle.Width = width;
			ellipsePath.Reset();
			ellipsePath.AddEllipse(rectangle);
		}

		/// <summary>
		/// Resets the height of the ellipse path contained in the specified GraphicsPath object.
		/// </summary>
		/// <param name="ellipsePath">The GraphicsPath object that will be set.</param>
		/// <param name="height">The new height.</param>
		public static void SetHeight(System.Drawing.Drawing2D.GraphicsPath ellipsePath, float height)
		{
			System.Drawing.RectangleF rectangle = ellipsePath.GetBounds();
			rectangle.Height = height;
			ellipsePath.Reset();
			ellipsePath.AddEllipse(rectangle);
		}
	}


}
