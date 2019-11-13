using System;

namespace TravelBlog.Extensions
{
    public static class StringExtensions
    {
        public static int CountWords(this string text)
        {
            int wordCount = 0, index = 0;

            // Skip all chars until first word
            while (index < text.Length && !char.IsLetter(text[index]))
                index++;

            while (index < text.Length)
            {
                // Go through all chars of the current word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                wordCount++;

                // Skip all chars until next word
                while (index < text.Length && !char.IsLetter(text[index]))
                    index++;
            }

            return wordCount;
        }
    }
}
