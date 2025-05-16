using UnityEditor;
using UnityEngine;

namespace UNSLOW.UnityUtils.Editor.Scripts
{
    /// <summary>
    ///     開発メモ用属性の描画
    /// </summary>
    [CustomPropertyDrawer(typeof(DevelopmentMemoAttribute))]
    public class DevelopmentMemoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // カスタム属性のインスタンスを取得
            var memoAttribute = (DevelopmentMemoAttribute)attribute;

            // 折りたたみ用のヘッダー
            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded,
                label + (property.isExpanded ? "" : ":" + property.stringValue));

            if (property.isExpanded)
            {
                // テキストエリアの描画
                EditorGUI.LabelField(
                    new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width,
                        EditorGUIUtility.singleLineHeight), memoAttribute.Label + ":");
                property.stringValue = EditorGUI.TextArea(
                    new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width,
                        EditorGUIUtility.singleLineHeight * memoAttribute.Lines),
                    property.stringValue
                );
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 折りたたみを反映した高さ
            return EditorGUIUtility.singleLineHeight *
                   (property.isExpanded ? 2 + ((DevelopmentMemoAttribute)attribute).Lines : 1);
        }
    }
}