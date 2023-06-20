using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemHandler
{
    public abstract bool CanGive();
    public abstract bool CanReceive(IItemHandler iGiver, Item iItem);
    public abstract bool Receive(IItemHandler iGiver, Item iItem);
}
