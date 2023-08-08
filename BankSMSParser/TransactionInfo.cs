using Newtonsoft.Json;

namespace BankSMSParser;

public class TransactionInfo
{
    public AccountInfo account { get; set; }
    public string transactionAmount { get; set; }
    public EnumTransactionType? transactionType { get; set; }
    public Balance balance { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
