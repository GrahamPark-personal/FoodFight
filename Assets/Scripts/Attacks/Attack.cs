using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    int Damage = 0;
    int Health = 0;
    int Slow = 0;
    int Stun = 0;
    int Taunt = 0;
    int Poison = 0;
    int Range = 0;
    int Radius = 0;
    int AOE = 0;
    int EffectDuration = 0;
    int DamageDuration = 0;
    int id = 0;
    IntVector2 mAttackPos;
    public static int ID;

    public virtual void Init(){}

    public virtual void Exit() {} 

    public void CreateID()
    {
        id = ID;
        ID++;
    }

    //if it returns false the removes the attack from the attackList in the attackManager
    public virtual void Execute(IntVector2 pos) { }

    #region Gets

    public int GetDamage() { return Damage; }
    public int GetSlow() { return Slow; }
    public int GetStun() { return Stun; }
    public int GetTaunt() { return Taunt; }
    public int GetPoison() { return Poison; }
    public int GetRange() { return Range; }
    public int GetRadius() { return Radius; }
    public int GetAOE() { return AOE; }
    public int GetEffectDuration() { return EffectDuration; }
    public int GetDamageDuration() { return DamageDuration; }
    public int GetID() { return id; }
    public int GetHealth() { return Health; }

    #endregion

    #region Sets

    public void SetDamage(int amount) { Damage = amount; }
    public void SetSlow(int amount) { Slow = amount; }
    public void SetStun(int amount) { Stun = amount; }
    public void SetTaunt(int amount) { Taunt = amount; }
    public void SetPoison(int amount) { Poison = amount; }
    public void SetRange(int amount) { Range = amount; }
    public void SetRadius(int amount) { Radius = amount; }
    public void SetAOE(int amount) { AOE = amount; }
    public void SetEffectDuration(int amount) { EffectDuration = amount; }
    public void SetDamageDuration(int amount) { DamageDuration = amount; }
    public void SetHealth(int amount) { Health = amount; }


    #endregion
}
