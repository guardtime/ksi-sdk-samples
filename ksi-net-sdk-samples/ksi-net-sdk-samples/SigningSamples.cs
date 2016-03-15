using System.Text;
using System.IO;
using Guardtime.KSI;
using Guardtime.KSI.Hashing;
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
    }
}