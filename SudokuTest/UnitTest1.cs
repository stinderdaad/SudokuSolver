namespace SudokuTest;

using NUnit.Framework;
using SudokuSolver;

public class Tests
{
    private SudokuSolver _solver;
    private readonly string _sampleInput1 = "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9" +
                                           " 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 " +
                                           "0 9 0 0 5 0 1 0 3 0 0";
    // Represents the 5 given sample inputs
    private readonly string[] _sampleInputs = new string[]
    {
        "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0",
        "2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3",
        "0 0 0 0 0 0 9 0 7 0 0 0 4 2 0 1 8 0 0 0 0 7 0 5 0 2 6 1 0 0 9 0 4 0 0 0 0 5 0 0 0 0 0 4 0 0 0 0 5 0 7 0 0 9 9 2 0 1 0 8 0 0 0 0 3 4 0 5 9 0 0 0 5 0 7 0 0 0 0 0 0",
        "0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0",
        "0 2 0 8 1 0 7 4 0 7 0 0 0 0 3 1 0 0 0 9 0 0 0 2 8 0 5 0 0 9 0 4 0 0 8 7 4 0 0 2 0 8 0 0 3 1 6 0 0 3 0 2 0 0 3 0 2 7 0 0 0 6 0 0 0 5 6 0 0 0 0 8 0 7 6 0 5 1 0 9 0"
    };
    
    [SetUp]
    public void Setup()
    {
        _solver = new SudokuSolver();
    }

    // Generate a random sudoku
    // Set the flag fullyFilled to true if you want to generate sudoku's that don't contain 0's
    private static string GenerateSudoku(bool fullyFilled)
    {
        var sudoku = new int[9, 9];

        for (var i = 0; i < 9; i += 3)
        {
            for (var j = 0; j < 9; j += 3)
            {
                FillSquare(sudoku, i, j);
            }
        }
        if(!fullyFilled) EmptyCells(sudoku);
        return SudokuToString(sudoku);
    }

    // Fill a 3x3 square randomly with numbers 1-9
    private static void FillSquare(int[,] sudoku, int startRow, int startCol)
    {
        var rnd = new Random();
        var numbers = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9};
        var shuffledNumbers = numbers.OrderBy(a => rnd.Next()).ToList();
        var index = 0;
        for (var i = startRow; i < startRow + 3; i++)
        {
            for (var j = startCol; j < startCol + 3; j++)
            {
                sudoku[i, j] = shuffledNumbers[index];
                index++;
            }
        }
    }

    // Randomly set cells to 0
    private static void EmptyCells(int[,] sudoku)
    {
        var rnd = new Random();
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                if(rnd.Next(0, 2) == 0) sudoku[i, j] = 0;
            }
        }
    }

    // Convert a potential sudoku to a string where each number is separated by a space
    private static string SudokuToString(int[,] sudoku)
    {
        var res = "";
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                res += $"{sudoku[i, j]} ";
            }
        }

        return res.Trim();
    }

    [Test]
    // Check the algorithm n times with randomised inputs
    public void CheckNTimes()
    {
        var n = 100;
        for (var i = 0; i < n; i++)
        {
            var sudoku = _solver.BuildSudoku(GenerateSudoku(false).Split(' '), false);
            var solution = _solver.Solve(sudoku);
            Assert.That(solution.solution.EvaluationResult, Is.EqualTo(0));
        }
    }
    
    [Test]
    // Check that the input for ILS is valid
    // Meaning all squares are valid
    public void CheckInput()
    {
        foreach (var input in _sampleInputs)
        {
            var sudoku = _solver.BuildSudoku(input.Split(' '), true);
            for (var i = 0; i < 9; i++)
            {
                Assert.That(IsUnique(sudoku.GetSquare(i)));
            }
        }
    }

    [Test]
    // Check that, if the input is a already valid sudoku, then the algorithm does not perform any iterations
    public void CheckCorrectSolution()
    {
        const string validSudokuString = "5 3 4 6 7 8 9 1 2 " +
                                         "6 7 2 1 9 5 3 4 8 " +
                                         "1 9 8 3 4 2 5 6 7 " +
                                         "8 5 9 7 6 1 4 2 3 " +
                                         "4 2 6 8 5 3 7 9 1 " +
                                         "7 1 3 9 2 4 8 5 6 " +
                                         "9 6 1 5 3 7 2 8 4 " +
                                         "2 8 7 4 1 9 6 3 5 " +
                                         "3 4 5 2 8 6 1 7 9";
        var sudoku = _solver.BuildSudoku(validSudokuString.Split(' '), true);
        var solution = _solver.Solve(sudoku);
        Assert.Multiple(() =>
        {
            Assert.That(solution.solution.EvaluationResult, Is.EqualTo(0));
            Assert.That(solution.iterationCount, Is.EqualTo(0));
        });
    }

    [Test]
    // Check that, if a solution is found, the solution is a valid sudoku
    public void CheckSolutionIsValid()
    {
        foreach (var input in _sampleInputs)
        {
            var sudoku = _solver.BuildSudoku(input.Split(' '), true);
            var solution = _solver.Solve(sudoku);
            if (solution.solution.EvaluationResult == 0)
            {
                Assert.That(SolutionValid(solution.solution), Is.True);
            }
        }
    }
    // Helper function that checks if each row, column and square of a sudoku contains unique numbers
    private static bool SolutionValid(Sudoku solution)
    {
        for (var i = 0; i < 9; i++)
        {
            Assert.That(IsUnique(solution.GetRow(i)) && IsUnique(solution.GetColumn(i)) &&
                        IsUnique(solution.GetSquare(i)));
        }

        return true;
    }
    // Helper function that checks if a list of SudokuItems contains unique numbers
    private static bool IsUnique(IEnumerable<SudokuItem> items)
    {
        var numbers = items.Where(item => item.Number != 0).Select(item => item.Number);
        var enumerable = numbers as int[] ?? numbers.ToArray();
        Assert.That(enumerable, Has.Length.EqualTo(enumerable.Distinct().Count()));
        return true;
    }
    
    [Test]
    // Test that determines how often we find solutions for a given input
    public void DetermineValidSolutionPercentage()
    {
        // The amount of times we want to run the algorithm
        const int iterations = 10;
        var counter = 1;
        
        foreach (var input in _sampleInputs)
        {
            var sudoku = _solver.BuildSudoku(input.Split(' '), true);
            var validSolutions = 0;
            
            for (var i = 0; i < iterations; i++)
            {
                var solution = _solver.Solve(sudoku).solution;
                if (solution.EvaluationResult == 0) validSolutions++;
            }
        
            var percentage = (double)(validSolutions / iterations) * 100;
            Console.WriteLine("Percentage of valid solutions over " + iterations + " runs of puzzle " + counter + ": " + percentage);
            counter++;
        }
    }

    [Test]
    // Test that determines the average amount of iterations
    public void DetermineAverageIterations()
    {
        // The amount of times we want to run the algorithm
        const int iterations = 10;
        var counter = 1;

        foreach (var input in _sampleInputs)
        {
            var totalIterations = 0;
            var sudoku = _solver.BuildSudoku(input.Split(' '), true);
        
            for (var i = 0; i < iterations; i++)
            {
                var solution = _solver.Solve(sudoku);
                totalIterations += solution.iterationCount;
            }
            var averageIterations = totalIterations / iterations;
            Console.WriteLine("Average number of iterations over " + iterations + " runs of puzzle " + counter + ": " + averageIterations);
            counter++;
        }

    }

    [Test]
    // Check the memory consumption of the algorithm
    public void DetermineMemoryConsumption()
    {
        var counter = 1;
        foreach (var input in _sampleInputs)
        {
            var sudoku = _solver.BuildSudoku(input.Split(' '), true);
            var startingMemory = GC.GetTotalMemory(true);
            var solution = _solver.Solve(sudoku);
            var endingMemory = GC.GetTotalMemory(true);
            var memoryUsed = endingMemory - startingMemory;
            Console.WriteLine("Memory used for puzzle " + counter + ": " + memoryUsed);
            counter++;
        }
    }
}