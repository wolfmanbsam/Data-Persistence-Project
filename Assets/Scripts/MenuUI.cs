using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class MenuUI : MonoBehaviour
{
    public InputField inputField;
    public static string text;
    public void StartNew()
    {
        InputField inputField = GetComponentInChildren<InputField>();
        text = inputField.text;
        SceneManager.LoadScene(1);
    }
}
