using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWall : MonoBehaviour
{
    public GridType _gridType;
    private void OnTriggerEnter(Collider other)
    {
        switch (_gridType)
        {
            case GridType.Room:
                if (other.tag == "Hallway" || other.tag == "Stairs")
                {
                    Destroy(gameObject);
                }
                break;
        }
    }
}
