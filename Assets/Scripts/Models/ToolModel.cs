using QFramework;

public interface IToolModel : IModel
{
    BindableProperty<ToolType> CurrentTool { get; }
    void SelectTool(ToolType tool);
}

public class ToolModel : AbstractModel, IToolModel
{
    public BindableProperty<ToolType> CurrentTool { get; } = new BindableProperty<ToolType>(ToolType.None);

    protected override void OnInit()
    {
        
    }

    public void SelectTool(ToolType tool)
    {
        CurrentTool.Value = tool;
    }
}
