 
using UnityEngine;

internal class UseSkillNodeCmd : ForkAiNodeCmd<ForkAiDemo2>
{
    public UseSkillNodeCmd(ForkAiDemo2 forkAi) : base(forkAi)
    {
    }

    public override void execute()
    {


        forkAi.addMsg("UseSkillNodeCmd:" + forkAi.getParam(0));

        forkAi.moveNext();
    }
}