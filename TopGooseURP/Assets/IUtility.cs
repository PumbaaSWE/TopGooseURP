using System;
using System.Runtime.Serialization;
using UnityEngine;

public interface IUtility
{
    public float Evaluate();
    public void Execute();

    public void Exit();
}