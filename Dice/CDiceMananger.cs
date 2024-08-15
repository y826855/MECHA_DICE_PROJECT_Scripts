using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CDiceMananger : MonoBehaviour
{


	public enum EDiceRollState 
	{ 
		NONE,
		ROLL_FIRST, //매 첫턴 굴리는 주사위
		READY_TO_GRAB_CUP, //주사위 보여줌
		ROLL_WAIT,  //굴리기 전 기다림
		ROLL_HANDLEING, //손으로 컵 잡고있을 때
		ROLLING_DICE,  //주사위 구르는 중
		ROLL_STOP, //주사위 멈췄을 때
		ROLL_STOP_ACTION_DONE, //주사위 멈췄을때 스킬이 끝났을때 //통상적인 행동은 여기서

		DICE_RESULT_SAVE,
		DICE_SWAP,

		MANA_SKILL_SELECT,
	}

	public CDiceHolder m_DiceHolder = null;
	public CDiceDisplay m_DiceChoiceArea = null;
	public CDiceDisplay m_DiceSaveArea = null;
	public Transform m_DiceDefaultArea = null;
	public Button m_Btn_DiceRoll = null;

	public List<CDice> m_Dices = new List<CDice>();

	//씬에서 TMP_RollCount 참조해야함
	public TMPro.TextMeshProUGUI m_TMP_DiceRoll = null;
	[SerializeField]int DiceRollCount = 0;
	public int m_DiceRollCount 
	{
		get { return DiceRollCount; }
		set 
		{
			DiceRollCount = value;
			if (m_TMP_DiceRoll != null)
				m_TMP_DiceRoll.text = string.Format("{0}/{1}",
					DiceRollCount, CGameManager.Instance.m_PlayerData.m_RollMax);
		}
	}

	[SerializeField] EDiceRollState state = EDiceRollState.READY_TO_GRAB_CUP;
	public EDiceRollState m_State
	{
		get { return state; }
		set { 
			state = value;
			if(m_CB_ChangeState != null) m_CB_ChangeState(state);
		}
	}

	public System.Action<EDiceRollState> m_CB_ChangeState = null;
	[Header("============================")]
	public CManaSkillManager m_ManaSkillMgr = null;

	public System.Action CB_SavedDice = null;

    private void Awake()
    {
		CGameManager.Instance.m_DiceManager = this;
    }
    public void Start()
	{
		//DiceFirstRoll();
	}

	public void ForceQuitTurn() 
	{
		m_DiceChoiceArea.m_Dices.Clear();
		m_DiceChoiceArea.GetDiceData(m_Dices);
		m_DiceHolder.ForceStop();
	}
	
	//매 첫턴 굴림은 모든 주사위 굴리게 하기
	public void DiceFirstRoll() 
	{
		Debug.Log("FIRST ROLL");

		//마나스킬 초기화
		foreach(var it in m_Dices) it.ResetManaSkill();

		m_DiceHolder.m_Cup.gameObject.SetActive(false);

		m_DiceChoiceArea.m_Dices.Clear();
		m_DiceChoiceArea.GetDiceData(m_Dices);

		m_DiceRollCount = CGameManager.Instance.m_PlayerData.m_RollMax;
		EnableButton_DiceRoll();
		m_State = EDiceRollState.ROLL_FIRST;
	}

	//주사위 굴림 버튼 활성화 체크
	public void EnableButton_DiceRoll() 
	{
		m_Btn_DiceRoll.interactable = m_DiceRollCount > 0;
	}

	//주사위 굴림 버튼 누르면 컵으로 이동하고 주사위 굴릴 준비 됨
	[HideInInspector] public EDiceRollState beforeState = EDiceRollState.NONE;
	public void OnClick_ReadyDiceRoll() 
	{
		CGameManager.Instance.m_Input.SetEscape(OnClick_Escape);

		switch (m_State) 
		{
			case EDiceRollState.ROLL_STOP_ACTION_DONE:
				if (m_DiceChoiceArea.m_Dices.Count > 0)
				{
					StartCoroutine(m_DiceHolder.CoSetReadyDiceRoll(m_DiceChoiceArea.m_Dices));
					m_State = EDiceRollState.READY_TO_GRAB_CUP;
					beforeState = EDiceRollState.ROLL_STOP_ACTION_DONE;
				}
				break;
			case EDiceRollState.ROLL_FIRST:
				StartCoroutine(m_DiceHolder.CoSetReadyDiceRoll(m_DiceChoiceArea.m_Dices));
				m_State = EDiceRollState.READY_TO_GRAB_CUP;
				beforeState = EDiceRollState.ROLL_FIRST;
				break;
		}
	}

	//취소 잠그고 굴림
	public void LockAndRoll() 
	{
		m_State = CDiceMananger.EDiceRollState.ROLL_HANDLEING;
		m_DiceRollCount--;
		EnableButton_DiceRoll();
		//m_DiceHolder.m_CupArea.m_IsCanEscape = false;
	}

	//주사위 롤 대기 상태 취소
	public void OnClick_Escape() 
	{
		if (m_State != EDiceRollState.READY_TO_GRAB_CUP) return;

		Debug.Log("ESCAPE!!");

		m_DiceChoiceArea.MoveDiceToPos_All();
		m_DiceHolder.m_Cup.gameObject.SetActive(false);
		m_State = beforeState;
	}

	//주사위 굴린 값 확정지음
	public void OnClick_SaveResult() 
	{
		switch (m_State)
		{
			case EDiceRollState.ROLL_STOP_ACTION_DONE:
				m_DiceSaveArea.GetDiceData(m_DiceChoiceArea.m_Dices);
				m_DiceChoiceArea.ResetDiceData();
				m_State = EDiceRollState.DICE_RESULT_SAVE;
				CGameManager.Instance.m_TurnManager.m_PlayerChar.OnDiceResult();

				if (CB_SavedDice != null) CB_SavedDice();
				break;
		}
	}

	public void OnClick_EndTurn() 
	{
		//CGameManager.Instance.m_TurnManager
		m_DiceSaveArea.ResetDiceData();
		m_DiceChoiceArea.ResetDiceData();

		//주사위 비활성화
		foreach (var dice in m_Dices) 
		{ 
			dice.transform.parent = m_DiceDefaultArea;
			StartCoroutine(dice.CoMoveToDisplay(m_DiceDefaultArea.position, _toggleUIMode: false));
		}
	}

	//주사위 멈췄을때 콜백
	public void DiceIsStopped() 
	{
		//주사위 선택 대기 칸으로 보내기
		//m_DiceChoiceArea.gameObject.SetActive(true);
		//m_DiceChoiceArea.MoveDiceToPos();
		m_DiceChoiceArea.DiceRollDone();
		StartCoroutine(CheckDiceStopAction());

		m_State = EDiceRollState.ROLL_STOP;
	}


	//멈춘 이후 STOP_SKILL 있으면 실행시킴
	IEnumerator CheckDiceStopAction() 
	{
		foreach (var dice in m_DiceChoiceArea.m_Dices) 
		{
			//if (dice.m_Skill_Stop != null)
			if (dice.m_ManaSkill != null)
			{
				//float duration = dice.m_Skill_Stop.DoAction();
				//yield return CUtility.GetSecD1To5s(duration);
				yield return StartCoroutine(dice.m_Skill_Stop.CoDoSkillAction());
				yield return CUtility.m_WFS_DOT4;
			}
		}

		yield return CUtility.m_WFS_1;


		m_State = EDiceRollState.ROLL_STOP_ACTION_DONE;
		//m_DiceChoiceArea.gameObject.SetActive(true);
		m_DiceChoiceArea.MoveDiceToPos();
		//m_DiceChoiceArea.m_SelectableArea.m_CanvasGroup.interactable = true;
		//m_DiceHolder.m_CupArea.Close_And_OutFocusOut_Force();
	}


	CDice m_SwapPick = null;
	//주사위 UI모드일때 선택했을때
	public void OnClick_Dice(CDice _dice) 
	{
		if (m_State == EDiceRollState.ROLL_STOP_ACTION_DONE)
		{//저장인지 초이스인지 선택
			switch (_dice.m_DiceState)
			{
				case CDice.EDiceState.DICE_SAVED:
					bool IsChangeFocusArea = CursorMoveRight(_dice);
					m_DiceSaveArea.ReleaseData(_dice);
					m_DiceChoiceArea.GetDiceData(_dice);
					//if (IsChangeFocusArea == true) //입력 포커싱 변경
					//{ ChangeFocusArea(m_DiceChoiceArea.m_SelectableArea, _dice); }
					break;
				case CDice.EDiceState.DICE_CHOICE_WAIT:
					IsChangeFocusArea = CursorMoveRight(_dice);
					m_DiceChoiceArea.ReleaseData(_dice);
					m_DiceSaveArea.GetDiceData(_dice);
					//if (IsChangeFocusArea == true) //입력 포커싱 변경
					//{ ChangeFocusArea(m_DiceSaveArea.m_SelectableArea, _dice); }
					break;
			}

			if (CB_SavedDice != null) CB_SavedDice();
			//m_SkillMgr.CheckCanUseCards();
		}
		else if (m_State == EDiceRollState.MANA_SKILL_SELECT) 
		{
			m_ManaSkillMgr.m_SelectedDice = _dice;
			m_ManaSkillMgr.End_PickDice();
		}
	}



	//주사위 커서 이동 변경
	public bool CursorMoveRight(CDice _dice) 
	{

		var selectable = _dice.m_Selectable;


		Debug.Log(_dice.transform.parent.name);

		return true;

	}


	//주사위 위치 변경
	public void DiceSwap(CDice _dice) 
	{
		var temp = _dice.transform.position;
		var swapPos = m_SwapPick.transform.position;
		StartCoroutine(_dice.CoMoveToDisplay(swapPos, true));
		StartCoroutine(m_SwapPick.CoMoveToDisplay(temp, true));

		m_SwapPick = null;
	}
}
