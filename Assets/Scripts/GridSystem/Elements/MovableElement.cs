using GridSystem.Elements.Base;
using UnityEngine;
using UnityEngine.UI;

namespace GridSystem.Elements {
    [RequireComponent(typeof(Button))]
    public class MovableElement : GridElement {

        [SerializeField] private Button Button;

        protected override void Reset() {
            base.Reset();
            TryGetComponent(out Button);
        }

        private void OnEnable() {
            Button.onClick.AddListener(RequestArrows);
        }

        private void OnDisable() {
            Button.onClick.RemoveListener(RequestArrows);
        }

        private void RequestArrows() {
            PuzzleManager.RequestArrows(this);
        }

        internal void Move(Vector2Int direction) {
            PuzzleManager.MoveElement(this, direction);
            PuzzleManager.RequestArrows(this);
        }

    }
}