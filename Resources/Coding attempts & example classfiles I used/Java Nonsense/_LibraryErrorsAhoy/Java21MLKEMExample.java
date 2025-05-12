package _LibraryErrorsAhoy;

import org.bouncycastle.jcajce.spec.KTSParameterSpec;
import org.bouncycastle.jcajce.spec.MLKEMParameterSpec;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.util.Arrays;
import org.bouncycastle.util.encoders.Hex;
import javax.crypto.KEM;
import javax.crypto.SecretKey;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.PublicKey;
import java.security.SecureRandom;
import java.security.Security;
/**
* Use of a Java 21 KEM based on ML-KEM to generate a 128 bit AES key.
*/
public class Java21MLKEMExample
{
	public static void main(String[] args) throws Exception
	{
		Security.addProvider(new BouncyCastleProvider());
		// Receiver side
		KeyPairGenerator g = KeyPairGenerator.getInstance("ML-KEM", "BC");
		g.initialize(MLKEMParameterSpec.ml_kem_768, new SecureRandom());
		KeyPair kp = g.generateKeyPair();
		PublicKey pkR = kp.getPublic();
		// Sender side
		KEM kemS = KEM.getInstance("ML-KEM");
		// Specify we want a 128 bit AES key using pkR
		KTSParameterSpec ktsSpec = new KTSParameterSpec.Builder("AES", 128).build();
		KEM.Encapsulator e = kemS.newEncapsulator(pkR, ktsSpec, null);
		KEM.Encapsulated enc = e.encapsulate();
		SecretKey secS = enc.key();
		byte[] em = enc.encapsulation();
		// Receiver side
		KEM kemR = KEM.getInstance("ML-KEM");
		KEM.Decapsulator d = kemR.newDecapsulator(kp.getPrivate(), ktsSpec);
		SecretKey secR = d.decapsulate(em);
		// secS and secR will be identical
		if (Arrays.areEqual(secS.getEncoded(), secR.getEncoded()))
		{
			System.out.println("AES key generated successfully: " + Hex.toHexString(secS.getEncoded()));
			System.exit(0);
		}
		System.exit(1);
	}
}