using UnityEngine;

public readonly struct DamageInfo
{
    public readonly TeamMember dealer;
    public readonly float amount;
    public readonly Vector3 point;
    public readonly float force;
    public readonly DamageType type;
    public readonly float radius;

    public DamageInfo(TeamMember dealer, float amount, DamageType type)
    {
        this.dealer = dealer;
        this.amount = amount;
        this.point = Vector3.zero;
        this.force = 0.0f;
        this.type = type;
        this.radius = 0;
    }

    public DamageInfo(TeamMember dealer, float amount, DamageType type, Vector3 point)
    {
        this.dealer = dealer;
        this.amount = amount;
        this.point = point;
        this.force = 0;
        this.type = type;
        this.radius = 0;
    }

    public DamageInfo(TeamMember dealer, float amount, DamageType type, Vector3 point, float force, float radius)
    {
        this.dealer = dealer;
        this.amount = amount;
        this.point = point;
        this.force = force;
        this.type = type;
        this.radius = radius;
    }
}
