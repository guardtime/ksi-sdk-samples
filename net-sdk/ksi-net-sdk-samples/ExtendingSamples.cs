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
using Guardtime.KSI.Publication;
using Guardtime.KSI.Signature;
using Guardtime.KSI.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guardtime.Ksi.Samples
{
    [TestClass]
    public class ExtendingSamples : KsiSamples
    {
        /// <summary>
        /// Extends signature to the closes publication according to signing time.
        /// </summary>
        [TestMethod]
        public void ExtendToClosestPublication()
        {
            KSI.Ksi ksi = GetKsi();

            // Read an existing signature from file, assume it to be not extended
            IKsiSignature signature = LoadUnextendedSignature();

            // Extends the signature to the closest publication found in the publications file
            // Assumes signature is not extended and at least one publication after
            // the signature obtained
            IKsiSignature extendedSignature = ksi.Extend(signature);

            // Double check if signature was extended
            if (extendedSignature.IsExtended)
            {
                Console.WriteLine("ExtendToClosestPublication > extended to publication > " +
                                  Util.ConvertUnixTimeToDateTime(extendedSignature.PublicationRecord.PublicationData.PublicationTime));
            }
            else
            {
                Console.WriteLine("ExtendToClosestPublication > signature not extended");
            }

            // Store the extended signature
            //using (FileStream stream = File.Create("sample-file-for-signing.txt.extended.ksig"))
            //{
            //    extendedSignature.WriteTo(stream);
            //}
        }

        /// <summary>
        /// Check if signature has been extended to a publication or not.
        /// </summary>
        [TestMethod]
        public void CheckExtended()
        {
            IKsiSignature signature = LoadUnextendedSignature();

            if (signature.IsExtended)
            {
                Console.WriteLine("CheckExtended > publication time > " + Util.ConvertUnixTimeToDateTime(signature.PublicationRecord.PublicationData.PublicationTime));
            }
            else
            {
                Console.WriteLine("CheckExtended > signature not extended");
            }
        }

        /// <summary>
        /// Finds the first publication in the publications file after the given date and prints its references.
        /// </summary>
        [TestMethod]
        public void PrintPublicationInfo()
        {
            KSI.Ksi ksi = GetKsi();

            PublicationRecord publicationRecord = ksi.GetPublicationsFile().GetNearestPublicationRecord(new DateTime(2016, 2, 1));

            foreach (string s in publicationRecord.PublicationReferences)
            {
                Console.WriteLine("PrintPublicationInfo > publication reference > " + s);
            }
        }

        /// <summary>
        /// Extends the given signature to the latest publication found in the publications file in case
        /// the signature was not extended or was extended to an earlier publication.
        /// </summary>
        [TestMethod]
        public void ExtendToLatestPublication()
        {
            KSI.Ksi ksi = GetKsi();
            IKsiSignature signature = LoadUnextendedSignature();

            PublicationRecordInPublicationFile latestPublicationRecord = ksi.GetPublicationsFile().GetLatestPublication();

            if (!signature.IsExtended || signature.PublicationRecord.PublicationData.PublicationTime < latestPublicationRecord.PublicationData.PublicationTime)
            {
                IKsiSignature extendedSignature = ksi.Extend(signature, latestPublicationRecord);

                if (extendedSignature.IsExtended)
                {
                    Console.WriteLine("ExtendToLatestPublication > signature extended to publication > " +
                                      Util.ConvertUnixTimeToDateTime(extendedSignature.PublicationRecord.PublicationData.PublicationTime));

                    // Store the extended signature
                    // ...
                }
            }
        }

        /// <summary>
        ///  Extends signature to a given date.
        /// </summary>
        [TestMethod]
        public void ExtendToGivenPublicationDate()
        {
            KSI.Ksi ksi = GetKsi();
            IKsiSignature signature = LoadUnextendedSignature();

            PublicationRecordInPublicationFile publicationRecord = ksi.GetPublicationsFile().GetNearestPublicationRecord(new DateTime(2016, 2, 15));

            if (publicationRecord == null)
            {
                Console.WriteLine("ExtendToGivenPublicationDate > no suitable publication yet. signature not extended");
                return;
            }

            Console.WriteLine("ExtendToGivenPublicationDate > trying to extend signature to publication > "
                              + Util.ConvertUnixTimeToDateTime(publicationRecord.PublicationData.PublicationTime));

            IKsiSignature extendedSignature = ksi.Extend(signature, publicationRecord);

            if (extendedSignature.IsExtended)
            {
                Console.WriteLine("ExtendToGivenPublicationDate > signature extended to publication > "
                                  + Util.ConvertUnixTimeToDateTime(extendedSignature.PublicationRecord.PublicationData.PublicationTime));
                // Store the extended signature
                // ...
            }
            else
            {
                Console.WriteLine("ExtendToGivenPublicationDate > signature not extended");
            }
        }

        /// <summary>
        /// Extend signature to a publication specified by publication code.
        /// </summary>
        [TestMethod]
        public void ExtendToGivenPublicationCode()
        {
            KSI.Ksi ksi = GetKsi();
            IKsiSignature signature = LoadUnextendedSignature();

            PublicationData publicationData = new PublicationData("AAAAAA-CWYEKQ-AAIYPA-UJ4GRT-HXMFBE-OTB4AB-XH3PT3-KNIKGV-PYCJXU-HL2TN4-RG6SCC-3ZGSBM");

            IKsiSignature extendedSignature = ksi.Extend(signature, publicationData);

            if (extendedSignature.IsExtended)
            {
                Console.WriteLine("ExtendToGivenPublicationCode > signature extended to publication > "
                                  + Util.ConvertUnixTimeToDateTime(extendedSignature.PublicationRecord.PublicationData.PublicationTime));

                // Store the extended signature
                // ...
            }
            else
            {
                Console.WriteLine("ExtendToGivenPublicationCode > signature not extended");
            }
        }
    }
}