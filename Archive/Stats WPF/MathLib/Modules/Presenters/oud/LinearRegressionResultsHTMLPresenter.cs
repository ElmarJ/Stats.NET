using System.Xml.Xsl;
using MathLib.Core.Analysis.Presenters;
using MathLib.Modules.Analysis;

namespace MathLib.Modules.Presenters
{
    public class LinearRegressionResultsHTMLPresenter: HtmlResultPresenter<LinearRegressionResults>
    {
        public LinearRegressionResultsHTMLPresenter(LinearRegressionResults results) :
            base(results)
        {
        }

        protected override XslCompiledTransform HtmlTransformation
        {
            get
            {
                System.Xml.Xsl.XslCompiledTransform transform = new System.Xml.Xsl.XslCompiledTransform();
                transform.Load("Modules\\Presenters\\LinearRegressionResultsToHtml.xslt");
                return transform;
            }
        }
    }
}
