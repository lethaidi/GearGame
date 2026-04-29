using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsShared : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider bgmSlider, sfxSlider;

    void OnEnable() // Gọi mỗi khi UI được bật
    {
        LoadValuesToUI();
        ApplyBGMVolume(); // Đảm bảo AudioMixer cũng nhận lại giá trị
        ApplySFXVolume();
    }

    public void LoadValuesToUI()
    {
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;
    }

    public void ApplyBGMVolume()
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Max(bgmSlider.value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefs.Save();
    }

    public void ApplySFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(sfxSlider.value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }
}
