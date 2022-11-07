using System.Collections;

namespace Redcode.Moroutines
{
    internal class EnumerableEnumerator : IEnumerable
    {
        private IEnumerator _enumerator;

        public EnumerableEnumerator(IEnumerator enumerator) => _enumerator = enumerator;

        public IEnumerator GetEnumerator() => _enumerator;
    }
}