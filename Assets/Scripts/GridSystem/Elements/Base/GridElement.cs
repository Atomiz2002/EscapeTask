using GridSystem.PuzzleGrid;
using UnityEditor;
using UnityEngine;

namespace GridSystem.Elements.Base {
    public abstract class GridElement : MonoBehaviour {

        internal static readonly Color OccupiedCellColor = new(1, 0, 0, .2f);
        internal static readonly Color TargetCellColor   = new(0, 1, 0, .2f);
        private const            int   SingleCellSize    = 108;

        [SerializeField] public PuzzleManager PuzzleManager;
        [SerializeField] public RectInt       Element;

        internal int X => Element.x;
        internal int Y => Element.y;
        internal int Width => Element.width;
        internal int Height => Element.height;
        internal int Area => Width * Height;

        protected virtual void Reset() {
            PuzzleManager ??= GetComponentInParent<PuzzleManager>();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            AdjustPosition();
        }

        private void OnDrawGizmosSelected() {
            if (Selection.activeGameObject != gameObject) return;

            Vector2 size = new(PuzzleManager.ScaledSize, PuzzleManager.ScaledSize);

            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                Handles.DrawSolidRectangleWithOutline(
                    new Rect(PuzzleManager.GetRealPosition(Element.position + new Vector2Int(x, y)), size),
                    OccupiedCellColor,
                    Color.red);
        }
#endif

        internal void AdjustPosition() {
            TryGetComponent(out RectTransform rect);
            rect.position = PuzzleManager.GetRealPosition(Element.position);
            rect.sizeDelta = (Vector2)Element.size * SingleCellSize;
        }

        internal void SetPosition(Vector2Int coordinates) {
            Element.position = coordinates;
            AdjustPosition();
        }

    }
}