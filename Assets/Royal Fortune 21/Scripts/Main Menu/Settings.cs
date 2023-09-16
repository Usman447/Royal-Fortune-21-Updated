using RoyalFortune21.Audio;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject m_SettingsPanel;
    [SerializeField] Toggle m_MusicToggler;

    [SerializeField] Slider m_MusicSlider;
    [SerializeField] Image m_SliderTargetGraphic;
    [SerializeField] Sprite m_MusicOn;
    [SerializeField] Sprite m_MusicOff;

    float Volume
    {
        get => PlayerPrefs.GetFloat("Volume", 1); 
        set => PlayerPrefs.SetFloat("Volume", value);
    }

    bool isVolumeOn
    {
        get
        {
            return PlayerPrefs.GetString("isVolumeOn", "ON") == "ON" ? true : false;
        }
        set
        {
            string val = value ? "ON" : "OFF";
            PlayerPrefs.SetString("isVolumeOn", val);
        }
    }

    private void Start()
    {
        m_MusicSlider.value = Volume;
        m_MusicToggler.isOn = isVolumeOn;

        ChangeMusicVolume(Volume);
        Mute_UnmuteSound(isVolumeOn);
    }

    public void ChangeMusicVolume(float _value)
    {
        if(AudioManager.instance != null)
        {
            AudioManager.instance.ChangeVolume(_value);
            Volume = _value;
        }
    }

    public void Mute_UnmuteSound(bool _isOn)
    {
        if(_isOn)
        {
            AudioManager.instance.UnMuteSoundFromMainMenu(Volume);
            m_MusicSlider.interactable = true;
            m_SliderTargetGraphic.sprite = m_MusicOn;
        }
        else
        {
            AudioManager.instance.MuteSound();
            m_MusicSlider.interactable = false;
            m_SliderTargetGraphic.sprite = m_MusicOff;
        }
        isVolumeOn = _isOn;
    }

    public void OnClickCloseSettings()
    {
        m_SettingsPanel.SetActive(false);
    }
}
