using UnityEngine;


[CreateAssetMenu(fileName = "TimerManager", menuName = "ScriptableObject/TimerManager")]
public class SpeedRunTimie : ScriptableObject
{
    float startTime;

    public void StartTimer()
    {
        startTime = Time.time;
    }

    public float StopTimeer()
    {
        return Time.time - startTime;
    }
}
