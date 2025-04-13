using UnityEngine;

// Currently, hallways are assumed to be 1 pixel wide and straight.
public class Hallway
{
    public Room StartRoom { get; set; }
    public Room EndRoom { get; set; }

    public Vector2Int StartPositionAbsolute => StartPosition + StartRoom.Area.position;
    public Vector2Int EndPositionAbsolute => EndPosition + EndRoom.Area.position;

    // Start direction will be known when the hallway is created and shouldn't change.
    public HallwayDirection StartDirection { get; private set; }
    // End direction will not be known until the connecting room is created, thus we need a setter.
    public HallwayDirection EndDirection { get; set; }
    
    public Vector2Int StartPosition { get; set; }
    public Vector2Int EndPosition { get; set; }

    // Note that a hallway will always overlap with the rooms it connects. For example, if there's a 3 pixel gap between the
    //  rooms, the hallway will be 5 pixels long.
    public Hallway(Vector2Int startPosition, HallwayDirection startDirection, Room startRoom= null)
    {
        StartPosition = startPosition;
        StartRoom = startRoom;

        StartDirection = startDirection;
    }
}
