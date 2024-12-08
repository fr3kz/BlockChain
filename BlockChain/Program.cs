// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;

Console.WriteLine("Hello, World!");

var blockchain = new BlockChain.models.BlockChain(2);


RSAParameters alicePrivateKey = blockchain.GetOrCreateWallet("Alice");
RSAParameters bobPrivateKey = blockchain.GetOrCreateWallet("Bob");


blockchain.AddBlock(new BlockChain.models.Block(1, DateTime.Now, new List<BlockChain.models.Transaction>
{
    new BlockChain.models.Transaction("Alice", "Bob", 10, alicePrivateKey),
    new BlockChain.models.Transaction("Bob", "Charlie", 5, bobPrivateKey)
}, blockchain.GetLatestBlock().Hash));


blockchain.AddBlock(new BlockChain.models.Block(2, DateTime.Now, new List<BlockChain.models.Transaction>
{
    new BlockChain.models.Transaction("Charlie", "Alice", 2, blockchain.GetOrCreateWallet("Charlie"))
}, blockchain.GetLatestBlock().Hash));


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
Console.ReadLine();
