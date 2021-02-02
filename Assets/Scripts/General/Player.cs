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
    int playerMaxHealth;
    int playerHealth;

    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        playerMaxHealth = 100;
        playerHealth = 100;
        launchForce = 1000;
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
        CalculatePelvisDirection();
        SetLaunchForce(Random.Range(50, 75));
        Invoke("LaunchWeapon", 3f);
    }

    public void LaunchWeapon()
    {
        AudioManager.Instance.PlayThrowSound();
        GetWeaponGO().GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        if(CharType == PlayerType.Enemy)
            GetWeaponGO().GetComponent<Rigidbody2D>().AddForce(-pelvis.transform.right * launchForce );
        else
            GetWeaponGO().GetComponent<Rigidbody2D>().AddForce(pelvis.transform.right * launchForce);

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
    public void SetLaunchForce(int force)
    {
        launchForce = force * 15;
    }

    public float GetPelvisAngle()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(pelvis.transform.position);
        Vector3 dir = Input.mousePosition - pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }

    public GameObject GetPelvis()
    {
        return pelvis;
    }

    public Vector2 GetPelvisDirection()
    {
        return pelvisDirection;
    }

    public void CalculatePelvisDirection()
    {
        float angle;

        if (CharType == PlayerType.Enemy)
        {
            angle = Random.Range(-15, -45);
        }
        else
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(pelvis.transform.position);
            pelvisDirection = Input.mousePosition - pos;
            angle = Mathf.Atan2(pelvisDirection.y, pelvisDirection.x) * Mathf.Rad2Deg;

            if (angle > 60 || angle < 0)
                angle = 60;
            if (angle < 0)
                angle = 0;       
        }

        pelvis.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
