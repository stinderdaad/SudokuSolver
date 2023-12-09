namespace SudokuSolver;

public record struct SudokuItem(int Number, bool IsFixed);

public class Sudoku
{
    // list of rows. Tuple contains (value, fixed).
    private readonly SudokuItem[,] _grid = new SudokuItem[9, 9];
    private readonly (int[] rows, int[] columns) _evaluation = (new int[9], new int[9]);

    public int EvaluationResult => _evaluation.rows.Sum() + _evaluation.columns.Sum();

    public Sudoku(int[,] input, bool setFixed)
    {
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                var value = input[i, j];
                if (value == 0 || !setFixed)
                {
                    _grid[i, j] = new SudokuItem(value, false);
                }
                else
                {
                    _grid[i, j] = new SudokuItem(value, true);
                }
            }
        }
    }

    // constructor om een nieuw Sudoku object te maken van een bestaande Sudoku
    public Sudoku(Sudoku sudoku)
    {
        _grid = (SudokuItem[,])sudoku._grid.Clone();
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

    public SudokuItem[] GetRow(int index)
    {
        var row = new SudokuItem[9];
        for (var i = 0; i < 9; i++)
        {
            row[i] = _grid[index, i];
        }
        return row;
    }

    public SudokuItem[] GetColumn(int index)
    {
        var column = new SudokuItem[9];
        for (var i = 0; i < 9; i++)
        {
            column[i] = _grid[i, index];
        }
        return column;
    }

    private static int[] ValuesFrom(SudokuItem[] array)
    {
        var values = new int[9];
        for (var i = 0; i < 9; i++)
        {
            values[i] = array[i].Number;
        }
        return values;
    }

    // squares have index:
    // 0 1 2
    // 3 4 5
    // 6 7 8
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
                square[i] = _grid[j, k];
                i++;
            }
        }
        return square;
    }

    // puts an input square into the grid, then updates the evaluation function
    public void PutSquare(SudokuItem[] square, int index, int a = -1, int b = -1)
    {
        var x = (index / 3) * 3;
        var y = (index % 3) * 3;
        var i = 0;
        for (var j = x; j < x + 3; j++)
        {
            for (var k = y; k < y + 3; k++)
            {
                _grid[j, k] = square[i];
                i++;
            }
        }

        // als er geen waarde aan a en b zijn gegeven, dan wordt evaluatiefunctie niet uitgevoerd
        if (a != -1 && b != -1)
        {
            UpdateEvaluation(index, a, b);
        }
        //EvaluateGrid();
    }

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

    public void EvaluateGrid()
    {
        for (var i = 0; i < 9; i++)
        {
            EvaluateRow(i);
            EvaluateColumn(i);
        }
    }

    // update alleen de evaluatie voor die rijen en kolommen die veranderd zijn door de swap
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

    public static SudokuItem[] Swap(SudokuItem[] square, int a, int b)
    {
        (square[a], square[b]) = (square[b], square[a]);
        return square;
    }

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
                if (_grid[i, j].IsFixed)
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" " + _grid[i, j].Number + " ");
                Console.ResetColor();
            }

            // evaluation score per row
            Console.WriteLine($"|{i}: {_evaluation.rows[i]}");

            if (i % 3 == 2)
            {
                Console.WriteLine("-------------------------------");
            }
        }

        // evaluation score per column
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