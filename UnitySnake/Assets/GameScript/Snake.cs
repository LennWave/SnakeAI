using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{

    public List<GameObject> Body = new List<GameObject>();

    public Vector3 dir;


    public void Initialize(Vector2Int location, Vector2Int direction)
    {
        dir = new Vector3(direction.x, direction.y, 0);
        this.transform.localPosition = new Vector3(location.x, location.y, 0);


        for (int i = 0; i < Body.Count; i++)
        {
            Body[i].transform.localPosition = (-dir * i);

            if (i == 0)
                Body[i].GetComponent<Renderer>().material.color = Color.green;
        }
    }

    public void Tick()
    {
        for (int i = Body.Count - 1; i >= 0; i--)
        {
            var b = Body[i];
            if (i == 0)
            {

                b.transform.localPosition += dir;
            }
            else
            {
                b.transform.localPosition = Body[i - 1].transform.localPosition;
            }
        }
    }
}
