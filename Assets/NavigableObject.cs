using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigableObject : MonoBehaviour
{

    public NavigationArea.ObjectType objectType;

    public Box boxPrefab;

    public Box box;

    // Start is called before the first frame update
    void Start()
    {
        NavigationArea.singleton.navCollection[Mathf.RoundToInt(transform.position.x) - NavigationArea.singleton.xOffset, Mathf.RoundToInt(transform.position.z) - NavigationArea.singleton.yOffset] = objectType;
        NavigationArea.singleton.navCollectionObjects[Mathf.RoundToInt(transform.position.x) - NavigationArea.singleton.xOffset, Mathf.RoundToInt(transform.position.z) - NavigationArea.singleton.yOffset] = this;

        if (NavigationArea.singleton.objectsByType[(int)objectType] == null)
            NavigationArea.singleton.objectsByType[(int)objectType] = new List<NavigableObject>();
        NavigationArea.singleton.objectsByType[(int)objectType].Add(this);

        if(objectType == NavigationArea.ObjectType.BoxLocation && boxPrefab)
        {
            box = Instantiate(boxPrefab.gameObject).GetComponent<Box>();

            box.Offset = Vector3.up * 0.2f;
            box.Owner = gameObject;

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        NavigationArea.singleton.objectsByType[(int)objectType].Remove(this);
    }
}
