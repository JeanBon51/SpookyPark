using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using SRDebugger;
using SRDebugger.Internal;
using SRDebugger.Services;
using SRF.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupBugReporter : BasePopup
{
    [SerializeField] private TMP_InputField _tInputFieldEmail;
    [SerializeField] private TMP_InputField _tInputFieldMessage;
    [SerializeField] private TextMeshProUGUI _emailText;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _submit;
    [SerializeField] private GameObject _sending;
    [SerializeField] private Slider _sliderSending;
    [SerializeField] private TextMeshProUGUI _textSlider;
    private Action _takingScreenshot;
    private Action _screenshotComplete;
    private Coroutine _coroutineSend;

    public override void Init(PopupContainer popupContainer)
    {
        base.Init(popupContainer);
       //this._button.onClick.AddListener(SendReport);
       //this._tInputFieldEmail.text = "";
       //this._tInputFieldMessage.text = "";
       //this._emailText.text = "";
       //this._messageText.text = "";
       //
       //this._submit.SetActive(true);
       //this._sending.SetActive(false);
       //this._sliderSending.minValue = 0;
       //this._sliderSending.maxValue = 1;
    }

    public override void Show()
    {
        //this._tInputFieldEmail.text = "";
        //this._tInputFieldMessage.text = "";
        //this._emailText.text = "";
        //this._messageText.text = "";
        //this._submit.SetActive(true);
        //this._sending.SetActive(false);
        //base.Show();
    }

    [Button]
    public void SendReport()
    {
        if(this._coroutineSend != null)return;
        this._coroutineSend = this._popupContainer.StartCoroutine(SubmitCo());
    }
    
    private IEnumerator SubmitCo()
    {
        this._submit.SetActive(false);
        this._sending.SetActive(true);
        this._popupContainer.EnablePopup(false,this.type);
        if (BugReportScreenshotUtil.ScreenshotData == null && Settings.Instance.EnableBugReportScreenshot)
        {
            if (_takingScreenshot != null)
            {
                _takingScreenshot();
            }

            yield return new WaitForEndOfFrame();

            yield return this._popupContainer.StartCoroutine(BugReportScreenshotUtil.ScreenshotCaptureCo());

            if (_screenshotComplete != null)
            {
                _screenshotComplete();
            }
        }
        this._popupContainer.EnablePopup(true,this.type);
        var s = SRServiceManager.GetService<IBugReportService>();

        var r = new BugReport();

        r.Email = this._emailText.text;
        r.UserDescription = this._messageText.text;

        r.ConsoleLog = Service.Console.AllEntries.ToList();
        r.SystemInformation = SRServiceManager.GetService<ISystemInformationService>().CreateReport();
        r.ScreenshotData = BugReportScreenshotUtil.ScreenshotData;
        
        JObject c =  SaveDataJsonInterface.GetJsonFileAsJobject();

        Dictionary<string, object> saveInfo = new Dictionary<string, object>();
        IEnumerator<KeyValuePair<string, JToken?>> enumerator = c.GetEnumerator();
        do
        {
            if(enumerator.Current.Key == null || enumerator.Current.Value == null) continue;
            saveInfo.Add(enumerator.Current.Key, enumerator.Current.Value);
        } while (enumerator.MoveNext() == true);
        saveInfo.Add("Json",c.ToString());
        r.SystemInformation.Add("Save", saveInfo);
        
        BugReportScreenshotUtil.ScreenshotData = null;

        s.SendBugReport(r, OnBugReportComplete, new Progress<float>(OnBugReportProgress));
    }
    
    private void OnBugReportProgress(float progress)
    {
        this._sliderSending.value = progress;
        this._textSlider.text = $"{(int)(progress * 100f)}%";
    }

    private void OnBugReportComplete(bool didSucceed, string errorMessage)
    {
        if (!didSucceed)
        {
            Debug.LogError($"<color=red>Error</color> sending bug report : {errorMessage}");
        }
        else
        {
            Debug.Log("<color=lime>Bug report submitted successfully</color>");
        }
        
        this._submit.SetActive(true);
        this._sending.SetActive(false);
        PopupContainer.StaticHidePopup(this.type);
        //SetLoadingSpinnerVisible(false);
        //SetFormEnabled(true);

        //if (SubmitComplete != null)
        //{
        //    SubmitComplete(didSucceed, errorMessage);
        //}

        this._coroutineSend = null;
    }

}
