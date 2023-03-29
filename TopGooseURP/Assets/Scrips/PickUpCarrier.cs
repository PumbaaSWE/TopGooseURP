using UnityEngine;

public class PickUpCarrier : MonoBehaviour
{
    private bool carring = false;
    private PickUp pickUp;

    public delegate void OnPickUpEvent(GameObject pickUp);
    public OnPickUpEvent OnPickUp;

    // Remove(or change) lator when new input system is online 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Drop(GetComponentInParent<Rigidbody>().velocity);
        }
    }

    public void Drop(Vector3 velocity)
    {
        if (carring)
        {
            pickUp.Drop(velocity);
            carring = false;
        }
    }

    public void Interact()
    {
        if (carring)
        {
            pickUp.OnInteract?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (carring) return;
        if (other.TryGetComponent(out PickUp pu) && pu.CanBePickedUp)
        {
            pu.PickedUp(transform);
            pickUp = pu;
            carring = true;
            OnPickUp?.Invoke(other.gameObject);
        }
    }
}
