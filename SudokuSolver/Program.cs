namespace SudokuSolver;
using System;

public class Program
{
    public static void Main()
    {
        //**********************************//
        //** How to run the Sudoku Solver **//
        //
        // The code below is an example of how to run the Sudoku Solver.
        // Input can be provided in 2 ways:
        // 1. As Console input (like is the case below)
        // 2. As a string representation of a Sudoku puzzle (row for row)
        // If you wish to use case 2, comment out line 37 and 38 and uncomment line 39 and define a sudokuString variable
        //
        // BuildSudoku takes 2 arguments: the input array and a boolean that indicates whether
        //  the input is fixed or not.
        // If the boolean is set to False, then no numbers of the input will be set to fixed.
        //  This way the program could solve fully filled in sudoku's for example.
        // If the boolean is set to True, then every non-zero number of the input will be set to fixed
        //
        // The application then prints the input sudoku in the Console followed by its initial evaluation value
        //  (red numbers are fixed, values at the end of rows/columns are the respective evaluation values for those rows/columns)
        // After that we solve the sudoku using the Iterated Local Search algorithm.
        // Solve takes 3 arguments: the sudoku to solve, the sValue and the maximum number of iterations.
        // The sValue is the number of times you want to execute random-walk.
        // The maximum number of iterations is the maximum number of iterations the algorithm will
        //  run before automatically terminating.
        
        // After this is done, the solution will be printed to the Console.
        //**********************************//
        //**********************************//
        
        var solver = new SudokuSolver();
        
        var inputArray = SudokuSolver.GetInput();
        var sudoku = solver.BuildSudoku(inputArray, true);
        //var sudoku = _solver.BuildSudoku(sudokuString.Split(' '), true);

        Console.WriteLine("Sudoku:");
        sudoku.Print();
        Console.WriteLine($"Evaluation function: {sudoku.EvaluationResult}");
        var solution = solver.Solve(sudoku, 2, 10000);
        Console.WriteLine("Solution:");
        solution.solution.Print();
    }
}
