using System.Collections;
using System.Collections.Generic;

public interface IForkAiNodeCmd {
    void execute();
}
public abstract class ForkAiNodeCmd<T> : IForkAiNodeCmd where T: BaseForkAi
{
   internal T forkAi;

    protected ForkAiNodeCmd(T forkAi)
    {
        this.forkAi = forkAi;
    }

    public abstract void execute();
}

