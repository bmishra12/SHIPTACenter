using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ComponentModel;
using Newtonsoft.Json;

namespace UmbracoShipTac.Code
{
    public static class Extensions
    {

        /// <summary>
        /// Descriptions the specified @enum.
        /// </summary>
        /// <param name="enum">The @enum to get a description for.</param>
        /// <returns></returns>
        public static string Description(this Enum @enum)
        {
            var value = @enum.ToString();
            var type = @enum.GetType();

            var descAttribute =
                (DescriptionAttribute[])type.GetField(value).GetCustomAttributes(typeof(DescriptionAttribute), false);

            return descAttribute.Length > 0 ? descAttribute[0].Description : value;
        }


        public static void AddNewHtmlLines(this StringBuilder builder, Int16 NumberOfLines)
        {
            string line = "<BR />";
            for (int i = 0; i < NumberOfLines; i++)
                builder.Append(line);
        }


        /// <summary>
        /// Add one html new line tag <BR />
        /// </summary>
        /// <param name="builder"></param>
        public static void AddNewHtmlLine(this StringBuilder builder)
        {
            string line = "<BR />";
            builder.Append(line);
        }

        public static string ToCamelCasing(this string input)// I think you mean Pascal Case.
        {
            //return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(input);
            if (!string.IsNullOrEmpty(input))
                return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(input);
            else
                return string.Empty;
        }





    }
}