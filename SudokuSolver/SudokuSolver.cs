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
        sudoku.EvaluateGrid();
        visitedStates.Add(sudoku);
        return sudoku;
    }

    public List<Sudoku> GenerateNeighbours(Sudoku sudoku)
    {
        var neighbours = new List<Sudoku>();
        var rnd = new Random();
        var randomNumber = rnd.Next(0,9);
        for(var i = 0; i < 9; i++)
        {
            for (var j = i+1; j < 9; j++)
            {
                Sudoku neighbour = new(sudoku);
                var square = neighbour.GetSquare(randomNumber);
                if (square[i].isFixed || square[j].isFixed)
                { continue; }

                Sudoku.Swap(square, i, j);
                neighbour.PutSquare(square, randomNumber);
                if (visitedStates.Contains(neighbour))
                { continue; }

                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }

    // returns new sudoku with lowest score, and true if it is different than the previous sudoku, false if it is the same
    public static (Sudoku, bool) ChooseNext(Sudoku current, List<Sudoku> input)
    {
        var lowestScore = current.EvaluationResult;
        var result = new Sudoku(current);
        foreach(var sudoku in input)
        {
            if (sudoku.EvaluationResult < lowestScore)
            {
                result = sudoku;
                lowestScore = sudoku.EvaluationResult;
            }
        }
        return (result, result != current);
    }

    // returns a sudoku with distance amount of random swaps
    public Sudoku RandomWalk(Sudoku sudoku, int distance)
    {
        var rnd = new Random();
        var distanceLeft = distance;
        while (distanceLeft > 0)
        {
            var randomSquare = rnd.Next(0,9);
            var square = sudoku.GetSquare(randomSquare);
            var a = rnd.Next(0,9);
            var b = rnd.Next(0,9);
            if (square[a].isFixed || square[b].isFixed)
            { continue; }

            Sudoku.Swap(square, a, b);
            sudoku.PutSquare(square, randomSquare);
            distanceLeft--;
        }
        return sudoku;
    }
}
