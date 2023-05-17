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
    [SerializeField] protected bool shownOnMap;
    [SerializeField] protected int count = 1;
    public bool IsComplete { get; private set; }


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

    protected void Register()
    {
        manager.RegisterObjective(this);
    }

    protected void Completed()
    {
        if(IsComplete) return;
        Debug.Assert(this != null, "Objective -> HUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUR är this null?????");
        manager.Completed(this);
        IsComplete = true;
        enabled = false;
    }

    public void CountOne()
    {
        Count(1);
    }

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
