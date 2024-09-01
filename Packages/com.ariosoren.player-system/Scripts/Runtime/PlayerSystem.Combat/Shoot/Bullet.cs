using UnityEngine;

public class Bullet : MonoBehaviour
{
    // todo this script must change to shoot package 
    public void OnDrop()
    {
        transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
    }
}