using UnityEngine;

public class Hallway
{
    public Room StartRoom { get; set; }
    public Room EndRoom { get; set; }

    public Vector2Int StartPositionAbsolute { get { return startPosition + StartRoom.Area.position; } }
    public Vector2Int EndPositionAbsolute { get { return endPosition + EndRoom.Area.position; } }

    // Start direction will be known when the hallway is created and shouldn't change.
    public HallwayDirection StartDirection { get; private set; }
    // End direction will not be known until the connecting room is created, thus we need a setter.
    public HallwayDirection EndDirection { get; set; }

    Vector2Int startPosition;
    Vector2Int endPosition;

    public Hallway(Vector2Int startPosition, HallwayDirection startDirection, Room startRoom= null)
    {
        this.startPosition = startPosition;
        StartRoom = startRoom;

        StartDirection = startDirection;
    }
}
