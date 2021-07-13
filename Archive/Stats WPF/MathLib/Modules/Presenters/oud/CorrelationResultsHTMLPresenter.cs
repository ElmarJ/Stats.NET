using System.Xml.Xsl;
using MathLib.Core.Analysis.Presenters;
using MathLib.Modules.Analysis;

namespace MathLib.Modules.Presenters
{
    public class CorrelationResultsHTMLPresenter: HtmlResultPresenter<CorrelationResults>
    {
        public CorrelationResultsHTMLPresenter(CorrelationResults results) :
            base(results)
        {
        }

        protected override XslCompiledTransform HtmlTransformation
        {
            get
            {
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load("Modules\\Presenters\\CorrelationResultsToHtml.xslt");
                return transform;
            }
        }

    }
}
