using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDamageable : damageable {

    Coroutine seduction;

    public override void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        base.Seduce(duration, target, owner);
        if(seduction != null) {
            StopCoroutine(seduction);
            myMovement.hamper--;
        }
        seduction = StartCoroutine(processSeduction(duration, target, owner));
    }

    IEnumerator processSeduction(float duration, GameObject target, SpellCaster owner)
    {
        myMovement.hamper++;
        float startTime = Time.time;
        NPCState previousState = myMovement.getCurrentState();
        becomeSeduced(myMovement.myType);
        while(Time.time - startTime < duration)
        {
            yield return new WaitForEndOfFrame();
        }
        myMovement.hamper--;
        seduction = null;
    }

    void becomeSeduced(EnemyData.CombatType combatType)
    {
        switch (combatType)
        {
            case EnemyData.CombatType.Melee:
                myMovement.changeState(new MeleeEnemySeduced());
                break;
            case EnemyData.CombatType.SpellCaster:
                break;
            case EnemyData.CombatType.Mixed:
                break;
            case EnemyData.CombatType.Ranged:
                break;
            case EnemyData.CombatType.Support:
                break;
        }
    }

    public override void Die()
    {
        ScoreKeeper.Instance.incrementScore();
        base.Die();
    }
}
