using UnityEngine;

public class FlyCourse : MonoBehaviour
{

    [SerializeField] private bool enforceOrder = true;
    [SerializeField] private Color highlightColor;

    [SerializeField] private FlyTarget[] flyTargets;
    private int targetsLeft;

    public delegate void FlyCourseCompleteEvent();
    public event FlyCourseCompleteEvent OnFlyCourseComplete;

    // Start is called before the first frame update
    void Start()
    {
        //flyTargets = GetComponentsInChildren<FlyTarget>();
        targetsLeft = flyTargets.Length;
        for (int i = 0; i < flyTargets.Length; i++)
        {
            flyTargets[i].OnTargetHit += FlyCourseOnTargetHit;
        }
        flyTargets[0].SetColor(highlightColor);
    }

    private void FlyCourseOnTargetHit(FlyTarget flyTarget)
    {
        if(enforceOrder && flyTarget != flyTargets[^targetsLeft])
        {
            return;
        }

        
        --targetsLeft;
        //flyTargets[^targetsLeft].SetColor(highlightColor);
        flyTarget.ConfirmHit();
        if (targetsLeft == 0)
        {
            OnFlyCourseComplete?.Invoke();
        }
        else
        {
            flyTargets[^targetsLeft].SetColor(highlightColor);
        }
    }
}
