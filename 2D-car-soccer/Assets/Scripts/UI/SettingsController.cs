using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class SettingsController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumePercentText;
    [SerializeField] private Transform keybindContainer;
    [SerializeField] private GameObject keybindItemPrefab;

    [Header("Settings")]
    [SerializeField] private InputActionAsset inputActions;

    private void OnEnable()
    {
        // Load Volume
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateVolumeUI(savedVolume);

        // Load Rebinds
string savedRebinds = PlayerPrefs.GetString("InputRebinds", string.Empty);
        if (!string.IsNullOrEmpty(savedRebinds))
        {
            inputActions.LoadBindingOverridesFromJson(savedRebinds);
        }

        RefreshKeybinds();
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        UpdateVolumeUI(value);
    }

    private void UpdateVolumeUI(float value)
    {
        if (volumePercentText != null)
        {
            volumePercentText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }

    public void RefreshKeybinds()
{
        // Clear existing
        foreach (Transform child in keybindContainer)
        {
            Destroy(child.gameObject);
        }

        // Generate keybind items for gameplay maps (excluding UI)
        foreach (var map in inputActions.actionMaps)
        {
            if (map.name == "UI") continue;

            // Optional: Add a map header
            GameObject header = Instantiate(keybindItemPrefab, keybindContainer);
            var headerTexts = header.GetComponentsInChildren<TMP_Text>();
            headerTexts[0].text = $"--- {map.name.ToUpper()} ---";
            headerTexts[1].text = "";
            header.GetComponentInChildren<Button>().gameObject.SetActive(false);

            foreach (var action in map.actions)
            {
                // Skip if no bindings
                if (action.bindings.Count == 0) continue;

                // Create item for each binding (handling composites)
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];

                    // If it's a composite, we usually want to rebind the parts, not the root
                    if (binding.isComposite) continue;

                    GameObject item = Instantiate(keybindItemPrefab, keybindContainer);
                    var texts = item.GetComponentsInChildren<TMP_Text>();
                    var button = item.GetComponentInChildren<Button>();

                    string label = action.name;
                    if (binding.isPartOfComposite)
                    {
                        label = $"{action.name} - {binding.name}";
                    }

                    texts[0].text = label; // Action Name
                    texts[1].text = action.GetBindingDisplayString(i); // Current Binding

                    int bindingIndex = i;
                    button.onClick.AddListener(() => StartRebinding(action, bindingIndex, texts[1]));
                }
            }
        }
    }

    private void StartRebinding(InputAction action, int bindingIndex, TMP_Text statusText)
    {
        statusText.text = "Press any key...";
        
        action.Disable();

        var rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                statusText.text = action.GetBindingDisplayString(bindingIndex);
                action.Enable();
                operation.Dispose();
                SaveRebinds();
            })
            .OnCancel(operation =>
            {
                statusText.text = action.GetBindingDisplayString(bindingIndex);
                action.Enable();
                operation.Dispose();
            })
            .Start();
    }

    private void SaveRebinds()
    {
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("InputRebinds", rebinds);
        PlayerPrefs.Save();
    }

    public void ResetSettings()
    {
        inputActions.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("InputRebinds");
        RefreshKeybinds();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}
