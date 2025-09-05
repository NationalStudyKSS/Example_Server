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

    [Header("Client Data")]
    [SerializeField] int _gold;                 // ������ ������ ���
    [SerializeField] int _score;                // ������ �ְ� ����

    [Header("Prefabs")]
    [SerializeField] GameObject _rankingSlotPrefab; // ��ŷ ���� ������

    private void Start()
    {
        // ���� �� �ε� �г� ��Ȱ��ȭ
        _loadingPanel.SetActive(false);
        // ���� ���� �׽�Ʈ �ڷ�ƾ ����
        //StartCoroutine(TestConnect());
    }

    IEnumerator TestConnect()
    {
        // ������ ���۵Ǹ� �ε� �г��� Ȱ��ȭ
        _loadingPanel.SetActive(true);

        // ������ GET ��û ������
        UnityWebRequest www = UnityWebRequest.Get(_mySQL_URL);

        // ��û ���� �� ���� ���
        yield return www.SendWebRequest();

        yield return new WaitForSeconds(1f);

        // �������� ���� ���� ���
        Debug.Log(www.downloadHandler.text);

        // ������ ������ �ε� �г��� ��Ȱ��ȭ
        _loadingPanel.SetActive(false);
    }

    public void OnClickSignUp()
    {
        if(string.IsNullOrEmpty(_inputFieldId.text) || string.IsNullOrEmpty(_inputFieldPw.text))
        {
            _PopUpText.text = "���̵�� ��й�ȣ�� ��� �Է��ϼ���.";
            _popUpPanel.SetActive(true);
            return;
        }
        else
        {
            Debug.Log("ȸ������ ����! ID : " + _inputFieldId.text + ", PW : " + _inputFieldPw.text);
            
            StartCoroutine(SignIn(_inputFieldId.text, _inputFieldPw.text));
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
            // ���⼭ ���
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
            // ���� ������ /new ����� ���� �޼����� ���ؼ� ������
            if (www.downloadHandler.text == "Insert Successfully")
            {
                _PopUpText.text = "ȸ�������� �Ϸ�Ǿ����ϴ�.";
                _popUpPanel.SetActive(true);
            }
            else
            {
                _popUpPanel.SetActive(true);
                _PopUpText.text = "�̹� �����ϴ� ���̵��Դϴ�.";
            }
        }

        yield return new WaitForSeconds(1f);
        _loadingPanel.SetActive(false);
    }

    /// <summary>
    /// id�� pw�� ���� �α����� �õ��ϰ�
    /// ����� ���� 
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
            // ���� ������ /login ����� ���� �޼����� ���ؼ� ������
            if (www.downloadHandler.text == "Login Successfully")
            {
                _popUpPanel.SetActive(true);
                _loadingPanel.SetActive(false);
                _PopUpText.text = "�α��� ����.";
                // �α��� ���� �� id�� ���� ������ �������� �ڷ�ƾ ����
                StartCoroutine(GetUserInfo(id));
            }
            else
            {
                _popUpPanel.SetActive(true);
                _PopUpText.text = "�Է� ������ �߸��Ǿ����ϴ�.";
            }
        }

        yield return new WaitForSeconds(1f);
        _loadingPanel.SetActive(false);
    }

    /// <summary>
    /// ������ id�� ���� ������ �������� �ڷ�ƾ
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IEnumerator GetUserInfo(string id)
    {
        // ������ ���۵Ǹ� �ε� �г��� Ȱ��ȭ
        _loadingPanel.SetActive(true);

        // WWWForm ������ ��ü ����
        WWWForm form = new WWWForm();
        // form�� �Է¹��� id�� ������� "user"��� Ű������ �߰�
        // user��� Ű���� �����ϴ°���
        form.AddField("user", id);

        // POST ������� ������ ������ ����
        UnityWebRequest www = UnityWebRequest.Post(_select_URL, form);
        // ��û ���� �� ���� ���
        yield return www.SendWebRequest();

        // ���� ���������� ������ ���� ���ߴٸ�
        if (www.result != UnityWebRequest.Result.Success)
        {
            // ���� ����� ���� �޼��� ���
            Debug.Log(www.result);
            Debug.Log(www.error);

            // �˾� �г� Ȱ��ȭ �� ���� �޼��� ǥ��
            _popUpPanel.SetActive(true);
            _PopUpText.text = www.error;
        }
        // ������ ���������� �޾Ҵٸ�
        else
        {
            // �������� ���� ������ debug.log�� ���
            Debug.Log(www.downloadHandler.text);
            // JSON �������� �Ľ�
            var jsonData = JSON.Parse(www.downloadHandler.text);

            Debug.Log("Gold : " + jsonData[0]["Gold"].GetType());
            Debug.Log("Score : " + jsonData[0]["Score"].GetType());

            _gold = jsonData[0]["Gold"];
            _score = jsonData[0]["Score"];

            // UI �ؽ�Ʈ ������Ʈ �� ���� �г� Ȱ��ȭ
            _goldText.text = "Gold: " + _gold.ToString();
            _scoreText.text = "Score: " + _score.ToString();
            _mainPanel.SetActive(true);
        }
        yield return new WaitForSeconds(1f);
        _loadingPanel.SetActive(false);
    }
}