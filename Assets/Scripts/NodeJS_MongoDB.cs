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
    [SerializeField] GameObject _loadingPanel;  // 로딩 시 활성화할 패널
    [SerializeField] GameObject _popUpPanel;    // 팝업 패널
    [SerializeField] GameObject _mainPanel;     // 메인 패널
    [SerializeField] GameObject _rankingPanel;  // 랭킹 패널

    [SerializeField] InputField _inputFieldId;  // 아이디 입력 필드
    [SerializeField] InputField _inputFieldPw;  // 비밀번호 입력 필드

    [SerializeField] Text _PopUpText;           // 팝업창에 표시할 텍스트
    [SerializeField] Text _goldText;            // 골드 텍스트
    [SerializeField] Text _scoreText;           // 점수 텍스트
    [SerializeField] Text _userIdText;          // 유저 아이디 텍스트

    [Header("Prefabs")]
    [SerializeField] GameObject _rankingSlotPrefab; // 랭킹 슬롯 프리팹
    [SerializeField] GameObject _content;            // 랭킹 슬롯이 추가될 컨텐츠 오브젝트

    [Header("Request Result")]
    [SerializeField] string _requestResult;     // 요청 결과 저장 변수

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
    [SerializeField] int _gold;                 // 유저가 보유한 골드
    [SerializeField] int _score;                // 유저의 최고 점수

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
                        Debug.Log("회원가입 성공");
                        webRequest.Dispose();
                        yield return new WaitForSeconds(1f);
                        _loadingPanel.SetActive(false);
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "회원가입에 성공했습니다.";
                    }
                    else
                    {
                        Debug.Log("회원가입 실패");
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "회원가입 실패";
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
                        Debug.Log("로그인 성공");
                        PlayerPrefs.SetString("UserId", _inputFieldId.text);
                        webRequest.Dispose();
                        yield return new WaitForSeconds(1f);
                        _loadingPanel.SetActive(false);
                        _popUpPanel.SetActive(true);
                        _userIdText.text = _inputFieldId.text;
                        _PopUpText.text = "로그인에 성공했습니다.";
                        _mainPanel.SetActive(true);
                        
                    }
                    else
                    {
                        Debug.Log("로그인 실패");
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "로그인 실패";
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
                        Debug.Log("업데이트 성공");
                        webRequest.Dispose();
                        yield return new WaitForSeconds(1f);
                        _loadingPanel.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("업데이트 실패");
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "업데이트 실패";
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
                        Debug.Log("탈퇴 성공");
                        PlayerPrefs.DeleteKey("UserId");
                        webRequest.Dispose();
                        yield return new WaitForSeconds(1f);
                        _loadingPanel.SetActive(false);
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "탈퇴에 성공했습니다.";
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                    else
                    {
                        Debug.Log("탈퇴 실패");
                        _popUpPanel.SetActive(true);
                        _PopUpText.text = "탈퇴 실패";
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
            _PopUpText.text = "아이디와 비밀번호를 모두 입력하세요.";
            _popUpPanel.SetActive(true);
            return;
        }
        else
        {
            Debug.Log("회원가입 성공! ID : " + _inputFieldId.text + ", PW : " + _inputFieldPw.text);

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
            _PopUpText.text = "아이디와 비밀번호를 모두 입력하세요.";
            _popUpPanel.SetActive(true);
            return;
        }
        else
        {
            Debug.Log("로그인 성공! ID : " + _inputFieldId.text + ", PW : " + _inputFieldPw.text);
            
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
    /// 업데이트 버튼을 눌렀을 때 UI를 갱신하는 함수
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
    /// 골드 증가 또는 감소 버튼을 눌렀을 때 호출되는 함수
    /// </summary>
    /// <param name="value">true면 증가</param>
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
    /// 점수 증가 또는 감소 버튼을 눌렀을 때 호출되는 함수
    /// </summary>
    /// <param name="value">true면 증가</param>
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
