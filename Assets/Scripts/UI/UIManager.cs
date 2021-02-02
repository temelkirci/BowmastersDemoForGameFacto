using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton

    private static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
    }

    #endregion

    public GameObject InGame;
    public GameObject MainMenu;

    public Text turnText;
    public Text distanceText;

    public Text playerHealthText;
    public Text enemyHealthText;

    public GameObject playerStats;

    public GameObject playerPowerAndAngle;
    public Text playerAngle;
    public Text playerPower;
    public Button startButton;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        
    }

    public void ShowMenu()
    {
        Debug.Log("ShowMenu");
        MainMenu.SetActive(true);
        InGame.SetActive(false);

        ShowPlayerStats(false);

        startButton.onClick.AddListener(GameManager.Instance.StartRound);
    }
    public void ShowInGame()
    {
        MainMenu.SetActive(false);
        InGame.SetActive(true);

        ShowPlayerStats(true);
    }

    public void ShowPlayerStats(bool show)
    {
        playerStats.SetActive(show);
    }

    public void ShowPowerAndAngle(bool show)
    {
        playerPowerAndAngle.SetActive(show);
    }
    public void SetPlayerAngle(int angle)
    {
        playerAngle.text = angle.ToString() + " °";
    }
    public void SetPlayerPower(int power)
    {
        playerPower.text = power.ToString() + " %";
    }

    public void SetTurn(string turn)
    {
        turnText.text = turn;
    }
    public void SetDistance(string distance)
    {
        distanceText.text = distance;
    }
    public void SetPlayerHealth(int health)
    {
        playerHealthText.text = health.ToString();
    }
    public void SetEnemyHealth(int health)
    {
        enemyHealthText.text = health.ToString();
    }
}
