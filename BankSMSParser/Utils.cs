using System.Text.RegularExpressions;

namespace BankSMSParser;

public static class Utils
{
    public static string Slice(this string source, int start, int end)
    {
        if (end < 0) // Keep this for negative end support
        {
            end = source.Length + end;
        }
        int len = end - start;               // Calculate length
        return source.Substring(start, len); // Return Substring of length
    }

    public static string trimLeadingAndTrailingChars(string str)
    {
        var (first, last) = (str[0], str[str.Length - 1]);


        var finalStr = char.IsDigit(last) ? str : str.Slice(0, -1);
        finalStr = char.IsDigit(first) ? finalStr : str.Slice(1, str.Length);

        return finalStr;
    }

    public static string extractBondedAccountNo(string accountNo)
    {
        var strippedAccountNo = accountNo.Replace("ac", "");
        var b = int.TryParse(accountNo, out _);
        return b ? "" : strippedAccountNo;
    }

    public static List<string> processMessage(string message)
    {
        var messageStr = message.ToLower();

        // remove '-'
        messageStr = Regex.Replace(messageStr, "-", "");
        // remove ':'
        messageStr = Regex.Replace(messageStr, ":", " ");
        // remove '/'
        messageStr = Regex.Replace(messageStr, "\\/", "");
        // remove '='
        messageStr = Regex.Replace(messageStr, "=", " ");
        // remove '{}'
        messageStr = Regex.Replace(messageStr, "[{}]", " ");
        // remove \n
        messageStr = Regex.Replace(messageStr, "\n", " ");
        // remove \r
        messageStr = Regex.Replace(messageStr, "\\r", " ");
        // remove 'ending'
        messageStr = Regex.Replace(messageStr, "ending ", "");
        // replace "'x'
        messageStr = Regex.Replace(messageStr, "x|[*]", "");
        // // remove 'is' 'with'
        // message = message.replace("\bis\b|\bwith\b", '");
        // replace "'is'
        messageStr = Regex.Replace(messageStr, "is ", "");
        // replace "'with'
        messageStr = Regex.Replace(messageStr, "with ", "");
        // remove 'no.'
        messageStr = Regex.Replace(messageStr, "no. ", "");
        // replace "all ac", acct", account with ac
        messageStr = Regex.Replace(messageStr, "\\bac\\b|\\bacct\\b|\\baccount\\b", "ac");
        // replace all 'rs' with 'rs. '
        messageStr = Regex.Replace(messageStr, "rs(?=\\w)", "rs. ");
        // replace all 'rs ' with 'rs. '
        messageStr = Regex.Replace(messageStr, "rs ", "rs. ");
        // replace all inr with rs.
        messageStr = Regex.Replace(messageStr, "inr(?=\\w)", "rs. ");
        //
        messageStr = Regex.Replace(messageStr, "inr ", "rs. ");

        messageStr = Regex.Replace(messageStr, "inr. ", "rs. ");
        // replace all 'rs. ' with 'rs.'
        messageStr = Regex.Replace(messageStr, "rs. ", "rs.");
        // replace all 'rs.' with 'rs. '
        messageStr = Regex.Replace(messageStr, "rs.(?=\\w)", "rs. ");

        messageStr = Regex.Replace(messageStr, "\\(", "");

        messageStr = Regex.Replace(messageStr, "\\)\\.", " ");

        messageStr = Regex.Replace(messageStr, "\\)", "");

        messageStr = Regex.Replace(messageStr, "<", "");

        messageStr = Regex.Replace(messageStr, ">", "");

        foreach (var item in Constants.combinedWords)
        {
            messageStr = Regex.Replace(messageStr, item.regex, item.word);
        }

        return messageStr.Split(" ").Where(x => x != "").ToList();
    }

    public static List<string> getProcessedMessage(object message)
    {
        if (message.GetType() == typeof(string))
        {
            return processMessage(message.ToString());
        }
        else
        {
            return (List<string>)message;
        }
    }

    public static string padCurrencyValue(string val)
    {
        decimal.TryParse(val, out decimal d);
        return d.ToString("F2");
    }

    public static bool isNumber(string str)
    {
        return decimal.TryParse(str, out _);
    }
}
