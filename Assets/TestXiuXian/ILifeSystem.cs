using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILifeSystem
{
    int MaxHealthPoint { get; set; }
    int HealthPoint { get; set; }
    bool IsDead { get; set; }
}
