using UnityEngine;

public class TeamMember : MonoBehaviour
{

    [SerializeField] private TeamData team;

    public TeamData Team => team;

    private TeamMember currAttacker;
    private float currValue;
    private TeamMember prevAttacker;
    private float prevValue;

    public int Kills { get; private set; }
    public int Deaths { get; private set; }
    public int Assists { get; private set; }
    public int Score { get; private set; }

    private Health health;

    public string info;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        health.OnChangeHealth += OnChangeHealth;
        health.OnDead += OnDeath;
        //team.TeamsManager.AddToTeam(this);
    }

    private void OnChangeHealth(float change, ChangeHealthType damageType, TeamMember attacker)
    {
        if (change < 0)
            Attack(attacker, change);
    }

    // Update is called once per frame
    void Update()
    {
        info = $"Score: {Score} Kills: {Kills} Assists: {Assists} Deaths: {Deaths}";
    }

    public void OnDeath()
    {
        if (currAttacker != null)
        {
            //Debug.Log(gameObject.name + " RewardKill: " + currAttacker.gameObject.name);
            currAttacker.RewardKill(currValue);
            currAttacker = null;
        }
        if (prevAttacker != null)
        {
            //Debug.Log(gameObject.name + " RewardAssist: " + prevAttacker.gameObject.name);
            prevAttacker.RewardAssist(prevValue);
            prevAttacker = null;
        }
        Deaths++;
    }

    private void RewardKill(float value)
    {
        //Debug.Assert(team != null, "team == null");
        //Debug.Assert(team.TeamsManager != null, "team.TeamsManager == null");
        Score += team.TeamsManager.KillScore;
        Kills++;
    }

    private void RewardAssist(float value)
    {
        Assists++;
        if (team.TeamsManager.DamageBasedAssistScore)
            Score += Mathf.RoundToInt(value); //ciel?
        else
            Score += team.TeamsManager.AssistScore;
    }

    //public void Heal(TeamMember healer, float value)
    //{
    //    healer.RewardHeal(team.TeamsManager.HealScore);
    //}

    public void Attack(TeamMember attacker, float value)
    {
        if(attacker != currAttacker)
        {
            //if (attacker == prevAttacker)
            //{
            //    value += prevValue;
            //}
            prevAttacker = currAttacker;
            prevValue = currValue;
            currAttacker = attacker;
            currValue = value;
        }
        else
        {
            currValue += value;
        }
    }

    public void RewardScore(int value)
    {
        Score += value;
    }

    public bool SameTeam(TeamMember teamMember)
    {
        if (teamMember == null) return false;
        return team == teamMember.team;
    }
}
