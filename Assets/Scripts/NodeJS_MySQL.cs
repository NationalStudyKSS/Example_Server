using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections;
using Unity.Android.Gradle.Manifest;

public class NodeJS_MySQL : MonoBehaviour
{
    [Header("Node.js + MySQL Server URLs")]
    [SerializeField] string _mySQL_URL;         // "http://localhost:3000/"
    [SerializeField] string _signIn_URL;        // "http://localhost:3000/new";
    [SerializeField] string _login_URL;         // "http://localhost:3000/login";
    [SerializeField] string _select_URL;        // "http://localhost:3000/select";
    [SerializeField] string _update_URL;        // "http://localhost:3000/update";
    [SerializeField] string _delete_URL;        // "http://localhost:3000/delete";
    [SerializeField] string _ranking_URL;       // "http://localhost:3000/ranking";

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

    [Header("Client Data")]
    [SerializeField] int _gold;                 // 유저가 보유한 골드
    [SerializeField] int _score;                // 유저의 최고 점수

    [Header("Prefabs")]
    [SerializeField] GameObject _rankingSlotPrefab; // 랭킹 슬롯 프리팹

    private void Start()
    {
        // 시작 시 로딩 패널 비활성화
        _loadingPanel.SetActive(false);
        // 서버 연결 테스트 코루틴 시작
        //StartCoroutine(TestConnect());
    }

    IEnumerator TestConnect()
    {
        // 연결이 시작되면 로딩 패널을 활성화
        _loadingPanel.SetActive(true);

        // 서버에 GET 요청 보내기
        UnityWebRequest www = UnityWebRequest.Get(_mySQL_URL);

        // 요청 전송 및 응답 대기
        yield return www.SendWebRequest();

        yield return new WaitForSeconds(1f);

        // 서버에서 받은 응답 출력
        Debug.Log(www.downloadHandler.text);

        // 연결이 끝나면 로딩 패널을 비활성화
        _loadingPanel.SetActive(false);
    }

    public void OnClickSignUp()
    {
        if(string.IsNullOrEmpty(_inputFieldId.text) || string.IsNullOrEmpty(_inputFieldPw.text))
        {
            _PopUpText.text = "아이디와 비밀번호를 모두 입력하세요.";
            _popUpPanel.SetActive(true);
            return;
        }
        else
        {
            Debug.Log("회원가입 성공! ID : " + _inputFieldId.text + ", PW : " + _inputFieldPw.text);
            
            StartCoroutine(SignIn(_inputFieldId.text, _inputFieldPw.text));
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
            // 여기서 통신
            StartCoroutine(LogIn(_inputFieldId.text, _inputFieldPw.text));
        }
    }

    IEnumerator SignIn(string id, string pw)
    {
        _loadingPanel.SetActive(true);

        WWWForm form = new WWWForm();
        form.AddField("user", id);
        form.AddField("pw", pw);

        UnityWebRequest www = UnityWebRequest.Post(_signIn_URL, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.result);
            Debug.Log(www.error);
            _popUpPanel.SetActive(true);
            _PopUpText.text = www.error;
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            // 내가 설정한 /new 경로의 성공 메세지와 비교해서 같으면
            if (www.downloadHandler.text == "Insert Successfully")
            {
                _PopUpText.text = "회원가입이 완료되었습니다.";
                _popUpPanel.SetActive(true);
            }
            else
            {
                _popUpPanel.SetActive(true);
                _PopUpText.text = "이미 존재하는 아이디입니다.";
            }
        }

        yield return new WaitForSeconds(1f);
        _loadingPanel.SetActive(false);
    }

    /// <summary>
    /// id와 pw를 통해 로그인을 시도하고
    /// 결과에 따라 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    IEnumerator LogIn(string id, string pw)
    {
        _loadingPanel.SetActive(true);

        WWWForm form = new WWWForm();
        form.AddField("user", id);
        form.AddField("pw", pw);

        UnityWebRequest www = UnityWebRequest.Post(_login_URL, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.result);
            Debug.Log(www.error);
            _popUpPanel.SetActive(true);
            _PopUpText.text = www.error;
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            // 내가 설정한 /login 경로의 성공 메세지와 비교해서 같으면
            if (www.downloadHandler.text == "Login Successfully")
            {
                _popUpPanel.SetActive(true);
                _loadingPanel.SetActive(false);
                _PopUpText.text = "로그인 성공.";
                // 로그인 성공 시 id를 통해 정보를 가져오는 코루틴 실행
                StartCoroutine(GetUserInfo(id));
            }
            else
            {
                _popUpPanel.SetActive(true);
                _PopUpText.text = "입력 정보가 잘못되었습니다.";
            }
        }

        yield return new WaitForSeconds(1f);
        _loadingPanel.SetActive(false);
    }

    /// <summary>
    /// 유저의 id를 통해 정보를 가져오는 코루틴
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IEnumerator GetUserInfo(string id)
    {
        // 연결이 시작되면 로딩 패널을 활성화
        _loadingPanel.SetActive(true);

        // WWWForm 형식의 객체 생성
        WWWForm form = new WWWForm();
        // form에 입력받은 id를 기반으로 "user"라는 키값으로 추가
        // user라는 키값을 생성하는거임
        form.AddField("user", id);

        // POST 방식으로 서버에 데이터 전송
        UnityWebRequest www = UnityWebRequest.Post(_select_URL, form);
        // 요청 전송 및 응답 대기
        yield return www.SendWebRequest();

        // 만약 성공적으로 응답을 받지 못했다면
        if (www.result != UnityWebRequest.Result.Success)
        {
            // 응답 결과와 에러 메세지 출력
            Debug.Log(www.result);
            Debug.Log(www.error);

            // 팝업 패널 활성화 및 에러 메세지 표시
            _popUpPanel.SetActive(true);
            _PopUpText.text = www.error;
        }
        // 응답을 성공적으로 받았다면
        else
        {
            // 서버에서 받은 응답을 debug.log로 출력
            Debug.Log(www.downloadHandler.text);
            // JSON 형식으로 파싱
            var jsonData = JSON.Parse(www.downloadHandler.text);

            Debug.Log("Gold : " + jsonData[0]["Gold"].GetType());
            Debug.Log("Score : " + jsonData[0]["Score"].GetType());

            _gold = jsonData[0]["Gold"];
            _score = jsonData[0]["Score"];

            // UI 텍스트 업데이트 및 메인 패널 활성화
            _goldText.text = "Gold: " + _gold.ToString();
            _scoreText.text = "Score: " + _score.ToString();
            _mainPanel.SetActive(true);
        }
        yield return new WaitForSeconds(1f);
        _loadingPanel.SetActive(false);
    }
}