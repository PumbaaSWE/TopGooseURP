using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewTeamsManager", menuName = "ScriptableObject/TeamsManager")]
public class TeamsManager : ScriptableObject
{

    private List<TeamMember>[] members = new List<TeamMember>[3];
    private readonly Dictionary<int, List<TeamMember>> teams = new();

    public int KillScore = 100;
    public bool DamageBasedAssistScore = true;
    public int AssistScore = 50;

    public bool TeamKill = false;

    public void AddToTeam(TeamMember teamMember)
    {
        if(teams.TryGetValue(teamMember.Team, out List<TeamMember> members)){
            members.Add(teamMember);
        }
        else
        {
            members = new() { teamMember };
            teams.Add(teamMember.Team, members);
        }
    }
}
