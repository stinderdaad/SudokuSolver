namespace SudokuSolver;
using System;

public class SudokuSolver
{
    // Keep track of visited states to prevent cycling
    HashSet<Sudoku> visitedStates = [];

    // Update the visited states, used to reset the visited states between runs in the tests
    public void UpdateVisitedStates(HashSet<Sudoku> newVisitedStates)
    {
        visitedStates = newVisitedStates;
    }

    // Get user input from the Console
    public static string[] GetInput()
    {
        Console.WriteLine(
            "Please insert a Sudoku puzzle of the right format (one line of numbers separated by spaces):");
        string inputString;
        while ((inputString = Console.ReadLine()).Length < 161)
        {
            Console.WriteLine("Invalid input! Please provide 81 numbers to fill a full Sudoku puzzle.");
        }
        var inputArray = inputString.Split(' ');
        return inputArray;
    }
    
    // Populate the sudoku grid (input is assumed as row-wise so numbers 1-9 are in the first row,
    // 10-18 in the second etc.)
    public static int[,] PopulateArray(string[] inputArray) {
        var inputSudoku = new int[9, 9];
        var index = 0;
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                while (!int.TryParse(inputArray[index], out inputSudoku[i, j]) || inputSudoku[i, j] < 0 || inputSudoku[i, j] > 9)
                {
                    Console.WriteLine("Invalid input! Please enter valid numbers from 0 to 9.");
                    inputArray = GetInput();
                }
                index++;
            }
        }
        return inputSudoku;
    }

    // Build a Sudoku object from the input array, and whether the input is fixed or not
    public Sudoku BuildSudoku(string[] inputArray, bool isFixed)
    {
        var sudoku = new Sudoku(PopulateArray(inputArray), isFixed);
        sudoku.InitState();
        // Add the initial state to the visited states
        visitedStates.Add(sudoku);
        return sudoku;
    }

    // Solve the Sudoku using the Iterated Local Search algorithm
    // returns a Tuple containing the resulting sudoku and the amount of iterations it took
    public (Sudoku solution, int iterationCount) Solve(Sudoku inputSudoku, int sValue, int maxIterations)
    {
        var result = IteratedLocalSearch.Solve(inputSudoku, visitedStates, 0, sValue, maxIterations);
        return result;
    }
}
