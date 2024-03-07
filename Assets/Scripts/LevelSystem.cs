using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdownText;
    private float countdownTime = 60f;
    private bool isCounting = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCountdown();
        }

        if (isCounting)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime <= 0f)
            {
                countdownTime = 0f;
                isCounting = false;
            }

            UpdateCountdownText();
        }
    }

    private void StartCountdown()
    {
        countdownTime = 60f;
        isCounting = true;
    }

    private void UpdateCountdownText()
    {
        if (isCounting)
        {
            countdownText.text = "Time: " + countdownTime.ToString("F0");    
        }
        else
        {
            countdownText.text = ("Press \"L\""); 
        }
        
    }
}