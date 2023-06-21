using UnityEngine;

public abstract class IItemHandler : MonoBehaviour
{
    // should not check if iReceiver can receive
    public abstract bool CanGive(IItemHandler iReceiver);
    public abstract bool CanBeOverriden();
    public abstract bool CanReceive(IItemHandler iGiver, Item iItem);
    public abstract bool Receive(IItemHandler iGiver, Item iItem);
}
