using System;
using System.Collections.Generic;
using System.Text;
using MathLib.Statistics;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Security.Permissions;
using System.Xml.Xsl;

namespace MathLib.Statistics.Analysis
{
    public class CorrelationResults: Results, IXmlSerializable
    {
        CorrelationCollection correlations;
        NGenerics.DataStructures.Graph<Variable> graph = new NGenerics.DataStructures.Graph<Variable>(false);

        internal CorrelationResults(CorrelationCollection correlations)
        {
            this.correlations = correlations;
        }

        public CorrelationResults()
        {
        }

        [XmlIgnore]
        public CorrelationCollection Correlations
        {
            get { return correlations; }
            set { correlations = value; }
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach(KeyValuePair<Variable, Dictionary<Variable, double>> kvp in this.correlations)
            {
                writer.WriteStartElement("Variable");
                writer.WriteAttributeString("Name", kvp.Key.Name);
                foreach (KeyValuePair<Variable, double> kvp2 in kvp.Value)
                {
                    writer.WriteStartElement("Correlation");
                    writer.WriteAttributeString("Name", kvp2.Key.Name);
                    writer.WriteValue(kvp2.Value.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        #endregion

        protected override XslCompiledTransform HtmlTransformation
        {
            get
            {
                System.Xml.Xsl.XslCompiledTransform transform = new System.Xml.Xsl.XslCompiledTransform();
                transform.Load("Statistics\\Analysis\\CorrelationResultsToHtml.xslt");
                return transform;
            }
        }

        public override System.Windows.Documents.FlowDocument ToFlowDocument()
        {
            CorrelationResultsDoc doc = new CorrelationResultsDoc();

            doc.DataContext = this;
            return doc;

        }
    }
}
