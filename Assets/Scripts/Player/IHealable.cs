using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealable
{
    bool Heal(int healthDelta, int cost);
}
