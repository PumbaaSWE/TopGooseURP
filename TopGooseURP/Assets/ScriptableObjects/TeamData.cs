using UnityEngine;


[CreateAssetMenu(fileName = "NewTeamData", menuName = "ScriptableObject/TeamData")]
public class TeamData : ScriptableObject
{
    public TeamsManager TeamsManager;
    public string TeamName;
    public string TeamDescription;
}
