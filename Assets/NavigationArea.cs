using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class NavigationArea : MonoBehaviour
{
    static NavigationArea _singleton;
    public static NavigationArea singleton
    {
        get 
        { 
            if(_singleton)
                return _singleton;

            GameObject singletonGO = new();
            singletonGO.name = "Navigation Area";

            _singleton = singletonGO.AddComponent<NavigationArea>();

            return _singleton;
        }
    }

    public enum ObjectType
    {
        BoxLocation = 1,
        Exit = 2,
        Floor = 3,
        Wall = 4,
        Hole = 5
    }

    public class ObjectNavCollection : NavCollection<ObjectType> 
    {
        public ObjectNavCollection(int width, int height, ObjectType defaultValue) => Constructor(width, height, defaultValue);

        public override int distanceValue(Vector2 position)
        {
            switch(this[position])
            {
                case (ObjectType.BoxLocation): return 1; break;
                case (ObjectType.Exit): return 1; break;
                case (ObjectType.Floor): return 1; break;
                default: return int.MaxValue;
            }
        }

        public override int distanceValue(int x, int y) => this[x, y] == ObjectType.Floor ? 1 : int.MaxValue;

    }
    public ObjectNavCollection navCollection;

    public NavCollection<NavigableObject> navCollectionObjects;

    public List<List<NavigableObject>> objectsByType;

    public int xOffset = 0;
    public int yOffset = 0;

    public int defaultWidth;
    public int defaultHeight;

    // Start is called before the first frame update
    void Awake()
    {
        if (_singleton == null)
            _singleton = this;
        else
            Destroy(this.gameObject);

        navCollection = new ObjectNavCollection(defaultWidth, defaultHeight, ObjectType.Hole);
        navCollectionObjects = new NavCollection<NavigableObject>(defaultWidth, defaultHeight, null);
        objectsByType = new List<List<NavigableObject>>();
        for(int i = 0; i <= (int)ObjectType.Hole; i++)
        {
            objectsByType.Add(new List<NavigableObject>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Vector3 topLeft = new Vector3(xOffset, 0, yOffset);
        Vector3 topRight = new Vector3(defaultWidth - xOffset, 0, yOffset);

        Vector3 botLeft = new Vector3(xOffset, 0, defaultHeight - yOffset);
        Vector3 botRight = new Vector3(defaultWidth - xOffset, 0, defaultHeight - yOffset);

        //Vector3 center = topLeft + botRight / 2;

        Gizmos.color = new Color(1,1,1,0.2f);

        for(int x = 0; x < defaultWidth; x++)
        {
            for (int y = 0; y < defaultHeight; y++)
            {
                Vector3 center = topLeft + new Vector3(x,0, y);
                Gizmos.DrawWireCube(center, new Vector3(1, 0, 1));
            }
        }


    }
}

