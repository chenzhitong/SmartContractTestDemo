using Neo;
using Neo.IO;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence.LevelDB;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VerifyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = new NeoSystem(new LevelDBStore("D:\\PrivateNet2\\NEO-GUI 2.9 release\\Chain_0001E240"));

            //var tx = SGASTest.MintTokens();
            var tx = CreateTx();

            try
            {
                tx = Transaction.DeserializeFrom(tx.ToArray());
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Transaction Format");
            }

            Console.WriteLine("Verify Transaction:" + tx.Verify(Blockchain.Singleton.GetSnapshot(), new List<Transaction> { tx }));

            Console.WriteLine("Raw Transaction:");
            Console.WriteLine(tx.ToArray().ToHexString());
            
            //Then Call neo-cli API：sendrawtransaction in postman.

            Console.ReadLine();
        }

        public static Transaction CreateTx()
        {
            var inputs = new List<CoinReference> {
                new CoinReference(){
                    PrevHash = new UInt256("0xf5d7e31115e14d0dc2ae3cf0dd33ef6b04d2e54f8f641f0654d815b7bac42a6d".Remove(0, 2).HexToBytes().Reverse().ToArray()),
                    PrevIndex = 0
                }
            }.ToArray();

            var outputs = new List<TransactionOutput>{ new TransactionOutput()
            {
                AssetId = Blockchain.UtilityToken.Hash,
                ScriptHash = "ARk2pLaBt2LfK1PrmSjs4SPoRkp94rsEsE".ToScriptHash(),
                Value = new Fixed8((long)(1 * (long)Math.Pow(10, 8)))
            }}.ToArray();


            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(2);
                sb.EmitPush("2014");

                var witness = new Witness
                {
                    InvocationScript = sb.ToArray(),
                    VerificationScript = File.ReadAllBytes("VerifySmartContract.avm")
                };
                return new ContractTransaction
                {
                    Version = 0,
                    Outputs = outputs,
                    Inputs = inputs,
                    Attributes = new TransactionAttribute[0],
                    Witnesses = new Witness[] { witness }
                };
            }
        }
    }
}
