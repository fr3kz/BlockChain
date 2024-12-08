using System.Collections;
using System.Security.Cryptography;

namespace BlockChain.models;

public class BlockChain: IEnumerable<Block>
{ 

    private List<Block> chain;
    private int difficulty = 0;
    private Dictionary<string, RSAParameters> wallets;
    
    public BlockChain(int difficulty)
    {
        this.difficulty = difficulty;
        chain = new List<Block> { CreateGenesisBlock() };
        wallets = new Dictionary<string, RSAParameters>();
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
        if (chain.Count > 0)
        {
            newBlock.PreviousHash = GetLatestBlock().Hash; // Set the previous hash only if there's at least one block in the chain
        }
        else
        {
            // For the first block, ensure there is no previous hash (genesis block)
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
            }
        }
        return wallets[participant];
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