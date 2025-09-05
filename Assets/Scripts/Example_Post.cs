using SimpleJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Example_Post : MonoBehaviour
{
    [SerializeField] string _serverURL;

    IEnumerator Start()
    {
        Debug.Log("Coroutine started");  // 여기 찍히는지 확인
        WWWForm form = new WWWForm();
        form.AddField("user", "UNITY");
        form.AddField("pw", "1q2w3e4r");

        UnityWebRequest www = UnityWebRequest.Post(_serverURL, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
        }
    }
}