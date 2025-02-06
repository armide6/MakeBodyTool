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

        // �f�t�H���g�ŏ��O����I�u�W�F�N�g
        string[] excludeNames = {"Armature", "Body", "Body_base"};

        Data[] toggleData;

        // �R���e�L�X�g���j���[
        [MenuItem("GameObject/�f�̍쐬�c�[��")]
        public static void MakeBodyTool (MenuCommand command)
        {
            GetWindow<ToolWindow>("�f�̍쐬�c�[��");
        }

        // �E�C���h�E���J���ꂽ�Ƃ��Ɏ��s
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

        // �E�C���h�E���X�V���ꂽ�Ƃ��Ɏ��s
        void OnGUI()
        {

            GUILayout.Label("�I�u�W�F�N�g���ꊇ�Ŕ�\���ɂ���\nEditorOnly�ɂ���c�[���ł�");
            GUILayout.Space(10);
            GUILayout.Label("�c�������I�u�W�F�N�g��I�����Ă�������", EditorStyles.boldLabel);


            // �f�t�H���g�ŏ��O����I�u�W�F�N�g
            EditorGUI.BeginDisabledGroup(true);
            foreach (var d in toggleData.Where(x => x.exclude))
            {
                EditorGUILayout.Toggle(d.obj.name, d.value);
            }
            EditorGUI.EndDisabledGroup();


            // ����ȊO�̃I�u�W�F�N�g
            foreach (ref var d in toggleData.AsSpan())
            {
                if (d.exclude) continue;
                d.value = EditorGUILayout.Toggle(d.obj.name, d.value);
            }

            // ���s�{�^��
            if (GUILayout.Button("���s"))
            {
                foreach (var d in toggleData)
                {
                    d.obj.SetActive(d.value);
                    d.obj.tag = d.value ? "Untagged" : "EditorOnly";
                    UnityEditor.EditorUtility.SetDirty(d.obj);
                }
            }
        }

        // ���O���閼�O���`�F�b�N
        bool IsExcludeName(string name)
        {
            return excludeNames.Contains(name, StringComparer.OrdinalIgnoreCase);
        }
    }
}