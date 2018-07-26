using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.Numerics;

namespace VerifySmartContract
{
    public class VerifySmartContract : SmartContract
    {
        public static bool Main(string method, object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                return method == "2014";
            }
            return false;
        }
    }
}
