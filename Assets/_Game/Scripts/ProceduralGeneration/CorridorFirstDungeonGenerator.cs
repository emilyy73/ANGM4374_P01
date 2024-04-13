using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using System.ComponentModel;
using Random = UnityEngine.Random;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField] [Range(0.1f, 1)]
    private float roomPercent = 0.8f;
    [SerializeField] [Range(0.01f, 0.75f)]
    private float corridorRemovalFactor = 0.1f;

    //change these to private and use mutators/accesors?
    // ADD WITCH SPAWN
    [HideInInspector]
    public Vector2Int playerSpawn;
    [HideInInspector]
    public Vector2Int bossSpawn;
    [HideInInspector]
    public Vector2Int witchSpawn01;
    [HideInInspector]
    public Vector2Int witchSpawn02;

    [HideInInspector]
    public HashSet<Vector2Int> floorPositions { get; private set; }

    [HideInInspector]
    public HashSet<Vector2Int> corridorPositions { get; private set; }

    public override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseCorridorWidth3x3(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        //REMOVE AFTER TESTING
        /*floorTileMap.ClearAllTiles();

        foreach (var position in floorPositions)
        {
            // TODO update with visuals for testing then remove
            //Debug.Log(position);
            Vector3 _position = new Vector3(position.x, -0.5f, position.y);
            // Debug.DrawLine(Vector3.zero, _position, Color.red, 300);
            //drawRect(position);

            floorTileMap.SetTile((Vector3Int)position, floorTile);
        }*/

        List<Vector2Int> checkRoomPos = roomPositions.OrderBy(x => Guid.NewGuid()).Take(roomPositions.Count).ToList();

        corridorPositions = RemoveRandomCorridorTiles(corridors);

        FindSpawns(corridors, playerSpawn);
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (roomFloors.Contains(position) == false)
            {
                var roomFloor = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(roomFloor);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighborsCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighborsCount++;
            }
            if (neighborsCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            corridors.Add(corridor);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
        return corridors;
    }

    public List<Vector2Int> IncreaseCorridorWidth3x3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    private void FindSpawns(List<List<Vector2Int>> checkRoomPosLL, Vector2Int playerSpawn)
    {
        
        List<Vector2Int> checkRoomPos = ListListToList(checkRoomPosLL);
        List<Vector2Int> orderedRooms = checkRoomPos.OrderBy(point => Mathf.Sqrt(Mathf.Pow(point.x - playerSpawn.x, 2) + Mathf.Pow(point.y -
            playerSpawn.y, 2))).ToList();

        playerSpawn = orderedRooms[0];
        witchSpawn01 = orderedRooms[orderedRooms.Count / 3];
        witchSpawn02 = orderedRooms[orderedRooms.Count * 2 / 3];
        bossSpawn = orderedRooms[orderedRooms.Count - 1];
    }

    private List<Vector2Int> ListListToList(List<List<Vector2Int>> listList)
    {
        List<Vector2Int> flattened = new List<Vector2Int>();

        foreach (List<Vector2Int> toFlatten in listList)
        {
            foreach (Vector2Int position in toFlatten)
            {
                flattened.Add(position);
            }
        }

        return flattened;
    }

    private HashSet<Vector2Int> RemoveRandomCorridorTiles(List<List<Vector2Int>> corridorListList)
    {
        List<Vector2Int> corridorList = ListListToList(corridorListList);
        int totTileCount = corridorList.Count();
        int tilesToChange = (int)Mathf.Round(totTileCount * corridorRemovalFactor);

        for (int i = 0; i < tilesToChange; i++)
        {
            corridorList.Remove(corridorList.ElementAt(Random.Range(0, corridorList.Count() - 1)));
        }

        return new HashSet<Vector2Int>(corridorList);
    }
}