using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using TxSpareParts.Utility.interfaces;
using TxSpareParts.Utility.Options;
using Twilio.Rest.Verify.V2;
using Twilio.Rest.Verify.V2.Service;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Utility
{
    public class PhoneVerificationHandler : IPhoneNumberVerification
    {
        private readonly PhoneNumberOptions _options;
        public PhoneVerificationHandler(IOptions<PhoneNumberOptions> options)
        {
            _options = options.Value;
        }

        private void InitializeService()
        {
            string accountSid = _options.accountSid;
            string authToken = _options.authToken;

            TwilioClient.Init(accountSid, authToken);
        }

        public async Task<string> SendToken(string phonenumber)
        {
            InitializeService();
            var verification = await VerificationResource.CreateAsync(
               to: phonenumber,
               channel: "sms",
               pathServiceSid: _options.serviceSid
            );  
            return $"Verification Token has been sent to your Phone number via SMS. Verification is currently {verification.Status}";

        }


        public async Task<string> VerifyToken(ApplicationUser user, string code)
        {
            InitializeService();
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: user.PhoneNumber,
                code: code,  
                pathServiceSid: _options.serviceSid
            );
            return $"Congratulations Verification has been successfully {verificationCheck.Status}";
        }
    }
}
