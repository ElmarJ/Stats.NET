using System.Text;
using System.Xml.Xsl;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Stats.Core.Results;

namespace Stats.Core.Analysis.Presenters
{
    public abstract class HtmlResultPresenter<T> where T : IResults
    {
        private T results;

        public HtmlResultPresenter(T results)
        {
            this.results = results;
        }

        protected T Results
        {
            get
            {
                return results;
            }
            
            set
            {
                results = value;
            }
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
                transform.Load("Core\\Analysis\\ResultsToHtml.xslt");
                return transform;
            }
        }

        public virtual string GenerateHtml()
        {
            string html;

            // The stream to serialize the results to:
            MemoryStream xmlStream = new MemoryStream();

            // Serialize the results-object:
            XmlSerializer serializer =
                new XmlSerializer(results.GetType());
            XmlTextWriter xmlWriter = new XmlTextWriter(xmlStream, Encoding.Default);
            serializer.Serialize(xmlWriter, results);

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
    }
}
