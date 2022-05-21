namespace Harmonia.Wrappers.Interfaces
{
    public interface IClipboardWrapper
    {
        event EventHandler<string> ClipboardTextChanged;
    }
}