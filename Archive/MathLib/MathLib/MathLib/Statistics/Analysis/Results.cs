using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Xsl;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Documents;

namespace MathLib.Statistics.Analysis
{
    public abstract class Results
    {
        private int decimals;

        /// <summary>
        /// Gets or sets the number of decimals.
        /// </summary>
        /// <value>The number of decimals.</value>
        public int Decimals
        {
            get { return decimals; }
            set { decimals = value; }
        }

        /// <summary>
        /// Gets an XSL-transformation that can transform the
        /// xml of the serialized <see cref="T:Results" /> object to
        /// an HTML-page that can be used to present the user the results.
        /// </summary>
        /// <remarks>
        /// Override this property with a custom XML transformation to provide
        /// users with a custom results-page.
        /// </remarks>
        /// <value>The transformation as a <see cref="T:System.Xml.Xsl.XslCompiledTransform" />.</value>
        [XmlIgnore]
        protected virtual XslCompiledTransform HtmlTransformation
        {
            get
            {
                XslCompiledTransform transform = new System.Xml.Xsl.XslCompiledTransform();
                transform.Load("Statistics\\Analysis\\ResultsToHtml.xslt");
                return transform;
            }
        }

        /// <summary>
        /// Returns an HTML-representation of the results.
        /// </summary>
        /// <returns>
        /// When creating a class that inherrits the <see cref="T:Results" /> class,
        /// please override the <see cref="M:HtmlTransformation" /> property with a
        /// custom transformation, or directly override this method, to ensure
        /// an appropriate presentation of the results to the user.
        /// </returns>
        public virtual string ToHtml()
        {
            return BuildHtml();
        }

        private string BuildHtml()
        {
            string html;

            // The stream to serialize the results to:
            MemoryStream xmlStream = new MemoryStream();

            // Serialize the results-object:
            XmlSerializer serializer =
                new XmlSerializer(this.GetType());
            XmlTextWriter xmlWriter = new XmlTextWriter(xmlStream, Encoding.Default);
            serializer.Serialize(xmlWriter, this);

            string xml = Encoding.Default.GetString(xmlStream.ToArray());

            // Transform XML to HTML and write the results to the htmlStream:
            MemoryStream htmlStream = new MemoryStream();

            XmlTextWriter htmlWriter = new XmlTextWriter(htmlStream, Encoding.Default);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            this.HtmlTransformation.Transform(xmlDocument, htmlWriter);

            // Read the HTML from the stream:
            html = Encoding.Default.GetString(htmlStream.ToArray());

            xmlWriter.Flush();
            htmlWriter.Flush();

            return html;
        }

        public abstract FlowDocument ToFlowDocument();
    }
}
