using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatSystem: ILifeSystem
{
    bool IsInCombat { get; set; }
    HashSet<XXEntry> EnemyList { get; set; }
}
