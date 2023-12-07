namespace SudokuSolver;

public class Sudoku
{
    // list of rows. Tuple contains (value, fixed).
    private readonly (int, bool)[,] _grid = new (int, bool)[9, 9];
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
                    _grid[i, j] = (value, false);
                }
                else
                {
                    _grid[i, j] = (value, true);
                }
            }
        }
    }

    // constructor om een nieuw Sudoku object te maken van een bestaande Sudoku
    public Sudoku(Sudoku sudoku)
    {
        _grid = ((int, bool)[,])sudoku._grid.Clone();
    }

    // Fills each 0 in each square with an unused number 1-9 randomly
    public void InitState()
    {
        var rnd = new Random();
        for (var i = 0; i < 9; i++)
        {
            var square = GetSquare(i);
            var presentNumbers = square.Select(item => item.Item1).Where(val => val != 0).ToList();
            var missingNumbers = Enumerable.Range(1, 9).Except(presentNumbers).ToList();
            
            for (var j = 0; j < 9; j++)
            {
                var curr = square[j].Item1;
                if (curr != 0) continue;
                var rndIndex = rnd.Next(0, missingNumbers.Count);
                square[j].Item1 = missingNumbers[rndIndex];
                missingNumbers.RemoveAt(rndIndex);
            }
            PutSquare(square, i);
        }
    }

    public (int, bool)[] GetRow(int index)
    {
        var row = new (int, bool)[9];
        for (var i = 0; i < 9; i++)
        {
            row[i] = _grid[index, i];
        }
        return row;
    }

    public (int, bool)[] GetColumn(int index)
    {
        var column = new (int, bool)[9];
        for (var i = 0; i < 9; i++)
        {
            column[i] = _grid[i, index];
        }
        return column;
    }

    private static int[] ValuesFrom((int, bool)[] array)
    {
        var values = new int[9];
        for (var i = 0; i < 9; i++)
        {
            values[i] = array[i].Item1;
        }
        return values;
    }

    // squares have index:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    public (int, bool)[] GetSquare(int index)
    {
        var square = new (int, bool)[9];
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

    public void PutSquare((int, bool)[] square, int index)
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
    }

    private void EvaluateRow(int row)
    {
        for (var i = 1; i < 10; i++)
        {
            if (!ValuesFrom(GetRow(row)).Contains(i))
            {
                _evaluation.rows[row]++;
            }
        }
    }

    private void EvaluateColumn(int col)
    {
        for (var i = 1; i < 10; i++)
        {
            if (!ValuesFrom(GetColumn(col)).Contains(i))
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

    public static (int, bool)[] Swap((int, bool)[] square, int a, int b)
    {
        var (_, fixed1) = square[a];
        var (_, fixed2) = square[b];
        if (fixed1 || fixed2)
        {
            return square;
        }

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
                Console.Write(" " + _grid[i, j].Item1 + " ");
            }
            Console.WriteLine("|");
            if (i % 3 == 2)
            {
                Console.WriteLine("-------------------------------");
            }
        }
    }
}