using SlidingPuzzle.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle.GameLogic
{
    public class PuzzleLogic
    {
        private Random random;

        private int[,] puzzleMap;
        private int width, height, emptyX, emptyY;

        public PuzzleLogic(int width = 5, int height = 5)
        {
            random = new Random();
            this.width = width;
            this.height = height;
            ResetPuzzle();
        }

        public void ResetPuzzle()
        {
            int[] initial = Enumerable.Range(1, width * height).ToArray();
            initial = ShuffleUtility.ShuffleArray(random, initial);
            puzzleMap = new int[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var loc = i + j * height;
                    puzzleMap[i, j] = initial[loc];
                    if (initial[loc] == height * width)
                    {
                        emptyX = i;
                        emptyY = j;
                    }
                }
            }
        }

        public bool IsPuzzleComplete()
        {
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    if (puzzleMap[i, j] != (i + 1 + j * height))
                        return false;
                }
            }
            return true;
        }

        public int[,] GetPuzzle()
        {
            return puzzleMap;
        }

        public int GetPuzzleNumberAt(int x, int y)
        {
            return puzzleMap[x, y];
        }

        public bool IsValidMove(int x, int y)
        {
            var xAmount = Math.Abs(emptyX - x);
            var yAmount = Math.Abs(emptyY - y);
            if (xAmount > 1 || yAmount > 1 || (xAmount - yAmount == 0) || (emptyX == x && emptyY == y))
                return false;
            return true;
        }

        public MoveData MakeMove(int x, int y)
        {
            var move = new MoveData();
            move.oldValue = puzzleMap[x, y];
            move.oldX = x;
            move.oldY = y;
            move.newValue = puzzleMap[emptyX, emptyY];
            move.newX = emptyX;
            move.newY = emptyY;

            var temp = puzzleMap[x, y];
            puzzleMap[x, y] = puzzleMap[emptyX, emptyY];
            puzzleMap[emptyX, emptyY] = temp;
            emptyX = x;
            emptyY = y;
            return move;
        }

        public bool CheckComplete()
        {
            if (puzzleMap[width - 1, height - 1] == width + (height - 1) * height) {
                return IsPuzzleComplete();
            }
            return false;
        }

        public bool IsEmptySpace(int x, int y)
        {
            return emptyX == x && emptyY == y;
        }
    }
}
