using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway
{
    public Room StartRoom { get; set; }
    public Room EndRoom { get; set; }

    public Vector2Int StartPositionAbsolute { get { return startPosition + StartRoom.Area.position; } }
    public Vector2Int EndPositionAbsolute { get { return endPosition + EndRoom.Area.position; } }

    Vector2Int startPosition;
    Vector2Int endPosition;

    public Hallway(Vector2Int startPosition, Room startRoom= null)
    {
        this.startPosition = startPosition;
        StartRoom = startRoom;
    }
}
