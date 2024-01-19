// Use Check and GenerateRangesCBT from SudokuSolver/ChronologicalBackTracking.cs
// GenerateRangesCBT can be used as initial dictionary for ForwardChecking
using ChronologicalBackTracking;
namespace SudokuSolver;

public class ForwardChecking
{
    // Default Sudoku to return if no solution can be found
    private static readonly Sudoku ErrorSudoku = new Sudoku(new int[9,9], true);

    // Solve a Sudoku using the Chronological Backtracking algorithm
    public static (Sudoku, int) FC(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, int iterationCount)
    {
        // First remove all fixed values from the ranges
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                if (sudoku.Grid[i, j].IsFixed)
                {
                    // Determine the square index of the current cell
                    var sIndex = (i / 3) * 3 + (j / 3);
                    UpdateRangesFC(sudoku, i, j, sIndex, ranges, false);
                }
            }
        }

        // Traverse the sudoku left-to-right, top-to-bottom
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                var counter = 0;

                // Some numbers in the input are fixed, we should not alter these
                if (sudoku.Grid[i, j].IsFixed) continue;

                // Determine the values that the current cell can have
                var range = ranges[(i,j)];

                // If the number is not fixed, set it to the first possible value
                sudoku.Grid[i, j] = new SudokuItem(range[counter], false);
                iterationCount++;

                // Determine the square index of the current cell
                var sIndex = (i / 3) * 3 + (j / 3);

                UpdateRangesFC(sudoku, i, j, sIndex, ranges, false);

                // Check if the current sudoku layout is valid
                while (!Check(sudoku, i, j, sIndex) || IsEmptyRangeSomewhere(ranges))
                {
                    // ReAdd the now unused value to the ranges
                    UpdateRangesFC(sudoku, i, j, sIndex, ranges, true);

                    // If it is not valid, we put the next possible value in the cell
                    counter++;

                    // If we have tried all possible values, we need to backtrack
                    if (counter == 9)
                    {
                        // Set the current cell back to 0
                        sudoku.Grid[i, j] = new SudokuItem(0, false);

                        // If we are in the first column, we need to go to the previous row or terminate
                        if (j == 0)
                        {
                            i--;
                            // If we reach the start of the sudoku, then no possible solution can be found
                            if (i == -1) return (ErrorSudoku, -1);
                            j = 8;
                        }
                        else
                        {
                            j--;
                        }

                        // While backtracking, we should not alter fixed numbers and set values of 9 back to 0
                        while (sudoku.Grid[i, j].IsFixed || sudoku.Grid[i, j].Number == 9)
                        {
                            if (sudoku.Grid[i, j].Number == 9 && sudoku.Grid[i,j].IsFixed == false) sudoku.Grid[i, j].Number = 0;
                            // If we are in the first column, we need to go to the previous row or terminate
                            if (j == 0)
                            {
                                i--;
                                // If we reach the start of the sudoku, then no possible solution can be found
                                if (i == -1) return (ErrorSudoku, -1);
                                j = 8;
                            }
                            else
                            {
                                j--;
                            }
                        }
                        // Set the counter to the subsequent value of the current cell
                        counter = Array.IndexOf(range, sudoku.Grid[i, j].Number) + 1;
                    }

                    // Update the value of the current cell
                    sudoku.Grid[i, j] = new SudokuItem(range[counter], false);

                    // Remove the new value from the ranges
                    UpdateRangesFC(sudoku, i, j, sIndex, ranges, false);

                    iterationCount++;
                }
            }
        }
        // Return the solution and the amount of value assignments it took
        return (sudoku, iterationCount);
    }

    public Dictionary<(int, int), int[]> UpdateRangesFC(Sudoku sudoku, int row,
        int column, int square, Dictionary<(int, int), int[]> ranges, bool reAdd)
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

    public void UpdateRange(int value, int row, int column, Dictionary<(int, int), int[]> ranges, bool reAdd)
    {
        var list = ranges[(row, column)].ToList();
        if (add)
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
            if (!list.Contains(value))
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

    public bool IsEmptyRangeSomewhere(Dictionary<(int, int), int[]> ranges)
    {
        foreach (var range in ranges)
        {
            if (range.Value.Length == 0) return true;
        }
        return false;
    }
}