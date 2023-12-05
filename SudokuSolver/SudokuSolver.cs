namespace SudokuSolver;
using System;

public class SudokuSolver
{
    public SudokuSolver()
    {
    }

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

    public static int[,] PopulateArray() { 
        // Populate the sudoku grid (input is assumed as row-wise so numbers 1-9 are in the first row,
        // 10-18 in the second etc.)
        var inputArray = GetInput();
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

    public static List<Sudoku> GenerateNeighbours(Sudoku sudoku)
    {
        List<Sudoku> neighbours = [];
        int randomNumber = 0; // TODO: make this a random number 0-8
        for(int i = 0; i < 9; i++)
        {
            for (int j = i+1; j < 9; j++)
            {
                Sudoku neighbour = new(sudoku);
                int[] square = neighbour.GetSquare(randomNumber);
                Sudoku.Swap(square, i, j); // TODO hier nog een check of een getal fixed is of niet 
                neighbour.PutSquare(square, randomNumber);
                // TODO hier nog een check of deze state al is geweest
                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }
}
