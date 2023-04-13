using UnityEngine;


public abstract class Objective : MonoBehaviour
{

    [SerializeField] private ObjectiveEventManager manager;
    [Space]
    [SerializeField] protected int score;
    [SerializeField] protected string title;
    [SerializeField] protected string description;
    [SerializeField] protected Texture2D icon;
    [SerializeField] private bool isPrimary;
    [SerializeField] protected bool shownOnMap;
    public bool IsComplete { get; private set; }


    public int Score => score;
    public string Title => title;
    public string Description => description;
    public Texture2D Icon => icon;
    public bool IsPrimary => isPrimary;
    public bool ShownOnMap => shownOnMap;

    /// <summary>
    /// If you gonna override at least call Register() or base.Awake();
    /// </summary>
    protected virtual void Awake()
    {
        Register();
    }

    protected void Register()
    {
        manager.RegisterObjective(this);
    }

    protected void Completed()
    {
        if(IsComplete) return;
        manager.Completed(this);
        IsComplete = true;
        enabled = false;
    }
}
