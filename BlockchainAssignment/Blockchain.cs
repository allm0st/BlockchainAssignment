﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockchainAssignment
{
    internal class Blockchain
    {
        public List<Block> blocks = new List<Block>();
        public List<Transaction> transactions = new List<Transaction>();
        int transactionsPerBlock = 5;

        public Blockchain()
        {
            blocks.Add(new Block());
        }

        public string getBlockAsString(int index)
        {
            if (index >= 0 && index < blocks.Count)
                return blocks[index].ToString();
            else return "Block doesn't exist!";
        }

        public Block getLastBlock()
        {
            return blocks[blocks.Count - 1];
        }

        public List<Transaction> getPendingTransactions(string strategy, string recipientAddress = "")
        {
            List<Transaction> pendingTransactions = new List<Transaction>();

            if (transactions.Count == 0)
                return pendingTransactions;

            int n = Math.Min(transactionsPerBlock, transactions.Count);

            switch (strategy)
            {
                case "Greedy":
                    for (int i = 0; i < n; i++)
                    {
                        // find the transaction with the highest fees
                        Transaction expensiveTransaction = transactions.Find(t => t.fees == transactions.Max(t2 => t2.fees));
                        // add it to the queue to be added to the block's transactions
                        pendingTransactions.Add(expensiveTransaction);
                        // then remove it from pending list
                        transactions.Remove(expensiveTransaction);
                    }
                    break;
                case "Altruistic":
                    // get the transactions which have been in the queue for the longest
                    pendingTransactions = transactions.GetRange(0, n);
                    transactions.RemoveRange(0, n);
                    break;
                case "Random":
                    // pick transactions at random
                    Random rng = new Random();
                    for (int i = 0; i < n; i++)
                    {
                        int randomIndex = rng.Next(0, transactions.Count);
                        pendingTransactions.Add(transactions[randomIndex]);
                        transactions.RemoveAt(randomIndex);
                    }
                    break;
                case "Preference":
                    // pick the transactions from a specified address first, then all the ones that have been waiting the longest
                    for (int i = 0; i < n; i++)
                    {
                        Transaction preferredTransaction = transactions.Find(t => t.recipientAddress == recipientAddress);
                        if (preferredTransaction == null)
                            preferredTransaction = transactions[0];

                        pendingTransactions.Add(preferredTransaction);
                        transactions.Remove(preferredTransaction);
                    }
                    break;
            }

            return pendingTransactions;
        }

        public Transaction createTransaction(string from, string to, double amount, double fees, string priv)
        {
            Transaction newTransaction = new Transaction(from, to, amount, fees, priv);
            transactions.Add(newTransaction);

            return newTransaction;
        }

        public double getBalance(string address)
        {
            double balance = 0;

            foreach (Block b in blocks)
                foreach (Transaction t in b.transactions)
                {
                    if (t.recipientAddress == address)
                        balance += t.amount;

                    if (t.senderAddress == address)
                        balance -= t.amount + t.fees;
                }

            return balance;
        }

        public bool validateHash(Block b)
        {
            string reHash = Block.CreateHash(b.hashKey + b.nonce.ToString());
            return reHash == b.hash;
        }

        public bool validateMerkleRoot(Block b)
        {
            string reMerkle = Block.getMerkleRoot(b.transactions);
            return reMerkle == b.merkleRoot;
        }

        public override string ToString()
        {
            string output = string.Empty;
            blocks.ForEach(b => output += b.ToString() + "\n");
            return output;
        }
    }
}
