namespace SudokuSolver;
using System;

public class SudokuSolver
{
    public static void Main(string[] args)
    {
        int[,] input = new int[9, 9]
        {
            { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            { 9, 8, 7, 6, 5, 4, 3, 2, 1 },
            { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
            { 3, 1, 2, 8, 4, 5, 9, 7, 6 },
            { 6, 9, 8, 1, 2, 3, 4, 5, 7 },
            { 7, 4, 5, 9, 6, 1, 8, 3, 2 },
            { 8, 6, 4, 2, 9, 7, 5, 1, 3 },
            { 5, 7, 1, 3, 4, 8, 2, 6, 9 },
            { 2, 3, 9, 5, 1, 6, 8, 4, 7 }
        };
        Sudoku sudoku = new Sudoku(input);
        int[] row = sudoku.GetRow(0);
        int[] column = sudoku.GetColumn(0);
        int[] square = sudoku.GetSquare(0);
        Console.WriteLine("Row 0: " + string.Join(", ", row));
        Console.WriteLine("Column 0: " + string.Join(", ", column));
        Console.WriteLine("Square 0: " + string.Join(", ", square));
    }
}
