﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    internal class Transaction
    {
        public string hash;
        string signature;
        public string senderAddress;
        public string recipientAddress;
        DateTime timestamp;
        public double amount;
        public double fees;

        public Transaction(string from, string to, double amount, double fees, string privateKey)
        {
            this.senderAddress = from;
            this.recipientAddress = to;
            this.amount = amount;
            this.fees = fees;
            this.timestamp = DateTime.Now;
            this.hash = CreateHash();
            this.signature = Wallet.Wallet.CreateSignature(from, privateKey, this.hash);
        }

        // Same logic used in Block.cs
        public string CreateHash()
        {
            SHA256 hasher = SHA256.Create();

            // Hash all properties
            string input = timestamp.ToString() + senderAddress + recipientAddress + amount.ToString() + fees.ToString() + signature;
            byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Convert Hash from Byte array to String
            string hash = hashByte.Aggregate(string.Empty, (acc, x) => acc + string.Format("{0:x2}", x));

            return hash;
        }

        public override string ToString()
        {
            return "---\n  Transaction Hash: " + hash
                + "\n  Digital signature: " + signature
                + "\n  Timestamp: " + timestamp.ToString()
                + "\n  Transferred: " + amount.ToString() + " " + BlockchainApp.assignmentCoin
                + "\n  Fees: " + fees.ToString()
                + "\n  Sender address: " + senderAddress
                + "\n  Recipient address: " + recipientAddress
                + "\n";
        }
    }
}
