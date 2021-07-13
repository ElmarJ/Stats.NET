namespace Stats.Core.Results
{
    public abstract class Results : IResults
    {
        public abstract ElementCollection Elements
        {
            get;
        }

        public T Render<T>(Stats.Core.Results.Presenters.IPresenter<T> presenter)
        {
            return presenter.RenderResults(this);
        }

        public string Name
        {
            get { return "Results Name"; }
        }
    }
}
