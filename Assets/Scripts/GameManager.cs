using TMPro;

public class GameManager : Singleton<GameManager>
{
    int score = 0;
    public TextMeshProUGUI scoreText;
    void Start()
    {
        score = 0;
        UpdateScore();
    }

    public void IncreaseScore()
    {
        score += 1;
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreText.text = $"Score : {score}";
    }
}
