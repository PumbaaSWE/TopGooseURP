using System;
using UnityEngine;

public static class TargetingMath
{
    /// <summary>
    /// Compute if impact can occur and if true you get out impact location and time to impact
    /// </summary>
    /// <param name="tPos">target position</param>
    /// <param name="tVel">target velocity</param>
    /// <param name="pPos">projectile position</param>
    /// <param name="pSpeed">projectile speed</param>
    /// <param name="location">impact location</param>
    /// <param name="tti">time to impact in seconds</param>
    /// <returns>true if impact can occur, otherwise false</returns>
    public static bool ComputeImpact(Vector3 tPos, Vector3 tVel, Vector3 pPos, float pSpeed, out Vector3 location, out float tti)
    {
        location = Vector3.zero;
        tti = 0;

        Vector3 relPos = tPos - pPos; //relative position
        float a = Vector3.Dot(tVel, tVel) - pSpeed * pSpeed;
        float b = 2 * Vector3.Dot(tVel, relPos);
        float c = Vector3.Dot(relPos, relPos);

        float discriminant = b * b - 4 * a * c;
        if (discriminant <= 0) return false;

        float p = -b / (2 * a);
        float q = MathF.Sqrt(discriminant) / (2 * a);

        float t1 = p - q;
        float t2 = p + q;

        if (t1 > t2 && t2 > 0) tti = t2;
        else tti = t1;
        if(tti < 0) return false;

        location = tPos + tVel * tti;
        return true;
    }

    /// <summary>
    /// Compute if impact can occur and if true you get out direction vector to achieve impact
    /// </summary>
    /// <param name="tPos">target position</param>
    /// <param name="tVel">target velocity</param>
    /// <param name="pPos">projectile position</param>
    /// <param name="pSpeed">projectile speed</param>
    /// <param name="pVel">velocity of projectile to achieve impact</param>
    /// <returns>true if impact can occur, otherwise false</returns>
    public static bool ComputeVector(Vector3 tPos, Vector3 tVel, Vector3 pPos, float pSpeed, out Vector3 pVel)
    {
        pVel = Vector3.zero;
        Vector3 LOS = tPos - pPos; //Line of sight vector aka relative position
        Vector3 center = tPos + tVel; //Circle center
        Vector3 toCenter = pPos - center; //vector to circle center ((pos.X - C.X),(pos.Y - C.Y))

        float a = Vector3.Dot(LOS, LOS);            //LOS.X * LOS.X + LOS.Y * LOS.Y;
        float b = 2 * Vector3.Dot(LOS, toCenter);   //2 * LOS.X * (pos.X - C.X) + 2 * LOS.Y * (pos.Y - C.Y);
        float c = Vector3.Dot(toCenter, toCenter) - pSpeed * pSpeed;  //(pos.X - C.X) * (pos.X - C.X) + (pos.Y - C.Y) * (pos.Y - C.Y) - speed * dt * speed * dt;

        float discriminant = b * b - 4 * a * c;
        if (discriminant <= 0)
            return false;

        /*
         * https://en.wikipedia.org/wiki/Quadratic_equation#Avoiding_loss_of_significance 
         */
        float t = 2 * c / (-b + MathF.Sqrt(discriminant));

        Vector3 pointOnLOS = LOS * t + pPos;
        pVel = center - pointOnLOS;

        return true;
    }
}
