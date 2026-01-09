using UnityEngine;
using TMPro;
using System.Data;
using System;
using System.Text; // 인코딩/글자 깨짐 연출을 위해 추가
using System.Collections.Generic;

public class Calculator : MonoBehaviour
{
    public TextMeshProUGUI inputField;
    private string currentInput = "";

    // [추가] 가중치 설정을 위한 데이터 셋 (인스펙터에 예쁘게 보임)
    [System.Serializable]
    public struct BugSpawnData
    {
        public string name;           // 알아보기 쉽게 이름 (예: 탱크버그)
        public GameObject prefab;     // 버그 프리팹
        [Range(1, 100)] public int weight; // 확률 가중치 (높을수록 잘 나옴)
    }

    [Header("🦟 Bug Database (여기서 한 번만 설정하세요)")]
    public List<BugSpawnData> bugDatabase; // 전체 버그 리스트

    [Header("🐞 BUG FLAGS (테스트용 체크박스)")]
    public bool bug_ReversePlus = false;   // 1. 덧셈(+) -> 뺄셈(-)
    public bool bug_TripleThree = false;   // 2. 숫자 3 -> 333
    public bool bug_CopyDelete = false;    // 3. 지우기 -> 복사 (증식)
    public bool bug_CorruptResult = false; // 4. 결과 깨짐


    public GameObject GetWeightedRandomBug()
    {
        if (bugDatabase == null || bugDatabase.Count == 0) return null;

        // 1. 전체 가중치 합 구하기
        int totalWeight = 0;
        foreach (var bug in bugDatabase)
        {
            totalWeight += bug.weight;
        }

        // 2. 랜덤 티켓 뽑기 (0 ~ 전체 가중치 사이)
        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        // 3. 당첨 확인 (누적 가중치 방식)
        int currentWeightSum = 0;
        foreach (var bug in bugDatabase)
        {
            currentWeightSum += bug.weight;
            if (randomValue < currentWeightSum)
            {
                return bug.prefab; // 당첨된 버그 반환
            }
        }

        return bugDatabase[0].prefab; // 혹시라도 실패하면 첫 번째꺼 반환
    }
    
    public void ClickButton(string value)
    {
        // [BUG 1] 덧셈을 눌렀는데 뺄셈이 입력됨
        if (bug_ReversePlus && value == "+")
        {
            value = "-";
        }

        // [BUG 2] 3을 눌렀는데 333이 입력됨
        if (bug_TripleThree && value == "3")
        {
            value = "333";
        }

        currentInput += value;
        inputField.text = currentInput;
    }

    // Backspace: 마지막 글자 하나 지우기
    public void Backspace()
    {
        // [BUG 3] 지우기를 눌렀는데 현재 텍스트가 한번 더 복사됨 (123 -> 123123)
        if (bug_CopyDelete)
        {
            if (currentInput.Length > 0)
            {
                currentInput += currentInput; // 자기 자신을 한 번 더 붙임
                inputField.text = currentInput;
            }
            return; // 정상적인 지우기 로직을 실행하지 않고 종료
        }

        // --- 정상 로직 ---
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            inputField.text = (currentInput == "") ? "0" : currentInput;
        }
    }

    // Square Root: 현재 입력된 숫자의 제곱근 계산
    public void Sqrt()
    {
        try
        {
            double value = Convert.ToDouble(new DataTable().Compute(currentInput, ""));
            double result = Math.Sqrt(value);
            currentInput = result.ToString();
            
            // [BUG 4] 결과 깨짐 (Sqrt 결과에도 적용)
            if (bug_CorruptResult)
            {
                inputField.text = MakeGibberish(currentInput);
            }
            else
            {
                inputField.text = currentInput;
            }
        }
        catch
        {
            inputField.text = "Error";
            currentInput = "";
        }
    }

    public void Calculate()
    {
        try
        {
            DataTable table = new DataTable();
            var result = table.Compute(currentInput, "");
            currentInput = result.ToString();

            // [BUG 4] 결과값 글자 깨짐 (인코딩 오류 연출)
            if (bug_CorruptResult)
            {
                inputField.text = MakeGibberish(currentInput);
            }
            else
            {
                inputField.text = currentInput;
            }
        }
        catch
        {
            inputField.text = "Error";
            currentInput = "";
        }
    }

    // [BUG 4 보조] 멀쩡한 문자열을 외계어로 바꾸는 함수
    private string MakeGibberish(string original)
    {
        // 단순하게 유니코드 특수문자나 알 수 없는 기호로 대체
        // "깨진 인코딩" 느낌을 주기 위한 문자셋
        string brokenChars = "ÆØÅ¢£¥§©®µ¶¿Ħ€ŁŁØ";
        char[] chars = original.ToCharArray();
        
        for (int i = 0; i < chars.Length; i++)
        {
            // 숫자만 깨뜨리거나 전체를 깨뜨림
            if (char.IsDigit(chars[i])) 
            {
                chars[i] = brokenChars[UnityEngine.Random.Range(0, brokenChars.Length)];
            }
        }
        
        return new string(chars);
    }

    public void RefreshDisplay()
    {
        // 내부에 저장된 값(currentInput)이 비어있으면 "0", 아니면 그 값을 그대로 보여줌
        inputField.text = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
    }
}