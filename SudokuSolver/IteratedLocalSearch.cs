namespace SudokuSolver;

public record struct SudokuResult(Sudoku Sudoku, bool Improved);

public class IteratedLocalSearch
{
    // Solve the Sudoku using the Iterated Local Search algorithm
    public static (Sudoku solution, int iterationCount) Solve(Sudoku inputSudoku, HashSet<Sudoku> visitedStates, int iterationCount, int sValue, int maxIterations)
    {
        var result = new SudokuResult(inputSudoku, true);
        var previousResult = new Sudoku(inputSudoku);
        // Counter to keep track of how many iterations have been done without improvement
        var counter = 0;
        // Keep track of which squares we have / have not visited yet
        var unvisitedSquares = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        // Loop until we have visited all squares,
        // or until we have too many iterations without improvement,
        // or until we find the solution (evaluation value is equal to 0),
        // or until we have reached the maximum number of iterations
        while (unvisitedSquares.Count != 0 && counter < 9 && result.Sudoku.EvaluationResult != 0 && iterationCount < maxIterations)
        {
            result = Step(result.Sudoku, visitedStates, unvisitedSquares);
            iterationCount++;
            if (result.Improved) 
            {
                unvisitedSquares = [0, 1, 2, 3, 4, 5, 6, 7, 8];

                // Check if the evaluation value has improved or is the same, if its the same we could be on a plateau
                if (result.Sudoku.EvaluationResult == previousResult.EvaluationResult)
                    counter++; 
                else counter = 0;
            }
            previousResult = new Sudoku(result.Sudoku);
        }
        
        // If the loop finishes, then either a solution is found, or no improvement is possible,
        // or we reached the maximum allowed iterations
        if (result.Sudoku.EvaluationResult == 0 || iterationCount >= maxIterations)
        {
            return (result.Sudoku, iterationCount);
        }

        // If no improvement is possible in the current state, we apply random walk
        result.Sudoku = RandomWalk(result.Sudoku, sValue);
        
        // Recursion
        (result.Sudoku, iterationCount) = Solve(result.Sudoku, visitedStates, iterationCount, sValue, maxIterations);
        return (result.Sudoku, iterationCount);
        
    }

    // Perform one step of the Algorithm
    private static SudokuResult Step(Sudoku inputSudoku, HashSet<Sudoku> visitedStates, List<int> squares)
    {
        // Generate a list of neighbouring states
        var neighbours = GenerateNeighbours(inputSudoku, visitedStates, squares);
        // Choose the best neighbour (one with the lowest/equal evaluation value)
        var bestNeighbour = ChooseNext(inputSudoku, neighbours);
        // Add the best neighbour to the visited states
        visitedStates.Add(bestNeighbour.Sudoku);
        // Return the best neighbour as the new current state
        return bestNeighbour;
    }

    // Generate a list of neighbouring states
    private static List<Sudoku> GenerateNeighbours(Sudoku sudoku, HashSet<Sudoku> visitedStates, List<int> squares)
    {
        var neighbours = new List<Sudoku>();
        var rnd = new Random();
        var randomNumber = rnd.Next(0, squares.Count);
        // Choose a random square
        var randomSquare = squares[randomNumber];
        for (var i = 0; i < 9; i++)
        {
            for (var j = i + 1; j < 9; j++)
            {
                Sudoku neighbour = new(sudoku);
                var square = sudoku.GetSquare(squares[randomNumber]);
                // If any of the numbers are fixed, we cannot swap them
                if (square[i].IsFixed || square[j].IsFixed)
                { continue; }

                // Swap 2 numbers in the square
                Sudoku.Swap(square, i, j);
                // Put the new square back into the Sudoku
                neighbour.PutSquare(square, squares[randomNumber], i, j);
                // If this configuration has already been visited, we skip it 
                if (visitedStates.Contains(neighbour))
                { continue; }

                neighbours.Add(neighbour);
            }
        }
        squares.Remove(randomSquare);
        return neighbours;
    }

    // Returns a new sudoku with the lowest score,
    // and true if it is different than the previous sudoku, false if it is the same
    private static SudokuResult ChooseNext(Sudoku current, List<Sudoku> input)
    {
        var lowestScore = current.EvaluationResult;
        var result = new Sudoku(current);
        foreach (var sudoku in input.Where(sudoku => sudoku.EvaluationResult <= lowestScore))
        {
            result = sudoku;
            lowestScore = sudoku.EvaluationResult;
        }
        return new SudokuResult(result, result != current);
    }

    // Returns a sudoku with a random walk applied to it (distance determines the amount of random walks)
    private static Sudoku RandomWalk(Sudoku sudoku, int distance)
    {
        var rnd = new Random();
        var distanceLeft = distance;
        while (distanceLeft > 0)
        {
            // Choose a random square
            var randomSquare = rnd.Next(0, 9);
            var square = sudoku.GetSquare(randomSquare);
            // Choose 2 random numbers in the square
            var a = rnd.Next(0, 9);
            var b = rnd.Next(0, 9);
            // If any of the numbers are fixed, we cannot swap them
            if (square[a].IsFixed || square[b].IsFixed)
            { continue; }

            // Swap 2 numbers in the square
            Sudoku.Swap(square, a, b);
            // Put the new square back into the Sudoku
            sudoku.PutSquare(square, randomSquare, a, b);
            distanceLeft--;
        }
        return sudoku;
    }
}