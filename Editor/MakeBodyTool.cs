using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using Cysharp.Threading.Tasks.Triggers;

namespace armide6.vrchat.makebodytool
{
    public class ToolWindow : EditorWindow
    {
        struct Data
        {
            public GameObject obj;
            public bool value;
            public bool exclude;
        }

        // デフォルトで除外するオブジェクト
        string[] excludeNames = {"Armature", "Body", "Body_base"};

        Data[] toggleData;

        // コンテキストメニュー
        [MenuItem("GameObject/素体作成ツール")]
        public static void MakeBodyTool (MenuCommand command)
        {
            GetWindow<ToolWindow>("素体作成ツール");
        }

        // ウインドウが開かれたときに実行
        void OnEnable()
        {
            var select = Selection.activeTransform;
            toggleData = new Data[select.childCount];

            foreach (var (o, i) in select.Cast<IEnumerable>()
                .Select((x, i) => (((Transform)x).gameObject, i)))
            {
                bool v = IsExcludeName(o.name);
                toggleData[i] = new Data {
                    obj = o,
                    value = IsExcludeName(o.name),
                    exclude = IsExcludeName(o.name)
                };
            }
        }

        // ウインドウが更新されたときに実行
        void OnGUI()
        {

            GUILayout.Label("オブジェクトを一括で非表示にして\nEditorOnlyにするツールです");
            GUILayout.Space(10);
            GUILayout.Label("残したいオブジェクトを選択してください", EditorStyles.boldLabel);


            // デフォルトで除外するオブジェクト
            EditorGUI.BeginDisabledGroup(true);
            foreach (var d in toggleData.Where(x => x.exclude))
            {
                EditorGUILayout.Toggle(d.obj.name, d.value);
            }
            EditorGUI.EndDisabledGroup();


            // それ以外のオブジェクト
            foreach (ref var d in toggleData.AsSpan())
            {
                if (d.exclude) continue;
                d.value = EditorGUILayout.Toggle(d.obj.name, d.value);
            }

            // 実行ボタン
            if (GUILayout.Button("実行"))
            {
                foreach (var d in toggleData)
                {
                    d.obj.SetActive(d.value);
                    d.obj.tag = d.value ? "Untagged" : "EditorOnly";
                    UnityEditor.EditorUtility.SetDirty(d.obj);
                }
            }
        }

        // 除外する名前かチェック
        bool IsExcludeName(string name)
        {
            return excludeNames.Contains(name, StringComparer.OrdinalIgnoreCase);
        }
    }
}