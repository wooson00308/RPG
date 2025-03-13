public interface IStateHub
{
    void NextState<T>() where T : State;
    void NextState<T>(T process) where T : State;
}
