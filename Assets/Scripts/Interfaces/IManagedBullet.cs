using UnityEngine;

public interface IManagedBullet
{
    void Poll();
    void Disable();
    bool IsAvailable();
    void SetParent(Transform parent);
}