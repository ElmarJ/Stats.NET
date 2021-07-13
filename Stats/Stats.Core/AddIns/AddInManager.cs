using System;
using System.AddIn.Hosting;
using System.Collections.ObjectModel;

namespace MathLib.Core.AddIns
{
    //
    class AddInManager
    {
        public void test()
        {
            // Get path for the pipeline root.
            // Assumes that the current directory is the  
            // pipeline directory structure root directory. 
            String pipeRoot = System.Environment.CurrentDirectory;

            // Update the cache files of the
            // pipeline segments and add-ins.
            string[] warnings = AddInStore.Update(pipeRoot);

            if (warnings.Length > 0)
            {
                foreach (string warning in warnings)
                {
                    Console.WriteLine(warning);
                }
            }

            // Search for add-ins
            // specifying the host's application base, instead of a path,
            // for the FindAddIns method.

            Collection<AddInToken> tokens =
                        AddInStore.FindAddIns(typeof(Core.Analysis.IAnalysis), PipelineStoreLocation.ApplicationBase);
            
            // Zie ook: http://blogs.microsoft.co.il/blogs/bursteg/archive/2007/08/13/Build-AddIns-with-System-AddIn.aspx
        }
    }
}
