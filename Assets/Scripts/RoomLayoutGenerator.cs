using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;
using UnityEngine;

public class RoomLayoutGenerator : MonoBehaviour
{
    [Header("Level Layout Settings")]
    [SerializeField] int seed = Environment.TickCount;

    // Width and length should be powers of 2, otherwise behavior may be undefined.
    [SerializeField] int levelWidth = 64;
    [SerializeField] int levelLength = 64;
    [SerializeField] int levelPadding = 1;
    [SerializeField] int roomMargin = 1;
    [SerializeField] int roomCountMin = 3;
    [SerializeField] int roomCountMax = 5;

    [Header("Room Settings")]
    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;
    [SerializeField] int doorwayDistanceFromCorner = 1;

    [Header("Hallway Settings")]
    [SerializeField] int hallwayWidthMin = 2;
    [SerializeField] int hallwayWidthMax = 3;

    [Header("Level Layout Display")]
    [SerializeField] GameObject levelLayoutDisplay;
    [SerializeField] List<Hallway> openDoorways;
    [SerializeField] bool _enableDebuggingInfo = false;

    // Use System.Random instead of Unity's Random to avoid potential overrides from 3rd party sources that change the behavior.
    Random random;
    Level level;


    [ContextMenu("Generate New Seed")]
    public void GenerateNewSeed()
    {
        seed = Environment.TickCount;
    }

    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel()
    {
        random = new Random(seed);
        level = new Level(levelWidth, levelLength);
        openDoorways = new List<Hallway>();

        var roomRect = GetStartRoomRect();
        Room startRoom = new Room(roomRect);
        level.AddRoom(startRoom);
        // TODO: Seems like we should just pass the Room object to CalcAllPossibleDoorways.
        List<Hallway> hallways = startRoom.CalcAllPossibleDoorways(startRoom.Area.width, startRoom.Area.height, doorwayDistanceFromCorner);
        foreach (Hallway h in hallways)
        {
            // Set the start room for each possible hallway to the level's start room.
            h.StartRoom = startRoom;
            // Add the possible hallway to the list of open doorways.
            openDoorways.Add(h);
        }

        AddRooms();

        DrawLayout();
    }

    [ContextMenu("Generate New Seed And New Level")]
    public void GenerateNewSeedAndNewLevel()
    {
        GenerateNewSeed();
        GenerateLevel();
    }

    /// <summary>
    /// Find a random rectangle in the level layout. The rect will be in the center of the level.
    /// </summary>
    /// <returns>A rectangle that is in the center of the level.</returns>
    private RectInt GetStartRoomRect()
    {
        int roomWidth = random.Next(roomWidthMin, roomWidthMax + 1);
        // Imagine folding a piece of paper in half and cutting off the randomly chosen room width.
        //  When we add back a quarter of the level width later, we will have an x-coord in the middle two quarters of the level.
        int availableWidthX = levelWidth / 2 - roomWidth;
        int randomX = random.Next(0, availableWidthX);
        int roomX = randomX + (levelWidth / 4);

        int roomLength = random.Next(roomLengthMin, roomLengthMax + 1);
        int availableLengthY = levelLength / 2 - roomLength;
        int randomY = random.Next(0, availableLengthY);
        int roomY = randomY + (levelLength / 4);

        return new RectInt(roomX, roomY, roomWidth, roomLength);
    }

    /// <summary>
    /// Draw the level layout to the texture. This will be used to display the level layout in the editor.
    /// </summary>
    /// <param name="roomCandidateRect">The rectangle that will be drawn in the level layout.</param>
    private void DrawLayout()
    {
        var renderer = levelLayoutDisplay.GetComponent<Renderer>();

        // This will allow us to change the texture from the editor. But this is usually not recommended.
        var layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;

        // We must do this if the width and height has changed since the last time we generated a texture.
        layoutTexture.Reinitialize(levelWidth, levelLength);
        levelLayoutDisplay.transform.localScale = new Vector3(levelWidth, levelLength, 1f);

        // Fill the whole texture black and then just fill our new room cyan.
        // TODO: This will likely need to change once we start adding more rooms.
        layoutTexture.FillWithColor(Color.black);

        Array.ForEach(level.Rooms, room => layoutTexture.DrawRectangle(room.Area, Color.white));
        Array.ForEach(level.Hallways, hallway => layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, Color.white));

        if (_enableDebuggingInfo)
        {
            // Mark open doorways with a differently colored pixel. The color is determine by the direction of the hallway.
            openDoorways.ForEach(h => layoutTexture.SetPixel(h.StartPositionAbsolute.x, h.StartPositionAbsolute.y, h.StartDirection.GetColor()));
        }

        layoutTexture.SaveAsset();
    }

    /// <summary>
    /// Select a random hallway from the list of possible doorways. The selected hallway is the next room's entrance.
    /// </summary>
    /// <param name="roomCandidate">The rectangle that is used to create the next room.</param>
    /// <param name="currentRoomExit">The current room's exit. This is used to determine the direction of the next room's entrance.</param>
    /// <returns>A hallway with a relative starting position to the roomCandidate. The start direction will point towards currentRoomExit.</returns>
    private Hallway SelectHallwayCandidate(RectInt roomCandidate, Hallway currentRoomExit)
    {
        // We're currently assuming that we'll always have space for the next room. That'll have to change
        //  or else we'll eventually run out of bounds.
        Room nextRoom = new Room(roomCandidate);

        List<Hallway> hallwayCandidates = nextRoom.CalcAllPossibleDoorways(roomCandidate.width, roomCandidate.height, doorwayDistanceFromCorner);
        HallwayDirection requiredDirection = currentRoomExit.StartDirection.GetOppositeDirection();
        List<Hallway> filteredHallwayCandidates = hallwayCandidates.Where(h => h.StartDirection == requiredDirection).ToList();

        return filteredHallwayCandidates.Count > 0 ? filteredHallwayCandidates[random.Next(filteredHallwayCandidates.Count)] : null;
    }

    /// <summary>
    /// Calculates the next room's position based on the current room's exit and the next room's entrance.
    /// </summary>
    /// <param name="roomWidth">The width of the next room.</param>
    /// <param name="roomHeight">The height of the next room.</param>
    /// <param name="distance">The distance between the current room's exit and the next room's entrance.</param>
    /// <param name="currentRoomExit">The current room's exit. This is used to determine the position and direction of the next room's entrance.</param>
    /// <param name="nextRoomEntrancePosition">The next room's entrance position. This is used to determine the position of the next room.</param>
    /// <returns>The position of the next room.</returns>
    private Vector2Int CalcNextRoomPosition(int roomWidth, int roomHeight, int distance, Hallway currentRoomExit, Vector2Int nextRoomEntrancePosition)
    {
        // A Room's position is the bottom left corner of the room.
        Vector2Int roomPosition = currentRoomExit.StartPositionAbsolute;
        switch (currentRoomExit.StartDirection)
        {
            case HallwayDirection.Top:
                roomPosition.x -= nextRoomEntrancePosition.x;
                roomPosition.y += distance + 1;
                break;
            case HallwayDirection.Right:
                roomPosition.x += distance + 1;
                roomPosition.y -= nextRoomEntrancePosition.y;
                break;
            case HallwayDirection.Bottom:
                roomPosition.x -= nextRoomEntrancePosition.x;
                roomPosition.y -= distance + roomHeight;
                break;
            case HallwayDirection.Left:
                roomPosition.x -= distance + roomWidth;
                roomPosition.y -= nextRoomEntrancePosition.y;
                break;
        }

        return roomPosition;
    }

    private Room ConstructNextRoom(Hallway currentRoomExit)
    {
        RectInt roomCandidateRect = new RectInt {
            width = random.Next(roomWidthMin, roomWidthMax + 1),
            height = random.Next(roomLengthMin, roomLengthMax + 1)
        };

        // We're creating the hallway for the next Room before we create the room itself. The order of operations seems backwards.
        //  I think we're doing it this way so that hallways are always straight. The room's position is based off of the hallway,
        //  instead of the other way around. But, if hallways are always straight, that'll look boring after a while.
        Hallway nextRoomEntrance = SelectHallwayCandidate(roomCandidateRect, currentRoomExit);
        if (nextRoomEntrance == null) return null;

        Vector2Int roomCandidatePosition = CalcNextRoomPosition(
            roomCandidateRect.width,
            roomCandidateRect.height,
            random.Next(hallwayWidthMin, hallwayWidthMax + 1),
            currentRoomExit,
            nextRoomEntrance.StartPosition
        );
        roomCandidateRect.position = roomCandidatePosition;

        if (!IsRoomCandidateValid(roomCandidateRect)) return null;

        Room nextRoom = new Room(roomCandidateRect);
        currentRoomExit.EndRoom = nextRoom;
        currentRoomExit.EndPosition = nextRoomEntrance.StartPosition;

        return nextRoom;
    }

    private void AddRooms()
    {
        int roomCount = random.Next(roomCountMin, roomCountMax + 1);

        Hallway currentRoomExit;
        while (openDoorways.Count > 0 && level.Rooms.Length < roomCount)
        {
            currentRoomExit = openDoorways[random.Next(openDoorways.Count)];
            Room newRoom = ConstructNextRoom(currentRoomExit);

            if (newRoom == null)
            {
                // If newRoom failed to be created, we need to remove that hallway from the list of open doorways.
                // TODO: Maybe the room failed to construct bc it was slightly too big. Could we try smaller sizes before we remove the doorway?
                openDoorways.Remove(currentRoomExit);
                continue;
            }

            level.AddRoom(newRoom);
            currentRoomExit.EndRoom = newRoom;
            level.AddHallway(currentRoomExit);
            openDoorways.Remove(currentRoomExit);

            // Get all new doorways
            List<Hallway> newDoorways = newRoom.CalcAllPossibleDoorways(newRoom.Area.width, newRoom.Area.height, doorwayDistanceFromCorner);
            // TODO: This should probably happen in CalcAllPossibleDoorways
            newDoorways.ForEach(h => h.StartRoom = newRoom);
            // Add all new doorways that do not point in the direction we just came from
            openDoorways.AddRange(newDoorways.Where(h => h.StartDirection != currentRoomExit.StartDirection.GetOppositeDirection()));
        }
    }

    private bool IsRoomCandidateValid(RectInt roomCandidateRect)
    {
        RectInt levelRect = new RectInt
        {
            xMin = levelPadding,
            yMin = levelPadding,
            width = levelWidth - (2 * levelPadding),// multiply by 2 to account for left and right sides
            height = levelLength - (2 * levelPadding)// multiply by 2 to account for top and bottom sides
        };

        return levelRect.Contains(roomCandidateRect) && !CheckRoomOverlap(roomCandidateRect, level.Rooms, level.Hallways, roomMargin);
    }

    // TODO: Could this be used to check hallway overlap?
    private bool CheckRoomOverlap(RectInt roomCandidateRect, Room[] rooms, Hallway[] hallways, int minRoomDistance)
    {
        RectInt paddedRoomRect = new RectInt
        {
            x = roomCandidateRect.x - minRoomDistance,
            y = roomCandidateRect.y - minRoomDistance,
            width = roomCandidateRect.width + (2 * minRoomDistance),
            height = roomCandidateRect.height + (2 * minRoomDistance)
        };

        foreach (Room room in rooms)
        {
            if (paddedRoomRect.Overlaps(room.Area))
            {
                return true;
            }
        }

        foreach (Hallway hallway in hallways)
        {
            if (paddedRoomRect.Overlaps(hallway.Area))
            {
                return true;
            }
        }

        return false;
    }

}
