using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{

    NavCollection<AStarSearchData> pathing;

    Vector2? goal;
    NavigableObject goalObject;

    bool isInitialized = false;

    Vector3 smoothGoal;

    Box box;

    bool hasPath = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);



        isInitialized = true;
        /*
        Vector2 inPos = start;
        while(inPos != pathing[inPos].position)
        {
            Debug.Log("Next: "+ pathing[inPos].position);
            inPos = pathing[inPos].position;
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized)
            return;

        if(goal.HasValue)
        {
            if(hasPath)
                GoToGoal();
            if (Vector2.Distance(GetFlatPos(transform.position), goal.Value) <  0.1f)
            {
                if (box != null)
                    PlaceBox();
                else
                    GrabBox();

                goal = null;
                pathing = new NavCollection<AStarSearchData>();
            }
        }
        else
        {
            if (box != null)
                goal = FindNearest(NavigationArea.ObjectType.Exit);
            else
                goal = FindNearest(NavigationArea.ObjectType.BoxLocation);

            PathGoal();
        }
    }

    Vector2 FindNearest(NavigationArea.ObjectType objectType)
    {
        NavigableObject closestObject = null;
        float shortestDistance = Mathf.Infinity;
        foreach(NavigableObject obj in NavigationArea.singleton.objectsByType[(int)objectType])
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            bool boxRequirement = objectType != NavigationArea.ObjectType.BoxLocation || obj.box != null;

            if (boxRequirement && distance < shortestDistance)
            {
                shortestDistance = distance;
                closestObject = obj;
            }
        }

        goalObject = closestObject;

        if (closestObject == null)
            return GetFlatPosRounded(transform.position);
        else
            return GetFlatPosRounded(closestObject.transform.position);
    }

    public static Vector2 GetFlatPosRounded(Vector3 position) => new Vector2(Mathf.Round(position.x), Mathf.Round(position.z));

    public static Vector2 GetFlatPos(Vector3 position) => new Vector2(position.x, position.z);

    public static Vector3 UnflattenPos(Vector2 flatPos, float height = 0) => new Vector3(flatPos.x, height, flatPos.y);

    void GrabBox()
    {
        if (goalObject == null)
            return;

        box = goalObject.box;
        goalObject.box = null;


        if (box == null)
            return;

        box.Offset = Vector3.up;
        box.Owner = gameObject;

    }

    void PlaceBox()
    {
        if (goalObject == null || box == null)
            return;

        box.Offset = Vector3.up * 0.2f;
        box.Owner = goalObject.gameObject;
        box = null;
    }

    void PathGoal()
    {
        //Debug.Log(goal.Value);
        //Debug.Log(GetFlatPos(transform.position));

        AStar(NavigationArea.singleton.navCollection, goal.Value, GetFlatPosRounded(transform.position), out pathing);

        //Debug.Log("Draw path");
        StartCoroutine(SlowDrawPath());
        

        smoothGoal = transform.position;
    }

    IEnumerator SlowDrawPath()
    {
        Vector2 inPos = GetFlatPos(transform.position);
        while (inPos != pathing[inPos].position)
        {
            Debug.DrawLine(UnflattenPos(inPos, 1), UnflattenPos(pathing[inPos].position, 1), Color.red, 2);
            inPos = pathing[inPos].position;

            yield return new WaitForSeconds(0.1f);
        }
    }

    void GoToGoal()
    {
        Vector3 directGoal = pathing[Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)].position;
        directGoal = new Vector3(directGoal.x, 0, directGoal.y);

        //Debug.Log($"{Mathf.RoundToInt(transform.position.x)},{ Mathf.RoundToInt(transform.position.z)}");

        smoothGoal = Vector3.Lerp(smoothGoal, directGoal, Time.deltaTime * 4);

        Vector3 direction = (smoothGoal - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.position += direction * Time.deltaTime * 2;
    }




    readonly Vector2[] offsets = { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
    void AStar<T>(NavCollection<T> navArea, Vector2 start, Vector2 goal, out NavCollection<AStarSearchData> pathing)
    {
        hasPath = false;

        AStarSearchDataComparer dataComparer = new AStarSearchDataComparer();

        pathing = new NavCollection<AStarSearchData>();

        AStarSearchData defaultPoint;
        defaultPoint.position = start;
        defaultPoint.distance = int.MaxValue;
        pathing.defaultValue = defaultPoint;

        AStarSearchData startPoint = pathing[start];
        startPoint.position = start;
        startPoint.distance = 0;
        pathing[start] = startPoint;

        List<AStarSearchData> currentFrontline = new List<AStarSearchData>();
        currentFrontline.Add(startPoint);

        while (currentFrontline.Count > 0)
        {
            Vector2 currentPos = currentFrontline[0].position;
            currentFrontline.RemoveAt(0);

            if (currentPos == goal)
                break;

            for (int offsetIndex = 0; offsetIndex < 4; offsetIndex++)
            {
                Vector2 nextPos = currentPos + offsets[offsetIndex];
                int newDistance;
                if (navArea.distanceValue(nextPos) == int.MaxValue)
                    newDistance = int.MaxValue;
                else
                    newDistance = pathing[currentPos].distance.Value + navArea.distanceValue(nextPos);

                if (pathing[nextPos].distance.HasValue == false || newDistance < pathing[nextPos].distance.Value)
                {
                    AStarSearchData nextPoint = pathing[nextPos];
                    nextPoint.distance = newDistance;
                    nextPoint.position = currentPos;
                    //Debug.Log(nextPos);
                    //Debug.Log(newDistance);
                    //Debug.Log(pathing[nextPos].distance.Value);
                    pathing[nextPos] = nextPoint;

                    AStarSearchData newFrontline;
                    newFrontline.distance = (int)Mathf.RoundToInt(newDistance + Vector2.Distance(nextPos, goal));
                    newFrontline.position = nextPos;
                    currentFrontline.Add(newFrontline);

                }
            }

            currentFrontline.Sort(dataComparer);

        }
        //Debug.Log("finish A Star");
        hasPath = true;
    }
}

public struct AStarSearchData
{
    public Vector2 position;
    public int? distance;
}

public class AStarSearchDataComparer : IComparer<AStarSearchData>
{
    public int Compare(AStarSearchData a, AStarSearchData b)
    {
        if (a.distance == b.distance)
            return 0;
        if (a.distance < b.distance)
            return -1;

        return 1;
    }
}