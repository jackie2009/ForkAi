internal class EmptyNodeCmd : ForkAiNodeCmd<BaseForkAi>
{
    public EmptyNodeCmd(BaseForkAi forkAi) : base(forkAi)
    {
    }

    public override void execute()
    {
        forkAi.executeNext();
    }
}