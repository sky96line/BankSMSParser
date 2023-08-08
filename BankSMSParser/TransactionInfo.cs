using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BankSMSParser;

public class TransactionInfo
{
    public AccountInfo account { get; set; }
    public string transactionAmount { get; set; }
    public EnumTransactionType? transactionType { get; set; }
    public Balance balance { get; set; }

    public override string ToString()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

        return JsonConvert.SerializeObject(this, Formatting.Indented, settings);
    }
}
