using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolorTermInfoViewController : MonoBehaviour,IController
{
    [SerializeField]
    private TextMeshProUGUI _solorTermName;
    [SerializeField]
    private TextMeshProUGUI _solorTermTemp;
    [SerializeField]
    private TextMeshProUGUI _solorTermLightInfo;
    [SerializeField]
    private Button _nextBtn;
    [SerializeField]
    private Button _preBtn;
    private ITermModel _termModel;
    public IArchitecture GetArchitecture()
    {
        return ShennongAlmanac.Interface;
    }

    void Start()
    {
        _termModel = this.GetModel<ITermModel>();
        //_nextBtn.onClick.AddListener(() => { _termModel.CurrentTermId.Value++; });
        //_preBtn.onClick.AddListener(() => { _termModel.CurrentTermId.Value--; });
        _termModel.CurrentTermId.RegisterWithInitValue(newCount => 
            {
                UpdateSolorTermInfo();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    public void UpdateSolorTermInfo()
    {
        _solorTermName.text = _termModel.CurrentTermName;
        _solorTermTemp.text = _termModel.CurrentTemp.ToString() + "°C";
        _solorTermLightInfo.text = _termModel.CurrentLight;
    }
}
