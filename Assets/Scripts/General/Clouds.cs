using UnityEngine;

public class Clouds : MonoBehaviour
{
    //-----------------------------------------------------------------

    #region Variables
    public Transform cloudStartPos;
    private float speed;
    private float tempPos;
    #endregion

    //-----------------------------------------------------------------

    #region Private Methods
    //setting up a random speed
    void Start()
    {
        speed = Random.Range(1f, 3f);
    }

    //applying the speed to a right movement
    void Update()
    {
        tempPos = (Time.deltaTime * speed) + transform.position.x;
        transform.position = new Vector3(tempPos, transform.position.y, transform.position.z);
    }

    //when collide with border I move ti back to the other end
    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.position = new Vector3(cloudStartPos.transform.position.x, transform.position.y, transform.position.z);
    }
    #endregion

    //-----------------------------------------------------------------
}
