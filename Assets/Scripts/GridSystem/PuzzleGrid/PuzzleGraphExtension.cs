using System.Collections.Generic;
using System.Linq;
using GridSystem.Elements;
using GridSystem.Elements.Base;
using UnityEngine;
using Random = System.Random;

namespace GridSystem.PuzzleGrid {
    public static class PuzzleGraphExtension {

        public static Vector2Int RandomDirection(this bool[,] grid, MovableElement element) {
            Vector2Int[] availableDirections = grid.AvailableDirections(element);

            return availableDirections
                .Where(dir => dir != Vector2Int.zero)
                .ElementAtOrDefault(new Random().Next(availableDirections.Length));
        }

        public static Vector2Int[] AvailableDirections(this bool[,] grid, MovableElement element) =>
            new[] {
                grid.Up(element),
                grid.Down(element),
                grid.Right(element),
                grid.Left(element)
            };

        public static Vector2Int Up(this bool[,] grid, MovableElement element) {
            int onTop = element.Y + element.Height;

            if (onTop >= grid.Height()) return Vector2Int.zero;

            for (int xCell = element.X; xCell < element.X + element.Width; xCell++) {
                if (grid.CellIsFree(xCell, onTop)) continue;

                return Vector2Int.zero;
            }

            return Vector2Int.up;
        }

        public static Vector2Int Down(this bool[,] grid, MovableElement element) {
            int onBottom = element.Y - 1;
            if (onBottom < 0) return Vector2Int.zero;

            for (int xCell = element.X; xCell < element.X + element.Width; xCell++) {
                if (grid.CellIsFree(xCell, onBottom)) continue;

                return Vector2Int.zero;
            }

            return Vector2Int.down;
        }

        public static Vector2Int Left(this bool[,] grid, MovableElement element) {
            int onLeft = element.X - 1;
            if (onLeft < 0) return Vector2Int.zero;

            for (int yCell = element.Y; yCell < element.Y + element.Height; yCell++) {
                if (grid.CellIsFree(onLeft, yCell)) continue;

                return Vector2Int.zero;
            }
            
            return Vector2Int.left;
        }

        public static Vector2Int Right(this bool[,] grid, MovableElement element) {
            int onRight = element.X + element.Width;

            if (onRight >= grid.Width()) return Vector2Int.zero;

            for (int yCell = element.Y; yCell < element.Y + element.Height; yCell++) {
                if (grid.CellIsFree(onRight, yCell)) continue;

                return Vector2Int.zero;
            }
            
            return Vector2Int.right;
        }

        public static int Width(this bool[,] grid) => grid.GetLength(0);

        public static int Height(this bool[,] grid) => grid.GetLength(1);

        public static bool RandomizePlacement(this bool[,] grid, GridElement element) {
            int randomX = new Random().Next(grid.Width());
            int randomY = new Random().Next(grid.Height());

            for (int x = 0; x < grid.Width(); x++) {
                int wrappedX = (x + randomX) % grid.Width();

                for (int y = 0; y < grid.Height(); y++) {
                    int wrappedY = (y + randomY) % grid.Height();

                    element.SetPosition(new Vector2Int(wrappedX, wrappedY));

                    if (!grid.TryOccupyCells(element)) continue;
                    
                    element.gameObject.SetActive(true);
                    return true;
                }
            }

            element.gameObject.SetActive(false);
            return false;
        }

        public static bool CellsAreFree(this bool[,] grid, GridElement cells) {
            for (int x = 0; x < cells.Width; x++)
            for (int y = 0; y < cells.Height; y++)
                if (!grid.CellIsFree(cells.X + x, cells.Y + y))
                    return false;

            return true;
        }

        public static bool CellIsFree(this bool[,] grid, int x, int y) {
            if (x < 0 || grid.Width() <= x) return false;
            if (y < 0 || grid.Height() <= y) return false;

            return !grid[x, y];
        }

        public static bool TryOccupyCells(this bool[,] grid, GridElement element) {
            if (!grid.CellsAreFree(element))
                return false;

            grid.OccupyCells(element);
            return true;
        }

        public static void OccupyCells(this bool[,] grid, GridElement element) {
            for (int x = element.X; x < element.X + element.Width; x++)
            for (int y = element.Y; y < element.Y + element.Height; y++)
                grid.OccupyCell(x, y);
        }

        public static void ClearCells(this bool[,] grid, RectInt cells) {
            for (int x = cells.x; x < cells.x + cells.width; x++)
            for (int y = cells.y; y < cells.y + cells.height; y++)
                grid.ClearCell(x, y);
        }

        public static void OccupyCell(this bool[,] grid, int x, int y) {
            grid[x, y] = true;
        }

        public static void ClearCell(this bool[,] grid, int x, int y) {
            grid[x, y] = false;
        }

        public static HashSet<Vector2Int> AsNodes(this bool[,] grid) {
            HashSet<Vector2Int> nodes = new();

            for (int x = 0; x < grid.Width(); x++)
            for (int y = 0; y < grid.Height(); y++)
                nodes.Add(new Vector2Int(x, y));

            return nodes;
        }

    }
}