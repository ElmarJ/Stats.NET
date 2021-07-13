using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Internal
{
    internal interface IContractServicesAdapter
    {
        T Cast<T>(object o);
        bool TryCast(Type contractType, object value, out object result);
    }

    internal class ContractServices
    {
        volatile static IContractServicesAdapter adapter;

        static ContractServices()
        {
            RegisterStructuredValuesAdapter(null);
        }

        public class DefaultImplementation : IContractServicesAdapter
        {
            public T Cast<T>(object o)
            {
                return (T)o;
            }

            public bool TryCast(Type contractType, object value, out object result)
            {
                if (value == null)
                {
                    result = null;
                    return true;
                }
                if (contractType.IsInstanceOfType(value))
                {
                    result = value;
                    return true;
                }
                result = null;
                return false;
            }
        }

        public static void RegisterStructuredValuesAdapter(IContractServicesAdapter csAdapter)
        {
            if (csAdapter != null)
            {
                adapter = csAdapter;
            }
            else
            {
                adapter = new DefaultImplementation();
            }
        }

        public static T Cast<T>(object o)
        {
            return adapter.Cast<T>(o);
        }

        public static bool TryCast(Type contractType, object value, out object result)
        {
            bool cast = adapter.TryCast(contractType, value, out result);

            if (!cast && typeof(Delegate).IsAssignableFrom(contractType))
            {
                ExportedDelegate exportedDelegate = value as ExportedDelegate;
                if (exportedDelegate != null)
                {
                    result = exportedDelegate.CreateDelegate(contractType);
                    cast = (result != null);
                }
            }

            return cast;
        }
    }
}

