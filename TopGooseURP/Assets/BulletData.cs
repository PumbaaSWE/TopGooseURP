using UnityEngine;


[CreateAssetMenu(menuName = "BulletData")]
public class BulletData : ScriptableObject
{
    
    public float timeToLive;
    public float trailTime; // another ScriptableObject with trail data?
    public float speed; //m/s

    public float damage;
    //public float size?;
    //public float mass; // assumed 1 (one) always...

    //public (Mesh)Renderer mesh; // for the looks?
    //other things?


    //dV=I/m ... dV = delta Speed, I = Impulse force, m = mass and ez math mecause it's 1...
    public float ImpulseFromSpeed
    {
        get { return speed; }
    }
}
