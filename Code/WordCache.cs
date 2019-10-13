using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace umbracoShip.Code
{
    public class WordCache
    {

        public static void LoadStaticCache()
        {
            // Get words - cache using the data cache
            List<String> words = loadWrod();

            HttpRuntime.Cache.Insert(
                /* key */                "ShipDictonary",
                /* value */              words,
                /* dependencies */       null,
                /* absoluteExpiration */ Cache.NoAbsoluteExpiration,
                /* slidingExpiration */  Cache.NoSlidingExpiration,
                /* priority */           CacheItemPriority.NotRemovable,
                /* onRemoveCallback */   null);
        }

        public static List<String> WordList
        {
            get
            {
                List<String> words = new List<String>();
                if (HttpRuntime.Cache["ShipDictonary"] == null)
                {
                    LoadStaticCache();
                    words = HttpRuntime.Cache["ShipDictonary"] as List<String>;

                }
                else
                    words = HttpRuntime.Cache["ShipDictonary"] as List<String>;

                return words;
            }
        }


        // Get words  - cache using the data cache
        public static List<String> loadWrod()
        {
            List<string> lines = new List<string>();
            var serverPath = System.Web.Hosting.HostingEnvironment.MapPath("~/fulldictionary.txt");
            using (StreamReader r = File.OpenText(serverPath))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    // "line" is a line in the file. Add it to our List if the word length is gt>3
                    if (line.Length > 3)
                        lines.Add(line);
                }
            }
            return lines;
        }
    }
}