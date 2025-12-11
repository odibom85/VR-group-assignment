using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    [Header("Crosshair Objects")]
    [Tooltip("Parent object for everything crosshair related")]
    public GameObject crosshairRoot;

    [Tooltip("Object for the default crosshair (white cross)")]
    public GameObject crosshairDefault;

    [Tooltip("Object for the hand icon crosshair")]
    public GameObject crosshairHand;

    /// <summary>
    /// Show the normal white cross.
    /// </summary>
    public void ShowDefault()
    {
        if (crosshairRoot != null) crosshairRoot.SetActive(true);
        if (crosshairDefault != null) crosshairDefault.SetActive(true);
        if (crosshairHand != null) crosshairHand.SetActive(false);
    }

    /// <summary>
    /// Show the hand icon when hovering an interactable.
    /// </summary>
    public void ShowHand()
    {
        if (crosshairRoot != null) crosshairRoot.SetActive(true);
        if (crosshairDefault != null) crosshairDefault.SetActive(false);
        if (crosshairHand != null) crosshairHand.SetActive(true);
    }

    /// <summary>
    /// Hide all crosshair (used when holding an object).
    /// </summary>
    public void HideAll()
    {
        if (crosshairRoot != null) crosshairRoot.SetActive(false);
    }
}
