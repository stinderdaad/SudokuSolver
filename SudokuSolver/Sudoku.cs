namespace SudokuSolver;

public class Sudoku
{
    // list of rows
    private int[,] _grid = new int[9, 9];

    public Sudoku(int[,] input)
    {
        _grid = input;
        // for (int i = 0; i < 9; i++)
        // {
        //     for (int j = 0; j < 9; j++)
        //     {
        //         _grid[i, j] = input[i, j];
        //     }
        // }
    }

    // public int[,] GetGrid()
    // {
    //     return _grid;
    // }

    public int[] GetRow(int index)
    {
        int[] row = new int[9];
        for (int i = 0; i < 9; i++)
        {
            row[i] = _grid[index, i];
        }
        return row;
    }

    public int[] GetColumn(int index)
    {
        //return _grid[index]; why wouldn't this work?
        int[] column = new int[9];
        for (int i = 0; i < 9; i++)
        {
            column[i] = _grid[i, index];
        }
        return column;
    }

    // squares have index:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    public int[] GetSquare(int index)
    {
        int[] square = new int[9];
        int x = (index % 3) * 3;
        int y = (index / 3) * 3;
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

    private bool RowValid(int[] row)
    {
        for (int i = 0; i < 9; i++)
        {
            if (!(row[i] > 0 && row[i] < 10)){
                return false;
            }

            // Check for duplicates
            for (int j = i + 1; j < 9; j++)
            {
                if (row[i] == row[j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool Valid()
    {
        // Check rows
        for (int i = 0; i < 9; i++)
        {
            if (!RowValid(GetRow(i)))
            {
                return false;
            }
        }

        // Check columns
        for (int i = 0; i < 9; i++)
        {
            if (!RowValid(GetColumn(i)))
            {
                return false;
            }
        }

        return true;
    }

    public void Print()
    {
        for (int i = 0; i < 9; i++)
        {
            Console.Write("| ");
            for (int j = 0; j < 9; j++)
            {
                Console.Write(_grid[i, j] + " | ");
            }
            Console.WriteLine();
            for (int k = 0; k < 9; k++)
            {
                Console.Write("--");
            }
            Console.WriteLine();
        }
    }
}