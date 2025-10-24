using UnityEngine;
using UnityEditor; // Add this line
using UnityEngine.PostProcessing;

namespace UnityEngine.PostProcessing.Editor
{
    public sealed class MinDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Use fully qualified name to resolve the MinAttribute conflict
            var attribute = (UnityEngine.PostProcessing.MinAttribute)base.attribute;
            
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = Mathf.Max(EditorGUI.IntField(position, label, property.intValue), (int)attribute.min);
            }
            else
            {
                property.floatValue = Mathf.Max(EditorGUI.FloatField(position, label, property.floatValue), attribute.min);
            }
        }
    }
}