internal class MPCheckNodeCmd : ForkAiNodeCmd<ForkAiDemo1>
{
    public MPCheckNodeCmd(ForkAiDemo1 forkAi) : base(forkAi)
    {
    }

    public override void execute()
    {
        forkAi.executeNextCondition(forkAi.getParamInt(1), forkAi.MP);
    }
}