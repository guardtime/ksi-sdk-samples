package com.guardtime.ksi.samples;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PrintWriter;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import com.guardtime.ksi.KSI;
import com.guardtime.ksi.exceptions.KSIException;
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
     * Creates a sample file and then signs it.
     */
    @Test
    public void createAndSignSampleFile() throws IOException, KSIException {
        KSI ksi = getKsi();

        // Let's create a file to be singed
        File fileToSign = new File("sample-file-for-signing.txt");
        PrintWriter writer = new PrintWriter(fileToSign);
        writer.println("Sample file, generated for signing!");
        writer.close();

        // Sign it
        KSISignature signature = ksi.sign(fileToSign);

        // Persist signature to file
        FileOutputStream fileOutputStream = new FileOutputStream(new File("sample-file-for-signing.txt.ksig"));
        signature.writeTo(fileOutputStream);
        fileOutputStream.close();
    }
}
