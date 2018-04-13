using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    public Dropdown graphicsQualityDropdown;
    public Dropdown screenResolutionDropdown;

    private List<string> graphicsQualitySettingsList;
    private List<string> screenResolutionSettingsList = new List<string>();

    // Use this for initialization
    void Start()
    {
        graphicsQualitySettingsList = QualitySettings.names.ToList();
        graphicsQualityDropdown.AddOptions(graphicsQualitySettingsList);

        Resolution[] resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            screenResolutionSettingsList.Add(resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + " Hz");
        }

        screenResolutionDropdown.AddOptions(screenResolutionSettingsList);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeImageQuality()
    {
        int selectedIndex = graphicsQualityDropdown.value;
        QualitySettings.SetQualityLevel(selectedIndex);
    }

    public void ChangeScreenResolution()
    {
        int selectedIndex = screenResolutionDropdown.value;
        int width = int.Parse(screenResolutionSettingsList[selectedIndex].Split(' ')[0]);
        int height = int.Parse(screenResolutionSettingsList[selectedIndex].Split(' ')[2]);
        int preferredRefreshRate = int.Parse(screenResolutionSettingsList[selectedIndex].Split(' ')[3]);

        Screen.SetResolution(width, height, true, preferredRefreshRate);
    }
}
