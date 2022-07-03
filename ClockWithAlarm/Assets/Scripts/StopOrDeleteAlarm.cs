using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class StopOrDeleteAlarm : MonoBehaviour
    {
        private CreateAnAlarm alarmButton;
        private Button thisButton;
        private AudioSource audioSource;

        private TimeController timeController;
        private TimeConvertions timeConvertions;

        private Text timeBeforeAlarm;

        private byte trigger = 0;

        void Start()
        {
            ShowOrHide(false);
            thisButton = gameObject.GetComponent<Button>();
            alarmButton = GameObject.Find("Alarm").GetComponent<CreateAnAlarm>();
            audioSource = GameObject.Find("AlarmAudioSource").GetComponent<AudioSource>();
            timeController = GameObject.Find("TimeController").GetComponent<TimeController>();
            timeConvertions = new TimeConvertions();
            
            timeBeforeAlarm = GameObject.Find("TimeBeforeAlarm").GetComponent<Text>();
            
        }

        void Update()
        {
            Activation();
            ShowRemainingTime();
        }
        
        void Activation()
        {
            if (alarmButton.GetRemainingAlarmTime() >= 0 && trigger !=0)
            {
                trigger = 0;
                ShowOrHide(true);
                timeBeforeAlarm.enabled = true;
                GetComponentInChildren<Text>().text = "Удалить будильник";
                
            }
            if (alarmButton.GetRemainingAlarmTime() < 0 && audioSource.isPlaying && trigger != 1)
            {
                trigger = 1;
                ShowOrHide(true);
                timeBeforeAlarm.enabled = true;
                GetComponentInChildren<Text>().text = "Остановить будильник";
                
            }
            if (alarmButton.GetRemainingAlarmTime() < 0 && !audioSource.isPlaying && timeController.GetIsAlarmChanging() == 1 && trigger != 2)
            {
                trigger = 2;
                timeBeforeAlarm.enabled = false;
                ShowOrHide(false);
            }
        }

        void ShowRemainingTime()
        {
            if (alarmButton.GetRemainingAlarmTime() >= 0)
            {
                timeBeforeAlarm.text = timeConvertions.SecondsToStringTime(alarmButton.GetRemainingAlarmTime());
            }
            else
            {
                timeBeforeAlarm.text = "00:00:00";
            }
        }
        
        void ShowOrHide(bool isShow)
        {
            GetComponent<Button>().enabled = isShow;
            GetComponent<Image>().enabled = isShow;
            GetComponentInChildren<Text>().enabled = isShow;
        }

        public void ButtonOnClick()
        {
            if (alarmButton.GetRemainingAlarmTime() >= 0)
            {
                alarmButton.SetRemainingAlarmTime(-1);
            }
            if (alarmButton.GetRemainingAlarmTime() < 0 && audioSource.isPlaying)
            {
                alarmButton.SetRemainingAlarmTime(-1);
                audioSource.Stop();
            }
        }
    }
}
