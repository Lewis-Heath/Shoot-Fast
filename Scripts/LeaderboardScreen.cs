using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class LeaderboardScreen : MonoBehaviour
{
    [SerializeField] private GameObject m_Names;
    [SerializeField] private GameObject m_Scores;
    [SerializeField] private GameObject m_Times;
    [SerializeField] private GameObject m_Accuracys;

    private void OnEnable()
    {
        ShowLeaderboard();
    }

    public void MenuSelected()
    {
        Debug.Log("Menu");
        SceneManager.LoadScene(0);
    }

    private void ShowLeaderboard()
    {
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

            for(int i = 0; i < a.Length; i++)
            {
                if(a[i] == ' ')
                {
                    if(posOfSpace1 == 0)
                    {
                        posOfSpace1 = i;
                    }

                    else if(posOfSpace1 != 0 && posOfSpace2 == 0)
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
            scores.Add(line.Substring(posOfSpace1 + 1, posOfSpace2 - posOfSpace1));
            times.Add(line.Substring(posOfSpace2 + 1, posOfSpace3 - posOfSpace2));
            accuracys.Add(line.Substring(posOfSpace3 + 1, (a.Length - 1) - posOfSpace3));
            counter++;
        }

        int c = 0;
        foreach (Transform child in m_Names.transform)
        {
            if(child.name != "Name")
            {
                child.gameObject.GetComponent<TMP_Text>().text = names[c];
                c++;
            }
        }

        c = 0;
        foreach (Transform child in m_Scores.transform)
        {
            if(child.name != "Score")
            {
                child.gameObject.GetComponent<TMP_Text>().text = scores[c];
                c++;
            }
        }

        c = 0;
        foreach (Transform child in m_Times.transform)
        {
            if(child.name != "Time")
            {
                child.gameObject.GetComponent<TMP_Text>().text = times[c];
                c++;
            }
        }

        c = 0;
        foreach (Transform child in m_Accuracys.transform)
        {
            if(child.name != "Accuracy")
            {
                child.gameObject.GetComponent<TMP_Text>().text = accuracys[c];
                c++;
            }
        }
    }
}
