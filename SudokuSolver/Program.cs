namespace SudokuSolver;
using System;

public class Program
{
    public static void Main()
    {
        // var input = new int[9, 9]
        // {
        //     { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        //     { 9, 8, 7, 6, 5, 4, 3, 2, 1 },
        //     { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
        //     { 3, 1, 2, 8, 4, 5, 9, 7, 6 },
        //     { 6, 9, 8, 1, 2, 3, 4, 5, 7 },
        //     { 7, 4, 5, 9, 6, 1, 8, 3, 2 },
        //     { 8, 6, 4, 2, 9, 7, 5, 1, 3 },
        //     { 5, 7, 1, 3, 4, 8, 2, 6, 9 },
        //     { 2, 3, 9, 5, 1, 6, 8, 4, 7 }
        // };
        // var sudoku = new Sudoku(input);
        // sudoku.Print();

        // var input = new int[9, 9]
        // {
        //     { 0, 2, 3, 0, 0, 6, 7, 0, 9 },
        //     { 9, 0, 0, 6, 5, 4, 3, 0, 1 },
        //     { 4, 5, 6, 0, 8, 9, 1, 0, 3 },
        //     { 3, 1, 0, 8, 4, 5, 0, 7, 6 },
        //     { 0, 0, 0, 1, 2, 3, 0, 5, 0 },
        //     { 7, 4, 5, 0, 0, 1, 0, 3, 2 },
        //     { 0, 6, 4, 2, 9, 7, 5, 1, 0 },
        //     { 5, 0, 0, 0, 4, 0, 2, 0, 9 },
        //     { 2, 3, 9, 5, 1, 6, 0, 4, 7 }
        // };
        // var sudoku = new Sudoku(input);
        // sudoku.Print();
        // Console.WriteLine(" Init: ");
        // sudoku.InitState();
        // sudoku.Print();
        var solver = new SudokuSolver();
        var inputArray = SudokuSolver.GetInput();
        var sudoku = solver.BuildSudoku(inputArray);
        Console.WriteLine("Sudoku:");
        sudoku.Print();
        sudoku.EvaluateGrid();
        Console.WriteLine($"Evaluation function: {sudoku.EvaluationResult}");
    }
}
