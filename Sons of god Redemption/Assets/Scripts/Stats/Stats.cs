using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newStats", menuName = "Stats")]

public class Stats : ScriptableObject{

    public int health, movementSpeed, baseAttack;
    public float movingRange, rotationSpeed, ViewDistance, hearDistance, attackDistance, attackSpeed;
  
}
