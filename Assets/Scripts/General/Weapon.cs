using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    bool isRotate;

    bool isEnemyWeapon;

    Rigidbody2D weaponRigidBody2D;

    float weaponAngle;
    Vector2 weaponDirection;
    public float RotationSpeed;
    int damage;

    // Start is called before the first frame update
    void Start()
    {
        weaponRigidBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetState() == GameState.EnemyShoot || GameManager.Instance.GetState() == GameState.PlayerShoot)
        {
            if (isRotate == true)
                StartRotate();
            else
                TrackMovement();
        }
    }

    public void SetEnemyWeapon(bool enemyWeapon)
    {
        isEnemyWeapon = enemyWeapon;
    }

    public void ThrowWeapon()
    {
        Invoke("EnableBoxCollider", 0.5f);
        Invoke("DestroyWeapon", 5f);
    }

    void StartRotate()
    {
        transform.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime);
    }

    void EnableBoxCollider()
    {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    void TrackMovement()
    {
        weaponDirection = weaponRigidBody2D.velocity;

        weaponAngle = Mathf.Atan2(weaponDirection.y, weaponDirection.x) * Mathf.Deg2Rad;
        transform.rotation = Quaternion.AngleAxis(weaponAngle, Vector3.forward);
    }

    public float GetWeaponAngle()
    {
        return transform.transform.rotation.eulerAngles.z;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        /*
        if(collision.gameObject.name == "Head")
        {
            DamageTarget(50);
        }
        if (collision.gameObject.name == "Body")
        {
            DamageTarget(30);
        }
        if (collision.gameObject.name == "Hand R" || collision.gameObject.name == "Hand L")
        {
            DamageTarget(20);
        }
        if (collision.gameObject.name == "Foot R" || collision.gameObject.name == "Foot L")
        {
            DamageTarget(10);
        }
        */
        if (collision.gameObject.CompareTag("Player") == true || collision.gameObject.CompareTag("Enemy") == true)
        {
            DamageTarget(25);
        }
        if (collision.gameObject.CompareTag("Ground") == true)
        {
            DestroyWeapon();
        }
    }

    void DestroyWeapon()
    {
        if (isEnemyWeapon == true)
        {
            GameManager.Instance.GetPlayer().TakeDamage(damage);
        }
        else
        {
            GameManager.Instance.GetEnemy().TakeDamage(damage);
        }

        Destroy(this.gameObject);
    }

    void DamageTarget(int damage)
    {
        weaponRigidBody2D.velocity = Vector2.zero;
        weaponRigidBody2D.isKinematic = true;

        this.damage = damage;

        DestroyWeapon();
    }

}
