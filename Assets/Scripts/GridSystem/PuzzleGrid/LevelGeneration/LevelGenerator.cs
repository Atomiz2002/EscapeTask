using UnityEngine;
using UnityEngine.UIElements;

namespace GridSystem.PuzzleGrid.LevelGeneration {
    public class LevelGenerator : MonoBehaviour {

        [SerializeField] private UIDocument    UI;
        [SerializeField] private PuzzleManager PuzzleManager;

        private void Reset() {
            TryGetComponent(out UI);
        }

        private void OnEnable() {
            UI.rootVisualElement.Q<Button>("generate-level").clicked += () => {
                PuzzleManager.HidePuzzleComplete();
                PuzzleManager.Family.Element.position = PuzzleManager.Puzzle.StartCoordinates;
                PuzzleManager.Puzzle.New(PuzzleManager);
                PuzzleManager.GenerateLevel();
            };
        }

    }
}