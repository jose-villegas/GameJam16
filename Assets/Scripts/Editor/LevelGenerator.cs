using System;
using UnityEditor;
using UnityEngine;

class Cell
{
    public enum CellType
    {
        Open,
        Close
    }
    
    public CellType type;
    // top-down-right-left
    public bool[] connected =  new bool[4];
}

class LevelGenerator : EditorWindow {
    // 4-way, 3-way, 2-way-I, 2-way-L
    private GameObject[] blocks = new GameObject[4];
    // top-down-right-left
    private Vector4 _chances;
    private System.Random rand = new System.Random(0);
    private int _seed = 0;
    private Vector2 _translation;
    
    [MenuItem ("Window/Level Generator")]
    public static void  ShowWindow () {
        EditorWindow.GetWindow(typeof(LevelGenerator));
    }
    
    void OnGUI () {
        if(blocks == null || blocks.Length == 0)
        {
            blocks = new GameObject[4];
        }
        
        GUILayout.Label("Blocks");
        
        blocks[0] = EditorGUILayout.ObjectField("Four-way", 
        blocks[0], typeof(GameObject), true, null) as 
        GameObject;
        
        blocks[1] = EditorGUILayout.ObjectField("Three-way", 
        blocks[1], typeof(GameObject), true, null) as 
        GameObject;
        
        blocks[2] = EditorGUILayout.ObjectField("Two-way Tunnel", 
        blocks[2], typeof(GameObject), true, null) as 
        GameObject;
        
        blocks[3] = EditorGUILayout.ObjectField("Two-way L", 
        blocks[3], typeof(GameObject), true, null) as 
        GameObject;
        
        _chances = EditorGUILayout.Vector4Field("Chances", 
        _chances, null);
        
        EditorGUILayout.BeginHorizontal();
        _seed = EditorGUILayout.IntField(_seed);
        
        if(GUILayout.Button("Seed"))
        {
            rand = new System.Random(_seed);
        }
        
        EditorGUILayout.EndHorizontal();
        
        _translation = EditorGUILayout.Vector2Field("Translation",
        _translation, null);
        
        if(GUILayout.Button("Generate Level"))
        {
            _GenerateLevel();
        }
    }

    private void _GenerateLevel()
    {
        var cell = new Cell();
        cell.type = Cell.CellType.Close;
        cell.connected = new bool[4]{true, true, true, true};
        
        var cellRoot = GameObject.Instantiate(blocks[0], 
        Vector3.zero, blocks[0].transform.rotation) as GameObject; 

        for(int i = 0; i < 4; i++)
        {
            var selected = _SelectedCell();
            
            Debug.Log(selected);
            
            if(cell.connected[i] && selected >= 0)
            {
                GameObject.Instantiate(blocks[selected],
                _NextPosition(cellRoot, i),
                blocks[selected].transform.rotation);
            }
        }
    }
    
    private Vector3 _NextPosition(GameObject root, int exit)
    {
        var position  = root.transform.position;
        
        if(exit == 0) //top
        {
            position.x += _translation.x;
        }
        else if(exit == 1) // down
        {
            position.x -= _translation.x;
        }
        else if(exit == 2) // right
        {
            position.z += _translation.y;
        }
        else if(exit == 3) // left
        {
            position.z -= _translation.y;
        }
        
        return position;
    }
    
    private int _SelectedCell()
    {
        var per = _chances;
        
        for(int i = 1; i < 4; i++)
        {
            per[i] += per[i-1];
        }
        
        var r = rand.Next(0, (int)per[3]);
        
        if(r <= per.x) { return 0; }       
        if(r <= per.y) { return 1; }
        if(r <= per.z) { return 2; }
        if(r <= per.w) { return 3; }

        return -1;
    }
}