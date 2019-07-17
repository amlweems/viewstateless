
namespace ViewStateless {

    class FakeMasterKeyProvider : IMasterKeyProvider {

        private CryptographicKey encryptionKey;
        private CryptographicKey validationKey;

        public FakeMasterKeyProvider(string encryptionKey, string validationKey) {
            byte[] array;

            array = CryptoUtil.HexToBinary(encryptionKey);
            if (array != null && array.Length != 0)
            {
                this.encryptionKey = new CryptographicKey(array);
            }

            array = CryptoUtil.HexToBinary(validationKey);
            if (array != null && array.Length != 0)
            {
                this.validationKey = new CryptographicKey(array);
            }
        }

        // Token: 0x06004D91 RID: 19857
        public CryptographicKey GetEncryptionKey() {
            return this.encryptionKey;
        }

        // Token: 0x06004D92 RID: 19858
        public CryptographicKey GetValidationKey() {
            return this.validationKey;
        }

    }
}