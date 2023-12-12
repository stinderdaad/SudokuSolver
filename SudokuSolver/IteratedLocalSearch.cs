namespace SudokuSolver;

public record struct SudokuResult(Sudoku Sudoku, bool Improved);

public class IteratedLocalSearch
{
    public static (Sudoku solution, int iterationCount) Solve(Sudoku inputSudoku, HashSet<Sudoku> visitedStates, int iterationCount, int sValue, int maxIterations)
    {
        var result = new SudokuResult(inputSudoku, true);
        var counter = 0; // counter telt hoe vaak achter elkaar er géén verbetering is. Als dat te vaak gebeurt is er een random walk
        
        // loops while result improves and solution is not yet found
        while (counter < 9 && result.Sudoku.EvaluationResult != 0 && iterationCount < maxIterations)
        {
            result = Step(result.Sudoku, visitedStates);
            iterationCount++;
            if (!result.Improved) 
            { counter++; }
            else counter = 0;

            // debugging
            //Console.WriteLine($"New Sudoku score: {result.Sudoku.EvaluationResult}");
        }
        
        // if loop finishes, then either solution is found, or no improvement possible
        if (result.Sudoku.EvaluationResult == 0 || iterationCount >= maxIterations)
        {
            return (result.Sudoku, iterationCount);
        }
        else
        {
            result.Sudoku = RandomWalk(result.Sudoku, sValue);

            // debugging
            //Console.WriteLine("random walk");
            //Console.WriteLine("Score after walk: " +  result.Sudoku.EvaluationResult);

            (result.Sudoku, iterationCount) = Solve(result.Sudoku, visitedStates, iterationCount, sValue, maxIterations); // recursion
            return (result.Sudoku, iterationCount);
        }
    }

    private static SudokuResult Step(Sudoku inputSudoku, HashSet<Sudoku> visitedStates)
    {
        var neighbours = GenerateNeighbours(inputSudoku, visitedStates);
        var bestNeighbour = ChooseNext(inputSudoku, neighbours);
        visitedStates.Add(bestNeighbour.Sudoku);
        return bestNeighbour;
    }

    public static List<Sudoku> GenerateNeighbours(Sudoku sudoku, HashSet<Sudoku> visitedStates)
    {
        var neighbours = new List<Sudoku>();
        var rnd = new Random();
        var randomNumber = rnd.Next(0, 9);
        for (var i = 0; i < 9; i++)
        {
            for (var j = i + 1; j < 9; j++)
            {
                Sudoku neighbour = new(sudoku);
                var square = sudoku.GetSquare(randomNumber);
                if (square[i].IsFixed || square[j].IsFixed)
                { continue; }

                Sudoku.Swap(square, i, j);
                neighbour.PutSquare(square, randomNumber, i, j);
                if (visitedStates.Contains(neighbour))
                { continue; }

                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }

    // returns new sudoku with lowest score, and true if it is different than the previous sudoku, false if it is the same
    public static SudokuResult ChooseNext(Sudoku current, List<Sudoku> input)
    {
        var lowestScore = current.EvaluationResult;
        var result = new Sudoku(current);
        foreach (var sudoku in input)
        {
            if (sudoku.EvaluationResult <= lowestScore)
            {
                result = sudoku;
                lowestScore = sudoku.EvaluationResult;
            }
        }
        return new SudokuResult(result, result.EvaluationResult != current.EvaluationResult);
    }

    // returns a sudoku with distance amount of random swaps
    public static Sudoku RandomWalk(Sudoku sudoku, int distance)
    {
        var rnd = new Random();
        var distanceLeft = distance;
        while (distanceLeft > 0)
        {
            var randomSquare = rnd.Next(0, 9);
            var square = sudoku.GetSquare(randomSquare);
            var a = rnd.Next(0, 9);
            var b = rnd.Next(0, 9);
            if (square[a].IsFixed || square[b].IsFixed)
            { continue; }

            Sudoku.Swap(square, a, b);
            sudoku.PutSquare(square, randomSquare, a, b);
            distanceLeft--;
        }
        return sudoku;
    }
}