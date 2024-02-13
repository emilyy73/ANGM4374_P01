using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkMapGenerator : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    [SerializeField]
    protected SimpleRandomWalkSO randomWalkParameters;

    public virtual void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        foreach (var position in floorPositions)
        {
            // TODO update with visuals for testing then remove
            //Debug.Log(position);
            Vector3 _position = new Vector3(position.x, 0, position.y);
            // Debug.DrawLine(Vector3.zero, _position, Color.red, 300);
            drawRect(position);
        }
    }

    private void drawRect(Vector2 position) {
        float xStart = position.x - 0.5f;
        float yStart = position.y - 0.5f;

        Debug.DrawLine(new Vector3(xStart, 0, yStart), new Vector3(xStart + 1, 0, yStart), Color.red, 2);
        Debug.DrawLine(new Vector3(xStart + 1, 0, yStart), new Vector3(xStart + 1, 0, yStart + 1), Color.red, 2);
        Debug.DrawLine(new Vector3(xStart + 1, 0, yStart + 1), new Vector3(xStart, 0, yStart + 1), Color.red, 2);
        Debug.DrawLine(new Vector3(xStart, 0, yStart + 1), new Vector3(xStart, 0, yStart), Color.red, 2);
    }

    // below function may not be used depending on implementation of player/boss spawn points
    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters)
    {
        var currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < randomWalkParameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, randomWalkParameters.walkLength);
            floorPositions.UnionWith(path);
            if (randomWalkParameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < randomWalkParameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, randomWalkParameters.walkLength);
            floorPositions.UnionWith(path);
            if (randomWalkParameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }
}
