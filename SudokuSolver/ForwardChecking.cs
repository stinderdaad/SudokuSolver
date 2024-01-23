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

                UpdateRangesFC(sudoku, i, j, ranges, false);
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

    public static void UpdateRangesFC(Sudoku sudoku, int row, int column,
        Dictionary<(int, int), int[]> ranges, bool reAdd)
    {
        // The value that needs to be deleted from the ranges
        var value = sudoku.Grid[row, column].Number;
        // Determine the square index of the current cell
        var square = (row / 3) * 3 + (column / 3);

        // Update the ranges of the current row
        for (var i = 0; i < 9; i++)
        {
            if (sudoku.Grid[row, i].IsFixed) continue; // fixed values don't have a range that can be updated
            if (i == column)continue; // don't remove the value from the cell that it was put in
            UpdateRange(value, row, i, ranges, reAdd);
        }

        // Update the ranges of the current column
        for (var i = 0; i < 9; i++)
        {
            if (sudoku.Grid[i, column].IsFixed) continue; // fixed values don't have a range that can be updated
            if (i == row) continue; // don't remove the value from the cell that it was put in
            UpdateRange(value, i, column, ranges, reAdd);
        }

        // Update the ranges of the current square
        var x = (square / 3) * 3;
        var y = (square % 3) * 3;
        for (var i = x; i < x + 3; i++)
        {
            for (var j = y; j < y + 3; j++)
            {
                if (sudoku.Grid[i, j].IsFixed) continue; // fixed values don't have a range that can be updated
                if (i == row && j == column) continue; // don't remove the value from the cell that it was put in
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
                System.Console.WriteLine($"Error: value {value} already in range");
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
                System.Console.WriteLine($"Error: value {value} not in range");
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

                UpdateRangesFC(sudoku, i, j, ranges, false);
            }
        }

        while (ContainsZeros(sudoku))
        {
            // Sort the sudoku mcv-wise
            var mcvArray = MCVRange(sudoku, ranges);
            // get most constrained cell 
            var (row, col) = mcvArray[0];

            var res = CBTStep(sudoku, ranges, ref iterationCount, ref row, ref col, true, true, mcvArray);
            if (res == (true, -1))
                return (ErrorSudoku, -1);
        }
        
        // Return the solution and the amount of value assignments it took
        return (sudoku, iterationCount);
    }

    private static bool ContainsZeros(Sudoku sudoku)
    {
        return sudoku.Grid.Cast<SudokuItem>().Any(cell => cell.Number == 0);
    }
    
    // returns an array with the keys for empty cell, sorted in ascending order of domain size (most-constrained-variable)
    private static (int, int)[] MCVRange(Sudoku sudoku, Dictionary<(int, int), int[]> ranges)
    {
        return ranges.Keys.Where(key => sudoku.Grid[key.Item1, key.Item2].Number == 0)
            .OrderBy(key => ranges[key].Length).ToArray();
    }
}
