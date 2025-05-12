package _LibraryErrorsAhoy;

import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.SecureRandom;
import java.security.Security;
import java.util.Arrays;
import java.util.Collection;
import org.bouncycastle.asn1.cmp.CMPCertificate;
import org.bouncycastle.asn1.nist.NISTObjectIdentifiers;
import org.bouncycastle.asn1.x509.AlgorithmIdentifier;
import org.bouncycastle.cert.cmp.CMSProcessableCMPCertificate;
import org.bouncycastle.cert.jcajce.JcaX509CertificateConverter;
import org.bouncycastle.cert.jcajce.JcaX509ExtensionUtils;
import org.bouncycastle.cms.CMSAlgorithm;
import org.bouncycastle.cms.CMSEnvelopedData;
import org.bouncycastle.cms.CMSEnvelopedDataGenerator;
import org.bouncycastle.cms.CMSProcessableByteArray;
import org.bouncycastle.cms.KEMRecipientId;
import org.bouncycastle.cms.RecipientInformation;
import org.bouncycastle.cms.RecipientInformationStore;
import org.bouncycastle.cms.jcajce.JceCMSContentEncryptorBuilder;
import org.bouncycastle.cms.jcajce.JceKEMEnvelopedRecipient;
import org.bouncycastle.cms.jcajce.JceKEMRecipientInfoGenerator;
import org.bouncycastle.jcajce.spec.MLKEMParameterSpec;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.util.Strings;
/**
 * Simple message encryption using ML-KEM-768
 */
public class MLKEMCMSExample
{
	private static byte[] msg = Strings.toByteArray("Hello, world!");
	public static void main(String[] args)
			throws Exception
	{
		Security.addProvider(new BouncyCastleProvider());
		// Generate the ML-KEM-768 key.
		KeyPairGenerator kpg = KeyPairGenerator.getInstance("MLKEM", "BC");
		kpg.initialize(MLKEMParameterSpec.ml_kem_768, new SecureRandom());
		KeyPair kp = kpg.generateKeyPair();
		// Setup the CMS EnvelopedData generator
		CMSEnvelopedDataGenerator edGen = new CMSEnvelopedDataGenerator();
		// we will skip generating a certificate and just use a subject key ID.
		JcaX509ExtensionUtils x509Utils = new JcaX509ExtensionUtils();
		byte[] subjectKeyId = x509Utils.createSubjectKeyIdentifier(kp.getPublic()).getKeyIdentifier();
		// add the KEM recipientInfo RFC 9629 style - we're using SHAKE256 as the KDF
		edGen.addRecipientInfoGenerator(new JceKEMRecipientInfoGenerator(subjectKeyId, kp.getPublic(), CMSAlgorithm.AES256_WRAP).setKDF(
				new AlgorithmIdentifier(NISTObjectIdentifiers.id_shake256)));
		// Add the payload and specify encryption using AES-256.
		CMSEnvelopedData encryptedData = edGen.generate(
				new CMSProcessableByteArray(msg),
				new JceCMSContentEncryptorBuilder(CMSAlgorithm.AES256_CBC).setProvider("BC").build());
		// These are the steps to recover the data - usually we would use the issuer/serial-number
		RecipientInformation recInfo = encryptedData.getRecipientInfos().get(new KEMRecipientId(subjectKeyId));
		byte[] recData = recInfo.getContent(new JceKEMEnvelopedRecipient(kp.getPrivate()).setProvider("BC"));
		if (Arrays.equals(msg, recData))
		{
			System.out.println("message successfully encrypted and decrypted");
			System.exit(0);
		}
		System.exit(1);
	}
}