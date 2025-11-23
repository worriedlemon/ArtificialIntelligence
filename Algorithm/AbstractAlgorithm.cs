using System.Reflection;

namespace ArtificialIntelligenceIHW.Algorithm
{
    // Attribute, which indicates that this property is an algorithm option
    [AttributeUsage(AttributeTargets.Property)]
    public class AlgorithmOptionAttribute : Attribute;

    public abstract class AbstractAlgorithm
    {
        private event Action<object, bool>? UpdateGraphics;

        // Name of algorithm is a name of class
        public virtual string Name { get => Utility.AddSpaces(GetType().Name); }

        public bool Stop = false;

        // A little bit of magic for getting option names from attribute on properties
        public List<(string name, Type type)> GetOptions() => GetType()
            .GetProperties()
            .Where(x => x.GetCustomAttribute<AlgorithmOptionAttribute>() is not null)
            .Select(x => (x.Name, x.PropertyType))
            .ToList();

        public override string ToString() => Name;

        public void OnUpdateGraphics(object data, bool forced = false) => UpdateGraphics?.Invoke(data, forced);
        public void SetGraphicsEvent(Action<object, bool>? f) => UpdateGraphics += f;
        public void ResetGraphicsEvents()
        {
            if (UpdateGraphics is null) return;

            foreach (Delegate d in UpdateGraphics.GetInvocationList())
            {
                UpdateGraphics -= (Action<object, bool>)d;
            };
        }
    }
}
