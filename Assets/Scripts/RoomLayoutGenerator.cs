using UnityEngine;

public class RoomLayoutGenerator : MonoBehaviour
{
    // These should be powers of 2, otherwise behavior may be undefined.
    [SerializeField] int levelWidth = 64;
    [SerializeField] int levelLength = 64;

    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;

    [SerializeField] GameObject levelLayoutDisplay;

    // Use System.Random instead of Unity's Random to avoid potential overrides from 3rd party sources that change the behavior.
    System.Random random;

    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel()
    {
        random = new System.Random();

        var roomRect = GetStartRoomRect();
        Debug.Log(roomRect);
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
        layoutTexture.DrawRectangle(roomCandidateRect, Color.cyan);
        layoutTexture.SaveAsset();
    }
}
