using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    GameObject lastOwner;
    GameObject owner;

    float transition = 1;

    Vector3 lastOffset = Vector3.zero;
    Vector3 offset = Vector3.zero;

    public Vector3 Offset
    {
        get => offset;
        set
        {
            lastOffset = offset;
            offset = value;
        }
    }

    public GameObject Owner
    {
        get => owner;
        set
        {
            lastOwner = owner;
            owner = value;

            if(lastOwner != null)
                transition = 0;
            else
                transform.position = owner.transform.position + offset;
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transition < 1)
        {
            transition += Time.deltaTime;
            transition = Mathf.Clamp01(transition);

            transform.position = Vector3.Lerp(lastOwner.transform.position + lastOffset, owner.transform.position + offset, SmoothTransition(transition));
        }
        else
            transform.position = owner.transform.position + offset;

    }

    float SmoothTransition(float x)
    {
        return -((Mathf.Cos(x * Mathf.PI) - 1) / 2);
    }
}
