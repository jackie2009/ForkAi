 

internal class SleepNodeCmd : ForkAiNodeCmd<ForkAiDemo1>
{
    public SleepNodeCmd(ForkAiDemo1 forkAi) : base(forkAi)
    {
    }

    public override void execute()
    {
        forkAi.stateMsg = "睡觉(￣o￣) . z Z";  
        forkAi.addHP(forkAi.getParamInt(0));
        forkAi.addMP(forkAi.getParamInt(1));
    
       

        forkAi.moveNext();
    }
}