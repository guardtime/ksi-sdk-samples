/*
 * Copyright 2013-2016 Guardtime, Inc.
 *
 * This file is part of the Guardtime client SDK.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not use this file except
 * in compliance with the License. You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0 Unless required by applicable law or agreed to in
 * writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES, CONDITIONS, OR OTHER LICENSES OF ANY KIND, either express or implied. See the License
 * for the specific language governing permissions and limitations under the License. "Guardtime"
 * and "KSI" are trademarks or registered trademarks of Guardtime, Inc., and no license to
 * trademarks is granted; Guardtime reserves and retains all trademark rights.
 */
package com.guardtime.ksi.samples;

import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import com.guardtime.ksi.KSI;
import com.guardtime.ksi.exceptions.KSIException;
import com.guardtime.ksi.publication.PublicationData;
import com.guardtime.ksi.publication.PublicationRecord;
import com.guardtime.ksi.unisignature.KSISignature;

/**
 * Samples related to extending KSI signatures.
 */
public class ExtendingSamples extends KsiSamples {

    @Before
    public void setUp() throws KSIException {
        setUpKsi();
    }

    @After
    public void tearDown() {
        tearDownKsi();
    }

    /**
     * Check if signature has been extended to a publication or not.
     */
    @Test
    public void checkExtended() throws IOException, KSIException {
        KSI ksi = getKsi();
        KSISignature signature = ksi.read(getFile("signme.txt.unextended-ksig"));

        if (signature.isExtended()) {
            System.out.println(
                    "checkExtended > publication time > " + signature.getPublicationRecord().getPublicationTime());
        } else {
            System.out.println("checkExtended > signature not extended");
        }
    }

    /**
     * Finds the first publication in the publications file after the given date and prints its
     * references.
     */
    @Test
    public void printPublicationInfo() throws IOException, KSIException, ParseException {
        KSI ksi = getKsi();

        Date publicationDate = new SimpleDateFormat("YYYY-MM-dd").parse("2016-02-01");
        PublicationRecord publicationRecord = ksi.getPublicationsFile().getPublicationRecord(publicationDate);

        for (String s : publicationRecord.getPublicationReferences()) {
            System.out.println("printPublicationInfo > publication reference > " + s);
        }
    }

    /**
     * Extends the given signature to the latest publication found in the publications file in case
     * the signature was not extended or was extended to an earlier publication.
     */
    @Test
    public void reExtendToLatestPublication() throws IOException, KSIException {
        KSI ksi = getKsi();
        KSISignature signature = ksi.read(getFile("signme.txt.unextended-ksig"));

        PublicationRecord latestPublicationRecord = ksi.getPublicationsFile().getLatestPublication();
        Date latestPublicationTime = latestPublicationRecord.getPublicationTime();
        if (!signature.isExtended()
                || signature.getPublicationRecord().getPublicationTime().before(latestPublicationTime)) {
            KSISignature extendedSignature = ksi.extend(signature, latestPublicationRecord);

            if (extendedSignature.isExtended()) {
                System.out.println("reExtendToLatestPublication > signature extended to publication > "
                        + extendedSignature.getPublicationRecord().getPublicationTime());

                // Store the extended signature
                // ...
            }
        }
    }

    /**
     * Extends signature to the closes publication according to signing time.
     */
    @Test
    public void extendToClosestPublication() throws IOException, KSIException {
        KSI ksi = getKsi();

        // Read an existing signature from file, assume it to be not extended
        KSISignature signature = ksi.read(getFile("signme.txt.unextended-ksig"));

        // Extends the signature to the closest publication found in the
        // publications file
        // Assumes signature is not extended and at least one publication after
        // the signature obtained
        KSISignature extendedSignature = ksi.extend(signature);

        // Double check if signature was extended
        if (extendedSignature.isExtended()) {
            System.out.println("extendToClosesPublication > extended to publication > "
                    + extendedSignature.getPublicationRecord().getPublicationTime());
        } else {
            System.out.println("extendToClosesPublication > signature not extended");
        }

        // Store the extended signature
        // ...
    }

    /**
     * Extends signature to a given date.
     */
    @Test
    public void extendToGivenPublicationDate() throws IOException, KSIException, ParseException {
        KSI ksi = getKsi();

        KSISignature signature = ksi.read(getFile("signme.txt.unextended-ksig"));
        Date publicationDate = new SimpleDateFormat("yyyy-MM-dd").parse("2016-03-01");

        PublicationRecord publicationRecord = ksi.getPublicationsFile().getPublicationRecord(publicationDate);

        System.out.println("extendToGivenPublicationDate > trying to extend signature to publication > "
                + publicationRecord.getPublicationTime());

        KSISignature extendedSignature = ksi.extend(signature, publicationRecord);

        if (extendedSignature.isExtended()) {
            System.out.println("extendToGivenPublicationDate > signature extended to publication > "
                    + extendedSignature.getPublicationRecord().getPublicationTime());
            // Store the extended signature
            // ...
        } else {
            System.out.println("extendToGivenPublicationDate > signature not extended");
        }

    }

    /**
     * Extend signature to a publication specified by publication code.
     */
    @Test
    public void extendToGivenPublicationCode() throws IOException, KSIException, ParseException {
        KSI ksi = getKsi();
        KSISignature signature = ksi.read(getFile("signme.txt.unextended-ksig"));

        // Publication code for 15.03.2016
        Date publicationDate = new PublicationData(
                "AAAAAA-CW45II-AAKWRK-F7FBNM-KB6FNV-DYYFW7-PJQN6F-JKZWBQ-3OQYZO-HCB7RA-YNYAGA-ODRL2V")
                        .getPublicationTime();

        PublicationRecord publicationRecord = ksi.getPublicationsFile().getPublicationRecord(publicationDate);
        KSISignature extendedSignature = ksi.extend(signature, publicationRecord);

        if (extendedSignature.isExtended()) {
            System.out.println("extendToGivenPublicationCode > signature extended to publication > "
                    + extendedSignature.getPublicationRecord().getPublicationTime());

            // Store the extended signature
            // ...
        } else {
            System.out.println("extendToGivenPublicationCode > signature not extended");
        }
    }
}
