using Neo;
using Neo.Core;
using Neo.Implementations.Blockchains.LevelDB;
using Neo.IO;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Need libleveldb.dll, and requires a platform(x86 or x64) that is consistent with the program.
            //Need protocal.json
            //Path of blockchain folder
            Blockchain.RegisterBlockchain(new LevelDBBlockchain("C:\\Users\\chenz\\Desktop\\PrivateNet\\neo-gui 2.7.6\\Chain_0001142D"));

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

            Console.WriteLine("Verify Transaction:" + tx.Verify(new List<Transaction> { tx }));

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
                ScriptHash = Wallet.ToScriptHash("ARk2pLaBt2LfK1PrmSjs4SPoRkp94rsEsE"),
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
                    Scripts = new Witness[] { witness }
                };
            }
        }
    }
}
