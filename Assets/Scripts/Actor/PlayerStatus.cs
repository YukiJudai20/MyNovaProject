using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : ScriptableObject
{
    public List<int> skillIds = new List<int>();
    public int hp;
    public int maxHp;
    public int def;
    public int atk;
}
