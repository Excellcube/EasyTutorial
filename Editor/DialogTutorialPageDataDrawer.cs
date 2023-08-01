using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Excellcube.EasyTutorial.Page
{
    [CustomPropertyDrawer(typeof(DialogTutorialPageData))]
    public class DialogTutorialPageDataDrawer : PropertyDrawer
    {
        private bool m_FoldOutEvents = false;
        private float m_Height = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_Height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float startY = position.y;

            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;
            position.y += EditorGUIUtility.singleLineHeight * 0.2f;

            // Title 라벨 그리기.
            EditorGUI.LabelField(position, "Dialog Page 정보", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 1;

            position.y += EditorGUIUtility.singleLineHeight * 1.1f;


            // StartDelay 영역 그리기.
            var startDelayProp = property.FindPropertyRelative(Field.StartDelay);
            EditorGUI.PropertyField(position, startDelayProp, new GUIContent("시작 딜레이 (초)"));

            position.y += EditorGUIUtility.singleLineHeight * 1.07f;


            var leftSpriteProp = property.FindPropertyRelative(Field.LeftSprite);
            var leftSpritePosition = position;
            leftSpriteProp.objectReferenceValue = EditorGUI.ObjectField(leftSpritePosition, "캐릭터 이미지", leftSpriteProp.objectReferenceValue, typeof(Sprite), false);

            position.y += leftSpritePosition.height;
            position.y += EditorGUIUtility.singleLineHeight * 0.1f;


            // Character name, Dialog 영역 그리기.
            if(GlobalContext.useLocalization) {
                var characterNameProp = property.FindPropertyRelative(Field.CharacterNameKey);
                var DialogProp        = property.FindPropertyRelative(Field.DialogKey);

                EditorGUI.PropertyField(position, characterNameProp, new GUIContent("캐릭터 이름 Key 값"));
                position.y += EditorGUIUtility.singleLineHeight * 1.1f;

                EditorGUI.PropertyField(position, DialogProp, new GUIContent("다이얼로그 Key 값"));
            } else {
                var characterNameProp = property.FindPropertyRelative(Field.CharacterName);
                var DialogProp        = property.FindPropertyRelative(Field.Dialog);

                EditorGUI.PropertyField(position, characterNameProp, new GUIContent("캐릭터 이름"));
                position.y += EditorGUIUtility.singleLineHeight * 1.1f;

                position.height *= 3;
                EditorGUI.PropertyField(position, DialogProp, new GUIContent("다이얼로그"));
            }
            
            position.y += EditorGUIUtility.singleLineHeight * 0.5f;
            position.y += position.height;

            var eventFoldPositiopn = new Rect(position.x, position.y, 15, EditorGUIUtility.singleLineHeight);
            m_FoldOutEvents = EditorGUI.Foldout(eventFoldPositiopn, m_FoldOutEvents, new GUIContent("페이지 실행 이벤트"));

            if(m_FoldOutEvents) {
                // 이벤트 영역 그리기.
                var eventBeginProp   = property.FindPropertyRelative(Field.OnTutorialBegin);
                var eventInvokedProp = property.FindPropertyRelative(Field.OnTutorialInvoked);
                var eventEndedProp   = property.FindPropertyRelative(Field.OnTutorialEnded);

                position.y += EditorGUIUtility.singleLineHeight * 1.3f;
                EditorGUI.PropertyField(position, eventBeginProp);

                position.y += EditorGUI.GetPropertyHeight(eventBeginProp, true);
                position.y += 3.0f;

                EditorGUI.PropertyField(position, eventInvokedProp);

                position.y += EditorGUI.GetPropertyHeight(eventInvokedProp, true);
                position.y += 3.0f;

                EditorGUI.PropertyField(position, eventEndedProp);

                position.y += EditorGUI.GetPropertyHeight(eventEndedProp, true);
                position.y += 3.0f;
            } else {
                // position y 기반으로 전체 높이를 추정.
                // 하단부의 마진이 거의 없는 관계로 한 줄의 마진 추가.
                position.y += EditorGUIUtility.singleLineHeight * 1.5f;
            }

            EditorGUI.indentLevel = 0;

            EditorGUI.EndProperty();

            float endY = position.y;
            m_Height = endY - startY;
        }
    }
}