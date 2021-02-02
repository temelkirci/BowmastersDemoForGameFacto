using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public enum GameState { MainMenu, PlayerAim, PlayerShoot, EnemyAim, EnemyShoot, GameOver }
public delegate void OnStateChangeHandler();

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    #endregion

    //used for checking the aim state
    private bool showTrajectory;

    private GameState currentGameState;
    public event OnStateChangeHandler OnStateChange;

    public GameObject[] playerList;
    public GameObject[] enemyList;

    public GameObject playerHolder;
    public GameObject enemyHolder;
    public GameObject trajectoryHolder;

    GameObject selectedPlayerGO;
    GameObject selectedEnemyGO;

    Player selectedPlayer;
    Player selectedEnemy;

    public GameObject point;
    GameObject[] points;
    public int numberOfPoints;
    public float gapBetweenPoints;
    bool aiming;

    Vector3 startPoint;

    public void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        instance = this;

        aiming = false;
        SetState(GameState.MainMenu);

        UIManager.Instance.ShowMenu();
    }

    public void StartRound()
    {
        SetState(GameState.PlayerAim);

        SelectCharacter();
        SelectRandomEnemy();

        selectedPlayer.CreateWeapon();
        ShowTrajectory(true);

        UIManager.Instance.ShowInGame();
        CameraManager.Instance.SetFollow(selectedPlayerGO);
    }

    void SelectCharacter()
    {
        selectedPlayerGO = Instantiate(playerList[Random.Range(0, playerList.Length)], playerHolder.transform.position, playerHolder.transform.rotation);
        selectedPlayerGO.name = "Temel";

        selectedPlayerGO.transform.SetParent(playerHolder.transform);

        selectedPlayer = selectedPlayerGO.GetComponent<Player>();
    }

    void SelectRandomEnemy()
    {
        selectedEnemyGO = Instantiate(enemyList[Random.Range(0, enemyList.Length)], enemyHolder.transform.position, enemyHolder.transform.rotation);
        selectedEnemyGO.name = "Bülent";

        selectedEnemyGO.transform.SetParent(enemyHolder.transform);

        selectedEnemy = selectedEnemyGO.GetComponent<Player>();
    }

    public void SetState(GameState state)
    {
        currentGameState = state;
        if (OnStateChange != null)
        {
            OnStateChange();
        }
    }

    //Getting the current state
    public GameState GetState()
    {
        return currentGameState;
    }

    //used to check what state is next after projectile hits
    public void CheckStateAfterHit()
    {
        if (currentGameState == GameState.EnemyShoot)
        {
            AudioManager.Instance.PlayHitSound();

            SetState(GameState.PlayerAim);
            CreateTrajectory();

            ShowTrajectory(true);
            selectedPlayer.CreateWeapon();
            CameraManager.Instance.SetFollow(selectedPlayerGO);

        }
        else if (currentGameState == GameState.PlayerShoot)
        {
            AudioManager.Instance.PlayHitSound();

            ShowTrajectory(false);
            SetState(GameState.EnemyAim);
            selectedEnemy.CreateWeapon();
            CameraManager.Instance.SetFollow(selectedEnemyGO);
            RemoveTrajectory();

            Invoke("AttackEnemy", 1f);
        }
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

    public GameObject GetPlayerGO()
    {
        return selectedPlayerGO;
    }
    public GameObject GetEnemyGO()
    {
        return selectedEnemyGO;
    }

    public Player GetPlayer()
    {
        return selectedPlayer;
    }
    public Player GetEnemy()
    {
        return selectedEnemy;
    }

    //called when Player wins
    public void PlayerWon()
    {
        CameraManager.Instance.SetFollow(selectedPlayerGO);

        //setting game state
        SetState(GameState.GameOver);

        //Plays win sound
        AudioManager.Instance.PlayWinSound();

        UIManager.Instance.ShowMenu();
    }

    //called when Player loses
    public void EnemyWon()
    {
        CameraManager.Instance.SetFollow(selectedEnemyGO);

        //setting game state
        SetState(GameState.GameOver);

        //Plays lose sound
        AudioManager.Instance.PlayLoseSound();

        UIManager.Instance.ShowMenu();
    }

    public void CreateTrajectory()
    {
        points = new GameObject[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i] = Instantiate(point, selectedPlayer.GetPelvis().transform.position, Quaternion.identity);
            points[i].transform.SetParent(trajectoryHolder.transform);
        }
    }
    
    Vector2 PointPosition(float t)
    {
        Vector2 position = (Vector2)selectedPlayer.GetPelvis().transform.position + (selectedPlayer.GetPelvisDirection().normalized * selectedPlayer.GetLaunchForce() * t) + 0.5f * Physics2D.gravity * (t * t);
        return position;
    }

    public void SetTrajectory()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i].transform.position = PointPosition(i * gapBetweenPoints);
        }
    }
    
    public void RemoveTrajectory()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            Destroy(points[i].gameObject);
        }
    }

    void ShowPlayersStats()
    {
        float distanceBetweenPlayers = Vector2.Distance(selectedEnemy.transform.position, selectedPlayer.transform.position);

        UIManager.Instance.SetDistance(distanceBetweenPlayers.ToString() + " m");
        UIManager.Instance.SetPlayerHealth(selectedPlayer.GetHealth());
        UIManager.Instance.SetEnemyHealth(selectedEnemy.GetHealth());
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentGameState)
        {
            case GameState.MainMenu:
                
                break;

            case GameState.GameOver:
                
                break;

            case GameState.EnemyAim:
                ShowPlayersStats();

                break;

            case GameState.EnemyShoot:
                ShowPlayersStats();

                break;

            case GameState.PlayerAim:
                AttackPlayer();
                ShowPlayersStats();

                break;

            case GameState.PlayerShoot:
                ShowPlayersStats();

                break;

            default:

                break;
        }

    }

    //calculate power
    private void CalculatePowerAndAngle(Vector3 startPos, Vector3 endPos)
    {
        int angle = (int)selectedPlayer.GetPelvisAngle();
        UIManager.Instance.SetPlayerAngle(angle);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(endPos);
        mousePos.z = 0;
        Vector3 start = Camera.main.ScreenToWorldPoint(startPos);
        start.z = 0;

        int distance = (int)Vector3.Distance(mousePos, start);
        int power = distance * 10;

        //clamping power to 100 (I found this after testing)
        if (power > 100)
        {
            power = 100;
        }

        selectedPlayer.SetLaunchForce(power);

        UIManager.Instance.SetPlayerPower(power);
    }

    void AttackEnemy()
    {
        UIManager.Instance.SetTurn(selectedEnemy.name);
        selectedEnemy.WaitAndAttack();
    }

    void AttackPlayer()
    {
        if (currentGameState == GameState.PlayerAim)
        {
            UIManager.Instance.SetTurn(selectedPlayer.name);        

            if(aiming)
            {
                UIManager.Instance.ShowPowerAndAngle(true);
                selectedPlayer.CalculatePelvisDirection();
                SetTrajectory(); //  redraw trajectory
                CalculatePowerAndAngle(startPoint, Input.mousePosition);
            }

            // I need to be the first touch
            if (Input.GetMouseButtonDown(0) && !aiming)
            {
                //saves the start point of the touch
                startPoint = Input.mousePosition;
                //setting the aim true so that calculations could start
                aiming = true;
            }

            if (Input.GetMouseButtonUp(0) && aiming)
            {
                CameraManager.Instance.SetFollow(selectedPlayer.GetWeaponGO());
                selectedPlayer.LaunchWeapon();
                aiming = false;
                UIManager.Instance.ShowPowerAndAngle(false);
                ShowTrajectory(false);
            }
        }      
        
    }

}
