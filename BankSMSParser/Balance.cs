namespace BankSMSParser;

using System.Text.RegularExpressions;

public class Balance
    {
        public string available { get; set; }
        public string outstanding { get; set; }

        private static string extractBalance(int index, string message, int length)
        {
            var balance = "";
            var sawNumber = false;
            var invalidCharCount = 0;
            char char_;
            var start = index;
            while (start < length)
            {
                char_ = message[start];

                if (char_ >= '0' && char_ <= '9')
                {
                    sawNumber = true;
                    // is_start = false;
                    balance += char_;
                }
                else if (sawNumber)
                {
                    if (char_ == '.')
                    {
                        if (invalidCharCount == 1)
                        {
                            break;
                        }
                        else
                        {
                            balance += char_;
                            invalidCharCount += 1;
                        }
                    }
                    else if (char_ != ',')
                    {
                        break;
                    }
                }

                start += 1;
            }

            return balance;
        }

        private static string findNonStandardBalance(string message, EnumBalanceKeyWordsType keyWordsType = EnumBalanceKeyWordsType.AVAILABLE)
        {
            var balanceKeywords = keyWordsType == EnumBalanceKeyWordsType.AVAILABLE ? Constants.availableBalanceKeywords : Constants.outstandingBalanceKeywords;

            string balKeywordRegex = string.Join("|", balanceKeywords).Replace("/", "\\/");
            string amountRegex = "([\\d]+\\.[\\d]+|[\\d]+)";

            // balance 100.00
            var regex = new Regex($"{amountRegex}\\s*{balKeywordRegex}", RegexOptions.IgnoreCase);
            var matches = regex.Match(message);

            if (matches.Success)
            {

                var balance = matches.Value.Split(" ").First(); // return only first match
                return Utils.isNumber(balance) ? balance : "";
            }

            // 100.00 available
            regex = new Regex($"{balKeywordRegex}\\s*{amountRegex}", RegexOptions.IgnoreCase);
            matches = regex.Match(message);

            if (matches.Success)
            {
                var balance = matches.Value.Split(" ").First(); // return only first match
                return Utils.isNumber(balance) ? balance : "";
            }

            return null;
        }

        public static string GetBalance(object message, EnumBalanceKeyWordsType keyWordsType = EnumBalanceKeyWordsType.AVAILABLE)
        {
            var processedMessage = Utils.getProcessedMessage(message);
            var messageString = string.Join(" ", processedMessage);
            var indexOfKeyword = -1;
            var balance = "";

            var balanceKeywords = keyWordsType == EnumBalanceKeyWordsType.AVAILABLE ? Constants.availableBalanceKeywords : Constants.outstandingBalanceKeywords;

            for (int i = 0; i < balanceKeywords.Length; i++)
            {
                var word = balanceKeywords[i];
                indexOfKeyword = messageString.IndexOf(word);

                if (indexOfKeyword != -1)
                {
                    indexOfKeyword += word.Length;
                    break;
                }
                else
                {
                    // eslint-disable-next-line no-continue
                    continue;
                }
            }

            var indexOfRs = -1;
            if (indexOfKeyword >= 0)
            {
                // found the index of keyword, moving on to finding 'rs.' occuring after indexOfKeyword
                var index = indexOfKeyword >= 0 ? indexOfKeyword : 0;
                string nextThreeChars = messageString.Substring(index, 3);

                index += 3;

                while (index < messageString.Count())
                {
                    // discard first char
                    nextThreeChars = nextThreeChars.Substring(1);


                    // add the current char at the end
                    nextThreeChars += messageString[index];


                    if (nextThreeChars.Equals("rs.", StringComparison.OrdinalIgnoreCase))
                    {
                        indexOfRs = index + 2;
                        break;
                    }

                    index += 1;
                }
            }


            // no occurence of 'rs.'
            if (indexOfRs == -1)
            {
                // check for non standard balance
                balance = findNonStandardBalance(messageString);
                return string.IsNullOrWhiteSpace(balance) ? null : Utils.padCurrencyValue(balance);
            }

            balance = extractBalance(indexOfRs, messageString, messageString.Length);

            return string.IsNullOrWhiteSpace(balance) ? null : Utils.padCurrencyValue(balance);
        }
    }
