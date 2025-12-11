using UnityEngine;

public class StampItem : MonoBehaviour, IUsableItem
{
    public string itemName = "Stamp";
    public bool isApprovalStamp = true; // true = approve, false = reject

    private CombinedInteraction playerInteraction;

    void Start()
    {
        playerInteraction = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<CombinedInteraction>();
    }

    public void Use()
    {
        Debug.Log("Using stamp!");

        // Raycast to see what we're stamping
        RaycastHit? lookTarget = playerInteraction.GetLookTarget();

        if (lookTarget.HasValue)
        {
            RaycastHit hit = lookTarget.Value;

            // Check if hit object can be stamped
            IStampable stampable = hit.collider.GetComponent<IStampable>();
            if (stampable != null)
            {
                if (isApprovalStamp)
                {
                    Debug.Log("APPROVED stamp on " + hit.collider.name);
                    stampable.OnStampApprove();
                }
                else
                {
                    Debug.Log("REJECTED stamp on " + hit.collider.name);
                    stampable.OnStampReject();
                }
            }
            else
            {
                Debug.Log("Can't stamp " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("Not looking at anything to stamp");
        }
    }

    public string GetItemName()
    {
        return itemName;
    }
}