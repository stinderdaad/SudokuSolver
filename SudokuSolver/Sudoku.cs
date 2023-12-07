namespace SudokuSolver;

public class Sudoku
{
    // list of rows. Tuple contains (value, fixed).
    private (int, bool)[,] _grid = new (int, bool)[9, 9];
    private (int[] rows, int[] columns) evaluation = (new int[9], new int[9]);

    public int EvaluationResult { get => evaluation.rows.Sum() + evaluation.columns.Sum(); }

    public Sudoku(int[,] input)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                int value = input[i, j];
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
        this._grid = ((int, bool)[,])sudoku._grid.Clone();
    }

    // Fills each 0 in each square with an unused number 1-9 randomly
    public void InitState()
    {
        var rnd = new Random();
        for (int i = 0; i < 9; i++)
        {
            (int, bool)[] square = GetSquare(i);
            for (int j = 0; j < 9; j++)
            {
                if (square[j].Item1 == 0)
                {
                    int randomNumber = rnd.Next(1, 10);
                    while (ValuesFrom(square).Contains(randomNumber))
                    {
                        randomNumber = rnd.Next(1, 10);
                    }
                    square[j].Item1 = randomNumber;
                }
            }
            PutSquare(square, i);
        }
    }

    public (int, bool)[] GetRow(int index)
    {
        (int, bool)[] row = new (int, bool)[9];
        for (int i = 0; i < 9; i++)
        {
            row[i] = _grid[index, i];
        }
        return row;
    }

    public (int, bool)[] GetColumn(int index)
    {
        (int, bool)[] column = new (int, bool)[9];
        for (int i = 0; i < 9; i++)
        {
            column[i] = _grid[i, index];
        }
        return column;
    }

    public int[] ValuesFrom((int, bool)[] array)
    {
        int[] values = new int[9];
        for (int i = 0; i < 9; i++)
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
        (int, bool)[] square = new (int, bool)[9];
        int x = (index / 3) * 3;
        int y = (index % 3) * 3;
        int i = 0;
        for (int j = x; j < x + 3; j++)
        {
            for (int k = y; k < y + 3; k++)
            {
                square[i] = _grid[j, k];
                i++;
            }
        }
        return square;
    }

    public void PutSquare((int, bool)[] square, int index)
    {
        int x = (index / 3) * 3;
        int y = (index % 3) * 3;
        int i = 0;
        for (int j = x; j < x + 3; j++)
        {
            for (int k = y; k < y + 3; k++)
            {
                _grid[j, k] = square[i];
                i++;
            }
        }
    }

    private void EvaluateRow(int row)
    {
        for (int i = 1; i < 10; i++)
        {
            if (!ValuesFrom(GetRow(row)).Contains(i))
            {
                evaluation.rows[row]++;
            }
        }
    }

    private void EvaluateColumn(int col)
    {
        for (int i = 1; i < 10; i++)
        {
            if (!ValuesFrom(GetColumn(col)).Contains(i))
            {
                evaluation.columns[col]++;
            }
        }
    }

    public void EvaluateGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            EvaluateRow(i);
            EvaluateColumn(i);
        }
    }

    public static (int, bool)[] Swap((int, bool)[] square, int a, int b)
    {
        (int value1, bool fixed1) = square[a];
        (int value2, bool fixed2) = square[b];
        if (fixed1 || fixed2)
        {
            return square;
        }
        else
        {
            (square[a], square[b]) = (square[b], square[a]);
            return square;
        }
    }

    public void Print()
    {
        Console.WriteLine("-------------------------------");
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
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