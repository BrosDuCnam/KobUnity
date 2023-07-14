using Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Interfaces
{
    /// <summary>
    /// Interface for moving objects.
    /// That's to move physics on this object.
    /// </summary>
    public interface IMovingObject
    {
        public UnityEvent<Vector3> OnMove { get; } // Need to be called when object is moving with Vector3 delta.
    }
}