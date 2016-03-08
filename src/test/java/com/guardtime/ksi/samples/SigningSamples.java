package com.guardtime.ksi.samples;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PrintWriter;
import java.nio.charset.Charset;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import com.guardtime.ksi.KSI;
import com.guardtime.ksi.exceptions.KSIException;
import com.guardtime.ksi.hashing.DataHasher;
import com.guardtime.ksi.hashing.HashAlgorithm;
import com.guardtime.ksi.unisignature.KSISignature;

public class SigningSamples extends KsiSamples {

    @Before
    public void setUp() throws KSIException {
        setUpKsi();
    }

    @After
    public void tearDown() {
        tearDownKsi();
    }

    /**
     * Creates a sample file, then signs it and stores the signature in a file.
     */
    @Test
    public void createAndSignSampleFile() throws IOException, KSIException {
        KSI ksi = getKsi();

        // Let's create a file to be singed
        File fileToSign = new File("sample-file-for-signing.txt");
        PrintWriter writer = new PrintWriter(fileToSign);
        writer.println("Sample file, generated for signing!");
        writer.close();

        // Sign it, the hash of the document is computed implicitly by the sign method
        KSISignature signature = ksi.sign(fileToSign);

        // Persist signature to file
        FileOutputStream fileOutputStream = new FileOutputStream(new File("sample-file-for-signing.txt.ksig"));
        signature.writeTo(fileOutputStream);
        fileOutputStream.close();
    }

    /**
     * Sign a byte array, in this example created from a simple line of text.
     */
    @Test
    public void signSampleByteArray() throws IOException, KSIException {
        KSI ksi = getKsi();

        // Whenever signing text data, make sure you control and know what the character set
        // (encoding) was otherwise you may have trouble in the verification later.
        byte[] document = "This is my document".getBytes(Charset.forName("UTF-8"));

        // Sign it, the hash of the document is computed implicitly by the sign method
        @SuppressWarnings("unused")
        KSISignature signature = ksi.sign(document);

        // Persist signature to file
        // signature.writeTo(...);
    }

    /**
     * Compute the hash of the signed document first using an input stream and then provide the hash
     * to the signing method.
     */
    @Test
    public void signHashDirectly() throws IOException, KSIException {
        KSI ksi = getKsi();

        // Compute the hash first, use the input stream to provide the data to save memory for
        // hashing very large documents
        // In this example we simply use an input stream from an array of bytes but in practice it
        // could be file input stream from a very large file (several GB)
        DataHasher dh = new DataHasher(HashAlgorithm.SHA2_256);
        dh.addData(new ByteArrayInputStream("Imagine this is a large file".getBytes(Charset.forName("UTF-8"))));

        // Provide the signing method with the computed hash instead of document itself
        @SuppressWarnings("unused")
        KSISignature signature = ksi.sign(dh.getHash());

        // Persist signature to file
        // signature.writeTo(...);
    }



}
