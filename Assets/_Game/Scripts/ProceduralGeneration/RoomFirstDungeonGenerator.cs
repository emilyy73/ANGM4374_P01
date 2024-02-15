using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using System.ComponentModel;
using Random = UnityEngine.Random;

// REPLACE AFTER TESTING
using UnityEngine.Tilemaps;

public class RoomFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private int minRoomWidth = 10, minRoomHeight = 10;
    [SerializeField]
    private int dungeonWidth = 100, dungeonHeight = 100;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    // REPLACE AFTER TESTING
    [SerializeField]
    private Tilemap floorTileMap;
    [SerializeField]
    private TileBase floorTile;


    public override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition,
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet <Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRandomRooms(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        //REMOVE AFTER TESTING
        floorTileMap.ClearAllTiles();

        foreach (var position in floor)
        {
            // TODO update with visuals for testing then remove
            //Debug.Log(position);
            Vector3 _position = new Vector3(position.x, 0, position.y);
            // Debug.DrawLine(Vector3.zero, _position, Color.red, 300);
            //drawRect(position);

            floorTileMap.SetTile((Vector3Int)position, floorTile);
        }
    }

    private void drawRect(Vector2 position)
    {
        float xStart = position.x;
        float yStart = position.y;

        Debug.DrawLine(new Vector3(xStart, 0, yStart), new Vector3(xStart + 1, 0, yStart), Color.red, 2);
        Debug.DrawLine(new Vector3(xStart + 1, 0, yStart), new Vector3(xStart + 1, 0, yStart - 1), Color.red, 2);
        Debug.DrawLine(new Vector3(xStart + 1, 0, yStart - 1), new Vector3(xStart, 0, yStart - 1), Color.red, 2);
        Debug.DrawLine(new Vector3(xStart, 0, yStart - 1), new Vector3(xStart, 0, yStart), Color.red, 2);
    }

    private HashSet<Vector2Int> CreateRandomRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) &&
                    position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        HashSet<Vector2Int> wideCorridor = new HashSet<Vector2Int>();

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }

        var corridorCount = corridor.Count;
        List<Vector2Int> corridorList = corridor.OrderBy(x => Guid.NewGuid()).Take(corridorCount).ToList();
        
        for (int i = 1; i < corridorList.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    wideCorridor.Add(corridorList[i - 1] + new Vector2Int(x, y));
                }
            }
        }

        corridor.UnionWith(wideCorridor);

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float length = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < length)
            {
                length = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}