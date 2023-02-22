using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PIDController
{

    //PID coefficients
    public float proportionalGain;
    public float integralGain;
    public float derivativeGain;

    public float outputMin = -1;
    public float outputMax = 1;
    public float integralSaturation;

    public float errorLast;
    public float valueLast;

    public float integrationStored;
  //  public float velocity;  //only used for the info display
    public bool derivativeInitialized;


    public float UpdatePID(float currentValue, float targetValue, float deltaTime)
    {
        float error = targetValue - currentValue;

        //calculate P term
        float P = proportionalGain * error;

        //calculate I term
        integrationStored = Mathf.Clamp(integrationStored + (error * deltaTime), -integralSaturation, integralSaturation);
        float I = integralGain * integrationStored;

        float errorRateOfChange = (error - errorLast) / deltaTime;
        errorLast = error;

        float valueRateOfChange = (currentValue - valueLast) / deltaTime;
        valueLast = currentValue;

        float D = derivativeGain * valueRateOfChange;

        float result = P + I + D;

        return Mathf.Clamp(result, outputMin, outputMax);
    }

    private static float AngleDifference(float a, float b)
    {
        return (a - b + 540) % 360 - 180;   //calculate modular difference, and remap to [-180, 180]
    }
}
