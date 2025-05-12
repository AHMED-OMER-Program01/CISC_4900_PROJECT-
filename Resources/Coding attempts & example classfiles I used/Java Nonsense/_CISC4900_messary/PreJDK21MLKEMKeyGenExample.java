package _CISC4900_messary;

import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.SecureRandom;
import java.security.Security;
import javax.crypto.KeyGenerator;
import org.bouncycastle.jcajce.SecretKeyWithEncapsulation;
import org.bouncycastle.jcajce.spec.KEMExtractSpec;
import org.bouncycastle.jcajce.spec.KEMGenerateSpec;
import org.bouncycastle.jcajce.spec.MLKEMParameterSpec;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.util.Arrays;
import org.bouncycastle.util.encoders.Hex;
/**
* Pre Java 21 ML-KEM shared key generation using the KeyGenerator class.
*/
public class PreJDK21MLKEMKeyGenExample
{
	public static void main(String[] args)
	throws Exception
	{
		Security.addProvider(new BouncyCastleProvider());
		// generate ML-KEM-512 key pair.
		KeyPairGenerator kpg = KeyPairGenerator.getInstance("ML-KEM", "BC");
		kpg.initialize(MLKEMParameterSpec.ml_kem_512, new SecureRandom());
		KeyPair kp = kpg.generateKeyPair();
		// Create the KeyGenerator - there are specific specs for creating encapsulations (KEMGenerateSpec) and for
		// extracting a secret key from an encapsulation using the private key (KEMExtractSpec)
		KeyGenerator keyGen = KeyGenerator.getInstance("ML-KEM", "BC");
		// initialise for creating an encapsulation and shared secret.
		keyGen.init(new KEMGenerateSpec(kp.getPublic(), "AES", 128), new SecureRandom());
		// SecretKeyWithEncapsulation is the class to use as the secret key, it has additional
		// methods on it for recovering the encapsulation as well.
		SecretKeyWithEncapsulation secEnc1 = (SecretKeyWithEncapsulation)keyGen.generateKey();
		keyGen.init(new KEMExtractSpec(kp.getPrivate(), secEnc1.getEncapsulation(), "AES", 128));
		// initialise for extracting the shared secret from the encapsulation.
		SecretKeyWithEncapsulation secEnc2 = (SecretKeyWithEncapsulation)keyGen.generateKey();
		// a quick check to make sure we got the same answer on both sides.
		if (Arrays.areEqual(secEnc1.getEncoded(), secEnc2.getEncoded()))
		{
			System.out.println("AES key generated successfully: " + Hex.toHexString(secEnc1.getEncoded()));
			System.exit(0);
		}
		System.exit(1);
	}
}