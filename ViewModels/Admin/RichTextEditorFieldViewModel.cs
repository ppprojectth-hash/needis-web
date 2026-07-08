namespace Needis.Web.ViewModels.Admin;

public class RichTextEditorFieldViewModel
{
    public string FieldName { get; set; } = string.Empty;
    public string? FieldId { get; set; }
    public string? Value { get; set; }
    public int Rows { get; set; } = 4;
    public string? Placeholder { get; set; }

    // "en" or "th" — controls the helper text and the default text inserted by toolbar buttons.
    public string Language { get; set; } = "en";
}
