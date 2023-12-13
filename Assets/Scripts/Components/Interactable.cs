using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum Type
    {
        None,
        Crafting_table
    }

    public Type type = Type.None;
}
