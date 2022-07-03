using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Assets.Scripts
{
    public class CreateAnAlarm : MonoBehaviour
    {
        private Button thisButton;
        private GameObject secArrow, minArrow, hourArrow;

        private TimeController timeController;
        private TimeConvertions timeConvertions;

        private AudioSource audioSource;
        private TMP_InputField inputField;
        private Image inputFieldImage;
        private TMP_Text inputFieldText;
        private Text alarmLabel;

        private List<GameObject> pacifiers;

        private int seconds, minutes, hours, tempHours, currTimeInSeconds, currentAlarmTime, remainingAlarmTime;
        private bool madeAturn, writeInInputFieldOnStart, isArrowChangingNow;
        private byte dayOrNight;

        void Start()
        {
            thisButton = gameObject.GetComponent<Button>();
            secArrow = GameObject.Find("SecArrow");
            minArrow = GameObject.Find("MinArrow");
            hourArrow = GameObject.Find("HourArrow");

            timeController = GameObject.Find("TimeController").GetComponent<TimeController>();
            timeConvertions = new TimeConvertions();

            audioSource = GameObject.Find("AlarmAudioSource").GetComponent<AudioSource>();
            inputField = GameObject.Find("InputField").GetComponent<TMP_InputField>();
            inputFieldImage = GameObject.Find("InputField").GetComponent<Image>();
            inputFieldText = GameObject.Find("InputFieldText").GetComponent<TMP_Text>();
            alarmLabel = GameObject.Find("AlarmLabel").GetComponent<Text>();

            pacifiers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Pacifier"));

            StartCoroutine("AlarmTimer");
            remainingAlarmTime = -1;
            isArrowChangingNow = false;
            writeInInputFieldOnStart = true;
            if (timeController.GetIsAlarmChanging() == 1)
            {
                thisButton.onClick.AddListener(ShowTime);
            }
        }

        void Update()
        {
            AlarmUIActivity();
        }

        void LateUpdate()
        {
            LateUpdateLogic();
        }
        
        void AlarmUIActivity()
        {
            if (timeController.GetIsAlarmChanging() == 0)
            {
                foreach (GameObject gameObject in pacifiers)
                {
                    gameObject.SetActive(true);
                }
                inputField.interactable = true;
                inputFieldImage.enabled = true;
                inputFieldText.enabled = true;
                alarmLabel.enabled = true;
            }
            else
            {
                foreach (GameObject gameObject in pacifiers)
                {
                    gameObject.SetActive(false);
                }
                inputField.interactable = false;
                inputFieldImage.enabled = false;
                inputFieldText.enabled = false;
                alarmLabel.enabled = false;
            }
        }
        
        void LateUpdateLogic()
        {
            if (timeController.GetIsAlarmChanging() == 0)
            {
                if (writeInInputFieldOnStart)
                {
                    inputField.text = timeConvertions.SecondsToStringTime(currTimeInSeconds);
                    writeInInputFieldOnStart = false;
                }
                if (isArrowChangingNow)
                {
                    currentAlarmTime = ArrowPosToSeconds();
                    inputField.text = timeConvertions.SecondsToStringTime(currentAlarmTime);
                    RemainingTime(currentAlarmTime);
                }
                else
                {
                    RemainingTime(currentAlarmTime);
                }
            }

            if (timeController.GetIsAlarmChanging() == 1)
            {
                writeInInputFieldOnStart = true;
                AlarmControl();
            }
        }

        private int ArrowPosToSeconds()
        {
            currentAlarmTime = 0;
            //Reading seconds, minutes and hours;
            int currentHour = currTimeInSeconds / 3600 % 3600;
            seconds = Convert.ToInt32((360 - secArrow.GetComponent<Rigidbody2D>().transform.rotation.eulerAngles.z) / 6);
            if (seconds == 60)
            {
                seconds = 0;
            }
            minutes = Convert.ToInt32((360 - minArrow.GetComponent<Rigidbody2D>().transform.rotation.eulerAngles.z) / 6);
            if (minutes == 60)
            {
                minutes = 0;
            }
            tempHours = Convert.ToInt32((360 - hourArrow.GetComponent<Rigidbody2D>().transform.rotation.eulerAngles.z) / 30);

            //Logic for getting hours
            // 00:00 - 11:00
            if (!madeAturn)
            {
                if (hours == 24)
                {
                    hours = 0;
                    madeAturn = false;
                }
                else if (tempHours == 12 && hours == 11)
                {
                    madeAturn = true;
                    hours = 12;
                }
                else if (tempHours == 12)
                {
                    hours = 0;
                }
                else if (tempHours == 11 && hours == 0)
                {
                    madeAturn = true;
                }
                else if (tempHours > 0 || tempHours < 12)
                {
                    hours = tempHours;
                }
            }
            //12:00 - 23:00
            if (madeAturn)
            {
                if (hours == 23 && tempHours == 12)
                {
                    madeAturn = false;
                }
                else if (tempHours == 11 && hours == 12)
                {
                    hours = tempHours;
                    madeAturn = false;
                }
                else if (tempHours == 12)
                {
                    hours = 12;
                }
                else if (tempHours > 0 || tempHours < 12)
                {
                    hours = (24 - (12 - tempHours));
                }
                else
                {
                    hours--;
                }
            }
            currentAlarmTime = seconds + minutes * 60 + hours * 3600;
            return currentAlarmTime;
        }

        void RemainingTime(int currentAlarmTime)
        {
            //remainingAlarmTime = -1;
            if (currentAlarmTime == currTimeInSeconds)
            {
                remainingAlarmTime = -1;
            }
            else if (currentAlarmTime > currTimeInSeconds)
            {
                remainingAlarmTime = currentAlarmTime - currTimeInSeconds;
            }
            else if (currentAlarmTime < currTimeInSeconds)
            {
                remainingAlarmTime = 86400 - (currTimeInSeconds - currentAlarmTime);
            }
        }

        void Save()
        {
            timeController.SetIsAlarmChanging(1);
            thisButton.GetComponentInChildren<Text>().text = "Задать будильник";
            thisButton.GetComponent<Button>().onClick.AddListener(ShowTime);
            StopCoroutine("ShowTimeCoroutine");
        }//Save alarm

        void ShowTime()
        {
            timeController.SetIsAlarmChanging(0);
            thisButton.GetComponentInChildren<Text>().text = "Сохранить";
            thisButton.GetComponent<Button>().onClick.AddListener(Save);
            StartCoroutine("ShowTimeCoroutine");
            audioSource.Stop();

            dayOrNight = timeController.GetTimeOfDay();
            madeAturn = false;
            if (dayOrNight == 1)
                madeAturn = true;
            hourArrow.GetComponent<Rigidbody2D>().transform.rotation = Quaternion.Euler(0, 0, -(currTimeInSeconds / 3600 % 3600 * 30));
        }//Create alarm

        IEnumerator ShowTimeCoroutine()
        {
            for (; ; )
            {
                currTimeInSeconds = timeController.GetCurrentTimeInSeconds();
                yield return new WaitForSeconds(0.5f);
            }
        }

        void AlarmControl()
        {
            if (remainingAlarmTime == 0)
            {
                remainingAlarmTime = -1;
                audioSource.PlayOneShot(audioSource.clip);
            }
        }

        IEnumerator AlarmTimer()
        {
            for (; ; )
            {
                if (remainingAlarmTime >= 0)
                {
                    remainingAlarmTime--;
                }
                yield return new WaitForSeconds(1);
            }

        }
        
        //Get-Set methods
        public int GetCurrentAlarmTime()
        {
            return currentAlarmTime;
        }
        public void SetCurrentAlarmTime(int i)
        {
            currentAlarmTime = i;
        }
        public int GetRemainingAlarmTime()
        {
            return remainingAlarmTime;
        }
        public void SetRemainingAlarmTime(int i)
        {
            remainingAlarmTime = i;
        }
        public bool GetArrowChangingNow()
        {
            return isArrowChangingNow;
        }
        public void SetIsArrowChangingNow(bool i)
        {
            isArrowChangingNow = i;
        }
    }
}