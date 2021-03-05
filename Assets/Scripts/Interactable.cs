using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactable : MonoBehaviour
{
    CircleCollider2D col;
    public float radius = 3f; //max interactable distance from player 
    public Transform interactionTransform;

    bool isFocus = false; //Is this interactable currently being focused? (closest to the player?)
    public Transform player;

    bool hasInteracted = false;

    void OnDrawGizmosSelected()
    {
        if(interactionTransform == null)
        {
            interactionTransform = transform;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        col = GetComponent<CircleCollider2D>();
        col.radius = radius;
    }

    void Update()
    {
        if (isFocus && !hasInteracted)
        {
            float distance = Vector2.Distance(player.position, transform.position);
            if(distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

    public virtual void Interact() { }

    public void OnFocused (Transform playerTransform)
    {
        isFocus = true;
        player = playerTransform;
        hasInteracted = false;
        //print(name + " focused.");
    }

    public void OnDefocused ()
    {
        isFocus = false;
        player = null;
        hasInteracted = false;
        //print(name + " defocused.");
    }
}
