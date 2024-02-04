using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jamarino.IntervalTree;

namespace Extras;

static public class IntervalGenerator
{
    public static Interval<long, int>[] GenerateLongInt(int length, Random rand)
    {
        var data = new Interval<long, int>[length];
        var start = rand.Next(0, 10);

        for (int i = 0; i < length; i++)
        {
            start += Math.Min(rand.Next(10), rand.Next(10));
            var end = Math.Min(rand.Next(1, 5), rand.Next(1, 5));
            end = end * end + rand.Next(10);
            data[i] = new(start, start + end, i);
        }

        Shuffle(data, rand);

        return data;
    }

    private static void Shuffle<T>(T[] data, Random rand)
    {
        var n = data.Length;
        while (n > 1)
        {
            var k = rand.Next(n--);
            var tmp = data[n];
            data[n] = data[k];
            data[k] = tmp;
        }
    }
}
