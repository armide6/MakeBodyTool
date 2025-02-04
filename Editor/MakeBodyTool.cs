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
        // �f�t�H���g�ŏ��O����I�u�W�F�N�g
        List<string> excludedNames = new List<string>() {"Armature", "Body", "Body_base"};

        // �^�v���̃��X�g
        List<(Transform transform, bool check)> pairs = null;


        // �R���e�L�X�g���j���[
        [MenuItem("GameObject/�f�̍쐬�c�[��")]
        public static void MakeBodyTool (MenuCommand command)
        {
            GetWindow<ToolWindow>("�f�̍쐬�c�[��");
        }

        // �E�C���h�E���J���ꂽ�Ƃ��Ɏ��s
        void OnEnable()
        {
            // �y�A�쐬
            pairs = new List<(Transform, bool)> { };
            var selectedTransform = Selection.activeTransform;

            foreach (Transform child in selectedTransform)
            {
                pairs.Add((child, IsExcludedName(child.name)));
            }

        }

        // �E�C���h�E���X�V���ꂽ�Ƃ��Ɏ��s
        void OnGUI()
        {
            GUILayout.Label("�I�u�W�F�N�g���ꊇ�Ŕ�\���ɂ���\nEditorOnly�ɂ���c�[���ł�");
            GUILayout.Space(10);
            GUILayout.Label("�c�������I�u�W�F�N�g��I�����Ă�������", EditorStyles.boldLabel);


            // �f�t�H���g�ŏ��O����I�u�W�F�N�g
            foreach (var pair in pairs.Where(x => IsExcludedName(x.transform.name)))
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle(pair.transform.name, pair.check);
                EditorGUI.EndDisabledGroup();
            }

            // ����ȊO�̃I�u�W�F�N�g
            foreach (var pair in pairs.Where(x => !IsExcludedName(x.transform.name)).ToArray())
            {
                bool value = EditorGUILayout.Toggle(pair.transform.name, pair.check);
                if (value != pair.check)
                {
                    var index = pairs.FindIndex(x => x.transform == pair.transform);
                    pairs[index] = (pair.transform, value);

                }
            }

            // ���s�{�^��
            if (GUILayout.Button("���s"))
            {
                foreach (var pair in pairs)
                {
                    pair.transform.gameObject.SetActive(pair.check);
                    pair.transform.gameObject.tag = pair.check ? "Untagged" : "EditorOnly";
                    UnityEditor.EditorUtility.SetDirty(pair.transform);
                }
            }
        }

        // ���O���閼�O���`�F�b�N
        bool IsExcludedName(string name)
        {
            return excludedNames.Contains(name, StringComparer.OrdinalIgnoreCase);
        }
    }
}