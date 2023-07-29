using System;
using GridSystem.Elements;
using UnityEngine;

namespace GridSystem.PuzzleGrid.LevelGeneration {
    [Serializable]
    [CreateAssetMenu(menuName = "Puzzle")]
    public class Puzzle : ScriptableObject {

        public bool[,] Grid;

        [field: Space]
        [field: SerializeField] public Vector2Int LevelSize { get; private set; }
        [field: SerializeField] public Vector2Int StartCoordinates { get; private set; }
        [field: SerializeField] public Vector2Int EndCoordinates { get; private set; }
        [field: SerializeField] public Vector2Int PathLengthLimit { get; private set; }

        private Family             family;
        private MovableElement[]   movableElements;
        private ImmovableElement[] immovableElements;

        public void New(PuzzleManager puzzleManager) {
            family = puzzleManager.Family;
            movableElements = puzzleManager.MovableElements;
            immovableElements = puzzleManager.ImmovableElements;

            Grid = new bool[LevelSize.x, LevelSize.y];
            
            OccupyGrid();
            
            puzzleManager.HideArrows();
        }

        public void OccupyGrid() {
            family.AdjustPosition();
            if (!Grid.TryOccupyCells(family))
                Debug.LogWarning("Failed to occupy cells for Family");

            foreach (MovableElement movableElement in movableElements) {
                movableElement.AdjustPosition();
                if (!Grid.TryOccupyCells(movableElement))
                    Debug.LogWarning($"Failed to occupy cells for {movableElement.gameObject.name}");
            }

            foreach (ImmovableElement immovableElement in immovableElements) {
                immovableElement.AdjustPosition();
                if (!Grid.TryOccupyCells(immovableElement))
                    Debug.LogWarning($"Failed to occupy cells for {immovableElement.gameObject.name}");
            }
        }

        public void MoveElement(MovableElement element, Vector2Int direction) {
            Grid.ClearCells(element.Element);

            if (!Grid.CellsAreFree(element)) return;

            element.Element.position += direction;
            Grid.OccupyCells(element);
            element.AdjustPosition();
        }

    }
}