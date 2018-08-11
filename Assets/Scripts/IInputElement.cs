using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputElement
{
    Collider MainCollider
    {
        get;
    }

    void ProcessMouseOver();
    void ProcessMouseLost();
    void ProcessClick(int mouseIndex);
    void ProcessDrag();
}
