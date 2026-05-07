using System.Collections.Generic;
using System.Linq;

namespace BanglaVowelEasy.Models
{
    public class PuzzleGame
    {
        public static readonly string[] BanglaVowels =
            { "অ", "আ", "ই", "ঈ", "উ", "ঊ", "ঋ", "এ", "ঐ", "ও", "ঔ" };

        public const int Rows = 4;
        public const int Cols = 4;
        public const string Empty = "";

        // 16 cells: 11 vowels + 5 blanks
        public List<string> Board { get; set; } = new();
        public bool IsSolved { get; set; }

        public static List<string> GetSolvedBoard()
        {
            var b = new List<string>(BanglaVowels);
            for (int i = 0; i < 5; i++) b.Add(Empty);
            return b;
        }

        public static List<string> GetShuffledBoard()
        {
            var b = GetSolvedBoard();
            var rng = new System.Random();
            do
            {
                for (int i = b.Count - 1; i > 0; i--)
                {
                    int j = rng.Next(i + 1);
                    (b[i], b[j]) = (b[j], b[i]);
                }
            } while (CheckSolved(b));
            return b;
        }

        public static bool CheckSolved(List<string> board)
        {
            var solved = GetSolvedBoard();
            return board.SequenceEqual(solved);
        }
    }
}
