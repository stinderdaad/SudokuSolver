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