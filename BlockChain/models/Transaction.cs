using System.Security.Cryptography;
using System.Text;

namespace BlockChain.models;

public class Transaction
{
    public string Sender { get; }
    public string Receiver { get; }
    public Decimal Amount { get; }
    public DateTime TimeStamp { get; }
    public byte[] Signature { get; }


    public Transaction(string sender, string receiver, decimal amount, RSAParameters senderPrivateKey)
    {
        Sender = sender;
        Receiver = receiver;
        Amount = amount;    
        TimeStamp = DateTime.Now;
        Signature = SignData($"{sender}{receiver}{amount}{TimeStamp}", senderPrivateKey);
    }

    private byte[] SignData(string data, RSAParameters privateKey)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            return rsa.SignData(Encoding.UTF8.GetBytes(data), "SHA256");
        }
    }

    public bool VerifySignature(RSAParameters privateKey)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            return rsa.VerifyData(Encoding.UTF8.GetBytes($"{Sender}{Receiver}{Amount}{TimeStamp}"),
                new SHA256CryptoServiceProvider(), Signature);
        }
    }

    public override string ToString()
    {
        return $"{Sender} sent {Amount} to {Receiver} at {TimeStamp}";

    }
}