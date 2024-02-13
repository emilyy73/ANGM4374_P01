using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleRandomWalkMapGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    SimpleRandomWalkMapGenerator generator;

    private void Awake()
    {
        generator = (SimpleRandomWalkMapGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create Dungeon"))
        {
            generator.RunProceduralGeneration();
        }
    }
}