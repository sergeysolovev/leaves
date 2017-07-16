using System.Collections;
using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public class Failure : IDictionary<string, object>
    {
        private readonly IDictionary<string, object> source;

        public Failure()
            => source = new Dictionary<string, object>();

        public Failure(string errorMessage)
            => source = new Dictionary<string, object> {
                ["error"] = errorMessage
            };

        public object this[string key] { get => source[key]; set => source[key] = value; }
        public ICollection<string> Keys => source.Keys;
        public ICollection<object> Values => source.Values;
        public int Count => source.Count;
        public bool IsReadOnly => source.IsReadOnly;
        public void Add(string key, object value) => source.Add(key, value);
        public void Add(KeyValuePair<string, object> item) => source.Add(item);
        public void Clear() => source.Clear();
        public bool Contains(KeyValuePair<string, object> item) => source.Contains(item);
        public bool ContainsKey(string key) => source.ContainsKey(key);
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => source.CopyTo(array, arrayIndex);
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => source.GetEnumerator();
        public bool Remove(string key) => source.Remove(key);
        public bool Remove(KeyValuePair<string, object> item) => source.Remove(item);
        public bool TryGetValue(string key, out object value) => source.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)source).GetEnumerator();
    }
}