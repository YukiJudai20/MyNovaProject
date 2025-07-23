using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayCheck : MonoBehaviour
{
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject upWall;
    public GameObject downWall;
    
    Vector3Int[] neighbors = {
        //平面移动
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
    };
    
    public void CheckAndDelWall(Grid3D<DungeonGenerator.CellType> grid, Vector3Int pos)
    {
        foreach (var offset in neighbors)
        {
            if(grid[pos+offset]!=DungeonGenerator.CellType.None)
            {
                if (offset.x == -1)
                {
                    Destroy(leftWall);
                }
                else if (offset.x == 1)
                {
                    Destroy(rightWall);
                }
                else if(offset.z == 1)
                {
                    Destroy(upWall);
                }
                else
                {
                    Destroy(downWall);
                }
            }    
        }
    }
}
