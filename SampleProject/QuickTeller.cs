﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment;
using Newtonsoft.Json.Linq;
using Payment.Models;

namespace SampleProject
{
    public class QuickTeller
    {
        static List<KeyValuePair<string, string>>  hashMapSecure, hashMap;
        static string clientId = "IKIAF6C068791F465D2A2AA1A3FE88343B9951BAC9C3";
        static string clientSecret = "FTbMeBD7MtkGBQJw1XoM74NaikuPL13Sxko1zb0DMjI=";
        static void Main(string[] args)
        {
            Interswitch.Init(clientId, clientSecret);
            var token = Interswitch.GetToken();
            Console.WriteLine(token);
            #region
            Console.WriteLine("************ Secure Data Starts here ****************");
            string[] secureData = Interswitch.GetSecureData("2347012345678", "1436", "6280511000000095", "1111", "111", "5004", "GTB00199212");
            Console.WriteLine(secureData[0]);
            Console.WriteLine(secureData[1]);

            hashMapSecure = new List<KeyValuePair<string, string>>();
            hashMapSecure.AddRange(new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>(Constants.ContentType, "application/json"),
                new KeyValuePair<string, string>("SignatureMethod", "SHA1"),
                new KeyValuePair<string, string>("terminalId", "3IWP0001")}
                );
            object doEnquiry = new
            {
                PaymentCode = "10803",
                CustomerId = "08124888436",
                CustomerEmail = "iswtester2@yahoo.com",
                CustomerMobile = "2348056731575",
                PageFlowValues = "",
                TerminalId = "3IWP0076"
            };

            hashMap = new List<KeyValuePair<string, string>>();
            hashMap.AddRange(new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>(Constants.ContentType, "application/json"),
                new KeyValuePair<string, string>("SignatureMethod", "SHA1"),
                new KeyValuePair<string, string>("terminalId", "3IWP0001")}
                );

            Console.WriteLine("Do Enquiry ....");
            var enquiryResponse = Interswitch.Send("/quicktellerservice/api/v3/Transactions/DoInquiry", "GET", doEnquiry, token, hashMapSecure).Result;
            JObject json = JObject.Parse(enquiryResponse.ToString());
            Console.WriteLine(json.GetValue("TransactionRef"));
            Console.WriteLine(enquiryResponse);
            Console.WriteLine("Do Enquiry done ....");


            #region
            object securePayment = new
            {                                                                
                TransactionRef = json.GetValue("TransactionRef"),                
                SecureData = secureData[1],
                PinData = secureData[0],                
                Msisdn = "2348052678744",
                bankCbnCode = "058",
                amount = "100"
            };
            
            Console.WriteLine("****************************");
            Console.WriteLine("Send Payment with secure ....");
            var secureResponse = Interswitch.Send("/quicktellerservice/api/v3/Transactions/sendtransaction", "GET", securePayment, token, hashMap).Result;
            Console.WriteLine(secureResponse);
            Console.WriteLine("Send Payment with secure done ....");
            #endregion

            Console.WriteLine("************ Secure Data Ends here ****************");


            Console.WriteLine("##############################################");
            Console.WriteLine("#************ Pay Code Starts here **********#");
            var payCode = Interswitch.Send("/cardless-service/api/v1/cardless-services/tokens", "POST", securePayment, token, hashMap).Result;
            Console.WriteLine(payCode);
            Console.WriteLine("************ Pay Code Ends here ****************");
            #endregion
            Console.ReadKey();
        }
    }
}
