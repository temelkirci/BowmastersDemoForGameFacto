using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //-----------------------------------------------------------------

    #region Variables
    public enum PlayerType
    {
        Player,
        Enemy
    }
    public PlayerType CharType;

    public GameObject weaponGO;
    GameObject weapon;

    public GameObject pelvis;
    public Transform weaponPosition;
    Vector3 pelvisDirection;

    float launchForce;
    int pelvisAngle;

    int playerHealth;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = 100;
    }

    //I needed some time before the shoot sequence begins so I used Invoke
    private void CheckState()
    {
        GameState curState = GameManager.Instance.GetState();

        switch (curState)
        {          
            case GameState.PlayerAim:
                GameManager.Instance.SetState(GameState.PlayerShoot);
                CameraManager.Instance.SetFollow(weapon);

                break;
            case GameState.EnemyAim:
                GameManager.Instance.SetState(GameState.EnemyShoot);
                CameraManager.Instance.SetFollow(weapon);

                break;
        }
    }

    public void WaitAndAttack()
    {
        int pelvisAngle = Random.Range(-15, -45);

        SetPelvisRotation(pelvisAngle);
        SetLaunchForce(Random.Range(50, 100));
        Invoke("LaunchWeapon", 3f);
    }

    public void LaunchWeapon()
    {
        AudioManager.Instance.PlayThrowSound();
        GetWeaponGO().GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        if (CharType == PlayerType.Enemy)
        {
            GetWeaponGO().GetComponent<Rigidbody2D>().AddForce(-pelvis.transform.right * launchForce * 150);
        }
        else
        {
            GetWeaponGO().GetComponent<Rigidbody2D>().AddForce(pelvis.transform.right * launchForce * 150);
        }

        GetWeapon().ThrowWeapon();
        CheckState();
    }

    public void CreateWeapon()
    {
        weapon = Instantiate(weaponGO, weaponPosition.transform.position, weaponPosition.transform.rotation);
        weapon.transform.parent = weaponPosition.transform.transform;

        weapon.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        pelvis.transform.rotation = new Quaternion(0, 0, 0, 0);
    }


    public Weapon GetWeapon()
    {
        if (weapon == null)
            return null;
        return weapon.GetComponent<Weapon>();
    }

    public GameObject GetWeaponGO()
    {
        return weapon;
    }

    public int GetHealth()
    {
        return playerHealth;
    }

    public float GetLaunchForce()
    {
        return launchForce;
    }
    public void SetLaunchForce(float force)
    {
        launchForce = (int)force;
    }

    public GameObject GetPelvis()
    {
        return pelvis;
    }

    public int GetPelvisAngle()
    {
        return pelvisAngle;
    }

    public Vector2 GetPelvisDirection()
    {
        return pelvisDirection;
    }
    public void SetPelvisRotation(float angle)
    {
        pelvisAngle = (int)angle;
        pelvis.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void CalculatePelvisDirection(Vector3 dir)
    {
        if (CharType == PlayerType.Player)
        {
            pelvisDirection = dir;

            float angle = Mathf.Atan2(pelvisDirection.y, pelvisDirection.x) * Mathf.Rad2Deg;

            if (angle > 60)
                angle = 60;
            if (angle < 0)
                angle = 0;

            SetPelvisRotation(angle);
        }
    }

    //used to apply damage to player/enemy
    public void TakeDamage(int damage)
    {
        //decreasing health
        playerHealth -= damage;

        //if health is below zero then depending of the PlayerType player lose or win
        if (playerHealth <= 0f)
        {
            playerHealth = 0;

            if (CharType == PlayerType.Player)
            {
                GameManager.Instance.EnemyWon();
            }
            else if (CharType == PlayerType.Enemy)
            {
                GameManager.Instance.PlayerWon();
            }
        }
        else
        {
            //if health above 0 checking the next state
            GameManager.Instance.CheckStateAfterHit();
        }
    }
}
