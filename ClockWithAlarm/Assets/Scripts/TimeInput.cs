using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using TMPro;
using Assets.Scripts;

using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TimeInput : MonoBehaviour
{
    private TimeController timeController;
    private TMP_InputField tMP_InputField;
    private Regex onlyDigits, minutesReg;
    private CreateAnAlarm alarmButton;
    private TimeConvertions timeConvertions;

    private int caretPos, caretPosBeforeTryingToSelect;
    private string inputSymbol, currentSymbol, inputFieldString, tmpInputFieldString;

    private void Awake()
    {
        tmpInputFieldString = "00:00:00";
    }

    void Start()
    {
        tMP_InputField = gameObject.GetComponent<TMP_InputField>();
        alarmButton = GameObject.Find("Alarm").GetComponent<CreateAnAlarm>();
        timeController = GameObject.Find("TimeController").GetComponent<TimeController>();
        timeConvertions = new TimeConvertions();

        tMP_InputField.text = tmpInputFieldString;
        onlyDigits = new Regex(@"[0-9]", RegexOptions.Compiled);
        minutesReg = new Regex(@"[0-5]", RegexOptions.Compiled);
    }

    void Update()
    {
        UpdateLogic();
    }

    void UpdateLogic()
    {
        if (GameObject.Find("InputFieldText").GetComponent<TMP_Text>().enabled == true)
        {
            caretPosBeforeTryingToSelect = tMP_InputField.caretPosition;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    tMP_InputField.ActivateInputField();
                    tMP_InputField.caretPosition = caretPosBeforeTryingToSelect;
                }
                else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        tMP_InputField.ActivateInputField();
                        tMP_InputField.caretPosition = caretPosBeforeTryingToSelect;
                    }
                }

            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                tMP_InputField.caretPosition = caretPosBeforeTryingToSelect;
            }
            DeleteOrBackspace();
            DigitInput();
            Colons();
        }
        tmpInputFieldString = tMP_InputField.text;
    }

    void Colons()
    {
        string tmpInputString = tMP_InputField.text;
        int length = tmpInputString.Length;
        if (length >= 3)
        {
            if (tmpInputString.Substring(2, 1) != ":")
            {
                tMP_InputField.text = tmpInputString.Remove(2, 1).Insert(2, ":");
            }
        }
        if (length >= 6)
        {
            if (tmpInputString.Substring(5, 1) != ":")
            {
                tMP_InputField.text = tmpInputString.Remove(5, 1).Insert(5, ":");
            }
        }
    }

    void DigitInput()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            caretPos = tMP_InputField.caretPosition;
            inputFieldString = tMP_InputField.text;
            inputSymbol = Input.inputString;

            if (caretPos < 8 && onlyDigits.IsMatch(inputSymbol))
            {
                currentSymbol = inputFieldString.Substring(caretPos, 1);
                if (caretPos < 2)
                {
                    inputFieldString = ReplaceSubstring(inputFieldString, caretPos, inputSymbol);
                    int hours = Convert.ToInt32(inputFieldString.Substring(0, 2));
                    if (hours > 23)
                    {
                        tMP_InputField.text = ReplaceSubstring(inputFieldString, caretPos, "23");
                        MoveCaretFromPosBy(caretPos, 2);
                    }
                    else
                    {
                        tMP_InputField.text = ReplaceSubstring(inputFieldString, caretPos, inputSymbol);
                        MoveCaretFromPosBy(caretPos, 1);
                    }
                } //for two digits for hours
                else if (currentSymbol == ":")
                {
                    if (minutesReg.IsMatch(inputSymbol))
                    {
                        if (inputSymbol != currentSymbol)
                        {
                            tMP_InputField.text = ReplaceSubstring(inputFieldString, caretPos + 1, inputSymbol);
                            MoveCaretFromPosBy(caretPos, 2);
                        }
                    }
                } //if current symbol is colons
                else if (caretPos == 3 || caretPos == 6)
                {
                    if (minutesReg.IsMatch(inputSymbol))
                    {
                        if (inputSymbol != currentSymbol)
                        {
                            tMP_InputField.text = ReplaceSubstring(inputFieldString, caretPos, inputSymbol);
                            MoveCaretFromPosBy(caretPos, 1);
                        }
                        else
                        {
                            MoveCaretFromPosBy(caretPos, 1);
                        }
                    }
                } //for first digits in minutes' number
                else
                {
                    if (inputSymbol != currentSymbol)
                    {
                        tMP_InputField.text = ReplaceSubstring(inputFieldString, caretPos, inputSymbol);
                        MoveCaretFromPosBy(caretPos, 1);
                    }
                    else
                    {
                        MoveCaretFromPosBy(caretPos, 1);
                    }
                }
            }
        }
    }

    void DeleteOrBackspace()
    {
        caretPos = tMP_InputField.caretPosition;

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            int i = caretPos;
            if (i > -1 && i < 8)
            {
                if (i == 1 || i == 4)
                {
                    tmpInputFieldString = ReplaceSubstring(tmpInputFieldString, i, "0");
                    tMP_InputField.text = tmpInputFieldString;
                    MoveCaretFromPosBy(i, 2);
                }
                else if (i == 2 || i == 5)
                {
                    tMP_InputField.text = tmpInputFieldString;
                    MoveCaretFromPosBy(i, 1);
                }
                else
                {
                    tmpInputFieldString = ReplaceSubstring(tmpInputFieldString, i, "0");
                    tMP_InputField.text = tmpInputFieldString;
                    MoveCaretFromPosBy(i, 1);
                }
            }
            tMP_InputField.text = tmpInputFieldString;
        }
        else if (Input.GetKey(KeyCode.Delete))
        {
            tMP_InputField.text = tmpInputFieldString;
            tMP_InputField.caretPosition = caretPos;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (caretPos > -1 && caretPos < 8)
            {
                if (caretPos == 3 || caretPos == 6)
                {
                    tMP_InputField.text = tmpInputFieldString;
                }
                else if (caretPos < 3)
                {
                    tmpInputFieldString = ReplaceSubstring(tmpInputFieldString, caretPos, "0");
                    tMP_InputField.text = tmpInputFieldString;
                }
                else
                {
                    tmpInputFieldString = ReplaceSubstring(tmpInputFieldString, caretPos, "0");
                    tMP_InputField.text = tmpInputFieldString;
                }
            }
        }
        else if (Input.GetKey(KeyCode.Backspace))
        {
            tmpInputFieldString = ReplaceSubstring(tmpInputFieldString, caretPos, "0");
            tMP_InputField.text = tmpInputFieldString;
            tMP_InputField.caretPosition = caretPos;
        }
    }

    private string ReplaceSubstring(string str, int startPos, string newStr)
    {
        return str.Remove(startPos, newStr.Length).Insert(startPos, newStr);
    }

    void MoveCaretFromPosBy(int from, int by)
    {
        tMP_InputField.caretPosition = from + by;
    }

    public void SetAlarmTimeInSeconds()
    {
        if (tMP_InputField.text.Length == 8)
        {
            int time = timeConvertions.StringTimeToSeconds(tMP_InputField.text);
            alarmButton.SetCurrentAlarmTime(time);
            try
            {
                timeController.MoveArrowsFromInputField(tMP_InputField.text);
            }
            catch(Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        
    }
}