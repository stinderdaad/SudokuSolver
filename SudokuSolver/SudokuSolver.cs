namespace SudokuSolver;
using System;

public class SudokuSolver
{
    HashSet<Sudoku> visitedStates = [];

    public SudokuSolver() { }

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

    private static int[,] PopulateArray(string[] inputArray) {
        // Populate the sudoku grid (input is assumed as row-wise so numbers 1-9 are in the first row,
        // 10-18 in the second etc.)
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

    public Sudoku BuildSudoku(string[] inputArray)
    {
        var sudoku = new Sudoku(PopulateArray(inputArray));
        sudoku.InitState();
        visitedStates.Add(sudoku);
        return sudoku;
    }

    public Sudoku Solve(Sudoku inputSudoku)
    {
        var result = IteratedLocalSearch.Solve(inputSudoku, visitedStates);
        return result;
    }
}
