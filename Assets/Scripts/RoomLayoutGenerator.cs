using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomLayoutGenerator : MonoBehaviour
{
    [Header("Level Layout Settings")]
    // These should be powers of 2, otherwise behavior may be undefined.
    [SerializeField] int levelWidth = 64;
    [SerializeField] int levelLength = 64;

    [Header("Room Settings")]
    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;
    [SerializeField] int doorwayDistanceFromCorner = 1;

    [Header("Level Layout Display")]
    [SerializeField] GameObject levelLayoutDisplay;
    [SerializeField] List<Hallway> openDoorways;

    // Use System.Random instead of Unity's Random to avoid potential overrides from 3rd party sources that change the behavior.
    System.Random random;
    Level level;

    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel()
    {
        random = new System.Random();
        openDoorways = new List<Hallway>();
        level = new Level(levelWidth, levelLength);

        var roomRect = GetStartRoomRect();
        Debug.Log(roomRect);

        Room startRoom = new Room(roomRect);
        // TODO: Seems like we should just pass the Room object to CalcAllPossibleDoorways.
        List<Hallway> hallways = startRoom.CalcAllPossibleDoorways(startRoom.Area.width, startRoom.Area.height, doorwayDistanceFromCorner);
        foreach(Hallway h in hallways)
        {
            // Set the start room for each possible hallway to the level's start room.
            h.StartRoom = startRoom;
            // Add the possible hallway to the list of open doorways.
            openDoorways.Add(h);
        }

        // TODO: Test Code. Remove Later.
        Room testRoom1 = new Room(new RectInt(3, 6, 6, 10));
        Room testRoom2 = new Room(new RectInt(15, 4, 10, 12));
        Hallway testHallway1 = new Hallway(new Vector2Int(6, 3), HallwayDirection.Right, testRoom1);
        testHallway1.EndPosition = new Vector2Int(0, 5);
        testHallway1.EndRoom = testRoom2;
        level.AddRoom(testRoom1);
        level.AddRoom(testRoom2);
        level.AddHallway(testHallway1);
        // End Test Code

        DrawLayout(roomRect);
    }

    /// <summary>
    /// Find a random rectangle in the level layout. The rect will be in the center of the level.
    /// </summary>
    /// <returns>A rectangle that is in the center of the level.</returns>
    private RectInt GetStartRoomRect()
    {
        int roomWidth = random.Next(roomWidthMin, roomWidthMax);
        // Imagine folding a piece of paper in half and cutting off the randomly chosen room width.
        //  When we add back a quarter of the level width later, we will have an x-coord in the middle two quarters of the level.
        int availableWidthX = levelWidth / 2 - roomWidth;
        int randomX = random.Next(0, availableWidthX);
        int roomX = randomX + (levelWidth / 4);

        int roomLength = random.Next(roomLengthMin, roomLengthMax);
        int availableLengthY = levelLength / 2 - roomLength;
        int randomY = random.Next(0, availableLengthY);
        int roomY = randomY + (levelLength / 4);
        
        return new RectInt(roomX, roomY, roomWidth, roomLength);
    }

    /// <summary>
    /// Draw the level layout to the texture. This will be used to display the level layout in the editor.
    /// </summary>
    /// <param name="roomCandidateRect">The rectangle that will be drawn in the level layout.</param>
    void DrawLayout(RectInt roomCandidateRect= new RectInt())
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

        layoutTexture.DrawRectangle(roomCandidateRect, Color.white);

        // Mark open doorways with a red pixel.
        openDoorways.ForEach(h => layoutTexture.SetPixel(h.StartPositionAbsolute.x, h.StartPositionAbsolute.y, h.StartDirection.GetColor()));

        layoutTexture.SaveAsset();
    }
}
