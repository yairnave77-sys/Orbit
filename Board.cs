using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit
{
    public struct Location
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public Location(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    public enum Troop
    {
        NO_TROOP = GameConsts.NO_TROOP_INT,
        BLACK_TROOP = GameConsts.BLACK_TROOP_INT,
        WHITE_TROOP = GameConsts.WHITE_TROOP_INT
    }
    internal class Board
    {
        private int[, ] board; 
       
        public Board()
        {
            board = new int[GameConsts.BOARD_SIZE, GameConsts.BOARD_SIZE];

            for (int i = 0; i < GameConsts.BOARD_SIZE; i++)
            {
                for (int j = 0; j < GameConsts.BOARD_SIZE; j++)
                {
                    board[i, j] = GameConsts.NO_TROOP_INT;
                }
            }
        }

        public bool PlaceTroop(Location place, Troop troop)
        {
            if (place.Row < 0 || place.Row >= board.GetLength(0) ||
                place.Col < 0 || place.Col >= board.GetLength(1))
            {
                return false;
            }
            board[place.Row, place.Col] = (int)troop;
            return true;
        }

        public int GetTroopAt(Location place)
        {
            if (place.Row < 0 || place.Row >= board.GetLength(0) ||
                place.Col < 0 || place.Col >= board.GetLength(1))
            {
                return -1;
            }
            return board[place.Row, place.Col];
        }

        public void rotate()
        {
            rotateCircle(new Location(0, 0), 3);
            rotateCircle(new Location(1, 1), 1);
        }

        private void rotateCircle(Location start, int length)
        {
            int rowChange = 1, colChange = 0;
            int row = start.Row, col = start.Col;
            int val = board[row, col], tmp;
            do
            {
                for (int i = 0; i < length; i++)
                {
                    row += rowChange;
                    col += colChange;

                    tmp = board[row, col];
                    board[row, col] = val;
                    val = tmp;
                }

                tmp = colChange;
                colChange = rowChange;
                rowChange = -tmp;

            } while (row != start.Row || col != start.Col);
            board[start.Row, start.Col] = val;
        }

        public string printBoard()
        {
            string result = "";
            for (int i = 0; i < GameConsts.BOARD_SIZE; i++)
            {
                for (int j = 0; j < GameConsts.BOARD_SIZE; j++)
                {
                    result += (board[i, j] + " ");
                }
                result += "\n";
            }
            return result;
        }
    }
}
