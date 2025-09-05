using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class Example_Server : MonoBehaviour
{
    [SerializeField] string _serverURL;

    IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequest.Get(_serverURL);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            var jsonData = JSON.Parse(www.downloadHandler.text);

            Debug.Log(jsonData["name"]);
            Debug.Log(jsonData["age"]);
            Debug.Log(jsonData["city"]);
        }
    }
}
