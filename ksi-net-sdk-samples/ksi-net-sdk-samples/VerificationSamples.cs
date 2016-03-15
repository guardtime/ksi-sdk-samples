using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Guardtime.KSI.Crypto.Microsoft.Hashing;
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
        /// Verifies signature against a publication using the publications in the publication file. The
        /// signature must be extended for the verification to succeed.</summary>
        [TestMethod]
        public void VerifyExtendedSignatureUsingPublicationsFile()
        {
            KSI.Ksi ksi = GetKsi();

            // Read the existing signature, assume it is extended
            IKsiSignature signature = LoadExtendedSignature();

            // We need to compute the hash from the original data, to make sure it
            // matches the one in the signature and has not been changed
            // Use the same algorithm as the input hash in the signature
            IDataHasher dataHasher = new DataHasher(signature.GetAggregationHashChains()[0].InputHash.Algorithm);
            dataHasher.AddData(File.ReadAllBytes("Resources/infile.txt"));

            // Do the verification and check the result
            VerificationPolicy policy = new PublicationBasedVerificationPolicy();
            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = dataHasher.GetHash(),
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

            IDataHasher dataHasher = new DataHasher(signature.GetAggregationHashChains()[0].InputHash.Algorithm);
            dataHasher.AddData(File.ReadAllBytes("Resources/infile.txt"));

            // The trust anchor in this example is the publication code in Financial Times or on Twitter
            PublicationData publicationData = new PublicationData("AAAAAA-CWYEKQ-AAIYPA-UJ4GRT-HXMFBE-OTB4AB-XH3PT3-KNIKGV-PYCJXU-HL2TN4-RG6SCC-3ZGSBM");

            // Do the verification and check the result
            VerificationPolicy policy = new PublicationBasedVerificationPolicy();

            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = dataHasher.GetHash(),
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

            IDataHasher dataHasher = new DataHasher(signature.GetAggregationHashChains()[0].InputHash.Algorithm);
            dataHasher.AddData(File.ReadAllBytes("Resources/infile.txt"));

            PublicationData publicationData = new PublicationData("AAAAAA-CWYEKQ-AAIYPA-UJ4GRT-HXMFBE-OTB4AB-XH3PT3-KNIKGV-PYCJXU-HL2TN4-RG6SCC-3ZGSBM");

            // Do the verification and check the result
            VerificationPolicy policy = new PublicationBasedVerificationPolicy();

            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = dataHasher.GetHash(),
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

            IDataHasher dataHasher = new DataHasher(signature.GetAggregationHashChains()[0].InputHash.Algorithm);
            dataHasher.AddData(File.ReadAllBytes("Resources/infile.txt"));

            VerificationPolicy policy = new KeyBasedVerificationPolicy(new X509Store(StoreName.Root), GetCertificateSubjectRdnSelector());
            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = dataHasher.GetHash(),
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

            IDataHasher dataHasher = new DataHasher(signature.GetAggregationHashChains()[0].InputHash.Algorithm);
            dataHasher.AddData(File.ReadAllBytes("Resources/infile.txt"));

            VerificationPolicy policy = new CalendarBasedVerificationPolicy();
            VerificationContext context = new VerificationContext(signature)
            {
                DocumentHash = dataHasher.GetHash(),
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