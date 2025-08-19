using System.Collections.Generic;
using UnityEngine;

public struct DoorInfo
{
    public Vector2Int Position;
    public int RoomA;
    public int RoomB;
}

public class RoomGraph
{
    public Dictionary<int, List<Vector2Int>> Rooms = new();
    public List<DoorInfo> Doors = new();
}

public static class RoomGraphUtility
{
    // Directions used for flood fill.
    static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    // Identify final rooms using the three phase approach described in the documentation.
    public static RoomGraph BuildRoomGraph(Texture2D layout)
    {
        int width = layout.width;
        int height = layout.height;
        int[,] labels = new int[width, height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                labels[x, y] = -1;

        int nextLabel = 0;
        // Phase 1: label contiguous floor regions.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (labels[x, y] != -1) continue;
                if (IsWall(layout.GetPixel(x, y)))
                {
                    labels[x, y] = -2; // mark walls.
                    continue;
                }
                FloodFill(layout, labels, new Vector2Int(x, y), nextLabel);
                nextLabel++;
            }
        }

        // Phase 2: find door candidates.
        HashSet<Vector2Int> doors = FindDoorCandidates(layout);

        // Phase 3: flood fill again treating doors as walls.
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (!IsWall(layout.GetPixel(x, y)))
                    labels[x, y] = -1;

        nextLabel = 0;
        RoomGraph graph = new();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (labels[x, y] != -1) continue;
                if (doors.Contains(new Vector2Int(x, y)))
                {
                    labels[x, y] = -2; // treat door as wall in this phase
                    continue;
                }
                FloodFill(layout, labels, new Vector2Int(x, y), nextLabel, doors);
                nextLabel++;
            }
        }

        // Build dictionary of room cells.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int label = labels[x, y];
                if (label >= 0)
                {
                    if (!graph.Rooms.ContainsKey(label))
                        graph.Rooms[label] = new List<Vector2Int>();
                    graph.Rooms[label].Add(new Vector2Int(x, y));
                }
            }
        }

        // Map doors to room pairs.
        foreach (var door in doors)
        {
            HashSet<int> adjacent = new();
            foreach (var dir in Directions)
            {
                Vector2Int p = door + dir;
                if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) continue;
                int label = labels[p.x, p.y];
                if (label >= 0) adjacent.Add(label);
            }
            if (adjacent.Count == 2)
            {
                var enumerator = adjacent.GetEnumerator();
                enumerator.MoveNext();
                int a = enumerator.Current;
                enumerator.MoveNext();
                int b = enumerator.Current;
                graph.Doors.Add(new DoorInfo { Position = door, RoomA = a, RoomB = b });
            }
        }

        AssimilateHallways(graph);

        return graph;
    }

    // Merge hallway rooms that loop back to a single neighbor.
    static void AssimilateHallways(RoomGraph graph)
    {
        bool changed;
        do
        {
            changed = false;
            foreach (var roomId in new List<int>(graph.Rooms.Keys))
            {
                List<DoorInfo> connected = graph.Doors.FindAll(d => d.RoomA == roomId || d.RoomB == roomId);
                Dictionary<int, int> neighborCounts = new();
                foreach (var d in connected)
                {
                    int other = d.RoomA == roomId ? d.RoomB : d.RoomA;
                    if (neighborCounts.TryGetValue(other, out int count)) neighborCounts[other] = count + 1;
                    else neighborCounts[other] = 1;
                }
                if (neighborCounts.Count == 1)
                {
                    foreach (var pair in neighborCounts)
                    {
                        if (pair.Value > 1) // more than one door to the same room
                        {
                            int target = pair.Key;
                            graph.Rooms[target].AddRange(graph.Rooms[roomId]);
                            graph.Rooms.Remove(roomId);
                            for (int i = graph.Doors.Count - 1; i >= 0; i--)
                            {
                                DoorInfo info = graph.Doors[i];
                                if ((info.RoomA == roomId && info.RoomB == target) || (info.RoomB == roomId && info.RoomA == target))
                                {
                                    graph.Doors.RemoveAt(i);
                                }
                                else
                                {
                                    if (info.RoomA == roomId) { info.RoomA = target; graph.Doors[i] = info; }
                                    if (info.RoomB == roomId) { info.RoomB = target; graph.Doors[i] = info; }
                                }
                            }
                            changed = true;
                        }
                    }
                }
                if (changed) break;
            }
        } while (changed);
    }

    static bool IsWall(Color c) => c == Color.black;

    static void FloodFill(Texture2D layout, int[,] labels, Vector2Int start, int label, HashSet<Vector2Int> treatAsWall = null)
    {
        Queue<Vector2Int> q = new();
        q.Enqueue(start);
        labels[start.x, start.y] = label;
        while (q.Count > 0)
        {
            Vector2Int p = q.Dequeue();
            foreach (var dir in Directions)
            {
                Vector2Int np = p + dir;
                if (np.x < 0 || np.x >= layout.width || np.y < 0 || np.y >= layout.height) continue;
                if (labels[np.x, np.y] != -1) continue;
                if (treatAsWall != null && treatAsWall.Contains(np))
                {
                    labels[np.x, np.y] = -2;
                    continue;
                }
                if (IsWall(layout.GetPixel(np.x, np.y)))
                {
                    labels[np.x, np.y] = -2;
                    continue;
                }
                labels[np.x, np.y] = label;
                q.Enqueue(np);
            }
        }
    }

    // Very simple door detection: a single floor tile with walls on opposite sides.
    static HashSet<Vector2Int> FindDoorCandidates(Texture2D layout)
    {
        int width = layout.width;
        int height = layout.height;
        HashSet<Vector2Int> doors = new();
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (IsWall(layout.GetPixel(x, y))) continue;
                bool north = !IsWall(layout.GetPixel(x, y + 1));
                bool south = !IsWall(layout.GetPixel(x, y - 1));
                bool east = !IsWall(layout.GetPixel(x + 1, y));
                bool west = !IsWall(layout.GetPixel(x - 1, y));

                if (north && south && !east && !west) doors.Add(new Vector2Int(x, y));
                else if (east && west && !north && !south) doors.Add(new Vector2Int(x, y));
            }
        }
        return doors;
    }
}
