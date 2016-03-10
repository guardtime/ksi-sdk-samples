# KSI Java SDK Samples
Guardtime Keyless Signature Infrastructure (KSI) is an industrial scale blockchain platform that cryptographically ensures data integrity and proves time of existence. Its keyless signatures, based on hash chains, link data to global calendar blockchain. The checkpoints of the blockchain, published in newspapers and electronic media, enable long term integrity of any digital asset without the need to trust any system. There are many applications for KSI, a classical example is signing of any type of logs - system logs, financial transactions, call records, etc. For more, see https://guardtime.com

This repository contains samples on how to use the KSI Java SDK for signing, signature extension and verification.
## Usage
Explore the code on Github or clone/download it and use your favorite editor to understand how to use the Java KSI SDK. The samples are implemented as JUnit tests and can be found in the src/test folder. In order to run the examples and test KSI:
 - Download / clone the repository
 - If you are not a KSI user, request for a trial account here https://guardtime.com/blockchain-developers
 - Run tests using Maven, providing the correct Aggregator and Extender service end point URLs and access credentials

```
mvn -Daggregator.url="http://host.net:8080/gt-signingservice" -Dextender.url="http://host.net:8081/gt-extendingservice" -Dksi.login.id=joe -Dksi.login.key=secret test
```

Additionally you can override the location of the publications file using 
```
-Dpublications.file.url="http://host.net/...."
```
The default is Guardtime KSI service publications file http://verify.guardtime.com/ksi-publications.bin

## License
See LICENSE file.