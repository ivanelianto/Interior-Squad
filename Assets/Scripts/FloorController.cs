using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    public GameObject floor;
    
    private List<GameObject> floors = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        // Instantiate Map
        // Min : 13
        // Max : 30

        for (int r = 0; r < 10; r++)
        {
            for (int c = 0; c < 10; c++)
            {
                //Instantiate(floor, new Vector3(r * 1, 0, c * 1), Quaternion.identity);
                GameObject newFloor = Instantiate(floor, new Vector3(r * 1, 0, c * 1), Quaternion.identity);
                floors.Add(newFloor);
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Plane")))
        {
            Debug.Log("hello");
        }

        Debug.DrawLine(ray.origin, ray.direction);
    }
}
