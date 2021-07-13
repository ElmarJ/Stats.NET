
using Stats.Core.Environment;
namespace Stats.Core.Results
{
    public interface IResults: IElement, IProjectItem
    {
        ElementCollection Elements { get; }
    }
}
