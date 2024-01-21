// Use Check and GenerateRangesCBT from SudokuSolver/ChronologicalBackTracking.cs
// GenerateRangesCBT can be used as initial dictionary for ForwardChecking

namespace SudokuSolver;
using static ChronologicalBackTracking;

public static class ForwardChecking
{
    // Solve a Sudoku using the Chronological Backtracking algorithm
    public static (Sudoku, int) FC(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, int iterationCount)
    {
        // First remove all fixed values from the ranges
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                if (!sudoku.Grid[i, j].IsFixed) continue;

                // Determine the square index of the current cell
                var sIndex = (i / 3) * 3 + (j / 3);
                UpdateRangesFC(sudoku, i, j, sIndex, ranges, false);
            }
        }

        // Traverse the sudoku left-to-right, top-to-bottoms
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                var res = CBTStep(sudoku, ranges, ref iterationCount, ref i, ref j, true);
                if (res == (true, -1))
                    return (ChronologicalBackTracking.ErrorSudoku, -1);
            }
        }
        // Return the solution and the amount of value assignments it took
        return (sudoku, iterationCount);
    }

    public static void UpdateRangesFC(Sudoku sudoku, int row, int column, int square,
        Dictionary<(int, int), int[]> ranges, bool reAdd)
    {
        // The value that needs to be deleted from the ranges
        var value = sudoku.Grid[row, column].Number;

        // Update the ranges of the current row
        for (var i = 0; i < 9; i++)
        {
            UpdateRange(value, row, i, ranges, reAdd);
        }

        // Update the ranges of the current column
        for (var i = 0; i < 9; i++)
        {
            UpdateRange(value, i, column, ranges, reAdd);
        }

        // Update the ranges of the current square
        var x = (square / 3) * 3;
        var y = (square % 3) * 3;
        for (var i = x; i < x + 3; i++)
        {
            for (var j = y; j < y + 3; j++)
            {
                UpdateRange(value, i, j, ranges, reAdd);
            }
        }
    }

    private static void UpdateRange(int value, int row, int column, Dictionary<(int, int), int[]> ranges, bool reAdd)
    {
        var list = ranges[(row, column)].ToList();
        if (reAdd)
        {
            if (list.Contains(value))
            {
                // Should not be possible
                System.Console.WriteLine("Error: value already in range");
            }
            else
            {
                list.Add(value);
                list.Sort();
            }
        }
        else {
            if (!list.Contains(value)) // deze check is niet nodig voor de Remove() methode, maar misschien handig voor debuggen...
            {
                // Should not be possible
                System.Console.WriteLine("Error: value not in range");
            }
            else
            {
                list.Remove(value);
            }
        }
        ranges[(row, column)] = list.ToArray();
    }

    // TODO: gaat dit nog goed met al ingevulde vakjes? of krijg je situaties waar een vakje met het correcte getal een
    // lege range heeft en daardoor backtracking veroorzaakt?
    public static bool IsEmptyRangeSomewhere(Dictionary<(int, int), int[]> ranges)
    {
        foreach (var range in ranges)
        {
            if (range.Value.Length == 0) return true;
        }
        return false;
    }
    
    public static (Sudoku, int) MCV(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, int iterationCount)
    {
        // First remove all fixed values from the ranges
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                if (!sudoku.Grid[i, j].IsFixed) continue;

                // Determine the square index of the current cell
                var sIndex = (i / 3) * 3 + (j / 3);
                UpdateRangesFC(sudoku, i, j, sIndex, ranges, false);
            }
        }

        // Traverse the sudoku mcv-wise
        var mcvArray = MCVRange(ranges);
        foreach (var cell in mcvArray)
        {
            // arbitrary assigments so that row and col can be passed by reference as required in CBTStep()
            var row = cell.Item1;
            var col = cell.Item2;
            
            var res = CBTStep(sudoku, ranges, ref iterationCount, ref row, ref col, true, true);
            if (res == (true, -1))
                return (ChronologicalBackTracking.ErrorSudoku, -1);
        }
        // Return the solution and the amount of value assignments it took
        return (sudoku, iterationCount);
    }
    
    // returns an array with the keys sorted in ascending order of domain size (most-constrained-variable)
    private static (int, int)[] MCVRange(Dictionary<(int, int), int[]> ranges)
    {
        return ranges.Keys.OrderBy(key => ranges[key].Length).ToArray();
    }
}
