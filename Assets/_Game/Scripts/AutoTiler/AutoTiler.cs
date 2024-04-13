using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AutoTiler : MonoBehaviour
{
    private IDictionary<Vector2, int> microTilePositions;
    private IDictionary<Vector2, int> emptyTilePositions;

    [Header("Tile Assets")]
    [SerializeField]
    private List<GameObject> tileA;
    [SerializeField]
    private List<GameObject> tileB;
    [SerializeField]
    private List<GameObject> tileC;
    [SerializeField]
    private List<GameObject> tileD;
    [SerializeField]
    private List<GameObject> tileE;
    [SerializeField]
    private List<GameObject> tileF;

    private IDictionary<int, (List<GameObject>, int)> microTiles = new Dictionary<int, (List<GameObject>, int)>();

    [Header("Tile Configuration")]
    [SerializeField]
    private CorridorFirstDungeonGenerator cfdg;
    [SerializeField]
    private GameObject pathTile;

    [Header("Vegetation")]
    [SerializeField]
    private GameObject grass;
    [SerializeField]
    private float flowerCoverage = 0.1f;
    [SerializeField]
    private GameObject flower01;
    [SerializeField]
    private GameObject flower02;
    [SerializeField]
    private float treeCoverage = 0.1f;
    [SerializeField]
    private GameObject tree01;
    [SerializeField]
    private GameObject tree02;

    public void RunAutoTiler()
    {
        microTiles = new Dictionary<int, (List<GameObject>, int)> {
        // moving counterclockwise
        {0, (tileA, 0)},
        {1, (tileB, 2)},
        {2, (tileB, 1)},
        {3, (tileD, 1)},
        {4, (tileB, -1)},
        {5, (tileD, 2)},
        {6, (tileC, 1)},
        {7, (tileE, -1)},
        {8, (tileB, 0)},
        {9, (tileC, 0)},
        {10, (tileD, 0)},
        {11, (tileE, 2)},
        {12, (tileD, -1)},
        {13, (tileE, 0)},
        {14, (tileE, 1)},
        {15, (tileF, 0)}
        };

        DrawDebugRectCentered(new Vector3(0, 0, 0), 0.5f, Color.red, 1000);

        AnchorPointToTilemap();
        (Vector2 min, Vector2 max) = FindCorners();
        (Vector2 center, int radius) = GetCircle(min, max);
        FillEmptySpace(center, radius);
        PlaceMicroTiles();
        PlaceEmptyTiles();
        FillCorridors();
        SpawnVegetation();
        SpawnTrees();
    }

    private void DrawDebugRectCentered(Vector3 center, float radius, Color color, float duration)
    {
        Vector3 topLeft = new Vector3(center.x - radius, 0, center.z + radius);
        Vector3 topRight = new Vector3(center.x + radius, 0, center.z + radius);
        Vector3 bottomLeft = new Vector3(center.x - radius, 0, center.z - radius);
        Vector3 bottomRight = new Vector3(center.x + radius, 0, center.z - radius);

        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }

    private void AnchorPointToTilemap() {
        microTilePositions = new Dictionary<Vector2, int>();
    foreach (Vector2Int position in cfdg.floorPositions) {
            Vector2 upperLeft = new Vector2(position.x - 0.5f, position.y - 0.5f);
            Vector2 upperRight = new Vector2(position.x + 0.5f, position.y - 0.5f);
            Vector2 bottomLeft = new Vector2(position.x - 0.5f, position.y + 0.5f);
            Vector2 bottomRight = new Vector2(position.x + 0.5f, position.y + 0.5f);

            if (microTilePositions.ContainsKey(upperLeft))
            {
                int index = microTilePositions[upperLeft];
                microTilePositions.Remove(upperLeft);
                microTilePositions.Add(upperLeft, index + 8);
            }
            else
            {
                microTilePositions.Add(upperLeft, 8);
            }

            if (microTilePositions.ContainsKey(upperRight))
            {
                int index = microTilePositions[upperRight];
                microTilePositions.Remove(upperRight);
                microTilePositions.Add(upperRight, index + 4);
            }
            else
            {
                microTilePositions.Add(upperRight, 4);
            }

            if (microTilePositions.ContainsKey(bottomLeft))
            {
                int index = microTilePositions[bottomLeft];
                microTilePositions.Remove(bottomLeft);
                microTilePositions.Add(bottomLeft, index + 2);
            }
            else
            {
                microTilePositions.Add(bottomLeft, 2);
            }

            if (microTilePositions.ContainsKey(bottomRight))
            {
                int index = microTilePositions[bottomRight];
                microTilePositions.Remove(bottomRight);
                microTilePositions.Add(bottomRight, index + 1);
            }
            else
            {
                microTilePositions.Add(bottomRight, 1);
            }
        }
    }

    private void PlaceMicroTiles()
    {
        foreach (KeyValuePair<Vector2, int> microTile in microTilePositions)
        {
            (List<GameObject> a, int b) tiles = microTiles[microTile.Value];
            GameObject tile = tiles.a[Random.Range(0, tiles.a.Count)];

            if (microTile.Value == 15)
            {
                Vector3 rotationEuler = new Vector3(-90, 90 * Random.Range(0, 3), 0);
                Quaternion rotationQuaternion = Quaternion.Euler(rotationEuler);
                Instantiate(tile, (new Vector3(microTile.Key.x, -0.5f, microTile.Key.y)), rotationQuaternion);
            }
            else
            {
                Vector3 rotationEuler = new Vector3(-90, 90 * tiles.b, 0);
                Quaternion rotationQuaternion = Quaternion.Euler(rotationEuler);
                Instantiate(tile, (new Vector3(microTile.Key.x, -0.5f, microTile.Key.y)), rotationQuaternion);
            }
        }
    }

    private void PlaceEmptyTiles()
    {
        foreach (KeyValuePair<Vector2, int> microTile in emptyTilePositions)
        {
            (List<GameObject> a, int b) tiles = microTiles[microTile.Value];
            GameObject tile = tiles.a[Random.Range(0, tiles.a.Count)];

            if (microTile.Value == 0)
            {
                Vector3 rotationEuler = new Vector3(-90, 90 * Random.Range(0, 4), 0);
                Quaternion rotationQuaternion = Quaternion.Euler(rotationEuler);
                Instantiate(tile, (new Vector3(microTile.Key.x, 0.5f, microTile.Key.y)), rotationQuaternion);
            }
        }
    }

    private (Vector2 min, Vector2 max) FindCorners()
    {
        Vector2 min = new Vector2(0, 0);
        Vector2 max = new Vector2(0, 0);

        foreach (Vector2 position in microTilePositions.Keys)
        {
            min = Vector2.Min(min, position);
            max = Vector2.Max(max, position);
        }

        return (min, max);
    }

    private (Vector2 center, int radius) GetCircle(Vector2 min, Vector2 max)
    {
        float width = Mathf.Abs(max.x - min.x);
        float height = Mathf.Abs(max.y - min.y);

        float radius = Mathf.Max(width, height) * 2 / 3;
        int radiusInt = (int)Mathf.Round(radius);

        Vector2 center = new Vector2(min.x + width / 2, min.y + height / 2);

        return (center, radiusInt);
    }

    private void FillEmptySpace(Vector2 center, int radius)
    {
        emptyTilePositions = new Dictionary<Vector2, int>();
        for (float col = center.x - radius; col < center.x + radius; col++)
        {
            for (float row = center.y - radius; row < center.y + radius; row++)
            {
                Vector2 current = new Vector2(Mathf.Floor(col)+0.5f, Mathf.Floor(row)+0.5f);
                if (microTilePositions.ContainsKey(current))
                {
                    continue;
                }

                if (Vector2.Distance(current, center) < radius)
                {
                    emptyTilePositions.Add(current, 0);
                }
            }
        }
    }

    private void FillCorridors()
    {
        HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>(cfdg.corridorPositions);
        foreach (Vector2Int position in corridorPositions)
        {
            Quaternion angle = Quaternion.Euler(-90f, Random.Range(-10, 10), 0f);
            Instantiate(pathTile, new Vector3(position.x, 0.02f, position.y), angle);
        }
    }

    private void SpawnVegetation()
    {
        foreach (KeyValuePair<Vector2, int> microTile in microTilePositions)
        {
            (List<GameObject> a, int b) tiles = microTiles[microTile.Value];
            GameObject tile = tiles.a[Random.Range(0, tiles.a.Count)];

            if (microTile.Value == 0)
            {
                continue;
            }

            float grassNoise = Mathf.PerlinNoise(microTile.Key.x, microTile.Key.y);
            if (grassNoise > 0.7f)
            {
                Instantiate(grass, new Vector3(microTile.Key.x + Random.Range(-0.25f, 0.25f), 0.5f, microTile.Key.y +
                    Random.Range(-0.25f, 0.25f)), Quaternion.identity);
            }
        }

        List<Vector2> listOfTiles = microTilePositions.Keys.ToList();
        for (int i = 0; i < microTilePositions.Count * flowerCoverage; i++)
        {
            Vector2 randomTile = listOfTiles[Random.Range(0, listOfTiles.Count)];
            if (microTilePositions[randomTile] == 15)
            {
                int flower = Random.Range(0, 2);
                if (flower == 0)
                {
                    Instantiate(flower01, new Vector3(randomTile.x, 1, randomTile.y), Quaternion.identity);
                }
                else
                {
                    Instantiate(flower02, new Vector3(randomTile.x, 1, randomTile.y), Quaternion.identity);
                }
            }
        }
    }

    private void SpawnTrees()
    {
        List<Vector2> boundary = findBoundary();

        List<Vector2> listOfTiles = microTilePositions.Keys.ToList();

        float boundaryRatio = 0.9f;
        float middleRatio = 0.1f;

        for (int i = 0; i < microTilePositions.Count * boundaryRatio * treeCoverage; i++)
        {
            Vector2 randomBoundaryTile = new Vector2();
            randomBoundaryTile = CheckDirections(boundary[Random.Range(0, boundary.Count)]);
            int tree = Random.Range(0, 2);

            if (tree == 0)
            {
                Instantiate(tree01, new Vector3(randomBoundaryTile.x, 0.5f, randomBoundaryTile.y), Quaternion.identity);
            }
            else
            {
                Instantiate(tree02, new Vector3(randomBoundaryTile.x, 0.5f, randomBoundaryTile.y), Quaternion.identity);
            }
        }

        for (int i = 0; i < microTilePositions.Count * middleRatio * treeCoverage; i++)
        {
            Vector2 randomBoundaryTile = listOfTiles[Random.Range(0, listOfTiles.Count)];
            int tree = Random.Range(0, 2);

            if (tree == 0)
            {
                Instantiate(tree01, new Vector3(randomBoundaryTile.x, 0.5f, randomBoundaryTile.y), Quaternion.identity);
            }
            else
            {
                Instantiate(tree02, new Vector3(randomBoundaryTile.x, 0.5f, randomBoundaryTile.y), Quaternion.identity);
            }
        }
    }

    private Vector2 CheckDirections(Vector2 position)
    {
        for (int row = -1; row < 2; row++)
        {
            for (int col = -1; col < 2; col++)
            {
                Vector2 posToCheck = new Vector2(position.x + row, position.y + col);
                if (microTilePositions.ContainsKey(posToCheck))
                {
                    return posToCheck;
                }
            }
        }
        return new Vector2(0, 0);
    }
    
    private List<Vector2> findBoundary()
    {
        List<Vector2> pointsLR = microTilePositions.Keys.OrderBy(point => point.x).ToList();
        int xCoords = (int)(pointsLR[pointsLR.Count - 1].x - pointsLR[0].x);
        List<List<Vector2>> columnRow = new List<List<Vector2>>();
        
        float xVal = pointsLR[0].x;
        int counterX = 0;
        for (int i = 0; i <= xCoords; i++)
        {
            List<Vector2> temporaryList = new List<Vector2>();
            while (counterX < pointsLR.Count && pointsLR[counterX].x == xVal)
            {
                temporaryList.Add(pointsLR[counterX]);
                counterX++;
            }

            columnRow.Add(temporaryList.OrderBy(point => point.y).ToList());
            xVal += 1;
        }

        List<Vector2> pointsTD = microTilePositions.Keys.OrderBy(point => point.y).ToList();
        int yCoords = (int)(pointsTD[pointsTD.Count - 1].y - pointsTD[0].y);
        List<List<Vector2>> rowColumn = new List<List<Vector2>>();

        float yVal = pointsTD[0].y;
        int counterY = 0;
        for (int i = 0; i <= yCoords; i++)
        {
            List<Vector2> temporaryList = new List<Vector2>();
            while (counterY < pointsTD.Count && pointsTD[counterY].y == yVal)
            {
                temporaryList.Add(pointsTD[counterY]);
                counterY++;
            }
            rowColumn.Add(temporaryList.OrderBy(point => point.x).ToList());
            yVal += 1;
        }

        List<Vector2> boundaryPoints = new List<Vector2>();

        foreach (List<Vector2> listX in columnRow)
        {
            boundaryPoints.Add(listX[0]);
            boundaryPoints.Add(listX[listX.Count - 1]);
        }

        foreach (List<Vector2> listY in rowColumn)
        {
            boundaryPoints.Add(listY[0]);
            boundaryPoints.Add(listY[listY.Count - 1]);
        }

        return boundaryPoints;
    }

    // below is daigo
    private void DebugClosedPath(List<Vector2> path)
    {
        Vector2 current = path[0];
        foreach (Vector2 next in path)
        {
            Debug.Log(current + " " + next);
            Debug.DrawLine(new Vector3(current.x, 2, current.y), new Vector3(next.x, 2, next.y), Color.red, 300f);
            current = next;
        }
    }

    private List<Vector2> findBoundaryPoints()
    {
        List<Vector2> pointsLR = microTilePositions.Keys.OrderBy(point => point.x).ToList();
        List<Vector2> boundaryPoints = new List<Vector2>();

        Vector2 parent = pointsLR[0];
        Vector2 currentBest = pointsLR[1];

        List<Vector2> toCheck = new List<Vector2>(pointsLR);
        while (parent != pointsLR[0])
        {
            toCheck = pointsLR.Except(boundaryPoints).ToList();
            Vector2 secondBest = currentBest;
            currentBest = toCheck[1];
            foreach (Vector2 nextBest in toCheck)
            {
                int counterClockwise = CounterClockwise(parent, currentBest, nextBest);

                if (counterClockwise > 0)
                {
                    secondBest = currentBest;
                    currentBest = nextBest;
                }
                else if (counterClockwise < 0)
                {
                    continue;
                }
                else
                {
                    if (Vector2.Distance(parent, currentBest) < Vector2.Distance(parent, nextBest))
                    {
                        secondBest = currentBest;
                        currentBest = nextBest;
                    }
                }
            }

            boundaryPoints.Add(secondBest);
            parent = secondBest;
        }
        
        return boundaryPoints;

    }

    private int CounterClockwise(Vector2 a, Vector2 b, Vector2 toCompare)
    {
        float slope = (b.y - a.y) * (toCompare.x - a.x) - (toCompare.y - a.y) * (b.x - a.x);

        if (slope < 0)
        {
            return 1;
        }
        else if (slope > 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}