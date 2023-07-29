using System.Collections.Generic;
using System.Linq;
using GridSystem.Elements;
using GridSystem.Elements.Base;
using UnityEngine;
using Random = System.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GridSystem.PuzzleGrid.LevelGeneration {
    public static class PuzzleGenerator {

        public static HashSet<Vector2Int> Path = new();

        public static bool GeneratePath(PuzzleManager puzzleManager, int attempts = 20) {
            Vector2Int[] validDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.right };
            Puzzle puzzle = puzzleManager.Puzzle;
            Vector2Int start = puzzle.StartCoordinates;
            Vector2Int end = puzzle.EndCoordinates;
            int minPathLength = puzzle.PathLengthLimit.x;
            int maxPathLength = puzzle.PathLengthLimit.y;

            if (puzzle.PathLengthLimit == Vector2Int.zero) {
                Debug.LogWarning("Path length range is 0");
                return false;
            }

            if (start == end) {
                Debug.LogWarning("Starting position identical to ending position");
                return false;
            }

            Random random = new();
            int attempt = 0;

            RegeneratePath:
            HashSet<Vector2Int> path = new();
            HashSet<Vector2Int> visited = new();
            Stack<Vector2Int> stack = new();
            stack.Push(start);

            while (stack.Count > 0) {
                Vector2Int current = stack.Peek();

                if (current == end) {
                    path.Add(current);

                    if (path.Count < minPathLength || maxPathLength < path.Count) {
                        if (attempt++ < attempts)
                            goto RegeneratePath;
                        Debug.LogWarning($"Failed {attempts} times to generate path " +
                                         $"with length range {minPathLength} {maxPathLength} " +
                                         $"within {puzzle.LevelSize}");
                        return false;
                    }

                    break;
                }

                visited.Add(current);

                List<Vector2Int> validMoves = validDirections
                    .Where(direction => IsValidMove(puzzle.LevelSize, current, direction, visited, path))
                    .ToList();

                if (validMoves.Count > 0) {
                    // Choose a random valid move as the new direction
                    int randomIndex = random.Next(validMoves.Count);
                    Vector2Int newDirection = validMoves[randomIndex];

                    Vector2Int newPosition = current + newDirection;
                    stack.Push(newPosition);

                    // Only add the current node to the path after the move is confirmed
                    path.Add(current);
                } else {
                    // No valid moves, backtrack by popping the current node and remove from path
                    stack.Pop();
                    path.Remove(current);
                }
            }

            Path = path;

            puzzleManager.DrawGizmos = PuzzleManager.PuzzleManagerGizmos.Generator;
#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif

            return true;
        }

        public static bool RandomizePlacements(PuzzleManager puzzleManager, int attempts = 20) {
            Puzzle puzzle = puzzleManager.Puzzle;
            Vector2Int puzzleSize = puzzle.LevelSize;
            List<GridElement> placedElements = new();
            List<GridElement> elements = new();
            elements.AddRange(puzzleManager.MovableElements);
            elements.AddRange(puzzleManager.ImmovableElements);
            elements.Sort((a, b) => b.Area.CompareTo(a.Area));

            int attempt = 0;
            while (attempt++ <= attempts) {
                puzzle.Grid = new bool[puzzleSize.x, puzzleSize.y];

                foreach (Vector2Int node in Path)
                    puzzle.Grid.OccupyCell(node.x, node.y);

                placedElements.Clear();
                placedElements.AddRange(elements.Where(element => puzzle.Grid.RandomizePlacement(element)));

                if (placedElements.Count != elements.Count) continue;

                foreach (Vector2Int node in Path)
                    puzzle.Grid.ClearCell(node.x, node.y);
                puzzle.Grid.OccupyCells(puzzleManager.Family);

                return true;
            }

            Debug.LogWarning($"Failed {attempts} times to place randomly all {elements.Count} elements " +
                             $"within {puzzle.LevelSize}");

            return false;
        }

        public static bool ShuffleElements(PuzzleManager puzzleManager, int attempts = 3) {
            int bestPathNodesCovered = 0;
            bool[,] bestGrid = puzzleManager.Puzzle.Grid;

            int attempt = 0;
            while (attempt++ < attempts) {
                bool[,] grid = puzzleManager.Puzzle.Grid;

                foreach (MovableElement element in puzzleManager.MovableElements)
                    puzzleManager.MoveElement(element, grid.RandomDirection(element));

                int pathNodesCovered = Path.Count(node => !grid.CellIsFree(node.x, node.y));

                if (pathNodesCovered <= bestPathNodesCovered) continue;

                bestPathNodesCovered = pathNodesCovered;
                bestGrid = grid;
            }

            puzzleManager.Puzzle.Grid = bestGrid;

            return true;
        }

        public static void GenerateLevel(PuzzleManager puzzleManager, int attempts = 50) {
            int attempt = 0;
            while (attempt++ < attempts)
                if (GeneratePath(puzzleManager) &&
                    RandomizePlacements(puzzleManager) &&
                    ShuffleElements(puzzleManager)) {
                    Debug.Log("<color=#00ff00><b>Successfully generated a new puzzle.</b></color>");
                    break;
                }
        }

        // Check if the move is within the grid boundaries and not visited
        private static bool IsValidMove(Vector2Int graphSize, Vector2Int current, Vector2Int direction,
            HashSet<Vector2Int> visited, HashSet<Vector2Int> path) {
            Vector2Int newPosition = current + direction;

            if (!IsValidPosition(graphSize, newPosition))
                return false;

            if (visited.Contains(newPosition))
                return false;

            if (path.Contains(newPosition))
                return false;

            // Check diagonals
            if (direction == Vector2Int.right || direction == Vector2Int.left) {
                Vector2Int upDiagonal = new(newPosition.x, newPosition.y + 1);
                Vector2Int downDiagonal = new(newPosition.x, newPosition.y - 1);
                return !path.Contains(upDiagonal) && !path.Contains(downDiagonal);
            }

            // Up or Down direction
            Vector2Int rightDiagonal = new(newPosition.x + 1, newPosition.y);
            Vector2Int leftDiagonal = new(newPosition.x - 1, newPosition.y);
            return !path.Contains(rightDiagonal) && !path.Contains(leftDiagonal);
        }

        private static bool IsValidPosition(Vector2Int graphSize, Vector2Int position) =>
            position.x >= 0 && position.x < graphSize.x &&
            position.y >= 0 && position.y < graphSize.y;

        private static bool IsValidPosition(Vector2Int graphSize, Vector2Int position, Vector2Int elementSize) =>
            position.x >= 0 && position.x < graphSize.x + elementSize.x &&
            position.y >= 0 && position.y < graphSize.y + elementSize.y;

    }
}