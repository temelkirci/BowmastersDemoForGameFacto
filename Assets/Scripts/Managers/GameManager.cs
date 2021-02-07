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


    private GameState currentGameState;
    public event OnStateChangeHandler OnStateChange;

    public GameObject[] playerList;
    public GameObject[] enemyList;

    public GameObject playerHolder;
    public GameObject enemyHolder;

    GameObject selectedPlayerGO;
    GameObject selectedEnemyGO;

    Player selectedPlayer;
    Player selectedEnemy;


    public void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        instance = this;

        SetState(GameState.MainMenu);

        UIManager.Instance.ShowMenu();
    }

    public void StartRound()
    {
        SetState(GameState.PlayerAim);

        SelectCharacter();
        SelectRandomEnemy();

        selectedPlayer.CreateWeapon();
        TrajectoryManager.Instance.ShowTrajectory(true);

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
            TrajectoryManager.Instance.CreateTrajectory();

            TrajectoryManager.Instance.ShowTrajectory(true);
            selectedPlayer.CreateWeapon();
            selectedEnemy.GetWeapon().SetEnemyWeapon(false);
            CameraManager.Instance.SetFollow(selectedPlayerGO);

        }
        else if (currentGameState == GameState.PlayerShoot)
        {
            AudioManager.Instance.PlayHitSound();

            TrajectoryManager.Instance.ShowTrajectory(false);
            SetState(GameState.EnemyAim);
            selectedEnemy.CreateWeapon();
            selectedEnemy.GetWeapon().SetEnemyWeapon(true);
            CameraManager.Instance.SetFollow(selectedEnemyGO);

            Invoke("AttackEnemy", 1f);
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

            if(TrajectoryManager.Instance.GetAiming() == true)
            {
                UIManager.Instance.ShowPowerAndAngle(true);
                TrajectoryManager.Instance.SetTrajectory(); //  redraw trajectory
                TrajectoryManager.Instance.CalculatePowerAndAngle();

                selectedPlayer.CalculatePelvisDirection(TrajectoryManager.Instance.GetDirection());

                UIManager.Instance.SetPlayerAngle((int)selectedPlayer.GetPelvisAngle());
                UIManager.Instance.SetPlayerPower(TrajectoryManager.Instance.GetPower());

                if (Input.GetMouseButtonUp(0)) // shoot
                {
                    CameraManager.Instance.SetFollow(selectedPlayer.GetWeaponGO());
                    selectedPlayer.SetLaunchForce(TrajectoryManager.Instance.GetPower());
                    selectedPlayer.LaunchWeapon();
                    UIManager.Instance.ShowPowerAndAngle(false);
                    TrajectoryManager.Instance.SetAiming(false);
                    TrajectoryManager.Instance.ShowTrajectory(false);
                }
            }
            else
            {
                // I need to be the first touch
                if (Input.GetMouseButtonDown(0))
                {
                    //saves the start point of the touch
                    TrajectoryManager.Instance.SetStartPosition(Input.mousePosition);
                    //setting the aim true so that calculations could start
                    TrajectoryManager.Instance.SetAiming(true);
                }
            }
        }      
        
    }

}
