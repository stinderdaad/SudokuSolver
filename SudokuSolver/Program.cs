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
        // If you wish to use case 2:
        //  Uncomment the sudokuString variable and switch the comments for the inputArray variables

        // The application prints the input sudoku in the Console
        //  (red numbers are fixed, values at the end of rows/columns are the respective evaluation values for those rows/columns)
        //  The evaluation values are not applicable to P2 and are only used in P1, but they are still present in the visualisation
        // After that we can solve the sudoku using one of the 3 algorithms that are implemented.
        //  In order to run one of the algorithms, uncomment the corresponding line (and comment the others)
        //  All algorithms take the following 3 parameters: the sudoku, the ranges, and the iteration count
        //  Only the ranges for CBT should be generated, the other algorithms will generate their own ranges from an empty Dictionary
        
        // After this is done, the solution will be printed to the Console as well as the amount of value assignments
        // (iterations) it took.
        //**********************************//
        //**********************************//
        
        /*const string sudokuString = "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9" +
                                    " 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 " +
                                    "0 9 0 0 5 0 1 0 3 0 0";*/
        
        var inputArray = SudokuSolver.GetInput();
        //var inputArray = sudokuString.Split(' ');
        var numberArray = SudokuSolver.PopulateArray(inputArray);
        var sudoku = new Sudoku(numberArray, true);
        Console.WriteLine("Sudoku:");
        sudoku.Print();
        var ranges = new Dictionary<(int, int), int[]>();
        var solution = 
            ChronologicalBackTracking.CBT(sudoku, ChronologicalBackTracking.GenerateRangesCBT(sudoku,ranges), 0);
        //var solution = ForwardChecking.FC(sudoku, ranges, 0);
        //var solution = ForwardChecking.MCV(sudoku, ranges, 0);

        Console.WriteLine("Solution:");
        solution.Item1.Print();
        Console.WriteLine($"Iterations: {solution.Item2}");

        //************************P1************************//
        // var solver = new SudokuSolver();
        // var sudoku = solver.BuildSudoku(inputArray, true);
        // //var sudoku = solver.BuildSudoku(sudokuString.Split(' '), true);
        //
        // Console.WriteLine("Sudoku:");
        // sudoku.Print();
        // Console.WriteLine($"Evaluation function: {sudoku.EvaluationResult}");
        // var solution = solver.Solve(sudoku, 2, 10000);
        // Console.WriteLine("Solution:");
        // solution.solution.Print();
        // // If you wish to see the amount of iterations it took:
        // //Console.WriteLine($"Iterations: {solution.iterationCount}");
        //************************P1************************//
    }
}
