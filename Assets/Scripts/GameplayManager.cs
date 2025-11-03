using System.Collections;
using TMPro;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private GameObject _scorePrefab;

    private int score;
    private int individualPoints = 0;

    private void Awake()
    {
         if (GameManager.IsReady())
    {
        GameManager.Instance.IsIntialized = true;
    }

        score = 0;
        _scoreText.text = score.ToString();
        SpawnScore();
    }

    public void UpdateScore()
    {
        score++;
        individualPoints++;
        
        _scoreText.text = score.ToString();
        SpawnScore();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(1);
        }
    }

    public int GetIndividualPoints() => individualPoints;

    private void SpawnScore()
    {
        Instantiate(_scorePrefab);
    }

    public void GameEnded()
    {
        GameManager.Instance.CurrentScore = score;
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.GoToMainMenu();
    }
}