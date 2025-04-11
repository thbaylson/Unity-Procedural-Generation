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
}
