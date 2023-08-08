namespace BankSMSParser;

using System.Text.RegularExpressions;

public enum EnumAccountType
    {
        CARD,
        WALLET,
        ACCOUNT,
        UPI,
    }

    public enum EnumBalanceKeyWordsType
    {
        AVAILABLE,
        OUTSTANDING
    }

    public enum EnumTransactionType
    {
        Debit,
        Credit,
        Null
    }

public class Engine
{
    public string getTransactionAmount(object messages)
    {
        var processedMessage = Utils.getProcessedMessage(messages);
        var index = processedMessage.IndexOf("rs.");

        // If "rs." does not exist
        // Return ""
        if (index == -1)
        {
            return "";
        }

        var money = processedMessage[index + 1];


        money = Regex.Replace(money.ToString(), ",", "");
        money = Regex.Replace(money.ToString(), ";", "");


        // If data is false positive
        // Look ahead one index and check for valid money
        // Else return the found money
        if (!Utils.isNumber(money))
        {
            money = processedMessage[index + 2].ToString();
            money = Regex.Replace(money.ToString(), ",", "");

            // If this is also false positive, return ""
            // Else return the found money
            if (!Utils.isNumber(money))
            {
                return "";
            }
            return Utils.padCurrencyValue(money);
        }
        return Utils.padCurrencyValue(money);
    }

    public EnumTransactionType? getTransactionType(object message)
    {

        Regex creditPattern = new Regex("(?:credited|credit|deposited|added|received|refund|repayment)", RegexOptions.ECMAScript);
        Regex debitPattern = new Regex("(?:debited|debit|deducted)", RegexOptions.ECMAScript);
        Regex miscPattern = new Regex("(?:payment|spent|paid|used\\s+at|charged|transaction\\son|transaction\\sfee|tran|booked|purchased|sent\\s+to|purchase\\s+of)", RegexOptions.ECMAScript);

        var messageStr = message.GetType() == typeof(string) ? message.ToString() : string.Join(" ", ((List<string>)message));

        var a = debitPattern.Match(messageStr);
        if (a.Success)
        {
            return EnumTransactionType.Debit;
        }

        a = creditPattern.Match(messageStr);
        if (a.Success)
        {
            return EnumTransactionType.Credit;
        }

        a = miscPattern.Match(messageStr);
        if (a.Success)
        {
            return EnumTransactionType.Debit;
        }

        return null;
    }

    public TransactionInfo getTransactionInfo(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return null;
        }

        var processedMessage = Utils.processMessage(message);
        var account = AccountInfo.GetAccount(processedMessage);
        var availableBalance = Balance.GetBalance(
          processedMessage,
          EnumBalanceKeyWordsType.AVAILABLE
        );

        var transactionAmount = getTransactionAmount(processedMessage);

        List<string> ava = new List<string>()
            {
                availableBalance,
                transactionAmount,
                account.number
            };

        var isValid = ava.Where(x => !string.IsNullOrWhiteSpace(x)).Count() >= 2;

        var transactionType = isValid ? getTransactionType(processedMessage) : null;
        Balance balance = new Balance()
        {
            available = availableBalance,
            outstanding = null
        };


        if (account.type.HasValue && account.type == EnumAccountType.CARD)
        {
            balance.outstanding = Balance.GetBalance(processedMessage, EnumBalanceKeyWordsType.OUTSTANDING);
        }

        return new TransactionInfo()
        {
            account = account,
            balance = balance,
            transactionAmount = transactionAmount,
            transactionType = transactionType.HasValue ? transactionType.Value : null
        };
    }
}