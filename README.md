# KSI SDK Samples
Guardtime's KSI Blockchain is an industrial scale blockchain platform that cryptographically ensures data integrity and proves time of existence. The KSI signatures, based on hash chains, link data to this global calendar blockchain. The checkpoints of the blockchain, published in newspapers and electronic media, enable long term integrity of any digital asset without the need to trust any system. There are many applications for KSI, a classical example is signing of any type of logs - system logs, financial transactions, call records, etc. For more, see https://guardtime.com

This repository contains samples on how to use the KSI SDK-s for signing, signature extension and verification.
## Usage
Explore the code on Github or clone/download it and use your favorite editor to understand how to use the SDK-s. In order to run the examples you need to have access to KSI service, the simplest is to request a trial account here https://guardtime.com/blockchain-developers

### Java SDK
The samples are implemented as JUnit tests and can be found in the src/test folder. In order to run the examples and test KSI:
 - Download / clone the repository
 - Run tests using Maven, providing the correct Aggregator and Extender service end point URLs and access credentials

```
mvn -Daggregator.url="http://host.net:8080/gt-signingservice" -Dextender.url="http://host.net:8081/gt-extendingservice" -Dksi.login.id=joe -Dksi.login.key=secret test
```

Additionally you can override the location of the publications file using 
```
-Dpublications.file.url="http://host.net/...."
```
The default is Guardtime KSI service publications file http://verify.guardtime.com/ksi-publications.bin

### .NET SDK
The samples are implemented as Unit tests. In order to run the examples and test KSI:
 - Download / clone the repository
 - Open .NET SDK samples solution located in folder named 'net-sdk'.
 - Set the correct URLs and access credentials in app.config. Application setting keys:
   - Aggregator service end point URL: HttpSigningServiceUrl
   - Aggregator service access credentials: HttpSigningServiceUser:HttpSigningServicePass
   - Extender service end point URL: HttpExtendingServiceUrl
   - Extender service access credentials: HttpExtendingServiceUser:HttpExtendingServicePass
   - Publications file location: HttpPublicationsFileUrl
 - Execute Unit tests.

### C SDK
The samples are located in the [libksi](https://github.com/GuardTime/libksi) repository at src/example.
  - Download / clone the [libksi](https://github.com/GuardTime/libksi) repository
  - Run "autoreconf -iv"
  - Run "./configure"
  - Run "make test" for the automated tests
  - Run "make check" for  the example programs if you skipped the previous step

## License
See LICENSE file.
