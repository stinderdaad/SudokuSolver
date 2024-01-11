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
        // If you wish to use case 2, comment out line 41 and 42 and uncomment line 43 and (re)define a sudokuString variable
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
        
        /*const string sudokuString = "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9" +
                                    " 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 " +
                                    "0 9 0 0 5 0 1 0 3 0 0";*/
        
        var inputArray = SudokuSolver.GetInput();
        var numberArray = SudokuSolver.PopulateArray(inputArray);
        var sudoku = new Sudoku(numberArray, true);
        Console.WriteLine("Sudoku:");
        sudoku.Print();
        var solution = ChronologicalBackTracking.CBT(sudoku, 0);

        Console.WriteLine("Solution:");
        solution.Item1.Print();
        Console.WriteLine($"Iterations: {solution.Item2}");

        //************************P1************************//
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
