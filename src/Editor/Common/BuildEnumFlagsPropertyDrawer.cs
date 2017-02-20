using SweetEngine.Build;
using UnityEditor;
using UnityEngine;


namespace SweetEditor.Build
{
    [CustomPropertyDrawer(typeof(BuildEnumFlagsAttribute))]
    public class EnumFlagsPropertyDrawer : PropertyDrawer
    {
        private const float _ROW_HEIGHT = 16;




        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _ROW_HEIGHT * ((BuildEnumFlagsAttribute)attribute).Rows;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int buttonsIntValue = 0;
            int enumLength = property.enumNames.Length;
            int rows = Mathf.Clamp(((BuildEnumFlagsAttribute) attribute).Rows, 1, enumLength);
            int buttonIndex = 0;

            int buttonPerRow = enumLength/rows;
            int remainderButtons = enumLength % rows;

            bool[] buttonPressed = new bool[enumLength];

            EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < rows; i++)
            {
                int buttonsForRow = buttonPerRow;

                if (remainderButtons > 0)
                {
                    buttonsForRow++;
                    remainderButtons--;
                }

                float buttonWidth = (position.width - EditorGUIUtility.labelWidth) / buttonsForRow;

                for (int j = 0; j < buttonsForRow; j++)
                {
                    // Check if the button is/was pressed
                    if ((property.intValue & (1 << buttonIndex)) == 1 << buttonIndex)
                    {
                        buttonPressed[buttonIndex] = true;
                    }

                    Rect buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth + buttonWidth * j, position.y + _ROW_HEIGHT * i, buttonWidth, _ROW_HEIGHT);
                    buttonPressed[buttonIndex] = GUI.Toggle(buttonPos, buttonPressed[buttonIndex], property.enumNames[buttonIndex], "Button");

                    if (buttonPressed[buttonIndex])
                    {
                        buttonsIntValue += 1 << buttonIndex;
                    }

                    buttonIndex++;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = buttonsIntValue;
            }
        }
    }
}