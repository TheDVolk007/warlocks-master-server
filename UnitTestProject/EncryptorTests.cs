using Dal.Common;
using Newtonsoft.Json;
using NUnit.Framework;


namespace UnitTestProject
{
    [TestFixture]
    public class EncryptorTests
    {
        [Test]
        public void SuccessfulSHA256()
        {
            // Arange.
            const string source = "Test string";
            const string expectedResult = "a3e49d843df13c2e2a7786f6ecd7e0d184f45d718d1ac1a8a63e570466e489dd";
            var encryptor = new Encryptor();

            // Act.
            var result = encryptor.SHA256(source);

            // Assert.
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void SuccessfulEncryptStringToBytes()
        {
            // Arange.
            const string source = "Test string";
            const string expectedStringifiedResult = "\"KXqmf/qENWb20tzw+NLnlA==\"";
            var encryptor = new Encryptor();

            // Act.
            var result = encryptor.EncryptStringToBytes(source);
            var stringifiedResult = JsonConvert.SerializeObject(result);

            // Assert.
            Assert.AreEqual(expectedStringifiedResult, stringifiedResult);
        }

        [Test]
        public void SuccessfulDecryptStringFromBytes()
        {
            // Arange.
            const string stringifiedSource = "\"KXqmf/qENWb20tzw+NLnlA==\"";
            const string expectedResult = "Test string";
            var encryptor = new Encryptor();

            // Act.
            var stringifiedResult = JsonConvert.DeserializeObject<byte[]>(stringifiedSource);
            var result = encryptor.DecryptStringFromBytes(stringifiedResult);
            

            // Assert.
            Assert.AreEqual(expectedResult, result);
        }
    }
}