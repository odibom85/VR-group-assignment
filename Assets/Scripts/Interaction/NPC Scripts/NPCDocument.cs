using UnityEngine;

public class NPCDocument : MonoBehaviour, IStampable
{
    public enum DocumentStatus { Pending, Approved, Rejected }
    public DocumentStatus status = DocumentStatus.Pending;

    public void OnStampApprove()
    {
        status = DocumentStatus.Approved;
        Debug.Log("Document APPROVED!");
        // Add visual feedback, update NPC behavior, etc.
    }

    public void OnStampReject()
    {
        status = DocumentStatus.Rejected;
        Debug.Log("Document REJECTED!");
        // Add visual feedback, update NPC behavior, etc.
    }
}