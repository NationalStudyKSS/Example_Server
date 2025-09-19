using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;

[System.Serializable]
public class ScriptData
{
    public string type;
    public string naration;
    public string scriptA;
    public string scriptB;
}

public class GameBookManager : MonoBehaviour
{
    [SerializeField] string _nodeDataPath;
    [SerializeField] string _scriptDataPath;

    [SerializeField] List<Dictionary<string, object>> _nodeData = new();
    [SerializeField] List<Dictionary<string, object>> _scriptData = new();

    [SerializeField] List<ScriptData> _dataList = new();

    [SerializeField] List<GameObject> _selectList = new();
    Queue<GameObject> _textQueue = new();

    [Header("UI Elements")]
    [SerializeField] Transform _parentCanvas;
    [SerializeField] Text _sceneIDXText;

    [SerializeField] GameObject _scriptOBJPrefab;
    [SerializeField] GameObject _selectOBJPrefab;
    [SerializeField] GameObject _resultOBJPrefab;

    [SerializeField] GameObject _content;

    [Header("GameBook")]
    [SerializeField] int _sceneIDX = 1;

    [SerializeField] GameObject _gameOverPanel;

    public enum SCENE_TYPE
    {
        CONTINUE,
        CHICE,
        ENDING,
    }

    [Header("Scene Type")]
    [SerializeField] SCENE_TYPE _sceneType;

    public enum SCRIPT_TYPE
    {
        TEXT,
        SELECT,
        RESULT,
    }

    [Header("Script Type")]
    [SerializeField] SCRIPT_TYPE _scriptType;

    [SerializeField] GameObject _resultOBJ;

    private void Start()
    {
        DOTween.Init();

        _nodeData = CSVReader.Read(_nodeDataPath);
        Debug.Log(_nodeData.Count);

        _scriptData = CSVReader.Read(_scriptDataPath);
        Debug.Log(_scriptData.Count);

        _parentCanvas = _content.transform;

        ReadCoroutine();
    }

    public void ReadCoroutine()
    {
        _sceneIDXText.text = $"SCENE IDX : {_nodeData[_sceneIDX - 1]["IDX"]}";
        StartCoroutine(ReadScriptDataRoutine());
    }

    IEnumerator ReadScriptDataRoutine()
    {
        yield return StartCoroutine(RemoveSelectListRoutine());

        // ScriptData 리스트를 돌면서
        for (int i=0; i<_scriptData.Count; i++)
        {
            if (!_scriptData[i].ContainsKey("SceneNum"))
            {
                Debug.LogWarning($"Row {i}에 'SceneNum' 키 없음! 현재 키: {string.Join(",", _scriptData[i].Keys)}");
                continue;
            }

            // ScriptData의 SceneNum을 파싱했을때 현재 씬의 IDX와 같다면
            if (int.Parse(_scriptData[i]["SceneNum"].ToString()) == _sceneIDX)
            {
                // ScriptData를 하나 생성하고
                ScriptData data = new();
                // 데이터를 해당 ScripData로부터 파싱해서 넣어준다.
                data.type = _scriptData[i]["Type"].ToString();
                data.naration = _scriptData[i]["Naration"].ToString();
                data.scriptA = _scriptData[i]["ScriptA"].ToString();
                data.scriptB = _scriptData[i]["ScriptB"].ToString();

                // 그리고 리스트에 추가한다.
                _dataList.Add(data);
            }
        }

        int nodeNum = 0;

        for (int i = 0; i < _dataList.Count; i++)
        {
            switch (_dataList[i].type)
            {
                case "TEXT":
                    MakeScript(_dataList[i].naration, nodeNum);
                    MakeScript(_dataList[i].scriptA, nodeNum);
                    MakeScript(_dataList[i].scriptB, nodeNum);
                    break;

                case "SELECT":
                    nodeNum++;
                    string n = "N" + nodeNum;
                    MakeSelect(_dataList[i].naration, int.Parse(_nodeData[_sceneIDX - 1][n].ToString()));
                    MakeSelect(_dataList[i].scriptA, int.Parse(_nodeData[_sceneIDX - 1][n].ToString()));
                    MakeSelect(_dataList[i].scriptB, int.Parse(_nodeData[_sceneIDX - 1][n].ToString()));
                    break;

                case "RESULT":
                    MakeResult(_dataList[i].naration, nodeNum);
                    MakeResult(_dataList[i].scriptA, nodeNum);
                    MakeResult(_dataList[i].scriptB, nodeNum);
                    // 결과가 RESULT 일 때 할일
                    break;

                default:
                    break;
            }
        }
    }

    public void MakeScript(string str, int n)
    {
        if (str == "0")
        {
            return;
        }

        GameObject obj = Instantiate(_scriptOBJPrefab, _parentCanvas);
        obj.transform.localScale = Vector3.one;
        obj.name = "TEXT";
        obj.tag = "TEXT";
        obj.GetComponentInChildren<Text>().text = str;
        _textQueue.Enqueue(obj);
        obj.SetActive(false);
    }

    public void MakeSelect(string str, int n)
    {
        if (str == "0")
        {
            return;
        }

        GameObject obj = Instantiate(_selectOBJPrefab, _parentCanvas);
        obj.transform.localScale = Vector3.one;
        obj.name = "SELECT";
        obj.tag = "SELECT";
        obj.GetComponentInChildren<Text>().text = str;
        obj.GetComponent<SelectButtonOnClick>().Initialize(n, this);
        _selectList.Add(obj);
        obj.SetActive(false);
    }

    public void MakeResult(string str, int n)
    {
        if (str == "0")
        {
            return;
        }

        _resultOBJ = Instantiate(_resultOBJPrefab, _parentCanvas);
        _resultOBJ.transform.localScale = Vector3.one;
        _resultOBJ.name = "RESULT";
        _resultOBJ.tag = "RESULT";
        _resultOBJ.GetComponentInChildren<Text>().text = str;
        _resultOBJ.SetActive(false);
    }

    public void OnClickNextButton()
    {
        if (_textQueue.Count > 0)
        {
            GameObject target = _textQueue.Dequeue();
            target.SetActive(true);
            string targetText = target.GetComponentInChildren<Text>().text;
            target.GetComponentInChildren<Text>().text = "";
            target.GetComponentInChildren<Text>().DOText(targetText, targetText.Length * 0.05f, true).OnComplete(() => Destroy(target,1f));
        }
        else if (_selectList.Count > 0)
        {
            foreach (var item in _selectList)
            {
                item.SetActive(true);
            }
        }
        else
        {
            _resultOBJ.SetActive(true);
        }
    }

    IEnumerator RemoveSelectListRoutine()
    {
        for(int i = 0; i<_selectList.Count; i++)
        {
            Destroy(_selectList[i]);
            _selectList.RemoveAt(i);
        }

        GameObject[] texts;
        yield return texts = GameObject.FindGameObjectsWithTag("TEXT");

        GameObject[] selects;
        yield return selects = GameObject.FindGameObjectsWithTag("SELECT");
        
        for(int i = 0; i < texts.Length; i++)
        {
            Destroy(texts[i]);
        }

        for (int i = 0; i < selects.Length; i++)
        {
            Destroy(selects[i]);
        }

        _selectList.Clear();
        _dataList.Clear();
    }

    public void SetSceneIDX(int n)
    {
        _sceneIDX = n;
    }

    public void LoadGameOver()
    {
        _gameOverPanel.SetActive(true);
    }
}
