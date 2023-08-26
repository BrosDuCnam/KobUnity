using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    [SerializeField] protected Boat attachedBoat;

    public void AttachBoat(Boat _boat)
    {
        attachedBoat = _boat;
    }
}
