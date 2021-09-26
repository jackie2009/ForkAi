 

internal class EatNodeCmd : ForkAiNodeCmd<ForkAiDemo1>
{
    public EatNodeCmd(ForkAiDemo1 forkAi) : base(forkAi)
    {
    }

    public override void execute()
    {
        forkAi.stateMsg = "吃饭Ψ(￣∀￣)Ψ";
        forkAi.addHP(forkAi.getParamInt(0));
        forkAi.addMP(forkAi.getParamInt(1));
        forkAi.moveNext();
    }
}