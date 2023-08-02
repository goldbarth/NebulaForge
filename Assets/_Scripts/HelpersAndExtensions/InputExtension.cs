using TMPro;

namespace HelpersAndExtensions
{
    public static class InputExtension
    {
        public static void Clear(this TMP_InputField inputField)
        {
            inputField.Select();
            inputField.text = "0.0";
        }
    }
}