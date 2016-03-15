using System;
using Guardtime.KSI.Signature;
using Guardtime.KSI.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guardtime.Ksi.Samples
{
    [TestClass]
    public class SignatureContentSamples : KsiSamples
    {
        /// <summary>
        /// Prints information found in the given signature's publication record.
        /// </summary>
        [TestMethod]
        public void PrintPublicationRecord()
        {
            IKsiSignature signature = LoadExtendedSignature();

            if (signature.PublicationRecord != null)
            {
                Console.WriteLine("PrintPublicationRecord > publication time >" + Util.ConvertUnixTimeToDateTime(signature.PublicationRecord.PublicationData.PublicationTime));
            }
            else
            {
                Console.WriteLine("PrintPublicationRecord > No publication record in signature");
            }
        }

        /// <summary>
        /// Prints information found in the given signature's aggregation hash chain.
        /// </summary>
        [TestMethod]
        public void PrintAggregationHashChain()
        {
            IKsiSignature signature = LoadExtendedSignature();

            foreach (AggregationHashChain ahc in signature.GetAggregationHashChains())
            {
                Console.WriteLine("PrintAggregationHashChain > chain identity > " + ahc.GetChainIdentity());
            }
        }

        /// <summary>
        /// Prints information found in the given signature's calendar hash chain.
        /// </summary>
        [TestMethod]
        public void PrintCalendarHashChain()
        {
            IKsiSignature signature = LoadExtendedSignature();

            Console.WriteLine("PrintCalendarHashChain > publication time > " + Util.ConvertUnixTimeToDateTime(signature.CalendarHashChain.PublicationTime));
            Console.WriteLine("PrintCalendarHashChain > registration time > " + Util.ConvertUnixTimeToDateTime(signature.CalendarHashChain.RegistrationTime));
        }

        /// <summary>
        /// Prints the identity in the signature.
        /// </summary>
        [TestMethod]
        public void PrintIdentity()
        {
            IKsiSignature signature = LoadExtendedSignature();

            Console.WriteLine("PrintIdentity > " + signature.Identity);
        }

        /// <summary>
        /// Prints the signing (aggregation) time of the signature.
        /// </summary>
        [TestMethod]
        public void PrintSigningTime()
        {
            IKsiSignature signature = LoadExtendedSignature();

            Console.WriteLine("PrintSigningTime > " + Util.ConvertUnixTimeToDateTime(signature.AggregationTime));
        }

        /// <summary>
        /// Prints the RSA signature type of the calendar authentication record.
        /// </summary>
        [TestMethod]
        public void PrintCalendarAuthenticationRecord()
        {
            IKsiSignature signature = LoadUnextendedSignature();

            Console.WriteLine("PrintCalendarAuthenticationRecord > signature type > " + signature.CalendarAuthenticationRecord.SignatureData.SignatureType);
        }
    }
}