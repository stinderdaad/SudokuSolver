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

    // constructor om een nieuw Sudoku object te maken van een bestaande Sudoku
    public Sudoku(Sudoku sudoku)
    {
        this._grid = (int[,])sudoku._grid.Clone();
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

    public void PutSquare(int[] square, int index)
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

    private bool ArrayValid(int[] array)
    {
        for (int i = 0; i < 9; i++)
        {
            if (!(array[i] > 0 && array[i] < 10)){
                return false;
            }

            // Check for duplicates
            for (int j = i + 1; j < 9; j++)
            {
                if (array[i] == array[j])
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
            if (!ArrayValid(GetRow(i)))
            {
                return false;
            }
        }

        // Check columns
        for (int i = 0; i < 9; i++)
        {
            if (!ArrayValid(GetColumn(i)))
            {
                return false;
            }
        }

        // Check squares
        // Squares zijn per definitie goed, dus hoeft niet gecheckt
        for (int i = 0; i < 9; i++)
        {
            if (!ArrayValid(GetSquare(i)))
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
                Console.Write("----");
            }
            Console.WriteLine();
        }
    }
}