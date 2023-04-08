using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    public bool IsActive { get;  protected set; }

    internal abstract void Activate();

    internal abstract void Deactivate();

    internal abstract void Deploy();
}
