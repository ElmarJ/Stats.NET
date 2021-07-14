using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data;
using System.IO;
using Stats.Core.Data.Observations;
using Stats.Core.Data.Variables;

namespace Stats.Interoperability.SPSS
{
    public static class SpssDataLoader
    {

        public static InMemoryDataMatrix FromSpssFile(SpssLib.SpssDataset.SpssDataset spssDataSet)
        {
            var variableDictionary = new Dictionary<Core.Data.IVariable<IObservation>, SpssLib.SpssDataset.Variable>();
            var matrix = new InMemoryDataMatrix();

            foreach (var spssVar in spssDataSet.Variables)
            {
                if (spssVar.Type == SpssLib.SpssDataset.DataType.Numeric)
                {
                    var variable = matrix.Variables.Add<NumericalVariable>(spssVar.Name);
                    variableDictionary.Add(variable, spssVar);
                }
                if (spssVar.Type == SpssLib.SpssDataset.DataType.Text)
                {
                    var variable = matrix.Variables.Add<TextVariable>(spssVar.Name);
                    variableDictionary.Add(variable, spssVar);
                }
            }

            foreach (var spssRecord in spssDataSet.Records)
            {
                var record = new Record(matrix);

                foreach (var variable in matrix.Variables)
                {
                    var spssVariable = variableDictionary[variable];
                    IObservation observation = variable.NewObservation();
                    var value = spssRecord[spssVariable];

                    if (observation is NummericalObservation && value is double)
                    {
                        ((NummericalObservation)observation).Value = (double)value;
                    }
                    if (observation is TextObservation && value is string)
                    {
                        ((TextObservation)observation).Value = (string)value;
                    }

                    record[variable] = observation;
                }

                matrix.Records.Add(record);
            }
            return matrix;
        }
    }
}
