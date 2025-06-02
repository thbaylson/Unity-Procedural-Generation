using UnityEngine;

public class TextureBasedLevel : ILevel
{
    // Unity's built in yellow has a very specific value, so let's use this for simplicity when creating new textures.
    public static Color yellow = new Color(1f, 1f, 0f, 1f);
    // Brown for caves. (#5C3B14)
    public static Color brown = new Color(0.36f, 0.23f, 0.08f);

    Texture2D levelTexture;
    public TextureBasedLevel(Texture2D levelTexture)
    {
        this.levelTexture = levelTexture;
    }
    public int Width => levelTexture.width;

    public int Length => levelTexture.height;


    public int Floor(int x, int y)
    {
        return 0;
    }

    public bool IsBlocked(int x, int y)
    {
        if(x < 0 || x >= levelTexture.width || y < 0 || y >= levelTexture.height) return true;

        Color pixel = levelTexture.GetPixel(x, y);
        return !Color.white.Equals(pixel);
    }
}
