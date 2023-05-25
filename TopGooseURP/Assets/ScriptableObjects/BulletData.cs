using UnityEngine;

[CreateAssetMenu(fileName = "NewBulletData", menuName = "ScriptableObject/BulletData")]
public class BulletData : ScriptableObject
{
    
    public float timeToLive;

    public bool hasTrail; // another ScriptableObject with trail data?

    public TrailConfig trailConfig;

    public float speed; //m/s

    public float damage;

    public DamageType damageType;
    public LayerMask hitLayer;
    public float gravity = 10;
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
