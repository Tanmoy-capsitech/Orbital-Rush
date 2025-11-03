using System.Collections;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _highscoreText;
    [SerializeField] private TMP_Text _newbestText;

    private void Awake()
    {
        if (GameManager.Instance.IsIntialized)
        {
            StartCoroutine(ShowScore());
        }

        else
        {
            _scoreText.gameObject.SetActive(false);
            _newbestText.gameObject.SetActive(false);
            _highscoreText.text = GameManager.Instance.HighScore.ToString();
        }
    }

    [SerializeField] private float _animationTime;
    [SerializeField] private AnimationCurve _speedCurve;

    private IEnumerator ShowScore()
    {
        int tempScore = 0;
        _scoreText.text = tempScore.ToString();

        int CurrentScore = GameManager.Instance.CurrentScore;
        int highScore = GameManager.Instance.HighScore;
        Debug.Log($"Current Score: {CurrentScore}, High Score: {highScore}");

        if (CurrentScore > highScore)
        {
            Debug.Log("New high score achieved!");
            _newbestText.gameObject.SetActive(true);
            GameManager.Instance.HighScore = CurrentScore;
        }
        else
        {    
             Debug.Log("No new high score");
            _newbestText.gameObject.SetActive(false);
        }
        _highscoreText.text = GameManager.Instance.HighScore.ToString();
        Debug.Log($"Final High Score Display: {GameManager.Instance.HighScore}");

        float speed = 1 / _animationTime;
        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            timeElapsed += speed * Time.deltaTime;
            tempScore = (int)(_speedCurve.Evaluate(timeElapsed) * CurrentScore);
            _scoreText.text = tempScore.ToString();
            yield return null;

        }

        tempScore = CurrentScore;
        _scoreText.text = tempScore.ToString();
        
    }

    [SerializeField] private AudioClip _clickClip;

    public void ClickedPlay()
    {   
        Debug.Log("SoundManager: " + SoundManager.Instance);
         Debug.Log("GameManager: " + GameManager.Instance);

        SoundManager.Instance.PlaySound(_clickClip);
        GameManager.Instance.GoToGameplay(); 
    }


}
