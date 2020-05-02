using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class Consts
    {
        public static string BossRoleName => "Boss";
        public static string DevRoleName => "Dev";
        public static string SubscriberRoleName => "Subscriber";

        public static string TradeRemoteApiName => "Trade";
    }

    public enum PaymentTypes
    {
        CREDIT,
        ATM
    }

    public enum ThirdPartyPayment
    { 
        EcPay
    }
    
}
