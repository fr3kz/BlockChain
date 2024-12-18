using System.Collections;
using System.Security.Cryptography;

namespace BlockChain.models;

public class BlockChain: IEnumerable<Block>
{ 

    private List<Block> chain;
    private int difficulty = 0;
    private Dictionary<string, RSAParameters> wallets;
    private Dictionary<string, decimal> stakes;
    
    public BlockChain(int difficulty)
    {
        this.difficulty = difficulty;
        chain = new List<Block> { CreateGenesisBlock() };
        wallets = new Dictionary<string, RSAParameters>();
        stakes = new Dictionary<string, decimal>();
    }
    
    public Block CreateGenesisBlock()
    {
        return new Block(0, DateTime.Now, new List<Transaction>(), null);
    }
    
    public Block GetLatestBlock()
    {
        return chain[^1];
    }
    
    public void AddBlock(Block newBlock)
    {
        if (stakes.Count == 0 || stakes.Values.All(s => s == 0))
        {
            throw new ApplicationException("Nie ma walidatora, brak uczestników z stake.");
        }

        // Wybór walidatora na podstawie stake/tokenów
        string validator = SelectValidator();

        Console.WriteLine($"Validator {validator} is adding the block.");

        if (chain.Count > 0)
        {
            newBlock.PreviousHash = GetLatestBlock().Hash;
        }
        else
        {
            newBlock.PreviousHash = null;
        }

        newBlock.MineBlock(difficulty);
        chain.Add(newBlock);
    }

    public bool IsValidChain()
    {
        for (int i = 1; i < chain.Count; i++)
        {
            Block current = chain[i];
            Block previous = chain[i-1];

            if (current.Hash != current.CalculateHash())
            {
                return false;
            }

            if (previous.Hash != previous.CalculateHash())
            {
                return false;
            }
        }
        return true;
    }
    
    public RSAParameters GetOrCreateWallet(string participant)
    {
        if (!wallets.ContainsKey(participant))
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                wallets[participant] = rsa.ExportParameters(true);
                stakes[participant] = 0;
            }
        }
        return wallets[participant];
    }

    public void AddStake(string participant, decimal stake)
    {
        if (!stakes.ContainsKey(participant))
        {
            throw new ApplicationException("Nie ma odpowiedniego walleta");
        }
        stakes[participant] += stake;
        Console.WriteLine($"Stake: {stake} for {participant}"); 
    }

    private string SelectValidator()
    {
        if (stakes.Count == 0) return null;
        decimal total_balance = stakes.Values.Sum();
        decimal random_point = new Random().Next(0,(int)total_balance);
        decimal cumulative_balance = 0;

        foreach (var stake in stakes)
        {
            cumulative_balance += stake.Value;
            if (random_point <= cumulative_balance)
            {
                return stake.Key;
            }
        }

        return null;
    }
    
    public IEnumerator<Block> GetEnumerator()
    { 
        return chain.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}