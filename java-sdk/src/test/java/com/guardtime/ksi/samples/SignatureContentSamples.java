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

import com.guardtime.ksi.Reader;
import com.guardtime.ksi.exceptions.KSIException;
import com.guardtime.ksi.publication.PublicationData;
import com.guardtime.ksi.unisignature.*;
import org.junit.Test;

import java.io.IOException;

public class SignatureContentSamples extends KsiSamples {

    /**
     * Prints information found in the given signature's publication record.
     */
    @Test
    public void printPublicationRecord() throws IOException, KSIException {
        Reader reader = getReader();

        KSISignature signature = reader.read(getFile("signme.txt.extended-ksig"));

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
    public void printAggregationHashChain() throws IOException, KSIException {
        Reader reader = getReader();

        KSISignature signature = reader.read(getFile("signme.txt.extended-ksig"));

        for (AggregationHashChain ahc : signature.getAggregationHashChains()) {
            System.out.println("printAggregationHashChain > link count > " + ahc.getChainLinks().size());
        }
    }

    /**
     * Prints information found in the given signature's calendar hash chain.
     */
    @Test
    public void printCalendarHashChain() throws IOException, KSIException {
        Reader reader = getReader();

        KSISignature signature = reader.read(getFile("signme.txt.extended-ksig"));

        CalendarHashChain chc = signature.getCalendarHashChain();
        System.out.println("printCalendarHashChain > publication time > " + chc.getPublicationTime());
    }

    /**
     * Prints the client IDs of the identity metadata in the signature.
     */
    @Test
    public void printIdentity() throws IOException, KSIException {
        Reader reader = getReader();
        KSISignature signature = reader.read(getFile("signme.txt.extended-ksig"));

        System.out.println(identityClientIdToString(signature.getAggregationHashChainIdentity()));
    }

    /**
     * Prints the signing (aggregation) time of the signature.
     */
    @Test
    public void printSigningTime() throws IOException, KSIException {
        Reader reader = getReader();

        KSISignature signature = reader.read(getFile("signme.txt.extended-ksig"));

        System.out.println("printSigningTime > " + signature.getAggregationTime());
    }

    /**
     * Prints the RSA signature type of the calendar authentication record.
     */
    @Test
    public void printCalendarAuthenticationRecord() throws IOException, KSIException {
        Reader reader = getReader();

        KSISignature signature = reader.read(getFile("signme.txt.unextended-ksig"));

        CalendarAuthenticationRecord car = signature.getCalendarAuthenticationRecord();
        PublicationData publicationData = car.getPublicationData();

        SignatureData signatureData = car.getSignatureData();
        publicationData.getPublicationString();

        System.out.println("printCalendarAuthenticationRecord > signature type > " + signatureData.getSignatureType());
    }
}
