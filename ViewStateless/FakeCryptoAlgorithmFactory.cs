//------------------------------------------------------------------------------
// <copyright file="FakeCryptoAlgorithmFactory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace ViewStateless {
    using System;
    using System.Security.Cryptography;

    // Can create cryptographic algorithms from a given <machineKey> element

    internal sealed class FakeCryptoAlgorithmFactory : ICryptoAlgorithmFactory {

        private string encryption;
        private string validation;

        public FakeCryptoAlgorithmFactory(string encryption, string validation) {
            this.encryption = encryption;
            this.validation = validation;
        }

        public SymmetricAlgorithm GetEncryptionAlgorithm() {
                // We suppress CS0618 since some of the algorithms we support are marked with [Obsolete].
                // These deprecated algorithms are *not* enabled by default. Developers must opt-in to
                // them, so we're secure by default.
#pragma warning disable 618
                switch (this.encryption) {
                    case "AES":
                    case "Auto": // currently "Auto" defaults to AES
                        return CryptoAlgorithms.CreateAes();

                    case "DES":
                        return CryptoAlgorithms.CreateDES();

                    case "3DES":
                        return CryptoAlgorithms.CreateTripleDES();

                    default:
                        return null; // unknown
#pragma warning restore 618
                }
        }

        public KeyedHashAlgorithm GetValidationAlgorithm() {
            switch (this.validation) {
                case "SHA1":
                    return CryptoAlgorithms.CreateHMACSHA1();

                case "HMACSHA256":
                    return CryptoAlgorithms.CreateHMACSHA256();

                case "HMACSHA384":
                    return CryptoAlgorithms.CreateHMACSHA384();

                case "HMACSHA512":
                    return CryptoAlgorithms.CreateHMACSHA512();

                default:
                    return null; // unknown
            }
        }

    }
}
