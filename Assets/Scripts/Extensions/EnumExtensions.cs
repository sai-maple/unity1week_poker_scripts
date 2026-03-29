using System;
using System.Collections.Generic;
using System.Linq;

namespace Laughter.Poker.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        public static int Count<TEnum>() where TEnum : Enum
        {
            return GetValues<TEnum>().Count();
        }

        public static T RandomOne<T>(this IEnumerable<T> self)
        {
            return self.OrderBy(_ => Guid.NewGuid()).First();
        }
    }
}
