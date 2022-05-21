using Harmonia.Wrappers.Interfaces;
using WK.Libraries.SharpClipboardNS;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace Harmonia.Wrappers
{
    public class ClipboardWrapper : IClipboardWrapper
    {
        public event EventHandler<string> ClipboardTextChanged;

        private readonly SharpClipboard _clipboard = new();

        public ClipboardWrapper()
        {
            _clipboard.ClipboardChanged += Clipboard_ClipboardChanged;
        }

        private void Clipboard_ClipboardChanged(object sender, ClipboardChangedEventArgs e)
        {
            if (e.ContentType is ContentTypes.Text)
            {
                ClipboardTextChanged?.Invoke(this, _clipboard.ClipboardText);
            }
        }
    }
}