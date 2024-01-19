using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Encryption
{
    public interface ICryptography
    {
        string Encrypt(string plainText);

        string Decrypt(string cypher);

        string EncryptLarge(string dataToEncrypt);

        string DecryptLarge(string cypher);
    }
}
