namespace SudokuSolver;

public record struct SudokuResult(Sudoku Sudoku, bool Improved);

public class IteratedLocalSearch
{
    public static Sudoku Solve(Sudoku inputSudoku, HashSet<Sudoku> visitedStates)
    {
        var b = true;
        var result = new SudokuResult(inputSudoku, true);
        
        // loops while result improves and solution is not yet found
        while (b && result.Sudoku.EvaluationResult != 0)
        {
            _ = Console.ReadLine();
            result = Step(inputSudoku, visitedStates);
            b = result.Improved;
            Console.WriteLine($"Improved: {b}");
            Console.WriteLine($"New Sudoku score: {result.Sudoku.EvaluationResult}");
            result.Sudoku.Print();

        }
        
        // if loop finishes, then either solution is found, or no improvement possible
        if (result.Sudoku.EvaluationResult == 0)
        {
            return result.Sudoku;
        }
        else
        {
            result.Sudoku = RandomWalk(result.Sudoku, 10);
            result.Sudoku = Solve(result.Sudoku, visitedStates);
            return result.Sudoku; // recursion
        }
    }

    private static SudokuResult Step(Sudoku inputSudoku, HashSet<Sudoku> visitedStates)
    {
        var neighbours = GenerateNeighbours(inputSudoku, visitedStates);
        var bestNeighbour = ChooseNext(inputSudoku, neighbours);
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
                neighbour.PutSquare(square, randomNumber);
                if (visitedStates.Contains(neighbour))
                { continue; }
                Console.WriteLine($"Swapped: square {randomNumber} number {i} and {j}");
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
            if (sudoku.EvaluationResult < lowestScore)
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
            sudoku.PutSquare(square, randomSquare);
            distanceLeft--;
        }
        return sudoku;
    }
}