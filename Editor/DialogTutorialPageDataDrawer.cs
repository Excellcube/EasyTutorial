using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Excellcube.EasyTutorial
{
    [CustomPropertyDrawer(typeof(DialogTutorialPageData))]
    public class DialogTutorialPageDataDrawer : PropertyDrawer
    {
        private Rect m_Position;

        private bool m_FoldOutEvents = false;
        private float m_Height = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_Height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_Position = position;

            float startY = m_Position.y;

            EditorGUI.BeginProperty(position, label, property);

            DrawTitleLabel();

            EditorGUI.indentLevel = 1;
            {
                DrawStartDelay(property);
                DrawDialogInfoArea(property);
                DrawEventArea(property);
            }
            EditorGUI.indentLevel = 0;

            EditorGUI.EndProperty();

            float endY = m_Position.y;
            m_Height = endY - startY;
        }

        
        // ==== 세부 구현 ==== //

        private void DrawTitleLabel() {
            m_Position.height = EditorGUIUtility.singleLineHeight;
            m_Position.y += EditorGUIUtility.singleLineHeight * 0.2f;

            // Title 라벨 그리기.
            EditorGUI.LabelField(m_Position, "Dialog Page 정보", EditorStyles.boldLabel);

            m_Position.y += EditorGUIUtility.singleLineHeight * 1.1f;
        }

        private void DrawStartDelay(SerializedProperty property) {
            // StartDelay 영역 그리기.
            var startDelayProp = property.FindPropertyRelative(Field.StartDelay);
            EditorGUI.PropertyField(m_Position, startDelayProp, new GUIContent("시작 딜레이 (초)"));

            m_Position.y += EditorGUIUtility.singleLineHeight;
            m_Position.y += 2.0f;
        }

        private void DrawDialogInfoArea(SerializedProperty property) {
            var leftSpriteProp = property.FindPropertyRelative(Field.LeftSprite);
            var leftSpritePosition = m_Position;
            leftSpriteProp.objectReferenceValue = EditorGUI.ObjectField(leftSpritePosition, "캐릭터 이미지", leftSpriteProp.objectReferenceValue, typeof(Sprite), false);

            m_Position.y += leftSpritePosition.height;
            m_Position.y += EditorGUIUtility.singleLineHeight * 0.1f;


            // Character name, Dialog 영역 그리기.
            if(GlobalContext.useLocalization) {
                var characterNameProp = property.FindPropertyRelative(Field.CharacterNameKey);
                var DialogProp        = property.FindPropertyRelative(Field.DialogKey);

                EditorGUI.PropertyField(m_Position, characterNameProp, new GUIContent("캐릭터 이름 Key 값"));
                m_Position.y += EditorGUIUtility.singleLineHeight * 1.1f;

                EditorGUI.PropertyField(m_Position, DialogProp, new GUIContent("다이얼로그 Key 값"));
            } else {
                var characterNameProp = property.FindPropertyRelative(Field.CharacterName);
                var DialogProp        = property.FindPropertyRelative(Field.Dialog);

                EditorGUI.PropertyField(m_Position, characterNameProp, new GUIContent("캐릭터 이름"));
                m_Position.y += EditorGUIUtility.singleLineHeight * 1.1f;

                m_Position.height *= 3;
                EditorGUI.PropertyField(m_Position, DialogProp, new GUIContent("다이얼로그"));
            }

            m_Position.y += EditorGUIUtility.singleLineHeight * 0.5f;
            m_Position.y += m_Position.height;
        }

        private void DrawEventArea(SerializedProperty property) {
            var eventFoldPositiopn = new Rect(m_Position.x, m_Position.y, 15, EditorGUIUtility.singleLineHeight);
            m_FoldOutEvents = EditorGUI.Foldout(eventFoldPositiopn, m_FoldOutEvents, new GUIContent("페이지 실행 이벤트"));

            if(m_FoldOutEvents) {
                // 이벤트 영역 그리기.
                var eventBeginProp   = property.FindPropertyRelative(Field.OnTutorialBegin);
                var eventInvokedProp = property.FindPropertyRelative(Field.OnTutorialInvoked);
                var eventEndedProp   = property.FindPropertyRelative(Field.OnTutorialEnded);

                m_Position.y += EditorGUIUtility.singleLineHeight * 1.3f;
                EditorGUI.PropertyField(m_Position, eventBeginProp);

                m_Position.y += EditorGUI.GetPropertyHeight(eventBeginProp, true);
                m_Position.y += 3.0f;

                EditorGUI.PropertyField(m_Position, eventInvokedProp);

                m_Position.y += EditorGUI.GetPropertyHeight(eventInvokedProp, true);
                m_Position.y += 3.0f;

                EditorGUI.PropertyField(m_Position, eventEndedProp);

                m_Position.y += EditorGUI.GetPropertyHeight(eventEndedProp, true);
                m_Position.y += 3.0f;
            } else {
                // position y 기반으로 전체 높이를 추정.
                // 하단부의 마진이 거의 없는 관계로 한 줄의 마진 추가.
                m_Position.y += EditorGUIUtility.singleLineHeight * 1.5f;
            }
        }
    }
}