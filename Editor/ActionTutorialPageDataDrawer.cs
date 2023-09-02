using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Excellcube.EasyTutorial
{
    [CustomPropertyDrawer(typeof(ActionTutorialPageData))]
    public class ActionTutorialPageDataDrawer : PropertyDrawer
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
                DrawActionLog(property);
                DrawTargetInfoArea(property);
                DrawConditionKey(property);
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
            EditorGUI.LabelField(m_Position, "Action Page 정보", EditorStyles.boldLabel);
            m_Position.y += EditorGUIUtility.singleLineHeight * 1.1f;
        }

        private void DrawStartDelay(SerializedProperty property) {
            // StartDelay 영역 그리기.
            var startDelayProp = property.FindPropertyRelative(Field.StartDelay);
            EditorGUI.PropertyField(m_Position, startDelayProp, new GUIContent("시작 딜레이 (초)"));

            m_Position.y += EditorGUIUtility.singleLineHeight;
            m_Position.y += 2.0f;
        }

        private void DrawActionLog(SerializedProperty property) {
            if(GlobalContext.useLocalization) {
                var actionLogProp = property.FindPropertyRelative(Field.ActionLogKey);
                EditorGUI.PropertyField(m_Position, actionLogProp, new GUIContent("액션 메시지 Key 값"));

                m_Position.y += EditorGUIUtility.singleLineHeight;
                m_Position.y += 2.0f;
            } else {
                var actionLogProp = property.FindPropertyRelative(Field.ActionLog);
                var propRect = m_Position;
                propRect.height = EditorGUIUtility.singleLineHeight * 3.0f;
                EditorGUI.PropertyField(propRect, actionLogProp, new GUIContent("액션 메시지"));

                m_Position.y += propRect.height;
                m_Position.y += 2.0f;
            }
        }

        private void DrawTargetInfoArea(SerializedProperty property) {
            var targetRootProp        = property.FindPropertyRelative(Field.DynamicTargetRoot);
            var targetKeyProp         = property.FindPropertyRelative(Field.DynamicTargetKey);
            var indicatorPositionProp = property.FindPropertyRelative(Field.IndicatorPosition);

            EditorGUI.PropertyField(m_Position, targetRootProp, new GUIContent("하이라이트 대상의 Root"));
            m_Position.y += EditorGUI.GetPropertyHeight(targetRootProp, true);
            m_Position.y += 2.0f;

            EditorGUI.PropertyField(m_Position, targetKeyProp, new GUIContent("하이라이트 대상의 Key"));
            m_Position.y += EditorGUI.GetPropertyHeight(targetKeyProp, true);
            m_Position.y += 2.0f;

            EditorGUI.PropertyField(m_Position, indicatorPositionProp, new GUIContent("화살표 위치"));
            m_Position.y += EditorGUI.GetPropertyHeight(indicatorPositionProp, true);
            m_Position.y += 2.0f;
        }

        private void DrawConditionKey(SerializedProperty property) {
            var conditionKeyProp = property.FindPropertyRelative(Field.ConditionKey);

            EditorGUI.PropertyField(m_Position, conditionKeyProp, new GUIContent("페이지 완료 조건"));
            m_Position.y += EditorGUI.GetPropertyHeight(conditionKeyProp, true);
            m_Position.y += 10.0f;
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