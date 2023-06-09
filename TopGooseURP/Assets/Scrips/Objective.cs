using UnityEngine;


public class Objective : MonoBehaviour
{

    [SerializeField] private ObjectiveEventManager manager;
    [Space]
    [SerializeField] protected int score = 100;
    [SerializeField] protected string title = "Objective";
    [SerializeField] protected string description = "Objective Description";
    //[SerializeField] protected Texture2D icon;
    [SerializeField] protected Sprite sprite;
    [SerializeField] private bool isPrimary;
    [Tooltip("Should this objective be shown on the map?")][SerializeField] protected bool shownOnMap;
    [Tooltip("How many times does an action/event occure befor awarding the objective")][SerializeField] protected int count = 1;
    public bool IsComplete { get; private set; }

    public delegate void OnCompleteEvent();
    public event OnCompleteEvent OnComplete;

    public int Score => score;
    public string Title => title;
    public string Description => description;
    //public Texture2D Icon => icon;
    public Sprite Sprite => sprite;
    public bool IsPrimary => isPrimary;
    public bool ShownOnMap => shownOnMap;

    /// <summary>
    /// If you gonna override at least call Register() or base.Awake();
    /// </summary>
    protected virtual void Awake()
    {
        if(manager == null)
        {
            Debug.LogWarning("Objective - ObjectiveEventManager is missing");
            IsComplete = true;
            return;
        }

        //if(icon != null && sprite == null) {
        //    sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.one / 2);
        //}

        Register();
        gameObject.name = Title;
    }

    /// <summary>
    /// Register self with the manager specicified in inspector
    /// </summary>
    protected void Register()
    {
        manager.RegisterObjective(this);
    }

    /// <summary>
    /// Mark this objective as completed
    /// </summary>
    protected void Completed()
    {
        if(IsComplete) return;
        manager.Completed(this);
        IsComplete = true;
        enabled = false;
        OnComplete?.Invoke();
    }

    /// <summary>
    /// Count this objective once
    /// </summary>
    public void CountOne()
    {
        Count(1);
    }

    /// <summary>
    /// Count this objective a set amount of times
    /// </summary>
    /// <param name="value"></param>
    public void Count(int value)
    {
        count -= value;
        if (count <= 0)
        {
            Completed();
            count = 0;
        }
    }
}
