using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public RectInt Area { get; private set; }

    public Room(RectInt area)
    {
        Area = area;
    }

    /// <summary>
    /// Calculates all possible doorways for a room. The doorways are the walls of the room, minus the distance from the corners.
    /// </summary>
    /// <param name="width">The width of the room.</param>
    /// <param name="height">The height of the room.</param>
    /// <param name="minDistFromCorner">The minimum distance from the corner of the room.</param>
    /// <returns>A list of hallways. The start positions of the hallways will be relative to the room they are assigned to.</returns>
    public List<Hallway> CalcAllPossibleDoorways(int width, int height, int minDistFromCorner)
    {
        List<Hallway> hallwayCandidates = new List<Hallway>();

        int minX = minDistFromCorner;
        int maxX = width - minDistFromCorner;
        for(int x = minX; x < maxX; x++)
        {
            // Top, aka north wall
            hallwayCandidates.Add(new Hallway(new Vector2Int(x, height - 1), HallwayDirection.Top));
            // Bottom, aka south wall
            hallwayCandidates.Add(new Hallway(new Vector2Int(x, 0), HallwayDirection.Bottom));
        }

        int minY = minDistFromCorner;
        int maxY = height - minDistFromCorner;
        for (int y = minY; y < maxY; y++)
        {
            // Left, aka west wall
            hallwayCandidates.Add(new Hallway(new Vector2Int(0, y), HallwayDirection.Left));
            // Right, aka east wall
            hallwayCandidates.Add(new Hallway(new Vector2Int(width - 1, y), HallwayDirection.Right));
        }

        return hallwayCandidates;
    }
}
