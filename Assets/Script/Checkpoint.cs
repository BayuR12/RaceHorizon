using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public CheckpointManager manager;

    private bool reached = false;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        rend.material.color = Color.gray;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !reached)
        {
            reached = true;

            manager.ReachCheckpoint(transform);

            rend.material.color = Color.green;
        }
    }
}