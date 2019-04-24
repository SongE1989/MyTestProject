using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BTSharedEntry : SharedVariable<XXEntry>
{
    public static implicit operator BTSharedEntry(XXEntry value) { return new BTSharedEntry { Value = value }; }
}
