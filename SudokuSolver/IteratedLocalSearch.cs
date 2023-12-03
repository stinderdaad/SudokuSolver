namespace SudokuSolver;

public class IteratedLocalSearch
{
    // Check if each row and column contains the distinct numbers 1-9
    // return the amount of missing numbers
    private int EvaluationFunction(Sudoku sudoku)
    {
        var score = 0;
        for (var i = 0; i < 9; i++)
        {
            var row = sudoku.GetRow(i);
            var column = sudoku.GetColumn(i);
            score += row.Distinct().Count();
            score += column.Distinct().Count();
        }
        return score;
    }
}