using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationIngameEditor : MonoBehaviour
{
    Camera mainCamera;

    public GameObject boxLocationPrefab;
    public GameObject exitPrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            SwapObject();
    }

    private void SwapObject()
    {
        Debug.Log("Mouse Up");
        
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        float distance = cameraRay.origin.y;

        Vector2 intersectionPoint = new Vector2(cameraRay.origin.x + cameraRay.direction.x * distance, cameraRay.origin.z + cameraRay.direction.z * distance);

        intersectionPoint = new Vector2(Mathf.Round(intersectionPoint.x), Mathf.Round(intersectionPoint.y));

        Debug.Log(intersectionPoint);

        Debug.DrawLine(cameraRay.origin, cameraRay.origin + cameraRay.direction * distance);

        NavigationArea.ObjectType lastType = NavigationArea.singleton.navCollection[intersectionPoint];
        if(lastType != NavigationArea.ObjectType.Hole)
        {
            NavigableObject navObject = NavigationArea.singleton.navCollectionObjects[intersectionPoint];

            if(navObject)
            {
                if (navObject.box)
                    Destroy(navObject.box.gameObject);

                Destroy(NavigationArea.singleton.navCollectionObjects[intersectionPoint].gameObject);
            }

        }


        switch(lastType)
        {
            case/*Last: */ (NavigationArea.ObjectType.BoxLocation): //Next: Exit
                Instantiate(exitPrefab, Navigator.UnflattenPos(intersectionPoint), Quaternion.identity);
                break;
            case/*Last: */  (NavigationArea.ObjectType.Exit):       //Next: Floor
                Instantiate(floorPrefab, Navigator.UnflattenPos(intersectionPoint), Quaternion.identity);
                break;
            case/*Last: */  (NavigationArea.ObjectType.Floor):      //Next: Wall
                Instantiate(wallPrefab, Navigator.UnflattenPos(intersectionPoint), Quaternion.identity);
                break;
            case/*Last: */  (NavigationArea.ObjectType.Wall):       //Next: Hole
                NavigationArea.singleton.navCollection[Mathf.RoundToInt(intersectionPoint.x) - NavigationArea.singleton.xOffset, Mathf.RoundToInt(intersectionPoint.y) - NavigationArea.singleton.yOffset] = NavigationArea.ObjectType.Hole;
                break;
            case/*Last: */  (NavigationArea.ObjectType.Hole):       //Next: BoxLocation
                Instantiate(boxLocationPrefab, Navigator.UnflattenPos(intersectionPoint), Quaternion.identity);
                break;
        }

    }
}
