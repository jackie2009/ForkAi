internal class HPCheckNodeCmd : ForkAiNodeCmd<ForkAiDemo1>
{
    public HPCheckNodeCmd(ForkAiDemo1 forkAi) : base(forkAi)
    {
    }

    public override void execute()
    {
        forkAi.executeNextCondition(forkAi.getParamInt(1),  forkAi .HP);
    }
}