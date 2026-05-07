using BanglaVowelEasy.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;

namespace BanglaVowelEasy.Controllers
{
    public class GameController : Controller
    {
        private const string SessionKey = "VowelEasyBoard";

        public IActionResult Index()
        {
            var board = LoadOrCreate();
            var vm = new GameViewModel
            {
                Board = board,
                IsSolved = PuzzleGame.CheckSolved(board),
                SolvedBoard = PuzzleGame.GetSolvedBoard()
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Move([FromBody] MoveRequest req)
        {
            var board = LoadOrCreate();
            int tileIdx = req.TileIndex;

            if (board[tileIdx] == PuzzleGame.Empty)
                return Json(new { board, solved = PuzzleGame.CheckSolved(board) });

            int tileRow = tileIdx / PuzzleGame.Cols;
            int tileCol = tileIdx % PuzzleGame.Cols;

            int targetIdx = -1;

            if (req.BlankIndex >= 0 && req.BlankIndex < board.Count
                && board[req.BlankIndex] == PuzzleGame.Empty)
            {
                int blankRow = req.BlankIndex / PuzzleGame.Cols;
                int blankCol = req.BlankIndex % PuzzleGame.Cols;
                bool adjacent = (tileRow == blankRow && System.Math.Abs(tileCol - blankCol) == 1)
                             || (tileCol == blankCol && System.Math.Abs(tileRow - blankRow) == 1);
                if (adjacent) targetIdx = req.BlankIndex;
            }

            if (targetIdx == -1)
            {
                int[] dr = { -1, 1, 0, 0 };
                int[] dc = { 0, 0, -1, 1 };
                for (int d = 0; d < 4; d++)
                {
                    int nr = tileRow + dr[d];
                    int nc = tileCol + dc[d];
                    if (nr < 0 || nr >= PuzzleGame.Rows || nc < 0 || nc >= PuzzleGame.Cols) continue;
                    int ni = nr * PuzzleGame.Cols + nc;
                    if (board[ni] == PuzzleGame.Empty) { targetIdx = ni; break; }
                }
            }

            if (targetIdx != -1)
            {
                (board[tileIdx], board[targetIdx]) = (board[targetIdx], board[tileIdx]);
                SaveBoard(board);
            }

            bool solved = PuzzleGame.CheckSolved(board);
            return Json(new { board, solved });
        }

        [HttpPost]
        public IActionResult NewGame()
        {
            var board = PuzzleGame.GetShuffledBoard();
            SaveBoard(board);
            return Json(new { board, solved = false });
        }

        private List<string> LoadOrCreate()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            if (json != null)
                return JsonSerializer.Deserialize<List<string>>(json)!;
            var board = PuzzleGame.GetShuffledBoard();
            SaveBoard(board);
            return board;
        }

        private void SaveBoard(List<string> board)
        {
            HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(board));
        }
    }

    public class GameViewModel
    {
        public List<string> Board { get; set; } = new();
        public bool IsSolved { get; set; }
        public List<string> SolvedBoard { get; set; } = new();
    }

    public class MoveRequest
    {
        public int TileIndex { get; set; }
        public int BlankIndex { get; set; } = -1;
    }
}
