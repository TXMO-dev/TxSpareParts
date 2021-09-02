using System;

namespace TxSpareParts.Utility
{
    public class SD
    {
        // App Constants
        public const string Admin = "Administrator";
        public const string Customer = "Customer";
        public const string Employee = "Employee";

        //Employee Constants
        public const string Supervisor = "Supervisor";
        public const string Staff = "Staff";

        //Administrator constants
        public const string ChiefAdmin = "Chief";
        /* any user object without administrative status 
         * but a role of administrator is a normal administrator
         * The chief administrator will be created by the I.T. Administrator.
         */

        //Product Category
        public const string GOE = "Genuine Original Equipment";
        public const string OEM = "Original Equipment Manufacturer";
        public const string AP = "Aftermarket Parts";
        public const string RP = "Remanufactured Parts";
        public const string RCP = "Reconditioned Parts";
        public const string SP = "Salvaged Parts";

        //Currencies
        public const string Dollars = "USD";
        public const string Naira = "NGN";
        public const string Ghana_Cedis = "GHS";

        //Types of order status
        public const string AWP = "Awaiting Payment";
        public const string PR = "Payment Received";
        public const string PU = "Payment Updated";
        public const string COM = "Completed";
        public const string RFP = "Refunded Partially";
        public const string RF = "Refunded";
        public const string CAN = "Cancelled";
        public const string FA = "Payment Failed";
        public const string EXP = "Order Expired";
        public const string PRO = "Processing";
        public const string RTS = "Ready To Ship";
        public const string SHI = "Shipped";

        //Shares Split           
        public const int my_split_percentage = 20;

        //Image Category
        public const string Product_type = "Product";
        public const string Company_type = "Company";
        public const string User_type = "User";

        //Dispute Selection
        public const string dispute_declined = "declined";
        public const string merchant_accepted = "merchant-accepted";       
    }
}
