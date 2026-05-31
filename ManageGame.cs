using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit
{
    public class ManageGame
    {
        private Board GameBoard;

        public bool IsBlackTurn { get; private set; }
        public int BlackPiecesInStack { get; private set; }
        public int WhitePiecesInStack { get; private set; }
        public bool GameOver { get; private set; }
        public Troop Winner { get; private set; }

        private const int STARTING_PIECES = 8;

        public ManageGame()
        {
            GameBoard = new Board();
            IsBlackTurn = true;
            BlackPiecesInStack = STARTING_PIECES;
            WhitePiecesInStack = STARTING_PIECES;
            GameOver = false;
            Winner = Troop.NO_TROOP;
        }

        public int GetTroopAt(Location location)
        {
            return GameBoard.GetTroopAt(location);
        }

        public bool PlacePiece(Location location)
        {
            if (GameOver) return false;

            int stackCount = IsBlackTurn ? BlackPiecesInStack : WhitePiecesInStack;
            if (stackCount <= 0) return false;

            Troop currentTroop = IsBlackTurn ? Troop.BLACK_TROOP : Troop.WHITE_TROOP;

            if (!putBall(location, currentTroop)) return false;

            if (IsBlackTurn)
                BlackPiecesInStack--;
            else
                WhitePiecesInStack--;

            GameBoard.rotate();

            if (CheckWin(out Troop winner))
            {
                Winner = winner;
                GameOver = true;
            }
            else
            {
                IsBlackTurn = !IsBlackTurn;
            }

            return true;
        }

        public bool putBall(Location location, Troop troop)
        {
            int currentTroop = GameBoard.GetTroopAt(location);
            if (currentTroop != (int)Troop.NO_TROOP)
                return false;
            GameBoard.PlaceTroop(location, troop);
            return true;
        }

        public bool moveBall(Location from, Location to)
        {
            int currentTroop = GameBoard.GetTroopAt(from);
            if (currentTroop != (int)Troop.WHITE_TROOP && currentTroop != (int)Troop.BLACK_TROOP)
                return false;
            if (GameBoard.GetTroopAt(to) != (int)Troop.NO_TROOP)
                return false;
            GameBoard.PlaceTroop(from, Troop.NO_TROOP);
            GameBoard.PlaceTroop(to, (Troop)currentTroop);
            return true;
        }

        private bool CheckWin(out Troop winner)
        {
            winner = Troop.NO_TROOP;

            for (int r = 0; r < GameConsts.BOARD_SIZE; r++)
            {
                if (CheckLine(new[] {
                    new Location(r, 0), new Location(r, 1),
                    new Location(r, 2), new Location(r, 3) }, out winner))
                    return true;
            }

            for (int c = 0; c < GameConsts.BOARD_SIZE; c++)
            {
                if (CheckLine(new[] {
                    new Location(0, c), new Location(1, c),
                    new Location(2, c), new Location(3, c) }, out winner))
                    return true;
            }

            if (CheckLine(new[] {
                new Location(0, 0), new Location(1, 1),
                new Location(2, 2), new Location(3, 3) }, out winner))
                return true;

            if (CheckLine(new[] {
                new Location(0, 3), new Location(1, 2),
                new Location(2, 1), new Location(3, 0) }, out winner))
                return true;

            return false;
        }

        private bool CheckLine(Location[] locations, out Troop winner)
        {
            winner = Troop.NO_TROOP;
            int first = GameBoard.GetTroopAt(locations[0]);
            if (first == GameConsts.NO_TROOP_INT) return false;

            for (int i = 1; i < locations.Length; i++)
            {
                if (GameBoard.GetTroopAt(locations[i]) != first) return false;
            }

            winner = (Troop)first;
            return true;
        }
    }
}
