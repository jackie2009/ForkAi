 
using UnityEngine;
internal class AttackNodeCmd : ForkAiNodeCmd<ForkAiDemo1>
{
    public AttackNodeCmd(ForkAiDemo1 forkAi) : base(forkAi)
    {
    }

    public override void execute()
    {

        forkAi.stateMsg = "打豆豆p(^o^)q";
        forkAi.addHP(forkAi.getParamInt(0));
        forkAi.addMP(forkAi.getParamInt(1));

        forkAi.moveNext();
    }
}