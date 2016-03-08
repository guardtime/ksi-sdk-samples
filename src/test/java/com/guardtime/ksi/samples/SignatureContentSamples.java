package com.guardtime.ksi.samples;

import java.io.IOException;

import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import com.guardtime.ksi.KSI;
import com.guardtime.ksi.exceptions.KSIException;
import com.guardtime.ksi.publication.PublicationData;
import com.guardtime.ksi.unisignature.AggregationHashChain;
import com.guardtime.ksi.unisignature.CalendarAuthenticationRecord;
import com.guardtime.ksi.unisignature.CalendarHashChain;
import com.guardtime.ksi.unisignature.KSISignature;
import com.guardtime.ksi.unisignature.SignatureData;
import com.guardtime.ksi.unisignature.SignaturePublicationRecord;

public class SignatureContentSamples extends KsiSamples {

    @Before
    public void setUp() throws KSIException {
        setUpKsi();
    }

    @After
    public void tearDown() {
        tearDownKsi();
    }

    /**
     * Prints information found in the given signature's publication record.
     */
    @Test
    public void printPublicationRecord() throws IOException, KSIException {
        KSI ksi = getKsi();

        KSISignature signature = ksi.read(getFile("signme.txt.extended-ksig"));

        SignaturePublicationRecord spr = signature.getPublicationRecord();
        if (spr != null)
            System.out.println("printPublicationRecord > publication time >" + spr.getPublicationTime());
        else
            System.out.println("printPublicationRecord > No publication record in signature");
    }

    /**
     * Prints information found in the given signature's aggregation hash chain.
     */
    @Test
    public void printAhc() throws IOException, KSIException {
        KSI ksi = getKsi();
        KSISignature signature = ksi.read(getFile("signme.txt.extended-ksig"));

        for (AggregationHashChain ahc : signature.getAggregationHashChains()) {
            System.out.println("printAhc > link count > " + ahc.getChainLinks().size());
        }
    }

    /**
     * Prints information found in the given signature's calendar hash chain.
     */
    @Test
    public void printChc() throws IOException, KSIException {
        KSI ksi = getKsi();

        KSISignature signature = ksi.read(getFile("signme.txt.extended-ksig"));

        CalendarHashChain chc = signature.getCalendarHashChain();
        System.out.println("printChc > publication time > " + chc.getPublicationTime());
        System.out.println("printChc > registration time > " + chc.getRegistrationTime());

    }

    /**
     * Prints the identity in the signature.
     */
    @Test
    public void printIdentity() throws IOException, KSIException {
        KSI ksi = getKsi();
        KSISignature signature = ksi.read(getFile("signme.txt.extended-ksig"));

        System.out.println("printIdentity > " + signature.getIdentity());
    }

    /**
     * Prints the signing (aggregation) time of the signature.
     */
    @Test
    public void printSigningTime() throws IOException, KSIException {
        KSI ksi = getKsi();

        KSISignature signature = ksi.read(getFile("signme.txt.extended-ksig"));

        System.out.println("printSigningTime > " + signature.getAggregationTime());
    }

    /**
     * Prints the RSA signature type of the calendar authentication record.
     */
    @Test
    public void printCalendarAuthenticationRecord() throws IOException, KSIException {
        KSI ksi = getKsi();

        KSISignature signature = ksi.read(getFile("signme.txt.extended-ksig"));

        CalendarAuthenticationRecord car = signature.getCalendarAuthenticationRecord();
        PublicationData publicationData = car.getPublicationData();

        SignatureData signatureData = car.getSignatureData();
        publicationData.getPublicationString();

        System.out.println("printCalendarAuthenticationRecord > signature type > " + signatureData.getSignatureType());
    }
}
