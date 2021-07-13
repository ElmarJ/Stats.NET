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
//UPGRADE_TODO: The type 'jdepend.framework.JDepend' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using JDepend = jdepend.framework.JDepend;
//UPGRADE_TODO: The type 'jdepend.framework.JavaPackage' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using JavaPackage = jdepend.framework.JavaPackage;
//UPGRADE_TODO: The type 'junit.framework.TestCase' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using TestCase = junit.framework.TestCase;
namespace edu.cmu
{
	
	/// <summary> Checks for package cycles.</summary>
	public class TestCyclicity:TestCase
	{
		private JDepend jdepend;
		
		public TestCyclicity(System.String name):base(name)
		{
		}
		
		public virtual void  setUp()
		{
			jdepend = new JDepend();
			System.IO.FileInfo file = new System.IO.FileInfo("./build/tetrad/classes");
			
			try
			{
				jdepend.addDirectory(file.FullName);
			}
			catch (System.IO.IOException e)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				fail(e.Message);
			}
		}
		
		public virtual void  tearDown()
		{
			jdepend = null;
		}
		
		/// <summary> Tests that a package dependency cycle does not exist for any of the
		/// analyzed packages.
		/// </summary>
		public virtual void  testAllPackagesCycle()
		{
			System.Collections.ICollection packages = jdepend.analyze();
			
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			for(Object aPackage: packages)
			{
				JavaPackage p = (JavaPackage) aPackage;
				
				if (p.containsCycle())
				{
					System.Console.Out.WriteLine("\n***Package: " + p.getName() + ".");
					System.Console.Out.WriteLine();
					System.Console.Out.WriteLine("This package participates in a package cycle. In the following " + "\nlist, for each i, some class in package i depends on some " + "\nclass in package i + 1. Please find the cycle and remove it.");
					
					//UPGRADE_TODO: Class 'java.util.LinkedList' was converted to 'System.Collections.ArrayList' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilLinkedList'"
					System.Collections.IList l = new System.Collections.ArrayList();
					p.collectCycle(l);
					System.Console.Out.WriteLine();
					
					for (int j = 0; j < l.Count; j++)
					{
						JavaPackage pack = (JavaPackage) l[j];
						System.Console.Out.WriteLine((j + 1) + ".\t" + pack.getName());
					}
					
					System.Console.Out.WriteLine();
				}
			}
			
			if (jdepend.containsCycles())
			{
				fail("Package cycle(s) found!");
			}
		}
		
		[STAThread]
		public static void  Main(System.String[] args)
		{
			junit.textui.TestRunner.run(typeof(TestCyclicity));
		}
	}
}