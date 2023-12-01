namespace SudokuSolver;

public class Sudoku
{
    // list of columns
    private int[,] _grid = new int[9, 9];

    public Sudoku(int[,] input)
    {
        // Copy input values to the grid
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                _grid[i, j] = input[i, j];
            }
        }
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
            row[i] = _grid[i, index];
        }
        return row;
    }

    public int[] GetColumn(int index)
    {
        return _grid[index];
    }

    private bool RowValid(int[] row)
    {
        for (int i = 0; i < 9; i++)
        {
            if !(row[i] > 0 && row[i] < 10){
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