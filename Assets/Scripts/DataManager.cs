using UnityEngine;
using GoogleSheetsToUnity;
using NUnit.Framework;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    [SerializeField] string _sheetId = "1WAozITqI1JwtE5DBDgn_iHNcld34a2XoWszIuvLV_zs";
    [SerializeField] string _sheetName = "MyData";

    public void OnClickData()
    {
        SpreadsheetManager.Read(new GSTU_Search(_sheetId, _sheetName), PrintDatas);
    }

    void PrintDatas(GstuSpreadSheet gstuSpreadSheet)
    {
        Debug.Log(gstuSpreadSheet["1", "Name"].value);
        List<GSTU_Cell> nameList = new();
        nameList = gstuSpreadSheet.columns["Name"];

        foreach (var name in nameList)
        {
            Debug.Log(name.value);
        }
    }

    public void OnClickUpdate()
    {
        List<string> datas = new List<string>()
        {
            "14", "NewMonster", "240", "10", "35", "3"
        };

        SpreadsheetManager.Write(new GSTU_Search(_sheetId, _sheetName, "A15"),
            new ValueRange(datas), UpdateComplete);
    }

    void UpdateComplete()
    {
        Debug.Log("업데이트 완료");
    }

    public void OnClickReadAndUpdate()
    {
        SpreadsheetManager.Read(new GSTU_Search(_sheetId, _sheetName), ReadAndUpdate);
    }

    void ReadAndUpdate(GstuSpreadSheet sheetRef)
    {
        sheetRef["1", "Hp"].UpdateCellValue(_sheetId, _sheetName, "1000", UpdateComplete);
    }
}
