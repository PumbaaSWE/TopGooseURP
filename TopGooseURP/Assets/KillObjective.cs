
using UnityEngine;

[RequireComponent(typeof(TeamMember))]
public class KillObjective : Objective
{

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TeamMember>().OnKillCallback += GotAKill;
    }

    private void GotAKill(TeamMember teamMember, int score)
    {
        CountOne();
    }
}
