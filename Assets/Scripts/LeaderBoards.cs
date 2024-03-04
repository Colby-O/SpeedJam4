using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LeaderBoards : MonoBehaviour
{
    public RectTransform viewport;

    private GameObject _entryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _entryPrefab = (GameObject)Resources.Load("LeaderBoardEntry");
        Reload();
    }


    [System.Serializable]
    public class Entry
    {
        public string name;
        public int time;
        public int balloons;
    }

    [System.Serializable]
    public class LeaderBoardData
    {
        public List<Entry> scores;

        public static LeaderBoardData CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<LeaderBoardData>(jsonString);
        }
    }

    IEnumerator GetRequest()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://ddmo.fr.to/speed/get-scores?count=100"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                LeaderBoardData e = LeaderBoardData.CreateFromJSON(webRequest.downloadHandler.text);
                LoadLeaderBoards(e.scores);
            }
        }
    }

    public void Reload()
    {
        StartCoroutine(GetRequest());
    }

    public void LoadLeaderBoards(List<Entry> entries)
    {
        int childCount = viewport.transform.childCount;
        for (int j = 1; j < childCount; j++)
        {
            Destroy(viewport.transform.GetChild(1).gameObject);
        }

        viewport.sizeDelta = new Vector2(0, 32 + entries.Count * 15);

        int i = 0;
        foreach (var entry in entries)
        {
            GameObject go = GameObject.Instantiate(_entryPrefab, viewport);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector3(0, -32 - i * 15, 0);

            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = entry.name;
            go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.time.ToString();
            go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = entry.balloons.ToString();
            i++;
        }
    }

}