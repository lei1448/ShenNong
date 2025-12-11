using System.Collections.Generic;
using QFramework;
using UnityEngine;

public interface IKnowledgeModel : IModel
{
    HashSet<string> UnlockedKnowledge { get; }
    void Unlock(string knowledgeId);
    bool IsUnlocked(string knowledgeId);
}

public class KnowledgeModel : AbstractModel, IKnowledgeModel
{
    public HashSet<string> UnlockedKnowledge { get; } = new HashSet<string>();

    protected override void OnInit()
    {
        
    }

    public void Unlock(string knowledgeId)
    {
        if (!string.IsNullOrEmpty(knowledgeId) && !UnlockedKnowledge.Contains(knowledgeId))
        {
            UnlockedKnowledge.Add(knowledgeId);
            this.SendEvent(new OnKnowledgeUnlocked() { KnowledgeId = knowledgeId });
            Debug.Log($"[Knowledge] Unlocked: {knowledgeId}");
        }
    }

    public bool IsUnlocked(string knowledgeId)
    {
        return UnlockedKnowledge.Contains(knowledgeId);
    }
}

public struct OnKnowledgeUnlocked
{
    public string KnowledgeId;
}
