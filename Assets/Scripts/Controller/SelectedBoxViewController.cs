using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class SelectedBoxViewController : MonoBehaviour, IController
{
    [SerializeField] private GameObject selectedBox;
    private ICursorModel cursorModel;
    public IArchitecture GetArchitecture()
    {
        return ShennongAlmanac.Interface;
    }

    void Start()
    {
        cursorModel = this.GetModel<ICursorModel>();
    }
    void Update()
    {
        cursorModel.CursorPos = CursorUtil.GetWorldPosTile();
        selectedBox.transform.position = cursorModel.CursorPos + new Vector3(0.5f, 0.5f, 0);
    }
}
