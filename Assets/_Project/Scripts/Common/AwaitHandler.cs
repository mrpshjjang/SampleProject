using Cysharp.Threading.Tasks;

public class AwaitHandler
{
    public bool IsComplete { get; private set; } = false;

    public void SetComplete()
    {
        IsComplete = true;
    }

    public async UniTask WaitComplete()
    {
        await UniTask.WaitUntil(() => IsComplete);
    }
}
