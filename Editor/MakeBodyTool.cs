using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace armide.vrchat.makebodytool
{
    public  class ToolWindow : EditorWindow
    {
        // デフォルトで除外するオブジェクト
        List<string> excludedNames = new List<string>() {"Armature", "Body", "Body_base"};

        // タプルのリスト
        List<(Transform transform, bool check)> pairs = null;


        // コンテキストメニュー
        [MenuItem("GameObject/素体作成ツール")]
        public static void MakeBodyTool (MenuCommand command)
        {
            GetWindow<ToolWindow>("素体作成ツール");
        }

        bool IsExcludedName(string name)
        {
            return excludedNames.Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        // ウインドウが開かれたときに実行
        private void OnEnable()
        {
            // ペア作成
            pairs = new List<(Transform, bool)> { };
            var selectedTransform = Selection.activeTransform.Cast<Transform>();

            foreach (var child in selectedTransform)
            {
                if (IsExcludedName(child.name))
                {
                    // デフォルトで除外するオブジェクト
                    pairs.Add((child, true));
                }
                else
                {
                    // それ以外のオブジェクト
                    pairs.Add((child, false));
                }

            }

        }

        // ウインドウが更新されたときに実行
        void OnGUI()
        {
            GUILayout.Label("オブジェクトを一括で非表示にして\nEditorOnlyにするツールです\n");
            GUILayout.Label("残したいオブジェクトを選択してください", EditorStyles.boldLabel);


            // デフォルトで除外するオブジェクト
            foreach (var pair in pairs.Where(x => IsExcludedName(x.transform.name)))
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle(pair.transform.name, pair.check);
                EditorGUI.EndDisabledGroup();
            }

            // それ以外のオブジェクト
            foreach (var pair in pairs.Where(x => !IsExcludedName(x.transform.name)).ToArray())
            {
                bool value = EditorGUILayout.Toggle(pair.transform.name, pair.check);
                if (value != pair.check)
                {
                    var index = pairs.FindIndex(x => x.transform == pair.transform);
                    pairs[index] = (pair.transform, value);

                }
            }

            // 実行ボタン
            if (GUILayout.Button("実行"))
            {
                foreach (var pair in pairs)
                {
                    //Debug.Log(x.transform.name+" : "+x.check.ToString()+" : "+x.transform.tag);
                    pair.transform.gameObject.SetActive(pair.check);
                    pair.transform.gameObject.tag = pair.check ? "Untagged" : "EditorOnly";
                }
            }
        }
    }
}