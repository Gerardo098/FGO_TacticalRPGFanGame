using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class Enemy : ScriptableObject
{
    public string enemyName;
    public ItemParameterContainer stats;
    public GameObject model;


    [MenuItem("Assets/Create/Data/Enemy")]
    static void CreateInstance()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "") { path = "Assets"; }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        Enemy enemy = CreateInstance<Enemy>();
        AssetDatabase.CreateAsset(enemy, AssetDatabase.GenerateUniqueAssetPath(path + "/Enemy.asset"));
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(enemy));

        // Creating the parameters
        enemy.stats = CreateInstance<ItemParameterContainer>();
        AssetDatabase.AddObjectToAsset(enemy.stats, enemy);
        enemy.stats.name = "Enemy Parameters";
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(enemy.stats));

        ParameterStructure parameterBase = (ParameterStructure)AssetDatabase.LoadAssetAtPath("Assets/Character/Parameters/Stats_Heroic.asset", typeof(ParameterStructure));
        enemy.stats.Form(parameterBase); 
    }
}
