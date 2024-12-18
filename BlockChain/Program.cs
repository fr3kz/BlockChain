using System.Security.Cryptography;
using BlockChain.models;
using System.Threading.Tasks;

namespace BlockChain;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Blockchain with Dynamic Transactions!");

        var blockchain = new models.BlockChain(2);
        blockchain.GetOrCreateWallet("Alice");
        blockchain.GetOrCreateWallet("Bob");
        blockchain.GetOrCreateWallet("Charlie");
        blockchain.AddStake("Alice", 50); 
        blockchain.AddStake("Bob", 30);
        blockchain.AddStake("Charlie", 20);

        blockchain.AddBlock(new Block(0, DateTime.Now, new List<Transaction>(), null));
        Task.Run(() => AddDynamicTransactions(blockchain));

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Current Blockchain State:");
            foreach (var block in blockchain)
            {
                Console.WriteLine($"Block: {block.Index}, Hash: {block.Hash}");
                foreach (var transaction in block.Transactions)
                {
                    Console.WriteLine(transaction);
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Is Blockchain Valid: {blockchain.IsValidChain()}");

            await Task.Delay(5000);
        }
    }

    static async Task AddDynamicTransactions(BlockChain.models.BlockChain blockchain)
    {
        Random random = new Random();
        List<string> participants = new List<string> { "Alice", "Bob", "Charlie" };

        int counter = 0;

        while (true)
        {
            await Task.Delay(3000);

            string sender = participants[random.Next(participants.Count)];
            string receiver = participants[random.Next(participants.Count)];
            while (receiver == sender)
            {
                receiver = participants[random.Next(participants.Count)];
            }

            decimal amount = random.Next(1, 100);
            RSAParameters senderPrivateKey = blockchain.GetOrCreateWallet(sender);

            var newBlock = new Block(counter + 1, DateTime.Now, new List<Transaction>
            {
                new Transaction(sender, receiver, amount, senderPrivateKey)
            }, blockchain.GetLatestBlock().Hash);

            blockchain.AddBlock(newBlock);
            counter++;
        }
    }
}
