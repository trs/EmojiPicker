using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace EmojiShortcutApp
{
    public class EmojiSearch {
        //public string Emoji;
        public List<string> unicode;
        public List<string> keywords;
    }

    public class Emoji
    {
        public static List<EmojiSearch> Load()
        {
            var emojiJSON = Encoding.UTF8.GetString(Resource.emoji_search);
            var emojiList = JsonConvert.DeserializeObject<List<EmojiSearch>>(emojiJSON);

            return emojiList;
        }
    }
}
