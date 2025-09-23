using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomTimer
{
    public float durationSecond = 0;
    public DateTime date = DateTime.Now;
    public bool isEnded = false;
    public UnityEvent<string> onTimeElapsed = new UnityEvent<string>();
    public UnityEvent onEnded = new UnityEvent();
}

public class TimerInterface
{
    private const string KeySave = "Timer-Save";

    public enum Format
    {
        MinutesSeconds,
        HoursMinutesSeconds,
        HoursMinutes,
        DaysHoursMinutesSeconds,
        DaysHourMinutes
    }

    private static Dictionary<string, CustomTimer> _dictCustomTimer = new Dictionary<string, CustomTimer>();
    
    public static void Init()
    {
        if (SaveDataJsonInterface.Exist<Dictionary<string, CustomTimer>>(KeySave))
        {
            _dictCustomTimer = SaveDataJsonInterface.GetObject<Dictionary<string, CustomTimer>>(KeySave);
        }

        UpdateTimer();
    }

    public static bool AddTimer(string timerKey, float durationSecond, out CustomTimer timer)
    {
        bool add = false;
        timer = new CustomTimer();
        timer.date = DateTime.Now;
        timer.durationSecond = durationSecond;
        if (_dictCustomTimer.TryAdd(timerKey, timer))
        {
            add = true;
        }

        SetSave();
        return add;
    }

    public static bool ResetTimer(string timerKey, float durationSecond, out CustomTimer timer)
    {
        bool add = false;
        timer = null;
        if (TimerExist(timerKey))
        {
            add = true;
            _dictCustomTimer[timerKey].date = DateTime.Now;
            _dictCustomTimer[timerKey].isEnded = false;
            _dictCustomTimer[timerKey].durationSecond = durationSecond;
            timer = _dictCustomTimer[timerKey];
        }

        SetSave();
        return add;
    }
    public static bool CancelTimer(string timerKey)
    {
        bool add = false;
        if (TimerExist(timerKey))
        {
            add = true;
            _dictCustomTimer[timerKey].isEnded = true;
        }

        SetSave();
        return add;
    }

    public static bool RemoveTimer(string timerKey)
    {
        bool ok = _dictCustomTimer.Remove(timerKey);
        SetSave();
        return ok;
    }

    public static bool TimerExist(string timerKey) => _dictCustomTimer.ContainsKey(timerKey);

    public static bool GetTimer(string timerKey, out CustomTimer timer)
    {
        if (TimerExist(timerKey))
        {
            timer = _dictCustomTimer[timerKey];
            return true;
        }
        else
        {
            timer = null;
            return false;
        }
    }

    private static void SetSave() =>
        SaveDataJsonInterface.SetObject<Dictionary<string, CustomTimer>>(KeySave, _dictCustomTimer);

    public static void UpdateTimer()
    {
        foreach (KeyValuePair<string, CustomTimer> valuePair in _dictCustomTimer)
        {
            if (valuePair.Value.isEnded) continue;

            TimeSpan timeLeft = (DateTime.Now - valuePair.Value.date);
            double leftTime = valuePair.Value.durationSecond - timeLeft.TotalSeconds;
            valuePair.Value.onTimeElapsed?.Invoke(GetFormatTime(leftTime, Format.HoursMinutesSeconds, true, true));

            if (leftTime <= 0)
            {
                valuePair.Value.isEnded = true;
                valuePair.Value.onEnded?.Invoke();
            }
        }
    }

    public static string GetFormatTime(double valueTime, Format format, bool displayUnits, bool convertAuto = true)
    {
        TimeSpan t = TimeSpan.FromSeconds(valueTime);
        string result = "";

        if (convertAuto)
            switch (valueTime)
            {
                case <= 3600: // 1h
                    format = Format.MinutesSeconds;
                    break;

                case <= 86400: //1j
                    format = Format.HoursMinutesSeconds;
                    break;

                case > 86400: //1J
                    format = Format.DaysHourMinutes;
                    break;
            }

        switch (format)
        {
            case Format.MinutesSeconds:
                result += displayUnits ? "{0:D2}m" : "{0:D2}:";
                result += displayUnits ? "{1:D2}s" : "{1:D2}";

                return string.Format(result, (int)t.TotalMinutes, t.Seconds);

            case Format.HoursMinutesSeconds:
                result += displayUnits ? "{0:D2}h" : "{0:D2}:";
                result += displayUnits ? "{1:D2}m" : "{1:D2}:";
                result += displayUnits ? "{2:D2}s" : "{2:D2}";

                return string.Format(result, (int)t.TotalHours, t.Minutes, t.Seconds);

            case Format.HoursMinutes:
                result += displayUnits ? "{0:D2}h" : "{0:D2}:";
                result += displayUnits ? "{1:D2}m" : "{1:D2}:";

                return string.Format(result, (int)t.TotalHours, t.Minutes);

            case Format.DaysHoursMinutesSeconds:
                result += displayUnits ? "{0:D2}d" : "{0:D2}:";
                result += displayUnits ? "{1:D2}h" : "{1:D2}:";
                result += displayUnits ? "{2:D2}m" : "{2:D2}:";
                result += displayUnits ? "{3:D2}s" : "{3:D2}";

                return string.Format(result, (int)t.TotalDays, (int)t.Hours, t.Minutes, t.Seconds);

            case Format.DaysHourMinutes:
                result += displayUnits ? "{0:D2}d" : "{0:D2}:";
                result += displayUnits ? "{1:D2}h" : "{1:D2}:";
                result += displayUnits ? "{2:D2}m" : "{2:D2}";
                //result += displayUnits ? "{3:D2}s" : "{3:D2}";

                return string.Format(result, (int)t.TotalDays, (int)t.Hours, t.Minutes);
        }

        return result;
    }
}