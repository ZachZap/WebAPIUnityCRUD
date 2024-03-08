using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.Networking;


public class JsonSerializer : MonoBehaviour
{
    public DataClass dataObj;
    public string filePath;
    public TMP_InputField userName, score, dateStarted, firstName, lastName;
    public List<PlayerDatum> fullDataList;
    public TMP_Text displayText;
    public PlayerDatum currentlySelectedEntry;

    // Start is called before the first frame update
    void Start()
    {
        displayText.autoSizeTextContainer = true;
        filePath = Path.Combine(Application.dataPath, "saveData.txt");
        //dataObj = new DataClass();
        //dataObj.level = 1;
        //dataObj.timeElapsed = 44443.232323f;
        //dataObj.name = "Jordan";
        string json = JsonUtility.ToJson(dataObj);
        Debug.Log(json);
        //StartCoroutine(SendWebData(json));
        File.WriteAllText(filePath, json);

        StartCoroutine(GetRequest("http://localhost:3000/getUnity"));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendButton()
    {
        var scoreData = int.Parse(score.text);
        var dateData = dateStarted.text;
        DataClass formData = new DataClass();
        formData.score = scoreData;
        formData.dateJoined = dateData;    
        formData.userName = userName.text;
        formData.firstName = firstName.text;
        formData.lastName = lastName.text;
        string json = JsonUtility.ToJson(formData);
        filePath = Path.Combine(Application.dataPath, "formData.txt");
        File.WriteAllText(filePath, json);
        StartCoroutine(SendWebData(json));

    }

    public void GetButton() {
        StartCoroutine(GetRequest("http://localhost:3000/getUnity"));
        
    }

    public void FindButton()
    {
        Debug.Log(userName.text);
        FindEntry(userName.text);
    }

    IEnumerator SendWebData(string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Post("http://localhost:3000/unity", json, "application-json"))
        {
            request.SetRequestHeader("content-type", "application/json");

            request.uploadHandler.contentType = "application/json";
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return request.SendWebRequest();

            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Data Posted");
            }
            request.uploadHandler.Dispose();
        }
    }

    IEnumerator UpdateWebData(string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Post("http://localhost:3000/updateUnity", json, "application-json"))
        {
            request.SetRequestHeader("content-type", "application/json");

            request.uploadHandler.contentType = "application/json";
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Data Posted");
            }
            request.uploadHandler.Dispose();
        }
    }

    IEnumerator DeleteWebData(string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Post("http://localhost:3000/deleteUnity", json, "application-json"))
        {
            request.SetRequestHeader("content-type", "application/json");

            request.uploadHandler.contentType = "application/json";
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Data Posted");
            }
            request.uploadHandler.Dispose();
        }
    }


    public void UpdateEntryButton()
    {
        if (currentlySelectedEntry.userName == string.Empty)
        {
            Debug.Log("No entry loaded");
            
        }
        else
        {
            currentlySelectedEntry.userName = userName.text;
            currentlySelectedEntry.firstName = firstName.text;
            currentlySelectedEntry.lastName = lastName.text;
            currentlySelectedEntry.score = int.Parse(score.text);
            currentlySelectedEntry.dateJoined = dateStarted.text;
            Debug.Log(FindEntryByID(currentlySelectedEntry._id));

            fullDataList[FindEntryByID(currentlySelectedEntry._id)].SetTo(currentlySelectedEntry);
            Debug.Log(fullDataList[FindEntryByID(currentlySelectedEntry._id)].userName);
            SortArray(fullDataList);

            DeconstructAndDisplay(fullDataList, displayText);

            string json = JsonUtility.ToJson(currentlySelectedEntry);
            filePath = Path.Combine(Application.dataPath, "formData.txt");
            File.WriteAllText(filePath, json);
            StartCoroutine(UpdateWebData(json));
        }
    }

    public void DeleteEntryButton()
    {
        
        if (currentlySelectedEntry.userName == string.Empty)
        {
            Debug.Log("No entry loaded");

        }
        else
        {
            fullDataList.Remove(fullDataList[FindEntryByID(currentlySelectedEntry._id)]);

            SortArray(fullDataList);

            DeconstructAndDisplay(fullDataList, displayText);

            string json = JsonUtility.ToJson(currentlySelectedEntry);
            filePath = Path.Combine(Application.dataPath, "formData.txt");
            File.WriteAllText(filePath, json);
            StartCoroutine(DeleteWebData(json));
        }
    }

    public void FindEntry(string datumName)
    {
        bool isFound = false;
        for (int i = 0; i < fullDataList.Count - 1;i++ )
        {
            if (fullDataList[i].userName.ToLower() == datumName.ToLower())
            {
                Debug.Log(fullDataList[i].userName);
                score.text = fullDataList[i].score.ToString();
                dateStarted.text = fullDataList[i].dateJoined;
                firstName.text = fullDataList[i].firstName;
                lastName.text = fullDataList[i].lastName;
                currentlySelectedEntry = fullDataList[i];
                isFound = true;
                break;
            }
        }
        if (!isFound)
            Debug.Log("No entry of that name found!");
    }

    public int FindEntryByID(string datumID)
    {
        for (int i = 0; i < fullDataList.Count - 1; i++)
        {
            if (fullDataList[i]._id == datumID)
            {
                return i;
            }
        }
        Debug.Log("No entry of that id found!");
        return 0000;
    }

    public List<PlayerDatum> SortArray(List<PlayerDatum> data)
    {
        var n = data.Count;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                if (string.Compare(data[j].userName, data[j + 1].userName) == 1)
                {
                    var tempVar = data[j];
                    data[j] = data[j + 1];
                    data[j + 1] = tempVar;
                }
        return data;
    }
    public void DeconstructAndDisplay(List<PlayerDatum> playerData, TMP_Text display)
    {
        display.text = "";
        for (int i = 0; i < playerData.Count; i++)
        {
            display.text += "Username: " + playerData[i].userName + "\nFirst Name: " + playerData[i].firstName + "\nLast Name: " + playerData[i].lastName + "\nScore: " + playerData[i].score + "\nDate Joined: " + playerData[i].dateJoined + "\n-----------------------------------------\n";
            
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest getRequest = UnityWebRequest.Get(uri))
        {
            yield return getRequest.SendWebRequest();

            var newData = System.Text.Encoding.UTF8.GetString(getRequest.downloadHandler.data);
            Debug.Log(newData);
            var newGetRequestData = JsonUtility.FromJson<DataClass>(newData);
            Debug.Log(newGetRequestData);
            Debug.Log(newGetRequestData);

            var newGetRoot = JsonUtility.FromJson<Root>(newData);
            Debug.Log(newGetRoot.playerdata[0].score);
            fullDataList.Clear();
            for (int i = 0; i<newGetRoot.playerdata.Length; i++ )
            {
                Debug.Log(newGetRoot.playerdata[i].userName);
                Debug.Log(newGetRoot.playerdata[i].firstName);
                Debug.Log(newGetRoot.playerdata[i].lastName);
                Debug.Log(newGetRoot.playerdata[i].score);
                Debug.Log(newGetRoot.playerdata[i].dateJoined);
                fullDataList.Add(newGetRoot.playerdata[i]);
                Debug.Log("Added user " + fullDataList[i].userName);
            }

            fullDataList = SortArray(fullDataList);

            DeconstructAndDisplay(fullDataList, displayText);
            //Debug.Log(newGetRequestData.name);
            //Debug.Log(newGetRequestData.level);
            //Debug.Log(newGetRequestData.timeElapsed);
        }
    }
}
