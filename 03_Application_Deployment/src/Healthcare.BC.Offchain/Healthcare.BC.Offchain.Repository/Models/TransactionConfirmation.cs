using System;
using System.Diagnostics;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace Healthcare.BC.Offchain.Repository.Models
{
    public sealed class TransactionConfirmation
    {
        public const int BlockHashFieldNumber = 1;
        public const int BlockNumberFieldNumber = 2;
        public const int LedgerAddressFieldNumber = 3;
        public const int LogsFieldNumber = 4;
        public const int TransactionHashFieldNumber = 5;
        public const int TransactionIndexFieldNumber = 6;
        public const int MessageFieldNumber = 7;
        public const int NameFieldNumber = 8;
        public const int ProxyIdFieldNumber = 9;

        public TransactionConfirmation()
        {
        }
        
        public TransactionConfirmation(TransactionConfirmation other)
        {

        }

        
        public static MessageDescriptor Descriptor { get; }
        
        
        public Any Message { get; set; }
        
        public string TransactionIndex { get; set; }
        
        public string TransactionHash { get; set; }
        
        public string LedgerAddress { get; set; }
        
        public string BlockNumber { get; set; }
        
        public string BlockHash { get; set; }
        
        public string ProxyId { get; set; }
        
        public string Name { get; set; }
    }
}