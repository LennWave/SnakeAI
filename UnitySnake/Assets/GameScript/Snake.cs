using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Snake : MonoBehaviour
{

    public List<GameObject> Body = new List<GameObject>();

    public Vector3 dir;

    public bool isDead = false;


    public void Initialize(Vector2Int location, Vector2Int direction)
    {
        dir = new Vector3(direction.x, direction.y, 0);
        this.transform.localPosition = Vector3.zero;

        var startpos = new Vector3(location.x, location.y, 0);

        for (int i = 0; i < Body.Count; i++)
        {
            Body[i].transform.localPosition = startpos + (-dir * i);

            if (i == 0)
                Body[i].GetComponent<Renderer>().material.color = Color.green;
        }
    }

    public bool eatsCandy(Vector3 location)
    {
        if (Body[0].transform.localPosition == location)
            return true;
        return false;
    }

    public bool IsDead(Vector2Int worldSize)
    {
        //out of borders
        if (Body[0].transform.localPosition.x >= worldSize.x ||
            Body[0].transform.localPosition.x < 0)
        {
            return true;
        }
        if (Body[0].transform.localPosition.y >= worldSize.y||
            Body[0].transform.localPosition.y < 0)
        {
            return true;
        }

        //eatsyourself
        if (Body.Any(x => x.transform.localPosition == Body[0].transform.localPosition && x != Body[0]))
        {
            return true;
        }

        return false;
    }

    public IEnumerator Dead()
    {
        isDead = true;
        for (int i = 0; i < Body.Count; i++)
        {
            Body[i].GetComponent<Renderer>().enabled = false;
            Body[i].GetComponentInChildren<ParticleSystem>(true).gameObject.SetActive(true);
            yield return new WaitForSeconds(.2f);
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
