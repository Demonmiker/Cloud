using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CognitiveServices.Speech;

namespace ClientServerLibrary.Logger
{
    class VoiceLogger : ILogger
    {
        SpeechSynthesizer SS;

        public VoiceLogger()
        {
            SS = new SpeechSynthesizer(SpeechConfig.FromSubscription
                ("YourSubscriptionKey", "YourServiceRegion"));
        }

        public void Write(String S)
        {
            SS.SpeakTextAsync(S);
        }

        public void WriteLine(String S)
        {
            SS.SpeakTextAsync(S);
        }
    }
}
