using System.Collections;
using System.Collections.Generic;

namespace Roshart.Services
{
    static class ShartCollectionExtensions
    {
        public static ShartCollection ToShartCollection(this IEnumerable<Shart> sharts)
        {
            var shartCollection = new ShartCollection();
            foreach (var shart in sharts)
                shartCollection.Add(shart);
            return shartCollection;
        }
    }

    sealed class ShartCollection : IReadOnlyList<Shart>
    {
        public static readonly ShartCollection Empty = new ShartCollection();

        readonly List<Shart> sharts = new List<Shart>();
        readonly Dictionary<string, Shart> slugToShart = new Dictionary<string, Shart>();

        public int Count => sharts.Count;

        public Shart this[int index] => sharts[index];
        public Shart this[string slug] => slugToShart[slug];

        public void Add(Shart shart)
        {
            slugToShart[shart.Name] = shart;
            sharts.Add(shart);
        }

        public bool TryGetShart(string slug, out Shart shart)
            => slugToShart.TryGetValue(slug, out shart);

        public IEnumerator<Shart> GetEnumerator()
            => sharts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            =>GetEnumerator();
    }
}
