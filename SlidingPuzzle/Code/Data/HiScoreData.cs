using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.Code.Data
{
    public struct HiScoreData
    {
        public string Name;
        public int Value;

        public HiScoreData(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
