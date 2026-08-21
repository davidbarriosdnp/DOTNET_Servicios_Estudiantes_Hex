using System.Security.Cryptography;
using System.Text;

namespace Servicios_Estudiantes.Aplicacion.Utilidades;

public static class EncriptacionExtensiones
{
    /// <summary>
    /// Encripta una cadena de texto utilizando AES.
    /// </summary>
    /// <param name="textoPlano">El texto que se desea encriptar.</param>
    /// <param name="claveSecreta">La clave secreta de 32 caracteres (256 bits) para encriptar.</param>
    /// <returns>El texto encriptado en formato Base64.</returns>
    public static string EncriptarAes(this string textoPlano, string claveSecreta)
    {
        if (string.IsNullOrEmpty(textoPlano)) return textoPlano;
        
        // Ajustar la longitud de la clave a 32 bytes (256 bits) rellenando o cortando
        byte[] keyBytes = AjustarClave(claveSecreta);
        byte[] iv = new byte[16]; // Vector de inicialización de 16 bytes. En producción es recomendable generarlo de forma aleatoria y adjuntarlo al mensaje.
        
        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using MemoryStream msEncrypt = new MemoryStream();
        using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(textoPlano);
        }
        
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    /// <summary>
    /// Desencripta una cadena de texto encriptada previamente con AES.
    /// </summary>
    /// <param name="textoEncriptadoBase64">El texto encriptado en formato Base64.</param>
    /// <param name="claveSecreta">La misma clave secreta que se utilizó para encriptar.</param>
    /// <returns>El texto desencriptado original.</returns>
    public static string DesencriptarAes(this string textoEncriptadoBase64, string claveSecreta)
    {
        if (string.IsNullOrEmpty(textoEncriptadoBase64)) return textoEncriptadoBase64;
        
        byte[] keyBytes = AjustarClave(claveSecreta);
        byte[] iv = new byte[16]; // Debe ser el mismo vector utilizado en la encriptación
        byte[] buffer;
        
        try
        {
            buffer = Convert.FromBase64String(textoEncriptadoBase64);
        }
        catch (FormatException)
        {
            // Si no es un Base64 válido, devolvemos el original
            return textoEncriptadoBase64;
        }

        using Aes aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream msDecrypt = new MemoryStream(buffer);
        using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new StreamReader(csDecrypt);
        
        return srDecrypt.ReadToEnd();
    }

    /// <summary>
    /// Asegura que la clave de encriptación tenga exactamente 32 bytes de longitud para AES-256.
    /// </summary>
    private static byte[] AjustarClave(string clave)
    {
        byte[] keyBytes = new byte[32];
        byte[] secretBytes = Encoding.UTF8.GetBytes(clave ?? string.Empty);
        
        int length = Math.Min(secretBytes.Length, keyBytes.Length);
        Array.Copy(secretBytes, keyBytes, length);
        
        return keyBytes;
    }
}
