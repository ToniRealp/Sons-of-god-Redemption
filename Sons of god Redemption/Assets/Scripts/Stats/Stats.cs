using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newStats", menuName = "Stats")]

public class Stats : ScriptableObject{

    public int health;
    public int movementSpeed;
    public int baseAttack;
    public int attackSpeed;
  
}
