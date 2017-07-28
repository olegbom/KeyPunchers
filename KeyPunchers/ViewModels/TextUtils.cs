using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace KeyPunchers.ViewModels
{
    public static class TextUtils
    {
        public static IEnumerable<string> GetLines(this TextBlock source)
        {
            var text = source.Text;
            int offset = 0;
            TextPointer lineStart = source.ContentStart.GetPositionAtOffset(1, LogicalDirection.Forward);
            do
            {
                TextPointer lineEnd = lineStart != null ? lineStart.GetLineStartPosition(1) : null;
                int length = lineEnd != null ? lineStart.GetOffsetToPosition(lineEnd) : text.Length - offset;
                yield return text.Substring(offset, length);
                offset += length;
                lineStart = lineEnd;
            }
            while (lineStart != null);
        }

        public static int GetFirtsLineEndPosition(this TextBlock source)
        {
            var text = source.Text;
            TextPointer lineStart = source.ContentStart.GetPositionAtOffset(1, LogicalDirection.Forward);
            TextPointer lineEnd = lineStart?.GetLineStartPosition(1);
            return lineEnd != null ? lineStart.GetOffsetToPosition(lineEnd) : text.Length;
        }
    }
}
