using System.Collections.Generic;

namespace CribbageRoyale.Server.Services
{
    public static class ChatCensor
    {
        private const char MaskCharacter = '*';

        private static readonly char[] PunctuationMarks =
        {
            '.', ',', ';', ':', '!', '¡', '?', '¿', '"', '\'', '(', ')', '-'
        };

        private static readonly HashSet<string> BlockedWords = new HashSet<string>
        {
            "pendejo", "pendeja", "pendejos", "pendejas",
            "puto", "puta", "putos", "putas",
            "cabron", "cabrón", "cabrones","chupala", "chupamela", "chupamelo", "chupamela",
            "verga", "mierda", "culero", "culera", "pinche",
            "fuck", "shit", "bitch", "asshole", "bastard", "dickhead", "cunt", 
            "slut", "whore", "fag", "nigger", "nigga"
        };

        public static string Censor(string text)
        {
            string[] words = text.Split(' ');

            for (int index = 0; index < words.Length; index++)
            {
                string cleanWord = words[index].Trim(PunctuationMarks).ToLowerInvariant();

                if (BlockedWords.Contains(cleanWord))
                {
                    words[index] = new string(MaskCharacter, words[index].Length);
                }
            }

            return string.Join(" ", words);
        }
    }
}