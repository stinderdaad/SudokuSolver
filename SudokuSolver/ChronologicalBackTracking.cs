﻿namespace SudokuSolver;

public static class ChronologicalBackTracking
{
    // Default Sudoku to return if no solution can be found
    public static readonly Sudoku ErrorSudoku = new Sudoku(new int[9,9], true);

    // Solve a Sudoku using the Chronological Backtracking algorithm
    public static (Sudoku, int) CBT(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, int iterationCount, bool fc = false, bool mcv = false)
    {
        // Traverse the sudoku left-to-right, top-to-bottom
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                var res = CBTStep(sudoku, ranges, ref iterationCount, ref i, ref j);
                if (res == (true, -1))
                    return (ErrorSudoku, -1);
            }
        }
        // Return the solution and the amount of value assignments it took
        return (sudoku, iterationCount);
    }

    // CBT for one cell of the sudoku, returns true if terminated and 0 with or -1 without solution, and false otherwise
    // iterationCount is passed by reference so that calling method can keep track of it
    // row and col are passed by ref so that it is consistent with the for loops in the calling methods of CBT and FC 
    public static (bool, int) CBTStep(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, ref int iterationCount,
        ref int row, ref int col, bool fc = false, bool mcv = false, (int, int)[]? array = null)
    {
        var counter = 0;

        // Some numbers in the input are fixed, we should not alter these
        if (sudoku.Grid[row, col].IsFixed) return (false, 0);

        // Determine the values that the current cell can have
        var range = ranges[(row,col)];

        // If the number is not fixed, set it to the first possible value
        sudoku.Grid[row, col] = new SudokuItem(range[counter], false);
        iterationCount++;
        
        if (fc)
            ForwardChecking.UpdateRangesFC(sudoku, row, col, ranges, false);
        
        // Check if the current sudoku layout is valid
            while (!Check(sudoku, row, col) || ForwardChecking.IsEmptyRangeSomewhere(ranges)) 
        {
            // ReAdd the now unused value to the ranges
            if (fc)
                ForwardChecking.UpdateRangesFC(sudoku, row, col, ranges, true);
            
            // If it is not valid, we put the next possible value in the cell
            counter++;
            
            // If we have tried all possible values, we need to backtrack
            range = ranges[(row, col)];
            if (counter >= range.Length) // i.e. if counter is the last number in the range
            {
                // Backtracking looks different in each algorithm
                if (mcv) 
                    (row, col) = ArrayBackTrack(sudoku, ranges, row, col, array!);
                
                else if (fc)
                    (row, col) = FCBackTrack(sudoku, ranges, row, col);
                
                else (row, col) = CBackTrack(sudoku, row, col); 
                
                // if we go all the way back to the beginning, return -1 to signal no solution found
                if (row == -1) return (true, -1);
                
                // Set the counter to the subsequent value of the current cell
                if (fc)
                    counter = Array.IndexOf(ranges[(row, col)], sudoku.Grid[row, col].Number) + 1;
                else counter = Array.IndexOf(range, sudoku.Grid[row, col].Number) + 1;
            }

            // ReAdd the now unused value to the ranges
            if (fc)
            {
                ForwardChecking.UpdateRangesFC(sudoku, row, col, ranges, true);
                // Update the value of the current cell
                sudoku.Grid[row, col] = new SudokuItem(ranges[(row, col)][counter], false);
                // Remove the new value from the ranges
                ForwardChecking.UpdateRangesFC(sudoku, row, col, ranges, false);
            }
            else 
                sudoku.Grid[row, col] = new SudokuItem(range[counter], false);
            
            iterationCount++;
        }

        return (true, 0); // succeeded, ready for next cell
    }

    private static (int, int) CBackTrack(Sudoku sudoku, int row, int col)
    {
        // Set the current cell back to 0
        sudoku.Grid[row, col] = new SudokuItem(0, false);

        // If we are in the first column, we need to go to the previous row or terminate
        if (col == 0)
        {
            row--;
            // If we reach the start of the sudoku, then no possible solution can be found
            if (row == -1) return (row, col);
            col = 8;
        }
        else
        {
            col--;
        }

        // While backtracking, we should not alter fixed numbers and set values of 9 back to 0
        var cell = sudoku.Grid[row, col];
        while (cell.IsFixed || cell.Number == 9)
        {
            if (cell is { Number: 9, IsFixed: false })
                sudoku.Grid[row, col].Number = 0;
            // If we are in the first column, we need to go to the previous row or terminate
            if (col == 0)
            {
                row--;
                // If we reach the start of the sudoku, then no possible solution can be found
                if (row == -1) return (row, col);
                col = 8;
            }
            else
            {
                col--;
            }

            cell = sudoku.Grid[row, col];
        }

        return (row, col);
    }

    private static (int, int) FCBackTrack(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, int row, int col)
    {
        // Set the current cell back to 0
        sudoku.Grid[row, col] = new SudokuItem(0, false);

        // If we are in the first column, we need to go to the previous row or terminate
        if (col == 0)
        {
            row--;
            // If we reach the start of the sudoku, then no possible solution can be found
            if (row == -1) return (row, col);
            col = 8;
        }
        else
        {
            col--;
        }

        // While backtracking, we should not alter fixed numbers and set values back to 0 if no options are left
        var cell = sudoku.Grid[row, col];
        var range = Array.Empty<int>();
        if (!cell.IsFixed)
            range = ranges[(row, col)];
        while (cell.IsFixed || Array.IndexOf(range, cell.Number) + 1 == range.Length)
        {
            if (!cell.IsFixed)
            {
                // Re-add the now unused value to the ranges
                ForwardChecking.UpdateRangesFC(sudoku, row, col, ranges, true);
                sudoku.Grid[row, col].Number = 0;
            }
            
            // If we are in the first column, we need to go to the previous row or terminate
            if (col == 0)
            {
                row--;
                // If we reach the start of the sudoku, then no possible solution can be found
                if (row == -1) return (row, col);
                col = 8;
            }
            else
            {
                col--;
            }
            
            cell = sudoku.Grid[row, col];
            if (!cell.IsFixed)
                range = ranges[(row, col)];
        }

        return (row, col);
    }

    private static (int, int) ArrayBackTrack(Sudoku sudoku, Dictionary<(int, int), int[]> ranges, int row, int col, (int, int)[] array)
    {
        // Set the current cell back to 0
        sudoku.Grid[row, col] = new SudokuItem(0, false);
        
        // get index of current cell in array of cells being iterated through
        var index = Array.IndexOf(array, (row, col));
        
        // if index < 1 then back at the beginning, so return row -1 to indicate this
        if (index < 1)
            return (-1, 0);
        
        // previous cell
        (row, col) = array[(index - 1)]; 
        
        // While backtracking, we should not alter fixed numbers and set values back to 0 if no options are left
        var cell = sudoku.Grid[row, col];
        var range = Array.Empty<int>();
        if (!cell.IsFixed)
            range = ranges[(row, col)];
        while (cell.IsFixed || Array.IndexOf(range, cell.Number) + 1 == range.Length)
        {
            if (!cell.IsFixed)
            {
                // Re-add the now unused value to the ranges
                ForwardChecking.UpdateRangesFC(sudoku, row, col, ranges, true);
                sudoku.Grid[row, col].Number = 0;
            }
            
            // get index of current cell in array of cells being iterated through
            index = Array.IndexOf(array, (row, col));
        
            // if index < 1 then back at the beginning, so return row -1 to indicate this
            if (index < 1)
                return (-1, 0);
            
            // previous cell
            (row, col) = array[(index - 1)]; 
            
            cell = sudoku.Grid[row, col];
            if (!cell.IsFixed)
                range = ranges[(row, col)];
        }

        return (row, col);
    }
    
    // Function to check that the current sudoku layout is valid, given a row, column, and square index
    private static bool Check(Sudoku sudoku, int row, int column)
    {
        // Determine the square index of the current cell
        var square = (row / 3) * 3 + (column / 3);
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
    
    

    // Generate ranges for empty cells for use in CBT
    public static Dictionary<(int, int), int[]> GenerateRangesCBT(Sudoku sudoku)
    {
        var rangesCBT = new Dictionary<(int, int), int[]>();

        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                if (sudoku.Grid[i, j].Number == 0)
                {
                    rangesCBT[(i, j)] = [1, 2, 3, 4, 5, 6, 7, 8, 9];
                }
            }
        }

        return rangesCBT;
    }
}
