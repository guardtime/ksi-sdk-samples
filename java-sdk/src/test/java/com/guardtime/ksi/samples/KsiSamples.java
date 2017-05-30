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

import java.io.File;
import java.io.IOException;
import java.security.cert.CertSelector;

import com.guardtime.ksi.KSI;
import com.guardtime.ksi.KSIBuilder;
import com.guardtime.ksi.exceptions.KSIException;
import com.guardtime.ksi.service.client.KSIExtenderClient;
import com.guardtime.ksi.service.client.KSIServiceCredentials;
import com.guardtime.ksi.service.client.ServiceCredentials;
import com.guardtime.ksi.service.client.http.HttpClientSettings;
import com.guardtime.ksi.service.http.simple.SimpleHttpClient;
import com.guardtime.ksi.trust.X509CertificateSubjectRdnSelector;

/**
 * The samples are implemented as JUnit tests. This is the base class that contains the common parts
 * for all the tests. See  {@see #setUpKsi()} how the KSI context is set up.
 */
public abstract class KsiSamples {

    /**
     * The end point URL of the Aggregation service, needed for signing, e.g.
     * http://host.net:8080/gt-signingservice. Use the JVM property aggregator.url to set to correct
     * value.
     */
    private String aggregatorUrl;

    /**
     * The end point URL of the Extender service, needed for extending signature, e.g.
     * http://host.net:8081/gt-extendingservice. Use the JVM property extender.url to set the
     * correct value.
     */
    private String extenderUrl;

    /**
     * The credentials to access the KSI Aggregation and/or Extending service, in order to avoid
     * hard coding them we use the Java system properties ksi.login.id and ksi.login.key to pass
     * them. Make sure you provide them to JVM when running the examples (e.g. java
     * -Dksi.login.id=... -Dksi.login.key=...
     */
    private ServiceCredentials credentials;

    /**
     * The publications file URL, needed for signature verification, e.g.
     * http://verify.guardtime.com/ksi-publications.bin for Guardtime KSI service. Use the JVM
     * property publications.file.url to override if needed.
     */
    private String publicationsFileUrl;

    /**
     * Certificate selector, used to filter which certificates are trusted when verifying the RSA
     * signature
     */
    private CertSelector certSelector;

    /**
     * The modularity of KSI Java SDK enables multiple implementations of the "clients" for signing,
     * extending in communication with the KSI Gateway. In these examples we use the
     * SimpleHttpClient implementation.
     */
    private SimpleHttpClient simpleHttpClient;

    /**
     * This is the KSI context which holds the references to the Aggregation service, Extender
     * service and other configuration data to perform the various operations. See
     * {@link #setUpKsi()} for how it is set up.
     */
    private KSI ksi;

    /**
     * Setup KSI context before running the samples / tests in sub-classes by specifying the end
     * points of the Aggregator and Extender services, the publications file location and the
     * credentials to access the services. Called from sub-classes before running the tests.
     */
    protected void setUpKsi() throws KSIException {
        // Get the Aggregator and Extender service end point URLs from JVM
        // properties
        aggregatorUrl = System.getProperty("aggregator.url");
        extenderUrl = System.getProperty("extender.url");

        // Get the Aggregator and Extender access credentials from URL
        String loginId = System.getProperty("ksi.login.id");
        String loginKey = System.getProperty("ksi.login.key");
        credentials = new KSIServiceCredentials(loginId, loginKey);

        // Override the publications file URL, if the JVM property is present
        publicationsFileUrl =
                System.getProperty("publications.file.url", "http://verify.guardtime.com/ksi-publications.bin");

        // We only trust certificates in verification of the publications file,
        // that have issued to the particular e-mail address
        certSelector = new X509CertificateSubjectRdnSelector("E=publications@guardtime.com");

        // Setup KSI context, we use the SimpleHttpClient so we provide the
        // settings through the HttpClientSettings class
        HttpClientSettings settings =
                new HttpClientSettings(aggregatorUrl, extenderUrl, publicationsFileUrl, credentials);

        simpleHttpClient = new SimpleHttpClient(settings);

        ksi = new KSIBuilder().setKsiProtocolSignerClient(simpleHttpClient)
                .setKsiProtocolExtenderClient(simpleHttpClient).setKsiProtocolPublicationsFileClient(simpleHttpClient)
                .setPublicationsFileTrustedCertSelector(certSelector).build();
    }

    /**
     * Close KSI after the tests have been finished. Called from sub-classes.
     */
    protected void tearDownKsi() {
        if (ksi == null)
            return;

        try {
            ksi.close();
        } catch (IOException e) {
            System.err.println(e.getMessage());
        }
    }

    /**
     * Make the KSI context accessible to the samples in the sub-classes.
     */
    protected KSI getKsi() {
        return ksi;
    }

    /**
     * Make the client accessible to the samples in the sub-classes.
     */
    protected SimpleHttpClient getSimpleHttpClient() {
        return simpleHttpClient;
    }

    /**
     * Utility method to access the file in the test/resources folder.
     * 
     * @param resourceName The name of the file.
     * @return The file.
     */
    public File getFile(String resourceName) {
        return new File(getClass().getClassLoader().getResource(resourceName).getFile());
    }

    public KSIExtenderClient getKsiExtenderClient(){
        return simpleHttpClient;
    }
}
