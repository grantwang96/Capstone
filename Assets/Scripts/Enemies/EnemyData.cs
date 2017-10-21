using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "EnemyDataObject")]
public class EnemyData : ScriptableObject {

    [Range(50, 200)]
    public int health;
    public CombatType myType;

    public enum CombatType
    {
        Melee,
        Ranged,
        Mixed,
        SpellCaster,
        Support,
    }

    public void doTheThing()
    {
        // Debug.Log("I did a thing!");
    }
}
