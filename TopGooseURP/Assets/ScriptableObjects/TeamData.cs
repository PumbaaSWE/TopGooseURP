using UnityEngine;


[CreateAssetMenu(fileName = "NewTeamData", menuName = "ScriptableObject/TeamData")]
public class TeamData : ScriptableObject
{
    public TeamsManager TeamsManager;
    public int Team;
    public string TeamName;
    public string TeamDescription;


    public static bool operator ==(TeamData a, TeamData b)
        => a.Team == b.Team;
    public static bool operator !=(TeamData a, TeamData b)
        => a.Team != b.Team;
}
