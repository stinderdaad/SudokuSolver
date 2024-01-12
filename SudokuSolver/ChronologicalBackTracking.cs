using System.ComponentModel.Design;

namespace SudokuSolver;

public class ChronologicalBackTracking
{
    // Default Sudoku to return if no solution can be found
    private static readonly Sudoku ErrorSudoku = new Sudoku(new int[9,9], true);

    // Solve a Sudoku using the Chronological Backtracking algorithm
    public static (Sudoku, int) CBT(Sudoku sudoku, int iterationCount)
    {
        // Determine the values that each cell can have
        var range = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        
        // Traverse the sudoku left-to-right, top-to-bottom
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                var counter = 0;

                // Some numbers in the input are fixed, we should not alter these
                if (sudoku.Grid[i, j].IsFixed) continue;

                // If the number is not fixed, set it to the first possible value
                sudoku.Grid[i, j] = new SudokuItem(range[counter], false);
                iterationCount++;

                // Determine the square index of the current cell
                var sIndex = (i / 3) * 3 + (j / 3);
                
                // Check if the current sudoku layout is valid
                while (!Check(sudoku, i, j, sIndex))
                {
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
                        // Set the counter to the value of the cell we are backtracking to
                        // This works because of the way the range variable is initialised
                        // As an example, if we are backtracking to a cell with value 5, then the counter will be set to
                        // 5, which in the range variable is the index of the number 6, the next number we would try
                        counter = sudoku.Grid[i, j].Number;
                    }

                    // Update the value of the current cell
                    sudoku.Grid[i, j] = new SudokuItem(range[counter], false);
                    iterationCount++;
                }
            }
        }
        // Return the solution and the amount of value assignments it took
        return (sudoku, iterationCount);
    }
    
    // Function to check that the current sudoku layout is valid, given a row, column, and square index
    private static bool Check(Sudoku sudoku, int row, int column, int square)
    {
        // Get the values of the row, column and square
        var rowValues = sudoku.GetRow(row);
        var columnValues = sudoku.GetColumn(column);
        var squareValues = sudoku.GetSquare(square);
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
        return squareValues.Where(item => item.Number != 0).All(item => squareSet.Add(item.Number));
    }
}