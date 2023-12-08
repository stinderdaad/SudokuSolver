namespace SudokuSolver;

public record struct SudokuResult(Sudoku Sudoku, bool Improved);

public class IteratedLocalSearch
{
    public List<Sudoku> GenerateNeighbours(Sudoku sudoku, HashSet<Sudoku> visitedStates)
    {
        var neighbours = new List<Sudoku>();
        var rnd = new Random();
        var randomNumber = rnd.Next(0, 9);
        for (var i = 0; i < 9; i++)
        {
            for (var j = i + 1; j < 9; j++)
            {
                Sudoku neighbour = new(sudoku);
                var square = neighbour.GetSquare(randomNumber);
                if (square[i].isFixed || square[j].isFixed)
                { continue; }

                Sudoku.Swap(square, i, j);
                neighbour.PutSquare(square, randomNumber);
                if (visitedStates.Contains(neighbour))
                { continue; }

                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }

    // returns new sudoku with lowest score, and true if it is different than the previous sudoku, false if it is the same
    public static (Sudoku, bool) ChooseNext(Sudoku current, List<Sudoku> input)
    {
        var lowestScore = current.EvaluationResult;
        var result = new Sudoku(current);
        foreach (var sudoku in input)
        {
            if (sudoku.EvaluationResult < lowestScore)
            {
                result = sudoku;
                lowestScore = sudoku.EvaluationResult;
            }
        }
        return (result, result != current);
    }

    // returns a sudoku with distance amount of random swaps
    public Sudoku RandomWalk(Sudoku sudoku, int distance)
    {
        var rnd = new Random();
        var distanceLeft = distance;
        while (distanceLeft > 0)
        {
            var randomSquare = rnd.Next(0, 9);
            var square = sudoku.GetSquare(randomSquare);
            var a = rnd.Next(0, 9);
            var b = rnd.Next(0, 9);
            if (square[a].isFixed || square[b].isFixed)
            { continue; }

            Sudoku.Swap(square, a, b);
            sudoku.PutSquare(square, randomSquare);
            distanceLeft--;
        }
        return sudoku;
    }
}