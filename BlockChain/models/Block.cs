using System.Security.Cryptography;
using System.Text;

namespace BlockChain.models;

public class Block
{
    public int Index { get; }
    public DateTime Timestamp { get; }
    public List<Transaction> Transactions { get; }
    public string PreviousHash { get; set; }
    public string Hash { get;  set; }
    private int Nonce { get; set; }

    public Block(int index, DateTime timestamp, List<Transaction> transactions,string previousHash)
    {
        Index = index;
        Timestamp = timestamp;
        Transactions = transactions;
        PreviousHash = previousHash;
        Nonce = 0;
        Hash = CalculateHash();
    }

    public  string CalculateHash()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string data = $"{Index}-{Timestamp}-{PreviousHash ?? ""}-{Nonce}";
            foreach (var trans in Transactions)
            {
                data += trans.ToString();
            }
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public void MineBlock(int difficulty)
    {
        string prefix = new string('0', difficulty);

        while (Hash.Substring(0, difficulty) != prefix)
        {
            Nonce++;
            Hash = CalculateHash();
        }
        Console.WriteLine($"Block mined: {Hash}");

    }
}