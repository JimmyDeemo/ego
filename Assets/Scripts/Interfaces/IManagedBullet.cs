using UnityEngine;

public interface IManagedBullet
{
    void Poll();
    void Disable();
    bool IsActive();
    void SetParent(Transform parent);
    Vector3 Position { get; }
}