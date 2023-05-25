using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Health))]
public class AnimatorHandler : MonoBehaviour
{

    private Animator animator;

    [SerializeField] private string deathTrigger = "OnDeath";

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        GetComponent<Health>().OnDead += () => animator.SetTrigger(deathTrigger);

        if (TryGetComponent(out RagdollHandler ragdollHandler))
        {
            ragdollHandler.onRagdollEnable += () => animator.enabled = false;
        }
    }
}
