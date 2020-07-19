using System;
using System.Collections.Generic;
using System.Text;

namespace CreditCardApplications
{
    public class CreditCardApplicationEvaluator
    {
        private const int AutoReferralMaxAge = 20;
        private const int HighIncomeThreshold = 100_000;
        private const int LowIncomeThreshold = 20_000;

        private readonly IFrequentFlyerNumberValidator _validator;
        private readonly FraudLookup _fraudLookup;

        public int ValidatorLookupCount { get; private set; }
        public CreditCardApplicationEvaluator(IFrequentFlyerNumberValidator validator, FraudLookup fraudLookup = null)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _validator.ValidatorLookupPerformed += ValidatorLookupPerformed;
            _fraudLookup = fraudLookup;
        }

        private void ValidatorLookupPerformed(object sender, EventArgs e)
        {
            ValidatorLookupCount++;
        }

        public CreditCardApplicationDecision Evaluate(CreditCardApplication application)
        {
            if (_fraudLookup != null && _fraudLookup.IsFraudRisk(application))
            {
                return CreditCardApplicationDecision.ReferredToHumanFraudRisk;
            }

            if (application.GrossAnnualIncome >= HighIncomeThreshold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            //-------------------------------------
            //if (_validator.LicenseKey == "EXPIRED")
            if (_validator.ServiceInformation.License.LicenseKey == "EXPIRED")
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }
            //---this has dependency from external service----------
            _validator.ValidationMode = application.Age >= 30 ? ValidationMode.Detailed : ValidationMode.Quick;


            //var isValidFrequentFlyerNumber = _validator.IsValid(application.FrequentFlyerNumber);//since the validator object is mocked.the bool value will always be false until we setup the return value uisng mock

            bool isValidFrequentFlyerNumber;
            try//for checking concept of THROWING EXCEPTION FROM MOCK OBJECTS
            {
                isValidFrequentFlyerNumber = _validator.IsValid(application.FrequentFlyerNumber);
            }
            catch
            {
                //log
                return CreditCardApplicationDecision.ReferredToHuman;
            }



            if (!isValidFrequentFlyerNumber)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            
            //--------------------------------------------------------

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            

            return CreditCardApplicationDecision.ReferredToHuman;
        }


        /*//FOR SETTING OUT PROPERTY
        public CreditCardApplicationDecision EvaluateUsingOut(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            _validator.IsValid(application.FrequentFlyerNumber,
                               out var isValidFrequentFlyerNumber);

            if (!isValidFrequentFlyerNumber)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

            return CreditCardApplicationDecision.ReferredToHuman;
        }*/
    }
}
