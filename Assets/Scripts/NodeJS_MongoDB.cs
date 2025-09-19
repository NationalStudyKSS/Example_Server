using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections;
using UnityEngine.SceneManagement;

public class NodeJS_MongoDB : MonoBehaviour
{
    [Header("Node.js + MySQL Server URLs")]
    [SerializeField] string _mySQL_URL;         // "http://localhost:3000/"
    [SerializeField] string _signIn_URL;        // "http://localhost:3000/new";
    [SerializeField] string _login_URL;         // "http://localhost:3000/login";
    [SerializeField] string _select_URL;        // "http://localhost:3000/select";
    [SerializeField] string _update_URL;        // "http://localhost:3000/update";
    [SerializeField] string _delete_URL;        // "http://localhost:3000/delete";
    [SerializeField] string _ranking_URL;       // "http://localhost:3000/ranking";
    [SerializeField] string _profile_URL;       // "http://localhost:3000/profile";

    [Header("UI Elements")]
    [SerializeField] GameObject _loadingPanel;  // �ε� �� Ȱ��ȭ�� �г�
    [SerializeField] GameObject _popUpPanel;    // �˾� �г�
    [SerializeField] GameObject _mainPanel;     // ���� �г�
    [SerializeField] GameObject _rankingPanel;  // ��ŷ �г�

    [SerializeField] InputField _inputFieldId;  // ���̵� �Է� �ʵ�
    [SerializeField] InputField _inputFieldPw;  // ��й�ȣ �Է� �ʵ�

    [SerializeField] Text _PopUpText;           // �˾�â�� ǥ���� �ؽ�Ʈ
    [SerializeField] Text _goldText;            // ��� �ؽ�Ʈ
    [SerializeField] Text _scoreText;           // ���� �ؽ�Ʈ
    [SerializeField] Text _userIdText;          // ���� ���̵� �ؽ�Ʈ

    [Header("Prefabs")]
    [SerializeField] GameObject _rankingSlotPrefab; // ��ŷ ���� ������
    [SerializeField] GameObject _content;            // ��ŷ ������ �߰��� ������ ������Ʈ

    [Header("Request Result")]
    [SerializeField] string _requestResult;     // ��û ��� ���� ����

    public enum REQUEST_TYPE
    {
        SIGNUP = 0,
        LOGIN,
        USERDATA,
        UPDATE,
        DELETE,
        RANKING
    }

    [Header("Request Type")]
    [SerializeField] REQUEST_TYPE _requestType;

    [Header("Client Data")]
    [SerializeField] int _gold;                 // ������ ������ ���
    [SerializeField] int _score;                // ������ �ְ� ����

    [SerializeField] Sprite[] _medals;
    [SerializeField] Texture2D _userPic;
    [SerializeField] Sprite _profileSprite;

    IEnumerator Request(string url, WWWForm form)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        _loadingPanel.SetActive(true);

        switch (_requestType)
        {
            case REQUEST_TYPE.SIGNUP:
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    _requestResult = webRequest.downloadHandler.text;
                    if(_requestResult == "SignIn Success")
                    {
                        Debug.Log("ȸ������ ����");
                        webRequest.Dispose();
                        yield return new WaitForSeconds(1f);
                        _loadingPanel.SetActive(false);
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "ȸ�����Կ� �����߽��ϴ�.";
                    }
                    else
                    {
                        Debug.Log("ȸ������ ����");
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "ȸ������ ����";
                        _loadingPanel.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Error: " + webRequest.error);
                    _loadingPanel.SetActive(false);
                    webRequest.Dispose();
                }
                break;
                
            case REQUEST_TYPE.LOGIN:
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    _requestResult = webRequest.downloadHandler.text;
                    if (_requestResult == "Login Success")
                    {
                        Debug.Log("�α��� ����");
                        PlayerPrefs.SetString("UserId", _inputFieldId.text);
                        webRequest.Dispose();
                        yield return new WaitForSeconds(1f);
                        _loadingPanel.SetActive(false);
                        _popUpPanel.SetActive(true);
                        _userIdText.text = _inputFieldId.text;
                        _PopUpText.text = "�α��ο� �����߽��ϴ�.";
                        _mainPanel.SetActive(true);
                        
                    }
                    else
                    {
                        Debug.Log("�α��� ����");
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "�α��� ����";
                        _loadingPanel.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Error: " + webRequest.error);
                    _loadingPanel.SetActive(false);
                    webRequest.Dispose();
                }

                break;

            case REQUEST_TYPE.USERDATA:
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    _requestResult = webRequest.downloadHandler.text;
                    var jsonData = JSON.Parse(_requestResult);
                    _gold = jsonData[0]["gold"];
                    _score = jsonData[0]["score"];
                    _goldText.text = "Gold : " + _gold;
                    _scoreText.text = "Score : " + _score;
                    webRequest.Dispose();
                }
                else
                {
                    Debug.Log("Error: " + webRequest.error);
                    webRequest.Dispose();
                }
                _loadingPanel.SetActive(false);
                break;
                
            case REQUEST_TYPE.UPDATE:
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    _requestResult = webRequest.downloadHandler.text;
                    if (_requestResult == "Update Success")
                    {
                        Debug.Log("������Ʈ ����");
                        webRequest.Dispose();
                        yield return new WaitForSeconds(1f);
                        _loadingPanel.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("������Ʈ ����");
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "������Ʈ ����";
                        _loadingPanel.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Error: " + webRequest.error);
                    _loadingPanel.SetActive(false);
                    webRequest.Dispose();
                }
                break;

            case REQUEST_TYPE.DELETE:
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    _requestResult = webRequest.downloadHandler.text;
                    if (_requestResult == "Delete Success")
                    {
                        Debug.Log("Ż�� ����");
                        PlayerPrefs.DeleteKey("UserId");
                        webRequest.Dispose();
                        yield return new WaitForSeconds(1f);
                        _loadingPanel.SetActive(false);
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "Ż�� �����߽��ϴ�.";
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                    else
                    {
                        Debug.Log("Ż�� ����");
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "Ż�� ����";
                        _loadingPanel.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Error: " + webRequest.error);
                    _loadingPanel.SetActive(false);
                    webRequest.Dispose();
                }
                break;

            case REQUEST_TYPE.RANKING:
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    _rankingPanel.SetActive(true);
                    _requestResult = webRequest.downloadHandler.text;
                    _loadingPanel.SetActive(false);
                    var jsonData = JSON.Parse(_requestResult);
                    
                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        GameObject slot = Instantiate(_rankingSlotPrefab, _content.transform);
                        slot.transform.localScale = Vector3.one;

                        slot.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
                        slot.transform.GetChild(1).GetComponent<Text>().text = jsonData[i]["userId"].ToString().Replace('"', ' ');
                        slot.transform.GetChild(2).GetComponent<Text>().text = jsonData[i]["score"].ToString();

                        yield return StartCoroutine(GetProfileImage(_profile_URL, jsonData[i]["ProfileImage"]));

                        if (jsonData[i]["ProfileImage"] != "")
                        {
                            slot.transform.GetChild(3).GetComponent<Image>().sprite = _profileSprite;
                        }

                        if (i == 0)
                        {
                            slot.transform.GetChild(4).GetComponent<Image>().sprite = _medals[i];
                        }
                        else if (i == 1)
                        {
                            slot.transform.GetChild(4).GetComponent<Image>().sprite = _medals[i];
                        }
                        else if (i == 2)
                        {
                            slot.transform.GetChild(4).GetComponent<Image>().sprite = _medals[i];
                        }
                        else
                        {
                            slot.transform.GetChild(4).gameObject.SetActive(false);
                        }
                    }
                    webRequest.Dispose();
                }
                else
                {
                    Debug.Log("Error: " + webRequest.error);
                    webRequest.Dispose();
                }
                break;

            default:

                break;
        }
    }

    public void OnClickSignUp()
    {
        if (string.IsNullOrEmpty(_inputFieldId.text) || string.IsNullOrEmpty(_inputFieldPw.text))
        {
            _PopUpText.text = "���̵�� ��й�ȣ�� ��� �Է��ϼ���.";
            _popUpPanel.SetActive(true);
            return;
        }
        else
        {
            Debug.Log("ȸ������ ����! ID : " + _inputFieldId.text + ", PW : " + _inputFieldPw.text);

            _requestType = REQUEST_TYPE.SIGNUP;
            WWWForm form = new WWWForm();
            form.AddField("userId", _inputFieldId.text);
            form.AddField("userPw", _inputFieldPw.text);

            StartCoroutine(Request(_signIn_URL, form));
        }
    }

    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(_inputFieldId.text) || string.IsNullOrEmpty(_inputFieldPw.text))
        {
            _PopUpText.text = "���̵�� ��й�ȣ�� ��� �Է��ϼ���.";
            _popUpPanel.SetActive(true);
            return;
        }
        else
        {
            Debug.Log("�α��� ����! ID : " + _inputFieldId.text + ", PW : " + _inputFieldPw.text);
            
            _requestType = REQUEST_TYPE.LOGIN;
            WWWForm form = new WWWForm();
            form.AddField("userId", _inputFieldId.text);
            form.AddField("userPw", _inputFieldPw.text);

            StartCoroutine(Request(_login_URL, form));
        }
    }

    public void GetUserInfo()
    {
        _requestType = REQUEST_TYPE.USERDATA;
        WWWForm form = new WWWForm();
        form.AddField("userId", _userIdText.text);
        _loadingPanel.SetActive(true);
        StartCoroutine(Request(_select_URL, form));
    }

    /// <summary>
    /// ������Ʈ ��ư�� ������ �� UI�� �����ϴ� �Լ�
    /// </summary>
    public void OnClickUpdateInfo()
    {
        _requestType = REQUEST_TYPE.UPDATE;
        WWWForm form = new WWWForm();
        form.AddField("userId", _userIdText.text);
        form.AddField("gold", _gold);
        form.AddField("score", _score);
        StartCoroutine(Request(_update_URL, form));

        _goldText.text = "Gold: " + _gold.ToString();
        _scoreText.text = "Score: " + _score.ToString();
    }

    public void OnClickDeleteInfo()
    {
        _requestType = REQUEST_TYPE.DELETE;
        WWWForm form = new WWWForm();
        form.AddField("userId", _userIdText.text);
        StartCoroutine(Request(_delete_URL, form));
    }

    public void OnClickRanking()
    {
        _rankingPanel.SetActive(true);
        _requestType = REQUEST_TYPE.RANKING;
        WWWForm form = new WWWForm();
        StartCoroutine(Request(_ranking_URL, form));
    }

    IEnumerator GetProfileImage(string url, string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            WWWForm form = new WWWForm();
            form.AddField("fileName", fileName);
            WWW www = new WWW(url, form);
            yield return www;
            yield return _userPic = www.texture;
            yield return _profileSprite = Sprite.Create(_userPic, new Rect(0, 0, _userPic.width, _userPic.height), new Vector2(0.5f, 0.5f));
            www.Dispose();
        }
    }

    /// <summary>
    /// ��� ���� �Ǵ� ���� ��ư�� ������ �� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="value">true�� ����</param>
    public void ControllGold(bool value)
    {
        if (value)
            _gold += 100;
        else
        {
            if (_gold >= 100)
                _gold -= 100;
        }
        _goldText.text = "Gold: " + _gold.ToString();
    }

    /// <summary>
    /// ���� ���� �Ǵ� ���� ��ư�� ������ �� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="value">true�� ����</param>
    public void ControllScore(bool value)
    {
        if (value)
            _score += 10;
        else
        {
            if (_score >= 10)
                _score -= 10;
        }
        _scoreText.text = "Score: " + _score.ToString();
    }

    public void DeleteAllRankingItems()
    {
        for (int i = 0; i < _content.transform.childCount; i++)
        {
            Destroy(_content.transform.GetChild(i).gameObject);
        }

        _rankingPanel.SetActive(false);
    }
}
