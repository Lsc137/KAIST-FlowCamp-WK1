using UnityEngine;
using TMPro;
using System.Data;
using System;

public class Calculator : MonoBehaviour
{
    public TextMeshProUGUI inputField;
    private string currentInput = "";

    [Header("Navigation")]
    public GameObject backButton; // [추가] 뒤로가기 버튼 (처음엔 꺼짐)

    [Header("🐞 Debug Flags")]
    public bool bug_ReversePlus = true;
    public bool bug_TripleThree = true;
    public bool bug_CopyDelete = true;
    public bool bug_CorruptResult = true;

    // 앱이 켜질 때마다 실행 (상태 복구 로직)
    void OnEnable()
    {
        Clear(); // 화면 숫자 초기화

        // 1. 이미 깬 상태인지 확인
        if (GameManager.Instance != null && GameManager.Instance.isCalcClear)
        {
            SetNormalMode(); // 정상 모드 (버그 OFF, 뒤로가기 ON)
        }
        else
        {
            // 2. 아직 못 깼으면 -> 버그 강제 활성화 & 못 나감
            bug_ReversePlus = true;
            bug_TripleThree = true;
            bug_CopyDelete = true;
            bug_CorruptResult = true;
            
            if (backButton) backButton.SetActive(false); // 감옥
        }
    }

    // 정상 모드로 전환하는 함수
    public void SetNormalMode()
    {
        bug_ReversePlus = false;
        bug_TripleThree = false;
        bug_CopyDelete = false;
        bug_CorruptResult = false;

        RefreshDisplay(); // 화면 깨진 거 복구

        if (backButton) backButton.SetActive(true); // 탈출구 열림
        Debug.Log("🛡️ 계산기: 정상 모드 가동");
    }

    // 버그가 해결될 때마다 호출해서 "다 깼나?" 확인하는 함수
    public void CheckAllBugsFixed()
    {
        // 4개 다 꺼졌는지 확인
        if (!bug_ReversePlus && !bug_TripleThree && !bug_CopyDelete && !bug_CorruptResult)
        {
            if (GameManager.Instance) GameManager.Instance.CompleteCalc();
            if (backButton) backButton.SetActive(true);
            Debug.Log("🎉 계산기 앱 완전 정복!");
        }
    }

    // --- 아래는 기존 계산기 로직 (그대로 유지) ---

    public void ClickButton(string value)
    {
        if (bug_ReversePlus && value == "+") value = "-";
        if (bug_TripleThree && value == "3") value = "333";

        currentInput += value;
        inputField.text = currentInput;
    }

    public void Clear()
    {
        currentInput = "";
        inputField.text = "0";
    }

    public void Backspace()
    {
        if (bug_CopyDelete)
        {
            if (currentInput.Length > 0)
            {
                currentInput += currentInput;
                inputField.text = currentInput;
            }
            return;
        }

        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            inputField.text = (currentInput == "") ? "0" : currentInput;
        }
    }

    public void Sqrt()
    {
        try
        {
            double value = Convert.ToDouble(new DataTable().Compute(currentInput, ""));
            double result = Math.Sqrt(value);
            currentInput = result.ToString();
            UpdateResultDisplay();
        }
        catch { ShowError(); }
    }

    public void Calculate()
    {
        try
        {
            DataTable table = new DataTable();
            var result = table.Compute(currentInput, "");
            currentInput = result.ToString();
            UpdateResultDisplay();
        }
        catch { ShowError(); }
    }

    private void UpdateResultDisplay()
    {
        if (bug_CorruptResult) inputField.text = MakeGibberish(currentInput);
        else inputField.text = currentInput;
    }

    private void ShowError()
    {
        inputField.text = "Error";
        currentInput = "";
    }

    // --- 외부 이벤트 연결용 (UniversalBugTrigger에서 호출) ---
    // [중요] 각 함수 끝에 CheckAllBugsFixed() 추가됨

    public void SetReversePlus(bool active) 
    { 
        bug_ReversePlus = active; 
        if (!active) CheckAllBugsFixed(); 
    }
    
    public void SetTripleInput(bool active) 
    { 
        bug_TripleThree = active; 
        if (!active) CheckAllBugsFixed();
    }
    
    public void SetCopyDelete(bool active) 
    { 
        bug_CopyDelete = active; 
        if (!active) CheckAllBugsFixed();
    }

    public void SetCorruptResult(bool active)
    {
        bug_CorruptResult = active;
        if (!active) 
        {
            RefreshDisplay();
            CheckAllBugsFixed();
        }
    }

    public void RefreshDisplay()
    {
        inputField.text = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
    }

    private string MakeGibberish(string original)
    {
        string brokenChars = "ÆØÅ¢£¥§©®µ¶¿Ħ€ŁŁØ";
        char[] chars = original.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (char.IsDigit(chars[i])) 
                chars[i] = brokenChars[UnityEngine.Random.Range(0, brokenChars.Length)];
        }
        return new string(chars);
    }
}