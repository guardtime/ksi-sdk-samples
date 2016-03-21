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

using System.IO;
using System.Security.Cryptography.X509Certificates;
using Guardtime.Ksi.Samples.Properties;
using Guardtime.KSI.Crypto.Microsoft.Crypto;
using Guardtime.KSI.Publication;
using Guardtime.KSI.Service;
using Guardtime.KSI.Signature;
using Guardtime.KSI.Trust;
using Guardtime.KSI;
using Guardtime.KSI.Crypto.Microsoft;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guardtime.Ksi.Samples
{
    [TestClass]
    public class KsiSamples
    {
        private static readonly KSI.Ksi Ksi;
        private static readonly IKsiService KsiService;
        private static readonly CertificateSubjectRdnSelector CertificateSubjectRdnSelector;

        static KsiSamples()
        {
            // The end point URL of the Aggregation service, needed for signing, e.g. http://host.net:8080/gt-signingservice.
            string signingServiceUrl = Settings.Default.HttpSigningServiceUrl;

            // The end point URL of the Extender service, needed for extending signature, e.g. *http://host.net:8081/gt-extendingservice
            string extendingServiceUrl = Settings.Default.HttpExtendingServiceUrl;

            // The publications file URL, needed for signature verification, e.g. http://verify.guardtime.com/ksi-publications.bin 
            string publicationsFileUrl = Settings.Default.HttpPublicationsFileUrl;

            // The credentials to access the KSI signing service
            ServiceCredentials signingServiceCredentials =
                new ServiceCredentials(Settings.Default.HttpSigningServiceUser, Settings.Default.HttpSigningServicePass);

            // The credentials to access the KSI extending service
            ServiceCredentials extendingServiceCredentials =
                new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, Settings.Default.HttpExtendingServicePass);

            HttpKsiServiceProtocol ksiServiceProtocol = new HttpKsiServiceProtocol(signingServiceUrl,
                extendingServiceUrl, publicationsFileUrl);

            // Certificate selector, used to filter which certificates are trusted when verifying the RSA signature.
            // We only trust certificates, that have issued to the particular e-mail address
            CertificateSubjectRdnSelector = new CertificateSubjectRdnSelector("E=publications@guardtime.com");

            // This is the KSI context which holds the references to the Aggregation service, Extender
            // service and other configuration data to perform the various operations.
            KsiService =
                new KsiService(
                    ksiServiceProtocol,
                    signingServiceCredentials,
                    ksiServiceProtocol,
                    extendingServiceCredentials,
                    ksiServiceProtocol,
                    new PublicationsFileFactory(new PkiTrustStoreProvider(new X509Store(StoreName.Root), CertificateSubjectRdnSelector)));

            Ksi = new KSI.Ksi(GetKsiService());

            // Set crypto provider to be used. Currently MicrosoftCryptoProvider and BouncyCastleCryptoProvider are available.
            KsiProvider.SetCryptoProvider(new MicrosoftCryptoProvider());
        }

        protected static CertificateSubjectRdnSelector GetCertificateSubjectRdnSelector()
        {
            return CertificateSubjectRdnSelector;
        }

        protected static IKsiService GetKsiService()
        {
            return KsiService;
        }

        protected static KSI.Ksi GetKsi()
        {
            return Ksi;
        }

        /// <summary>
        /// Load extended signature from file
        /// </summary>
        /// <returns></returns>
        protected static IKsiSignature LoadExtendedSignature()
        {
            IKsiSignature signature;
            using (FileStream stream = new FileStream("Resources/infile_2016-02-14-extended.ksig", FileMode.Open))
            {
                signature = new KsiSignatureFactory().Create(stream);
            }
            return signature;
        }

        /// <summary>
        /// Load unextended signature from file
        /// </summary>
        /// <returns></returns>
        protected static IKsiSignature LoadUnextendedSignature()
        {
            IKsiSignature signature;
            using (FileStream stream = new FileStream("Resources/infile_2016-02-14.ksig", FileMode.Open))
            {
                signature = new KsiSignatureFactory().Create(stream);
            }
            return signature;
        }
    }
}