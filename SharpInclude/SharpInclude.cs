/*<# GenerationEnvironment.Length -= 2;/*/using/*/#><#@ import namespace="/**/System.Collections.Generic;//"#>
/*<# GenerationEnvironment.Length -= 2;/*/using/*/#><#@ import namespace="/**/System.Collections.Specialized;//"#>
/*<# GenerationEnvironment.Length -= 2;/*/using/*/#><#@ import namespace="/**/System.IO;//"#>
/*<# GenerationEnvironment.Length -= 2;/*/using/*/#><#@ import namespace="/**/System.Text;//"#>
/*<# GenerationEnvironment.Length -= 2;/*/using/*/#><#@ import namespace="/**/System.Text.RegularExpressions;//"#>
/*<# GenerationEnvironment.Length -= 2;#>
<#@ template debug="true" hostspecific="true" language="C#" #>
<#
    //<--
    SharpInclude.Initialize(Host);
    //-->
    SharpIncludeSelfInit();
//*///#><#+/*
#if DEBUG
using System;

class SharpIncludeTemplate : Microsoft.VisualStudio.TextTemplating.TextTransformation
{
#pragma warning disable 0169, 0649
    Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host;
#pragma warning restore 0169, 0649

    public override string TransformText()
    {
        return "";
    }
//*///#><#+
    //<++
    static class SharpInclude
    {
        public static string SharpIncludeStartDelim = @"//<(\+\++|--+|\*\*+|@@+)";
        public static string SharpIncludeEndDelim = @"//(\+\++|--+|\*\*+|@@+)>";

        public static string StartDelim = "<" + "#";
        public static string EndDelim = "#" + ">";
        public static string StartFeatureDelim = StartDelim + "+";
        public static string EndFeatureDelim = EndDelim;
        public static string StartExpressionDelim = StartDelim + "=";

        public static string EnableCommentStartDelim = "/*#";
        public static string EnableCommentEndDelim = "#*/";
        public static string EnableStartDelim = "<*#";
        public static string EnableEndDelim = "#*>";

        public static string NewLine = Environment.NewLine;

        public static Regex SliceBlockRegex = new Regex(SharpIncludeStartDelim + @"[\*\+\-\$]?(\S*)(\r\n|\r|\n)([\s\S]*?)(\r\n|\r|\n)[ \t]*" + SharpIncludeEndDelim);
        public static Regex UsingRegex = new Regex(@".*(using)([ \t]+|.*/\*\*/[ \t]*)+([_a-zA-z][_a-zA-Z0-9\.]*)[ \t#\*/>""]*;");

        public static Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host;

        public static void Initialize(Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost BaseHost)
        {
            SharpInclude.Host = BaseHost;
            Environment.CurrentDirectory = Path.GetDirectoryName(Host.TemplateFile);
        }

        public static string Include()
        {
            return Include("");
        }

        public static string Include(string file)
        {
            return Include(file, "");
        }

        public static string Include(string file, string pos)
        {
            return Include(file, pos, true);
        }

        public static string Include(string file, bool inusing)
        {
            return Include(file, "", inusing);
        }

        public static string Include(string file, string pos, bool inusing)
        {
            return IncludeFile(file, pos, inusing, false);
        }

        public static string IncludeList(params string[] files)
        {
            StringBuilder usingsrc = new StringBuilder();
            StringBuilder blocksrc = new StringBuilder();

            for (int i = 0; i < files.Length; i++)
            {
                string src = File.ReadAllText(files[i]);
                usingsrc.Append(src + NewLine);
                blocksrc.Append(src + NewLine);
            }

            return GetUsingString(usingsrc.ToString(), true) + IncludeString(blocksrc.ToString(), "", false, false);
        }

        public static string IncludeListPos(NameValueCollection files)
        {
            StringBuilder usingsrc = new StringBuilder();
            StringBuilder blocksrc = new StringBuilder();

            foreach (string key in files.AllKeys)
            {
                foreach (string val in files.GetValues(key))
                {
                    string src = File.ReadAllText(key);
                    usingsrc.Append(src + NewLine);
                    blocksrc.Append(GetPos(src, val) + NewLine);
                }
            }

            return GetUsingString(usingsrc.ToString(), true) + IncludeString(blocksrc.ToString(), "", false, false);
        }

        public static string Mixin(string file)
        {
            return Mixin(file, "");
        }

        public static string Mixin(string file, string pos)
        {
            return IncludeFile(file, pos, false, true);
        }

        public static string MixinList(params string[] files)
        {
            StringBuilder blocksrc = new StringBuilder();
            for (int i = 0; i < files.Length; i++)
            {
                blocksrc.Append(IncludeFile(files[i], "", false, true) + NewLine);
            }
            return blocksrc.ToString();
        }

        public static string MixinListPos(NameValueCollection files)
        {
            StringBuilder blocksrc = new StringBuilder();
            Environment.CurrentDirectory = Path.GetDirectoryName(Host.TemplateFile);
            foreach (string key in files.AllKeys)
            {
                foreach (string val in files.GetValues(key))
                {
                    string src = File.ReadAllText(key);
                    blocksrc.Append(GetPos(src, val) + NewLine);
                }
            }
            return IncludeString(blocksrc.ToString(), "", false, true);
        }

        public static string IncludeFile(string file, string pos, bool inusing, bool mixin)
        {
            return IncludeString(GetFile(file), pos, inusing, mixin);
        }

        public static string IncludeString(string src, string pos, bool inusing, bool mixin)
        {
            StringBuilder ret = new StringBuilder();
            
            string possrc;

            possrc = pos.Length > 0 ? GetPos(src, pos) : src;

            if (!mixin) IncludeDataList.Initialize();

            Match match = SliceBlockRegex.Match(possrc);
            while (match.Success)
            {
                if (!mixin)
                {
                    IncludeDataList.AddData(match.Groups[1].ToString()[0], match.Groups[4].ToString());
                }
                else
                {
                    ret.Append(NewLine + match.Groups[4].ToString() + NewLine);
                }
                match = match.NextMatch();
            }

            if (!mixin) ret.Append(IncludeDataList.Generate());

            if (inusing)
            {
                ret.Insert(0, GetUsingString(src, true));
            }

            return ret.ToString();
        }

        public static string GetPos(string src, string pos)
        {
            StringBuilder ret = new StringBuilder();
            if (pos.Length > 0 && pos[0] == '$')
            {
                string cls = GetClassSource.GetClass(src, pos);
                cls = Regex.Replace(cls, @"[ \t]*" + SharpIncludeStartDelim + @"(\r\n|\r|\n)", "");
                cls = Regex.Replace(cls, @"(\r\n|\r|\n)[ \t]*" + SharpIncludeEndDelim + @"(\r\n|\r|\n)", "");
                ret.Append(cls);
                ret.Insert(0, "//<++" + NewLine);
                ret.Append(NewLine + "//++>");
            }
            else
            {
                char postype;
                string posname;

                if (pos.Length > 0 && (pos[0] == '*' || pos[0] == '-' || pos[0] == '+'))
                {
                    postype = pos[0];
                    posname = pos.Substring(1);
                }
                else
                {
                    postype = ' ';
                    posname = pos;
                }

                Match match = SliceBlockRegex.Match(src);
                while (match.Success)
                {
                    Group name = match.Groups[2];
                    Group type = match.Groups[1];
                    if (posname.Length == 0 || posname.Equals(name.ToString()))
                    {
                        if (postype == ' ' || postype == type.ToString()[0])
                        {
                            ret.Append(match.Groups[0].ToString() + NewLine);
                            if (posname.Length > 0) break;
                        }
                    }
                    match = match.NextMatch();
                }
            }
            return ret.ToString();
        }

        public static string GetUsingString(string src, bool isimport)
        {
            StringBuilder ret = new StringBuilder();
            List<string> implist = new List<string>();
            if(isimport) implist.Add("System");
            Match match = UsingRegex.Match(src);
            bool init = true;
            while (match.Success)
            {
                Group grp = match.Groups[3];
                if (implist.IndexOf(grp.ToString()) == -1)
                {
                    ret.Append(init ? "" : NewLine);
                    init = false;
                    if (isimport)
                    {
                        ret.Append(StartDelim + "@ import namespace=\"" + grp.ToString() + "\" " + EndDelim);
                    }
                    else
                    {
                        ret.Append(match.Groups[0].ToString());
                    }
                    implist.Add(grp.ToString());
                }
                match = match.NextMatch();
            }
            return ret.ToString() + NewLine;
        }

        public static string GetUsingFile(string file, bool isimport)
        {
            return GetUsingString(GetFile(file), isimport);
        }

        public static string GetFile(string file)
        {
            if (file.Length == 0) file = Host.TemplateFile;
            return File.ReadAllText(file);
        }

        static class GetClassSource 
        {
            public static Regex SliceClassRegex = new Regex(@"(([ \t]*\[[^\]]*\]\s+)*[\sa-z]*class\s+([a-zA-Z_][0-9a-zA-Z_]*)([\s<][^{]*)?)({[\s\S]*})");

            public static string GetClass(string src, string name)
            {
                StringBuilder ret = new StringBuilder();
                int con = 0;
                bool init = true;

                Match match = SliceClassRegex.Match(src);
                while (match.Success)
                {
                    string cls = SearchBraces(match.Groups[5].ToString());
                    con = match.Index + match.Groups[1].ToString().Length + (name.Length > 0 ? cls.Length : 0);
                    if (name.Length == 0 || name.Equals(match.Groups[3].ToString()))
                    {
                        ret.Append(init ? "" : NewLine);
                        ret.Append(match.Groups[1].ToString());
                        ret.Append(cls);
                        if (name.Length > 0) break;
                        init = false;
                    }
                    match = SliceClassRegex.Match(src, con);
                }

                return ret.ToString();
            }

            public static string SearchBraces(string src)
            {
                int indent = 0;
                bool isAsterisk = false;
                bool isSlash = false;
                bool isBlockComment = false;
                bool isLineComment = false;
                bool isDoubleQuote = false;
                bool isSingleQuote = false;
                bool isBackSlash = false;

                for (int i = 0; i < src.Length; i++)
                {
                    switch (src[i])
                    {
                        case '*':
                            if (isSlash && !isBlockComment && !isLineComment && !isDoubleQuote && !isSingleQuote)
                            {
                                isSlash = false;
                                isBlockComment = true;
                            }
                            else
                            {
                                isAsterisk = true;
                            }
                            break;
                        case '/':
                            if (isAsterisk && isBlockComment && !isLineComment && !isDoubleQuote && !isSingleQuote)
                            {
                                isAsterisk = false;
                                isBlockComment = false;
                            }
                            else if (isSlash && !isBlockComment && !isDoubleQuote && !isSingleQuote)
                            {
                                isSlash = false;
                                isLineComment = true;
                            }
                            else
                            {
                                isSlash = true;
                            }
                            break;
                        case '\r':
                        case '\n':
                            isLineComment = false;
                            break;
                        case '"':
                            if (!isBlockComment && !isLineComment && !isBackSlash)
                            {
                                if (!isDoubleQuote)
                                {
                                    isDoubleQuote = true;
                                }
                                else
                                {
                                    isDoubleQuote = false;
                                }
                            }
                            else if (isBackSlash)
                            {
                                isBackSlash = false;
                            }
                            break;
                        case '\'':
                            if (!isBlockComment && !isLineComment && !isBackSlash)
                            {
                                if (!isSingleQuote)
                                {
                                    isSingleQuote = true;
                                }
                                else
                                {
                                    isSingleQuote = false;
                                }
                            }
                            else if (isBackSlash)
                            {
                                isBackSlash = false;
                            }
                            break;
                        case '\\':
                            if (!isBlockComment && !isLineComment && (isDoubleQuote || isSingleQuote))
                            {
                                if (!isBackSlash)
                                {
                                    isBackSlash = true;
                                }
                                else
                                {
                                    isBackSlash = false;
                                }
                            }
                            break;
                        case '{':
                            if (!isBlockComment && !isLineComment && !isDoubleQuote && !isSingleQuote)
                            {
                                indent++;
                            }
                            break;
                        case '}':
                            if (!isBlockComment && !isLineComment && !isDoubleQuote && !isSingleQuote)
                            {
                                indent--;
                                if (indent <= 0)
                                {
                                    return src.Substring(0, i + 1);
                                }
                            }
                            break;
                        default:
                            isAsterisk = false;
                            isSlash = false;
                            isBackSlash = false;
                            break;
                    }
                }
                return src;
            }
        }

        public static class IncludeDataList
        {
            static List<IncludeData> DataList = new List<IncludeData>();
            static List<StringBuilder> SourceData = new List<StringBuilder>();

            static IncludeDataList()
            {
                int max = 0;
                foreach (int val in IncludeData.SourceIndex.Values)
                {
                    max = val > max ? val : max;
                }
                for (int i = 0; i <= max; i++)
                {
                    SourceData.Add(new StringBuilder());
                }
            }

            public static void Initialize()
            {
                DataList.Clear();
                for (int i = 0; i < SourceData.Count; i++)
                {
                    SourceData[i].Clear();
                }
            }

            public static void AddData(char type, string src)
            {
                IncludeData item = new IncludeData();
                item.SourceType = type;
                item.SourceData = src;
                DataList.Add(item);
            }

            public static string Convert(IncludeData data)
            {
                if (IncludeData.ConvertFunc.ContainsKey(data.SourceType))
                {
                    return IncludeData.ConvertFunc[data.SourceType](data.SourceData);
                }
                return data.SourceData;
            }


            public static string Generate()
            {
                StringBuilder ret = new StringBuilder();

                foreach (IncludeData item in DataList)
                {
                    SourceData[IncludeData.SourceIndex[item.SourceType]].Append(Convert(item));
                }
                for (int i = 0; i < SourceData.Count; i++)
                {
                    ret.Append(SourceData[i].ToString());
                }

                return ret.ToString();
            }

            public class IncludeData
            {
                public Char SourceType;
                public String SourceData;

                public static Dictionary<char, Func<string, string>> ConvertFunc = new Dictionary<char, Func<string, string>>
                {
                    {
                        '@', delegate(string src)
                        {
                            return src; 
                        }
                    },
                    {
                        '*', delegate(string src)
                        {
                            string repstring = src;
                            repstring = Regex.Replace(repstring, Regex.Escape(EnableCommentEndDelim) + @"[_a-zA-Z][_\.0-9a-zA-Z]+", EnableCommentEndDelim);
                            repstring = repstring.Replace(EnableCommentStartDelim, StartDelim);
                            repstring = repstring.Replace(EnableCommentEndDelim, EndDelim);
                            repstring = repstring.Replace(EnableStartDelim, StartDelim);
                            repstring = repstring.Replace(EnableEndDelim, EndDelim);
                            return repstring;
                         }
                    },
                    {
                        '-', delegate(string src) 
                        {
                            return StartDelim + NewLine + src + NewLine + EndDelim + NewLine;
                        }
                    },
                    {
                        '+', delegate(string src) 
                        {
                            return StartFeatureDelim + NewLine + src + NewLine + EndFeatureDelim + NewLine;
                        }
                    },
                };

                public static Dictionary<char, int> SourceIndex = new Dictionary<char, int>
                {
                    {'@', 0},
                    {'*', 0},
                    {'-', 0},
                    {'+', 1},
                };
            }
        }

    }

    void ClearAndWrite(string str)
    {
        GenerationEnvironment.Length = 0;
        Write(str);
    }
    //++>

    void SharpIncludeSelfInit()
    {
        if (Path.GetFileName(Host.TemplateFile).Equals("SharpInclude.cs"))
        {
            Host.SetFileExtension(".t4");
            ClearAndWrite(SharpInclude.Include());
        }
    }
//*///#><#+/*
}
#endif
//*///#>