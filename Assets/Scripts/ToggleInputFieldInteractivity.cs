using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ToggleInputFieldInteractivity : MonoBehaviour
{
    public TMP_InputField inputField;
    public Toggle toggle;

    private void Start()
    {
        // Add a listener to the toggle's onValueChanged event
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    // This method is called when the Toggle's value changes
    private void OnToggleValueChanged(bool isOn)
    {
        // Set the interactable property of the input field based on the Toggle's state
        inputField.interactable = !isOn;
    }
}