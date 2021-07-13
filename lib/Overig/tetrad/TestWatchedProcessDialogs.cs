///////////////////////////////////////////////////////////////////////////////
// For information as to what this class does, see the Javadoc, below.       //
// Copyright (C) 2005 by Peter Spirtes, Richard Scheines, Joseph Ramsey,     //
// and Clark Glymour.                                                        //
//                                                                           //
// This program is free software; you can redistribute it and/or modify      //
// it under the terms of the GNU General Public License as published by      //
// the Free Software Foundation; either version 2 of the License, or         //
// (at your option) any later version.                                       //
//                                                                           //
// This program is distributed in the hope that it will be useful,           //
// but WITHOUT ANY WARRANTY; without even the implied warranty of            //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the             //
// GNU General Public License for more details.                              //
//                                                                           //
// You should have received a copy of the GNU General Public License         //
// along with this program; if not, write to the Free Software               //
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA //
///////////////////////////////////////////////////////////////////////////////
using System;
using WatchedProcess = edu.cmu.tetradapp.util.WatchedProcess;
//UPGRADE_TODO: The type 'junit.framework.TestCase' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using TestCase = junit.framework.TestCase;
namespace edu.cmu
{
	
	/// <summary> Tests to make sure the field <code>showDialog</code> in WatchProcess is set
	/// to <code>true</code>. This must be the case in order for the little progress
	/// dialogs to be displayed while algorithms are running, etc. It is convenient
	/// to set this to false while debugging, but Tetrad must not be posted with this
	/// set to false.
	/// 
	/// </summary>
	/// <author>  Joseph Ramsey
	/// </author>
	/// <version>  $Revision: 4524 $ $Date: 2006-01-06 22:37:50 -0500 (Fri, 06 Jan
	/// 2006) $
	/// </version>
	/// <seealso cref="edu.cmu.tetradapp.util.TetradSerializableUtils">
	/// </seealso>
	public class TestWatchedProcessDialogs:TestCase
	{
		public TestWatchedProcessDialogs(System.String name):base(name)
		{
		}
		
		public virtual void  testWatchedProcessDialogs()
		{
			try
			{
				//UPGRADE_TODO: The differences in the expected value  of parameters for method 'java.lang.Class.getDeclaredField'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
				System.Reflection.FieldInfo field = typeof(WatchedProcess).GetField("SHOW_DIALOG", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Static);
				
				//UPGRADE_ISSUE: Method 'java.lang.reflect.Field.getModifiers' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangreflectFieldgetModifiers'"
				int modifiers = field.getModifiers();
				//UPGRADE_ISSUE: Method 'java.lang.reflect.Modifier.isStatic' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangreflectModifier'"
				bool _static = Modifier.isStatic(modifiers);
				//UPGRADE_ISSUE: Method 'java.lang.reflect.AccessibleObject.setAccessible' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangreflectAccessibleObject'"
				field.setAccessible(true);
				
				if (!_static || !((bool) field.GetValue(null)))
				{
					throw new System.SystemException("Class does not define static " + "boolean SHOW_DIALOG = true. Please revise before " + "posting next time.");
				}
			}
			catch (System.FieldAccessException e)
			{
				throw new System.SystemException("No field showDialog in WatchedProcess!");
			}
			catch (System.UnauthorizedAccessException e)
			{
				throw new RuntimeException(e);
			}
		}
	}
}