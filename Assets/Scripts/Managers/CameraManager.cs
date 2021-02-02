using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public Vector3 offset;
    Vector3 targetPos;
    GameObject followGO;

    private static CameraManager instance;
    public static CameraManager Instance
    {
        get { return instance; }
    }

    private void Start()
    {
        instance = this;

        targetPos = transform.position;
    }

    public void SetFollow(GameObject follow)
    {
        followGO = follow;
    }

    public void Update()
    {
        if (followGO != null)
            FocusOnPlayer();
    }
    public void FocusOnPlayer()
    {
        if (GameManager.Instance.GetPlayerGO() != null && GameManager.Instance.GetEnemyGO() != null)
        {
            Vector3 posNoZ = transform.position;
            posNoZ.z = followGO.transform.position.z;

            Vector3 targetDirection = (followGO.transform.position - posNoZ);

            interpVelocity = targetDirection.magnitude * 10f;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);
        }
    }
}
