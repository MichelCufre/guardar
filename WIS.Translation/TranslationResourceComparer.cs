using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Translation
{
    public class TranslationResourceComparer : IEqualityComparer<TranslatedValue>
    {
        public bool Equals(TranslatedValue item1, TranslatedValue item2)
        {
            if (item1.Key == null && item2.Key == null)
                return true;

            else if ((item1.Key != null && item2.Key == null) ||
                    (item1.Key == null && item2.Key != null))
                return false;

            return item1.Key.Equals(item2.Key) &&
                   item1.Key.Equals(item2.Key);
        }

        public int GetHashCode(TranslatedValue item)
        {
            return item.Key.GetHashCode();
        }
    }
}
