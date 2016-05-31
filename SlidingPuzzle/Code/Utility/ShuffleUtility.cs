using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle
{
    class ShuffleUtility
    {
        public static int[] ShuffleArray(Random random, int[] array)
        {
            var parity = 0;
            for (int i = array.Length; i > 0; i--)
            {
                int j = random.Next(i);
                int k = array[j];
                parity += Math.Abs(j - i - 1) % 2;
                array[j] = array[i - 1];
                array[i - 1] = k;
            }
            if (parity % 2 == 1)
            {
                int k = array[0];
                array[0] = array[1];
                array[1] = k;
            }
            return array;
        }
    }
}
