using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsCheck : MonoBehaviour
{
    public GameObject leftWall;
    public GameObject rightWall;
    
    Vector3Int[] neighbors = {
        //平面移动
        //第一层左右四格
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 0, 1),
        new Vector3Int(-1, 0, 1),
        //上一层左右四格
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(1, 1, 1),
        new Vector3Int(-1, 1, 1),
    };
    
    public void CheckAndDelStairsWall(Grid3D<DungeonGenerator.CellType> grid, Vector3Int pos)
    {
        foreach (var offset in neighbors)
        {
            if(grid[pos+offset]==DungeonGenerator.CellType.Room)
            {
                if (offset.x == -1)
                {
                    //Destroy(leftWall);
                }
                else if (offset.x == 1)
                {
                    //Destroy(rightWall);
                }
            }    
        }
    }
}
