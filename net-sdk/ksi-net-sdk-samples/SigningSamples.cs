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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Guardtime.KSI;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Service;
using Guardtime.KSI.Signature;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guardtime.Ksi.Samples
{
    [TestClass]
    public class SigningSamples : KsiSamples
    {
        /// <summary>
        /// Creates a sample file, then signs it and stores the signature in a file.
        /// </summary>
        [TestMethod]
        public void CreateAndSignSampleFile()
        {
            KSI.Ksi ksi = GetKsi();

            // Let's create a file to be singed
            string inputFileName = "sample-file-for-signing.txt";
            File.WriteAllText(inputFileName, "Sample file, generated for signing!");

            IKsiSignature signature;

            using (FileStream stream = File.OpenRead(inputFileName))
            {
                // Sign it, the hash of the document is computed implicitly by the sign method
                signature = ksi.Sign(stream);
            }

            // Persist signature to file
            using (FileStream stream = File.Create("sample-file-for-signing.txt.ksig"))
            {
                signature.WriteTo(stream);
            }
        }

        /// <summary>
        /// Sign a byte array, in this example created from a simple line of text.
        /// </summary>
        [TestMethod]
        public void SignSampleByteArray()
        {
            KSI.Ksi ksi = GetKsi();

            // Whenever signing text data, make sure you control and know what the character set
            // (encoding) was otherwise you may have trouble in the verification later.
            byte[] document = Encoding.UTF8.GetBytes("This is my document");

            // Sign it, the hash of the document is computed implicitly by the sign method
            IKsiSignature signature = ksi.Sign(document);

            // Persist signature to file
            //using (FileStream stream = File.OpenRead("sample-file-for-signing.txt.ksig"))
            //{
            //    signature.WriteTo(stream);
            //}
        }

        /// <summary>
        /// Compute the hash of the signed document first using an input stream and then provide the hash to the signing method.
        /// </summary>
        [TestMethod]
        public void SignHashDirectly()
        {
            KSI.Ksi ksi = GetKsi();

            // Compute the hash first, use the input stream to provide the data to save memory for
            // hashing very large documents
            // In this example we simply use an input stream from an array of bytes but in practice it
            // could be file input stream from a very large file (several GB)
            IDataHasher dataHasher = KsiProvider.CreateDataHasher();

            using (MemoryStream stream = new MemoryStream())
            {
                byte[] data = Encoding.UTF8.GetBytes("Imagine this is a large file");
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                dataHasher.AddData(stream);
            }

            // Provide the signing method with the computed hash instead of document itself
            IKsiSignature signature = ksi.Sign(dataHasher.GetHash());

            // Persist signature to file
            //using (FileStream stream = File.OpenRead("sample-file-for-signing.txt.ksig"))
            //{
            //    signature.WriteTo(stream);
            //}
        }

        /// <summary>
        /// Signs numbers 1 - 50 (as text) using client side aggregation (block signer). The Merkle tree
        /// is built locally and only a single request is sent to KSI Gateway. For each item individual
        /// KSI signature is returned. This helps achieving a great performance when a huge number of
        /// files are needed to be signed without overloading the KSI GW.
        /// </summary>
        [TestMethod]
        public void SignMultipleItemsWithLocalAggregation()
        {
            BlockSigner ksiBlockSigner = new BlockSigner(GetKsiService());
            int itemCount = 50;

            // Add the items that need to be signed to the block signer
            IDataHasher dh = KsiProvider.CreateDataHasher(HashAlgorithm.Sha2256);

            for (int i = 1; i <= itemCount; i++)
            {
                dh.Reset();
                dh.AddData(Encoding.UTF8.GetBytes(i.ToString()));
                ksiBlockSigner.Add(dh.GetHash());
            }

            // Submit the signing request
            IEnumerable<IKsiSignature> signatures = ksiBlockSigner.Sign();

            // Just to illustrate that there are as many signatures as items
            Assert.AreEqual(itemCount, signatures.Count());
            // Store the signatures as needed
            // ...
        }

        /// <summary>
        /// Besides performance optimization, client side aggregation can be also used by embedding
        /// metadata. This can be used, for instance, for linking the user identity authenticated by 3rd
        /// party provider (in the same way as KSI GW does for its users). Although, the metadata fields
        /// are fixed and named after how KSI infrastructure uses them, its up to the use case what is
        /// the content and interpretation of metadata to be embedded, KSI signature just ensures its
        /// integrity.
        /// </summary>
        [TestMethod]
        public void LinkUserIdToSignature()
        {
            BlockSigner ksiBlockSigner = new BlockSigner(GetKsiService());
            IDataHasher dh = KsiProvider.CreateDataHasher(HashAlgorithm.Sha2256);

            // This is the data we are signing
            string data = "data";
            dh.AddData(Encoding.UTF8.GetBytes(data));

            // Suppose that this is the user that initiated the signing
            // and it has been verified using a 3rd party authentication provider (e.g. LDAP)
            string userId = "john.smith";

            // Add both, the data and the user to the block signer
            ksiBlockSigner.Add(dh.GetHash(), new IdentityMetadata(userId));
            IKsiSignature[] signatures = ksiBlockSigner.Sign().ToArray();

            // We should get only one signature as we only had one item that we signed
            Assert.AreEqual(1, signatures.Length);

            // Print the last part of the identity to show john.smith is there
            IIdentity[] identity = signatures[0].GetIdentity().ToArray();
            Console.WriteLine("User: " + identity[identity.Length - 1].ClientId);
            // Store the signature as needed
            // ...
        }
    }
}