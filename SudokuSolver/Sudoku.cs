namespace SudokuSolver;

public record struct SudokuItem(int Number, bool IsFixed);

public class Sudoku
{
    // list of rows. Tuple contains (value, fixed).
    private readonly SudokuItem[,] _grid = new SudokuItem[9, 9];
    private readonly (int[] rows, int[] columns) _evaluation = (new int[9], new int[9]);

    public int EvaluationResult => _evaluation.rows.Sum() + _evaluation.columns.Sum();

    public Sudoku(int[,] input)
    {
        for (var i = 0; i < 9; i++)
        {
            for (var j = 0; j < 9; j++)
            {
                var value = input[i, j];
                if (value == 0)
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
    public void PutSquare(SudokuItem[] square, int index)
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
        EvaluateGrid();
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
                Console.Write(" " + _grid[i, j].Number + " ");
            }
            Console.WriteLine($"|{i}: {_evaluation.rows[i]}");
            if (i % 3 == 2)
            {
                Console.WriteLine("-------------------------------");
            }
        }
    }
}