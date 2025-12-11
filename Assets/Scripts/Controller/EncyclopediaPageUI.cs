using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncyclopediaPageUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image CropIcon;
    public TextMeshProUGUI CropName;
    public TextMeshProUGUI Description;
    public GameObject LockedOverlay; // Optional: to hide content if locked
    public void Setup(CropConfig config, bool isUnlocked, bool isLeftPage)
    {
        if (config == null) return;

        if (isUnlocked)
        {
            if (LockedOverlay != null) LockedOverlay.SetActive(false);

            if (isLeftPage)
            {
                // Left Page: Icon and Name
                if (CropIcon != null)
                {
                    CropIcon.gameObject.SetActive(true);
                    CropIcon.sprite = config.Icon;
                }
                if (CropName != null)
                {
                    CropName.gameObject.SetActive(true);
                    CropName.text = config.CropName;
                }
                if (Description != null) Description.gameObject.SetActive(false);
            }
            else
            {
                // Right Page: Description
                if (CropIcon != null) CropIcon.gameObject.SetActive(false);
                if (CropName != null) CropName.gameObject.SetActive(false);
                if (Description != null)
                {
                    Description.gameObject.SetActive(true);
                    Description.text = string.IsNullOrEmpty(config.KnowledgeDescription)
                        ? "No description available."
                        : config.KnowledgeDescription;
                }
            }
        }
        else
        {
            // Locked State
            if (LockedOverlay != null)
            {
                LockedOverlay.SetActive(true);
            }
            else
            {
                // Fallback if no overlay
                if (CropIcon != null)
                {
                    CropIcon.gameObject.SetActive(true);
                    CropIcon.color = Color.black; // Silhouette
                    CropIcon.sprite = config.Icon;
                }
                if (CropName != null)
                {
                    CropName.gameObject.SetActive(true);
                    CropName.text = "???";
                }
                if (Description != null) Description.gameObject.SetActive(false);
            }
        }
    }
}
