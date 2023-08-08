namespace BankSMSParser;

public class AccountInfo
{
    public EnumAccountType? type { get; set; }
    public string number { get; set; }
    public string name { get; set; }

    public AccountInfo()
    {

    }

    public static AccountInfo GetAccount(object message)
    {
        var processedMessage = Utils.getProcessedMessage(message);
        int accountIndex = -1;
        var account = new AccountInfo();

        for (int i = 0; i < processedMessage.Count(); i++)
        {
            var word = processedMessage[i];
            if (word.Contains("upiref"))
            {
                var accountNo = Utils.trimLeadingAndTrailingChars(processedMessage[i + 2]);
                var isDigit = decimal.TryParse(accountNo, out _);
                if (!isDigit)
                {
                    continue;
                }
                else
                {
                    accountIndex = i;
                    account.type = EnumAccountType.UPI;
                    account.number = accountNo;

                    return account;
                }
            }
        }

        for (int i = 0; i < processedMessage.Count(); i++)
        {
            var word = processedMessage[i];

            if (word.Equals("ac"))
            {
                if (i + 1 < processedMessage.Count())
                {
                    var accountNo = Utils.trimLeadingAndTrailingChars(processedMessage[i + 1]);

                    var isDigit = int.TryParse(accountNo, out _);
                    if (!isDigit)
                    {
                        continue;
                    }
                    else
                    {
                        accountIndex = i;
                        account.type = EnumAccountType.ACCOUNT;
                        account.number = accountNo;

                        break;
                    }

                }
                else
                {
                    continue;
                }
            }
            else if (word.Contains("ac"))
            {
                var extractedAccountNo = Utils.extractBondedAccountNo(word);

                if (string.IsNullOrWhiteSpace(extractedAccountNo))
                {
                    continue;
                }
                else
                {
                    accountIndex = i;
                    account.type = EnumAccountType.ACCOUNT;
                    account.number = extractedAccountNo;

                    break;
                }
            }
        }

        if (accountIndex == -1)
        {
            account = GetCard(processedMessage);
        }

        if (account.type == null)
        {
            var wallet = processedMessage.FirstOrDefault(x => Constants.wallets.Contains(x));
            if (!string.IsNullOrWhiteSpace(wallet))
            {
                account.type = EnumAccountType.WALLET;
                account.name = wallet;
            }
        }

        // Check for special accounts
        if (account.type == null)
        {
            var specialAccount = Constants.combinedWords.FirstOrDefault(x => x.type == EnumAccountType.ACCOUNT && processedMessage.Contains(x.word));
            if (specialAccount != null)
            {
                account.type = specialAccount.type;
                account.name = specialAccount.word;
            }
        }


        if ((!string.IsNullOrWhiteSpace(account.number)) && account.number.Length > 4 && account.type != EnumAccountType.UPI)
        {
            account.number = account.number.Substring(account.number.Length - 4, 4);
        }

        return account;
    }

    private static AccountInfo GetCard(List<string> message)
    {
        var combinedCardName = "";

        var cardIndex = message.FindIndex(
            (word) =>
            word.Equals("card") ||
            Constants.combinedWords
            .Where(x => x.type == EnumAccountType.CARD)
            .Any((x) =>
            {
                if (x.word == word)
                {
                    combinedCardName = x.word;
                    return true;
                }
                return false;
            })
            );

        AccountInfo card = new AccountInfo();

        if (cardIndex != -1)
        {
            card.number = message[cardIndex + 1];
            card.type = EnumAccountType.CARD;

            if (!Utils.isNumber(card.number))
            {
                return new AccountInfo()
                {
                    type = string.IsNullOrWhiteSpace(combinedCardName) ? card.type : null,
                    name = combinedCardName,
                    number = null
                };
            }

            return card;
        }

        return new AccountInfo();
    }
}
