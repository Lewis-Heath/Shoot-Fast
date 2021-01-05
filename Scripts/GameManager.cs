using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_Canvas;
    [SerializeField] private ScreenEntity[] m_Screens;
    [SerializeField] private Text m_ScoreNum;
    [SerializeField] private Text m_TimerNum;
    [SerializeField] private Text m_AccuracyNum;
    [SerializeField] private Text m_EnemiesLeft;
    [SerializeField] private float m_EnemyNumber;

    public string m_PlayerName;
    public string m_PlayerScore;
    public string m_PlayerTime;
    public string m_PlayerAccuracy;

    private TextManager m_TextManager;
    private AudioManager m_AudioManager;

    private float m_EndGameCounter = 0.0f;
    private bool m_GameEnding = false;
    private bool m_GameEnded = false;
    public float m_Score = 0;
    public float m_Time = 0.0f;
    public float m_Accuracy;
    private float m_BulletsFired = 1f;
    private float m_BulletsHit = 1f;
    public ScreenType m_CurrentScreen = ScreenType.Playing;

    void Start()
    {
        m_TextManager = FindObjectOfType<TextManager>();
        m_AudioManager = FindObjectOfType<AudioManager>();
        m_EnemiesLeft.text = m_EnemyNumber.ToString();
    }

    void Update()
    {
        m_Time += Time.deltaTime;
        double temp = System.Math.Round(m_Time, 2);
        m_TimerNum.text = temp.ToString();

        float accuracyNum = (m_BulletsHit / m_BulletsFired) * 100;
        m_Accuracy = Mathf.Round(accuracyNum);
        string strAccuracy = m_Accuracy.ToString() + "%";
        m_AccuracyNum.text = strAccuracy;

        if (m_CurrentScreen != ScreenType.EndScreen && m_CurrentScreen != ScreenType.NameScreen && Input.GetKeyDown(KeyCode.P))
        {
            if (m_CurrentScreen != ScreenType.Paused)
            {
                EnableScreen("Pause");
                m_CurrentScreen = ScreenType.Paused;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
            }
            else
            {
                DisableScreen("Pause");
                m_CurrentScreen = ScreenType.Playing;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
            }
        }

        if (m_EnemyNumber <= 0f || Input.GetKeyDown(KeyCode.M) && !m_GameEnded)
        {
            m_GameEnding = true;
        }

        if(m_GameEnding && !m_GameEnded)
        {
            if(m_EndGameCounter > 0.5f)
            {
                EndGame();
                m_GameEnded = true;
            }
            else
            {
                m_EndGameCounter += Time.deltaTime;
            }
        }
    }

    public void ChangeTotalMoney(int amount)
    {
        m_Score += amount;
        m_ScoreNum.text = m_Score.ToString();
        if(m_EnemyNumber > 1)
        {
            if (amount < 0)
                m_TextManager.ShowTextCanvas(new Vector2(-600f, 520f), amount.ToString(), Color.red, new Vector2(600f, 200f));
            else
            {
                string text = "+" + amount.ToString();
                m_TextManager.ShowTextCanvas(new Vector2(-750f, 520f), text, Color.green, new Vector2(600f, 200f));
            }
        }
    }

    private void EndGame()
    {
        m_AudioManager.gameObject.SetActive(false);

        float scoreMultiplier = -(m_Time - 1000f);
        scoreMultiplier = scoreMultiplier / 100;
        m_Score *= scoreMultiplier;

        float accuracyMultiplier = m_Accuracy / 10;
        m_Score *= accuracyMultiplier;

        m_Score = Mathf.Round(m_Score);
        m_Time = Mathf.Round(m_Time);

        m_Canvas.transform.Find("HUD").gameObject.SetActive(false);
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.None;
        EnableScreen("NameScreen");
        m_CurrentScreen = ScreenType.NameScreen;
    }

    public void IncreaseBulletHit()
    {
        m_BulletsHit += 1;
        m_BulletsFired += 1;
    }

    public void IncreaseBulletMiss()
    {
        m_BulletsFired += 1;
    }

    public void DecreaseEnemyCount()
    {
        m_EnemyNumber -= 1f;
        m_EnemiesLeft.text = m_EnemyNumber.ToString();
    }

    public void EnableScreen(string name)
    {
        ScreenEntity screenToEnable = Array.Find(m_Screens, screen => screen.m_Name == name);
        screenToEnable.m_GameObject.SetActive(true);
    }

    public void DisableScreen(string name)
    {
        ScreenEntity screenToDisable = Array.Find(m_Screens, screen => screen.m_Name == name);
        screenToDisable.m_GameObject.SetActive(false);
    }

    public void UpdateLeaderboard()
    {
        m_PlayerScore = m_Score.ToString();
        m_PlayerTime = m_Time.ToString();
        m_PlayerAccuracy = m_Accuracy.ToString();

        string filePath = Application.streamingAssetsPath + "/Leaderboard.txt";
        List<string> fileLines = File.ReadAllLines(filePath).ToList();

        int counter = 0;
        List<string> names = new List<string>();
        List<string> scores = new List<string>();
        List<string> times = new List<string>();
        List<string> accuracys = new List<string>();
        foreach (string line in fileLines)
        {
            string a;
            a = line;
            int posOfSpace1 = 0;
            int posOfSpace2 = 0;
            int posOfSpace3 = 0;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == ' ')
                {
                    if (posOfSpace1 == 0)
                    {
                        posOfSpace1 = i;
                    }

                    else if (posOfSpace1 != 0 && posOfSpace2 == 0)
                    {
                        posOfSpace2 = i;
                    }

                    else if (posOfSpace1 != 0 && posOfSpace2 != 0 && posOfSpace3 == 0)
                    {
                        posOfSpace3 = i;
                    }
                }
            }
            if (counter == 10)
                break;

            names.Add(line.Substring(0, posOfSpace1));
            scores.Add(line.Substring(posOfSpace1 + 1, (posOfSpace2 - posOfSpace1)-1));
            times.Add(line.Substring(posOfSpace2 + 1, (posOfSpace3 - posOfSpace2)-1));
            accuracys.Add(line.Substring(posOfSpace3 + 1, (a.Length - 1) - posOfSpace3));
            counter++;
        }


        int indexOfSmallestNumber = IndexofSmallestElement(scores, 10);
        if(Int32.Parse(scores[9]) < Int32.Parse(m_PlayerScore))
        {
            names[indexOfSmallestNumber] = m_PlayerName;
            scores[indexOfSmallestNumber] = m_PlayerScore;
            times[indexOfSmallestNumber] =  m_PlayerTime;
            accuracys[indexOfSmallestNumber] = m_PlayerAccuracy;
        }

        int n = 10;
        string tempName;
        string tempScore;
        string tempTime;
        string tempAccuracy;

        for (int i = 0; i < n; i++)
        {
            for(int j = i+1; j < n; j++)
            {
                if(Int32.Parse(scores[i]) > Int32.Parse(scores[j]))
                {
                    tempName = names[i];
                    tempScore = scores[i];
                    tempTime = times[i];
                    tempAccuracy = accuracys[i];

                    names[i] = names[j];
                    scores[i] = scores[j];
                    times[i] = times[j];
                    accuracys[i] = accuracys[j];

                    names[j] = tempName;
                    scores[j] = tempScore;
                    times[j] = tempTime;
                    accuracys[j] = tempAccuracy;
                }
            }
        }

        File.WriteAllText(filePath, String.Empty);

        for (int i = 9; i > -1; i--)
        {
            string entry = names[i] + " " + scores[i] + " " + times[i] + " " + accuracys[i];
            File.AppendAllText(filePath, entry + Environment.NewLine);
        }
    }

    public int IndexofSmallestElement(List<string> list, int size) // Returns the smallest index of the array passed in.
    {
        int index = 0;

        if (size != 1)
        {
            int n = Int32.Parse(list[0]);
            for (int i = 1; i < size; i++)
            {
                if (Int32.Parse(list[i]) < n)
                {
                    n = Int32.Parse(list[i]);
                    index = i;
                }
            }
        }
        return index;
    }
}
