using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agents
{
    public static class Program
    {
        static void Main(string[] args)
        {
            new Game1().Run();
        }

        public static int SearchCount<T>(this IEnumerable<T> list, Predicate<T> predicate)
        {
            int output = 0;
            foreach (T item in list)
                if (predicate.Invoke(item))
                    output++;
            return output;
        }
    }
}
