using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using MathLib.Core.Analysis;

namespace MathLib.Core.AddIns
{
    //TODO: Implement using MEF (http://www.codeplex.com/MEF)
    class PluginLoader
    {

        public static List<Type> LoadPlugs(string path)
        {
            string[] files = Directory.GetFiles(path, "*.plug");
            List<Type> plugins = new List<Type>();

            foreach (string f in files)
            {
                Assembly a = Assembly.LoadFrom(f);

                System.Type[] types = a.GetTypes();
                foreach (System.Type type in types)
                {
                    var interfaces =
                        type.GetInterfaces();

                    bool isIAnalysis = 
                        interfaces.Any<Type>((i) => (
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IAnalysis<,>)
                            ));
                    
                    if (isIAnalysis)
                    {
                        if ((type.GetCustomAttributes(typeof(PluginDisplayNameAttribute), false).Length == 1) &&
                            type.GetCustomAttributes(typeof(PluginDescriptionAttribute), false).Length == 1)
                        {
                            plugins.Add(type);
                        }
                    }
                }
            }

            return plugins;
        }
    }
}
