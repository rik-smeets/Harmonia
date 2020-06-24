using System;
using Harmonia.Wrappers.Interfaces;
using WK.Libraries.SharpClipboardNS;

namespace Harmonia.Wrappers
{
    public class ClipboardWrapper : IClipboardWrapper
    {
        public event EventHandler<string> ClipboardTextChanged;

        private readonly SharpClipboard _clipboard = new SharpClipboard();

        public ClipboardWrapper()
        {
            _clipboard.ClipboardChanged += Clipboard_ClipboardChanged;
        }

        private void Clipboard_ClipboardChanged(object sender, SharpClipboard.ClipboardChangedEventArgs e)
        {
            if (e.ContentType == SharpClipboard.ContentTypes.Text)
            {
                ClipboardTextChanged?.Invoke(this, _clipboard.ClipboardText);
            }
        }
    }
}