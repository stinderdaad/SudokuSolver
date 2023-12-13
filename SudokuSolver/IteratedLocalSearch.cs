namespace SudokuSolver;

public record struct SudokuResult(Sudoku Sudoku, bool Improved);

public class IteratedLocalSearch
{
    public static (Sudoku solution, int iterationCount) Solve(Sudoku inputSudoku, HashSet<Sudoku> visitedStates, int iterationCount, int sValue, int maxIterations)
    {
        var result = new SudokuResult(inputSudoku, true);
        var previousResult = new Sudoku(inputSudoku);
        var counter = 0; // counter telt hoe vaak achter elkaar er géén verbetering is. Als dat te vaak gebeurt is er een random walk
        var unvisitedSquares = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 }; // houdt bij welke vierkanten al bezocht zijn om snel lokaal maximum te bepalen
        
        // loops while result improves and solution is not yet found
        while (unvisitedSquares.Count != 0 && counter < 9 && result.Sudoku.EvaluationResult != 0)
        while (counter < 9 && result.Sudoku.EvaluationResult != 0 && iterationCount < maxIterations)
        {
            result = Step(result.Sudoku, visitedStates, unvisitedSquares);
            iterationCount++;
            if (result.Improved) 
            {
                unvisitedSquares = [0, 1, 2, 3, 4, 5, 6, 7, 8];

                // check of 'verbetering' dezelfde score heeft, want zo ja, dan kan het een plateau zijn
                if (result.Sudoku.EvaluationResult == previousResult.EvaluationResult)
                    counter++; 
                else counter = 0;
            }
            previousResult = new Sudoku(result.Sudoku);

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

    private static SudokuResult Step(Sudoku inputSudoku, HashSet<Sudoku> visitedStates, List<int> squares)
    {
        var neighbours = GenerateNeighbours(inputSudoku, visitedStates, squares);
        var bestNeighbour = ChooseNext(inputSudoku, neighbours);
        visitedStates.Add(bestNeighbour.Sudoku);
        return bestNeighbour;
    }

    public static List<Sudoku> GenerateNeighbours(Sudoku sudoku, HashSet<Sudoku> visitedStates, List<int> squares)
    {
        var neighbours = new List<Sudoku>();
        var rnd = new Random();
        var randomNumber = rnd.Next(0, squares.Count);
        var randomSquare = squares[randomNumber];
        for (var i = 0; i < 9; i++)
        {
            for (var j = i + 1; j < 9; j++)
            {
                Sudoku neighbour = new(sudoku);
                var square = sudoku.GetSquare(squares[randomNumber]);
                if (square[i].IsFixed || square[j].IsFixed)
                { continue; }

                Sudoku.Swap(square, i, j);
                neighbour.PutSquare(square, squares[randomNumber], i, j);
                if (visitedStates.Contains(neighbour))
                { continue; }

                neighbours.Add(neighbour);
            }
        }
        squares.Remove(randomSquare);
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
        return new SudokuResult(result, result != current);
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