using UnityEngine;
using UnityEngine.UI;

public class SelectButtonOnClick : MonoBehaviour
{
    [SerializeField] Image _image;

    [SerializeField] int _nodeNum;

    [SerializeField] GameBookManager _gameBookManager;

    public void Initialize(int num, GameBookManager gameBookManager)
    {
        _nodeNum = num;
        _gameBookManager = gameBookManager;

        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonOnClick);
    }

    public void ButtonOnClick()
    {
        _gameBookManager.SetSceneIDX(_nodeNum);
        _image.sprite = Resources.Load<Sprite>($"Background/{_nodeNum}");
        _gameBookManager.ReadCoroutine();
    }
}
