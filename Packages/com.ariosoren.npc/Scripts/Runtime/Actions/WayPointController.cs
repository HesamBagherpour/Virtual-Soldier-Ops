using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointController : MonoBehaviour
{

    public List<Transform> wayPoints = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform t in wayPoints)
            t.GetComponent<MeshRenderer>().enabled = false;

    }

    public Vector3 GetRandomPoint()
    {
        return wayPoints[Random.Range(0, wayPoints.Count)].position;
    }

}
