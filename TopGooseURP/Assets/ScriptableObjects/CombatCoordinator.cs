using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCombatCoordinator", menuName = "ScriptableObject/CombatCoordinator")]
public class CombatCoordinator : ScriptableObject
{


    public enum Role { Chaser, Flanker, Zoomer, Idler }

    public int maxChasers = 1;
    public int maxFlankers = 1;
    public int maxZoomers = 1;

    private readonly RoleQueue<AIActor>[] roleQueues =
    {
        new RoleQueue<AIActor>(10),
        new RoleQueue<AIActor>(10),
        new RoleQueue<AIActor>(10),
    };
    private readonly int[] roleCount = new int[3];
    private readonly int[] maxCount = new int[3];

    public void Awake()
    {
        maxCount[0] = maxChasers;
        maxCount[1] = maxFlankers;
        maxCount[2] = maxZoomers;
    }

    public void OnValidate()
    {
        maxCount[0] = maxChasers;
        maxCount[1] = maxFlankers;
        maxCount[2] = maxZoomers;
    }

    public bool RequestRole(Role role, AIActor actor)
    {
        if (role == Role.Idler) return true; //everybody can be an idler
        int r = (int)role;
        if (roleCount[r] < maxCount[r])
        {
            roleCount[r]++;
            return true;
        }
        roleQueues[r].TryEnqueue(actor); //no space right now, queue incase it becoms availible
        return false;
    }

    public void ReturnRole(Role role)
    {
        if (role == Role.Idler) return;
        roleCount[(int)role]--; //one less
        while (roleQueues[(int)role].TryDequeue(out AIActor actor)) // dequuee until an actor accept the role or queue is empty
        {
            if (actor.OfferRole(role))
            {
                roleCount[(int)role]++; //one more again
                break;
            }
        }
    }
}

public class RoleQueue<T>
{
    private readonly T[] values;
    private int head = 0;
    private int tail = 0;
    public int Count { get; private set; }

    public RoleQueue(int max){
        values = new T[max];
    }

    public bool TryEnqueue(T value)
    {
        if(Count < values.Length)
        {
            values[head] = value;
            Increment();
            return true;
        }
        return false;
    }
    public T Dequeue()
    {
        if (Count == 0) throw new IndexOutOfRangeException("RoleQueue - Dequeue - nothing to dequeue!");
        T value = values[tail];
        Decrease();
        return value;
    }

    public bool TryDequeue(out T result)
    {
        if (Count == 0)
        {
            result = default;
            return false;
        }
        result = values[tail];
        Decrease();

        return true;
    }

    private void Increment()
    {
        head++;
        if(head == values.Length)
        {
            head = 0; 
        }
        Count++;
    }

    private void Decrease()
    {
        tail++;
        if (tail == values.Length)
        {
            tail = 0;
        }
        Count--;
    }
}
