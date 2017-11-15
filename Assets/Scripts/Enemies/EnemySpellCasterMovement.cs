using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellCasterMovement : Movement, SpellCaster
{
    #region Public Variables
    SpellBook heldSpell;
    #endregion
    #region Movement Implementations

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override IEnumerator attack(Vector3 target)
    {
        hamper++;
        anim.Play("Attack");
        bool fired = false;
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if(anim.GetCurrentAnimatorStateInfo(0).length >= 0.5f && !fired) {
                // Debug.Log("Pew");
                fired = true;
                // heldSpell.primaryCast();
            }
            yield return new WaitForEndOfFrame();
        }
        hamper--;
    }

    #endregion

    #region SpellCaster Implementations
    public void AddSpell(SpellBook newSpell)
    {
        
    }

    public void addToSeductionList(damageable loser)
    {
        
    }

    public void DropSpell(SpellBook dropSpell)
    {
        if(heldSpell == dropSpell)
        {
            heldSpell.transform.parent = null;
            heldSpell = null;
        }
    }

    public void getHitList(List<damageable> hitList, SpellCaster owner)
    {
        
    }

    public void initiateSeduction(float duration)
    {
        
    }

    public void removeFromSeductionList(damageable loser)
    {
        
    }
    #endregion
}
