using GridSystem.PuzzleGrid;
using GridSystem.PuzzleGrid.LevelGeneration;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PuzzleManager))]
public class PuzzleManagerEditor : Editor {

    public VisualTreeAsset UXML;

    public override VisualElement CreateInspectorGUI() {
        VisualElement root = new();

        UXML.CloneTree(root);

        VisualElement generationContainer = root.Q<VisualElement>("generate-level-container");
        Toggle generateToggle = generationContainer.Q<Toggle>("generate-level");
        Foldout generationFoldout = generationContainer.Q<Foldout>("foldout");
        VisualElement generationAttempts = generationFoldout.Q<VisualElement>("attempts");

        generationFoldout.RegisterCallback<ChangeEvent<bool>>(_ => {
            if (!generateToggle.value)
                generationFoldout.value = false;
        });
        generateToggle.RegisterCallback<ChangeEvent<bool>>(value => {
            generationFoldout.value = value.newValue;
            generationAttempts.style.display = value.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        });

        PuzzleManager puzzleManager = (PuzzleManager) target;

        generationAttempts.Q<IntegerField>("level-generation").Q<Button>().clicked +=
            () => puzzleManager.GenerateLevel();
        generationAttempts.Q<IntegerField>("path-generation").Q<Button>().clicked +=
            () => puzzleManager.GeneratePath();
        generationAttempts.Q<IntegerField>("randomize-placements").Q<Button>().clicked +=
            () => puzzleManager.RandomizePlacements();
        generationAttempts.Q<IntegerField>("shuffle-elements").Q<Button>().clicked +=
            () => puzzleManager.ShuffleElements();

        return root;
    }

}