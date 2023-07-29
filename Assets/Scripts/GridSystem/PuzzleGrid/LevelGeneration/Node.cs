using UnityEngine;

namespace GridSystem.PuzzleGrid.LevelGeneration {
    public class Node {

        public Node       Parent;
        public Vector2Int Position;

        public int X {
            get => Position.x;
            set => Position.x = value;
        }

        public int Y {
            get => Position.y;
            set => Position.y = value;
        }

        public Node(Vector2Int position) {
            this.Position = position;
        }

    }
}