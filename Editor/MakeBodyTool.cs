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

        // ウインドウが開かれたときに実行
        void OnEnable()
        {
            // ペア作成
            pairs = new List<(Transform, bool)> { };
            var selectedTransform = Selection.activeTransform;

            foreach (Transform child in selectedTransform)
            {
                pairs.Add((child, IsExcludedName(child.name)));
            }

        }

        // ウインドウが更新されたときに実行
        void OnGUI()
        {
            GUILayout.Label("オブジェクトを一括で非表示にして\nEditorOnlyにするツールです");
            GUILayout.Space(10);
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
                    pair.transform.gameObject.SetActive(pair.check);
                    pair.transform.gameObject.tag = pair.check ? "Untagged" : "EditorOnly";
                    UnityEditor.EditorUtility.SetDirty(pair.transform);
                }
            }
        }

        // 除外する名前かチェック
        bool IsExcludedName(string name)
        {
            return excludedNames.Contains(name, StringComparer.OrdinalIgnoreCase);
        }
    }
}