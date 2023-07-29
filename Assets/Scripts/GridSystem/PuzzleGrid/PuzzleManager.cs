using System.Collections;
using GridSystem.Elements;
using GridSystem.PuzzleGrid.LevelGeneration;
using UnityEditor;
using UnityEngine;
using static GridSystem.Elements.Base.GridElement;

namespace GridSystem.PuzzleGrid {
    public class PuzzleManager : MonoBehaviour {

        [SerializeField] internal Camera      Cam;
        [SerializeField] public   int         CellSize;
        [SerializeField] internal CanvasGroup PuzzleComplete;
        [Header("Controls")]
        [SerializeField] private Arrow ArrowUp;
        [SerializeField] private Arrow ArrowDown;
        [SerializeField] private Arrow ArrowLeft;
        [SerializeField] private Arrow ArrowRight;
        [Header("Gizmos")]
        [SerializeField] public PuzzleManagerGizmos DrawGizmos;
        [Header("Level Generation")]
        [SerializeField] private bool GenerateLevels;
        [SerializeField] private int LevelGenerationAttempts;
        [SerializeField] private int PathGenerationAttempts;
        [SerializeField] private int PlacementAttempts;
        [SerializeField] private int ShuffleAttempts;
        [Header("Level Elements")]
        [SerializeField] public Puzzle Puzzle;
        [SerializeField] public Family             Family;
        [SerializeField] public MovableElement[]   MovableElements;
        [SerializeField] public ImmovableElement[] ImmovableElements;

        internal float ScaledSize => CellSize * Mathf.Clamp01(Cam.aspect);
        private Arrow[] Arrows => new[] { ArrowUp, ArrowDown, ArrowLeft, ArrowRight };

        private void Reset() {
            Cam = Camera.main;
        }

        private void OnEnable() {
            Puzzle.New(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            OnRectTransformDimensionsChange();
        }

        public void OnDrawGizmosSelected() {
            switch (DrawGizmos) {
                case PuzzleManagerGizmos.None:
                    break;
                case PuzzleManagerGizmos.Occupations:
                    if (!Application.isPlaying)
                        Puzzle.New(this);

                    Vector2 position = transform.position;

                    for (int x = 0; x < Puzzle.LevelSize.x; x++)
                    for (int y = 0; y < Puzzle.LevelSize.y; y++) {
                        float offsetX = position.x - (float) Puzzle.LevelSize.x / 2 * ScaledSize + x * ScaledSize;
                        float offsetY = position.y - (float) Puzzle.LevelSize.y / 2 * ScaledSize + y * ScaledSize;

                        Handles.DrawSolidRectangleWithOutline(new Rect(offsetX, offsetY, ScaledSize, ScaledSize),
                            Puzzle.Grid.CellIsFree(x, y) ? Color.clear : OccupiedCellColor,
                            Color.red);
                    }

                    Vector2 targetPosition = GetRealPosition(Puzzle.EndCoordinates);

                    Handles.DrawSolidRectangleWithOutline(
                        new Rect(targetPosition.x, targetPosition.y, ScaledSize, ScaledSize),
                        TargetCellColor, Color.clear);
                    break;

                case PuzzleManagerGizmos.Generator:
                    float size = ScaledSize;

                    foreach (Vector2Int node in PuzzleGenerator.Path) {
                        Vector2 nodePos = GetRealPosition(new Vector2Int(node.x, node.y));
                        Handles.DrawSolidRectangleWithOutline(new Rect(nodePos.x, nodePos.y, size, size),
                            TargetCellColor,
                            Color.green);
                    }

                    break;
            }
        }
#endif

        private void OnRectTransformDimensionsChange() {
            if (!Cam) return;

            Family.AdjustPosition();
            foreach (MovableElement movableElement in MovableElements)
                movableElement.AdjustPosition();
            foreach (ImmovableElement immovableElement in ImmovableElements)
                immovableElement.AdjustPosition();
        }

        public Vector2 GetRealPosition(Vector2Int coordinates) =>
            (Vector2) transform.position -
            ScaledSize * (Vector2) Puzzle.LevelSize / 2 +
            ScaledSize * (Vector2) coordinates;

        internal void MoveElement(MovableElement element, Vector2Int direction) {
            Puzzle.MoveElement(element, direction);

            if (element is not Family family || !CheckPuzzleCompleted(family)) return;
            StartCoroutine(ShowPuzzleCompleteScreen());
        }

        internal void RequestArrows(MovableElement movableElement) {
            ArrowUp.LinkTo(movableElement, Puzzle.Grid.Up(movableElement));
            ArrowDown.LinkTo(movableElement, Puzzle.Grid.Down(movableElement));
            ArrowLeft.LinkTo(movableElement, Puzzle.Grid.Left(movableElement));
            ArrowRight.LinkTo(movableElement, Puzzle.Grid.Right(movableElement));
        }

        internal void HideArrows() {
            ArrowUp.gameObject.SetActive(false);
            ArrowDown.gameObject.SetActive(false);
            ArrowLeft.gameObject.SetActive(false);
            ArrowRight.gameObject.SetActive(false);
        }

        internal void HidePuzzleComplete() {
            StartCoroutine(HidePuzzleCompleteScreen());
        }

        private IEnumerator HidePuzzleCompleteScreen() {
            PuzzleComplete.blocksRaycasts = false;
            while (PuzzleComplete.alpha > 0) {
                PuzzleComplete.alpha -= .05f;
                yield return new WaitForSeconds(.01f);
            }
        }

        private IEnumerator ShowPuzzleCompleteScreen() {
            HideArrows();

            PuzzleComplete.blocksRaycasts = true;
            while (PuzzleComplete.alpha < 1) {
                PuzzleComplete.alpha += .05f;
                yield return new WaitForSeconds(.01f);
            }
        }

        private bool CheckPuzzleCompleted(Family family) => family.X == Puzzle.EndCoordinates.x
                                                            && family.Y == Puzzle.EndCoordinates.y;

        public void GenerateLevel() {
            PuzzleGenerator.GenerateLevel(this, LevelGenerationAttempts);
        }

        public void GeneratePath() {
            PuzzleGenerator.GeneratePath(this, PathGenerationAttempts);
        }

        public void RandomizePlacements() {
            PuzzleGenerator.RandomizePlacements(this, PlacementAttempts);
        }

        public void ShuffleElements() {
            PuzzleGenerator.ShuffleElements(this, ShuffleAttempts);
        }

        public enum PuzzleManagerGizmos {

            None,
            Occupations,
            Generator

        }

    }
}