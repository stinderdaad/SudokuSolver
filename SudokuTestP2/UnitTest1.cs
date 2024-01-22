namespace SudokuTestP2;

using NUnit.Framework;
using SudokuSolver;

public class Tests
{
    private readonly string _sampleInput1 = "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9" +
                                            " 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 " +
                                            "0 9 0 0 5 0 1 0 3 0 0";
    // Represents the 5 given sample inputs
    private readonly string[] _sampleInputs = {
        "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0",
        "2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3",
        "0 0 0 0 0 0 9 0 7 0 0 0 4 2 0 1 8 0 0 0 0 7 0 5 0 2 6 1 0 0 9 0 4 0 0 0 0 5 0 0 0 0 0 4 0 0 0 0 5 0 7 0 0 9 9 2 0 1 0 8 0 0 0 0 3 4 0 5 9 0 0 0 5 0 7 0 0 0 0 0 0",
        "0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0",
        "0 2 0 8 1 0 7 4 0 7 0 0 0 0 3 1 0 0 0 9 0 0 0 2 8 0 5 0 0 9 0 4 0 0 8 7 4 0 0 2 0 8 0 0 3 1 6 0 0 3 0 2 0 0 3 0 2 7 0 0 0 6 0 0 0 5 6 0 0 0 0 8 0 7 6 0 5 1 0 9 0"
    };
    
    [SetUp]
    public void Setup()
    {
    }

    public bool CheckValidSudoku(Sudoku sudoku)
    {
        for(var i=0; i<9; i++)
        {
            var rowValues = sudoku.GetRow(i);
            var columnValues = sudoku.GetColumn(i);
            var squareValues = sudoku.GetSquare(i);
            // Check that there are no duplicates in the row
            var rowSet = new HashSet<int>();
            if (rowValues.Where(item => item.Number != 0).Any(item => !rowSet.Add(item.Number)))
                return false;
            // Check that there are no duplicates in the column
            var columnSet = new HashSet<int>();
            if (columnValues.Where(item => item.Number != 0).Any(item => !columnSet.Add(item.Number)))
                return false;
            // Check that there are no duplicates in the square
            var squareSet = new HashSet<int>();
            if (squareValues.Where(item => item.Number != 0).All(item => !squareSet.Add(item.Number)))
                return false;
        }

        return true;
    }

    [Test]
    public void TwoSudokusSame()
    {
        var validSudoku = new int[9, 9]
        {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };
        var a = new Sudoku(validSudoku, true);
        var b = new Sudoku(validSudoku, true);
        Assert.That(a, Is.EqualTo(b));
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

        var sudoku = new Sudoku(SudokuSolver.PopulateArray(validSudokuString.Split(' ')), true);
        var ranges = ChronologicalBackTracking.GenerateRangesCBT(sudoku);
        var solution = ChronologicalBackTracking.CBT(sudoku, ranges, 0);
        Assert.That(solution.Item2, Is.EqualTo(0));
    }

    [Test]
    // Check that CBT can solve each of the 5 sample inputs
    public void CBTSolve()
    {
        foreach (var input in _sampleInputs)
        {
            var sudoku = new Sudoku(SudokuSolver.PopulateArray(input.Split(' ')), true);
            var ranges = ChronologicalBackTracking.GenerateRangesCBT(sudoku);
            var solution = ChronologicalBackTracking.CBT(sudoku, ranges, 0);
            Assert.Multiple(() =>
            {
                Assert.That(solution.Item2, Is.Not.EqualTo(-1));
                Assert.That(CheckValidSudoku(solution.Item1));
            });
        }
    }

    [Test]
    // Determine avg amount of iterations for CBT to solve each of the 5 sample inputs
    public void CBTAvgIt()
    {
        const int runs = 5;
        foreach (var input in _sampleInputs)
        {
            for (var i = 0; i < runs; i++)
            {
                var sudoku = new Sudoku(SudokuSolver.PopulateArray(input.Split(' ')), true);
                var ranges = ChronologicalBackTracking.GenerateRangesCBT(sudoku);
                var solution = ChronologicalBackTracking.CBT(sudoku, ranges, 0);
                Assert.Multiple(() =>
                {
                    Assert.That(solution.Item2, Is.Not.EqualTo(-1));
                    Assert.That(CheckValidSudoku(solution.Item1));
                });
                Console.WriteLine("Run " + i + " took " + solution.Item2 + " iterations");
            }
        }
    }
    
    [Test]
    // Check that FC can solve each of the 5 sample inputs
    public void FCSolve()
    {
        foreach (var input in _sampleInputs)
        {
            var sudoku = new Sudoku(SudokuSolver.PopulateArray(input.Split(' ')), true);
            var ranges = ChronologicalBackTracking.GenerateRangesCBT(sudoku);
            var solution = ForwardChecking.FC(sudoku, ranges, 0);
            Assert.Multiple(() =>
            {
                Assert.That(solution.Item2, Is.Not.EqualTo(-1));
                Assert.That(CheckValidSudoku(solution.Item1));
            });
        }
    }
    [Test]
    // Determine avg amount of iterations for CBT to solve each of the 5 sample inputs
    public void FCAvgIt()
    {
        const int runs = 5;
        foreach (var input in _sampleInputs)
        {
            for (var i = 0; i < runs; i++)
            {
                var sudoku = new Sudoku(SudokuSolver.PopulateArray(input.Split(' ')), true);
                var ranges = ChronologicalBackTracking.GenerateRangesCBT(sudoku);
                var solution = ForwardChecking.FC(sudoku, ranges, 0);
                Assert.Multiple(() =>
                {
                    Assert.That(solution.Item2, Is.Not.EqualTo(-1));
                    Assert.That(CheckValidSudoku(solution.Item1));
                });
                Console.WriteLine("Run " + i + " took " + solution.Item2 + " iterations");
            }
        }
    }
    
    [Test]
    // Check that FC with MCV can solve each of the 5 sample inputs
    public void MCVSolve()
    {
        foreach (var input in _sampleInputs)
        {
            var sudoku = new Sudoku(SudokuSolver.PopulateArray(input.Split(' ')), true);
            var ranges = ChronologicalBackTracking.GenerateRangesCBT(sudoku);
            var solution = ForwardChecking.MCV(sudoku, ranges, 0);
            Assert.Multiple(() =>
            {
                Assert.That(solution.Item2, Is.Not.EqualTo(-1));
                Assert.That(CheckValidSudoku(solution.Item1));
            });
        }
    }
    [Test]
    // Determine avg amount of iterations for CBT to solve each of the 5 sample inputs
    public void MCVAvgIt()
    {
        const int runs = 5;
        foreach (var input in _sampleInputs)
        {
            for (var i = 0; i < runs; i++)
            {
                var sudoku = new Sudoku(SudokuSolver.PopulateArray(input.Split(' ')), true);
                var ranges = ChronologicalBackTracking.GenerateRangesCBT(sudoku);
                var solution = ForwardChecking.MCV(sudoku, ranges, 0);
                Assert.Multiple(() =>
                {
                    Assert.That(solution.Item2, Is.Not.EqualTo(-1));
                    Assert.That(CheckValidSudoku(solution.Item1));
                });
                Console.WriteLine("Run " + i + " took " + solution.Item2 + " iterations");
            }
        }
    }
}