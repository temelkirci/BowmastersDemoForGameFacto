using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour
{
    #region Singleton
    private static TrajectoryManager instance;
    public static TrajectoryManager Instance
    {
        get { return instance; }
    }
    #endregion

    //used for checking the aim state
    private bool showTrajectory;
    public GameObject trajectoryHolder;

    public GameObject point;
    GameObject[] points;
    public int numberOfPoints;
    public float gapBetweenPoints;
    bool aiming;
    int power;

    Vector3 startPos;
    Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        aiming = false;
    }

    public Vector3 GetDirection()
    {
        return dir;
    }

    public void SetStartPosition(Vector3 start)
    {
        startPos = start;
    }


    public int GetPower()
    {
        return power;
    }

    public void SetAiming(bool aim)
    {
        aiming = aim;
    }
    public bool GetAiming()
    {
        return aiming;
    }
    public void ShowTrajectory(bool show)
    {
        showTrajectory = show;

        if (showTrajectory == true)
        {
            CreateTrajectory();
        }
        else
        {
            RemoveTrajectory();
        }
    }

    public void CreateTrajectory()
    {
        points = new GameObject[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i] = Instantiate(point, GameManager.Instance.GetPlayer().GetPelvis().transform.position, Quaternion.identity);
            points[i].transform.SetParent(trajectoryHolder.transform);
        }
    }
    public void SetTrajectory()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i].transform.position = PointPosition(i * gapBetweenPoints);
        }
    }

    Vector2 PointPosition(float t)
    {
        Vector2 position = (Vector2)GameManager.Instance.GetPlayer().GetPelvis().transform.position + (GameManager.Instance.GetPlayer().GetPelvisDirection().normalized * GameManager.Instance.GetPlayer().GetLaunchForce() * t) + 0.5f * Physics2D.gravity * (t * t);
        return position;
    }

    public void RemoveTrajectory()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            Destroy(points[i].gameObject);
        }
    }

    //This returns the angle in radians
    public static float AngleInRad(Vector3 vec1, Vector3 vec2)
    {
        return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
    }

    //This returns the angle in degrees
    public static float AngleInDeg(Vector3 vec1, Vector3 vec2)
    {
        return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
    }

    public void CalculatePowerAndAngle()
    {
        Vector3 endPos = Input.mousePosition;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(endPos);
        mousePos.z = 0;
        Vector3 start = Camera.main.ScreenToWorldPoint(startPos);
        start.z = 0;      

        if (start.x > mousePos.x)
        {
            dir = endPos - mousePos;

            float distance = Vector3.Distance(mousePos, start) * 10;

            power = (int)distance;

            //clamping power to 100 (I found this after testing)
            if (distance > 100)
            {
                power = 100;
            }
        }
    }
}
