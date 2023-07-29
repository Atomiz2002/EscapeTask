using GridSystem.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace GridSystem.PuzzleGrid {
    public class Arrow : MonoBehaviour {

        [SerializeField] private Vector2Int Direction;
        [SerializeField] private Button     Button;

        private MovableElement movableElement;

        private void Reset() {
            TryGetComponent(out Button);
        }

        private void OnEnable() {
            Button.onClick.AddListener(MoveElement);
        }

        private void OnDisable() {
            Button.onClick.RemoveListener(MoveElement);
        }

        internal void LinkTo(MovableElement movableElement, Vector2Int direction) {
            gameObject.SetActive(direction == Direction);

            if (movableElement == this.movableElement) return;

            transform.SetParent(movableElement.transform, false);
            this.movableElement = movableElement;
        }

        private void MoveElement() {
            movableElement.Move(Direction);
        }

    }
}