using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumePercentText;
    [SerializeField] private Transform keybindContainer;
    [SerializeField] private GameObject keybindItemPrefab;

    [Header("Settings")]
    [Tooltip("Only the Input Action assets actually used by gameplay (e.g. PlayerAction1, PlayerAction2). " +
             "Only these are shown in the keybind list.")]
    [SerializeField] private InputActionAsset[] inputActionAssets;

    private void OnEnable()
    {
        // Load Volume
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateVolumeUI(savedVolume);

        // Load Rebinds
        LoadRebinds();

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
        if (keybindContainer == null || keybindItemPrefab == null) return;

        // Clear existing
        foreach (Transform child in keybindContainer)
        {
            Destroy(child.gameObject);
        }

        if (inputActionAssets == null) return;

        // Only show keybinds from the input assets actually used by gameplay.
        foreach (var asset in inputActionAssets)
        {
            if (asset == null) continue;

            foreach (var map in asset.actionMaps)
            {
                // Never show the auto-generated UI navigation map.
                if (map.name == "UI") continue;

                bool headerAdded = false;

                foreach (var action in map.actions)
                {
                    if (action.bindings.Count == 0) continue;

                    for (int i = 0; i < action.bindings.Count; i++)
                    {
                        var binding = action.bindings[i];

                        // Skip composite roots (rebind the parts, not the container).
                        if (binding.isComposite) continue;

                        // Skip bindings with no key assigned - these aren't actually used.
                        if (string.IsNullOrEmpty(binding.effectivePath)) continue;

                        // Add the map header lazily, only if it has real bindings to show.
                        if (!headerAdded)
                        {
                            GameObject header = Instantiate(keybindItemPrefab, keybindContainer);
                            var headerTexts = header.GetComponentsInChildren<TMP_Text>();
                            if (headerTexts.Length > 0) headerTexts[0].text = $"--- {map.name.ToUpper()} ---";
                            if (headerTexts.Length > 1) headerTexts[1].text = "";
                            var headerButton = header.GetComponentInChildren<Button>();
                            if (headerButton != null) headerButton.gameObject.SetActive(false);
                            headerAdded = true;
                        }

                        GameObject item = Instantiate(keybindItemPrefab, keybindContainer);
                        var texts = item.GetComponentsInChildren<TMP_Text>();
                        var button = item.GetComponentInChildren<Button>();

                        string label = action.name;
                        if (binding.isPartOfComposite)
                        {
                            label = $"{action.name} - {binding.name}";
                        }

                        if (texts.Length > 0) texts[0].text = label; // Action Name
                        TMP_Text statusText = texts.Length > 1 ? texts[1] : null;
                        if (statusText != null) statusText.text = action.GetBindingDisplayString(i); // Current Binding

                        int bindingIndex = i;
                        InputAction capturedAction = action;
                        if (button != null)
                        {
                            button.onClick.AddListener(() => StartRebinding(capturedAction, bindingIndex, statusText));
                        }
                    }
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

    private void LoadRebinds()
    {
        if (inputActionAssets == null) return;

        foreach (var asset in inputActionAssets)
        {
            if (asset == null) continue;
            string saved = PlayerPrefs.GetString(RebindKey(asset), string.Empty);
            if (!string.IsNullOrEmpty(saved))
            {
                asset.LoadBindingOverridesFromJson(saved);
            }
        }
    }

    private void SaveRebinds()
    {
        if (inputActionAssets == null) return;

        foreach (var asset in inputActionAssets)
        {
            if (asset == null) continue;
            PlayerPrefs.SetString(RebindKey(asset), asset.SaveBindingOverridesAsJson());
        }
        PlayerPrefs.Save();
    }

    private static string RebindKey(InputActionAsset asset)
    {
        return "InputRebinds_" + asset.name;
    }

    public void ResetSettings()
    {
        if (inputActionAssets != null)
        {
            foreach (var asset in inputActionAssets)
            {
                if (asset == null) continue;
                asset.RemoveAllBindingOverrides();
                PlayerPrefs.DeleteKey(RebindKey(asset));
            }
        }
        RefreshKeybinds();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}
