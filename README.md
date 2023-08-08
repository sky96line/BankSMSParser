# BankSMSParser
This to extract transection data from bank sms.

```
Engine engine = new Engine();
var sms = <bank sms>;

TransactionInfo transection = engine.GetTransactionInfo(s.Trim());

Console.WriteLine(t.ToString());
```

Here are some of the example.

* IDBI Bank A/c NN9089 debited for INR 1000; ATM WDL. Bal INR 5106.36 (incl. of uncleared chqs) as of 06AUG 20:51hrs. If card not used by you, call 1800226999
```
{
  "account": {
    "type": "card",
    "number": null,
    "name": ""
  },
  "transactionAmount": "1000.00",
  "transactionType": "debit",
  "balance": {
    "available": "5106.36",
    "outstanding": null
  }
}
```
* Your a/c no. XXXXXXXXXXX9089 is debited for Rs.180.00 on 06-08-23 and credited to a/c no. XXXXXXXXXX0007  (UPI Ref no 321829270097).To block UPI services of IDBI Bank, Send SMS as UPIBLOCK <type your mobile no> to 07799000565 from your registered number, or call 18002094324 immediately.- IDBI BANK
```
{
  "account": {
    "type": "upi",
    "number": "321829270097",
    "name": null
  },
  "transactionAmount": "180.00",
  "transactionType": "debit",
  "balance": {
    "available": null,
    "outstanding": null
  }
}
```

===============================

* IDBI Bank A/C NN09089 debited INR. 8934.00 Det:ACH-AAVAS FINANCIER-DLAMR01420. Bal (incl. of chq in clg) INR. 6286.36 as of 05AUG 15:55 hrs.
```
{
  "account": {
    "type": "account",
    "number": "avas",
    "name": null
  },
  "transactionAmount": "8934.00",
  "transactionType": "debit",
  "balance": {
    "available": "6286.36",
    "outstanding": null
  }
}
```

===============================

* IDBI Bank A/c NN09089 credited for INR 10000.00 thru UPI. Bal INR 15220.36 (incl. of chq in clg) as of 03 AUG 16:02hr. If not used by you, call 18002094324
```
{
  "account": {
    "type": null,
    "number": null,
    "name": null
  },
  "transactionAmount": "10000.00",
  "transactionType": "credit",
  "balance": {
    "available": "15220.36",
    "outstanding": null
  }
}
```
* IDBI Bank A/c NN09089 credited for INR 2.00 thru UPI. Bal INR 12119.36 (incl. of chq in clg) as of 29 JUL 05:00hr. If not used by you, call 18002094324
```
{
  "account": {
    "type": null,
    "number": null,
    "name": null
  },
  "transactionAmount": "2.00",
  "transactionType": "credit",
  "balance": {
    "available": "12119.36",
    "outstanding": null
  }
}
```
