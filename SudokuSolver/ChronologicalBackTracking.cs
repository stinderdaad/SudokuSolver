using System.ComponentModel.Design;

namespace SudokuSolver;

public class ChronologicalBackTracking
{
    private static readonly Sudoku ErrorSudoku = new Sudoku(new int[9,9], true);

    public static (Sudoku, int) CBT(Sudoku sudoku, int iterationCount)
    {
        var range = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                var curr = sudoku.Grid[i, j].Number;
                var counter = 0;

                if (sudoku.Grid[i, j].IsFixed) continue;

                sudoku.Grid[i, j] = new SudokuItem(range[counter], false);
                iterationCount++;

                var sIndex = (i / 3) * 3 + (j / 3);
                while (!Check(sudoku, i, j, sIndex))
                {
                    counter++;
                    if (counter == 9)
                    {
                        sudoku.Grid[i, j] = new SudokuItem(0, false);

                        if (j == 0)
                        {
                            i--;
                            if (i == -1) return (ErrorSudoku, -1);
                            j = 8;
                        }
                        else
                        {
                            j--;
                        }

                        while (sudoku.Grid[i, j].IsFixed || sudoku.Grid[i, j].Number == 9)
                        {
                            if (j == 0)
                            {
                                i--;
                                if (i == -1) return (ErrorSudoku, -1);
                                j = 8;
                            }
                            else
                            {
                                j--;
                            }
                        }
                        counter = sudoku.Grid[i, j].Number;
                        //sudoku.Grid[i, j] = new SudokuItem(0, false);
                        sudoku.Grid[i, j] = new SudokuItem(range[counter], false);
                        iterationCount++;
                    }
                    else
                    {
                        sudoku.Grid[i, j] = new SudokuItem(range[counter], false);
                        iterationCount++;
                    }
                }
                //Console.WriteLine("Done with row" + j);
            }
        }
        return (sudoku, iterationCount);
    }


    private static bool Check(Sudoku sudoku, int row, int column, int square)
    {
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