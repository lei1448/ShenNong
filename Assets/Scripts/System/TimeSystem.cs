using QFramework;

public class TimeSystem : AbstractSystem
{
    private ITermModel _termModel;

    // 0.0 = 00:00, 0.5 = 12:00, 1.0 = 24:00 (Next Day)
    public BindableProperty<float> CurrentTimeOfDay { get; set; } = new(0.25f); // Default start at 6am
    public float TimeScale { get; set; } = 1.0f;

    protected override void OnInit()
    {
        _termModel = this.GetModel<ITermModel>();
    }

    // 由 Controller 每帧调用
    public void AdvanceTime(float dayFraction)
    {
        CurrentTimeOfDay.Value += dayFraction * TimeScale;

        if (CurrentTimeOfDay.Value >= 1.0f)
        {
            CurrentTimeOfDay.Value -= 1.0f;
            AdvanceDay();
        }
    }

    // 外部调用此方法让时间前进一天
    public void AdvanceDay()
    {
        _termModel.CurrentDayInTerm.Value++;

        // 检查是否超过一个节气的天数
        if (_termModel.CurrentDayInTerm.Value > _termModel.DaysPerTerm)
        {
            _termModel.CurrentDayInTerm.Value = 1;
            _termModel.CurrentTermId.Value++; // 切换到下一个节气
            
            // 发送节气变更事件（可选）
            // this.SendEvent(new OnSeasonChange { TermName = _termModel.CurrentTermName });
        }

        // 发送新的一天事件，通知农作物生长
        this.SendEvent(new OnDayPass { CurrentDay = _termModel.CurrentDayInTerm.Value });
    }
}