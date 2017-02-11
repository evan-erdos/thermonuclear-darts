/* Ben Scott * @evan-erdos * bescott@andrew.cmu.edu * 2015-07-16 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

public static class Markdown {
    public static string EmptyElementSuffix {get;set;} = " />";
    public static bool LinkEmails {get;set;}
    public static bool StrictBoldItalic {get;set;} = true;
    public static bool AsteriskIntraWordEmphasis {get;set;}
    public static bool AutoNewlines {get;set;}
    public static bool AutoHyperlink {get;set;}


    enum TokenType { Text, Tag }

    struct Token {
        public TokenType Type;
        public string Value;

        public Token(TokenType type, string value) {
            this.Type = type;
            this.Value = value;
        }
    }

    const int nestDepth = 6;
    const int tabWidth = 4;
    const string markerUL = @"[*+-]";
    const string markerOL = @"\d+[.]";
    const string charInsideUrl = @"[-A-Z0-9+&@#/%?=~_|\[\]\(\)!:,\.;\x1a]";
    const string charEndingUrl = "[-A-Z0-9+&@#/%=~_|\\[\\])]";
    static readonly Map<string> escapeTable = new Map<string>();
    static readonly Map<string> invertedEscapeTable = new Map<string>();
    static readonly Map<string> backslashEscapeTable = new Map<string>();
    static readonly Map<string> urls = new Map<string>();
    static readonly Map<string> titles = new Map<string>();
    static readonly Map<string> htmlBlocks = new Map<string>();

    static int listLevel;
    static string AutoLinkPreventionMarker = "\x1AP";
    static Regex newlinesLeadingTrailing = new Regex(@"^\n+|\n+\z");
    static Regex newlinesMultiple = new Regex(@"\n{2,}");
    static Regex leadingWhitespace = new Regex(@"^[ ]*");
    static Regex htmlBlockHash = new Regex("\x1AH\\d+H");

    static string nestedBracketsPattern;
    static string nestedParensPattern;

    static Regex linkDef = new Regex(string.Format(
        @"^[ ]{{0,{0}}}\[([^\[\]]+)\]:  # id = $1
        [ ]*\n?[ ]*<?(\S+?)>?[ ]*\n?[ ]*(?:
        (?<=\s)[""(](.+?)["")][ ]*)?(?:\n+|\Z)", tabWidth-1),
        RegexOptions.Multiline |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex blocksHtml = new Regex(GetBlockPattern(),
        RegexOptions.Multiline |
        RegexOptions.IgnorePatternWhitespace);

    static Regex htmlTokens = new Regex(
        @"(<!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->)|(<\?.*?\?>)|"
        + RepeatString(@"(<[A-Za-z\/!$](?:[^<>]|",nestDepth)
        + RepeatString(@")*>)",nestDepth),
        RegexOptions.Multiline |
        RegexOptions.Singleline |
        RegexOptions.ExplicitCapture |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex anchorRef = new Regex(
        $@"(\[ ({GetNestedBracketsPattern()}) \][ ]?
        (?:\n[ ]*)?\[(.*?)\])",
        RegexOptions.IgnorePatternWhitespace);

    static Regex anchorInline = new Regex(
        $@"(\[ ({GetNestedBracketsPattern()}) \]
        \( [ ]* ({GetNestedParensPattern()}) [ ]*
        ((['""]) (.*?) \5 [ ]* )? \))",
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex anchorRefShortcut = new Regex(
        @"(\[ ([^\[\]]+) \])",
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex imagesRef = new Regex(
        @"(!\[(.*?)\][ ]?(?:\n[ ]*)?\[(.*?)\])",
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex imagesInline = new Regex(
        @"(!\[(.*?)\]\s?
        \([ ]* ({GetNestedParensPattern()}) [ ]*
        ((['""])(.*?)\5[ ]*)?\))",
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex headerSetext = new Regex(
        @"(.+?)[ ]*\n(=+|-+)[ ]*\n+",
        RegexOptions.Multiline |
        RegexOptions.IgnorePatternWhitespace);

    static Regex headerAtx = new Regex(
        @"(\#{1,6}) [ ]* (.+?) [ ]* \#* \n+",
        RegexOptions.Multiline |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex horizontalRules = new Regex(
        @"^[ ]{0,3}([-*_])(?>[ ]{0,2}\1){2,}[ ]*$",
        RegexOptions.Multiline |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static string wholeList = string.Format(
        @"(([ ]{{0,{1}}}({0})[ ]+)(?s:.+?)
        (\z|\n{{2,}}(?=\S)(?![ ]*{0}[ ]+)))",
        string.Format("(?:{0}|{1})", markerUL, markerOL), tabWidth - 1);

    static Regex listNested = new Regex(
        $@"^{wholeList}",
        RegexOptions.Multiline |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex listTopLevel = new Regex(
        $@"(?:(?<=\n\n)|\A\n?){wholeList}",
        RegexOptions.Multiline |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex codeBlock = new Regex(string.Format(
        @"(?:\n\n|\A\n?)((?:(?:[ ]{{{0}}}).*\n+)+)
        ((?=^[ ]{{0,{0}}}[^ \t\n])|\Z)",tabWidth),
        RegexOptions.Multiline |
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Compiled);

    static Regex codeSpan = new Regex(
        @"(?<![\\`])(`+)(?!`)(.+?)(?<!`)\1(?!`)",
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Singleline |
        RegexOptions.Compiled);

    static Regex bold = new Regex(
        @"(\*\*|__) (?=\S) (.+?[*_]*) (?<=\S) \1",
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Singleline |
        RegexOptions.Compiled);

    static Regex semiStrictBold = new Regex(
        @"(?=.[*_]|[*_])(^|(?=\W__|(?!\*)[\W_]\*\*|\w\*\*\w).)(\*\*|__)(?!\2)(?=\S)((?:|.*?(?!\2).)(?=\S_|\w|\S\*\*(?:[\W_]|$)).)(?=__(?:\W|$)|\*\*(?:[^*]|$))\2",
        RegexOptions.Singleline |
        RegexOptions.Compiled);

    static Regex strictBold = new Regex(
        @"(^|[\W_])(?:(?!\1)|(?=^))(\*|_)\2(?=\S)(.*?\S)\2\2(?!\2)(?=[\W_]|$)",
        RegexOptions.Singleline |
        RegexOptions.Compiled);

    static Regex italic = new Regex(
        @"(\*|_) (?=\S) (.+?) (?<=\S) \1",
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Singleline |
        RegexOptions.Compiled);

    static Regex semiStrictItalic = new Regex(
        @"(?=.[*_]|[*_])(^|(?=\W_|(?!\*)(?:[\W_]\*|\D\*(?=\w)\D)).)(\*|_)(?!\2\2\2)(?=\S)((?:(?!\2).)*?(?=[^\s_]_|(?=\w)\D\*\D|[^\s*]\*(?:[\W_]|$)).)(?=_(?:\W|$)|\*(?:[^*]|$))\2",
        RegexOptions.Singleline |
        RegexOptions.Compiled);

    static Regex strictItalic = new Regex(
        @"(^|[\W_])(?:(?!\1)|(?=^))(\*|_)(?=\S)((?:(?!\2).)*?\S)\2(?!\2)(?=[\W_]|$)",
        RegexOptions.Singleline |
        RegexOptions.Compiled);

    static Regex blockquote = new Regex(
        @"((^[ ]*>[ ]?.+\n(.+\n)*\n*)+)",
        RegexOptions.IgnorePatternWhitespace |
        RegexOptions.Multiline |
        RegexOptions.Compiled);

    static Regex autolinkBare = new Regex(
        $@"(<|="")?\b(https?|ftp)(://{charInsideUrl}*{charEndingUrl})(?=$|\\W)",
        RegexOptions.IgnoreCase |
        RegexOptions.Compiled);

    static Regex endCharRegex = new Regex(
        charEndingUrl,
        RegexOptions.IgnoreCase |
        RegexOptions.Compiled);

    static Regex outDent = new Regex(
        @"^[ ]{1,"+tabWidth+@"}",
        RegexOptions.Multiline);

    static Regex codeEncoder = new Regex(@"&|<|>|\\|\*|_|\{|\}|\[|\]");


    static Markdown() {
        var backslashPattern = "";
        foreach (var c in @"\`*_{}[]()>#+-.!/:") {
            var key = c.ToString();
            var hash = GetHashKey(key, isHtmlBlock: false);
            escapeTable.Add(key, hash);
            invertedEscapeTable.Add(hash, key);
            backslashEscapeTable.Add($@"\{key}", hash);
            backslashPattern += Regex.Escape($@"\{key}")+"|";
        } backslashEscapes = new Regex(
            backslashPattern.Substring(0, backslashPattern.Length-1));
    }

    public static string Transform(string text) {
        if (string.IsNullOrEmpty(text)) return "";
        Setup();
        text = Normalize(text);
        text = HashHTMLBlocks(text);
        text = StripLinkDefinitions(text);
        text = RunBlockGamut(text);
        text = Unescape(text);
        Cleanup();
        return $"{text}\n";
    }

    static string RunBlockGamut(
                    string text,
                    bool unhash=true,
                    bool createParagraphs=false) {
        text = DoHeaders(text);
        text = DoHorizontalRules(text);
        text = DoLists(text);
        text = DoCodeBlocks(text);
        text = DoBlockQuotes(text);
        text = HashHTMLBlocks(text);
        text = FormParagraphs(
            text: text,
            unhash: unhash,
            createParagraphs: createParagraphs);
        return text;
    }

    static string RunSpanGamut(string text) {
        text = DoCodeSpans(text);
        text = EscapeSpecialCharsWithinTagAttributes(text);
        text = EscapeBackslashes(text);
        text = DoImages(text);
        text = DoAnchors(text);
        text = DoAutoLinks(text);
        text = text.Replace(AutoLinkPreventionMarker,"://");
        text = EncodeAmpsAndAngles(text);
        text = DoItalicsAndBold(text);
        text = DoHardBreaks(text);
        return text;
    }

    static string FormParagraphs(
                    string text,
                    bool unhash=true,
                    bool createParagraphs=false) {
        var grafs = newlinesMultiple.Split(
            newlinesLeadingTrailing.Replace(text,""));
        for (var i=0;i<grafs.Length;i++){
            if (grafs[i].Contains("\x1AH")) {
                if (unhash) {
                    var sanityCheck = 50;
                    var keepGoing = true;
                    while (keepGoing && sanityCheck>0) {
                        keepGoing = false;
                        grafs[i] = htmlBlockHash.Replace(grafs[i], match => {
                            keepGoing = true;
                            return htmlBlocks[match.Value];
                        });
                        sanityCheck--;
                    }
                }
            } else grafs[i] = leadingWhitespace.Replace(
                RunSpanGamut(grafs[i]),
                createParagraphs ?"<p>":"")+(createParagraphs?"</p>":"");
        } return string.Join("\n\n", grafs);
    }


    static void Setup() {
        urls.Clear();
        titles.Clear();
        htmlBlocks.Clear();
        listLevel = 0;
    }

    static void Cleanup() => Setup();

    static string GetNestedBracketsPattern() {
        if (nestedBracketsPattern == null)
            nestedBracketsPattern =
                RepeatString(@"
                (?>              # Atomic matching
                   [^\[\]]+      # Anything other than brackets
                 |
                   \[
                       ", nestDepth) + RepeatString(
                @" \]
                )*", nestDepth);
        return nestedBracketsPattern;
    }


    static string GetNestedParensPattern() {
        if (nestedParensPattern == null)
            nestedParensPattern =
                RepeatString(@"
                (?>              # Atomic matching
                   [^()\s]+      # Anything other than parens or whitespace
                 |
                   \(
                       ", nestDepth) + RepeatString(
                @" \)
                )*", nestDepth);
        return nestedParensPattern;
    }


    static string StripLinkDefinitions(string text) =>
        linkDef.Replace(text, new MatchEvaluator(LinkEvaluator));

    static string LinkEvaluator(Match match) {
        var linkID = match.Groups[1].Value.ToLowerInvariant();
        urls[linkID] = EncodeAmpsAndAngles(match.Groups[2].Value);
        if (match.Groups[3] != null && match.Groups[3].Length > 0)
            titles[linkID] = match.Groups[3].Value.Replace("\"", "&quot;");
        return "";
    }


    static string GetBlockPattern() {
        var blockTagsA = "ins|del";
        var blockTagsB = "p|div|h[1-6]|blockquote|pre|table|dl|ol|ul|address|script|noscript|form|fieldset|iframe|math";
        var attr = @"
        (?>                         # optional tag attributes
          \s                        # starts with whitespace
          (?>
            [^>""/]+                # text outside quotes
          |
            /+(?!>)                 # slash not followed by >
          |
            ""[^""]*""              # text inside double quotes (tolerate >)
          |
            '[^']*'                 # text inside single quotes (tolerate >)
          )*
        )?
        ";

        var content =
            RepeatString(@"
            (?>
              [^<]+                 # content without tag
            |
              <\2                   # nested opening tag
                " + attr + @"       # attributes
              (?>
                  />
              |
                  >", nestDepth) + ".*?" +
            RepeatString(@"
                  </\2\s*>          # closing nested tag
              )
              |
              <(?!/\2\s*>           # other tags with a different name
              )
            )*", nestDepth);

        var content2 = content.Replace(@"\2", @"\3");

        var pattern = @"
        (?>
              (?>
                (?<=\n)     # Starting at the beginning of a line
                |           # or
                \A\n?       # the beginning of the doc
              )
              (             # save in $1

                # Match from `\n<tag>` to `</tag>\n`, handling nested tags
                # in between.

                    <($block_tags_b_re)   # start tag = $2
                    $attr>                # attributes followed by > and \n
                    $content              # content, support nesting
                    </\2>                 # the matching end tag
                    [ ]*                  # trailing spaces
                    (?=\n+|\Z)            # followed by a newline or end of document

              | # Special version for tags of group a.

                    <($block_tags_a_re)   # start tag = $3
                    $attr>[ ]*\n          # attributes followed by >
                    $content2             # content, support nesting
                    </\3>                 # the matching end tag
                    [ ]*                  # trailing spaces
                    (?=\n+|\Z)            # followed by a newline or end of document

              | # Special case just for <hr />. It was easier to make a special
                # case than to make the other regex more complicated.

                    [ ]{0,$less_than_tab}
                    <hr
                    $attr                 # attributes
                    /?>                   # the matching end tag
                    [ ]*
                    (?=\n{2,}|\Z)         # followed by a blank line or end of document

              | # Special case for standalone HTML comments:

                  (?<=\n\n|\A)            # preceded by a blank line or start of document
                  [ ]{0,$less_than_tab}
                  (?s:
                    <!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->
                  )
                  [ ]*
                  (?=\n{2,}|\Z)            # followed by a blank line or end of document

              | # PHP and ASP-style processor instructions (<? and <%)

                  [ ]{0,$less_than_tab}
                  (?s:
                    <([?%])                # $4
                    .*?
                    \4>
                  )
                  [ ]*
                  (?=\n{2,}|\Z)            # followed by a blank line or end of document

              )
        )";

        pattern = pattern.Replace("$less_than_tab", (tabWidth-1).ToString());
        pattern = pattern.Replace("$block_tags_b_re", blockTagsB);
        pattern = pattern.Replace("$block_tags_a_re", blockTagsA);
        pattern = pattern.Replace("$attr", attr);
        pattern = pattern.Replace("$content2", content2);
        pattern = pattern.Replace("$content", content);
        return pattern;
    }


    static string HashHTMLBlocks(string text) =>
        blocksHtml.Replace(text, new MatchEvaluator(HtmlEvaluator));

    static string HtmlEvaluator(Match match) {
        var text = match.Groups[1].Value;
        var key = GetHashKey(text, isHtmlBlock: true);
        htmlBlocks[key] = text;
        return string.Concat("\n\n", key, "\n\n");
    }

    static string GetHashKey(string s, bool isHtmlBlock) {
        var delim = isHtmlBlock ? 'H' : 'E';
        return "\x1A" + delim +  Math.Abs(s.GetHashCode()).ToString() + delim;
    }


    static List<Token> TokenizeHTML(string text) {
        int pos = 0, tagStart = 0;
        var tokens = new List<Token>();
        foreach (Match m in htmlTokens.Matches(text)) {
            tagStart = m.Index;
            if (pos < tagStart)
                tokens.Add(new Token(
                    TokenType.Text,
                    text.Substring(pos, tagStart - pos)));
            tokens.Add(new Token(TokenType.Tag, m.Value));
            pos = tagStart + m.Length;
        } if (pos < text.Length)
            tokens.Add(new Token(
                TokenType.Text,
                text.Substring(pos, text.Length - pos)));
        return tokens;
    }


    static string DoAnchors(string text) {
        if (!text.Contains("[")) return text;
        text = anchorRef.Replace(
            text, new MatchEvaluator(AnchorRefEvaluator));
        text = anchorInline.Replace(
            text, new MatchEvaluator(AnchorInlineEvaluator));
        text = anchorRefShortcut.Replace(
            text, new MatchEvaluator(AnchorRefShortcutEvaluator));
        return text;
    }

    static string SaveFromAutoLinking(string s) =>
        s.Replace("://", AutoLinkPreventionMarker);

    static string AnchorRefEvaluator(Match match) {
        var wholeMatch = match.Groups[1].Value;
        var linkText = SaveFromAutoLinking(match.Groups[2].Value);
        var linkID = match.Groups[3].Value.ToLowerInvariant();
        var result = "";

        if (linkID == "") linkID = linkText.ToLowerInvariant();

        if (urls.ContainsKey(linkID)) {
            var url = urls[linkID];
            url = AttributeSafeUrl(url);
            result = $"<a href=\"{url}\"";
            if (titles.ContainsKey(linkID)) {
                var title = AttributeEncode(titles[linkID]);
                title = AttributeEncode(EscapeBoldItalic(title));
                result += $" title=\"{title}\"";
            } result += $">{linkText}</a>";
        } else result = wholeMatch;
        return result;
    }

    static string AnchorRefShortcutEvaluator(Match match) {
        var wholeMatch = match.Groups[1].Value;
        var linkText = SaveFromAutoLinking(match.Groups[2].Value);
        var linkID = Regex.Replace(
            linkText.ToLowerInvariant(), @"[ ]*\n[ ]*", " ");
        var result = "";

        if (urls.ContainsKey(linkID)) {
            var url = urls[linkID];
            url = AttributeSafeUrl(url);
            result = $"<a href=\"{url}\"";
            if (titles.ContainsKey(linkID)) {
                var title = AttributeEncode(titles[linkID]);
                title = EscapeBoldItalic(title);
                result += $" title=\"{title}\"";
            } result += ">{linkText}</a>";
        } else result = wholeMatch;
        return result;
    }


    static string AnchorInlineEvaluator(Match match) {
        var linkText = SaveFromAutoLinking(match.Groups[2].Value);
        var url = match.Groups[3].Value;
        var title = match.Groups[6].Value;
        var result = "";

        if (url.StartsWith("<") && url.EndsWith(">"))
            url = url.Substring(1, url.Length - 2);
        url = AttributeSafeUrl(url);
        result = $"<a href=\"{url}\"";
        if (!string.IsNullOrEmpty(title)) {
            title = AttributeEncode(title);
            title = EscapeBoldItalic(title);
            result += $" title=\"{title}\"";
        } result += $">{title}</a>";
        return result;
    }


    static string DoImages(string text) {
        if (!text.Contains("![")) return text;
        text = imagesRef.Replace(
            text, new MatchEvaluator(ImageReferenceEvaluator));
        text = imagesInline.Replace(
            text, new MatchEvaluator(ImageInlineEvaluator));
        return text;
    }

    static string EscapeImageAltText(string s) {
        s = EscapeBoldItalic(s);
        s = Regex.Replace(s, @"[\[\]()]", m => escapeTable[m.ToString()]);
        return s;
    }

    static string ImageReferenceEvaluator(Match match) {
        var wholeMatch = match.Groups[1].Value;
        var altText = match.Groups[2].Value;
        var linkID = match.Groups[3].Value.ToLowerInvariant();
        if (linkID == "") linkID = altText.ToLowerInvariant();

        if (urls.ContainsKey(linkID)) {
            var url = urls[linkID];
            string title = null;
            if (titles.ContainsKey(linkID)) title = titles[linkID];
            return ImageTag(url, altText, title);
        } else return wholeMatch;
    }

    static string ImageInlineEvaluator(Match match) {
        var alt = match.Groups[2].Value;
        var url = match.Groups[3].Value;
        var title = match.Groups[6].Value;

        if (url.StartsWith("<") && url.EndsWith(">"))
            url = url.Substring(1, url.Length-2);
        return ImageTag(url, alt, title);
    }

    static string ImageTag(string url, string altText, string title) {
        altText = EscapeImageAltText(AttributeEncode(altText));
        url = AttributeSafeUrl(url);
        var result = $"<img src=\"{url}\" alt=\"{altText}\"";
        if (!string.IsNullOrEmpty(title)) {
            title = AttributeEncode(EscapeBoldItalic(title));
            result += $" title=\"{title}\"";
        } return result + EmptyElementSuffix;
    }

    static string DoHeaders(string text) {
        text = headerSetext.Replace(
            text, new MatchEvaluator(SetextHeaderEvaluator));
        text = headerAtx.Replace(
            text, new MatchEvaluator(AtxHeaderEvaluator));
        return text;
    }

    static string SetextHeaderEvaluator(Match match) {
        var header = match.Groups[1].Value;
        var level = match.Groups[2].Value.StartsWith("=") ? 1 : 2;
        return $"<h{level}>{RunSpanGamut(header)}</h{level}>\n\n";
    }

    static string AtxHeaderEvaluator(Match match) {
        var header = match.Groups[2].Value;
        var level = match.Groups[1].Value.Length;
        return $"<h{level}>{RunSpanGamut(header)}</h{level}>\n\n";
    }

    static string DoHorizontalRules(string text) =>
        horizontalRules.Replace(text, $"<hr{EmptyElementSuffix}\n");


    static string DoLists(string text) => (listLevel > 0)
        ? listNested.Replace(text, new MatchEvaluator(ListEvaluator))
        : listTopLevel.Replace(text, new MatchEvaluator(ListEvaluator));

    static string ListEvaluator(Match match) {
        var list = match.Groups[1].Value;
        var marker = match.Groups[3].Value;
        var listType = Regex.IsMatch(marker, markerUL) ? "ul" : "ol";
        string result, start = "";
        if (listType == "ol") {
            var firstNumber = int.Parse(marker.Substring(0,marker.Length-1));
            if (firstNumber != 1 && firstNumber != 0)
                start = $" start=\"{firstNumber}\"";
        }

        result = ProcessListItems(list, listType == "ul" ? markerUL : markerOL);
        return $"<{listType}{start}>\n{result}</{listType}>\n";
    }

    static string ProcessListItems(string list, string marker) {
        listLevel++;
        list = Regex.Replace(list, @"\n{2,}\z", "\n");

        var pattern =
            $@"(^[ ]*) ({0}) [ ]+ ((?s:.+?) (\n+))
            (?= (\z | \1 ({marker}) [ ]+))";

        MatchEvaluator ListItemEvaluator = (Match match) => {
            var item = match.Groups[3].Value;
            item = RunBlockGamut(
                text: $"{Outdent(item)}\n",
                unhash: false,
                createParagraphs:false);
            return $"<li>{item}</li>\n";
        };

        list = Regex.Replace(
            list, pattern, new MatchEvaluator(ListItemEvaluator),
            RegexOptions.IgnorePatternWhitespace |
            RegexOptions.Multiline);
        listLevel--;
        return list;
    }

    static string DoCodeBlocks(string text) =>
        codeBlock.Replace(text, new MatchEvaluator(CodeBlockEvaluator));

    static string CodeBlockEvaluator(Match match) {
        var codeBlock = match.Groups[1].Value;
        codeBlock = EncodeCode(Outdent(codeBlock));
        codeBlock = newlinesLeadingTrailing.Replace(codeBlock, "");
        return $"\n\n<pre><code>{codeBlock}\n</code></pre>\n\n";
    }


    static string DoCodeSpans(string text) =>
        codeSpan.Replace(text, new MatchEvaluator(CodeSpanEvaluator));

    static string CodeSpanEvaluator(Match match) {
        var span = match.Groups[2].Value;
        span = Regex.Replace(span, @"^[ ]*", "");
        span = Regex.Replace(span, @"[ ]*$", "");
        span = EncodeCode(span);
        span = SaveFromAutoLinking(span);
        return $"<code>{span}</code>";
    }


    static string DoItalicsAndBold(string text) {
        if (!(text.Contains("*") || text.Contains("_"))) return text;
        if (StrictBoldItalic) {
            if (AsteriskIntraWordEmphasis) {
                text = semiStrictBold.Replace(text, "$1<strong>$3</strong>");
                text = semiStrictItalic.Replace(text, "$1<em>$3</em>");
            } else {
                text = strictBold.Replace(text, "$1<strong>$3</strong>");
                text = strictItalic.Replace(text, "$1<em>$3</em>");
            }
        } else {
            text = bold.Replace(text, "<strong>$2</strong>");
            text = italic.Replace(text, "<em>$2</em>");
        } return text;
    }

    static string DoHardBreaks(string text) => (AutoNewlines)
        ? Regex.Replace(text,@"\n",$"<br{EmptyElementSuffix}\n")
        : Regex.Replace(text,@" {2,}\n",$"<br{EmptyElementSuffix}\n");

    static string DoBlockQuotes(string text) =>
        blockquote.Replace(text, new MatchEvaluator(BlockQuoteEvaluator));

    static string BlockQuoteEvaluator(Match match) {
        var bq = match.Groups[1].Value;

        bq = Regex.Replace(bq, @"^[ ]*>[ ]?", "", RegexOptions.Multiline);
        bq = Regex.Replace(bq, @"^[ ]+$", "", RegexOptions.Multiline);
        bq = RunBlockGamut(bq);
        bq = Regex.Replace(bq, @"^", "  ", RegexOptions.Multiline);
        bq = Regex.Replace(bq, @"(\s*<pre>.+?</pre>)",
            new MatchEvaluator(BlockQuoteEvaluator2),
            RegexOptions.IgnorePatternWhitespace |
            RegexOptions.Singleline);

        bq = $"<blockquote>\n{bq}\n</blockquote>";
        var key = GetHashKey(bq, isHtmlBlock: true);
        htmlBlocks[key] = bq;
        return $"\n\n{key}\n\n";
    }

    static string BlockQuoteEvaluator2(Match match) =>
        Regex.Replace(
            match.Groups[1].Value,@"^  ","",
            RegexOptions.Multiline);

    static string handleTrailingParens(Match match) {
        if (match.Groups[1].Success) return match.Value;
        var protocol = match.Groups[2].Value;
        var link = match.Groups[3].Value;
        if (!link.EndsWith(")")) return $"<{protocol}{link}>";
        var level = 0;
        foreach (Match c in Regex.Matches(link, "[()]")) {
            if (c.Value == "(") {
                if (level <= 0) level = 1;
                else level++;
            } else level--;
        }
        var tail = "";
        if (level < 0)
            link = Regex.Replace(
                link, @"\){1," + (-level) + "}$",
                m => { tail = m.Value; return ""; });
        if (tail.Length > 0) {
            var lastChar = link[link.Length - 1];
            if (!endCharRegex.IsMatch(lastChar.ToString())) {
                tail = lastChar + tail;
                link = link.Substring(0, link.Length - 1);
            }
        } return $"<{protocol}{link}>{tail}";
    }


    static string DoAutoLinks(string text) {
        if (AutoHyperlink)
            text = autolinkBare.Replace(text, handleTrailingParens);
        text = Regex.Replace(
            text, "<((https?|ftp):[^'\">\\s]+)>",
            new MatchEvaluator(HyperlinkEvaluator));
        if (LinkEmails)
            text = Regex.Replace(
                text,
                @"<(?:mailto:)?([-.\w]+\@
                    [-a-z0-9]+(\.[-a-z0-9]+)*\.[a-z]+)>",
                new MatchEvaluator(EmailEvaluator),
                RegexOptions.IgnoreCase |
                RegexOptions.IgnorePatternWhitespace);
        return text;
    }

    static string HyperlinkEvaluator(Match match) {
        var link = match.Groups[1].Value;
        return $"<a href=\"{AttributeSafeUrl(link)}\">{link}</a>";
    }

    static string EmailEvaluator(Match match) {
        var email = $"mailto:{Unescape(match.Groups[1].Value)}";
        email = EncodeEmailAddress(email);
        email = $"<a href=\"{email}\">{email}</a>";
        email = Regex.Replace(email, "\">.+?:", "\">");
        return email;
    }

    static string Outdent(string block) => outDent.Replace(block, "");


    static string EncodeEmailAddress(string addr) {
        var sb = new StringBuilder(addr.Length * 5);
        var rand = new Random();
        int r;
        foreach (var c in addr) {
            r = rand.Next(1, 100);
            if ((r > 90 || c == ':') && c != '@') sb.Append(c);
            else if (r < 45) sb.AppendFormat("&#x{0:x};", (int) c);
            else sb.AppendFormat("&#{0};", (int)c);
        } return sb.ToString();
    }


    static string EncodeCode(string code) =>
        codeEncoder.Replace(code,EncodeCodeEvaluator);

    static string EncodeCodeEvaluator(Match match) {
        switch (match.Value) {
#if NOT_UNITY
            case "&": return "&amp;";
            case "<": return "&lt;";
            case ">": return "&gt;";
#endif
            default: return escapeTable[match.Value];
        }
    }

#if NOT_UNITY
    static Regex amps = new Regex(
        @"&(?!((#[0-9]+)|(#[xX][a-fA-F0-9]+)|([a-zA-Z][a-zA-Z0-9]*));)",
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled);
    static Regex angles = new Regex(
        @"<(?![A-Za-z/?\$!])",
        RegexOptions.ExplicitCapture);
#endif

    static string EncodeAmpsAndAngles(string s) {
#if NOT_UNITY
        s = amps.Replace(s, "&amp;");
        s = angles.Replace(s, "&lt;");
#endif
        return s;
    }

    static Regex backslashEscapes;

    static string EscapeBackslashes(string s) =>
        backslashEscapes.Replace(s,
            new MatchEvaluator(EscapeBackslashesEvaluator));

    static string EscapeBackslashesEvaluator(Match match) =>
        backslashEscapeTable[match.Value];

    static Regex unescapes = new Regex(
        "\x1A" + "E\\d+E", RegexOptions.Compiled);

    static string Unescape(string s) =>
        unescapes.Replace(s, new MatchEvaluator(UnescapeEvaluator));

    static string UnescapeEvaluator(Match match) =>
        invertedEscapeTable[match.Value];

    static string EscapeBoldItalic(string s) {
        s = s.Replace("*", escapeTable["*"]);
        s = s.Replace("_", escapeTable["_"]);
        return s;
    }

    static string AttributeEncode(string s) => s
        .Replace(">", "&gt;").Replace("<", "&lt;")
        .Replace("\"", "&quot;").Replace("'", "&#39;");

    static string AttributeSafeUrl(string s) {
        s = AttributeEncode(s);
        foreach (var c in "*_:()[]")
            s = s.Replace(c.ToString(), escapeTable[c.ToString()]);
        return s;
    }

    static string EscapeSpecialCharsWithinTagAttributes(string text) {
        var tokens = TokenizeHTML(text);
        var sb = new StringBuilder(text.Length);

        foreach (var token in tokens) {
            var value = token.Value;
            if (token.Type==TokenType.Tag) {
                value = value.Replace(@"\", escapeTable[@"\"]);
                if (AutoHyperlink && value.StartsWith("<!"))
                    value = value.Replace("/", escapeTable["/"]);
                value = Regex.Replace(value,
                    "(?<=.)</?code>(?=.)", escapeTable[@"`"]);
                value = EscapeBoldItalic(value);
            } sb.Append(value);
        } return sb.ToString();
    }

    static string Normalize(string text) {
        var output = new StringBuilder(text.Length);
        var line = new StringBuilder();
        bool valid = false;
        for (int i=0;i<text.Length;i++) switch (text[i]) {
            case '\n':
                if (valid) output.Append(line);
                output.Append('\n');
                line.Length = 0; valid = false;
                break;
            case '\r':
                if ((i < text.Length - 1) && (text[i + 1] != '\n')) {
                    if (valid) output.Append(line);
                    output.Append('\n');
                    line.Length = 0; valid = false;
                } break;
            case '\t':
                int width = (tabWidth - line.Length % tabWidth);
                for (int k = 0; k < width; k++) line.Append(' ');
                break;
            case '\x1A':
                break;
            default:
                if (!valid && text[i] != ' ') valid = true;
                line.Append(text[i]);
                break;
        }
        if (valid) output.Append(line);
        output.Append('\n');
        return output.Append("\n\n").ToString();
    }

    static string RepeatString(string text, int count) {
        var sb = new StringBuilder(text.Length * count);
        for (int i=0;i<count;i++) sb.Append(text);
        return sb.ToString();
    }
}
