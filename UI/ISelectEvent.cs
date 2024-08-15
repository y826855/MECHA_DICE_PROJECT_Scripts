using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectEvent 
{
    // Start is called before the first frame update
    // 버튼 클릭 이벤트 핸들러
    public abstract void OnButtonClick();

    // 버튼 호버 진입 이벤트 핸들러
    public abstract void OnButtonHoverEnter();

    // 버튼 호버 종료 이벤트 핸들러
    public abstract void OnButtonHoverExit();

    // 버튼 선택 이벤트 핸들러
    public abstract void OnButtonSelect();

    // 버튼 선택 해제 이벤트 핸들러
    public abstract void OnButtonDeselect();

    public abstract void OnInteraction();

    public abstract void OnInputEscape();
}
