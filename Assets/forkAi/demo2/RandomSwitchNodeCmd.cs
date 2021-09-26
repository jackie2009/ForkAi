using UnityEngine;

internal class RandomSwitchNodeCmd : ForkAiNodeCmd<ForkAiDemo2>
{
    public RandomSwitchNodeCmd(ForkAiDemo2 forkAi) : base(forkAi)
    {
    }

    public override void execute()
    {
        var node = forkAi.currentNode;
        int rat = Random.Range(0, 100);
        for (int i = 0; i < node.param.Length / 2; i++)
        {
            rat -= int.Parse(node.param[i]);
            if (rat < 0)
            {
                forkAi.executeTo(int.Parse(node.param[node.param.Length / 2 + i]));
                return;
            };
        }
    }
}