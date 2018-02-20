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
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Guardtime.KSI;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Publication;
using Guardtime.KSI.Signature;
using Guardtime.KSI.Signature.Verification;
using Guardtime.KSI.Signature.Verification.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guardtime.Ksi.Samples
{
    [TestClass]
    public class VerificationSamples : KsiSamples

    {
        /// <summary>
        /// Verifies unextended signature using simple wrapper and default verification policy.
        /// </summary>
        [TestMethod]
        public void VerifyUnextendedSignatureUsingDefaultPolicy()
        {
            // Create simple wrapper.
            KSI.Ksi ksi = GetKsi();

            // Read signature, assume to be not extended
            IKsiSignature signature = LoadUnextendedSignature();

            // We need to compute the hash from the original data, to make sure it
            // matches the one in the signature and has not been changed
            // Use the same algorithm as the input hash in the signature
            DataHash documentHash = KsiProvider.CreateDataHasher(signature.InputHash.Algorithm)
                                               .AddData(File.ReadAllBytes("Resources/infile.txt"))
                                               .GetHash();

            // Do the verification and check the result.
            // At first KSI signature is verified against given document hash.
            // Then the signature is extended. If extending succeeds then the signature is verified 
            // against publications file (publications file is automatically downloaded by simple wrapper).
            // If extending is not yet possible then key based verification is done.
            VerificationResult verificationResult = ksi.Verify(signature, documentHash);

            if (verificationResult.ResultCode == VerificationResultCode.Ok)
            {
                Console.WriteLine("VerifyUnextendedSignatureUsingDefaultPolicy > signature valid");
            }
            else
            {
                Console.WriteLine("VerifyUnextendedSignatureUsingDefaultPolicy > verification failed with error > " + verificationResult.VerificationError);
            }
        }

        /// <summary>
        /// Verifies extended signature using simple wrapper and default verification policy.
        /// </summary>
        [TestMethod]
        public void VerifyExtendedSignatureUsingDefaultPolicy()
        {
            // Create simple wrapper.
            KSI.Ksi ksi = GetKsi();

            // Read the existing signature, assume it is extended
            IKsiSignature signature = LoadExtendedSignature();

            DataHash documentHash = KsiProvider.CreateDataHasher(signature.InputHash.Algorithm)
                                               .AddData(File.ReadAllBytes("Resources/infile.txt"))
                                               .GetHash();

            // Do the verification and check the result.
            // The signature is verified against given document hash and publications file (publications file is automatically downloaded by simple wrapper). 
            VerificationResult verificationResult = ksi.Verify(signature, documentHash);

            if (verificationResult.ResultCode == VerificationResultCode.Ok)
            {
                Console.WriteLine("VerifyExtendedSignatureUsingDefaultPolicy > signature valid");
            }
            else
            {
                Console.WriteLine("VerifyExtendedSignatureUsingDefaultPolicy > verification failed with error > " + verificationResult.VerificationError);
            }
        }

        /// <summary>
        /// Verifies signature against a publication using the publications in the publication file. 
        /// The signature must be extended for the verification to succeed.
        /// </summary>
        [TestMethod]
        public void VerifyExtendedSignatureUsingPublicationsFile()
        {
            KSI.Ksi ksi = GetKsi();

            // Read the existing signature, assume it is extended
            IKsiSignature signature = LoadExtendedSignature();

            DataHash documentHash = KsiProvider.CreateDataHasher(signature.InputHash.Algorithm)
                                               .AddData(File.ReadAllBytes("Resources/infile.txt"))
                                               .GetHash();

            // Do the verification and check the result
            VerificationPolicy policy = new PublicationBasedVerificationPolicy();
            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = documentHash,
                PublicationsFile = ksi.GetPublicationsFile(),
            };
            VerificationResult verificationResult = policy.Verify(context);

            if (verificationResult.ResultCode == VerificationResultCode.Ok)
            {
                Console.WriteLine("VerifyExtendedSignatureUsingPublicationsFile > signature valid");
            }
            else
            {
                Console.WriteLine("VerifyExtendedSignatureUsingPublicationsFile > verification failed with error > " + verificationResult.VerificationError);
            }
        }

        /// <summary>
        ///  Verifies the signature against a publication using the specified publication string (code).
        /// </summary>
        [TestMethod]
        public void VerifyExtendedSignatureUsingPublicationsCode()
        {
            // Read the existing signature, assume it is extended
            IKsiSignature signature = LoadExtendedSignature();

            DataHash documentHash = KsiProvider.CreateDataHasher(signature.InputHash.Algorithm)
                                               .AddData(File.ReadAllBytes("Resources/infile.txt"))
                                               .GetHash();

            // The trust anchor in this example is the publication code in Financial Times or on Twitter
            PublicationData publicationData = new PublicationData("AAAAAA-CWYEKQ-AAIYPA-UJ4GRT-HXMFBE-OTB4AB-XH3PT3-KNIKGV-PYCJXU-HL2TN4-RG6SCC-3ZGSBM");

            // Do the verification and check the result
            VerificationPolicy policy = new PublicationBasedVerificationPolicy();

            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = documentHash,
                UserPublication = publicationData
            };
            VerificationResult verificationResult = policy.Verify(context);

            if (verificationResult.ResultCode == VerificationResultCode.Ok)
            {
                Console.WriteLine("VerifyExtendedSignatureUsingPublicationsCode > signature valid");
            }
            else
            {
                Console.WriteLine("VerifyExtendedSignatureUsingPublicationsCode > signature verification failed with error > " + verificationResult.VerificationError);
            }
        }

        /// <summary>
        /// Verify the given signature against a publication. The signature is not extended but
        /// auto-extending is enabled and possible (there is a publication after signing time) so the
        /// verification should succeed.
        ///  </summary>
        [TestMethod]
        public void VerifyExtendedSignatureUsingPublicationsCodeAutoExtend()
        {
            // Read signature, assume to be not extended
            IKsiSignature signature = LoadUnextendedSignature();

            DataHash documentHash = KsiProvider.CreateDataHasher(signature.InputHash.Algorithm)
                                               .AddData(File.ReadAllBytes("Resources/infile.txt"))
                                               .GetHash();

            PublicationData publicationData = new PublicationData("AAAAAA-CWYEKQ-AAIYPA-UJ4GRT-HXMFBE-OTB4AB-XH3PT3-KNIKGV-PYCJXU-HL2TN4-RG6SCC-3ZGSBM");

            // Do the verification and check the result
            VerificationPolicy policy = new PublicationBasedVerificationPolicy();

            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = documentHash,
                UserPublication = publicationData,
                IsExtendingAllowed = true,
                KsiService = GetKsiService(),
            };

            VerificationResult verificationResult = policy.Verify(context);

            if (verificationResult.ResultCode == VerificationResultCode.Ok)
            {
                Console.WriteLine("VerifyExtendedSignatureUsingPublicationsCodeAutoExtend > signature valid");
            }
            else
            {
                Console.WriteLine("VerifyExtendedSignatureUsingPublicationsCodeAutoExtend > signature verification failed with error > " + verificationResult.VerificationError);
            }
        }

        /// <summary>
        /// Verifies signature using key-based verification policy.
        /// </summary>
        [TestMethod]
        public void VerifyKeyBased()
        {
            KSI.Ksi ksi = GetKsi();

            // Read signature, assume to be not extended
            IKsiSignature signature = LoadUnextendedSignature();

            DataHash documentHash = KsiProvider.CreateDataHasher(signature.InputHash.Algorithm)
                                               .AddData(File.ReadAllBytes("Resources/infile.txt"))
                                               .GetHash();

            VerificationPolicy policy = new KeyBasedVerificationPolicy(new X509Store(StoreName.Root), GetCertificateSubjectRdnSelector());
            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = documentHash,
                PublicationsFile = ksi.GetPublicationsFile(),
            };

            VerificationResult verificationResult = policy.Verify(context);

            if (verificationResult.ResultCode == VerificationResultCode.Ok)
            {
                Console.WriteLine("VerifyKeyBased > signature valid");
            }
            else
            {
                Console.WriteLine("VerifyKeyBased > signature verification failed with error > " + verificationResult.VerificationError);
            }
        }

        /// <summary>
        ///  Verifies signature using calendar-based verification policy.
        /// </summary>
        [TestMethod]
        public void VerifyCalendarBasedUnextended()
        {
            IKsiSignature signature = LoadUnextendedSignature();

            DataHash documentHash = KsiProvider.CreateDataHasher(signature.InputHash.Algorithm)
                                               .AddData(File.ReadAllBytes("Resources/infile.txt"))
                                               .GetHash();

            VerificationPolicy policy = new CalendarBasedVerificationPolicy();
            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = documentHash,
                KsiService = GetKsiService(),
            };

            VerificationResult verificationResult = policy.Verify(context);

            if (verificationResult.ResultCode == VerificationResultCode.Ok)
            {
                Console.WriteLine("VerifyCalendarBasedUnextended > signature valid");
            }
            else
            {
                Console.WriteLine("VerifyCalendarBasedUnextended > signature verification failed with error > " + verificationResult.VerificationError);
            }
        }
    }
}