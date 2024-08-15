using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectEvent 
{
    // Start is called before the first frame update
    // ��ư Ŭ�� �̺�Ʈ �ڵ鷯
    public abstract void OnButtonClick();

    // ��ư ȣ�� ���� �̺�Ʈ �ڵ鷯
    public abstract void OnButtonHoverEnter();

    // ��ư ȣ�� ���� �̺�Ʈ �ڵ鷯
    public abstract void OnButtonHoverExit();

    // ��ư ���� �̺�Ʈ �ڵ鷯
    public abstract void OnButtonSelect();

    // ��ư ���� ���� �̺�Ʈ �ڵ鷯
    public abstract void OnButtonDeselect();

    public abstract void OnInteraction();

    public abstract void OnInputEscape();
}
