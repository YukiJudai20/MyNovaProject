using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomWallType
{
    Left,
    Right,
    Up,
    Down
}
public class RoomCheck : MonoBehaviour
{
    public List<GameObject> hideWalls;
    public bool IsRoomValid()
    {
        if (hideWalls.Count == 4)
        {
            Destroy(gameObject);
            return false;
        }

        return true;
    }
}
