using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] private GameObject questionUI;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private RoomArchiveManager roomArchiveManager;
    private GameObject roomObj;
    private string answer;

    //제출버튼을 눌렀을때 작동. 방 obj를 저장하고 질문을 받는 ui를 실행.
    public void StartQuestion(GameObject go)
    {
        questionUI.SetActive(true);
        roomObj = go;
    }

    //질문에 대한 답을 완료하고 제출버튼을 눌렀을때 실행. 적힌 답변을 저장하여 방 obj화 함께 아카이브 스크립트로 넘김.
    public void SubmitAnswer()
    {
        answer = inputField.text;
        inputField.text = "";
        if(roomObj!=null)
        {
            roomArchiveManager.saveRoom(roomObj,answer);
            questionUI.SetActive(false);
        }
        else
        {
            Debug.Log("roomObj error");
        }     
    }
}
