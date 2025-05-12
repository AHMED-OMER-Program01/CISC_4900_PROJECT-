package _LibraryErrorsAhoy;

import java.math.BigInteger;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.Security;
import java.util.Date;
import org.bouncycastle.asn1.cmp.CMPCertificate;
import org.bouncycastle.asn1.cmp.PKIBody;
import org.bouncycastle.asn1.cmp.PKIStatus;
import org.bouncycastle.asn1.cmp.PKIStatusInfo;
import org.bouncycastle.asn1.crmf.CertTemplate;
import org.bouncycastle.asn1.crmf.SubsequentMessage;
import org.bouncycastle.asn1.nist.NISTObjectIdentifiers;
import org.bouncycastle.asn1.x500.X500Name;
import org.bouncycastle.asn1.x509.AlgorithmIdentifier;
import org.bouncycastle.asn1.x509.BasicConstraints;
import org.bouncycastle.asn1.x509.Extension;
import org.bouncycastle.asn1.x509.GeneralName;
import org.bouncycastle.asn1.x509.SubjectPublicKeyInfo;
import org.bouncycastle.cert.CertException;
import org.bouncycastle.cert.CertIOException;
import org.bouncycastle.cert.X509CertificateHolder;
import org.bouncycastle.cert.X509v3CertificateBuilder;
import org.bouncycastle.cert.cmp.CMSProcessableCMPCertificate;
import org.bouncycastle.cert.cmp.CertificateConfirmationContent;
import org.bouncycastle.cert.cmp.CertificateConfirmationContentBuilder;
import org.bouncycastle.cert.cmp.ProtectedPKIMessage;
import org.bouncycastle.cert.cmp.ProtectedPKIMessageBuilder;
import org.bouncycastle.cert.crmf.CertificateRepMessage;
import org.bouncycastle.cert.crmf.CertificateRepMessageBuilder;
import org.bouncycastle.cert.crmf.CertificateReqMessages;
import org.bouncycastle.cert.crmf.CertificateReqMessagesBuilder;
import org.bouncycastle.cert.crmf.CertificateRequestMessage;
import org.bouncycastle.cert.crmf.CertificateResponse;
import org.bouncycastle.cert.crmf.CertificateResponseBuilder;
import org.bouncycastle.cert.crmf.jcajce.JcaCertificateRequestMessageBuilder;
import org.bouncycastle.cert.jcajce.JcaX509CertificateConverter;
import org.bouncycastle.cert.jcajce.JcaX509v3CertificateBuilder;
import org.bouncycastle.cms.CMSAlgorithm;
import org.bouncycastle.cms.CMSEnvelopedData;
import org.bouncycastle.cms.CMSEnvelopedDataGenerator;
import org.bouncycastle.cms.jcajce.JceCMSContentEncryptorBuilder;
import org.bouncycastle.cms.jcajce.JceKEMEnvelopedRecipient;
import org.bouncycastle.cms.jcajce.JceKEMRecipientInfoGenerator;
import org.bouncycastle.jcajce.spec.MLDSAParameterSpec;
import org.bouncycastle.jcajce.spec.MLKEMParameterSpec;
import org.bouncycastle.jce.provider.BouncyCastleProvider;
import org.bouncycastle.operator.ContentSigner;
import org.bouncycastle.operator.MacCalculator;
import org.bouncycastle.operator.OperatorCreationException;
import org.bouncycastle.operator.PBEMacCalculatorProvider;
import org.bouncycastle.operator.jcajce.JcaContentSignerBuilder;
import org.bouncycastle.operator.jcajce.JcaContentVerifierProviderBuilder;
import org.bouncycastle.operator.jcajce.JcaDigestCalculatorProviderBuilder;
import org.bouncycastle.pkcs.jcajce.JcePBMac1CalculatorBuilder;
import org.bouncycastle.pkcs.jcajce.JcePBMac1CalculatorProviderBuilder;
/**
 * Simple message encryption using ML-KEM-768
 */
public class MLKEMCRMFExample
{
	private static X509CertificateHolder makeV3Certificate(String _subDN, KeyPair issKP)
			throws OperatorCreationException, CertException, CertIOException
	{
		PrivateKey issPriv = issKP.getPrivate();
		PublicKey issPub = issKP.getPublic();
		X509v3CertificateBuilder certGen = new JcaX509v3CertificateBuilder(
				new X500Name(_subDN),
				BigInteger.valueOf(System.currentTimeMillis()),
				new Date(System.currentTimeMillis()),
				new Date(System.currentTimeMillis() + (1000L * 60 * 60 * 24 * 100)),
				new X500Name(_subDN),
				issKP.getPublic());
		certGen.addExtension(Extension.basicConstraints, true, new BasicConstraints(0));
		ContentSigner signer = new JcaContentSignerBuilder("ML-DSA").build(issPriv);
		X509CertificateHolder certHolder = certGen.build(signer);
		return certHolder;
	}
	private static X509CertificateHolder makeEndEntityCertificate(SubjectPublicKeyInfo pubKey, X500Name _subDN,
			KeyPair issKP, String _issDN)
					throws OperatorCreationException, CertException, CertIOException
	{
		PrivateKey issPriv = issKP.getPrivate();
		PublicKey issPub = issKP.getPublic();
		X509v3CertificateBuilder certGen = new JcaX509v3CertificateBuilder(
				new X500Name(_issDN),
				BigInteger.valueOf(System.currentTimeMillis()),
				new Date(System.currentTimeMillis()),
				new Date(System.currentTimeMillis() + (1000L * 60 * 60 * 24 * 100)),
				_subDN,
				pubKey);
		certGen.addExtension(Extension.basicConstraints, true, new BasicConstraints(false));
		ContentSigner signer = new JcaContentSignerBuilder("ML-DSA").build(issPriv);
		X509CertificateHolder certHolder = certGen.build(signer);
		return certHolder;
	}
	public static void main(String[] args)
			throws Exception
	{
		Security.addProvider(new BouncyCastleProvider());
		// set up - in this section we are just establishing the private key and the certificate for our ML-DSA CA
		GeneralName sender = new GeneralName(new X500Name("CN=ML-KEM Subject"));
		GeneralName recipient = new GeneralName(new X500Name("CN=ML-DSA Issuer"));
		KeyPairGenerator dilKpGen = KeyPairGenerator.getInstance("ML-DSA", "BC");
		dilKpGen.initialize(MLDSAParameterSpec.ml_dsa_87);
		KeyPair dilKp = dilKpGen.generateKeyPair();
		X509CertificateHolder caCert = makeV3Certificate("CN=ML-DSA Issuer", dilKp);
		// First step for the client - we generate a key pair
		KeyPairGenerator mlKemKpGen = KeyPairGenerator.getInstance("ML-KEM", "BC");
		mlKemKpGen.initialize(MLKEMParameterSpec.ml_kem_768);
		KeyPair mlKemKp = mlKemKpGen.generateKeyPair();
		// Second Step: We generate our certification request (CRMF).
		BigInteger certReqId = BigInteger.valueOf(System.currentTimeMillis());
		JcaCertificateRequestMessageBuilder certReqBuild = new JcaCertificateRequestMessageBuilder(certReqId);
		certReqBuild
		.setPublicKey(mlKemKp.getPublic())
		.setSubject(X500Name.getInstance(sender.getName()))
		.setProofOfPossessionSubsequentMessage(SubsequentMessage.encrCert);
		CertificateReqMessagesBuilder certReqMsgsBldr = new CertificateReqMessagesBuilder();
		certReqMsgsBldr.addRequest(certReqBuild.build());
		// Third Step: We wrap the CRMF certification request in a CMP message for sending, protecting it using a MAC.
		char[] senderMacPassword = "secret".toCharArray();
		MacCalculator senderMacCalculator = new JcePBMac1CalculatorBuilder("HmacSHA256",
				256).setProvider("BC").build(senderMacPassword);
		ProtectedPKIMessage message = new ProtectedPKIMessageBuilder(sender, recipient)
				.setBody(PKIBody.TYPE_INIT_REQ, certReqMsgsBldr.build())
				.build(senderMacCalculator);
		System.out.println("PKIBody.TYPE_INIT_REQ sent");
		// extract
		PBEMacCalculatorProvider macCalcProvider = new JcePBMac1CalculatorProviderBuilder().setProvider("BC").build();
		if (message.verify(macCalcProvider, senderMacPassword))
		{
			System.out.println("PKIBody.TYPE_INIT_REQ verified");
		}
		else
		{
			System.exit(1);
		}
		CertificateReqMessages requestMessages = CertificateReqMessages.fromPKIBody(message.getBody());
		CertificateRequestMessage senderReqMessage = requestMessages.getRequests()[0];
		CertTemplate certTemplate = senderReqMessage.getCertTemplate();
		X509CertificateHolder cert = makeEndEntityCertificate(certTemplate.getPublicKey(), certTemplate.getSubject(), dilKp, "CN=ML-DSA Issuer");
		// Send response with encrypted certificate
		CMSEnvelopedDataGenerator edGen = new CMSEnvelopedDataGenerator();
		// note: use cert req ID as key ID, don't want to use issuer/serial in this case!
		edGen.addRecipientInfoGenerator(new JceKEMRecipientInfoGenerator(senderReqMessage.getCertReqId().getEncoded(),
				new JcaX509CertificateConverter().setProvider("BC").getCertificate(cert).getPublicKey(),
				CMSAlgorithm.AES256_WRAP).setKDF(
						new AlgorithmIdentifier(NISTObjectIdentifiers.id_shake256)));
		CMSEnvelopedData encryptedCert = edGen.generate(
				new CMSProcessableCMPCertificate(cert),
				new JceCMSContentEncryptorBuilder(CMSAlgorithm.AES128_CBC).setProvider("BC").build());
		CertificateResponseBuilder certRespBuilder = new CertificateResponseBuilder(senderReqMessage.getCertReqId(), new
				PKIStatusInfo(PKIStatus.granted));
		certRespBuilder.withCertificate(encryptedCert);
		CertificateRepMessageBuilder repMessageBuilder = new CertificateRepMessageBuilder(caCert);
		repMessageBuilder.addCertificateResponse(certRespBuilder.build());
		ContentSigner signer = new JcaContentSignerBuilder("ML-DSA").setProvider("BC").build(dilKp.getPrivate());
		CertificateRepMessage repMessage = repMessageBuilder.build();
		ProtectedPKIMessage responsePkixMessage = new ProtectedPKIMessageBuilder(sender, recipient)
				.setBody(PKIBody.TYPE_INIT_REP, repMessage)
				.build(signer);
		// decrypt the certificate
		System.out.println("PKIBody.TYPE_INIT_REP sent");
		if (responsePkixMessage.verify(new JcaContentVerifierProviderBuilder().build(caCert)))
		{
			System.out.println("PKIBody.TYPE_INIT_REP verified");
		}
		else
		{
			System.exit(1);
		}
		CertificateRepMessage certRepMessage = CertificateRepMessage.fromPKIBody(responsePkixMessage.getBody());
		CertificateResponse certResp = certRepMessage.getResponses()[0];
		if (certResp.hasEncryptedCertificate())
		{
			System.out.println("PKIBody.TYPE_INIT_REP contains encrypted certificate");
		}
		else
		{
			System.exit(1);
		}
		// this is the preferred way of recovering an encrypted certificate
		CMPCertificate receivedCMPCert = certResp.getCertificate(new JceKEMEnvelopedRecipient(mlKemKp.getPrivate()));
		X509CertificateHolder receivedCert = new X509CertificateHolder(receivedCMPCert.getX509v3PKCert());
		X509CertificateHolder caCertHolder = certRepMessage.getX509Certificates()[0];
		if (receivedCert.isSignatureValid(new JcaContentVerifierProviderBuilder().build(caCertHolder)))
		{
			System.out.println("Received certificate decrypted and verified against CA certificate");
		}
		else
		{
			System.exit(1);
		}
		// confirmation message calculation
		CertificateConfirmationContent content = new CertificateConfirmationContentBuilder()
				.addAcceptedCertificate(cert, BigInteger.ONE)
				.build(new JcaDigestCalculatorProviderBuilder().build());
		message = new ProtectedPKIMessageBuilder(sender, recipient)
				.setBody(PKIBody.TYPE_CERT_CONFIRM, content)
				.build(senderMacCalculator);
		System.out.println("PKIBody.TYPE_CERT_CONFIRM sent");
		// confirmation receiving
		CertificateConfirmationContent recContent = CertificateConfirmationContent.fromPKIBody(message.getBody());
		if (recContent.getStatusMessages()[0].isVerified(receivedCert, new JcaDigestCalculatorProviderBuilder().build()))
		{
			System.out.println("PKIBody.TYPE_CERT_CONFIRM verified");
		}
		else
		{
			System.exit(1);
		}
		System.exit(0);
	}
}
