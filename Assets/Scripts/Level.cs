using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int Width { get; private set; }
    public int Length { get; private set; }

    List<Room> rooms;
    List<Hallway> hallways;

    public Level(int width, int length)
    {
        Width = width;
        Length = length;

        rooms = new List<Room>();
        hallways = new List<Hallway>();
    }
}
