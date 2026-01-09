using UnityEngine;
using TMPro;
using System.Data;
using System;
using System.Text; 

public class Calculator : MonoBehaviour
{
    public TextMeshProUGUI inputField;
    private string currentInput = "";

    [Header("🐞 Debug Flags (이벤트에 의해 제어됨)")]
    public bool bug_ReversePlus = false;
    public bool bug_TripleThree = false;
    public bool bug_CopyDelete = false;
    public bool bug_CorruptResult = false;

    // --- 기본 계산기 기능 ---

    public void ClickButton(string value)
    {
        // [BUG 1] 덧셈 -> 뺄셈
        if (bug_ReversePlus && value == "+") value = "-";
        
        // [BUG 2] 3 -> 333
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
        // [BUG 3] 지우기 -> 증식
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
        // [BUG 4] 결과 깨짐
        if (bug_CorruptResult)
        {
            inputField.text = MakeGibberish(currentInput);
        }
        else
        {
            inputField.text = currentInput;
        }
    }

    private void ShowError()
    {
        inputField.text = "Error";
        currentInput = "";
    }

    // --- 외부 이벤트 연결용 함수 (UnityEvent용) ---
    // UniversalBugTrigger에서 이 함수들을 호출합니다.

    public void SetReversePlus(bool active) => bug_ReversePlus = active;
    
    public void SetTripleInput(bool active) => bug_TripleThree = active;
    
    public void SetCopyDelete(bool active) => bug_CopyDelete = active;

    public void SetCorruptResult(bool active)
    {
        bug_CorruptResult = active;
        // 버그가 꺼질 때(false) 화면을 즉시 정상화
        if (!active) RefreshDisplay();
    }

    // 화면 강제 새로고침 (버그 해제 시 호출)
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