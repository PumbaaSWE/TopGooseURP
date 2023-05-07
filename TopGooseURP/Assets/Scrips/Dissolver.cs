using UnityEngine;

public class Dissolver : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 1.0f)] private float time;
    [SerializeField] private Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
        _renderer.material.SetFloat("_T", time);
    }
}
