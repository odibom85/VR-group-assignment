using UnityEngine;

public interface IUsableItem
{
    void Use(); // Called when player uses the item (left click while holding)
    string GetItemName();
}