using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;
    private GameObject paddleObject;

    public Text ScoreText;
    public Text HighScoreText;

    public GameObject GameOverText;
    
    [SerializeField] bool m_Started = false;
    private int m_Points;
    public string playerName;
    private int m_HighScore;
    private string m_BestPlayer;
    
    [SerializeField] bool m_GameOver = false;

    public static MainManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    [System.Serializable]
    class SaveData
    {
        public int highScore;
        public string bestPlayer;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LetsStart;
    }

    // Start is called before the first frame update
    void LetsStart(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "menu")
        {
            LoadHighScore();
            const float step = 0.6f;
            int perLine = Mathf.FloorToInt(4.0f / step);

            int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
            for (int i = 0; i < LineCount; ++i)
            {
                for (int x = 0; x < perLine; ++x)
                {
                    Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                    var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                    brick.PointValue = pointCountArray[i];
                    brick.onDestroyed.AddListener(AddPoint);
                }
            }
            playerName = MenuUI.text;
            m_GameOver = false;
            ScoreText.text = playerName + " Score : 0";
        }
    }


    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();
                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = false;
                paddleObject = GameObject.Find("Paddle");
                Ball = paddleObject.GetComponentInChildren<Rigidbody>();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = playerName + $" Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        SaveHighScore();
        GameOverText.SetActive(true);
    }

    public void SaveHighScore()
    {
        SaveData data = new SaveData();
        data.highScore = m_Points;
        data.bestPlayer = playerName;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefileb.json", json);
    }

    public void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/savefileb.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            HighScoreText.text = "Best Score: " + data.bestPlayer + ": " + data.highScore;
           
        } else
        {
            HighScoreText.text = "Best Score: Name: 0";
        }
    }
}
