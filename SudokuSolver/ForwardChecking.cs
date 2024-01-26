// Use Check and GenerateRangesCBT from SudokuSolver/ChronologicalBackTracking.cs
// GenerateRangesCBT can be used as initial dictionary for ForwardChecking

namespace SudokuSolver;
using static ChronologicalBackTracking;

public static class ForwardChecking
{
    // Update the ranges to match the current assigned values
    public static void MakeConsistent(Sudoku sudoku, Dictionary<(int, int), int[]> ranges)
    {
        // Generate default ranges (1-9) for each empty cell
        ranges = GenerateRangesCBT(sudoku, ranges);
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                if (sudoku.Grid[i,j].Number == 0) continue;
                // If the cell is fixed, remove its value from the other ranges in its row, column, and square
                UpdateRangesFC(sudoku, i, j, ranges);
            }
        }
    }
    
    // Solve a Sudoku using the Forward Checking algorithm
    public static (Sudoku, int) FC(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, int iterationCount)
    {
        // First remove all fixed values from the ranges
        MakeConsistent(sudoku, ranges);

        // Traverse the sudoku left-to-right, top-to-bottom
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                // Because of the similarities with Chronological Backtracking, we can reuse the CBTStep method
                var res = CBTStep(sudoku, ranges, ref iterationCount, ref i, ref j, true);
                // If no solution can be found, we return a default sudoku
                if (res == (true, -1))
                    return (ErrorSudoku, -1);
            }
        }
        // Return the solution and the amount of value assignments it took
        return (sudoku, iterationCount);
    }

    // Update all applicable ranges whenever a new value is assigned to a cell
    public static void UpdateRangesFC(Sudoku sudoku, int row, int column,
        Dictionary<(int, int), int[]> ranges)
    {
        // The value that needs to be deleted from the ranges
        var value = sudoku.Grid[row, column].Number;
        // Determine the square index of the current cell
        var square = (row / 3) * 3 + (column / 3);

        // Update the ranges of the current row
        for (var i = 0; i < 9; i++)
        {
            // Fixed values don't have a range that can be updated
            if (sudoku.Grid[row, i].IsFixed || sudoku.Grid[row, i].Number != 0) continue; 
            // Don't remove the value from the cell that was assigned the value
            if (i == column) continue; 
            UpdateRange(value, row, i, ranges);
        }

        // Update the ranges of the current column
        for (var i = 0; i < 9; i++)
        {
            // Fixed values don't have a range that can be updated
            if (sudoku.Grid[i, column].IsFixed || sudoku.Grid[i, column].Number != 0) continue; 
            // Don't remove the value from the cell that it was put in
            if (i == row) continue; 
            UpdateRange(value, i, column, ranges);
        }

        // Update the ranges of the current square
        var x = (square / 3) * 3;
        var y = (square % 3) * 3;
        for (var i = x; i < x + 3; i++)
        {
            if (i == row) continue;
            for (var j = y; j < y + 3; j++)
            {
                if (j == column) continue;
                if (sudoku.Grid[i, j].IsFixed || sudoku.Grid[i, j].Number != 0) continue;
                UpdateRange(value, i, j, ranges);
            }
        }
    }

    // Remove a value from a range
    private static void UpdateRange(int value, int row, int column, Dictionary<(int, int), int[]> ranges)
    {
        ranges[(row, column)] = ranges[(row, column)].Where(e => e != value).ToArray();
    }
    
    // Determine if any range is empty. If so, then we should backtrack
    public static bool IsEmptyRangeSomewhere(Dictionary<(int, int), int[]> ranges)
    {
        return ranges.Any(range => range.Value.Length == 0);
    }
    
    // Solve a Sudoku using the Forward Checking algorithm using the Most-Constrained-Variable heuristic
    public static (Sudoku, int) MCV(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, int iterationCount)
    {
        // First remove all fixed values from the ranges
        MakeConsistent(sudoku, ranges);

        // Initial array that stores the cells based on their domain size
        var mcvArray = MCVRange(ranges, ranges.Keys.ToArray(), 0);
        
        for (var i = 0; i < mcvArray.Length; i++)
        {
            // Sort the sudoku MCV-wise
            mcvArray = MCVRange(ranges, mcvArray, i);
            var res = MCVStep(sudoku, ranges, ref iterationCount, i, mcvArray);
            // If no solution can be found, we return a default sudoku
            if (res == (true, -1))
                return (ErrorSudoku, -1);
            i = res.Item2;
        }
        
        // Return the solution and the amount of value assignments it took
        return (sudoku, iterationCount);
    }
    
    // Perform a step of the FC MCV algorithm
    private static (bool, int) MCVStep(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, ref int iterationCount,
        int index, (int, int)[] array)
    {
        var (row, col) = array[index];
        var counter = 0;
        // If we're backtracking, go to the next number
        if (sudoku.Grid[row, col].Number != 0)
            counter = Array.IndexOf(ranges[(row, col)], sudoku.Grid[row, col].Number) + 1;

        // Determine the values that the current cell can have
        var range = ranges[(row,col)];

        // Set it to the next possible value
        sudoku.Grid[row, col] = new SudokuItem(range[counter], false);
        iterationCount++;

        // Update the other ranges according to this new value assignment
        UpdateRangesFC(sudoku, row, col, ranges);
        var res = index;
        // Check if the current sudoku layout is valid and if any range is empty
        if (!IsEmptyRangeSomewhere(ranges)) return (true, res);
        // Otherwise we backtrack
        res = ArrayBackTrack(sudoku, ranges, index, array, counter, ref iterationCount);
        return res == -1 ? (true, -1) : (true, res);
    }
    
    // Backtrack function for the FC MCV algorithm
    private static int ArrayBackTrack(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, int index, (int, int)[] array, int counter, ref int iterationCount)
    {
        var (row, col) = array[index];
        var range = ranges[(row, col)];
        // If it is not valid, we put the next possible value in the cell
        counter++;
            
        // If we have tried all possible values, we need to go to previous index
        if (counter >= range.Length)
        {
            // Set the current cell back to 0
            sudoku.Grid[row, col] = new SudokuItem(0, false);
        
            // if index < 1 then we are back at the beginning, so return row -1 to indicate this
            if (index < 1)
                return -1;
        
            // Get the previous cell
            var newIndex = index - 1;
            (row, col) = array[newIndex];
            // Set the counter to index of the value of the new cell
            counter = Array.IndexOf(ranges[(row, col)], sudoku.Grid[row, col].Number);
            var v = ArrayBackTrack(sudoku, ranges, newIndex, array, counter, ref iterationCount);

            if (v == -1)
                return -1;
            return v;
        }
        
        // Update the value of the current cell
        sudoku.Grid[row, col] = new SudokuItem(ranges[(row, col)][counter], false);
            
        // Remove the new value from the ranges
        MakeConsistent(sudoku, ranges);
            
        iterationCount++;
        return index;
    }
    
    // Returns an array with the keys for empty cells, sorted in ascending order of domain size (Most-Constrained-Variable)
    private static (int, int)[] MCVRange(Dictionary<(int, int), int[]> ranges, (int, int)[] array, int skip = 0)
    {
        return array.Take(skip).Concat(
                array.Skip(skip).OrderBy(key => ranges[key].Length)
                ).ToArray();
    }
}
