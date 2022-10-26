using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    public static GameManager Instance{set; get;}
    public bool isDead { set; get; }
    private bool isGameStarted = false;
    private PlayerMotor motor;
    public Animator gameCanvasAnim;

    // UI and UI fields
    public TextMeshProUGUI scoreText, coinText, modifierText, hiscoreText;
    private float score, coinScore, modifierScore;
    public int lastScore;

    //Death  Menu
    public Animator deathMenuAnim, menuAnim;
    public TextMeshProUGUI deadCointext, deadScoreText;
    private void Awake()
    {
        Instance = this;
        modifierScore = 1;
        modifierText.text = "x" + modifierScore.ToString("0.0");
        coinText.text = coinScore.ToString();
        scoreText.text = score.ToString("0");
        hiscoreText.text = PlayerPrefs.GetInt("Hiscore").ToString();
    }

    private void Update()
    {
        if(MobileInput.Instance.Tap && !isGameStarted)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) // remember to change to mobile
                return;
           StartPlayTrigger();
        }

        if (isGameStarted && !isDead)
        {
            //scores up
            score += (Time.deltaTime * modifierScore);
            if (lastScore != (int)score) {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
        }
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void GetCoin()
    {
        coinScore++;
        score += COIN_SCORE_AMOUNT;
        coinText.text = coinScore.ToString();
        scoreText.text = score.ToString("0");
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("game");
    }

    public void OnDeath()
    {
        isDead = true;
        deadCointext.text = coinScore.ToString("0");
        deadScoreText.text = score.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        gameCanvasAnim.SetTrigger("Hide");

        if (score > PlayerPrefs.GetInt("Hiscore"))
        {
            float s = score;
            if (s % 1 == 0)
                s += 1;
            PlayerPrefs.SetInt("Hiscore", (int)s);
        }
    }
    public void StartPlayTrigger()
    {
        isGameStarted = true;
        FindObjectOfType<PlayerMotor>().StartRunning();
        FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        FindObjectOfType<CameraMotor>().IsMoving = true;
        gameCanvasAnim.SetTrigger("Show");
        menuAnim.SetTrigger("HideMenu");
    }
}
