namespace SudokuSolver;

public record struct SudokuItem(int Number, bool IsFixed);

public class Sudoku
{
    // Each cell in the Sudoku grid is represented by a SudokuItem, which contains a number and a boolean
    // The number is the value of the cell, and the boolean indicates whether the number is fixed or not
    private readonly (int[] rows, int[] columns) _evaluation = (new int[9], new int[9]);
    public SudokuItem[,] Grid { get; set; } = new SudokuItem[9, 9];

    // Represents the evaluation value of the Sudoku
    public int EvaluationResult => _evaluation.rows.Sum() + _evaluation.columns.Sum();

    // Allow 2 sudoku's to be compared
    // 2 sudoku's are the same if they have the same numbers in the same places and the same fixed numbers
    public override bool Equals(object? obj)
    {
        var other = (Sudoku)obj;

        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                if (Grid[i, j].Number != other.Grid[i, j].Number || Grid[i, j].IsFixed != other.Grid[i, j].IsFixed)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Initialize a Sudoku object from an input array and whether or not the input is fixed
    public Sudoku(int[,] input, bool setFixed)
    {
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                var value = input[i, j];
                if (value == 0 || !setFixed)
                {
                    Grid[i, j] = new SudokuItem(value, false);
                }
                else
                {
                    Grid[i, j] = new SudokuItem(value, true);
                }
            }
        }
    }

    // Generate a new Sudoku object from an existing Sudoku object
    public Sudoku(Sudoku sudoku)
    {
        Grid = (SudokuItem[,])sudoku.Grid.Clone();
        EvaluateGrid();
    }

    // Fills each 0 in each square with an unused number 1-9 randomly
    public void InitState()
    {
        var rnd = new Random();
        for (var i = 0; i < 9; i++)
        {
            var square = GetSquare(i);
            var presentNumbers = square.Select(item => item.Number).Where(val => val != 0).ToList();
            var missingNumbers = Enumerable.Range(1, 9).Except(presentNumbers).ToList();
            
            for (var j = 0; j < 9; j++)
            {
                var curr = square[j].Number;
                if (curr != 0) continue;
                var rndIndex = rnd.Next(0, missingNumbers.Count);
                square[j].Number = missingNumbers[rndIndex];
                missingNumbers.RemoveAt(rndIndex);
            }
            PutSquare(square, i);
        }
        EvaluateGrid();
    }

    // Returns a Sudoku row at the given index
    public SudokuItem[] GetRow(int index)
    {
        var row = new SudokuItem[9];
        for (var i = 0; i < 9; i++)
        {
            row[i] = Grid[index, i];
        }
        return row;
    }
    
    // Returns a Sudoku column at the given index
    public SudokuItem[] GetColumn(int index)
    {
        var column = new SudokuItem[9];
        for (var i = 0; i < 9; i++)
        {
            column[i] = Grid[i, index];
        }
        return column;
    }

    // Returns the SudokuItem number values as an int array
    private static int[] ValuesFrom(SudokuItem[] array)
    {
        var values = new int[9];
        for (var i = 0; i < 9; i++)
        {
            values[i] = array[i].Number;
        }
        return values;
    }

    // Returns a Sudoku square at the given index (squares follow a row wise order)
    public SudokuItem[] GetSquare(int index)
    {
        var square = new SudokuItem[9];
        var x = (index / 3) * 3;
        var y = (index % 3) * 3;
        var i = 0;
        for (var j = x; j < x + 3; j++)
        {
            for (var k = y; k < y + 3; k++)
            {
                square[i] = Grid[j, k];
                i++;
            }
        }
        return square;
    }

    // Puts an input square into the grid, then updates the evaluation function
    public void PutSquare(SudokuItem[] square, int index, int a = -1, int b = -1)
    {
        var x = (index / 3) * 3;
        var y = (index % 3) * 3;
        var i = 0;
        for (var j = x; j < x + 3; j++)
        {
            for (var k = y; k < y + 3; k++)
            {
                Grid[j, k] = square[i];
                i++;
            }
        }

        // If a and b are not -1, then we know that we swapped 2 numbers in the square and we should update the evaluation
        if (a != -1 && b != -1)
        {
            UpdateEvaluation(index, a, b);
        }
    }

    // Determine the missing distinct numbers from a row, given the row index
    private void EvaluateRow(int row)
    {
        _evaluation.rows[row] = 0;
        var rowValues = ValuesFrom(GetRow(row));
        for (var i = 1; i < 10; i++)
        {
            if (!rowValues.Contains(i))
            {
                _evaluation.rows[row]++;
            }
        }
    }

    // Determine the missing distinct numbers from a column, given the column index
    private void EvaluateColumn(int col)
    {
        _evaluation.columns[col] = 0;
        var colValues = ValuesFrom(GetColumn(col));
        for (var i = 1; i < 10; i++)
        {
            if (!colValues.Contains(i))
            {
                _evaluation.columns[col]++;
            }
        }
    }

    // Evaluate the entire Sudoku grid
    private void EvaluateGrid()
    {
        for (var i = 0; i < 9; i++)
        {
            EvaluateRow(i);
            EvaluateColumn(i);
        }
    }

    // Only update the evaluation function for the rows and columns that are affected by the swap
    private void UpdateEvaluation(int index, int a, int b)
    {
        var row1 = (index / 3) * 3 + (a / 3);
        var row2 = (index / 3) * 3 + (b / 3);
        var col1 = (index % 3) * 3 + (a % 3);
        var col2 = (index % 3) * 3 + (b % 3);

        EvaluateRow(row1);
        if (row1 != row2) EvaluateRow(row2);
        EvaluateColumn(col1);
        if (col2 != col1) EvaluateColumn(col2);
    }

    // Swap 2 numbers within a square at indices a and b
    public static void Swap(SudokuItem[] square, int a, int b)
    {
        (square[a], square[b]) = (square[b], square[a]);
    }

    // Pretty print a Sudoku grid
    public void Print()
    {
        Console.WriteLine("-------------------------------");
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                if (j % 3 == 0)
                {
                    Console.Write("|");
                }
                // Fixed numbers are colored red
                if (Grid[i, j].IsFixed)
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" " + Grid[i, j].Number + " ");
                Console.ResetColor();
            }

            // Evaluation score per row
            Console.WriteLine($"|{i}: {_evaluation.rows[i]}");

            if (i % 3 == 2)
            {
                Console.WriteLine("-------------------------------");
            }
        }

        // Evaluation score per column
        Console.Write("|");
        for (var i = 0; i < 9; i++)
        {
            Console.Write($" {_evaluation.columns[i]} ");
            if ((i+1) % 3 == 0)
            {
                Console.Write("|");
            }
        }
        Console.Write("\n");
    }
}