using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyInterface
{
    public int Health { get; set; }

    public void AttackHero();

    public void HurtEnemy(int damage);

}