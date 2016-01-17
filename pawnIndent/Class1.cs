using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pawnIndent
{
    public class pawn
    {
        public static List<string> temp = new List<string>();
        public static int tabSize = 0;
        public static int transformIndex = 0;
        public static int slash = 0;

        public static int bracketLevel = 0;

        public enum Level
        {
            None,
            Comment,    // /**/
            Bracket,    // ()
            Quote,      // ""
            CharQuote   // ''
        };

        public static Level blockLevel = Level.None;
        public static bool quoteFix = false; // no quote in bracket

        public static List<string> transform(List<string> pawnSource)
        {
            temp = pawnSource;
            int linesBefore = temp.Count;
            long timer = DateTime.Now.Ticks;

            for (int transformIndex = 0; ; transformIndex++)
            {
                if (transformIndex == temp.Count)
                    break;

                string line = (blockLevel == Level.None ? temp[transformIndex].Trim() : temp[transformIndex]); // remove spaces

                if (line.Length == 0 && blockLevel == Level.None) // empty lines
                {
                    temp.RemoveAt(transformIndex); // delete this line
                    transformIndex--;
                    continue;
                }

                if (line == "{")
                {
                    if (blockLevel == Level.None)
                    {
                        if (tabSize > 0)
                            line = "".PadLeft(tabSize * 4) + line;

                        tabSize++;
                    }
                }
                else if (line == "}")
                {
                    if (blockLevel == Level.None)
                    {
                        if (--tabSize > 0)
                            line = "".PadLeft(tabSize * 4) + line;
                    }
                }
                else
                {
                    line = deformLine(transformIndex, line);
                    if (line.Trim().Length == 0)
                    {
                        temp.RemoveAt(transformIndex); // delete this line
                        transformIndex--;
                        continue;
                    }
                    line = "".PadLeft(tabSize * 4) + line;
                }
                temp[transformIndex] = line;
            }
            temp.Insert(0, "// -- Elapsed time: " + new TimeSpan(DateTime.Now.Ticks - timer));
            temp.Insert(0, "// -- Total lines. Before: " + linesBefore + ", After: " + (temp.Count + 2));
            temp.Insert(0, "// -- application by Seregamil");
            return temp;
        }

        public static string deformLine(int lineID, string line)
        {
            string result = line;
            for (int index = 0; index != result.Length; index++)
            {
                char key = result[index];

                #region '' | ""
                if (key == '\\')
                {
                    if (blockLevel == Level.CharQuote || blockLevel == Level.Quote)
                    {
                        slash = index;
                        continue;
                    }
                }

                if (key == '\'')
                {
                    if (blockLevel == Level.None)
                    {
                        blockLevel = Level.CharQuote;
                        continue;
                    }

                    if (blockLevel == Level.CharQuote)
                    {
                        blockLevel = Level.None;
                        continue;
                    }

                    if (index - slash == 1)
                    {
                        continue;
                    }
                }

                if (key == '"')
                {
                    if (blockLevel == Level.None)
                    {
                        blockLevel = Level.Quote;
                        continue;
                    }

                    if (blockLevel == Level.Quote)
                    {
                        blockLevel = Level.None;
                        continue;
                    }

                    if (index - slash == 1)
                    {
                        continue;
                    }

                    if (blockLevel == Level.Bracket)
                    {
                        quoteFix = !quoteFix; // change level
                        continue;
                    }
                }

                #endregion

                #region ()
                if (key == '(')
                {
                    if (blockLevel == Level.Bracket)
                    {
                        if (!quoteFix) bracketLevel++;
                        continue;
                    }

                    if (blockLevel == Level.None) // if level == none
                    {
                        blockLevel = Level.Bracket; // set bracket level
                        bracketLevel++;
                        continue;
                    }
                }

                if (key == ')')
                {
                    if (blockLevel == Level.Bracket && !quoteFix) // if blocklevel == bracket
                    {
                        if (--bracketLevel == 0)
                        {
                            blockLevel = Level.None; // disable blocking
                            continue;
                        }
                    }
                }
                #endregion

                #region /* | //
                if (key == '/')
                {
                    if (blockLevel == Level.None)
                    {
                        if (index != result.Length - 1)
                        { // no end of line
                            index++; // add index
                            key = result[index]; // get next char
                            if (key == '/')
                            {
                                break; // end of line
                            }

                            if (key == '*')
                            {
                                blockLevel = Level.Comment;
                                continue;
                            }
                        }
                    }
                }

                if (key == '*')
                {
                    if (blockLevel == Level.Comment)
                    {
                        if (index != result.Length - 1)
                        { // no end of line
                            index++; // add index
                            key = result[index]; // get next char
                            if (key == '/')
                            {
                                blockLevel = Level.None;
                                continue;
                            }
                        }
                    }
                }
                #endregion

                #region {}
                if (blockLevel == Level.None)
                {
                    if (key == '{')
                    {
                        if (index == result.Length - 1) // end of line
                        {
                            result = result.Remove(index);
                            temp.Insert(lineID + 1, "{");
                        }
                        else
                        {
                            result = result.Remove(index, 1); // remove char
                            string next = result.Substring(index); // get sub line
                            result = result.Remove(index); // remove all line
                            temp.Insert(lineID + 1, "{"); // insert char
                            temp.Insert(lineID + 2, next); // insert saved
                        }
                        break;
                    }

                    if (key == '}')
                    {
                        if (index == result.Length - 1) // end of line
                        {
                            result = result.Remove(index);
                            temp.Insert(lineID + 1, "}");
                        }
                        else
                        {
                            result = result.Remove(index, 1); // remove char
                            string next = result.Substring(index); // get sub line
                            result = result.Remove(index); // remove all line
                            temp.Insert(lineID + 1, "}"); // insert char
                            temp.Insert(lineID + 2, next); // insert saved
                        }
                        break;
                    }
                }
                #endregion

                #region ;
                if (key == ';')
                {
                    if (index != result.Length - 1)
                    {
                        if (blockLevel == Level.None)
                        {
                            string next = result.Substring(index + 1); // get sub line
                            result = result.Remove(index + 1); // remove all line
                            temp.Insert(lineID + 1, next); // insert saved
                            break;
                        }
                    }
                }
                #endregion
            }

            return result;
        }
    }
}
