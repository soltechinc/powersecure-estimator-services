using System;
using System.Collections.ObjectModel;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public interface IInstructionSet
    {
        string Id { get; }
        string Module { get; }
        string Name { get; }
        string Key { get; }
        string Instructions { get; }
        ReadOnlyCollection<string> Parameters { get; }
        ReadOnlyCollection<string> ChildInstructionSets { get; }
        DateTime StartDate { get; }
        DateTime CreationDate { get; }
    }
}