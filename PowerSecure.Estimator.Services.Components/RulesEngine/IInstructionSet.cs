using System.Collections.ObjectModel;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public interface IInstructionSet
    {
        string Name { get; }
        string Instructions { get; }
        ReadOnlyCollection<string> Parameters { get; }
        ReadOnlyCollection<string> ChildInstructionSets { get; }
        int Sequence { get; }
    }
}