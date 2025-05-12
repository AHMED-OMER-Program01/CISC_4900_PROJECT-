package _CISC4900_messary;

import java.security.Key;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.SecureRandom;
import java.security.Security;
import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;
import org.bouncycastle.jcajce.provider.asymmetric.MLKEM;
import org.bouncycastle.jcajce.spec.KEMParameterSpec;
import org.bouncycastle.jcajce.spec.KTSParameterSpec;
import org.bouncycastle.jcajce.spec.MLKEMParameterSpec;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.pqc.jcajce.spec.KyberParameterSpec;
import org.bouncycastle.util.Arrays;
import org.bouncycastle.util.encoders.Hex;
/**
* Pre Java 21 ML-KEM shared key based key wrap using the Cipher class.
*/
public class PreJDK21MLKEMWrapExample
{
	public static void main(String[] args)
	throws Exception
	{
		Security.addProvider(new BouncyCastleProvider());
		// define our key to be wrapped
		byte[] aesKey = Hex.decode("0102030405060708090a0b0c0d0e0f");
		SecretKey key = new SecretKeySpec(aesKey, "AES");
		// generate an ML-KEM-768 key pair
		KeyPairGenerator kpg = KeyPairGenerator.getInstance("MLKEM", "BC");
		kpg.initialize(MLKEMParameterSpec.ml_kem_768, new SecureRandom());
		KeyPair kp = kpg.generateKeyPair();
		// specify we want to use 256 bit AES-KWP for wrapping our key
		KTSParameterSpec ktsParameterSpec = new KTSParameterSpec.Builder("AES-KWP", 256).build();
		// set up the wrapping cipher
		Cipher w1 = Cipher.getInstance("MLKEM", "BC");
		w1.init(Cipher.WRAP_MODE, kp.getPublic(), ktsParameterSpec);
		// wrap the key
		byte[] data = w1.wrap(key);
		// set up the unwrapping cipher
		Cipher w2 = Cipher.getInstance("MLKEM", "BC");
		w2.init(Cipher.UNWRAP_MODE, kp.getPrivate(), ktsParameterSpec);
		// unwrap the encrypted key
		Key k = w2.unwrap(data, "AES", Cipher.SECRET_KEY);
		// check that the key was recovered successfully
		if (Arrays.areEqual(aesKey, k.getEncoded()))
		{
			System.out.println("AES key successfully wrapped and unwrapped");
			System.exit(0);
		}
		System.exit(1);
	}
}