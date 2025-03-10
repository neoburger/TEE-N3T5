using System;
using LibHelper;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;
using Neo.VM;
using Neo.VM.Types;
using Neo.Wallets;

namespace LibRPC
{
    public static class Program
    {
        private static readonly string RPC = Environment.GetEnvironmentVariable("RPC");
        private static readonly Uri URI = new(RPC);
        private static readonly ProtocolSettings settings = ProtocolSettings.Load("/dev/stdin");
        private static readonly RpcClient CLI = new(URI, null, null, settings);
        private static readonly TransactionManagerFactory factory = new(CLI);
        static void Main(string[] args)
        {
            Call(new byte[] { ((byte)OpCode.RET) });
            "OK!".Log();
        }
        public static StackItem[] Call(this byte[] script)
        {
            RpcInvokeResult result = CLI.InvokeScriptAsync(script).GetAwaiter().GetResult();
            if (result.State != VMState.HALT)
            {
                throw new Exception();
            }
            return result.Stack;
        }
        public static long GetUnclaimedGas(this string address) => CLI.GetUnclaimedGasAsync(address).GetAwaiter().GetResult().Unclaimed;
        public static long GetUnclaimedGas(this UInt160 account) => GetUnclaimedGas(account.ToAddress(settings.AddressVersion));
        public static UInt256 Send(this Transaction tx) => CLI.SendRawTransactionAsync(tx).GetAwaiter().GetResult();
        public static TransactionManager TxMgr(this byte[] script, Signer[] signers = null) => factory.MakeTransactionAsync(script, signers).GetAwaiter().GetResult();
    }
}
