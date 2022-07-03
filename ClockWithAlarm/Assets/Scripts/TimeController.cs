using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Assets.Scripts;

public class TimeController : MonoBehaviour
{
    private GameObject secArrow, minArrow, hourArrow;
    private ReadNetTime readNetTime;
    private TimeConvertions timeConvertions;
    private Text timeText;

    private byte dayOrNight, isAlarmChanging;
    private int currentTimeInSeconds, lastHour;

    void Awake()
    {
        isAlarmChanging = 1;
    }

    void Start()
    {
        Application.runInBackground = true;

        timeText = GameObject.FindGameObjectWithTag("TimeText").GetComponent<Text>();
        secArrow = GameObject.Find("ClockParts/SecArrow");
        minArrow = GameObject.Find("ClockParts/MinArrow");
        hourArrow = GameObject.Find("ClockParts/HourArrow");
        readNetTime = new ReadNetTime();
        timeConvertions = new TimeConvertions();

        //Read time from ntp-server and start coroutine for increase seconds to currentTimeInSeconds variable
        currentTimeInSeconds = timeConvertions.DateTimeToSeconds(readNetTime.GetTime());
        StartCoroutine(Timer());
        
        lastHour = currentTimeInSeconds / 3600 % 3600;

        //Show time
        timeText.text = timeConvertions.SecondsToDateTime(currentTimeInSeconds).ToLongTimeString();
        SetTimeOfDay(currentTimeInSeconds);
    }

    private void FixedUpdate()
    {
        FixedUpdateLogic();
    }

    void FixedUpdateLogic()
    {
        //Clarification time every hour
        ReadNetTime();
        //Zeroing day seconds
        DaySecondTimer();
        //Arrow movement and time showing
        MoveArrows(currentTimeInSeconds);

        timeText.text = timeConvertions.SecondsToDateTime(currentTimeInSeconds).ToLongTimeString();
        SetTimeOfDay(currentTimeInSeconds);
    }

    private void MoveArrows(int seconds)
    {
        if (isAlarmChanging == 1)
        {
            secArrow.GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Euler(0, 0, -(seconds % 60) * 6f);
            minArrow.GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Euler(0, 0, -(seconds / 60 % 60) * 6f);
            //Logic for moving hour arrow by 12min-segments
            float hourDegrees = (seconds / 3600 % 3600) * 30f;
            if (seconds / 60 % 60 >= 12)
            {
                hourDegrees += ((seconds / 60 % 60) / 12) * 6;
            }
            hourArrow.GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Euler(0, 0, -hourDegrees);
        }
    }

    public void MoveArrowsFromInputField(string strSeconds)
    {
        int seconds = timeConvertions.StringTimeToSeconds(strSeconds);
        if (isAlarmChanging == 0)
        {
            secArrow.GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Euler(0, 0, -(seconds % 60) * 6f);
            minArrow.GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Euler(0, 0, -(seconds / 60 % 60) * 6f);
            hourArrow.GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Euler(0, 0, -(seconds / 3600 % 3600) * 30f);
        }
    }

    void ReadNetTime()
    {
        if (currentTimeInSeconds / 3600 % 3600 > lastHour || currentTimeInSeconds == 86400)
        {
            lastHour = currentTimeInSeconds / 3600 % 3600;
            currentTimeInSeconds = timeConvertions.DateTimeToSeconds(readNetTime.GetTime());
        }
    }

    void DaySecondTimer()
    {
        if (currentTimeInSeconds >= 86400)
        {
            currentTimeInSeconds = 0;
        }
    }

    void SetTimeOfDay(int seconds) //AM or PM
    {
        if (seconds >= 0 && seconds <= 43200)
        {
            dayOrNight = 0;
            timeText.text += " AM";
        }
        else
        {
            dayOrNight = 1;
            timeText.text += " PM";
        }
    }

    IEnumerator Timer()
    {
        for (; ; )
        {
            currentTimeInSeconds++;
            //Debug.Log("Current seconds: " + currentTimeInSeconds);
            yield return new WaitForSeconds(1);
        }
    }

    //Get-Set methods
    public int GetCurrentTimeInSeconds()
    {
        return currentTimeInSeconds;
    }
    public byte GetTimeOfDay()
    {
        return dayOrNight;
    }
    public byte SetIsAlarmChanging(byte i)
    {
        return isAlarmChanging = i;
    }
    public byte GetIsAlarmChanging()
    {
        return isAlarmChanging;
    }
}