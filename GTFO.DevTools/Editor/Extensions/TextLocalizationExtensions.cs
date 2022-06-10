using Localization;

namespace GTFO.DevTools.Extensions
{
    public static class TextLocalizationExtensions
    {
        public static string TranslateText(this LocalizedText localizedText, GTFORundownDataBlocks datablocks)
        {
            if (!localizedText.HasTranslation)
                return localizedText.UntranslatedText;
            return datablocks.Text.GetBlockByID(localizedText.Id)?.English ?? "";
        }
    }
}