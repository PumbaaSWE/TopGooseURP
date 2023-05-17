public class DestroyObjective : Objective
{



    void Start()
    {
        GetComponent<Health>().OnDead += OnDead;
    }

    private void OnDead()
    {
        Completed();
    }
}
