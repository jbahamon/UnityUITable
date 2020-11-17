using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json;

public class TableLoader : MonoBehaviour
{
    public Text title;
    public GameObject table;
    public Text headerCell;
    public Text tableCell;

    private bool pendingUpdate = false;

    private FileSystemWatcher watcher;

    void Start()
    {
        UpdateTable();
        watcher = new FileSystemWatcher();
        watcher.Path = Application.streamingAssetsPath;
        watcher.Filter = "JsonChallenge.json";
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += OnSourceFileChanged;
        watcher.EnableRaisingEvents = true;
    }

    void Update()
    {
        if (pendingUpdate)
        {
            pendingUpdate = false;
            UpdateTable();
        }
    }

    public void UpdateTable()
    {
        RemoveAllCells();

        string jsonString = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "JsonChallenge.json"));

        try
        {

            TableData parsedTable = TableData.FromJson(jsonString);

            title.text = parsedTable.title;

            table.GetComponent<GridLayoutGroup>().constraintCount = parsedTable.columnHeaders.Length;

            foreach (string columnName in parsedTable.columnHeaders)
            {
                Text newHeader = Instantiate(headerCell, this.table.transform);
                newHeader.text = columnName;
            }


            foreach (Dictionary<string, string> row in parsedTable.data)
            {
                foreach (string columnName in parsedTable.columnHeaders)
                {
                    Text newCell = Instantiate(tableCell, table.transform);
                    newCell.text = row[columnName];
                }
            }
        }
        catch (System.Exception e)
        {
            title.text = e.Message;
        }
    }

    private void OnSourceFileChanged(object source, FileSystemEventArgs e)
    {
        this.pendingUpdate = true;
    }

    private void RemoveAllCells()
    {
        List<GameObject> children = new List<GameObject>(table.transform.childCount);
        foreach (Transform child in table.transform)
        {
            children.Add(child.gameObject);
        }

        foreach(GameObject child in children)
        {
            Destroy(child);
        }
    }
}

[System.Serializable]
public class TableData
{
    public string title;
    public string[] columnHeaders;

    public List<Dictionary<string, string>> data; 

    public static TableData FromJson(string jsonString)
    {
        return JsonConvert.DeserializeObject<TableData>(jsonString);
    }
}