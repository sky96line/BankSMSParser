namespace BankSMSParser;

public static class Constants
{
    public static readonly string[] availableBalanceKeywords = new string[] {
            "avbl bal",
            "available balance",
            "available limit",
            "available credit limit",
            "limit available",
            "a/c bal",
            "ac bal",
            "available bal",
            "avl bal",
            "updated balance",
            "total balance",
            "new balance",
            "bal",
            "avl lmt",
            "available",
        };

    public static readonly string[] outstandingBalanceKeywords = new string[]
    {
            "outstanding"
    };

    public static readonly string[] wallets = new string[]
    {
            "paytm", "simpl", "lazypay", "amazon_pay"
    };

    public class ICombinedWords
    {
        public string regex { get; set; }
        public string word { get; set; }
        public EnumAccountType type { get; set; }
    }

    public static readonly ICombinedWords[] combinedWords = new ICombinedWords[]
    {
            new()
            {
                regex = "credit\\scard",
                word= "c_card",
                type= EnumAccountType.CARD,
            },
            new()
            {
                regex= "amazon\\spay",
                word= "amazon_pay",
                type= EnumAccountType.WALLET,
            },
            new()
            {
                regex= "uni\\scard",
                word= "uni_card",
                type= EnumAccountType.CARD,
            },
            new()
            {
                regex= "niyo\\scard",
                word= "niyo",
                type= EnumAccountType.ACCOUNT,
            },
            new()
            {
                regex= "slice\\scard",
                word= "slice_card",
                type= EnumAccountType.CARD,
            },
            new()
            {
                regex= "one\\s*card",
                word= "one_card",
                type= EnumAccountType.CARD,
            },
            new()
            {
                regex= "upi\\sref",
                word= "upiref",
                type= EnumAccountType.UPI,
            }
            //new()
            //{
            //    regex= "/upi\\ref/g",
            //    word= "upi",
            //    type= EnumAccountType.UPI,
            //}
    };
}

