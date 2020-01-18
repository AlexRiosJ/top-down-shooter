using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public Text gameOverScoreUI;
    public RectTransform healthBar;

    Spawner spawner;
    Player player;

    void Start () {
        player = FindObjectOfType<Player> ();
        player.OnDeath += OnGameOver;
    }

    private void Awake () {
        spawner = FindObjectOfType<Spawner> ();
        spawner.OnNewWave += OnNewWave;
    }

    void Update () {
        scoreUI.text = ScoreKeeper.score.ToString ("D6");
        float healthPercent = 0;
        if (player != null) {
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3 (healthPercent, 1, 1);
    }

    void OnNewWave (int waveNumber) {
        string[] numbers = { "One", "Two", "Three", "Four", "Five", "Six", "Seven" };
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
        newWaveEnemyCount.text = "Enemies: " + (spawner.waves[waveNumber - 1].infiniteEnemies ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");

        StopCoroutine ("AnimateNewWaveBanner");
        StartCoroutine ("AnimateNewWaveBanner");
    }

    void OnGameOver () {
        Cursor.visible = true;
        StartCoroutine (Fade (Color.clear, new Color (0, 0, 0, 0.9f), 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive (false);
        healthBar.transform.parent.gameObject.SetActive (false);

        gameOverUI.SetActive (true);
    }

    IEnumerator AnimateNewWaveBanner () {

        float delayTime = 2f;
        float speed = 2.5f;
        float animationPercent = 0;
        int direction = 1;
        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (animationPercent >= 0) {
            animationPercent += Time.deltaTime * speed * direction;

            if (animationPercent >= 1) {
                animationPercent = 1;
                if (Time.time > endDelayTime) {
                    direction = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp (-250, 15, animationPercent);
            yield return null;
        }
    }

    IEnumerator Fade (Color from, Color to, float time) {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp (from, to, percent);
            yield return null;
        }
    }

    // UI Input
    public void StartNewGame () {
        SceneManager.LoadScene ("Game");
    }

    public void ReturnToMainMenu () {
        SceneManager.LoadScene ("Menu");
    }

}