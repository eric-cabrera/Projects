using System.Data;
using System.Data.SqlClient;
using MongoDB.Driver;
using PolicyMigration;


// details of the Mongodb Database/Collection I am writing to; The first is to my localhost I test with, the second is the proper database

string mongodbConn = "mongodb+srv://mongo:Trains11%21ns@cluster0.s2crc.mongodb.net/test?authSource=admin&replicaSet=atlas-nxiu03-shard-0&readPreference=primary&ssl=true";
string dbName = "insurance_policies";
string collectionName = "status";



/*
string mongodbConn = "mongodb://dbzuser:0hG011y0hG33zium@mongodbd1.assurity.local:27017/?replicaSet=mongodev&ssl=true";
string dbName = "Events";
string collectionName = "ApplicationTrackerV2";
*/

var client = new MongoClient(mongodbConn);
var db = client.GetDatabase(dbName);
var collection = db.GetCollection<Policy>(collectionName);


using (var conn = new SqlConnection("Data Source=DMDEV; Initial Catalog=DataStore;Integrated Security=True; MultipleActiveResultSets = true"))
{
	conn.Open();
	using (SqlCommand cmd = new SqlCommand(@"
        SELECT
			polc.COMPANY_CODE AS CompanyCode,
			polc.POLICY_NUMBER AS PolicyNumber,
			PRODUCT_CODE AS ProductCode,
			pd.ProdCategory AS ProductCategory, --Also returned on benefit. Duplicated here for easy accessibility.
			pd.AltProdDesc AS ProductDescription, --Also returned on benefit. Duplicated here for easy accessibility.
			CONTRACT_CODE AS PolicyStatus,
			CONTRACT_REASON AS PolicyStatusReason,
			ISSUE_STATE AS IssueState,
			RES_STATE AS ResidentState,
			BILLING_MODE AS BillingMode, --Needs mapped to English
			BILLING_FORM AS BillingForm, --Probably needs mapped to English
			polc.ISSUE_DATE AS IssueDate,
			PAID_TO_DATE AS PaidToDate,
			MODE_PREMIUM AS ModePremium,
			ANNUAL_PREMIUM AS AnnualPremium,
			POLICY_BILL_DAY AS BillingDay,
			FACE_AMOUNT AS FaceAmount
		FROM lifepro_r.PPOLC polc
		INNER JOIN alic.ProductDescription pd
			ON polc.PRODUCT_CODE = pd.ProdNumber AND polc.PRODUCT_CODE = pd.CovID
		INNER JOIN lifepro_r.PPEND_NEW_BUSINESS_PENDING pnbp
			on polc.POLICY_NUMBER = pnbp.POLICY_NUMBER
		WHERE polc.POLICY_NUMBER = '5051114103'
", conn))
	{
		using (SqlDataReader reader = cmd.ExecuteReader())
		{
			while (reader.Read())
			{
				Policy policy = new Policy();
				policy.PolicyNumber = (string)reader["PolicyNumber"];
				policy.CompanyCode = (string)reader["CompanyCode"];
				policy.ProductCode = (string)reader["ProductCode"];
				if (!reader.IsDBNull("ProductCategory"))
				{
					policy.ProductCategory = (string)reader["ProductCategory"];
				}
				if (!reader.IsDBNull("ProductCategory"))
				{
					policy.ProductDescription = (string)reader["ProductDescription"];
				}
				policy.PolicyStatus = parseStatus((string)reader["PolicyStatus"], (string)reader["PolicyStatusReason"]);
				policy.PolicyStatusReason = parseReason((string)reader["PolicyStatus"], (string)reader["PolicyStatusReason"]);
				policy.IssueState = parseState((string)reader["IssueState"]);
				policy.ResidentState = parseState((string)reader["ResidentState"]);
				policy.BillingMode = ((Int16)reader["BillingMode"]).ToString();
				policy.BillingForm = (string)reader["BillingForm"];

				policy.IssueDate = parseDate(((Int32)reader["IssueDate"]).ToString());
				policy.PaidToDate = parseDate(((Int32)reader["PaidToDate"]).ToString());

				policy.ModePremium = (decimal)reader["ModePremium"];
				policy.AnnualPremium = (decimal)reader["AnnualPremium"];
				policy.BillingDay = (short)reader["BillingDay"];
				policy.FaceAmount = (decimal)reader["FaceAmount"];

				policy.Owners = new List<Owner>();
				addOwners(policy, conn);
				policy.Payors = new List<Payor>();
				addPayors(policy, conn);
				policy.Insureds = new List<Insured>();
				addInsured(policy, conn);
				policy.Agents = new List<Agent>();
				addAgent(policy, conn);
				policy.Benefits = new List<Benefit>();
				addBenefits(policy, conn);

				await collection.InsertOneAsync(policy);
				
			}
		}
	}
}

static void addBenefits(Policy policy, SqlConnection conn)
{
	// getting basic Benefit info
	using (SqlCommand cmd = new SqlCommand(@$"
    SELECT 
		pben.POLICY_NUMBER AS PolicyNumber,
		CASE BENEFIT_SEQ WHEN 1 THEN 'Base' ELSE 'Rider' END AS CoverageType,
		pben.PBEN_ID AS BenefitId,
		PLAN_CODE AS PlanCode,
		pd.ProdCategory AS BenefitCategory, --Also returned on benefit. Duplicated here for easy accessibility.
		pd.AltProdDesc AS BenefitDescription, --Also returned on benefit. Duplicated here for easy accessibility.
		pben.STATUS_CODE AS BenefitStatus, 
		pben.STATUS_REASON AS BenefitStatusReason,
		ISNULL(pbenbaor.NUMBER_OF_UNITS * pbenbaor.ANN_PREM_PER_UNIT, pbenbf.NUMBER_OF_UNITS * pbenbf.ANN_PREM_PER_UNIT) AS BenfitAmount
	FROM lifepro_r.PPOLC polc
	INNER JOIN lifepro_r.PPBEN_POLICY_BENEFITS pben
		ON polc.POLICY_NUMBER = pben.POLICY_NUMBER
	LEFT JOIN lifepro_r.PPBEN_POLICY_BENEFITS_TYPES_BA_OR pbenbaor
		ON pben.PBEN_ID = pbenbaor.PBEN_ID
	LEFT JOIN lifepro_r.PPBEN_POLICY_BENEFITS_TYPES_BF pbenbf
		ON pben.PBEN_ID = pbenbf.PBEN_ID
	INNER JOIN alic.ProductDescription pd
		ON polc.PRODUCT_CODE = pd.ProdNumber AND pben.PLAN_CODE = pd.CovID
	WHERE pben.BENEFIT_TYPE IN ('BA','OR','BF') --Is this OK? What is BF? Why does one of my policies have an SU?
	AND pben.POLICY_NUMBER in ('{policy.PolicyNumber}')
	ORDER BY pben.PBEN_ID 
", conn))
	{
		using (SqlDataReader reader = cmd.ExecuteReader())
		{
			while (reader.Read())
			{
				Benefit benefit = new Benefit();
				benefit.CoverageType = parseCoverageType((string)reader["CoverageType"]);
				benefit.BenefitId = (Int64)reader["BenefitId"];
				benefit.PlanCode = (string)reader["PlanCode"];
				if (!reader.IsDBNull("BenefitCategory"))
				{
					benefit.BenefitCategory = (string)reader["BenefitCategory"];
				}
				if (!reader.IsDBNull("BenefitDescription"))
				{
					benefit.BenefitDescription = (string)reader["BenefitDescription"];
				}
				benefit.BenefitStatus = parseStatus((string)reader["BenefitStatus"], (string)reader["BenefitStatusReason"]);
				benefit.BenefitStatusReason = parseReason((string)reader["BenefitStatus"], (string)reader["BenefitStatusReason"]);
				benefit.BenefitAmount = (decimal)reader["BenfitAmount"];

				findAdditionalInfo(benefit, conn);

				policy.Benefits.Add(benefit);

			}
		}
	}

}

static void findAdditionalInfo(Benefit benefit, SqlConnection conn)
{
	using (SqlCommand cmd = new SqlCommand(@$"
		SELECT
		polc.POLICY_NUMBER AS PolicyNumber,
		pben.PBEN_ID AS BenefitId,
		muin.NAME_ID,
		muin.MULT_RELATE AS InsuredRelationhipToPrimary,
		muin.KD_DEF_SEGT_ID, 
		muin.KD_BEN_EXTEND_KEYS
	FROM lifepro_r.PPOLC polc
	INNER JOIN lifepro_r.PRELA_RELATIONSHIP_MASTER rela
		ON polc.COMPANY_CODE + polc.POLICY_NUMBER = rela.IDENTIFYING_ALPHA
	LEFT JOIN lifepro_r.PPBEN_POLICY_BENEFITS pben
		ON polc.POLICY_NUMBER = pben.POLICY_NUMBER AND rela.BENEFIT_SEQ_NUMBER = pben.BENEFIT_SEQ
	INNER JOIN lifepro_r.PMUIN_MULTIPLE_INSUREDS muin
		ON pben.POLICY_NUMBER = muin.POLICY_NUMBER AND pben.BENEFIT_SEQ = muin.BENEFIT_SEQ AND rela.NAME_ID = muin.NAME_ID AND rela.RELATE_CODE = muin.RELATIONSHIP_CODE
	WHERE pben.BENEFIT_SEQ > 0 --Not sure what 0 represents, but it appears to drop when joined to PPBEN anyway.
	AND pben.BENEFIT_TYPE IN ('BA','OR','BF') --Why does one of my policies have an SU?
	AND pben.PBEN_ID in ('{benefit.BenefitId}')
	ORDER BY pben.PBEN_ID 
", conn))
	{
		using (SqlDataReader reader = cmd.ExecuteReader())
		{
			while (reader.Read())
			{
				var insuredRelationshipToPrimary = ((string)reader["InsuredRelationhipToPrimary"]).Trim();
				var KD_DEF_SEGT_ID = (string)reader["KD_DEF_SEGT_ID"];
				var KD_BEN_EXTEND_KEYS = ((string)reader["KD_BEN_EXTEND_KEYS"]).Trim();
				switch (insuredRelationshipToPrimary)
                {
					case "SPOUSE":
						benefit.SpouseBenefitOptions = findKeyOptions(KD_DEF_SEGT_ID, KD_BEN_EXTEND_KEYS, conn);
						break;
					case "CHILD":
						benefit.ChildBenefitOptions = findKeyOptions(KD_DEF_SEGT_ID, KD_BEN_EXTEND_KEYS, conn);
						break;
					default:
						benefit.SelfBenefitOptions = findKeyOptions(KD_DEF_SEGT_ID, KD_BEN_EXTEND_KEYS, conn);
						break;
				}
			}
		}
	}
}


static BenefitOptions findKeyOptions(string keyIdentifier, string KD_BEN_EXTEND_KEYS, SqlConnection conn)
{
	BenefitOptions benefitOption = new BenefitOptions();
	benefitOption.ExtendedKeys = new List<ExtendedKey>();
	int iteration = 1;
	while (KD_BEN_EXTEND_KEYS.Length >= 2)
    {
		var key = KD_BEN_EXTEND_KEYS.Substring(0, 2);
		KD_BEN_EXTEND_KEYS = KD_BEN_EXTEND_KEYS.Substring(2, KD_BEN_EXTEND_KEYS.Length - 2);
		using (SqlCommand cmd = new SqlCommand(@$"
		SELECT 
			key0.IDENT AS KEY_IDENTIFIER, 
			key0.KDEF_DESC AS KEY_CATEGORY, 
			key0.DESC_NUM AS KEY_CATEGORY_VALUE, 
			key1.KDEF_DESC AS KEY_OPTION, 
			key1.DESC_NUM AS KEY_OPTION_VALUE
		FROM lifepro_r.PKDEF_KEY_DEFINITION key0
		INNER JOIN lifepro_r.PKDEF_KEY_DEFINITION key1
			ON key0.IDENT = key1.IDENT AND key0.KEY_NUM = 0 AND key0.DESC_NUM = key1.KEY_NUM AND key0.DESC_NUM > 0
		WHERE key0.IDENT = '{keyIdentifier}'
		AND key0.DESC_NUM = {iteration}
		AND key1.DESC_NUM = {key} 
", conn))
		{
			using (SqlDataReader reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					var extendedKey = new ExtendedKey();
					extendedKey.KeyOption = ((string)reader["KEY_CATEGORY"]).Trim();
					extendedKey.KeyValue = ((string)reader["KEY_OPTION"]).Trim();
					benefitOption.ExtendedKeys.Add(extendedKey);
				}
			}
		}
		iteration++;
	}
	return benefitOption;
}


static void addOwners(Policy policy, SqlConnection conn) {
	using (SqlCommand cmd = new SqlCommand(@$"
    SELECT 
		polc.POLICY_NUMBER AS PolicyNumber,
		CASE rela.RELATE_CODE WHEN 'PO' THEN 'Primary' ELSE 'Contingent' END AS OwnerType,
		CASE name.NAME_FORMAT_CODE WHEN 'B' THEN 'True' ELSE 'False' END AS ParticipantIsBusiness,
		name.NAME_BUSINESS AS ParticipantBusinessName, --Only relevant when IsBusiness
		name.BUSINESS_EMAIL_ADR AS ParticipantBusinessEmailAddress, --Only relevant when IsBusiness (Assumed: there are records in LifePro with both email address types.)
		name.SEX_CODE AS ParticipantPersonGender, --Only relevant when NOT IsBusiness
		name.DATE_OF_BIRTH AS ParticipantPersonDateOfBirth, --Only relevant when NOT IsBusiness
		name.NAME_ID NameID,
		name.INDIVIDUAL_PREFIX AS ParticipantPersonIndividualPrefix, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_FIRST AS ParticipantPersonIndividualFirst, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_MIDDLE AS ParticipantPersonIndividualMiddle, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_LAST AS ParticipantPersonIndividualLast, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_SUFFIX AS ParticipantPersonIndividualSuffix, --Only relevant when NOT IsBusiness
		name.PERSONAL_EMAIL_ADR AS ParticipantPersonEmailAddress, --Only relevant when NOT IsBusiness (Assumed: there are records in LifePro with both email address types.)
		nalk.TELE_NUM AS ParticipantPhoneNumber,
		addr.ADDR_LINE_1 AS ParticipantAddressLine1, 
		addr.ADDR_LINE_2 AS ParticipantAddressLine2,
		addr.ADDR_LINE_3 AS ParticipantAddressLine3, 
		addr.CITY AS ParticipantAddressCity,
		addr.STATE AS ParticipantAddressState,
		addr.ZIP AS ParticipantAddressZip, 
		addr.ZIP_EXTENSION AS ParticipantAddressZipExtension, 
		addr.BOX_NUMBER AS ParticipantAddressBoxNumber,
		addr.COUNTRY AS ParticipantAddressCountry
	FROM lifepro_r.PPOLC polc
	INNER JOIN lifepro_r.PRELA_RELATIONSHIP_MASTER rela
		ON polc.COMPANY_CODE + polc.POLICY_NUMBER = rela.IDENTIFYING_ALPHA
	INNER JOIN lifepro_r.PNAME name
		ON rela.NAME_ID = name.NAME_ID
	INNER JOIN lifepro_r.PNALK nalk
		ON name.NAME_ID = nalk.NAME_ID
	INNER JOIN lifepro_r.PADDR addr
		ON nalk.ADDRESS_ID = addr.ADDRESS_ID AND nalk.ADDRESS_CODE = '' AND nalk.CANCEL_DATE = 0
	WHERE rela.RELATE_CODE IN ('PO','O1')
	AND polc.POLICY_NUMBER IN ('{policy.PolicyNumber}')
", conn))
	{
		using (SqlDataReader reader = cmd.ExecuteReader())
		{
			while (reader.Read())
			{
				Owner owner = new Owner();
				owner.OwnerType = parseOwner((string)reader["OwnerType"]);
				owner.Participant = new Participant();
				owner.Participant.IsBusiness = parseBoolean((string)reader["ParticipantIsBusiness"]);
				if (owner.Participant.IsBusiness)
                {
					owner.Participant.Business = new Business();
					owner.Participant.Business.Name = new Name();
					owner.Participant.Business.Name.BusinessName = (string)reader["ParticipantBusinessName"];
					owner.Participant.Business.EmailAddress = (string)reader["ParticipantBusinessEmailAddress"];
				}
				owner.Participant.Person = new Person();
				owner.Participant.Person.Gender = parseGender((string)reader["ParticipantPersonGender"]);
				owner.Participant.Person.DateOfBirth = parseDate(((Int32)reader["ParticipantPersonDateOfBirth"]).ToString());
				owner.Participant.Person.Name = new Name();
				owner.Participant.Person.Name.NameId = (Int32)reader["NameID"];
				owner.Participant.Person.Name.IndividualPrefix = ((string)reader["ParticipantPersonIndividualPrefix"]).Trim();
				owner.Participant.Person.Name.IndividualFirst = CapitalizeFirstLetter(((string)reader["ParticipantPersonIndividualFirst"]).Trim());
				owner.Participant.Person.Name.IndividualMiddle = ((string)reader["ParticipantPersonIndividualMiddle"]).Trim();
				owner.Participant.Person.Name.IndividualLast = CapitalizeFirstLetter(((string)reader["ParticipantPersonIndividualLast"]).Trim());
				owner.Participant.Person.Name.IndividualSuffix = ((string)reader["ParticipantPersonIndividualSuffix"]).Trim();
				owner.Participant.Person.EmailAddress = ((string)reader["ParticipantPersonEmailAddress"]).Trim();
				owner.Participant.Address = new Address();
				owner.Participant.Address.Line1 = ((string)reader["ParticipantAddressLine1"]).Trim();
				owner.Participant.Address.Line2 = ((string)reader["ParticipantAddressLine2"]).Trim();
				owner.Participant.Address.Line3 = ((string)reader["ParticipantAddressLine3"]).Trim();
				owner.Participant.Address.City = CapitalizeFirstLetter(((string)reader["ParticipantAddressCity"]).Trim());
				owner.Participant.Address.StateAbbreviation = parseState((string)reader["ParticipantAddressState"]);
				owner.Participant.Address.ZipCode = (string)reader["ParticipantAddressZip"];
				owner.Participant.Address.ZipExtension = (string)reader["ParticipantAddressZipExtension"];
				owner.Participant.Address.BoxNumber = (string)reader["ParticipantAddressBoxNumber"];
				owner.Participant.Address.Country = parseCountry((string)reader["ParticipantAddressCountry"]);

				policy.Owners.Add(owner);
			}
		}
	}
}

static void addPayors(Policy policy, SqlConnection conn)
{
	using (SqlCommand cmd = new SqlCommand(@$"
    SELECT 
		polc.POLICY_NUMBER AS PolicyNumber,
		CASE rela.RELATE_CODE WHEN 'PA' THEN 'Primary' ELSE 'Additional' END AS PayorType, --Unable to find information on how secondary addressee is stored in code or copy books. Will ignore for now.
		CASE name.NAME_FORMAT_CODE WHEN 'B' THEN 'True' ELSE 'False' END AS ParticipantIsBusiness,
		name.NAME_BUSINESS AS ParticipantBusinessName, --Only relevant when IsBusiness
		name.BUSINESS_EMAIL_ADR AS ParticipantBusinessEmailAddress, --Only relevant when IsBusiness (Assumed: there are records in LifePro with both email address types.)
		name.SEX_CODE AS ParticipantPersonGender, --Only relevant when NOT IsBusiness
		name.DATE_OF_BIRTH AS ParticipantPersonDateOfBirth, --Only relevant when NOT IsBusiness
		name.NAME_ID NameID,
		name.INDIVIDUAL_PREFIX AS ParticipantPersonIndividualPrefix, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_FIRST AS ParticipantPersonIndividualFirst, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_MIDDLE AS ParticipantPersonIndividualMiddle, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_LAST AS ParticipantPersonIndividualLast, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_SUFFIX AS ParticipantPersonIndividualSuffix, --Only relevant when NOT IsBusiness
		name.PERSONAL_EMAIL_ADR AS ParticipantPersonEmailAddress, --Only relevant when NOT IsBusiness (Assumed: there are records in LifePro with both email address types.)
		nalk.TELE_NUM AS ParticipantPhoneNumber,
		addr.ADDR_LINE_1 AS ParticipantAddressLine1, 
		addr.ADDR_LINE_2 AS ParticipantAddressLine2,
		addr.ADDR_LINE_3 AS ParticipantAddressLine3, 
		addr.CITY AS ParticipantAddressCity,
		addr.STATE AS ParticipantAddressState,
		addr.ZIP AS ParticipantAddressZip, 
		addr.ZIP_EXTENSION AS ParticipantAddressZipExtension, 
		addr.BOX_NUMBER AS ParticipantAddressBoxNumber,
		addr.COUNTRY AS ParticipantAddressCountry
	FROM lifepro_r.PPOLC polc
	INNER JOIN lifepro_r.PRELA_RELATIONSHIP_MASTER rela
		ON polc.COMPANY_CODE + polc.POLICY_NUMBER = rela.IDENTIFYING_ALPHA
	INNER JOIN lifepro_r.PNAME name
		ON rela.NAME_ID = name.NAME_ID
	INNER JOIN lifepro_r.PNALK nalk
		ON name.NAME_ID = nalk.NAME_ID
	INNER JOIN lifepro_r.PADDR addr
		ON nalk.ADDRESS_ID = addr.ADDRESS_ID AND nalk.ADDRESS_CODE = '' AND nalk.CANCEL_DATE = 0
	WHERE rela.RELATE_CODE IN ('PA','P1')
	AND polc.POLICY_NUMBER IN ('{policy.PolicyNumber}')
", conn))
	{
		using (SqlDataReader reader = cmd.ExecuteReader())
		{
			while (reader.Read())
			{
				Payor payor = new Payor();
				payor.PayorType = parsePayor((string)reader["PayorType"]);
				payor.Participant = new Participant();
				payor.Participant.IsBusiness = parseBoolean((string)reader["ParticipantIsBusiness"]);
				if (payor.Participant.IsBusiness)
				{
					payor.Participant.Business = new Business();
					payor.Participant.Business.Name = new Name();
					payor.Participant.Business.Name.BusinessName = (string)reader["ParticipantBusinessName"];
					payor.Participant.Business.EmailAddress = (string)reader["ParticipantBusinessEmailAddress"];
				}
				payor.Participant.Person = new Person();
				payor.Participant.Person.Gender = parseGender((string)reader["ParticipantPersonGender"]);
				payor.Participant.Person.DateOfBirth = parseDate(((Int32)reader["ParticipantPersonDateOfBirth"]).ToString());
				payor.Participant.Person.Name = new Name();
				payor.Participant.Person.Name.NameId = (Int32)reader["NameID"];
				payor.Participant.Person.Name.IndividualPrefix = ((string)reader["ParticipantPersonIndividualPrefix"]).Trim();
				payor.Participant.Person.Name.IndividualFirst = CapitalizeFirstLetter(((string)reader["ParticipantPersonIndividualFirst"]).Trim());
				payor.Participant.Person.Name.IndividualMiddle = ((string)reader["ParticipantPersonIndividualMiddle"]).Trim();
				payor.Participant.Person.Name.IndividualLast = CapitalizeFirstLetter(((string)reader["ParticipantPersonIndividualLast"]).Trim());
				payor.Participant.Person.Name.IndividualSuffix = ((string)reader["ParticipantPersonIndividualSuffix"]).Trim();
				payor.Participant.Person.EmailAddress = ((string)reader["ParticipantPersonEmailAddress"]).Trim();
				payor.Participant.PhoneNumber = (string)reader["ParticipantPhoneNumber"];
				payor.Participant.Address = new Address();
				payor.Participant.Address.Line1 = ((string)reader["ParticipantAddressLine1"]).Trim();
				payor.Participant.Address.Line2 = ((string)reader["ParticipantAddressLine2"]).Trim();
				payor.Participant.Address.Line3 = ((string)reader["ParticipantAddressLine3"]).Trim();
				payor.Participant.Address.City = CapitalizeFirstLetter(((string)reader["ParticipantAddressCity"]).Trim());
				payor.Participant.Address.StateAbbreviation = parseState((string)reader["ParticipantAddressState"]);
				payor.Participant.Address.ZipCode = (string)reader["ParticipantAddressZip"];
				payor.Participant.Address.ZipExtension = (string)reader["ParticipantAddressZipExtension"];
				payor.Participant.Address.BoxNumber = (string)reader["ParticipantAddressBoxNumber"];
				payor.Participant.Address.Country = parseCountry((string)reader["ParticipantAddressCountry"]);

				policy.Payors.Add(payor);
			}
		}
	}
}

static void addInsured(Policy policy, SqlConnection conn)
{
	using (SqlCommand cmd = new SqlCommand(@$"
    SELECT DISTINCT --Made DISTINCT because insureds may appear on mutiple benefits
		polc.POLICY_NUMBER AS PolicyNumber,
		muin.MULT_RELATE AS InsuredRelationhipToPrimary, --Not currently used in the model.
		CASE name.NAME_FORMAT_CODE WHEN 'B' THEN 'True' ELSE 'False' END AS ParticipantIsBusiness,
		name.NAME_ID NameID,
		name.NAME_BUSINESS AS ParticipantBusinessName, --Only relevant when IsBusiness
		name.BUSINESS_EMAIL_ADR AS ParticipantBusinessEmailAddress, --Only relevant when IsBusiness (Assumed: there are records in LifePro with both email address types.)
		name.SEX_CODE AS ParticipantPersonGender, --Only relevant when NOT IsBusiness
		name.DATE_OF_BIRTH AS ParticipantPersonDateOfBirth, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_PREFIX AS ParticipantPersonIndividualPrefix, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_FIRST AS ParticipantPersonIndividualFirst, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_MIDDLE AS ParticipantPersonIndividualMiddle, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_LAST AS ParticipantPersonIndividualLast, --Only relevant when NOT IsBusiness
		name.INDIVIDUAL_SUFFIX AS ParticipantPersonIndividualSuffix, --Only relevant when NOT IsBusiness
		name.PERSONAL_EMAIL_ADR AS ParticipantPersonEmailAddress, --Only relevant when NOT IsBusiness (Assumed: there are records in LifePro with both email address types.)
		nalk.TELE_NUM AS ParticipantPhoneNumber,
		addr.ADDR_LINE_1 AS ParticipantAddressLine1, 
		addr.ADDR_LINE_2 AS ParticipantAddressLine2,
		addr.ADDR_LINE_3 AS ParticipantAddressLine3, 
		addr.CITY AS ParticipantAddressCity,
		addr.STATE AS ParticipantAddressState,
		addr.ZIP AS ParticipantAddressZip, 
		addr.ZIP_EXTENSION AS ParticipantAddressZipExtension, 
		addr.BOX_NUMBER AS ParticipantAddressBoxNumber,
		addr.COUNTRY AS ParticipantAddressCountry
	FROM lifepro_r.PPOLC polc
	INNER JOIN lifepro_r.PRELA_RELATIONSHIP_MASTER rela
		ON polc.COMPANY_CODE + polc.POLICY_NUMBER = rela.IDENTIFYING_ALPHA
	LEFT JOIN lifepro_r.PPBEN_POLICY_BENEFITS pben
		ON polc.POLICY_NUMBER = pben.POLICY_NUMBER AND rela.BENEFIT_SEQ_NUMBER = pben.BENEFIT_SEQ
	INNER JOIN lifepro_r.PMUIN_MULTIPLE_INSUREDS muin
		ON pben.POLICY_NUMBER = muin.POLICY_NUMBER AND pben.BENEFIT_SEQ = muin.BENEFIT_SEQ AND rela.NAME_ID = muin.NAME_ID AND rela.RELATE_CODE = muin.RELATIONSHIP_CODE
	INNER JOIN lifepro_r.PNAME name
		ON rela.NAME_ID = name.NAME_ID
	INNER JOIN lifepro_r.PNALK nalk
		ON name.NAME_ID = nalk.NAME_ID
	INNER JOIN lifepro_r.PADDR addr
		ON nalk.ADDRESS_ID = addr.ADDRESS_ID AND nalk.ADDRESS_CODE = '' AND nalk.CANCEL_DATE = 0
	WHERE pben.BENEFIT_SEQ > 0 --Not sure what 0 represents, but it appears to drop when joined to PPBEN anyway.
	AND pben.BENEFIT_TYPE IN ('BA','OR','BF') --Why does one of my policies have an SU?
	AND polc.POLICY_NUMBER IN ('{policy.PolicyNumber}')
", conn))
	{
		using (SqlDataReader reader = cmd.ExecuteReader())
		{
			while (reader.Read())
			{
				Insured insured = new Insured();
				insured.Participant = new Participant();
				insured.Participant.IsBusiness = parseBoolean((string)reader["ParticipantIsBusiness"]);
				if (insured.Participant.IsBusiness)
				{
					insured.Participant.Business = new Business();
					insured.Participant.Business.Name = new Name();
					insured.Participant.Business.Name.BusinessName = (string)reader["ParticipantBusinessName"];
					insured.Participant.Business.EmailAddress = (string)reader["ParticipantBusinessEmailAddress"];
				}
				insured.RelationshipToPrimaryInsured = parseInsured((string)reader["InsuredRelationhipToPrimary"]);
				insured.Participant.Person = new Person();
				insured.Participant.Person.Gender = parseGender((string)reader["ParticipantPersonGender"]);
				insured.Participant.Person.DateOfBirth = parseDate(((Int32)reader["ParticipantPersonDateOfBirth"]).ToString());
				insured.Participant.Person.Name = new Name();
				insured.Participant.Person.Name.NameId = (Int32)reader["NameID"];
				insured.Participant.Person.Name.IndividualPrefix = ((string)reader["ParticipantPersonIndividualPrefix"]).Trim();
				insured.Participant.Person.Name.IndividualFirst = CapitalizeFirstLetter(((string)reader["ParticipantPersonIndividualFirst"]).Trim());
				insured.Participant.Person.Name.IndividualMiddle = ((string)reader["ParticipantPersonIndividualMiddle"]).Trim();
				insured.Participant.Person.Name.IndividualLast = CapitalizeFirstLetter(((string)reader["ParticipantPersonIndividualLast"]).Trim());
				insured.Participant.Person.Name.IndividualSuffix = ((string)reader["ParticipantPersonIndividualSuffix"]).Trim();
				insured.Participant.Person.EmailAddress = ((string)reader["ParticipantPersonEmailAddress"]).Trim();
				insured.Participant.Address = new Address();
				insured.Participant.Address.Line1 = ((string)reader["ParticipantAddressLine1"]).Trim();
				insured.Participant.Address.Line2 = ((string)reader["ParticipantAddressLine2"]).Trim();
				insured.Participant.Address.Line3 = ((string)reader["ParticipantAddressLine3"]).Trim();
				insured.Participant.Address.City = CapitalizeFirstLetter(((string)reader["ParticipantAddressCity"]).Trim());
				insured.Participant.Address.ZipCode = (string)reader["ParticipantAddressZip"];
				insured.Participant.Address.ZipExtension = (string)reader["ParticipantAddressZipExtension"];
				insured.Participant.Address.BoxNumber = (string)reader["ParticipantAddressBoxNumber"];
				insured.Participant.Address.Country = parseCountry((string)reader["ParticipantAddressCountry"]);

				policy.Insureds.Add(insured);
			}
		}
	}
}

static void addAgent(Policy policy, SqlConnection conn)
{
	using (SqlCommand cmd = new SqlCommand(@$"
    SELECT 
		c.POLICY_NUMBER AS PolicyNumber, 
		CASE cs.SERVICE_AGENT_IND WHEN 'X' Then 'True' ELSE 'False' END AS IsServicingAgent,
		cs.AGENT AS AgentId, 
		cs.MARKET_CODE AS MarketCode, 
		cs.AGENT_LEVEL AS Level
	FROM DataStore.lifepro_r.PCOMC_COMMISSION_CONTROL c
	INNER JOIN DataStore.lifepro_r.PCOMC_COMMISSION_CONTROL_TYPE_S cs
		ON c.COMC_ID = cs.COMC_ID AND c.RECORD_TYPE = 'S'
	WHERE c.POLICY_NUMBER = ('{policy.PolicyNumber}')
", conn))
	{
		using (SqlDataReader reader = cmd.ExecuteReader())
		{
			while (reader.Read())
			{
				Agent agent = new Agent();
				agent.AgentId = ((string)reader["AgentId"]).Trim();
				agent.MarketCode = ((string)reader["MarketCode"]).Trim();
				agent.IsServicingAgent = parseBoolean((string)reader["IsServicingAgent"]);
				agent.Level = (string)reader["Level"];
				
				policy.Agents.Add(agent);
			}
		}
	}
}

static PayorType parsePayor(string payor)
{
	switch (payor)
	{
		case "Primary":
			return PayorType.Primary;
		case "Additional":
			return PayorType.Additional;
		case "SecondaryAddressee":
			return PayorType.SecondaryAddressee;
	}
	return PayorType.Primary; //not sure what the defualt return should be
}

static bool parseBoolean(string value)
{
	switch (value)
	{
		case "False":
			return false;
		case "True":
			return true;
	}
	return false;
}

static Gender parseGender(string gender)
{
	switch (gender)
    {
		case "M":
			return Gender.Male;
		case "F":
			return Gender.Female;
    }
	return Gender.Male; //not sure what the defualt return should be
}

static OwnerType parseOwner(string owner)
{
	switch (owner)
    {
		case "Primary":
			return OwnerType.Primary;
		case "Contingent":
			return OwnerType.Contingent;
    }
	return OwnerType.Primary; //not sure what the defualt return should be
}

static InsuredRelationshipToPrimary parseInsured(string insured)
{
	switch (insured.Trim())
	{
		case "SPOUSE":
			return InsuredRelationshipToPrimary.Spouse;
		case "CHILD":
			return InsuredRelationshipToPrimary.Child;
		default:
			return InsuredRelationshipToPrimary.Self; //not sure what the defualt return should be
	}
}

static DateTime parseDate(string date)
{
	if (date.Length == 8)
    {
		int year = Int32.Parse(date.Substring(0, 4));
		int month = Int32.Parse(date.Substring(4, 2));
		int day = Int32.Parse(date.Substring(6, 2));
		DateOnly dateTime = new DateOnly(year, month, day);
		return dateTime.ToDateTime(TimeOnly.MinValue);
	}
    else
    {
		return new DateOnly(1971, 01, 01).ToDateTime(TimeOnly.MinValue); //not sure what the defualt return should be

	}
	
}

static CoverageType parseCoverageType(string coverageType)
{
	switch (coverageType)
    {
		case "Base":
			return CoverageType.Base;
		default:
			return CoverageType.Rider; //not sure what the defualt return should be
	}
}

static PolicyStatus parseStatus(string status, string reason)
{
	switch (status)
    {
		case "A":
			return PolicyStatus.Active;
		case "T":
			if (reason == "LP")
			{
				return PolicyStatus.Lapsed;
			}
			return PolicyStatus.Terminated;
		case "P":
			return PolicyStatus.Pending;
		case "S":
			if (reason == "LP")
            {
				return PolicyStatus.Lapsed;
            }
			return PolicyStatus.Active;
	}
	return PolicyStatus.Terminated; //not sure what the defualt return should be
}

static PolicyStatusReason parseReason(string status, string reason)
{
	switch (reason)
	{
		case "":
		case " ":
		case "  ":
			return PolicyStatusReason.None;
		case "RI":
			if (status == "A")
			{
				return PolicyStatusReason.Reinstated;
			}
			else if (status == "P")
			{
				return PolicyStatusReason.ReadyToIssue;
			}
			else
			{
				return PolicyStatusReason.Reissued;
			}

		case "RS":
			if (status == "A")
			{
				return PolicyStatusReason.Restored;
			}
			else
			{
				return PolicyStatusReason.Reserved;
			}

		case "AN":
			return PolicyStatusReason.AnniversaryProcessingError;
		case "BE":
			return PolicyStatusReason.BillingError;
		case "CP":
			return PolicyStatusReason.ConversionPending;
		case "DP":
			return PolicyStatusReason.DeathPending;
		case "HO":
			return PolicyStatusReason.HomeOfficeSuspended;
		case "LP":
			if (status == "S")
			{
				return PolicyStatusReason.LapsePending;
			}
			else
			{
				return PolicyStatusReason.Lapse;
			}

		case "MP":
			return PolicyStatusReason.ModeaversaryProcessingError;
		case "MX":
			return PolicyStatusReason.MonthaversaryProcessingError;
		case "PC":
			return PolicyStatusReason.PolicyChange;
		case "RP":
			if (status == "S")
			{
				return PolicyStatusReason.RenewalPending;
			}
			else
			{
				return PolicyStatusReason.Replaced;
			}

		case "SP":
			return PolicyStatusReason.SurrenderPending;
		case "WP":
			return PolicyStatusReason.WaiverPending;
		case "NI":
			return PolicyStatusReason.NotIssued;
		case "NT":
			return PolicyStatusReason.NotTaken;
		case "PO":
			return PolicyStatusReason.Postponed;
		case "SR":
			return PolicyStatusReason.Surrendered;
		case "WI":
			return PolicyStatusReason.Withdrawn;
		case "IC":
			return PolicyStatusReason.Incomplete;
		case "SM":
			return PolicyStatusReason.Submitted;
		case "CA":
			return PolicyStatusReason.Canceled;
		case "CE":
			return PolicyStatusReason.ETIConversion;
		case "CR":
			return PolicyStatusReason.RPIConversion;
		case "CV":
			return PolicyStatusReason.Conversion;
		case "DC":
			return PolicyStatusReason.DeathClaim;
		case "DE":
			return PolicyStatusReason.Declined;
		case "EB":
			return PolicyStatusReason.ExchangeBenefit;
		case "EX":
			return PolicyStatusReason.Expire;
		case "MA":
			return PolicyStatusReason.Matured;
		case "MM":
			return PolicyStatusReason.MPRtoMDVConversion;
		case "UC":
			return PolicyStatusReason.UnappliedCash;
		case "RE":
			return PolicyStatusReason.ReturnedEft;
		case "MB":
			return PolicyStatusReason.IncompleteMIB;
		case "IE":
			return PolicyStatusReason.Ineligible;
		case "DU":
			return PolicyStatusReason.DeclinedFullUnd;
		case "ER":
			return PolicyStatusReason.Error;
		case "PW":
			return PolicyStatusReason.WithdrawnPayor;
		case "DR":
			return PolicyStatusReason.DeclinedReApply;
		case "IA":
			return PolicyStatusReason.IneligibleAge;
		case "OW":
			return PolicyStatusReason.WithdrawnOwner;
		case "IM":
			return PolicyStatusReason.IneligibleMaxCV;
		case "ON":
			return PolicyStatusReason.NotTakenOwner;
		case "IH":
			return PolicyStatusReason.IneligibleHospital;
		case "RC":
			return PolicyStatusReason.ReturnedCheck;
		case "DO":
			return PolicyStatusReason.DeclinedCOCPositive;
		case "DN":
			return PolicyStatusReason.DeclinedNonreapply;
		case "ID":
			return PolicyStatusReason.IneligibleDrugQuestion;
		case "PN":
			return PolicyStatusReason.NotTakenPayor;
		case "RW":
			return PolicyStatusReason.IncompleteRewrite;
		case "NR":
			return PolicyStatusReason.NotTakenRating;
		default:
			return PolicyStatusReason.Unknown;
	}
}

static string CapitalizeFirstLetter(string str)
{
	if (str.Length > 0)
	{
		return char.ToUpper(str[0]) + str.Substring(1).ToLower();
	}
	return str;
}

static State parseState(string state)
{
	switch (state)
    {
		case "AK":
			return State.AK;
		case "AL":
			return State.AL;
		case "AR":
			return State.AR;
		case "AZ":
			return State.AZ;
		case "CA":
			return State.CA;
		case "CO":
			return State.CO;
		case "CT":
			return State.CT;
		case "DC":
			return State.DC;
		case "DE":
			return State.DE;
		case "FL":
			return State.FL;
		case "GA":
			return State.GA;
		case "HI":
			return State.HI;
		case "IA":
			return State.IA;
		case "ID":
			return State.ID;
		case "IL":
			return State.IL;
		case "IN":
			return State.IN;
		case "KS":
			return State.KS;
		case "KY":
			return State.KY;
		case "LA":
			return State.LA;
		case "MA":
			return State.MA;
		case "MD":
			return State.MD;
		case "MO":
			return State.MO;
		case "MS":
			return State.MS;
		case "MT":
			return State.MT;
		case "NC":
			return State.NC;
		case "ND":
			return State.ND;
		case "NE":
			return State.NE;
		case "NH":
			return State.NH;
		case "NJ":
			return State.NJ;
		case "NM":
			return State.NM;
		case "NV":
			return State.NV;
		case "NY":
			return State.NY;
		case "OH":
			return State.OH;
		case "OK":
			return State.OK;
		case "OR":
			return State.OR;
		case "PA":
			return State.PA;
		case "PR":
			return State.PR;
		case "RI":
			return State.RI;
		case "SC":
			return State.SC;
		case "SD":
			return State.SD;
		case "TN":
			return State.TN;
		case "TX":
			return State.TX;
		case "UT":
			return State.UT;
		case "VA":
			return State.VA;
		case "VI":
			return State.VI;
		case "VT":
			return State.VT;
		case "WA":
			return State.WA;
		case "WI":
			return State.WI;
		case "WV":
			return State.WV;
		case "WY":
			return State.WY;
	}
	return State.NE; //not sure what the defualt return should be
}

static Country parseCountry(string country) //there is definetly more work to be done here
{
	switch (country)
	{
		default:
			return Country.USA;
	}
}