using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Example_FireStore : MonoBehaviour
{
    [SerializeField] string _userEmail;
    [SerializeField] string _userPassword;
    [SerializeField] bool _isExist = false;

    [SerializeField] InputField _emailInputField;
    [SerializeField] InputField _passwordInputField;
    [SerializeField] Text _logText;
    [SerializeField] Text _popupText;

    [SerializeField] GameObject _popupPanel;
    [SerializeField] GameObject _mainPanel;
    [SerializeField] GameObject _loadingPanel;
    [SerializeField] GameObject _rankingPanel;
    [SerializeField] GameObject _content;

    [SerializeField] int _gold;
    [SerializeField] int _score;

    [SerializeField] Text _goldText;
    [SerializeField] Text _scoreText;
    [SerializeField] Text _userIdText;

    [SerializeField] GameObject _rankingPrefab;

    [SerializeField] FirebaseAuth _auth;

    FirebaseFirestore _db;

    private void Start()
    {
        _db = FirebaseFirestore.DefaultInstance;
        StartCoroutine(TestAddData());
    }

    public void OnClickSignUp()
    {
        _userEmail = _emailInputField.text;
        _userPassword = _passwordInputField.text;
        if (string.IsNullOrEmpty(_userEmail) || string.IsNullOrEmpty(_userPassword))
        {
            ShowPopup("���̵�� �н����带 ��� �Է����ּ���.");
            return;
        }
        StartCoroutine(CreateUser());
    }

    public void OnClickLogIn()
    {
        _userEmail = _emailInputField.text;
        _userPassword = _passwordInputField.text;
        if (string.IsNullOrEmpty(_userEmail) || string.IsNullOrEmpty(_userPassword))
        {
            ShowPopup("���̵�� �н����带 ��� �Է����ּ���.");
            return;
        }
        StartCoroutine(CheckUser());
    }

    public void OnClickUpdate()
    {
        if (!_isExist)
        {
            ShowPopup("�α��� �� �̿����ּ���.");
            return;
        }
        _loadingPanel.SetActive(true);
        StartCoroutine(UpdateUserData());
    }

    public void OnClickDelete()
    {
        if (!_isExist)
        {
            ShowPopup("�α��� �� �̿����ּ���.");
            return;
        }
        _loadingPanel.SetActive(true);
        StartCoroutine(DeleteUser());
    }

    public void OnClickRanking()
    {
        if (!_isExist)
        {
            ShowPopup("�α��� �� �̿����ּ���.");
            return;
        }
        _loadingPanel.SetActive(true);
        _rankingPanel.SetActive(true);
        StartCoroutine(LoadRankingData());
    }

    IEnumerator CreateUser()
    {
        _loadingPanel.SetActive(true);
        yield return _db = FirebaseFirestore.DefaultInstance;

        DocumentReference docRef;
        yield return docRef = _db.Collection("users").Document(_userEmail);

        Dictionary<string, object> user = new Dictionary<string, object>
            {
            { "gold", 1000 },
            {"profileImage", "..." },
            { "score", 0 },
            {"updateTime", FieldValue.ServerTimestamp },
            {"userPW", _userPassword }
            };

        yield return docRef.SetAsync(user).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("������ �������� ���������� �߰��Ǿ����ϴ�." + _userEmail);
                _logText.text = "������ �������� ���������� �߰��Ǿ����ϴ�." + _userEmail;
                _loadingPanel.SetActive(false);
                _popupText.text = "ȸ�������� �Ϸ�Ǿ����ϴ�.";
            }
            else
            {
                Debug.Log("������ ������ �߰��� �����߽��ϴ�." + task.Exception);
                _logText.text = "������ ������ �߰��� �����߽��ϴ�." + task.Exception;
                _loadingPanel.SetActive(false);
                _popupText.text = "ȸ�����Կ� �����߽��ϴ�. �ٽ� �õ����ּ���.";
            }
        });
    }

    IEnumerator CheckUser()
    {
        _loadingPanel.SetActive(true);
        yield return _db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef;
        yield return docRef = _db.Collection("users").Document(_userEmail);
        yield return docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
                    Dictionary<string, object> user = snapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in user)
                    {
                        Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
                    }
                    string pw = user["userPW"].ToString();
                    if (pw == _userPassword)
                    {
                        Debug.Log("�α��� ����");
                        _logText.text = "�α��� ����";
                        _isExist = true;
                        _gold = int.Parse(user["gold"].ToString());
                        _score = int.Parse(user["score"].ToString());
                        _goldText.text = "Gold: " + _gold;
                        _scoreText.text = "Score: " + _score;
                        _userIdText.text = _userEmail;
                        _mainPanel.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("��й�ȣ�� Ʋ�Ƚ��ϴ�.");
                        _logText.text = "��й�ȣ�� Ʋ�Ƚ��ϴ�.";
                        ShowPopup("��й�ȣ�� Ʋ�Ƚ��ϴ�.");
                    }
                }
                else
                {
                    Debug.Log("�ش� ������ �������� �ʽ��ϴ�.");
                    _logText.text = "�ش� ������ �������� �ʽ��ϴ�.";
                    ShowPopup("�ش� ������ �������� �ʽ��ϴ�.");
                }
            }
            else
            {
                Debug.Log("���� ���� ��ȸ�� �����߽��ϴ�." + task.Exception);
                _logText.text = "���� ���� ��ȸ�� �����߽��ϴ�." + task.Exception;
                ShowPopup("���� ���� ��ȸ�� �����߽��ϴ�. �ٽ� �õ����ּ���.");
            }
            _loadingPanel.SetActive(false);
        });
    }

    IEnumerator UpdateUserData()
    {
        if (_isExist)
        {
            _loadingPanel.SetActive(true);
            yield return _db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef;
            yield return docRef = _db.Collection("users").Document(_userEmail);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "gold", _gold },
                { "score", _score },
                { "updateTime", FieldValue.ServerTimestamp }
            };
            yield return docRef.UpdateAsync(updates).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("������ �������� ���������� ������Ʈ�Ǿ����ϴ�." + _userEmail);
                    _logText.text = "������ �������� ���������� ������Ʈ�Ǿ����ϴ�." + _userEmail;
                    ShowPopup("�������� ���������� ������Ʈ�Ǿ����ϴ�.");
                }
                else
                {
                    Debug.Log("������ ������ ������Ʈ�� �����߽��ϴ�." + task.Exception);
                    _logText.text = "������ ������ ������Ʈ�� �����߽��ϴ�." + task.Exception;
                    ShowPopup("������ ������Ʈ�� �����߽��ϴ�. �ٽ� �õ����ּ���.");
                }
                _loadingPanel.SetActive(false);
            });
        }
        else
        {
            ShowPopup("�α��� �� �̿����ּ���.");


        }
    }

    IEnumerator DeleteUser()
    {
        if (_isExist)
        {
            _loadingPanel.SetActive(true);
            yield return _db = FirebaseFirestore.DefaultInstance;
            DocumentReference docRef;
            yield return docRef = _db.Collection("users").Document(_userEmail);
            yield return docRef.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("������ �������� ���������� �����Ǿ����ϴ�." + _userEmail);
                    _logText.text = "������ �������� ���������� �����Ǿ����ϴ�." + _userEmail;
                    ShowPopup("�������� ���������� �����Ǿ����ϴ�.");
                    _isExist = false;
                    _loadingPanel.SetActive(false);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                else
                {
                    Debug.Log("������ ������ ������ �����߽��ϴ�." + task.Exception);
                    _logText.text = "������ ������ ������ �����߽��ϴ�." + task.Exception;
                    ShowPopup("������ ������ �����߽��ϴ�. �ٽ� �õ����ּ���.");
                }
                _loadingPanel.SetActive(false);
            });
        }
        else
        {
            ShowPopup("�α��� �� �̿����ּ���.");
        }
    }

    IEnumerator LoadRankingData()
    {
        _loadingPanel.SetActive(true);
        yield return _db = FirebaseFirestore.DefaultInstance;
        Query query = _db.Collection("users").OrderByDescending("score").Limit(10);
        yield return query.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;
                int rank = 1;
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Dictionary<string, object> user = document.ToDictionary();

                    // (�α� ����� �ʿ�ø�)
                    foreach (KeyValuePair<string, object> pair in user)
                    {
                        Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
                    }

                    GameObject go = Instantiate(_rankingPrefab, _content.transform);
                    go.transform.GetChild(0).GetComponent<Text>().text = rank.ToString();
                    go.transform.GetChild(1).GetComponent<Text>().text = document.Id;
                    go.transform.GetChild(2).GetComponent<Text>().text = user["score"].ToString();

                    rank++;
                }
            }
            else
            {
                Debug.Log("��ŷ ������ �ε� ����" + task.Exception);
                _logText.text = "��ŷ ������ �ε� ����" + task.Exception;
                ShowPopup("��ŷ ������ �ε忡 �����߽��ϴ�. �ٽ� �õ����ּ���.");
                _rankingPanel.SetActive(false);
            }
            _loadingPanel.SetActive(false);
        });
    }

    void ShowPopup(string message)
    {
        _popupPanel.SetActive(true);
        _popupText.text = message;
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

    IEnumerator TestAddData()
    {
        DocumentReference docRef;
        yield return docRef = _db.Collection("users").Document("alovelace");

        Dictionary<string, object> user = new Dictionary<string, object>
{
    { "First", "Ada" },
    { "Last", "Lovelace" },
    { "Born", 1815 },
};
        docRef.SetAsync(user).ContinueWithOnMainThread(task =>
        {
            Debug.Log("Added data to the alovelace document in the users collection.");
        });
    }
}
