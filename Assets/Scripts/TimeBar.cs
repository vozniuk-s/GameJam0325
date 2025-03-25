using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    [SerializeField] private float maximumTime = 50.0f;
    [SerializeField] private Text endText;

    private Image timeBarFilling;
    private float currentTime;
    private float second = 0.1f;

    private void Awake()
    {
        timeBarFilling = GetComponent<Image>();
    }

    public void Start()
    {
        timeBarFilling.fillAmount = 1f;
        currentTime = maximumTime;

        StartCoroutine(TimeChanger());
    }

    private IEnumerator TimeChanger()
    {
        while(currentTime > 0.0f)
        {
            ChangeTime();
            yield return new WaitForSeconds(second);
        }
    }

    private void ChangeTime()
    {
        currentTime -= second;

        if (currentTime < 0.0f)
            OnEndTime();
        else
        {
            float currentTimeAsPercantage = currentTime / maximumTime;
            timeBarFilling.fillAmount = currentTimeAsPercantage;
        }
    }

    private void OnEndTime()
    {
        GameData.GameEnded = true;
        Time.timeScale = 0f;
        endText.gameObject.SetActive(true);
    }
}
