/*
 * Copyright 2013-2016 Guardtime, Inc.
 *
 * This file is part of the Guardtime client SDK.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *     http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES, CONDITIONS, OR OTHER LICENSES OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 * "Guardtime" and "KSI" are trademarks or registered trademarks of
 * Guardtime, Inc., and no license to trademarks is granted; Guardtime
 * reserves and retains all trademark rights.
 */

using System;
using System.Linq;
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
                Console.WriteLine("PrintAggregationHashChain > chain identity > " +
                                  string.Join("::", ahc.GetIdentity().Select(i => i.ClientId).ToArray()));
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
            Console.WriteLine("PrintCalendarHashChain > aggregation time > " + Util.ConvertUnixTimeToDateTime(signature.CalendarHashChain.AggregationTime));
        }

        /// <summary>
        /// Prints the identity in the signature.
        /// </summary>
        [TestMethod]
        public void PrintIdentity()
        {
            IKsiSignature signature = LoadExtendedSignature();

            Console.WriteLine("PrintIdentity > " + string.Join("::", signature.GetIdentity().Select(i => i.ClientId).ToArray()));
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