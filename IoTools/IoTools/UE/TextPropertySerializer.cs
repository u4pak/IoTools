using System.Text;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.UObject;

namespace PropertyEditor.Core.UE;

public class TextPropertySerializer
{
    private List<byte> _bytes = new();

    public TextPropertySerializer(FText fText)
    {
        _bytes.AddRange(BitConverter.GetBytes(fText.Flags));
        _bytes.Add((byte)fText.HistoryType);
        
        var text = (FTextHistory.Base)fText.TextHistory;

        _bytes.AddRange(BitConverter.GetBytes(text.Namespace.Length));
        _bytes.AddRange(Encoding.ASCII.GetBytes(text.Namespace));
        
        _bytes.AddRange(BitConverter.GetBytes(text.Key.Length));
        _bytes.AddRange(Encoding.ASCII.GetBytes(text.Key));
        
        _bytes.AddRange(BitConverter.GetBytes(text.SourceString.Length));
        _bytes.AddRange(Encoding.ASCII.GetBytes(text.SourceString));
    }
    
    public byte[] GetBytes() => _bytes.ToArray();
}