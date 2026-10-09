public interface ILeverTarget
{
    string Passcode { get; }

    void Activate();
    void Deactivate();
}
