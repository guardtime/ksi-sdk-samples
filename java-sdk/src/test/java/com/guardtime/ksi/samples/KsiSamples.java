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

import com.guardtime.ksi.*;
import com.guardtime.ksi.exceptions.KSIException;
import com.guardtime.ksi.service.KSIExtendingClientServiceAdapter;
import com.guardtime.ksi.service.KSISigningClientServiceAdapter;
import com.guardtime.ksi.service.client.*;
import com.guardtime.ksi.service.client.http.CredentialsAwareHttpSettings;
import com.guardtime.ksi.service.client.http.HttpSettings;
import com.guardtime.ksi.service.http.simple.SimpleHttpExtenderClient;
import com.guardtime.ksi.service.http.simple.SimpleHttpPublicationsFileClient;
import com.guardtime.ksi.service.http.simple.SimpleHttpSigningClient;
import com.guardtime.ksi.trust.X509CertificateSubjectRdnSelector;
import com.guardtime.ksi.unisignature.Identity;
import com.guardtime.ksi.unisignature.verifier.policies.ContextAwarePolicyAdapter;

import java.io.File;
import java.io.IOException;
import java.security.cert.CertSelector;

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
     * SimpleHttpClient implementation (see details in {@link #setUpKsi()}.
     */
    private KSISigningClient ksiSigningClient;
    private KSIExtenderClient ksiExtenderClient;
    private KSIPublicationsFileClient ksiPublicationsFileClient;

    /**
     * The end-user interfaces for the various KSI operations.
     */
    private Signer signer;
    private Extender extender;
    private Reader reader;
    private PublicationsHandler publicationsHandler;
    private Verifier verifier;

    /**
     * Initialize instances for the end-user interfaces before running the samples / tests in sub-classes by specifying the end
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

        // Create reader for reading existing KSI signatures, each signature read is automatically verified
        // using
        reader = new SignatureReader(ContextAwarePolicyAdapter.createInternalPolicy());

        // Create the signer for signing data
        ksiSigningClient = new SimpleHttpSigningClient(new CredentialsAwareHttpSettings(aggregatorUrl, credentials));
        signer = new SignerBuilder().setSigningService(new KSISigningClientServiceAdapter(ksiSigningClient)).build();

        // Create publications handler to be used for signature extending and verification
        ksiPublicationsFileClient = new SimpleHttpPublicationsFileClient(new HttpSettings(publicationsFileUrl));
        publicationsHandler = new PublicationsHandlerBuilder().setKsiProtocolPublicationsFileClient(ksiPublicationsFileClient).setPublicationsFileCertificateConstraints(certSelector).build();

        // Create extender for extending KSI signatures
        ksiExtenderClient = new SimpleHttpExtenderClient(new CredentialsAwareHttpSettings(extenderUrl, credentials));
        extender = new ExtenderBuilder().setExtendingService(new KSIExtendingClientServiceAdapter(ksiExtenderClient)).setKsiProtocolPublicationsFileClient(ksiPublicationsFileClient).setPublicationsFileCertificateConstraints(certSelector).build();

        // Create verifier for verifying signatures
        verifier = new SignatureVerifier();
    }


    /**
     * Close resources after the tests have been finished. Called from sub-classes.
     */
    protected void tearDownKsi() {
        try {
            if (signer != null)
                signer.close();
            if (extender != null)
                extender.close();
        } catch (IOException e) {
            System.err.println(e.getMessage());
        }
    }


    protected Signer getSigner() {
        return signer;
    }

    protected Extender getExtender() {
        return extender;
    }

    protected Reader getReader() {
        return reader;
    }

    protected PublicationsHandler getPublicationsHandler() {
        return publicationsHandler;
    }

    protected Verifier getVerifier() {
        return verifier;
    }

    protected KSISigningClient getKsiSigningClient() {
        return ksiSigningClient;
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

    /**
     * Converts the client ID found in the identity chain to a string using
     * 'space,colon,colon,space' as a separator.
     *
     * @return Client ID from identity metadata as string.
     */
    public String identityClientIdToString(Identity[] identity) {
        StringBuilder stringBuilder = new StringBuilder();
        for (int k = 0; k < identity.length; k++) {
            stringBuilder.append(identity[k].getDecodedClientId());
            if (k < identity.length - 1)
                stringBuilder.append(" :: ");
        }
        return stringBuilder.toString();
    }
}
