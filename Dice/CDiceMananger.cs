using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CDiceMananger : MonoBehaviour
{


	public enum EDiceRollState 
	{ 
		NONE,
		ROLL_FIRST, //�� ù�� ������ �ֻ���
		READY_TO_GRAB_CUP, //�ֻ��� ������
		ROLL_WAIT,  //������ �� ��ٸ�
		ROLL_HANDLEING, //������ �� ������� ��
		ROLLING_DICE,  //�ֻ��� ������ ��
		ROLL_STOP, //�ֻ��� ������ ��
		ROLL_STOP_ACTION_DONE, //�ֻ��� �������� ��ų�� �������� //������� �ൿ�� ���⼭

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

	//������ TMP_RollCount �����ؾ���
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
	
	//�� ù�� ������ ��� �ֻ��� ������ �ϱ�
	public void DiceFirstRoll() 
	{
		Debug.Log("FIRST ROLL");

		//������ų �ʱ�ȭ
		foreach(var it in m_Dices) it.ResetManaSkill();

		m_DiceHolder.m_Cup.gameObject.SetActive(false);

		m_DiceChoiceArea.m_Dices.Clear();
		m_DiceChoiceArea.GetDiceData(m_Dices);

		m_DiceRollCount = CGameManager.Instance.m_PlayerData.m_RollMax;
		EnableButton_DiceRoll();
		m_State = EDiceRollState.ROLL_FIRST;
	}

	//�ֻ��� ���� ��ư Ȱ��ȭ üũ
	public void EnableButton_DiceRoll() 
	{
		m_Btn_DiceRoll.interactable = m_DiceRollCount > 0;
	}

	//�ֻ��� ���� ��ư ������ ������ �̵��ϰ� �ֻ��� ���� �غ� ��
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

	//��� ��װ� ����
	public void LockAndRoll() 
	{
		m_State = CDiceMananger.EDiceRollState.ROLL_HANDLEING;
		m_DiceRollCount--;
		EnableButton_DiceRoll();
		//m_DiceHolder.m_CupArea.m_IsCanEscape = false;
	}

	//�ֻ��� �� ��� ���� ���
	public void OnClick_Escape() 
	{
		if (m_State != EDiceRollState.READY_TO_GRAB_CUP) return;

		Debug.Log("ESCAPE!!");

		m_DiceChoiceArea.MoveDiceToPos_All();
		m_DiceHolder.m_Cup.gameObject.SetActive(false);
		m_State = beforeState;
	}

	//�ֻ��� ���� �� Ȯ������
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

		//�ֻ��� ��Ȱ��ȭ
		foreach (var dice in m_Dices) 
		{ 
			dice.transform.parent = m_DiceDefaultArea;
			StartCoroutine(dice.CoMoveToDisplay(m_DiceDefaultArea.position, _toggleUIMode: false));
		}
	}

	//�ֻ��� �������� �ݹ�
	public void DiceIsStopped() 
	{
		//�ֻ��� ���� ��� ĭ���� ������
		//m_DiceChoiceArea.gameObject.SetActive(true);
		//m_DiceChoiceArea.MoveDiceToPos();
		m_DiceChoiceArea.DiceRollDone();
		StartCoroutine(CheckDiceStopAction());

		m_State = EDiceRollState.ROLL_STOP;
	}


	//���� ���� STOP_SKILL ������ �����Ŵ
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
	//�ֻ��� UI����϶� ����������
	public void OnClick_Dice(CDice _dice) 
	{
		if (m_State == EDiceRollState.ROLL_STOP_ACTION_DONE)
		{//�������� ���̽����� ����
			switch (_dice.m_DiceState)
			{
				case CDice.EDiceState.DICE_SAVED:
					bool IsChangeFocusArea = CursorMoveRight(_dice);
					m_DiceSaveArea.ReleaseData(_dice);
					m_DiceChoiceArea.GetDiceData(_dice);
					//if (IsChangeFocusArea == true) //�Է� ��Ŀ�� ����
					//{ ChangeFocusArea(m_DiceChoiceArea.m_SelectableArea, _dice); }
					break;
				case CDice.EDiceState.DICE_CHOICE_WAIT:
					IsChangeFocusArea = CursorMoveRight(_dice);
					m_DiceChoiceArea.ReleaseData(_dice);
					m_DiceSaveArea.GetDiceData(_dice);
					//if (IsChangeFocusArea == true) //�Է� ��Ŀ�� ����
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



	//�ֻ��� Ŀ�� �̵� ����
	public bool CursorMoveRight(CDice _dice) 
	{

		var selectable = _dice.m_Selectable;


		Debug.Log(_dice.transform.parent.name);

		return true;

	}


	//�ֻ��� ��ġ ����
	public void DiceSwap(CDice _dice) 
	{
		var temp = _dice.transform.position;
		var swapPos = m_SwapPick.transform.position;
		StartCoroutine(_dice.CoMoveToDisplay(swapPos, true));
		StartCoroutine(m_SwapPick.CoMoveToDisplay(temp, true));

		m_SwapPick = null;
	}
}
