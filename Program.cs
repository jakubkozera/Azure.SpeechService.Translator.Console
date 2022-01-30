// 1 Translacja mowy do tekstu
// 2 Przeczytanie przetłumaczonego teskstu w konkretnym języku
// 3 Zapętlenie rozmowy

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

var key = "<speech-service-key>";
var region = "<speech-service-region>";

LanguageSettings es = new("es", "es-MX", "es-MX-JorgeNeural");
LanguageSettings pl = new("pl", "pl-PL", "pl-PL-MarekNeural");
while (true)
{
    await Translate(pl.Locale, es);
    await Translate(es.Locale, pl);
}

async Task Translate(string sourceLocale, LanguageSettings targetLanguage)
{
    var translationnConfig = SpeechTranslationConfig.FromSubscription(key, region);
    translationnConfig.SpeechRecognitionLanguage = sourceLocale;
    translationnConfig.AddTargetLanguage(targetLanguage.Language);

    using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

    using var recognizer = new TranslationRecognizer(translationnConfig, audioConfig);
    var result = await recognizer.RecognizeOnceAsync();

    Console.WriteLine($"Input: {result.Text}");
    var translatedResult = result.Translations[targetLanguage.Language];
    Console.WriteLine($"Output: {translatedResult}");

    var config = SpeechConfig.FromSubscription(key, region);
    config.SpeechRecognitionLanguage = targetLanguage.Locale;
    config.SpeechSynthesisVoiceName = targetLanguage.VoiceName;

    using var synthesizer = new SpeechSynthesizer(config);
    await synthesizer.SpeakTextAsync(translatedResult);
}

record LanguageSettings(string Language, string Locale, string VoiceName);